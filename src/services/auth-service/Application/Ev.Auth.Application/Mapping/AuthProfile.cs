using AutoMapper;
using Ev.Auth.Application.Dtos;
using Ev.Auth.Domain;

namespace Ev.Auth.Application.Mapping;

public sealed class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, AuthResponse>();
    }
}
