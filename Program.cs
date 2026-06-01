using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
// ?? Base de datos MySQL con Pomelo 
builder.Services.AddDbContext<SkyinitContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("SkyInitDB"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("SkyInitDB"))
    )
);
builder.Services.AddScoped<GmailSenderService>();

// ?? Autenticación: Cookie + Google OAuth (NO MODIFICAR)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    // Estos valores deben estar en appsettings.json
    options.ClientId = builder.Configuration["GOOGLE_CLIENT_ID"];
    options.ClientSecret = builder.Configuration["GOOGLE_CLIENT_SECRET"];
    options.CallbackPath = "/signin-google";

    options.Scope.Add(GmailService.Scope.GmailSend);
    options.SaveTokens = true;
});

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Menu/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANTE: habilitar autenticación antes de autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Agentes}/{action=Index}/{id?}");

app.Run();
