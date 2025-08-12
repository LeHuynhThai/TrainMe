using Microsoft.Extensions.Logging;
using Moq;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories;
using TrainMe.Core.Interfaces.Services.Auth;
using TrainMe.Services;
using Xunit;

namespace TrainMe.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IPasswordService> _passwordServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _passwordServiceMock = new Mock<IPasswordService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _passwordServiceMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_WithValidRequest_ShouldReturnSuccess()
        {
            // Arrange
            var request = new RegisterRequest
            {
                UserName = "testuser",
                Password = "Test123!",
                DisplayName = "Test User",
                Email = "test@example.com",
                Role = Role.Names.User
            };

            var role = new Role { Id = 1, Name = Role.Names.User };
            var hashedPassword = "hashedPassword";

            _userRepositoryMock.Setup(x => x.ExistsByUserNameAsync(request.UserName))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
                .ReturnsAsync(false);
            _roleRepositoryMock.Setup(x => x.GetByNameAsync(request.Role))
                .ReturnsAsync(role);
            _passwordServiceMock.Setup(x => x.HashPassword(request.Password))
                .Returns(hashedPassword);
            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) => { user.Id = 1; return user; });

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(request.UserName, result.Data.UserName);
            Assert.Equal(request.DisplayName, result.Data.DisplayName);
            Assert.Equal(request.Email, result.Data.Email);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingUserName_ShouldReturnError()
        {
            // Arrange
            var request = new RegisterRequest
            {
                UserName = "existinguser",
                Password = "Test123!",
                Role = Role.Names.User
            };

            _userRepositoryMock.Setup(x => x.ExistsByUserNameAsync(request.UserName))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Tên đăng nhập đã tồn tại", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var request = new LoginRequest
            {
                UserName = "testuser",
                Password = "Test123!"
            };

            var role = new Role { Id = 1, Name = Role.Names.User };
            var user = new User
            {
                Id = 1,
                UserName = request.UserName,
                PasswordHash = "hashedPassword",
                IsActive = true,
                Role = role,
                RoleId = role.Id
            };

            var accessToken = "accessToken";
            var refreshToken = new RefreshToken { Token = "refreshToken" };

            _userRepositoryMock.Setup(x => x.GetByUserNameWithRoleAsync(request.UserName))
                .ReturnsAsync(user);
            _passwordServiceMock.Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
                .Returns(true);
            _tokenServiceMock.Setup(x => x.CreateAccessToken(user))
                .Returns(accessToken);
            _tokenServiceMock.Setup(x => x.CreateRefreshToken(user, null))
                .Returns(refreshToken);
            _tokenServiceMock.Setup(x => x.GetAccessTokenExpiration())
                .Returns(DateTime.UtcNow.AddMinutes(30));
            _tokenServiceMock.Setup(x => x.GetRefreshTokenExpiration())
                .Returns(DateTime.UtcNow.AddDays(7));

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(accessToken, result.Data.AccessToken);
            Assert.Equal(refreshToken.Token, result.Data.RefreshToken);
            Assert.Equal(user.UserName, result.Data.User.UserName);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ShouldReturnError()
        {
            // Arrange
            var request = new LoginRequest
            {
                UserName = "testuser",
                Password = "wrongpassword"
            };

            var role = new Role { Id = 1, Name = Role.Names.User };
            var user = new User
            {
                Id = 1,
                UserName = request.UserName,
                PasswordHash = "hashedPassword",
                IsActive = true,
                Role = role,
                RoleId = role.Id
            };

            _userRepositoryMock.Setup(x => x.GetByUserNameWithRoleAsync(request.UserName))
                .ReturnsAsync(user);
            _passwordServiceMock.Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
                .Returns(false);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Thông tin đăng nhập không chính xác", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithInactiveUser_ShouldReturnError()
        {
            // Arrange
            var request = new LoginRequest
            {
                UserName = "testuser",
                Password = "Test123!"
            };

            var role = new Role { Id = 1, Name = Role.Names.User };
            var user = new User
            {
                Id = 1,
                UserName = request.UserName,
                PasswordHash = "hashedPassword",
                IsActive = false,
                Role = role,
                RoleId = role.Id
            };

            _userRepositoryMock.Setup(x => x.GetByUserNameWithRoleAsync(request.UserName))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Tài khoản đã bị vô hiệu hóa", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentUser_ShouldReturnError()
        {
            // Arrange
            var request = new LoginRequest
            {
                UserName = "nonexistentuser",
                Password = "Test123!"
            };

            _userRepositoryMock.Setup(x => x.GetByUserNameWithRoleAsync(request.UserName))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Thông tin đăng nhập không chính xác", result.Message);
        }
    }
}
