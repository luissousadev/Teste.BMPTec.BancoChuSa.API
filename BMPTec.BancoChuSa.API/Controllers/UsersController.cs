using BMPTec.BancoChuSa.API.Domain.Entities.BankAccount;
using BMPTec.BancoChuSa.API.DTOs.Auth;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BMPTec.BancoChuSa.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController: ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IConfiguration _configuration;

        public UsersController(ILogger<UsersController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {

            try
            {
                // 1. Obter CONNECT STRING do appsettings
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                // 2. Criar conexão com Npgsql
                using var connection = new NpgsqlConnection(connectionString);

                // 3. Query simples pra testar
                var sql = "SELECT id, nome FROM teste;";

                // 4. Executar com Dapper
                var result = connection.Query(sql);

                return Ok(new { success = true, data = result });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar banco");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
            
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginRequest loginRequest)
        {

            if(loginRequest.Email != "lp_as@hotmail.com" || loginRequest.Password != "123456")
            {
                return Unauthorized(new { message = "Usuário ou senha inválidos." });
            }

            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiresInMinutes = int.Parse(jwtSection["ExpiresInMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginRequest.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, loginRequest.Email),
                new Claim(ClaimTypes.Role, "login")
            };

            var expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var loginResponse = new LoginResponse { Token = tokenString };

            return Ok(loginResponse);
        }
    }
}
