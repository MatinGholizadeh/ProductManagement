using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using ProdManagement.Application.Abstractions.Authentication;
using ProdManagement.Application.Features.Users.Login;
using ProdManagement.Domain.Entities.User;

namespace ProdManagement.Test.UnitTest.Application.Features.Users.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();

        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);

        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        _handler = new LoginCommandHandler(
            _jwtTokenGeneratorMock.Object,
            _userManagerMock.Object,
            _signInManagerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCredentialsAreCorrect_UsingEmail()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@example.com",
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "123456789"
        };

        var request = new LoginCommand("test@example.com", "Password123");

        _userManagerMock
            .Setup(u => u.FindByEmailAsync(request.EmailOrUsername))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(s => s.CheckPasswordSignInAsync(user, request.Password, false))
            .ReturnsAsync(SignInResult.Success);

        _jwtTokenGeneratorMock
            .Setup(j => j.GenerateToken(user.Id, user.Email, user.FirstName, user.LastName, user.UserName, user.PhoneNumber))
            .Returns("fake-jwt-token");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be("fake-jwt-token");
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginCommand("notfound@example.com", "Password123");

        _userManagerMock
            .Setup(u => u.FindByEmailAsync(request.EmailOrUsername))
            .ReturnsAsync((ApplicationUser)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("invalid credentials*");
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@example.com",
            UserName = "testuser"
        };

        var request = new LoginCommand("test@example.com", "WrongPassword");

        _userManagerMock
            .Setup(u => u.FindByEmailAsync(request.EmailOrUsername))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(s => s.CheckPasswordSignInAsync(user, request.Password, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("invalid credentials*");
    }
}
