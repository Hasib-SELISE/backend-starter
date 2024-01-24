using Selise.Ecap.Entities.PrimaryEntities.RecruitingMoms.Common.Models;

namespace Application.Event.Commands
{
    public record CreateEventCommand(
        string Name,  
        string EventTag, 
        string BannerImageId, 
        string Description, 
        DateTime RegistrationStartDate, 
        DateTime RegistrationEndDate, 
        DateTime StartDate,
        DateTime EndDate, 
        string StartTime, 
        string EndTime, 
        string LocationName,
        string LocationUrl,
        List<RmwSpeaker> Speakers);
}
