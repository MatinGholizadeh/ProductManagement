using MediatR;
using System.Text.Json.Serialization;
using ProdManagement.Application.Features.Products.DTOs;

namespace ProdManagement.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    DateTime ProductDate,
    bool IsAvailable) : IRequest<CreateProductResponse>
{
    [JsonIgnore]
    public string ManufacturePhone { get; init; } = string.Empty;

    [JsonIgnore]
    public string ManufactureEmail { get; init; } = string.Empty;

    [JsonIgnore]
    public string CreatedBy { get; init; } = string.Empty;

};