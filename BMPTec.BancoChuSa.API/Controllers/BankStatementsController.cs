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
    public class BankStatementsController: ControllerBase
    {
        private readonly ILogger<BankStatementsController> _logger;
        private readonly IBankTransferService _bankTransferService;

        public BankStatementsController(ILogger<BankStatementsController> logger, IBankTransferService bankTransferService)
        {
            _logger = logger;
            _bankTransferService = bankTransferService;
        }

        /// <summary>
        /// Gera extrato de uma conta bancária por período.
        /// </summary>
        /// <param name="accountId">Id da conta bancária</param>
        /// <param name="start">Data inicial (yyyy-MM-dd)</param>
        /// <param name="end">Data final (yyyy-MM-dd)</param>
        [HttpGet("{accountId:guid}")]
        [ProducesResponseType(typeof(BankStatementResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetByPeriod(
            Guid accountId,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            try
            {
                var statement = _bankTransferService.GetStatement(accountId, start, end);
                return Ok(statement);
            }
            catch(ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao gerar extrato.");
                return BadRequest(new { message = ex.Message });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao gerar extrato.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Erro interno ao gerar extrato." });
            }
        }
    }
}
