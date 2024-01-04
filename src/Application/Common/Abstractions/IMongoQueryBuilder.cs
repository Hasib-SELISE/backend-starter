using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Common.Abstractions;

public interface IMongoQueryBuilder
{
    FilterDefinition<T> Build<T>(Expression<Func<T, bool>> filter, bool includeDeletedItem = false, bool checkPermission = false);
    FilterDefinition<T> BuildFromBson<T>(BsonDocument filter, bool includeDeletedItem = false, bool checkPermission = false);
    FilterDefinition<T> Build<T>(FilterDefinition<T> filter, bool includeDeletedItem = false, bool checkPermission = false);
    FilterDefinition<T> GetDefaultQuery<T>(bool includeDeletedItem = false, bool checkPermission = false);
}