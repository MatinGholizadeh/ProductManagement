using MediatR;

namespace ProdManagement.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(string CreatedBy, string ProductName) : IRequest<bool>;
