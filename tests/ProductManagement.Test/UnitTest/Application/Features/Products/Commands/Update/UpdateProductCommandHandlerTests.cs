using Moq;
using ProdManagement.Domain.Entities.Products;
using ProdManagement.Application.Abstractions.Persistence;
using ProdManagement.Application.Features.Products.Commands.UpdateProduct;
using FluentAssertions;

namespace ProdManagement.Test.UnitTest.Application.Features.Products.Commands.Update;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductWriteRepository> _writeRepositoryMock;
    private readonly Mock<IProductReadRepository> _readRepositoryMock;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _writeRepositoryMock = new Mock<IProductWriteRepository>();
        _readRepositoryMock = new Mock<IProductReadRepository>();
        _handler = new UpdateProductCommandHandler(_writeRepositoryMock.Object, _readRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenProductNotFound()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "New Name", true, "user123");

        _readRepositoryMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUserIsUnauthorized()
    {
        var product = new Product("Old Name", DateTime.Today, "123", "email@test.com", true, "someoneElse");

        var command = new UpdateProductCommand(Guid.NewGuid(), "New Name", true, "user123");

        typeof(Product).GetProperty("Id")!.SetValue(product, command.Id);

        _readRepositoryMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldUpdateProduct_WhenInputIsValid()
    {
        var product = new Product("Old Name", DateTime.Today, "123", "email@test.com", true, "user123");
        var command = new UpdateProductCommand(Guid.NewGuid(), "Updated Name", false, "user123");

        typeof(Product).GetProperty("Id")!.SetValue(product, command.Id);

        _readRepositoryMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _writeRepositoryMock.Setup(w => w.UpdateAsync(product, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        product.Name.Should().Be("Updated Name");
        product.IsAvailable.Should().BeFalse();

        _writeRepositoryMock.Verify(w => w.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }
}
