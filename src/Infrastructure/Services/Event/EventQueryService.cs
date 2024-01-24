using Application.Common.Abstractions;
using Application.Common.DTOs.Event;
using Application.Common.Models.ResponseModels;
using Application.Event.Queries;
using Application.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Driver;
using Selise.Ecap.Entities.PrimaryEntities.RecruitingMoms.Event;
using Selise.Ecap.Infrastructure;
using System.Text.RegularExpressions;

namespace Infrastructure.Services.Event
{
    public class EventQueryService : IEventQueryService
    {
        private readonly IMapper _mapper;
        private readonly IRmwRepository _repository;
        private readonly ISecurityContextProvider _securityProvider;

        public EventQueryService(
        IMapper mapper,
        IRmwRepository repository, ISecurityContextProvider securityContextProvider)
        {
            _mapper = mapper;
            _repository = repository;
            _securityProvider = securityContextProvider;
        }

        /// <summary>
        /// Get All Events
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<GenericPaginationResponseModel<EventListItemDto>> GetAllEvents(GetAllEventsQuery query)
        {
            var filter = GetEventsCommonDbQuery<RmwEvent>(query);

            var queryable = _repository.GetQueryableItems(filter)
                .ProjectTo<EventListItemDto>(new MapperConfiguration(cfg => cfg.CreateMap<RmwEvent, EventListItemDto>()));

            var data = queryable
                .OrderByDescending(x => x.CreateDate)
                .Skip(query.PageNumber * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return new GenericPaginationResponseModel<EventListItemDto>
            {
                Data = data,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalData = queryable.Count()
            };
        }


        /// <summary>
        /// Prepare filter for Event Db Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>

        private FilterDefinition<RmwEvent> GetEventsCommonDbQuery<T>(GetAllEventsQuery query) where T : class
        {
            var filter = Builders<RmwEvent>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                var regexFilter = Regex.Escape(query.Name);
                var bsonRegex = new BsonRegularExpression(regexFilter, "i");
                filter &= Builders<RmwEvent>.Filter.Regex(eventData => eventData.Name, bsonRegex);
            }

            if (query.Type == GetEventTypeEnum.Upcoming.ToString())
            {
                var currentDateTime = DateTime.UtcNow;

                filter &= Builders<RmwEvent>.Filter.Gte(eventData => eventData.StartDate, currentDateTime);
            }

            else if (query.Type == GetEventTypeEnum.Past.ToString())
            {
                var currentDateTime = DateTime.UtcNow;

                filter &= Builders<RmwEvent>.Filter.Lte(eventData => eventData.EndDate, currentDateTime);
            }

            if (query.EventStartDate != default && query.EventEndDate != default)
            {
                // Checks if there is an intersection point between (query => start date - end date) and (event => start date - end date)

                filter &= Builders<RmwEvent>.Filter.And(Builders<RmwEvent>.Filter.Lte(eventData => eventData.StartDate, query.EventEndDate), Builders<RmwEvent>.Filter.Gte(eventData => eventData.EndDate, query.EventStartDate));
            }


            return filter;
        }

    }
}
