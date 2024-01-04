using System.Linq.Expressions;
using Application.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Selise.Ecap.GraphQL.Entity;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Abstractions;

public interface IRmwRepository : IRepository
{
    /// <summary>
    /// Get single item with projection
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="projection"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<TResult> GetItemWithProjectionAsync<TResult, T>(Expression<Func<T, bool>> filter, Expression<Func<T, TResult>> projection);
    
    Task<IEnumerable<TResult>> GetSortedPageAsync<TResult, T>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, object>> sortKeySelector, 
        int pageSize, 
        int pageNumber,
        Expression<Func<T, TResult>> projection, 
        string sort);

    Task<long> UpdateManyPartialAsync<T>(Expression<Func<T, bool>> filter, IDictionary<string, object> updates);
    
    IMongoCollection<T> GetCollection<T>(string collectionName, string tenantId = "", bool isPlural = true);

    Task<bool> UpdateOnePartialAsync<T>(Expression<Func<T, bool>> filter, IDictionary<string, object> updates);

    #region Get

    List<T> GetByFilterDefinition<T>(FilterDefinition<T> filterDefinition);
    Task<List<T>> GetByFilterDefinitionAsync<T>(FilterDefinition<T> filterDefinition);
    Task<dynamic> GetItemAsync(string collectionName, string _id, bool isPlural = true, bool excludeDeleted = true);
    dynamic GetItem(string collectionName, string _id, bool isPlural = true, bool excludeDeleted = true);
    Task<T> GetItemAsync<T>(string _id, bool isPlural = true, bool excludeDeleted = true);
    T GetItem<T>(string _id, bool isPlural = true, bool excludeDeleted = true);

    Task<T?> GetItemAsync<T>(Expression<Func<T, bool>> filter, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true);

    T GetItem<T>(Expression<Func<T, bool>> filter, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true);

    Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> filter, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true);

    Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> filter, IMongoDatabase tenantDataContext, string tenantId = "");

    /// <summary>
    /// Retrieves items of type T based on the specified filter, sort, projection, and exclusion of deleted items.
    /// </summary>
    /// <typeparam name="T">The type of items to retrieve.</typeparam>
    /// <typeparam name="TProjectedValue">The type of the projected value.</typeparam>
    /// <param name="filter">The filter expression to apply when retrieving items.</param>
    /// <param name="sort">The sort definition to apply when retrieving items.</param>
    /// <param name="projection">The projection definition to apply when retrieving items.</param>
    /// <param name="excludeDeleted">Optional. Determines whether to exclude deleted items. Defaults to true.</param>
    /// <returns>A task representing the asynchronous operation that returns a list of items of type T.</returns>
    Task<(List<TProjectedValue>, long)> GetItemsWithCountAsync<T, TProjectedValue>(
        FilterDefinition<T> filter,
        SortDefinition<T> sort,
        ProjectionDefinition<T> projection,
        int pageSize,
        int pageNo,
        bool excludeDeleted = true);
    
    Task<(List<T>, long)> GetItemsAsync<T>(FilterDefinition<T> filterDefinition, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true);

    List<T> GetItemsList<T>(Expression<Func<T, bool>> filter, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true);

    IQueryable<T> GetQueryableItems<T>(Expression<Func<T, bool>> filter, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true) where T : EntityBase;

    IQueryable<T> GetQueryableItems<T>(FilterDefinition<T> filter, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true);

    IAggregateFluent<T> GetItems<T>(BsonDocument filter, bool isPlural = true, bool excludeDeleted = true)
        where T : EntityBase;

    IAggregateFluent<TProjection> GetItems<TCollection, TProjection>(BsonDocument filter,
        BsonDocument projection = null, bool isPlural = true, bool excludeDeleted = true);

    IAggregateFluent<TGroup> GetItemsGroup<TCollection, TGroup>(FilterDefinition<TCollection> filter,
        BsonDocument group, bool isPlural = true, bool excludeDeleted = true);

    List<T> GetItemsList<T>(BsonDocument filter, bool isPlural = true, bool excludeDeleted = true);
    T GetItem<T>(BsonDocument filter, bool isPlural = true, bool excludeDeleted = true);

    List<BsonDocument> GetBsonItems(string collectionName, FilterDefinition<BsonDocument> filter,
        BsonDocument projection, string tenantId = "", bool isPlural = true, bool excludeDeleted = true);

    List<BsonDocument> GetBsonItemsByProjection(string collectionName, BsonDocument projection,
        FilterDefinition<BsonDocument> filter, string tenantId = "", bool isPlural = true, bool excludeDeleted = true);

    IAggregateFluent<BsonDocument> GetBsonAggr(string collectionName, FilterDefinition<BsonDocument> filter,
        BsonDocument projection, BsonDocument sort, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true);

    IFindFluent<BsonDocument, BsonDocument> GetBsonData(string collectionName, FilterDefinition<BsonDocument> filter,
        BsonDocument projection, BsonDocument sort, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true);

    BsonDocument GetBsonItem(string collectionName, FilterDefinition<BsonDocument> filter, BsonDocument projection,
        string tenantId = "", bool isPlural = true, bool excludeDeleted = true);

    #endregion

    #region Get Item Count

    /// <summary>
    /// Asynchronously retrieves the number of items in the data store that match the specified filter expression.
    /// </summary>
    /// <typeparam name="T">The type of item to count.</typeparam>
    /// <param name="filter">The filter expression used to query the data store.</param>
    /// <returns>A task representing the asynchronous operation that will eventually complete with the count of items that match the filter expression.</returns>
    Task<long> GetItemCountAsync<T>(Expression<Func<T, bool>> filter);

    /// <summary>
    /// Asynchronously retrieves the number of items in the data store that match the specified filter.
    /// </summary>
    /// <typeparam name="T">The type of item to count.</typeparam>
    /// <param name="filter">The filter used to query the data store.</param>
    /// <returns>A task representing the asynchronous operation that will eventually complete with the count of items that match the filter.</returns>
    Task<long> GetItemCountAsync<T>(FilterDefinition<T> filter);



    #endregion

    #region Check if exist
    Task<bool> ExistsAsync<T>(
            Expression<Func<T, bool>> filter
            , string tenantId = "");

    Task<bool> ExistsAsync<T>(
            Expression<Func<T, bool>> filter
            , bool isPlural, string tenantId = "");

    #endregion

    #region Insert

    T Insert<T>(T entity, string tenantId = "", bool isPlural = true);
    Task<T> InsertAsync<T>(T entity, string tenantId = "", bool isPlural = true);

    IEnumerable<T> InsertMany<T>(IEnumerable<T> entities, string tenantId = "", bool isPlural = true);

    //List<BsonDocument> GetBsonItems<T>(string v, FilterDefinition<T> filter, T bsonDocuments);
    Task<IEnumerable<T>> InsertManyAsync<T>(IEnumerable<T> entities, string tenantId = "", bool isPlural = true);

    #endregion

    #region Update

    T Update<T>(string id, T entity, string tenantId = "", bool isPlural = true);
    Task<T> UpdateAsync<T>(string id, T entity, string tenantId = "", bool isPlural = true);

    long UpdateMany(IEnumerable<BsonDocument> entities, string collectionName, string tenantId = "",
        bool isPlural = true);

    long Update(BsonDocument entity, string collectionName, string tenantId = "", bool isPlural = true);
    void UpdateMany<T>(IEnumerable<T> entities, string tenantId = "", bool isPlural = true) where T : EntityBase;
    Task UpdateManyAsync<T>(IEnumerable<T> entities, string tenantId = "", bool isPlural = true) where T : EntityBase;

    Task<DbActionModel> UpdateOneAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> update);
    Task<DbActionModel> UpdateManyAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> update);

    #endregion

    #region Update Partial

    void UpdatePartial(BsonDocument filter, BsonDocument update, string collectionName, string tenantId = "",
        bool isPlural = true);

    void UpdatePartial(string id, IDictionary<string, object> entity, string collectionName, string tenantId = "",
        bool isPlural = true);

    Task UpdatePartialAsync(string id, IDictionary<string, object> entity, string collectionName, string tenantId = "",
        bool isPlural = true);

    void UpdatePartial(BsonDocument filter, IDictionary<string, object> entity, string collectionName,
        string tenantId = "", bool isPlural = true);

    Task<UpdateResult> UpdateOnePartialAsync<T>(FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> updateDefinition);

    void UpdateBulkPartial(BsonDocument filter, IDictionary<string, object> entity, string collectionName,
        string tenantId = "", bool isPlural = true);

    long UpdateBulkPartial(BsonDocument filter, UpdateDefinition<BsonDocument> update, string collectionName,
        string tenantId = "", bool isPlural = true);

    Task UpdateBulkPartialAsync(BsonDocument filter, IDictionary<string, object> entity, string collectionName,
        string tenantId = "", bool isPlural = true);

    void UpdatePartial(FilterDefinition<BsonDocument> filter, IDictionary<string, object> entity, string collectionName,
        string tenantId = "", bool isPlural = true);

    Task UpdatePartialAsync(FilterDefinition<BsonDocument> filter, IDictionary<string, object> entity,
        string collectionName, string tenantId = "", bool isPlural = true);

    void UpdateBulkPartial(FilterDefinition<BsonDocument> filter, IDictionary<string, object> entity,
        string collectionName, string tenantId = "", bool isPlural = true);

    Task UpdateBulkPartialAsync(FilterDefinition<BsonDocument> filter, IDictionary<string, object> entity,
        string collectionName, string tenantId = "", bool isPlural = true);

    #endregion

    #region Delete

    void DeleteMany<T>(Expression<Func<T, bool>> dataFilters);
    Task DeleteManyAsync<T>(Expression<Func<T, bool>> dataFilters);
    Task<DeleteResult> DeleteManyAsync<T>(FilterDefinition<T> filter);
    long Delete(BsonDocument filter, string collectionName, string tenantId = "", bool isPlural = true);
    long DeleteMany(BsonDocument filter, string collectionName, string tenantId = "", bool isPlural = true);



    #endregion

    #region Upsert

    void Upsert<T>(T entity, bool isCollectionNamePlural = true) where T : EntityBase;
    Task<string> UpsertAsync<T>(T entity, bool isCollectionNamePlural = true) where T : EntityBase;
    void UpsertMany<T>(IEnumerable<T> entities, bool isCollectionNamePlural = true) where T : EntityBase;
    Task UpsertManyAsync<T>(IEnumerable<T> entities, bool isCollectionNamePlural = true) where T : EntityBase;
    Task UpsertManyAsync<T>(Expression<Func<T, bool>> filter, IEnumerable<T> entities) where T : EntityBase;

    #endregion
}