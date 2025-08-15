using AutoMapper;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;

namespace TrainMe.Core.Mapping;

/// <summary>
/// AutoMapper profile for WorkoutItem entity mappings
/// </summary>
public class WorkoutItemMappingProfile : Profile
{
    public WorkoutItemMappingProfile()
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
}
