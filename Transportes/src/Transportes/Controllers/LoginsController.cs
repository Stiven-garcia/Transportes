using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Transportes.Models;

namespace Transportes.Controllers
{
    [AllowAnonymous]
    public class LoginsController : Controller
    {
        private readonly TransportesContext _context;
        private readonly IConfiguration _configuration;

        public LoginsController(TransportesContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Registra un usuario
        /// </summary>
        /// <returns></returns>
        public IActionResult Registrarse()
        {
            return View();
        }

        /// <summary>
        /// Registra en base de datos un usuario
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrarse(Login login)
        {
            if (ModelState.IsValid)
            {
                _context.Add(login);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IniciarSesion));
            }
            return View(login);
        }

        /// <summary>
        /// Iniciar sesion de un usuario
        /// </summary>
        /// <returns></returns>
        public IActionResult IniciarSesion()
        {
            return View();
        }

        /// <summary>
        /// valida e inicia sesion
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IniciarSesion(Login login)
        {
            if (login == null || login.Contraseña == null || login.Correo == null )
            {
                return BadRequest();
            }

            if (IsValidUser(login.Correo, login.Contraseña))
            {
                // Crear los claims del usuario
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, login.Correo)
                };

                // Generar el token JWT
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                );

                // Devolver el token JWT
                HttpContext.Request.Headers.Add("Authorization", $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}");
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return Unauthorized();
        }

        private bool IsValidUser(string username, string password)
        {
            return _context.Login.Any(x => x.Correo.Equals(username) && x.Contraseña.Equals(password));
        }

    }
}
