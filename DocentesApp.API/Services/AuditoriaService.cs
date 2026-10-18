using System.Globalization;
using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Common.Formatos;
using DocentesApp.Data.Auditoria;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Extensions;
using DocentesApp.Shared;
using DocentesApp.Shared.DTOs.Auditoria;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DocentesApp.API.Services
{
    // Lectura del historial de cambios. Las filas guardan valores crudos (FKs como id, enums como número,
    // serialización invariante); acá se traducen para mostrar:
    //   - entidades y propiedades con etiquetas en castellano (lo desconocido, con su nombre crudo)
    //   - FKs por nombre, una consulta por tipo de principal y por página; si el principal ya no existe, "#id"
    //   - enums con Descripcion(), booleanos Sí/No, fechas dd/MM/yyyy (0001-01-01 nunca se muestra como fecha)
    // El tipo de cada propiedad sale del modelo de EF (entidad registrada en ReglasAuditoria).
    public class AuditoriaService : IAuditoriaService
    {
        private const string Sistema = "Sistema";
        private static readonly CultureInfo EsAr = CultureInfo.GetCultureInfo("es-AR");

        private readonly DocentesDbContext _context;

        public AuditoriaService(DocentesDbContext context)
        {
            _context = context;
        }

        public Task<PaginaDto<HistorialCambioDto>> BuscarAsync(FiltroAuditoriaDto filtro)
        {
            var tamanio = ValidarPaginado(filtro.Pagina, filtro.TamanioPagina);
            if (filtro.Desde.HasValue && filtro.Hasta.HasValue && filtro.Desde > filtro.Hasta)
                throw new BadRequestException("La fecha desde no puede ser posterior a la fecha hasta.");

            var query = _context.HistorialCambios.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filtro.UsuarioId))
                query = string.Equals(filtro.UsuarioId, FiltroAuditoriaDto.UsuarioSistema, StringComparison.OrdinalIgnoreCase)
                    ? query.Where(h => h.UsuarioId == null)
                    : query.Where(h => h.UsuarioId == filtro.UsuarioId);

            // Días de Argentina -> rango UTC; hasta inclusive (< 00:00 del día siguiente)
            if (filtro.Desde.HasValue)
            {
                var desdeUtc = ZonaHorariaArgentina.InicioDelDiaUtc(filtro.Desde.Value);
                query = query.Where(h => h.FechaUtc >= desdeUtc);
            }
            if (filtro.Hasta.HasValue)
            {
                var hastaUtc = ZonaHorariaArgentina.InicioDelDiaUtc(filtro.Hasta.Value.AddDays(1));
                query = query.Where(h => h.FechaUtc < hastaUtc);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Entidad))
                query = query.Where(h => h.Entidad == filtro.Entidad);

            if (filtro.Accion.HasValue)
                query = query.Where(h => h.Accion == filtro.Accion.Value);

            return PaginarAsync(query, filtro.Pagina, tamanio);
        }

        // Historial de un docente: sus datos, sus membresías, sus designaciones y los cargos y actividades de ellas
        public Task<PaginaDto<HistorialCambioDto>> PorDocenteAsync(int docenteId, int pagina, int tamanioPagina)
        {
            var tamanio = ValidarPaginado(pagina, tamanioPagina);
            return PaginarAsync(_context.HistorialCambios.AsNoTracking().Where(h => h.DocenteId == docenteId), pagina, tamanio);
        }

        // Historial de una designación: la cabecera, sus cargos y sus actividades
        public Task<PaginaDto<HistorialCambioDto>> PorDesignacionAsync(int designacionId, int pagina, int tamanioPagina)
        {
            var tamanio = ValidarPaginado(pagina, tamanioPagina);
            return PaginarAsync(_context.HistorialCambios.AsNoTracking().Where(h => h.DesignacionId == designacionId), pagina, tamanio);
        }

        public IEnumerable<EntidadAuditoriaDto> Entidades() =>
            ReglasAuditoria.Entidades
                .Select(e => new EntidadAuditoriaDto { Entidad = e, Nombre = EtiquetasAuditoria.Entidad(e) })
                .OrderBy(e => e.Nombre, StringComparer.Create(EsAr, ignoreCase: true))
                .ToList();

        // pagina >= 1; tamanioPagina >= 1, más de 200 se recorta a 200. Devuelve el tamaño a usar.
        private static int ValidarPaginado(int pagina, int tamanioPagina)
        {
            if (pagina < 1)
                throw new BadRequestException("La página tiene que ser 1 o mayor.");
            if (tamanioPagina < 1)
                throw new BadRequestException("El tamaño de página tiene que ser 1 o mayor.");
            return Math.Min(tamanioPagina, FiltroAuditoriaDto.TamanioPaginaMaximo);
        }

        private async Task<PaginaDto<HistorialCambioDto>> PaginarAsync(IQueryable<HistorialCambio> query, int pagina, int tamanio)
        {
            var total = await query.CountAsync();

            // Más nuevo primero (Id desempata filas de la misma operación)
            var filas = await query
                .OrderByDescending(h => h.FechaUtc).ThenByDescending(h => h.Id)
                .Skip((pagina - 1) * tamanio).Take(tamanio)
                .Include(h => h.Detalles.OrderBy(d => d.Id))
                .ToListAsync();

            var nombres = await ResolverForaneasAsync(filas);

            return new PaginaDto<HistorialCambioDto>
            {
                Items = filas.Select(f => ToDto(f, nombres)).ToList(),
                Total = total,
                Pagina = pagina,
                TamanioPagina = tamanio
            };
        }

        #region Valores

        // Propiedad del modelo de EF para una fila (null si la entidad no está registrada o la propiedad no existe, ej. "Rol" de Usuario)
        private IProperty? PropiedadDelModelo(string entidad, string propiedad)
        {
            var tipo = ReglasAuditoria.TipoDeEntidad(entidad);
            return tipo == null ? null : _context.Model.FindEntityType(tipo)?.FindProperty(propiedad);
        }

        // Tipo del principal si la propiedad es una FK simple (DocenteId -> Docente, ReemplazaACargoId -> CargoDesignacion, ...)
        private static Type? PrincipalDe(IProperty? propiedad) =>
            propiedad?.GetContainingForeignKeys().FirstOrDefault(fk => fk.Properties.Count == 1)?.PrincipalEntityType.ClrType;

        // Nombres de todas las FKs de la página: una consulta por tipo de principal
        private async Task<Dictionary<(Type Tipo, int Id), string>> ResolverForaneasAsync(List<HistorialCambio> filas)
        {
            var idsPorTipo = new Dictionary<Type, HashSet<int>>();
            foreach (var fila in filas)
                foreach (var detalle in fila.Detalles)
                {
                    var principal = PrincipalDe(PropiedadDelModelo(fila.Entidad, detalle.Propiedad));
                    if (principal == null)
                        continue;
                    foreach (var valor in new[] { detalle.ValorAnterior, detalle.ValorNuevo })
                        if (int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                        {
                            if (!idsPorTipo.TryGetValue(principal, out var ids))
                                idsPorTipo[principal] = ids = new HashSet<int>();
                            ids.Add(id);
                        }
                }

            var nombres = new Dictionary<(Type, int), string>();
            foreach (var (tipo, ids) in idsPorTipo)
                foreach (var (id, nombre) in await NombresAsync(tipo, ids))
                    nombres[(tipo, id)] = nombre;
            return nombres;
        }

        // Tipos de principal que se muestran por nombre. Otros: el id crudo.
        private async Task<IEnumerable<(int Id, string Nombre)>> NombresAsync(Type tipo, HashSet<int> ids)
        {
            if (tipo == typeof(Docente))
                return (await _context.Docentes.AsNoTracking().Where(d => ids.Contains(d.Id))
                        .Select(d => new { d.Id, d.Apellido, d.Nombre }).ToListAsync())
                    .Select(d => (d.Id, $"{d.Apellido}, {d.Nombre}"));

            if (tipo == typeof(Udb))
                return (await _context.Udbs.AsNoTracking().Where(u => ids.Contains(u.Id))
                        .Select(u => new { u.Id, u.Nombre }).ToListAsync())
                    .Select(u => (u.Id, u.Nombre));

            if (tipo == typeof(Asignatura))
                return (await _context.Asignaturas.AsNoTracking().Where(a => ids.Contains(a.Id))
                        .Select(a => new { a.Id, a.Nombre }).ToListAsync())
                    .Select(a => (a.Id, a.Nombre));

            if (tipo == typeof(Curso))
                return (await _context.Cursos.AsNoTracking().Where(c => ids.Contains(c.Id)).ToListAsync())
                    .Select(c => (c.Id, TextosDesignacion.Curso(c.Año, c.Carrera, c.NroComision, c.Turno)));

            if (tipo == typeof(CargoDesignacion))
                return (await _context.CargosDesignacion.AsNoTracking().Where(c => ids.Contains(c.Id))
                        .Select(c => new { c.Id, c.Cargo, c.CantidadDedicacion, c.TipoDedicacion, c.Designacion.Docente.Apellido, c.Designacion.Docente.Nombre })
                        .ToListAsync())
                    .Select(c => (c.Id, TextosDesignacion.CargoConDocente(c.Apellido, c.Nombre, c.Cargo, c.CantidadDedicacion, c.TipoDedicacion)));

            if (tipo == typeof(Designacion))
                return (await _context.Designaciones.AsNoTracking().Where(d => ids.Contains(d.Id))
                        .Select(d => new { d.Id, d.FechaInicio, d.Docente.Apellido, d.Docente.Nombre }).ToListAsync())
                    .Select(d => (d.Id, $"{d.Apellido}, {d.Nombre} – desde {d.FechaInicio.ToString("dd/MM/yyyy", EsAr)}"));

            return Enumerable.Empty<(int, string)>();
        }

        private static readonly HashSet<Type> PrincipalesConNombre = new()
        {
            typeof(Docente), typeof(Udb), typeof(Asignatura), typeof(Curso), typeof(CargoDesignacion), typeof(Designacion)
        };

        private string? Mostrar(HistorialCambio fila, string propiedad, string? valor, Dictionary<(Type, int), string> nombres)
        {
            if (valor == null)
                return null;

            var prop = PropiedadDelModelo(fila.Entidad, propiedad);
            var principal = PrincipalDe(prop);
            if (principal != null && PrincipalesConNombre.Contains(principal)
                && int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                return nombres.GetValueOrDefault((principal, id)) ?? $"#{id}"; // borrado: "#id"

            return Formatear(prop?.ClrType, valor);
        }

        // Valor crudo (invariante) -> texto para mostrar, según el tipo de la propiedad
        public static string? Formatear(Type? tipo, string valor)
        {
            var t = tipo == null ? null : Nullable.GetUnderlyingType(tipo) ?? tipo;
            if (t == null)
                return valor;

            if (t.IsEnum)
                return long.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) && Enum.IsDefined(t, Enum.ToObject(t, n))
                    ? ((Enum)Enum.ToObject(t, n)).Descripcion()
                    : valor;

            if (t == typeof(bool))
                return valor switch { "true" => "Sí", "false" => "No", _ => valor };

            if (t == typeof(DateTime) && DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var fecha))
                return fecha.Year == 1 ? null // 0001-01-01: sin fecha (columnas de auditoría de filas anteriores)
                    : fecha.TimeOfDay == TimeSpan.Zero ? fecha.ToString("dd/MM/yyyy", EsAr) : fecha.ToString("dd/MM/yyyy HH:mm", EsAr);

            if (t == typeof(DateTimeOffset) && DateTimeOffset.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaHora))
                return fechaHora.Year == 1 ? null
                    : ZonaHorariaArgentina.DesdeUtc(fechaHora.UtcDateTime).ToString("dd/MM/yyyy HH:mm", EsAr);

            if (t == typeof(DateOnly) && DateOnly.TryParse(valor, CultureInfo.InvariantCulture, out var dia))
                return dia.Year == 1 ? null : dia.ToString("dd/MM/yyyy", EsAr);

            if ((t == typeof(decimal) || t == typeof(double) || t == typeof(float))
                && decimal.TryParse(valor, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var numero))
                return numero.ToString("0.##", EsAr);

            return valor;
        }

        #endregion

        private HistorialCambioDto ToDto(HistorialCambio fila, Dictionary<(Type, int), string> nombres) => new()
        {
            Id = fila.Id,
            FechaUtc = DateTime.SpecifyKind(fila.FechaUtc, DateTimeKind.Utc),
            FechaArgentina = ZonaHorariaArgentina.DesdeUtc(fila.FechaUtc),
            OperacionId = fila.OperacionId,
            UsuarioId = fila.UsuarioId,
            UsuarioNombre = fila.UsuarioId == null ? Sistema : fila.UsuarioNombre ?? fila.UsuarioEmail ?? fila.UsuarioId,
            UsuarioEmail = fila.UsuarioEmail,
            Entidad = fila.Entidad,
            EntidadNombre = EtiquetasAuditoria.Entidad(fila.Entidad),
            EntidadId = fila.EntidadId,
            EntidadDescripcion = fila.EntidadDescripcion,
            Accion = fila.Accion,
            AccionNombre = fila.Accion.Descripcion(),
            DocenteId = fila.DocenteId,
            DesignacionId = fila.DesignacionId,
            Detalles = fila.Detalles.Select(d => new HistorialCambioDetalleDto
            {
                Propiedad = EtiquetasAuditoria.Propiedad(d.Propiedad),
                Anterior = Mostrar(fila, d.Propiedad, d.ValorAnterior, nombres),
                Nuevo = Mostrar(fila, d.Propiedad, d.ValorNuevo, nombres)
            }).ToList()
        };
    }
}
