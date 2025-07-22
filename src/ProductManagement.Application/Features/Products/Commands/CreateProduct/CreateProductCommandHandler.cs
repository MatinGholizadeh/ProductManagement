using MediatR;
using ProdManagement.Application.Abstractions.Persistence;
using ProdManagement.Application.Features.Products.DTOs;
using ProdManagement.Domain.Entities.Products;

namespace ProdManagement.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly IProductWriteRepository _writeRepository;
    private readonly IProductReadRepository _readRepository;

    public CreateProductCommandHandler(
        IProductWriteRepository writeRepository,
        IProductReadRepository readRepository)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // بررسی یونیک بودن (مثلاً بر اساس Email و ProductDate)
        bool exists = await _readRepository.ExistsByEmailAndDateAsync(request.ManufactureEmail, request.ProductDate, cancellationToken);
        if (exists)
            throw new Exception("Product with this ManufactureEmail and ProductDate already exists.");

        var product = new Product(
            name: request.Name,
            productDate: request.ProductDate,
            manufacturePhone: request.ManufacturePhone,
            manufactureEmail: request.ManufactureEmail,
            isAvailable: request.IsAvailable,
            createdBy: request.CreatedBy
        );

        await _writeRepository.AddAsync(product, cancellationToken);

        return new CreateProductResponse(
            product.Id,
            product.Name,
            product.ProductDate,
            product.ManufactureEmail,
            product.ManufacturePhone,
            product.IsAvailable,
            product.CreatedBy);
    }
}