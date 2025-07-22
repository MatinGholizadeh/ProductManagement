namespace ProdManagement.Application.Features.Products.DTOs;

public record CreateProductResponse(
    Guid Id,
    string Name,
    DateTime ProductDate,
    string ManufactureEmail,
    string ManufacturePhone,
    bool IsAvailable,
    string CreatedBy);