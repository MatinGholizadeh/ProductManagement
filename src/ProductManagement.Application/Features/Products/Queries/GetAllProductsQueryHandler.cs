using MediatR;
using AutoMapper;
using ProdManagement.Application.Abstractions.Persistence;
using ProdManagement.Application.Features.Products.DTOs;

namespace ProdManagement.Application.Features.Products.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<CreateProductResponse>>
{
    private readonly IProductReadRepository _readRepository;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(IMapper mapper, IProductReadRepository readRepository)
    {
        _mapper = mapper;
        _readRepository = readRepository;
    }

    public async Task<List<CreateProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = string.IsNullOrWhiteSpace(request.CreatedBy)
            ? await _readRepository.GetAllAsync()
            : await _readRepository.GetByCreatedByAsync(request.CreatedBy, cancellationToken);

        return _mapper.Map<List<CreateProductResponse>>(products);
    }
}