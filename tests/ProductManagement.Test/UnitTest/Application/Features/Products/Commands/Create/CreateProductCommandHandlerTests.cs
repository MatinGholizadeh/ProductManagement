using Moq;
using FluentAssertions;
using ProdManagement.Domain.Entities.Products;
using ProdManagement.Application.Abstractions.Persistence;
using ProdManagement.Application.Features.Products.Commands.CreateProduct;

namespace ProdManagement.Test.UnitTest.Application.Features.Products.Commands.Create;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductReadRepository> _mockReadRepo;
    private readonly Mock<IProductWriteRepository> _mockWriteRepo;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _mockReadRepo = new Mock<IProductReadRepository>();
        _mockWriteRepo = new Mock<IProductWriteRepository>();

        _handler = new CreateProductCommandHandler(_mockWriteRepo.Object, _mockReadRepo.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenProductExists()
    {
        // Arrange
        var command = new CreateProductCommand("Test", DateTime.UtcNow, true)
        {
            ManufactureEmail = "email@test.com",
            ManufacturePhone = "123456",
            CreatedBy = "user1"
        };

        _mockReadRepo
            .Setup(r => r.ExistsByEmailAndDateAsync(command.ManufactureEmail, command.ProductDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Product with this ManufactureEmail and ProductDate already exists.");
    }

    [Fact]
    public async Task Handle_ShouldAddProductAndReturnResponse_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new CreateProductCommand("Test", DateTime.UtcNow, true)
        {
            ManufactureEmail = "email@test.com",
            ManufacturePhone = "123456",
            CreatedBy = "user1"
        };

        _mockReadRepo
            .Setup(r => r.ExistsByEmailAndDateAsync(command.ManufactureEmail, command.ProductDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Product? addedProduct = null;

        _mockWriteRepo
            .Setup(w => w.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((prod, _) => addedProduct = prod)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.ManufactureEmail.Should().Be(command.ManufactureEmail);
        result.ManufacturePhone.Should().Be(command.ManufacturePhone);
        result.IsAvailable.Should().Be(command.IsAvailable);
        result.CreatedBy.Should().Be(command.CreatedBy);
        result.ProductDate.Should().Be(command.ProductDate);

        // همچنین مطمئن شو که AddAsync فراخوانی شده
        _mockWriteRepo.Verify(w => w.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);

        // مطمئن شو که محصول ساخته شده با داده‌های درست است
        addedProduct.Should().NotBeNull();
        addedProduct!.Name.Should().Be(command.Name);
        addedProduct.ManufactureEmail.Should().Be(command.ManufactureEmail);
    }
}
