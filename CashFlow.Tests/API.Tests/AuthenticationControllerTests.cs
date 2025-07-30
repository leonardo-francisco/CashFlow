
using CashFlow.AuthenticationAPI.Configurations;
using CashFlow.AuthenticationAPI.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Tests.API.Tests
{
    public class AuthenticationControllerTests
    {
        [Fact]
        public void Login_ReturnsOk_WithToken_WhenCredentialsAreValid()
        {
            // Arrange
            var credentials = new AdminCredentials
            {
                Username = "admin@cashflow.com.br",
                Password = "123456"
            };
            var optionsMock = new Mock<IOptions<AdminCredentials>>();
            optionsMock.Setup(o => o.Value).Returns(credentials);

            var controller = new AuthenticationController();
            var loginRequest = new LoginRequest
            {
                Email = "admin@cashflow.com.br",
                Password = "123456"
            };

            // Act
            var result = controller.Login(loginRequest, optionsMock.Object);

            // Assert
            Assert.NotNull(result);

        }

        [Fact]
        public void Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var credentials = new AdminCredentials
            {
                Username = "admin@cashflow.com.br",
                Password = "123456"
            };
            var optionsMock = new Mock<IOptions<AdminCredentials>>();
            optionsMock.Setup(o => o.Value).Returns(credentials);

            var controller = new AuthenticationController();
            var loginRequest = new LoginRequest
            {
                Email = "wrong@cashflow.com.br",
                Password = "wrongpassword"
            };

            // Act
            var result = controller.Login(loginRequest, optionsMock.Object);

            // Assert
            Assert.IsType<UnauthorizedHttpResult>(result);
        }
    }
}
