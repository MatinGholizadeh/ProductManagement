using MediatR;

namespace ProdManagement.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string? Name,
    bool? IsAvailable,
    string CurrentUsername) : IRequest<bool>;