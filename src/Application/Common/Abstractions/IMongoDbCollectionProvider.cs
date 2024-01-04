using MongoDB.Driver;
using Selise.Ecap.GraphQL.Entity;

namespace Application.Common.Abstractions;

public interface IMongoDbCollectionProvider
{
    IMongoCollection<T> GetDbCollection<T>()
        where T : EntityBase;
}