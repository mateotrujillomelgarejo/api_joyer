using System;
using api_joyeria.Application.Interfaces;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Application.Services;

public class GuestTokenGenerator : IGuestTokenGenerator
{
    public GuestToken Generate()
    {
        var token = Guid.NewGuid().ToString("N");
        return new GuestToken
        {
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(7)
        };
    }
}