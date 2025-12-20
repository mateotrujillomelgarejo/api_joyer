using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Services;

public class GuestTokenGenerator : IGuestTokenGenerator
{
    public GuestToken Generate()
    {
        var token = new GuestToken
        {
            Token = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(7)
        };
        return token;
    }
}