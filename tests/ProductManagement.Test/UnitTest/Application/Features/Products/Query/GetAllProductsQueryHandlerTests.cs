using Moq;
using AutoMapper;
using FluentAssertions;
using ProdManagement.Domain.Entities.Products;
using ProdManagement.Application.Features.Products.DTOs;
using ProdManagement.Application.Abstractions.Persistence;
using ProdManagement.Application.Features.Products.Queries;

namespace ProdManagement.Test.UnitTest.Application.Features.Products.Query;

public class GetAllProductsQueryHandlerTests
{
    private readonly Mock<IProductReadRepository> _readRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllProductsQueryHandler _handler;

    public GetAllProductsQueryHandlerTests()
    {
        _readRepositoryMock = new Mock<IProductReadRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllProductsQueryHandler(_mapperMock.Object, _readRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllProducts_WhenCreatedByIsEmpty()
    {
        // Arrange
        var products = new List<Product>
            {
                new("Product1", DateTime.Today, "123456", "test1@test.com", true, "user1"),
                new("Product2", DateTime.Today, "654321", "test2@test.com", false, "user2")
            };

        _readRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(products);

        _mapperMock
            .Setup(m => m.Map<List<CreateProductResponse>>(products))
            .Returns(new List<CreateProductResponse>());

        var query = new GetAllProductsQuery("");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _readRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        _readRepositoryMock.Verify(r => r.GetByCreatedByAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFilteredProducts_WhenCreatedByIsProvided()
    {
        // Arrange
        var createdBy = "user123";
        var products = new List<Product>
            {
                new("Product1", DateTime.Today, "123456", "test@test.com", true, createdBy)
            };

        _readRepositoryMock
            .Setup(r => r.GetByCreatedByAsync(createdBy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _mapperMock
            .Setup(m => m.Map<List<CreateProductResponse>>(products))
            .Returns(new List<CreateProductResponse>());

        var query = new GetAllProductsQuery(createdBy);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _readRepositoryMock.Verify(r => r.GetByCreatedByAsync(createdBy, It.IsAny<CancellationToken>()), Times.Once);
        _readRepositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
    }
}