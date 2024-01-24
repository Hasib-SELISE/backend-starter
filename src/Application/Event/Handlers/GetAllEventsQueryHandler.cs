using Application.Common.Handlers;
using Application.Common.Models;
using Application.Event.Queries;
using Application.Services;
using MediatR;
using Selise.Ecap.Entities.PrimaryEntities.DWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp;

namespace Application.Event.Handlers
{
    public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, ServiceResponse>
    {
        private readonly IEventQueryService _eventQueryService;
        public GetAllEventsQueryHandler(IEventQueryService eventQueryService)
        {
            _eventQueryService = eventQueryService;
        }

        public async Task<ServiceResponse> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
        {
            var response = await _eventQueryService.GetAllEvents(request);

            return ServiceResponseHandler.HandleSuccess(response, HttpStatusCode.OK);
        }
    }
}
