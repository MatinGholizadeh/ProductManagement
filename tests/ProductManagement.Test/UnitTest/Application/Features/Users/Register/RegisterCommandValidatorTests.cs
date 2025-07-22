using FluentValidation.TestHelper;
using ProdManagement.Application.Features.Users.Register;

namespace ProdManagement.Test.UnitTest.Application.Features.Users.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Should_HaveValidationErrors_WhenFieldsAreEmpty()
    {
        var command = new RegisterCommand("", "", "", "", "", "");
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
        result.ShouldHaveValidationErrorFor(x => x.Username);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_PassValidation_WhenFieldsAreValid()
    {
        var command = new RegisterCommand(
            FirstName: "Ali",
            LastName: "Rezaei",
            Username: "alirezaei",
            Email: "ali@test.com",
            Password: "123456",
            PhoneNumber: "09123456789"
        );

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_WhenEmailInvalid()
    {
        var command = new RegisterCommand(
            FirstName: "Ali",
            LastName: "Rezaei",
            Username: "alirezaei",
            Email: "invalid-email",
            Password: "123456",
            PhoneNumber: "09123456789"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_HaveError_WhenPasswordTooShort()
    {
        var command = new RegisterCommand(
            FirstName: "Ali",
            LastName: "Rezaei",
            Username: "alirezaei",
            Email: "ali@test.com",
            Password: "123",
            PhoneNumber: "09123456789"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}