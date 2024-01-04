using System.Linq.Expressions;
using Application.Common.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using Selise.Ecap.GraphQL.Entity;

namespace Application.Common.Implementations;

public class MongoQueryBuilder : IMongoQueryBuilder
{
    public FilterDefinition<T> Build<T>(Expression<Func<T, bool>> filter, bool includeDeletedItem = false, bool checkPermission = false)
    {
        throw new NotImplementedException();
    }

    public FilterDefinition<T> BuildFromBson<T>(BsonDocument filter, bool includeDeletedItem = false, bool checkPermission = false)
    {
        throw new NotImplementedException();
    }

    public FilterDefinition<T> Build<T>(FilterDefinition<T> filter, bool includeDeletedItem = false, bool checkPermission = false)
    {
        var propertyName = nameof(EntityBase.IsMarkedToDelete);
        if (!includeDeletedItem)
        {
            filter &= Builders<T>.Filter.Eq(propertyName, false);
        }

        return filter;
    }

    public FilterDefinition<T> GetDefaultQuery<T>(bool includeDeletedItem = false, bool checkPermission = false)
    {
        throw new NotImplementedException();
    }

}