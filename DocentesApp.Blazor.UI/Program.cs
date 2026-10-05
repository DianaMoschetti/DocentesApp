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
        // [Diana desde v4.0 OBSOLETO]
        // options.ExpireTimeSpan = TimeSpan.FromHours(8);

        // La cookie no debe vivir más que el JWT. La expiración real la fija /auth/signin
        // con ExpiresUtc = jwt.ValidTo; esto es solo el valor por defecto, alineado con
        // Jwt:DurationInMinutes de la API (15).
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);

        // SlidingExpiration renovaría la cookie en requests HTTP más allá del vencimiento del JWT
        // (cookie válida + token vencido = 401). Además, dentro de un circuito de Blazor Server
        // no hay requests HTTP, así que el sliding no tendría efecto práctico igual.
        options.SlidingExpiration = false;
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

// Aviso de sesión expirada / 401, uno por circuito (ver SessionExpirationWatcher)
builder.Services.AddScoped<SessionExpirationService>();

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
    // [Diana desde v4.0 OBSOLETO]
    // return new Client("https://localhost:7270", httpClient);

    // El Client se crea en el scope del circuito, así que puede avisar los 401
    // al SessionExpirationService del mismo circuito.
    return new Client("https://localhost:7270", httpClient)
    {
        SessionExpiration = sp.GetRequiredService<SessionExpirationService>()
    };
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

// [Diana desde v4.0 OBSOLETO]
// app.MapGet("/auth/signout", async (HttpContext ctx) =>
// {
//     await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//     ctx.Response.Redirect("/login");
// });

// Borra la cookie y redirige al login. Con ?expired=true (lo usa SessionExpirationWatcher)
// el login muestra el mensaje de sesión expirada.
app.MapGet("/auth/signout", async (HttpContext ctx, bool? expired) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    ctx.Response.Redirect(expired == true ? "/login?expired=true" : "/login");
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public record SignInRequest(string Token);