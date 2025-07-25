using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Auth;
using DeveloperEvaluation.Core.Application.DTOs;

namespace DeveloperEvaluation.Core.Application.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<LoginRequestDto, LoginCommand>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

        CreateMap<LoginResponse, LoginResponseDto>()
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token));
    }
}
