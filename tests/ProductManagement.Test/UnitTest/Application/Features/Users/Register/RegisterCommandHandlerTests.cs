using Moq;
using Microsoft.AspNetCore.Identity;
using ProdManagement.Domain.Entities.User;
using ProdManagement.Application.Features.Users.Register;
using ProdManagement.Application.Abstractions.Authentication;

namespace ProdManagement.Test.UnitTest.Application.Features.Users.Register;

public class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRegisterUserAndReturnToken_WhenRegistrationSuccessful()
    {
        // Arrange
        var mockUserManager = MockUserManager<ApplicationUser>();
        var mockJwtGenerator = new Mock<IJwtTokenGenerator>();

        var handler = new RegisterCommandHandler(mockUserManager.Object, mockJwtGenerator.Object);

        var command = 
            new RegisterCommand("John","Doe", "johndoe", "john@example.com", "1234567890", "Password123");

        // Set up CreateAsync to assign an Id and return success
        mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .Callback<ApplicationUser, string>((user, _) => user.Id = "1")
            .ReturnsAsync(IdentityResult.Success);

        // Set up GenerateToken with exact parameters
        mockJwtGenerator
            .Setup(j => j.GenerateToken(
                "1",
                "john@example.com",
                "John",
                "Doe",
                "johndoe",
                "1234567890"))
            .Returns("fake-jwt-token");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("fake-jwt-token", result);
    }

    // Helper method to create a mock UserManager
    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(
            store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenRegistrationFails()
    {
        // Arrange
        var mockUserManager = MockUserManager();
        var mockJwtGenerator = new Mock<IJwtTokenGenerator>();

        var handler = new RegisterCommandHandler(mockUserManager.Object, mockJwtGenerator.Object);
        var command = new RegisterCommand(
            FirstName: "John",
            LastName: "Doe",
            Username: "johndoe",
            Email: "john@example.com",
            Password: "Pass123!",
            PhoneNumber: "1234567890"
        );

        var identityErrors = new[] { new IdentityError { Description = "Username already taken" } };
        mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        Assert.Contains("Username already taken", ex.Message);
    }

    private Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );
    }
}