using FluentValidation.TestHelper;
using ProdManagement.Application.Features.Users.Login;

namespace ProdManagement.Test.UnitTest.Application.Features.Users.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_EmailOrUsernameIsEmpty()
    {
        var result = _validator.TestValidate(new LoginCommand("", "Password123"));
        result.ShouldHaveValidationErrorFor(x => x.EmailOrUsername);
    }

    [Fact]
    public void Should_HaveError_When_EmailOrUsernameIsNotEmail()
    {
        var result = _validator.TestValidate(new LoginCommand("invalid-email", "Password123"));
        result.ShouldHaveValidationErrorFor(x => x.EmailOrUsername);
    }

    [Fact]
    public void Should_HaveError_When_PasswordIsEmpty()
    {
        var result = _validator.TestValidate(new LoginCommand("test@example.com", ""));
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_NotHaveError_When_ValidInput()
    {
        var result = _validator.TestValidate(new LoginCommand("test@example.com", "Password123"));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
