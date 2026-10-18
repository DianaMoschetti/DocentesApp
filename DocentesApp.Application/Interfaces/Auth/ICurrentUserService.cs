namespace DocentesApp.Application.Interfaces.Auth
{
    // Usuario del request actual (auditoría). Todo null fuera de un request autenticado (seed, login): el autor es "Sistema".
    // Sale solo de los claims del token: no consulta la base (evita el ciclo DbContext <-> interceptor de auditoría).
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Nombre { get; }
        string? Email { get; }
    }
}
