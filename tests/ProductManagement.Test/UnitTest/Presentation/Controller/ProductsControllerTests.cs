using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProdManagement.Application.Features.Products.Commands.CreateProduct;
using ProdManagement.Application.Features.Products.Commands.DeleteProduct;
using ProdManagement.Application.Features.Products.Commands.UpdateProduct;
using ProdManagement.Application.Features.Products.DTOs;
using ProdManagement.Application.Features.Products.Queries;
using ProdManagement.Presentation.Controllers;
using ProdManagement.Presentation.Models.Products;
using System.Security.Claims;

namespace ProdManagement.Test.UnitTest.Presentation.Controller;

public class ProductsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();

        _controller = new ProductsController(_mediatorMock.Object);

        var claims = new[]
        {
                new Claim("Username", "testuser"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim("PhoneNumber", "09120000000")
            };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithProductResponse()
    {
        var command = new CreateProductCommand(
            Name: "Sample Product",
            ProductDate: new DateTime(2025, 1, 1),
            IsAvailable: true
        );

        var expectedResponse = new CreateProductResponse
        (
            Guid.NewGuid(),
            command.Name,
            command.ProductDate,
            "test@example.com",
            "09120000000",
            command.IsAvailable,
            "testuser"
        );

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateProductCommand>(c =>
                    c.Name == command.Name &&
                    c.CreatedBy == "testuser" &&
                    c.ManufactureEmail == "test@example.com" &&
                    c.ManufacturePhone == "09120000000"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.Create(command);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ProductsController.GetById), createdResult.ActionName);

        var actualResponse = Assert.IsType<CreateProductResponse>(createdResult.Value);
        Assert.Equal(expectedResponse.Id, actualResponse.Id);
        Assert.Equal("Sample Product", actualResponse.Name);
        Assert.Equal("testuser", actualResponse.CreatedBy);
        Assert.Equal("test@example.com", actualResponse.ManufactureEmail);
        Assert.Equal("09120000000", actualResponse.ManufacturePhone);
    }

    [Fact]
    public async Task DeleteByName_ReturnsNoContent_WhenSuccess()
    {
        var request = new DeleteProductRequest { ProductName = "ProductToDelete" };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<DeleteProductCommand>(c =>
                    c.CreatedBy == "testuser" &&
                    c.ProductName == request.ProductName),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.DeleteByName(request);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteByName_ReturnsNotFound_WhenFail()
    {
        var request = new DeleteProductRequest { ProductName = "NotExists" };

        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<DeleteProductCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _controller.DeleteByName(request);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("NotExists", notFound.Value.ToString());
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenSuccess()
    {
        var request = new UpdateProductRequest
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedProduct",
            IsAvailable = true
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<UpdateProductCommand>(c =>
                    c.Id == request.Id &&
                    c.Name == request.Name &&
                    c.IsAvailable == request.IsAvailable &&
                    c.CurrentUsername == "testuser"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.Update(request);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsForbid_WhenUnauthorizedOrNotFound()
    {
        var request = new UpdateProductRequest
        {
            Id = Guid.NewGuid(),
            Name = "BadUpdate",
            IsAvailable = false
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<UpdateProductCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _controller.Update(request);

        var forbid = Assert.IsType<ForbidResult>(result);
    }
}