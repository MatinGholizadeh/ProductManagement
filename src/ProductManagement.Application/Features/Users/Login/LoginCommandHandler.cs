using MediatR;
using Microsoft.AspNetCore.Identity;
using ProdManagement.Domain.Entities.User;
using ProdManagement.Application.Abstractions.Authentication;

namespace ProdManagement.Application.Features.Users.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    #region Constructor & DI

    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginCommandHandler(
        IJwtTokenGenerator jwtTokenGenerator,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    #endregion Constructor & DI - End

    #region MyRegion

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = null;

        // Login Condition - Email Or Username
        if (request.EmailOrUsername.Contains("@"))
            user = await _userManager.FindByEmailAsync(request.EmailOrUsername);
        else
            user = await _userManager.FindByNameAsync(request.EmailOrUsername);

        if (user == null)
            throw new Exception("invalid credentials. please try again!");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
            throw new Exception("invalid credentials. please try again!");

        return _jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.UserName!,
            user.PhoneNumber!);

    }

    #endregion Handle - End
}
