using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DocentesApp.Blazor.UI.Services.Auth
{
    public class CustomAuthStateProvider : RevalidatingServerAuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomAuthStateProvider(
            ILoggerFactory loggerFactory,
            IHttpContextAccessor httpContextAccessor) : base(loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        protected override Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}