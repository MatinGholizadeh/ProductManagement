using FluentAssertions;
using FluentValidation.TestHelper;
using ProdManagement.Application.Features.Products.Commands.CreateProduct;

namespace ProdManagement.Test.UnitTest.Application.Features.Products.Commands.Create;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Required_Fields_Are_Missing()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "",
            ProductDate: default,
            IsAvailable: false
        )
        {
            ManufacturePhone = "",
            ManufactureEmail = "",
            CreatedBy = ""
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
        result.ShouldHaveValidationErrorFor(c => c.ProductDate);
        result.ShouldHaveValidationErrorFor(c => c.ManufacturePhone);
        result.ShouldHaveValidationErrorFor(c => c.ManufactureEmail);
        result.ShouldHaveValidationErrorFor(c => c.CreatedBy);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_ProductDate_Is_In_The_Future()
    {
        var futureDate = DateTime.Today.AddDays(1);

        var command = new CreateProductCommand(
            Name: "Test Product",
            ProductDate: futureDate,
            IsAvailable: true
        )
        {
            ManufacturePhone = "+989123456789",
            ManufactureEmail = "email@test.com",
            CreatedBy = "admin"
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.ProductDate);
    }

    [Fact]
    public void Validator_Should_Have_Error_For_Invalid_Email_And_Phone()
    {
        var command = new CreateProductCommand(
            Name: "Test Product",
            ProductDate: DateTime.Today,
            IsAvailable: true
        )
        {
            ManufacturePhone = "123", // invalid
            ManufactureEmail = "not-an-email", // invalid
            CreatedBy = "admin"
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.ManufacturePhone);
        result.ShouldHaveValidationErrorFor(c => c.ManufactureEmail);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_For_Valid_Command()
    {
        var command = new CreateProductCommand(
            Name: "Valid Product",
            ProductDate: DateTime.Today,
            IsAvailable: true
        )
        {
            ManufacturePhone = "+989123456789",
            ManufactureEmail = "email@test.com",
            CreatedBy = "admin"
        };

        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeTrue();
    }
}

