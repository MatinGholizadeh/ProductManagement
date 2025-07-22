using MediatR;
using Microsoft.AspNetCore.Identity;
using ProdManagement.Domain.Entities.User;
using ProdManagement.Application.Abstractions.Authentication;

namespace ProdManagement.Application.Features.Users.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
{
    #region Constructor & DI

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    #endregion Constructor & DI - End

    #region Handler

    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Username,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"User registration failed: {errors}");
        }

        return _jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.UserName!,
            user.PhoneNumber!);
    }

    #endregion Handler - End
}
