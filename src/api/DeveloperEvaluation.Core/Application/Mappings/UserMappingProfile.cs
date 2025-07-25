using AutoMapper;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Commands.Users;
using DeveloperEvaluation.Core.Domain.Entities;

namespace DeveloperEvaluation.Core.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<CreateUserDto, CreateUserCommand>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<UpdateUserDto, UpdateUserCommand>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<UserNameDto, UserNameDto>();
        CreateMap<UserAddressDto, UserAddressDto>();

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => "********"));

        CreateMap<UserName, UserNameDto>();

        CreateMap<UserAddress, UserAddressDto>()
            .ForMember(dest => dest.GeoLocation, opt => opt.MapFrom(src => src.GeoLocation));

        CreateMap<GeoLocation, GeoLocationDto>();
    }
}
