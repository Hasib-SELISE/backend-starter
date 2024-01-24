using Selise.Ecap.Entities.PrimaryEntities.RecruitingMoms.Common.Models;

namespace Application.Common.DTOs.Event;

public class EventListItemDto
{
    public string ItemId { get; set; }

    public DateTime CreateDate { get; set; }
    public string Name { get; set; }
    public string EventTag { get; set; }
    public string BannerImageId { get; set; }
    public string Description { get; set; }
    public DateTime RegistrationStartDate { get; set; }
    public DateTime RegistrationEndDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string LocationName { get; set; }
    public string LocationUrl { get; set; }
    public List<RmwSpeaker> Speakers { get; set; }
    public bool? IsRegistered { get; set; }
}
