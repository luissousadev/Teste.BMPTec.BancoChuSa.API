using BMPTec.BancoChuSa.API.Application.Interfaces.Services;
using BMPTec.BancoChuSa.API.Domain.Entities.BankAccount;
using BMPTec.BancoChuSa.API.DTOs.Auth;
using BMPTec.BancoChuSa.API.DTOs.BankAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace BMPTec.BancoChuSa.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class BankAccountsController: ControllerBase
    {
        private readonly ILogger<BankAccountsController> _logger;
        private readonly IBankAccountService _bankAccountService;

        public BankAccountsController(ILogger<BankAccountsController> logger, IBankAccountService bankAccountService)
        {
            _logger = logger;
            _bankAccountService = bankAccountService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(BankAccount), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] CreateBankAccountRequest request)
        {
            var account = new BankAccount
            {                
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,    
                BankCode = request.BankCode,
                BankName = request.BankName,
                BankBranch = request.BankBranch,
                AccountNumber = request.AccountNumber,
                AccountDigit = request.AccountDigit,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                
                var created = _bankAccountService.Create(account);
                return StatusCode(StatusCodes.Status201Created, created);
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "Erro inesperado ao criar conta bancária.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro interno ao criar conta bancária." });
            }
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BankAccount), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var account = _bankAccountService.GetById(id);

            if(account is null)
                return NotFound();

            return Ok(account);
        }
    }
}
