namespace ProdManagement.Domain.Entities.Products;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTime ProductDate { get; private set; }
    public string ManufacturePhone { get; private set; }
    public string ManufactureEmail { get; private set; }
    public bool IsAvailable { get; private set; }
    public string CreatedBy { get; private set; }

    public Product(string name, DateTime productDate, string manufacturePhone, string manufactureEmail, bool isAvailable, string createdBy)
    {
        Id = Guid.NewGuid();
        Name = name;
        ProductDate = productDate;
        ManufacturePhone = manufacturePhone;
        ManufactureEmail = manufactureEmail;
        IsAvailable = isAvailable;
        CreatedBy = createdBy;
    }

    // Update
    public void Update(string? name = null, bool? isAvailable = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (isAvailable.HasValue)
            IsAvailable = isAvailable.Value;
    }

}

