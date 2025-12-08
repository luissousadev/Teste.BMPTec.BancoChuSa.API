using BMPTec.BancoChuSa.API.Application.Interfaces.Services;
using BMPTec.BancoChuSa.API.Application.Services;
using BMPTec.BancoChuSa.API.Domain.Entities.BankAccount;
using BMPTec.BancoChuSa.API.DTOs.Auth;
using BMPTec.BancoChuSa.API.DTOs.BankAccount;
using BMPTec.BancoChuSa.API.DTOs.BankTransfer;
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
    public class BankTransfersController: ControllerBase
    {
        private readonly ILogger<BankTransfersController> _logger;
        private readonly IBankTransferService _bankTransferService;

        public BankTransfersController(ILogger<BankTransfersController> logger, IBankTransferService bankTransferService)
        {
            _logger = logger;
            _bankTransferService = bankTransferService;
        }

        [HttpPost]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create(
                   [FromBody] CreateBankTransferRequest request)
        {
            if(!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var transfer = _bankTransferService.Transfer(
                    request.FromAccountId,
                    request.ToAccountId,
                    request.Amount,
                    request.Description);

                var response = Ok(transfer);
                
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao realizar transferência.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Erro interno ao realizar transferência." });
            }
        }
    }
}
