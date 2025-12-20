using api_joyeria.Domain.Entities;
using System;

namespace api_joyeria.Application.Interfaces;

public interface IGuestTokenGenerator
{
    GuestToken Generate();
}