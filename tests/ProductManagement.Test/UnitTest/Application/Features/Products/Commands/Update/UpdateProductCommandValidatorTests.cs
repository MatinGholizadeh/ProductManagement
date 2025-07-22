using FluentValidation.TestHelper;
using ProdManagement.Application.Features.Products.Commands.UpdateProduct;

namespace ProdManagement.Test.UnitTest.Application.Features.Products.Commands.Update;

public class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator;

    public UpdateProductCommandValidatorTests()
    {
        _validator = new UpdateProductCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new UpdateProductCommand(Guid.Empty, "Test", true, "user123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_NullOrWhiteSpace()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "", true, "user123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Valid()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Valid Name", true, "user123");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
