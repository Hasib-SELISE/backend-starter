using System.Globalization;
using Application.Common.Abstractions;
using MongoDB.Driver;
using Selise.Ecap.GraphQL.Entity;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Implementations;

public class MongoDbCollectionProvider : IMongoDbCollectionProvider
{
    private readonly IEcapMongoDbDataContextProvider _ecapMongoDbDataContextProvider;

    public MongoDbCollectionProvider(
        IEcapMongoDbDataContextProvider ecapMongoDbDataContextProvider)
    {
        _ecapMongoDbDataContextProvider = ecapMongoDbDataContextProvider ?? throw new ArgumentNullException(nameof(ecapMongoDbDataContextProvider));
    }

    public IMongoCollection<T> GetDbCollection<T>()
        where T : EntityBase
        => _ecapMongoDbDataContextProvider
            .GetTenantDataContext()
            .GetCollection<T>(string.Format(
                CultureInfo.InvariantCulture,
                "{0}s",
                typeof(T).Name));
}