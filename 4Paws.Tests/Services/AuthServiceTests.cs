using Moq;
using Xunit;
using FluentAssertions;
using _4Paws.Services.Auth;
using _4Paws.DTOs.Auth.Requests;
using _4Paws.Data;
using _4Paws.Common.Services;
using _4Paws.Models;
using Microsoft.EntityFrameworkCore;

namespace _4Paws.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<DataContext> _mockDbContext;
        private readonly Mock<SmtpService> _mockSmtpService;
        private readonly Mock<JwtService> _mockJwtService;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockDbContext = new Mock<DataContext>();
            _mockSmtpService = new Mock<SmtpService>();
            _mockJwtService = new Mock<JwtService>();
            _authService = new AuthService(_mockDbContext.Object, _mockSmtpService.Object, _mockJwtService.Object);
        }

        #region Register Tests

       
        [Fact]
        public void Register_WithInvalidRequest_ReturnsValidationError()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Username = "",
                Email = "invalid-email",
                Password = "short"
            };

            // Act
            var result = _authService.Register(registerRequest);

            // Assert
            result.Status.Should().Be(400);
            result.Errors.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
        }

        #endregion
    }
}