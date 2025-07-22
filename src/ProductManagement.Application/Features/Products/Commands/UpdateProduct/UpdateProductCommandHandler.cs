using MediatR;
using ProdManagement.Application.Abstractions.Persistence;

namespace ProdManagement.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductWriteRepository _writeRepository;
    private readonly IProductReadRepository _readRepository;

    public UpdateProductCommandHandler(
        IProductWriteRepository writeRepository,
        IProductReadRepository readRepository)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _readRepository.GetByIdAsync(request.Id, cancellationToken);

        //in the end going to have exceptionhandler
        if (product == null) throw new KeyNotFoundException("Product not found.");

        if (product.CreatedBy != request.CurrentUsername)
            throw new UnauthorizedAccessException();

        product.Update(request.Name, request.IsAvailable);

        await _writeRepository.UpdateAsync(product, cancellationToken);

        return true;
    }
}