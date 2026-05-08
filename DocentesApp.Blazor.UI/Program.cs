using DocentesApp.Blazor.UI.Components;
using DocentesApp.Blazor.UI.Services.Auth;
using DocentesApp.Blazor.UI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Auth
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "BlazorAuth";
    options.DefaultChallengeScheme = "BlazorAuth";
})
.AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions,
           NoOpAuthenticationHandler>("BlazorAuth", _ => { });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<IAuthService, AuthService>(); // registra el servicio de autenticación personalizado

// Cliente NSwag hacia la API
//builder.Services.AddHttpClient<IClient, Client>(cl => cl.BaseAddress = new Uri("https://localhost:7270"));

// Handler JWT
builder.Services.AddTransient<AuthorizationMessageHandler>();

// Cliente NSwag hacia la API
builder.Services.AddHttpClient<IClient, Client>(cl =>
    cl.BaseAddress = new Uri("https://localhost:7270"))
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

// Necesario para <AuthorizeView> y [Authorize] en Blazor
//builder.Services.AddAuthentication();
//builder.Services.AddAuthorization();  // reemplaza AddAuthorizationCore()

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();