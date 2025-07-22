using MediatR;

namespace ProdManagement.Application.Features.Users.Login;

public record LoginCommand(
    string EmailOrUsername,
    string Password) : IRequest<string>; // Output: JWT Token
