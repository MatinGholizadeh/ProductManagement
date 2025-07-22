using MediatR;
using ProdManagement.Application.Abstractions.Persistence;

namespace ProdManagement.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductReadRepository _readRepository;
    private readonly IProductWriteRepository _writeRepository;

    public DeleteProductCommandHandler(
        IProductReadRepository readRepository,
        IProductWriteRepository writeRepository)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var userProducts = await _readRepository.GetByCreatedByAsync(request.CreatedBy, cancellationToken);
        var product = userProducts.FirstOrDefault(p => p.Id.ToString() == request.ProductName);

        if (product == null)
            return false;

        await _writeRepository.DeleteAsync(product, cancellationToken);
        return true;
    }
}
