using Housemaid.api.Models;

namespace Housemaid.api.Services;

public interface ITokenService
{
    string CreateAccessToken(User user);
    (string Raw, string hash) CreateRefreshToken();
    string Hash(string token);
}
