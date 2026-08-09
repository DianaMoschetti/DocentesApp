using DocentesApp.Blazor.UI.Components;
using DocentesApp.Blazor.UI.Services.Auth;
using DocentesApp.Blazor.UI.Services.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpContext
builder.Services.AddHttpContextAccessor();

// Cookie auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorizationCore();

// Necesario para que el Task<AuthenticationState> cascada llegue a los componentes
// renderizados con @rendermode InteractiveServer (el <CascadingAuthenticationState />
// de Routes.razor no cruza ese límite de render mode).
builder.Services.AddCascadingAuthenticationState();

// Blazor auth
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<IAuthService, AuthService>();

// Handler JWT � antes del AddHttpClient
builder.Services.AddTransient<AuthorizationMessageHandler>();

// Cliente NSwag
builder.Services.AddHttpClient("DocentesAPI", cl =>
    cl.BaseAddress = new Uri("https://localhost:7270"))
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped<IClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = factory.CreateClient("DocentesAPI");
    return new Client("https://localhost:7270", httpClient);
});

builder.Services.AddHttpClient("BlazorInternal", cl =>
    cl.BaseAddress = new Uri("https://localhost:7255"));
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

// Minimal API para manejar el login/logout, ya que Blazor Server no
// tiene un mecanismo de navegaci�n tradicional para redirigir a una p�gina de logout
// El endpoint setea la cookie y redirige a /
app.MapGet("/auth/signin", async (HttpContext ctx, string token) =>
{
    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var jwt = handler.ReadJwtToken(token);

    var claims = jwt.Claims.ToList();
    claims.Add(new System.Security.Claims.Claim("access_token", token));

    var identity = new System.Security.Claims.ClaimsIdentity(
        claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new System.Security.Claims.ClaimsPrincipal(identity);

    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
        new AuthenticationProperties
        {
            IsPersistent = false,
            ExpiresUtc = jwt.ValidTo
        });

    ctx.Response.Redirect("/");
});

app.MapPost("/auth/signout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok();
});

app.MapGet("/auth/signout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    ctx.Response.Redirect("/login");
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public record SignInRequest(string Token);