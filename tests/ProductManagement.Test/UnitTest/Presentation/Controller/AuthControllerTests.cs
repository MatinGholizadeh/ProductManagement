using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using ProdManagement.Application.Features.Users.Login;
using ProdManagement.Application.Features.Users.Register;
using ProdManagement.Presentation.Controllers;

namespace ProdManagement.Test.UnitTest.Presentation.Controller;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ReturnsOk_WithToken()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync("fake-jwt-token");

        var controller = new AuthController(mediatorMock.Object);
        var command = new LoginCommand("testuser", "password");

        // Act
        var result = await controller.Login(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        // اینجا اضافه کن
        var json = JsonConvert.SerializeObject(okResult.Value);
        dynamic data = JsonConvert.DeserializeObject<dynamic>(json);
        string token = data.Token;

        Assert.Equal("fake-jwt-token", token);
    }


    [Fact]
    public async Task Register_ReturnsOk_WithToken()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("fake-jwt-token");

        var controller = new AuthController(mediatorMock.Object);
        var command = new RegisterCommand(
            "TestFirstName",    // FirstName
            "TestLastName",     // LastName
            "testuser",         // Username
            "test@example.com", // Email
            "1234567890",       // PhoneNumber
            "password"          // Password
        );

        // Act
        var result = await controller.Register(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);

        // استفاده از Newtonsoft.Json برای Deserialize کردن و استخراج Token
        var json = JsonConvert.SerializeObject(okResult.Value);
        dynamic data = JsonConvert.DeserializeObject<dynamic>(json);
        string token = data.Token;

        Assert.Equal("fake-jwt-token", token);
    }

}
