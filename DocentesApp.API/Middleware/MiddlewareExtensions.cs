namespace DocentesApp.API.Middleware
{
    public static class MiddlewareExtensions // cuando se rompe inesperadamente algo
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}