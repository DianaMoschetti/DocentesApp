using DocentesApp.Application.Common.Formatos;
using DocentesApp.Data.Identity;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DocentesApp.Data.Auditoria
{
    // Cómo se audita un tipo: etiqueta de la entidad, descripción (copia al momento del cambio) y vínculo con docente/designación.
    public sealed record ReglaAuditoria(
        string Entidad,
        Func<object, string?> Descripcion,
        Func<VinculoContexto, ValueTask<(int? DocenteId, int? DesignacionId)>>? Vinculo);

    // Registro único Tipo -> regla. Para auditar y vincular un tipo nuevo (ej. Resoluciones, Movimientos) alcanza
    // con agregar una entrada acá (ej. Movimiento -> su DesignacionId -> DocenteDeDesignacionAsync).
    // Los tipos no registrados se auditan igual, con el nombre del tipo y sin vínculo.
    public static class ReglasAuditoria
    {
        private static readonly (int? DocenteId, int? DesignacionId) SinVinculo = (null, null);

#pragma warning disable CS0618 // Cargo y Dedicacion son obsoletas pero siguen teniendo endpoints de escritura: se auditan
        private static readonly Dictionary<Type, ReglaAuditoria> Reglas = new()
        {
            [typeof(Docente)] = Regla<Docente>("Docente",
                d => $"{d.Apellido}, {d.Nombre}",
                v => new((v.Entero(nameof(Docente.Id)), null))),

            [typeof(Designacion)] = Regla<Designacion>("Designacion",
                DescripcionDesignacion,
                v => new((v.Entero(nameof(Designacion.DocenteId)), v.Entero(nameof(Designacion.Id))))),

            [typeof(CargoDesignacion)] = Regla<CargoDesignacion>("CargoDesignacion",
                c => TextosDesignacion.CargoCorto(c.Cargo, c.CantidadDedicacion, c.TipoDedicacion),
                async v =>
                {
                    var designacionId = v.Entero(nameof(CargoDesignacion.DesignacionId));
                    return (await v.DocenteDeDesignacionAsync(designacionId), designacionId);
                }),

            [typeof(Actividad)] = Regla<Actividad>("Actividad",
                a => a.Asignatura != null ? $"{a.Especificacion.Descripcion()} – {a.Asignatura.Nombre}" : a.Especificacion.Descripcion(),
                async v =>
                {
                    var designacionId = await v.DesignacionDeCargoAsync(v.Entero(nameof(Actividad.CargoDesignacionId)));
                    return (await v.DocenteDeDesignacionAsync(designacionId), designacionId);
                }),

            [typeof(DocenteUdb)] = Regla<DocenteUdb>("DocenteUdb",
                DescripcionDocenteUdb,
                v => new((v.Entero(nameof(DocenteUdb.DocenteId)), null))),

            // Catálogos y usuarios: sin vínculo con docente/designación
            [typeof(Udb)] = Regla<Udb>("Udb", u => u.Nombre),
            [typeof(Asignatura)] = Regla<Asignatura>("Asignatura", a => a.Nombre),
            [typeof(Curso)] = Regla<Curso>("Curso", c => TextosDesignacion.Curso(c.Año, c.Carrera, c.NroComision, c.Turno)),
            [typeof(AsignaturaModulo)] = Regla<AsignaturaModulo>("AsignaturaModulo",
                am => am.Asignatura != null ? $"{am.Asignatura.Nombre} – {am.Modulo}" : am.Modulo.ToString()),
            [typeof(PuntosPorCargo)] = Regla<PuntosPorCargo>("PuntosPorCargo", p => p.Cargo.Descripcion()),
            [typeof(Laboratorio)] = Regla<Laboratorio>("Laboratorio", l => l.Nombre),
            [typeof(Cargo)] = Regla<Cargo>("Cargo", c => $"{c.Denominacion.Descripcion()} {c.TipoCargo.Descripcion()}"),
            [typeof(Dedicacion)] = Regla<Dedicacion>("Dedicacion", d => $"{d.DescTipo.Descripcion()} {d.CantidadDedicacion}"),
            [typeof(ApplicationUser)] = Regla<ApplicationUser>("Usuario", DescripcionUsuario),
        };
#pragma warning restore CS0618

        public static ReglaAuditoria? Obtener(Type tipo) => Reglas.GetValueOrDefault(tipo);

        // Al leer el historial: la clave guardada en HistorialCambio.Entidad -> tipo (null si no está registrada)
        public static Type? TipoDeEntidad(string entidad) =>
            Reglas.FirstOrDefault(kv => kv.Value.Entidad == entidad).Key;

        // Claves de entidad registradas (para el filtro de la pantalla de auditoría)
        public static IEnumerable<string> Entidades => Reglas.Values.Select(r => r.Entidad);

        private static ReglaAuditoria Regla<T>(
            string entidad,
            Func<T, string?> descripcion,
            Func<VinculoContexto, ValueTask<(int? DocenteId, int? DesignacionId)>>? vinculo = null) =>
            new(entidad, o => descripcion((T)o), vinculo ?? (_ => new(SinVinculo)));

        // Las navegaciones pueden no estar cargadas: se usan solo si están
        private static string DescripcionDesignacion(Designacion d)
        {
            var texto = $"Desde {d.FechaInicio:dd/MM/yyyy}";
            if (!string.IsNullOrWhiteSpace(d.NroResolucion))
                texto += $" – Res. {d.NroResolucion}";
            return d.Docente is { } docente ? $"{docente.Apellido}, {docente.Nombre} – {texto}" : texto;
        }

        private static string DescripcionDocenteUdb(DocenteUdb du)
        {
            var partes = new List<string>();
            if (du.Docente is { } docente)
                partes.Add($"{docente.Apellido}, {docente.Nombre}");
            if (du.Udb is { } udb)
                partes.Add(udb.Nombre);
            partes.Add(du.Rol.Descripcion());
            return string.Join(" – ", partes);
        }

        private static string DescripcionUsuario(ApplicationUser u)
        {
            var nombre = $"{u.Apellido}, {u.Nombre}".Trim(' ', ',');
            var usuario = u.UserName ?? u.Email;
            return nombre.Length > 0 ? $"{nombre} ({usuario})" : usuario ?? string.Empty;
        }
    }

    // Lo que una regla puede consultar para vincular una fila: los valores de la entrada (originales para
    // Modified/Deleted, actuales para Added) y helpers que buscan primero en lo trackeado y si no en la base,
    // con caché por guardado.
    public sealed class VinculoContexto
    {
        private readonly ResolutorVinculos _resolutor;
        private readonly EntityEntry _entry;
        private readonly bool _usarOriginales;

        internal VinculoContexto(ResolutorVinculos resolutor, EntityEntry entry, bool usarOriginales)
        {
            _resolutor = resolutor;
            _entry = entry;
            _usarOriginales = usarOriginales;
        }

        public object? Valor(string propiedad)
        {
            var p = _entry.Property(propiedad);
            return _usarOriginales ? p.OriginalValue : p.CurrentValue;
        }

        public int? Entero(string propiedad) => Valor(propiedad) is int valor ? valor : null;

        public ValueTask<int?> DocenteDeDesignacionAsync(int? designacionId) => _resolutor.DocenteDeDesignacionAsync(designacionId);

        public ValueTask<int?> DesignacionDeCargoAsync(int? cargoId) => _resolutor.DesignacionDeCargoAsync(cargoId);
    }

    // Caché y consultas de vínculos para un guardado. Con async = false todo es sincrónico (camino SaveChanges()).
    internal sealed class ResolutorVinculos
    {
        private readonly DbContext _context;
        private readonly bool _async;
        private readonly CancellationToken _cancellationToken;
        private readonly Dictionary<int, int?> _docentePorDesignacion = new();
        private readonly Dictionary<int, int?> _designacionPorCargo = new();

        public ResolutorVinculos(DbContext context, bool async, CancellationToken cancellationToken)
        {
            _context = context;
            _async = async;
            _cancellationToken = cancellationToken;
        }

        public async ValueTask<int?> DocenteDeDesignacionAsync(int? designacionId)
        {
            if (designacionId is not int id)
                return null;
            if (_docentePorDesignacion.TryGetValue(id, out var cacheado))
                return cacheado;

            var trackeada = _context.ChangeTracker.Entries<Designacion>().FirstOrDefault(e => e.Entity.Id == id);
            int? docenteId = trackeada != null
                ? ValorTrackeado<int>(trackeada, nameof(Designacion.DocenteId))
                : await PrimeroAsync(_context.Set<Designacion>().AsNoTracking()
                    .Where(d => d.Id == id).Select(d => (int?)d.DocenteId));

            _docentePorDesignacion[id] = docenteId;
            return docenteId;
        }

        public async ValueTask<int?> DesignacionDeCargoAsync(int? cargoId)
        {
            if (cargoId is not int id)
                return null;
            if (_designacionPorCargo.TryGetValue(id, out var cacheado))
                return cacheado;

            var trackeado = _context.ChangeTracker.Entries<CargoDesignacion>().FirstOrDefault(e => e.Entity.Id == id);
            int? designacionId = trackeado != null
                ? ValorTrackeado<int>(trackeado, nameof(CargoDesignacion.DesignacionId))
                : await PrimeroAsync(_context.Set<CargoDesignacion>().AsNoTracking()
                    .Where(c => c.Id == id).Select(c => (int?)c.DesignacionId));

            _designacionPorCargo[id] = designacionId;
            return designacionId;
        }

        // Una entidad borrada en este mismo guardado conserva su valor original
        private static T ValorTrackeado<T>(EntityEntry entry, string propiedad)
        {
            var p = entry.Property(propiedad);
            return (T)(entry.State == EntityState.Deleted ? p.OriginalValue : p.CurrentValue)!;
        }

        private async Task<int?> PrimeroAsync(IQueryable<int?> query) =>
            _async ? await query.FirstOrDefaultAsync(_cancellationToken) : query.FirstOrDefault();
    }
}
