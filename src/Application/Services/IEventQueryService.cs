using Application.Common.DTOs.Event;
using Application.Common.Models.ResponseModels;
using Application.Event.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IEventQueryService
    {
        /// <summary>
        /// Get all events
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<GenericPaginationResponseModel<EventListItemDto>> GetAllEvents(GetAllEventsQuery query);
    }
}
