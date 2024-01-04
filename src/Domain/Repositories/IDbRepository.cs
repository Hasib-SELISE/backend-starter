using System.Linq.Expressions;
using Domain.Entities;
using MongoDB.Driver;

namespace Domain.Repositories;

public interface IDbRepository<T>
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter);
    
    List<T> GetAll(Expression<Func<T, bool>> filter);

    List<TProjection> GetAllWithProjection<TProjection>(Expression<Func<T, bool>> filter);
    
    Task<List<TProjection>> GetAllWithProjectionAsync<TProjection>(Expression<Func<T, bool>> filter);

    IQueryable<T> GetQueryable(FilterDefinition<T> filterDefinition);

    Task<T> GetByItemIdAsync(string itemId);
}