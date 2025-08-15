using AutoMapper;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;

namespace TrainMe.Core.Mapping;

/// <summary>
/// AutoMapper profile for User entity mappings
/// </summary>
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // User to DTOs
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.WorkoutItems, opt => opt.MapFrom(src => src.WorkoutItems));

        CreateMap<User, UserSummaryDto>();

        // Request DTOs to User
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutItems, opt => opt.Ignore());

        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.WorkoutItems, opt => opt.Ignore());
    }
}
