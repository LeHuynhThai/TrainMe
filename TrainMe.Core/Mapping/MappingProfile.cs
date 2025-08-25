using AutoMapper;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;

namespace TrainMe.Core.Mapping;

/// <summary>
/// Cấu hình AutoMapper cho tất cả các entity
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigureUserMappings();
        ConfigureWorkoutItemMappings();
        ConfigureRandomExerciseMappings();
    }

    private void ConfigureUserMappings()
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

    private void ConfigureWorkoutItemMappings()
    {
        // WorkoutItem to DTOs
        CreateMap<WorkoutItem, WorkoutItemDto>();
        CreateMap<WorkoutItem, WorkoutItemSummaryDto>();

        // Request DTOs to WorkoutItem
        CreateMap<CreateWorkoutItemRequest, WorkoutItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<UpdateWorkoutItemRequest, WorkoutItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }

    private void ConfigureRandomExerciseMappings()
    {
        // RandomExercise to DTO
        CreateMap<RandomExercise, RandomExerciseDto>();
    }
}
