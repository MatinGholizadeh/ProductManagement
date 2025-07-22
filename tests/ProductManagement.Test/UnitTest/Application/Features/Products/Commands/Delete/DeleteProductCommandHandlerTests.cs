using FluentAssertions;
using Moq;
using ProdManagement.Application.Abstractions.Persistence;
using ProdManagement.Application.Features.Products.Commands.DeleteProduct;
using ProdManagement.Domain.Entities.Products;

namespace ProdManagement.Tests.Unit.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductReadRepository> _readRepositoryMock;
    private readonly Mock<IProductWriteRepository> _writeRepositoryMock;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _readRepositoryMock = new Mock<IProductReadRepository>();
        _writeRepositoryMock = new Mock<IProductWriteRepository>();
        _handler = new DeleteProductCommandHandler(_readRepositoryMock.Object, _writeRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(
            "user123",
            productId.ToString());

        var product = new Product(
            name: "Test Product",
            productDate: DateTime.Today,
            manufacturePhone: "123456789",
            manufactureEmail: "email@test.com",
            isAvailable: true,
            createdBy: "user123"
        );
        // {01dd6dad-5d50-4d9e-9f92-4fabf3708259}
        // {01dd6dad-5d50-4d9e-9f92-4fabf3708259}
        typeof(Product).GetProperty("Id")!.SetValue(product, productId);

        _readRepositoryMock
            .Setup(r => r.GetByCreatedByAsync(command.CreatedBy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { product });

        _writeRepositoryMock
            .Setup(w => w.DeleteAsync(product, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Console.WriteLine($"productId: {productId}");  // {36b39c16-ed73-48f4-8c16-073ca609867c}
        Console.WriteLine($"product.Id: {product.Id}");// {36b39c16-ed73-48f4-8c16-073ca609867c}
        Console.WriteLine($"command.ProductName: {command.ProductName}"); // "user123"
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _writeRepositoryMock.Verify(w => w.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new DeleteProductCommand(
            "user123", 
            Guid.NewGuid().ToString());

        _readRepositoryMock
            .Setup(r => r.GetByCreatedByAsync(command.CreatedBy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>()); // لیست خالی

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _writeRepositoryMock.Verify(w => w.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
