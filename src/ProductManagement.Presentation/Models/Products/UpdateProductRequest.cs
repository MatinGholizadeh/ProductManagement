namespace ProdManagement.Presentation.Models.Products;

public class UpdateProductRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool? IsAvailable { get; set; }
}