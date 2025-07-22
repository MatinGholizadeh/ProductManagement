namespace ProdManagement.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email, string firstName, string lastName, string username, string phoneNumber);
}
