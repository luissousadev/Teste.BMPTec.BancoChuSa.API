using BMPTec.BancoChuSa.API.Application.Interfaces.Services;
using BMPTec.BancoChuSa.API.Controllers;
using BMPTec.BancoChuSa.API.Domain.Entities.BankAccount;
using BMPTec.BancoChuSa.API.DTOs.BankAccount;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BMPTec.BancoChuSa.API.Tests
{
    public class BankAccountsControllerTests
    {
        private readonly Mock<ILogger<BankAccountsController>> _loggerMock;
        private readonly Mock<IBankAccountService> _serviceMock;
        private readonly BankAccountsController _controller;

        public BankAccountsControllerTests()
        {
            _loggerMock = new Mock<ILogger<BankAccountsController>>();
            _serviceMock = new Mock<IBankAccountService>();

            _controller = new BankAccountsController(
                _loggerMock.Object,
                _serviceMock.Object
            );
        }

        [Fact]
        public void Create_ShouldReturnCreatedResult_WhenAccountIsValid()
        {            
            var request = new CreateBankAccountRequest
            {
                CustomerId = Guid.NewGuid(),
                BankCode = "001",
                BankName = "Banco do Brasil",
                BankBranch = "1234",
                AccountNumber = "987654",
                AccountDigit = "0"
            };

            var expectedAccount = new BankAccount
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

            _serviceMock
                .Setup(x => x.Create(It.IsAny<BankAccount>()))
                .Returns(expectedAccount);

            // Act
            var result = _controller.Create(request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();

            var createdResult = result as CreatedAtActionResult;
            createdResult!.Value.Should().BeEquivalentTo(expectedAccount);
        }
    }
}
