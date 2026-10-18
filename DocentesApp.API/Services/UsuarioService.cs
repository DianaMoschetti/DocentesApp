using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Data.Auditoria;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Enums;
using DocentesApp.Data.Identity;
using DocentesApp.Shared.Auth;
using DocentesApp.Shared.DTOs.Auth;
using DocentesApp.Shared.DTOs.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.API.Services
{
    /// <summary>
    /// Gestión de usuarios. Los usuarios no se borran: se desactivan (se conserva el historial).
    /// Reglas: nadie se desactiva ni se cambia el rol a sí mismo, y siempre queda al menos un Administrador activo.
    /// Desactivar, cambiar el rol y cambiar/resetear la contraseña cambian el security stamp:
    /// los tokens ya emitidos dejan de valer (ValidacionTokenUsuario).
    /// Los errores son AppException: el middleware los devuelve como ProblemDetails.
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DocentesDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IAuditoriaEventos _auditoria;

        // Auditoría: los cambios de columnas (alta, Nombre, Apellido, Activo) los registra el interceptor;
        // el rol y la contraseña no son columnas de AspNetUsers (o se ignoran): se registran como eventos explícitos,
        // dentro de la misma transacción que el cambio (mismo OperacionId).
        private const string EntidadUsuario = "Usuario";
        private const string PropiedadRol = "Rol";
        private const string PropiedadContrasenia = "Contraseña"; // sin valores: nunca se guarda la contraseña ni el hash

        public UsuarioService(UserManager<ApplicationUser> userManager, DocentesDbContext context, ITokenService tokenService, IAuditoriaEventos auditoria)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _auditoria = auditoria;
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
        {
            var usuarios = await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre).ThenBy(u => u.UserName)
                .ToListAsync();

            // Un solo query para los roles de todos (en vez de GetRolesAsync por usuario)
            var roles = await (
                    from ur in _context.UserRoles
                    join r in _context.Roles on ur.RoleId equals r.Id
                    select new { ur.UserId, r.Name })
                .ToListAsync();
            var rolPorUsuario = roles
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Name).First()!);

            return usuarios.Select(u => ToDto(u, rolPorUsuario.GetValueOrDefault(u.Id) ?? string.Empty)).ToList();
        }

        public async Task<UsuarioDto> GetByIdAsync(string id)
        {
            var user = await BuscarAsync(id);
            return ToDto(user, await RolDeAsync(user));
        }

        public async Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto)
        {
            ValidarRol(dto.Rol);

            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                throw new ConflictException($"Ya existe un usuario con el email {dto.Email}.");

            if (await _userManager.FindByNameAsync(dto.UserName) != null)
                throw new ConflictException($"Ya existe un usuario con el nombre de usuario {dto.UserName}.");

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.UserName,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                EmailConfirmed = true,
                Activo = true
            };

            // Usuario y rol juntos: si falla el rol no queda un usuario sin rol
            await using var transaction = await _context.Database.BeginTransactionAsync();

            Verificar(await _userManager.CreateAsync(user, dto.Password));
            Verificar(await _userManager.AddToRoleAsync(user, dto.Rol));
            await RegistrarEventoAsync(user, PropiedadRol, null, dto.Rol); // el alta del usuario la registra el interceptor

            await transaction.CommitAsync();

            return ToDto(user, dto.Rol);
        }

        public async Task UpdateAsync(string id, UpdateUsuarioDto dto)
        {
            var user = await BuscarAsync(id);

            user.Nombre = dto.Nombre;
            user.Apellido = dto.Apellido;

            Verificar(await _userManager.UpdateAsync(user));
        }

        public async Task CambiarRolAsync(string id, string rol, string actorId)
        {
            ValidarRol(rol);

            if (id == actorId)
                throw new ConflictException("No podés cambiar tu propio rol.");

            var user = await BuscarAsync(id);
            var rolesActuales = await _userManager.GetRolesAsync(user);

            if (rolesActuales.Count == 1 && rolesActuales[0] == rol)
                return; // ya tiene ese rol: no se invalida su sesión

            if (rolesActuales.Contains(Roles.Administrador) && rol != Roles.Administrador)
                await ValidarQueQuedeOtroAdministradorActivoAsync(user);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (rolesActuales.Count > 0)
                Verificar(await _userManager.RemoveFromRolesAsync(user, rolesActuales));
            Verificar(await _userManager.AddToRoleAsync(user, rol));
            // El token actual tiene el rol viejo: se corta y tiene que volver a loguearse
            Verificar(await _userManager.UpdateSecurityStampAsync(user));
            await RegistrarEventoAsync(user, PropiedadRol, string.Join(", ", rolesActuales), rol);

            await transaction.CommitAsync();
        }

        public async Task DesactivarAsync(string id, string actorId)
        {
            if (id == actorId)
                throw new ConflictException("No podés desactivarte a vos mismo.");

            var user = await BuscarAsync(id);
            if (!user.Activo)
                return;

            if (await _userManager.IsInRoleAsync(user, Roles.Administrador))
                await ValidarQueQuedeOtroAdministradorActivoAsync(user);

            user.Activo = false;
            // UpdateSecurityStampAsync guarda también Activo y corta los tokens ya emitidos
            Verificar(await _userManager.UpdateSecurityStampAsync(user));
        }

        public async Task ReactivarAsync(string id)
        {
            var user = await BuscarAsync(id);
            if (user.Activo)
                return;

            user.Activo = true;
            Verificar(await _userManager.UpdateAsync(user));
        }

        public async Task ResetPasswordAsync(string id, string passwordNueva)
        {
            var user = await BuscarAsync(id);

            // Contraseña y evento de auditoría juntos
            await using var transaction = await _context.Database.BeginTransactionAsync();

            // ResetPasswordAsync valida las reglas de contraseña y cambia el security stamp
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            Verificar(await _userManager.ResetPasswordAsync(user, token, passwordNueva));
            await RegistrarEventoAsync(user, PropiedadContrasenia, null, null);

            await transaction.CommitAsync();
        }

        public async Task<AuthResponseDto> CambiarMiPasswordAsync(string actorId, CambiarMiPasswordDto dto)
        {
            var user = await BuscarAsync(actorId);

            // Contraseña y evento de auditoría juntos (el autor del evento es el propio usuario, por su token)
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var result = await _userManager.ChangePasswordAsync(user, dto.PasswordActual, dto.PasswordNueva);
            if (!result.Succeeded && result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.PasswordMismatch)))
                throw new BadRequestException("La contraseña actual no es correcta.");
            Verificar(result);
            await RegistrarEventoAsync(user, PropiedadContrasenia, null, null);

            await transaction.CommitAsync();

            // ChangePasswordAsync cambió el stamp: se devuelve un token nuevo para seguir con la sesión
            return await _tokenService.CreateTokenAsync(user);
        }

        // Defensa extra: con las reglas "no a uno mismo" el que opera es un admin activo que queda,
        // pero esto cubre requests simultáneos y cambios hechos a mano en la base.
        private async Task ValidarQueQuedeOtroAdministradorActivoAsync(ApplicationUser user)
        {
            var administradores = await _userManager.GetUsersInRoleAsync(Roles.Administrador);
            if (!administradores.Any(a => a.Activo && a.Id != user.Id))
                throw new ConflictException("Tiene que quedar al menos un Administrador activo.");
        }

        // Evento explícito sobre el usuario (Modificación de una "propiedad" que no es una columna auditada)
        private Task RegistrarEventoAsync(ApplicationUser user, string propiedad, string? anterior, string? nuevo) =>
            _auditoria.RegistrarAsync(
                EntidadUsuario,
                user.Id,
                ReglasAuditoria.Obtener(typeof(ApplicationUser))?.Descripcion(user),
                AccionAuditoria.Modificacion,
                new[] { new DetalleEventoAuditoria(propiedad, anterior, nuevo) });

        private async Task<ApplicationUser> BuscarAsync(string id) =>
            await _userManager.FindByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el usuario con ID {id}.");

        private async Task<string> RolDeAsync(ApplicationUser user) =>
            (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;

        private static void ValidarRol(string rol)
        {
            if (!Roles.EsValido(rol))
                throw new BadRequestException($"El rol debe ser uno de: {string.Join(", ", Roles.Todos)}.");
        }

        // Errores de Identity (contraseña débil, etc.) -> 400 con las descripciones
        private static void Verificar(IdentityResult result)
        {
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }

        private static UsuarioDto ToDto(ApplicationUser user, string rol) => new()
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Nombre = user.Nombre,
            Apellido = user.Apellido,
            Rol = rol,
            Activo = user.Activo
        };
    }
}
