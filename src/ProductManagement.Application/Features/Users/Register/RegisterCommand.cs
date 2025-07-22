using MediatR;

namespace ProdManagement.Application.Features.Users.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string PhoneNumber,
    string Password) : IRequest<string>; // Output: JWT Token
