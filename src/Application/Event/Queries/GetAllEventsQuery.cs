using Application.Common.Models;
using MediatR;

namespace Application.Event.Queries;

public class GetAllEventsQuery : BasePaginationQueryModel, IRequest<ServiceResponse>
{
    public string Name { get; set; }
    public DateTime EventStartDate { get; set; }
    public DateTime EventEndDate { get; set; }
    public string Type { get; set; }
}
