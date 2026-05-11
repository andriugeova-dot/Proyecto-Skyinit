using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;
using Proyecto_SkyInit.Services;

public class AccountController : Controller
{
    private readonly SkyinitContext _context;
    private readonly GmailSenderService _gmailSender;

    public AccountController(SkyinitContext context, GmailSenderService gmailSender)
    {
        _context = context;
        _gmailSender = gmailSender;
    }


    // LOGIN LOCAL

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string correo, string contrasena)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo && u.EstadoCuenta == "Activa");
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(contrasena, usuario.ContrasenaHash))
        {
            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim("UsuarioID", usuario.UsuarioID.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Menu");
    }


    // OLVIDÉ MI CONTRASEÑA

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string correo)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);
        if (usuario == null)
        {
            ViewBag.Error = "El correo no está registrado.";
            return View();
        }

        var token = Guid.NewGuid().ToString();
        var expiration = DateTime.UtcNow.AddMinutes(15);

        _context.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = usuario.UsuarioID,
            Token = token,
            ExpirationDate = expiration
        });
        await _context.SaveChangesAsync();

        var resetLink = Url.Action("ResetPassword", "Account", new { token }, Request.Scheme);


        _gmailSender.SendEmail(usuario.Correo,
       "Recuperación de contraseña",
       $"Haz clic en el siguiente enlace para restablecer tu contraseña: {resetLink}");

        ViewBag.Message = "Se envió un correo con instrucciones.";
        return View();
    }


    // RESET PASSWORD

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        var resetToken = _context.PasswordResetTokens
            .FirstOrDefault(t => t.Token == token && t.ExpirationDate > DateTime.UtcNow);

        if (resetToken == null)
        {
            ViewBag.Error = "El enlace no es válido o expiró.";
            return View("Error");
        }

        return View(new ResetPasswordViewModel { Token = token });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        var resetToken = _context.PasswordResetTokens
            .FirstOrDefault(t => t.Token == model.Token && t.ExpirationDate > DateTime.UtcNow);

        if (resetToken == null)
        {
            ViewBag.Error = "El enlace no es válido o expiró.";
            return View("Error");
        }

        var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == resetToken.UserId);
        usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(model.NuevaContrasena);

        _context.PasswordResetTokens.Remove(resetToken);
        await _context.SaveChangesAsync();

        ViewBag.Message = "Tu contraseña fue restablecida correctamente.";
        return RedirectToAction("Index", "Login");
    }
}