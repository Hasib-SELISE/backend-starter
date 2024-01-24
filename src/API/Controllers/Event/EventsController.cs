using Application.Common.Models;
using Application.Event.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Selise.Ecap.Infrastructure;

namespace API.Controllers.Event
{
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public EventsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProtectedEndPoint]
        public async Task<ServiceResponse> GetAll([FromQuery] GetAllEventsQuery query)
        {
            return await _mediator.Send(query);
        }
    }
}
