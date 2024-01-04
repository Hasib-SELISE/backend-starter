using System.Linq.Expressions;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Selise.Ecap.Infrastructure;

namespace Infrastructure.Persistence.Repositories;

public class DbRepository<T> : IDbRepository<T>
{
    private readonly ILogger<DbRepository<T>> _logger;
    private readonly IRepository _repository;
    private readonly ISecurityContextProvider _securityContextProvider;
    private readonly IEcapMongoDbDataContextProvider _ecapMongoDbDataContextProvider;

    public DbRepository(
        ILogger<DbRepository<T>> logger,
        IRepository repository,
        ISecurityContextProvider securityContextProvider,
        IEcapMongoDbDataContextProvider ecapMongoDbDataContextProvider)
    {
        _logger = logger;
        _repository = repository;
        _securityContextProvider = securityContextProvider;
        _ecapMongoDbDataContextProvider = ecapMongoDbDataContextProvider;
    }

    #region Get

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        var queryable = await GetMongoDbCollection(typeof(T)).FindAsync<T>(filter);
        return await queryable.ToListAsync();
    }

    public List<T> GetAll(Expression<Func<T, bool>> filter)
    {
        var queryable = GetMongoDbCollection(typeof(T)).Find<T>(filter);
        return queryable.ToList();
    }

    public List<TProjection> GetAllWithProjection<TProjection>(Expression<Func<T, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public Task<List<TProjection>> GetAllWithProjectionAsync<TProjection>(Expression<Func<T, bool>> filter)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Update

    

    #endregion

    public IQueryable<T> GetQueryable(FilterDefinition<T> filterDefinition)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetByItemIdAsync(string itemId)
    {
        throw new NotImplementedException();
    }

    private IMongoCollection<T> GetMongoDbCollection(Type collectionType)
    {
        var mongoDbContext = _ecapMongoDbDataContextProvider.GetTenantDataContext(
            _securityContextProvider.GetSecurityContext()
                .TenantId);
        return mongoDbContext.GetCollection<T>($"{collectionType.Name}");
    } 
}