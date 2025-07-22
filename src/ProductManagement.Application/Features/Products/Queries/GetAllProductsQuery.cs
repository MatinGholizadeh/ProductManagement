using MediatR;
using ProdManagement.Application.Features.Products.DTOs;

namespace ProdManagement.Application.Features.Products.Queries;

public record GetAllProductsQuery(string CreatedBy) : IRequest<List<CreateProductResponse>>;

