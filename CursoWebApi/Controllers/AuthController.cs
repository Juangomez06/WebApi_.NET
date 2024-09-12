using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CursoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Inyección de la configuración (IConfiguration) en el controlador para acceder a valores de configuración, como claves y emisores de JWT
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;  // Asigna la configuración pasada al campo _configuration
        }

        // Define un endpoint HTTP POST que devuelve un token JWT
        [HttpPost("token")]
        public IActionResult GetToken()
        {
            // Definición de las reclamaciones (claims) del JWT, que contienen información sobre el usuario
            var claims = new[]
            {
        // El claim "sub" (subject) indica el ID del usuario (aquí se usa un valor de ejemplo "yourUserId")
        new Claim(JwtRegisteredClaimNames.Sub, "yourUserId"),
        // El claim "jti" (JWT ID) proporciona un identificador único para el token, generado con un GUID
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            // Obtiene la clave secreta desde la configuración (configurada en Jwt:Key) y la convierte a bytes
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            // Define las credenciales de firma usando la clave secreta y el algoritmo de seguridad HMAC con SHA-256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crea el token JWT con el emisor, la audiencia, las reclamaciones, la fecha de expiración y las credenciales de firma
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],  // Define el emisor del token
                audience: _configuration["Jwt:Audience"],  // Define la audiencia para la cual es válido el token
                claims: claims,  // Incluye las reclamaciones definidas
                expires: DateTime.Now.AddDays(30),  // Define la expiración del token a 30 días desde su creación
                signingCredentials: creds  // Usa las credenciales de firma generadas
            );

            // Devuelve el token JWT serializado como respuesta HTTP
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
