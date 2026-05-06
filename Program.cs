using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google; // IMPORTANTE
using Proyecto_SkyInit.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Conexión a MySQL (no cambiar)
builder.Services.AddDbContext<SkyinitContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("SkyInitDB")));

// Configuración de autenticación con Google
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
});

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();


