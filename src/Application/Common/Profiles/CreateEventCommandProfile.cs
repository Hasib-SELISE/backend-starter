using Application.Event.Commands;
using AutoMapper;
using Selise.Ecap.Entities.PrimaryEntities.RecruitingMoms.Event;

namespace Application.Common.Profiles;

public class CreateEventCommandProfile : Profile
{
    public CreateEventCommandProfile()
    {
        CreateMap<CreateEventCommand, RmwEvent>();
    }
}
