using System.Linq.Expressions;
using Application.Common.Abstractions;
using Application.Common.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using MongoDB.Driver.Linq;
using Selise.Ecap.GraphQL.Entity;
using Selise.Ecap.Infrastructure;
using BulkWriteError = Selise.Ecap.Infrastructure.BulkWriteError;

namespace Application.Common.Implementations;

public class RmwRepository : IRmwRepository
{
    private readonly ISecurityContextProvider _securityContextProvider;
    private readonly IEcapMongoDbDataContextProvider _ecapMongoDbDataContextProvider;
    private readonly IRepository _repository;
    private readonly IRmwLogger<RmwRepository> _logger;

    public RmwRepository(
        ISecurityContextProvider securityContextProvider,
        IEcapMongoDbDataContextProvider ecapMongoDbDataContextProvider,
        IRepository repository,
        IRmwLogger<RmwRepository> logger)
    {
        _securityContextProvider = securityContextProvider;
        _ecapMongoDbDataContextProvider = ecapMongoDbDataContextProvider;
        _repository = repository;
        _logger = logger;
    }

    public async Task<TResult> GetItemWithProjectionAsync<TResult, T>(Expression<Func<T, bool>> filter, Expression<Func<T, TResult>> projection)
    {
        var collection = _ecapMongoDbDataContextProvider.GetTenantDataContext()
            .GetCollection<T>(typeof(T).Name + "s");
        
        var options = new FindOptions<T, TResult>
        {
            Projection = Builders<T>.Projection.Expression(projection)
        };

        var result = await collection.FindAsync(filter, options);

        return await result.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TResult>> GetSortedPageAsync<TResult, T>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, object>> sortKeySelector,
        int pageSize,
        int pageNumber,
        Expression<Func<T, TResult>> projection, string sort = "{CreateDate: -1}")
    {
        var sortDocument = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(sort);
        var sortDefinition = Builders<T>.Sort.Descending(sortDocument.Elements.First().Name)
                             ?? Builders<T>.Sort.Ascending(sortDocument.Elements.First().Name);

        var collection = _ecapMongoDbDataContextProvider.GetTenantDataContext()
            .GetCollection<T>(typeof(T).Name + "s");

        var result = await collection.FindAsync(filter, new FindOptions<T, TResult>
        {
            Skip = pageSize * (pageNumber - 1),
            Limit = pageSize,
            Sort = sortDefinition,
            Projection = Builders<T>.Projection.Expression(projection)
        });

        return await result.ToListAsync();
    }

    public async Task<IEnumerable<TResult>> GetSortedPageAsync<TResult, T>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, object>> sortKeySelector,
        bool ascending,
        int pageSize,
        int pageNumber)
    {
        var sort = ascending
            ? Builders<T>.Sort.Ascending(sortKeySelector)
            : Builders<T>.Sort.Descending(sortKeySelector);

        var collection = _ecapMongoDbDataContextProvider.GetTenantDataContext()
            .GetCollection<T>(typeof(T).Name + "s");

        var result = await collection.FindAsync(filter, new FindOptions<T, TResult>
        {
            Skip = pageSize * (pageNumber - 1),
            Limit = pageSize,
            Sort = sort
        });
        return await result.ToListAsync();
    }

    public async Task<long> UpdateManyPartialAsync<T>(
        Expression<Func<T, bool>> filter,
        IDictionary<string, object> updates)
    {
        var updateDefinitionBuilder = Builders<T>.Update;

        var collection = _ecapMongoDbDataContextProvider.GetTenantDataContext()
            .GetCollection<T>(typeof(T).Name + "s");

        var updateDefinitionList =
            (from update in updates
                let propertyName = update.Key
                let propertyValue = update.Value
                select updateDefinitionBuilder.Set(propertyName, propertyValue)).ToList();

        var combinedUpdateDefinition = updateDefinitionBuilder.Combine(updateDefinitionList);
        var result = await collection.UpdateManyAsync(filter, combinedUpdateDefinition);
        return result.ModifiedCount;
    }

    public async Task<bool> UpdateOnePartialAsync<T>(
        Expression<Func<T, bool>> filter,
        IDictionary<string, object> updates)
    {
        var updateDefinitionBuilder = Builders<T>.Update;

        var collection = _ecapMongoDbDataContextProvider.GetTenantDataContext()
            .GetCollection<T>(typeof(T).Name + "s");

        var updateDefinitionList =
            (from update in updates
                let propertyName = update.Key
                let propertyValue = update.Value
                select updateDefinitionBuilder.Set(propertyName, propertyValue)).ToList();

        var combinedUpdateDefinition = updateDefinitionBuilder.Combine(updateDefinitionList);
        var result = await collection.UpdateOneAsync(filter, combinedUpdateDefinition);

        return result.ModifiedCount > 0;
    }

    public async Task<T> GetAsyncWithRelatedData<T>(Expression<Func<T, bool>> filter, params string[] include)
    {
        var pipeline = new List<BsonDocument> { new("$match", filter.ToBsonDocument()) };

        var collection = _ecapMongoDbDataContextProvider.GetTenantDataContext()
            .GetCollection<T>(typeof(T).Name + "s");

        pipeline.AddRange(include.Select(inc => new BsonDocument("$lookup",
            new BsonDocument
            {
                { "from", inc }, 
                { "localField", $"{typeof(T).Name}Id" }, 
                { "foreignField", "Id" }, { "as", inc }
            })));

        var result = await collection.Aggregate<T>(pipeline).FirstOrDefaultAsync();
        return result;
    }
    
    public IMongoCollection<T> GetCollection<T>(string collectionName, string tenantId = "", bool isPlural = true)
    {
        if (isPlural)
        {
            collectionName += "s";
        }

        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);
        var collection = mongoDb.GetCollection<T>(collectionName);
        return collection;
    }

    #region Get

    public Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> dataFilters)
    {
        return _repository.ExistsAsync(dataFilters);
    }

    public T GetItem<T>(Expression<Func<T, bool>> dataFilters)
    {
        return _repository.GetItem(dataFilters);
    }

    public Task<T> GetItemAsync<T>(Expression<Func<T, bool>> dataFilters)
    {
        return _repository.GetItemAsync<T>(dataFilters);
    }

    public Task<T> GetItemAsync<T>(string tenantId, Expression<Func<T, bool>> dataFilters)
    {
        return _repository.GetItemAsync<T>(tenantId, dataFilters);
    }

    public IQueryable<T> GetItems<T>(Expression<Func<T, bool>> dataFilters)
    {
        return _repository.GetItems(dataFilters);
    }

    public IQueryable<T> GetItems<T>()
    {
        return _repository.GetItems<T>();
    }

    public List<T> GetByFilterDefinition<T>(FilterDefinition<T> filterDefinition)
    {
        return _ecapMongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>(typeof(T).Name + "s")
            .Find<T>(filterDefinition).ToList();
    }

    public async Task<List<T>> GetByFilterDefinitionAsync<T>(FilterDefinition<T> filterDefinition)
    {
        var items = await _ecapMongoDbDataContextProvider.GetTenantDataContext().GetCollection<T>(typeof(T).Name + "s")
            .FindAsync<T>(filterDefinition);
        return items.ToList();
    }

    #endregion

    #region Save

    public void Save<T>(T data, string collectionName = "")
    {
        _repository.Save(data, collectionName);
    }

    public void Save<T>(List<T> datas)
    {
        _repository.Save(datas);
    }

    public Task SaveAsync<T>(string tenantId, T data)
    {
        return _repository.SaveAsync(tenantId, data);
    }

    public Task SaveAsync<T>(T data, string collectionName = "")
    {
        return _repository.SaveAsync(data, collectionName);
    }

    public Task SaveAsync<T>(string tenantId, List<T> datas)
    {
        return _repository.SaveAsync(tenantId, datas);
    }

    public Task SaveAsync<T>(List<T> datas)
    {
        return _repository.SaveAsync(datas);
    }

    public Task<BulkWriteError[]> SaveExpectingFailuresAsync<T>(List<T> datas)
    {
        return _repository.SaveExpectingFailuresAsync<T>(datas);
    }

    #endregion

    #region Update

    public void Update<T>(Expression<Func<T, bool>> dataFilters, T data)
    {
        _repository.Update(dataFilters, data);
    }

    public Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, T data)
    {
        return _repository.UpdateAsync(dataFilters, data);
    }

    public Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, string tenantId, T data)
    {
        return _repository.UpdateAsync<T>(dataFilters, tenantId, data);
    }

    public Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, IDictionary<string, object> updates)
    {
        return _repository.UpdateAsync(dataFilters, updates);
    }

    public Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, string tenantId,
        IDictionary<string, object> updates)
    {
        return _repository.UpdateAsync(dataFilters, tenantId, updates);
    }

    public void UpdateMany<T>(Expression<Func<T, bool>> dataFilters, object data, string collectionName = "")
    {
        _repository.UpdateMany(dataFilters, data, collectionName);
    }

    public Task UpdateManyAsync<T>(Expression<Func<T, bool>> dataFilters, IDictionary<string, object> updates)
    {
        return _repository.UpdateManyAsync(dataFilters, updates);
    }

    public Task UpdateManyAsync<T>(Expression<Func<T, bool>> dataFilters, string tenantId,
        IDictionary<string, object> updates)
    {
        return _repository.UpdateManyAsync(dataFilters, tenantId, updates);
    }

    #endregion

    #region Delete

    public void Delete<T>(Expression<Func<T, bool>> dataFilters)
    {
        _repository.Delete(dataFilters);
    }

    public void Delete<T>(Expression<Func<T, bool>> dataFilters, string collectionName)
    {
        _repository.Delete(dataFilters, collectionName);
    }

    public Task DeleteAsync<T>(Expression<Func<T, bool>> dataFilters)
    {
        return _repository.DeleteAsync<T>(dataFilters);
    }

    public Task DeleteAsync<T>(Expression<Func<T, bool>> dataFilters, string collectionName)
    {
        return _repository.DeleteAsync<T>(dataFilters, collectionName);
    }

    #endregion


    #region Custom

    #region Insert

    public T Insert<T>(T entity, string tenantId = "", bool isPlural = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);

        collection.InsertOne(entity);

        return entity;
    }

    public async Task<T> InsertAsync<T>(T entity, string tenantId = "", bool isPlural = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);

        await collection.InsertOneAsync(entity);

        return entity;
    }

    public IEnumerable<T> InsertMany<T>(IEnumerable<T> entities, string tenantId = "", bool isPlural = true)
    {
        if (entities.Any())
        {
            var type = typeof(T);
            var collectionName = isPlural ? type.Name + "s" : type.Name;
            var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
            var collection = mongoDb.GetCollection<T>(collectionName);

            collection.InsertMany((IEnumerable<T>)entities);
        }

        return entities;
    }

    public async Task<IEnumerable<T>> InsertManyAsync<T>(IEnumerable<T> entities, string tenantId = "",
        bool isPlural = true)
    {
        if (entities.Any())
        {
            var type = typeof(T);
            var collectionName = isPlural ? type.Name + "s" : type.Name;
            var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
            var collection = mongoDb.GetCollection<T>(collectionName);

            await collection.InsertManyAsync((IEnumerable<T>)entities);
        }

        return entities;
    }

    #endregion

    #region Update

    public T Update<T>(string id, T entity, string tenantId = "", bool isPlural = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);

        var filter = Builders<T>.Filter.Eq("_id", id);
        collection.ReplaceOne(filter, entity);

        return entity;
    }

    public async Task<T> UpdateAsync<T>(string id, T entity, string tenantId = "", bool isPlural = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);

        var filter = Builders<T>.Filter.Eq("_id", id);
        await collection.ReplaceOneAsync(filter, entity);

        return entity;
    }

    public long UpdateMany(IEnumerable<BsonDocument> entities, string collectionName, string tenantId = "",
        bool isPlural = true)
    {
        long updated = 0;
        if (entities.Any())
        {
            collectionName = isPlural ? collectionName + "s" : collectionName;
            var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
            var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

            var updates = new List<WriteModel<BsonDocument>>();
            var securityContext = _securityContextProvider.GetSecurityContext();
            foreach (var doc in entities)
            {
                var filter = new BsonDocument { { "_id", doc.GetValue("_id") } };
                var replace = new ReplaceOneModel<BsonDocument>(filter, doc);
                replace.IsUpsert = true;
                updates.Add(replace);
            }

            var response = collection.BulkWrite(updates);
            updated = response.ModifiedCount;
        }

        return updated;
    }

    public long Update(BsonDocument entity, string collectionName, string tenantId = "", bool isPlural = true)
    {
        long updated = 0;

        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        var update = new List<WriteModel<BsonDocument>>();
        var securityContext = _securityContextProvider.GetSecurityContext();
        var filter = new BsonDocument { { "_id", entity.GetValue("_id") } };

        var response = collection.ReplaceOne(filter, entity, new ReplaceOptions { IsUpsert = true });
        updated = response.ModifiedCount;

        return updated;
    }

    public void UpdateMany<T>(IEnumerable<T> entities, string tenantId = "", bool isPlural = true) where T : EntityBase
    {
        if (entities.Any())
        {
            var type = typeof(T);
            var collectionName = isPlural ? type.Name + "s" : type.Name;
            var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
            var collection = mongoDb.GetCollection<T>(collectionName);

            var updates = new List<WriteModel<T>>();
            var securityContext = _securityContextProvider.GetSecurityContext();
            foreach (var doc in entities)
            {
                var filter = Builders<T>.Filter.Eq("_id", doc.ItemId);
                doc.LastUpdateDate = DateTime.UtcNow;
                doc.LastUpdatedBy = securityContext.UserId;
                updates.Add(new ReplaceOneModel<T>(filter, doc));
            }

            var response = collection.BulkWrite(updates);
        }
    }

    public async Task UpdateManyAsync<T>(IEnumerable<T> entities, string tenantId = "", bool isPlural = true)
        where T : EntityBase
    {
        if (entities.Any())
        {
            var type = typeof(T);
            var collectionName = isPlural ? type.Name + "s" : type.Name;
            var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
            var collection = mongoDb.GetCollection<T>(collectionName);

            var updates = new List<WriteModel<T>>();
            var securityContext = _securityContextProvider.GetSecurityContext();

            foreach (var doc in entities)
            {
                var filter = Builders<T>.Filter.Eq("_id", doc.ItemId);
                doc.LastUpdateDate = DateTime.UtcNow;
                doc.LastUpdatedBy = securityContext.UserId;
                updates.Add(new ReplaceOneModel<T>(filter, doc));
            }

            var response = await collection.BulkWriteAsync(updates);
        }
    }

    public async Task<DbActionModel> UpdateOneAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        var actionResponse = new DbActionModel();
        var collection = GetCollection<T>(string.Format("{0}", typeof(T).Name,false));
        var result = await collection.UpdateOneAsync(filter, update);
        actionResponse.SetActionData(result, result.UpsertedId?.ToString());

        return actionResponse;
    }

    public async Task<DbActionModel> UpdateManyAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        var actionResponse = new DbActionModel();
        var collection = GetCollection<T>(string.Format("{0}s", typeof(T).Name));
        var result = await collection.UpdateManyAsync(filter, update);
        actionResponse.SetActionData(result, result.UpsertedId?.ToString());

        return actionResponse;
    }

    #endregion

    #region Update Partial

    public long Delete(BsonDocument filter, string collectionName, string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);


        var result = collection.DeleteOne(filter);
        return result.DeletedCount;
    }

    public long DeleteMany(BsonDocument filter, string collectionName, string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        var result = collection.DeleteMany(filter);
        return result.DeletedCount;
    }

    public void UpdatePartial(BsonDocument filter, BsonDocument update, string collectionName, string tenantId = "",
        bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        collection.UpdateOne(filter, update);
    }

    public void UpdatePartial(string id, IDictionary<string, object> entity, string collectionName,
        string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
        UpdateDefinition<BsonDocument> update = null;
        foreach (var (key, value) in entity)
        {
            if (update == null)
            {
                var builder = Builders<BsonDocument>.Update;
                update = builder.Set(key, value);
            }
            else
            {
                update = update.Set(key, value);
            }
        }

        collection.UpdateOne(filter, update);
    }

    public async Task UpdatePartialAsync(string id, IDictionary<string, object> entity, string collectionName,
        string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
        UpdateDefinition<BsonDocument> update = null;
        foreach (var (key, value) in entity)
        {
            if (update == null)
            {
                var builder = Builders<BsonDocument>.Update;
                update = builder.Set(key, value);
            }
            else
            {
                update = update.Set(key, value);
            }
        }

        await collection.UpdateOneAsync(filter, update);
    }

    public void UpdatePartial(
        BsonDocument filter, 
        IDictionary<string, object> entity, 
        string collectionName,
        string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        UpdateDefinition<BsonDocument> update = null;
        foreach (var (key, value) in entity)
        {
            if (update == null)
            {
                var builder = Builders<BsonDocument>.Update;
                update = builder.Set(key, value);
            }
            else
            {
                update = update.Set(key, value);
            }
        }

        collection.UpdateOne(filter, update);
    }

    public async Task<UpdateResult> UpdateOnePartialAsync<T>(
        FilterDefinition<BsonDocument> filter, 
        UpdateDefinition<BsonDocument> updateDefinition)
    {
        var collectionName = typeof(T).Name + "s";
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        return await collection.UpdateOneAsync(filter, updateDefinition);
    }

    public void UpdateBulkPartial(BsonDocument filter, IDictionary<string, object> entity, string collectionName,
        string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        UpdateDefinition<BsonDocument> update = null;
        foreach (var (key, value) in entity)
        {
            if (update == null)
            {
                var builder = Builders<BsonDocument>.Update;
                update = builder.Set(key, value);
            }
            else
            {
                update = update.Set(key, value);
            }
        }

        collection.UpdateMany(filter, update);
    }

    public long UpdateBulkPartial(BsonDocument filter, UpdateDefinition<BsonDocument> update, string collectionName,
        string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);
        var resp = collection.UpdateMany(filter, update);
        return resp.ModifiedCount;
    }

    public async Task UpdateBulkPartialAsync(BsonDocument filter, IDictionary<string, object> entity,
        string collectionName, string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        UpdateDefinition<BsonDocument> update = null;
        foreach (var (key, value) in entity)
        {
            if (update == null)
            {
                var builder = Builders<BsonDocument>.Update;
                update = builder.Set(key, value);
            }
            else
            {
                update = update.Set(key, value);
            }
        }

        await collection.UpdateManyAsync(filter, update);
    }

    public void UpdatePartial(FilterDefinition<BsonDocument> filter, IDictionary<string, object> entity,
        string collectionName, string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);


        UpdateDefinition<BsonDocument> update = null;
        foreach (var (key, value) in entity)
        {
            if (update == null)
            {
                var builder = Builders<BsonDocument>.Update;
                update = builder.Set(key, value);
            }
            else
            {
                update = update.Set(key, value);
            }
        }

        collection.UpdateOne(filter, update);
    }

    public async Task UpdatePartialAsync(FilterDefinition<BsonDocument> filter, IDictionary<string, object> entity,
        string collectionName, string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        UpdateDefinition<BsonDocument> update = null;
        foreach (var (key, value) in entity)
        {
            if (update == null)
            {
                var builder = Builders<BsonDocument>.Update;
                update = builder.Set(key, value);
            }
            else
            {
                update = update.Set(key, value);
            }
        }

        await collection.UpdateOneAsync(filter, update);
    }

    public void UpdateBulkPartial(FilterDefinition<BsonDocument> filter, IDictionary<string, object> entity,
        string collectionName, string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);


        UpdateDefinition<BsonDocument> update = null;
        foreach (var item in entity)
        {
            if (update == null)
            {
                var builder = Builders<BsonDocument>.Update;
                update = builder.Set(item.Key, item.Value);
            }
            else
            {
                update = update.Set(item.Key, item.Value);
            }
        }

        collection.UpdateMany(filter, update);
    }

    public async Task UpdateBulkPartialAsync(FilterDefinition<BsonDocument> filter, IDictionary<string, object> entity,
        string collectionName, string tenantId = "", bool isPlural = true)
    {
        collectionName = isPlural ? collectionName + "s" : collectionName;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        UpdateDefinition<BsonDocument> update = null;
        foreach (var (key, value) in entity)
        {
            if (update == null)
            {
                var builder = Builders<BsonDocument>.Update;
                update = builder.Set(key, value);
            }
            else
            {
                update = update.Set(key, value);
            }
        }

        await collection.UpdateManyAsync(filter, update);
    }

    #endregion

    #region Get

    public async Task<dynamic> GetItemAsync(string collectionName, string _id, bool isPlural = true,
        bool excludeDeleted = true)
    {
        if (isPlural)
        {
            collectionName += "s";
        }

        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<dynamic>(collectionName);

        var filterDefinition = GetFilterDefinition<dynamic>(_id, excludeDeleted);

        var find = await collection.FindAsync<dynamic>(filterDefinition);
        var data = find.FirstOrDefault();
        return data;
    }

    public dynamic GetItem(string collectionName, string _id, bool isPlural = true, bool excludeDeleted = true)
    {
        if (isPlural)
        {
            collectionName += "s";
        }

        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<dynamic>(collectionName);

        var filterDefinition = GetFilterDefinition<dynamic>(_id, excludeDeleted);

        var find = collection.FindSync<dynamic>(filterDefinition);
        var data = find.FirstOrDefault();
        return data;
    }


    public List<BsonDocument> GetBsonItems(string collectionName, FilterDefinition<BsonDocument> filter,
        BsonDocument projection, string tenantId = "", bool isPlural = true, bool excludeDeleted = true)
    {
        if (isPlural)
        {
            collectionName += "s";
        }

        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);
        var filterDefinition = GetFilterDefinition(filter, excludeDeleted);

        var find = collection.Aggregate().Match(filterDefinition).Project(p => projection);
        var data = find.ToList();
        return data;
    }

    public List<BsonDocument> GetBsonItemsByProjection(
        string collectionName,
        BsonDocument projection,
        FilterDefinition<BsonDocument> filter,
        string tenantId = "",
        bool isPlural = true,
        bool excludeDeleted = true)
    {
        if (isPlural)
        {
            collectionName += "s";
        }

        var mongoDatabase = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDatabase.GetCollection<BsonDocument>(collectionName);

        var filterDefinition = GetFilterDefinition(filter, excludeDeleted);

        return collection.Aggregate().Project(p => projection).Match(filterDefinition).ToList();
    }

    public IAggregateFluent<BsonDocument> GetBsonAggr(string collectionName, FilterDefinition<BsonDocument> filter,
        BsonDocument projection, BsonDocument sort, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true)
    {
        if (isPlural)
        {
            collectionName += "s";
        }

        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        var filterDefinition = GetFilterDefinition(filter, excludeDeleted);
        return collection.Aggregate().Match(filterDefinition).Project(p => projection).Sort(sort);
    }

    public IFindFluent<BsonDocument, BsonDocument> GetBsonData(string collectionName,
        FilterDefinition<BsonDocument> filter, BsonDocument projection, BsonDocument sort, string tenantId = "",
        bool isPlural = true, bool excludeDeleted = true)
    {
        if (isPlural)
        {
            collectionName += "s";
        }

        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        var filterDefinition = GetFilterDefinition(filter, excludeDeleted);
        return collection.Find(filterDefinition).Project(projection).Sort(sort);
    }

    public BsonDocument GetBsonItem(string collectionName, FilterDefinition<BsonDocument> filter,
        BsonDocument projection, string tenantId = "", bool isPlural = true, bool excludeDeleted = true)
    {
        if (isPlural)
        {
            collectionName += "s";
        }

        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<BsonDocument>(collectionName);

        var filterDefinition = GetFilterDefinition(filter, excludeDeleted);
        return collection.Aggregate().Match(filterDefinition).Project(p => projection).FirstOrDefault();
    }

    public async Task<T> GetItemAsync<T>(string _id, bool isPlural = true, bool excludeDeleted = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);

        var filterDefinition = GetFilterDefinition<T>(_id, excludeDeleted);
        var find = await collection.FindAsync<T>(filterDefinition);
        return find.FirstOrDefault();
    }

    public T GetItem<T>(string _id, bool isPlural = true, bool excludeDeleted = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);

        var filterDefinition = GetFilterDefinition<T>(_id, excludeDeleted);
        return collection.FindSync<T>(filterDefinition).FirstOrDefault();
    }

    public async Task<T?> GetItemAsync<T>(Expression<Func<T, bool>> filter, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<T>(collectionName);

        var filterDefinition = GetFilterDefinition(filter, excludeDeleted);
        var find = await collection.FindAsync<T>(filterDefinition);
        return find.FirstOrDefault();
    }

    public T GetItem<T>(Expression<Func<T, bool>> filter, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<T>(collectionName);

        var filterDefinition = GetFilterDefinition(filter, excludeDeleted);
        return collection.FindSync<T>(filterDefinition).FirstOrDefault();
    }

    public async Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> filter, string tenantId = "",
        bool isPlural = true, bool excludeDeleted = true)
    {
        try
        {
            var type = typeof(T);
            var collectionName = isPlural ? type.Name + "s" : type.Name;
            var mongoDb = string.IsNullOrWhiteSpace(tenantId)
                ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
                : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

            var collection = mongoDb.GetCollection<T>(collectionName);

            var filterDefinition = GetFilterDefinition(filter, excludeDeleted);
            var find = await collection.FindAsync<T>(filterDefinition);
            var data = find.ToList();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError($"RMW Db Repository error: {ex.Message}");
            return new List<T>();
        }
    }

    public async Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> filter, IMongoDatabase tenantDataContext, string tenantId = "")
    {
        try
        {
            var type = typeof(T);
            var collectionName = type.Name + "s";

            var mongoDb = tenantDataContext;

            var collection = mongoDb.GetCollection<T>(collectionName);

            var filterDefinition = GetFilterDefinition(filter, true);
            var find = await collection.FindAsync<T>(filterDefinition);
            var data = find.ToList();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError($"RMW Db Repository error: {ex.Message}");
            return new List<T>();
        }
    }

    public async Task<(List<TProjectedValue>, long)> GetItemsWithCountAsync<T, TProjectedValue>(
        FilterDefinition<T> filter, 
        SortDefinition<T> sort, 
        ProjectionDefinition<T> projection,
        int pageSize, 
        int pageNo, 
        bool excludeDeleted = true)
    {
        var collection = GetCollection<T>(string.Format("{0}s", typeof(T).Name));
        
        var find = collection.Find(filter);
            
        var count = await find.CountDocumentsAsync();
        var data = await find
            .Sort(sort)
            .Project(projection)
            .Skip(pageSize * pageNo)
            .Limit(pageSize)
            .ToListAsync();

        var projectionData = data
            .Select(d => BsonSerializer.Deserialize<TProjectedValue>(d))
            .ToList();

        return (projectionData, count);
    }

    public async Task<(List<T>, long)> GetItemsAsync<T>(FilterDefinition<T> filterDefinition, string tenantId = "",
        bool isPlural = true, bool excludeDeleted = true)
    {
        var type = typeof(T);
            var collectionName = isPlural ? type.Name + "s" : type.Name;
            var mongoDb = string.IsNullOrWhiteSpace(tenantId)
                ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
                : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

            var collection = mongoDb.GetCollection<T>(collectionName);
            
            var find = collection.Find(filterDefinition);
            
            var count = await find.CountDocumentsAsync();
            
            var data = find.ToList();
            
            return (data, count);
    }

    public List<T> GetItemsList<T>(Expression<Func<T, bool>> filter, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<T>(collectionName);

        var filterDefinition = GetFilterDefinition(filter, excludeDeleted);
        var find = collection.FindSync<T>(filterDefinition);
        var data = find.ToList();
        return data;
    }

    public IQueryable<T> GetQueryableItems<T>(Expression<Func<T, bool>> filter, string tenantId = "",
        bool isPlural = true, bool excludeDeleted = true)
        where T : EntityBase
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<T>(collectionName);

        return excludeDeleted
            ? collection.AsQueryable().Where(filter).Where(entity => entity.IsMarkedToDelete != true)
            : collection.AsQueryable().Where(filter);
    }

    public IQueryable<T> GetQueryableItems<T>(FilterDefinition<T> filter, string tenantId = "", bool isPlural = true,
        bool excludeDeleted = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = string.IsNullOrWhiteSpace(tenantId)
            ? _ecapMongoDbDataContextProvider.GetTenantDataContext()
            : _ecapMongoDbDataContextProvider.GetTenantDataContext(tenantId);

        var collection = mongoDb.GetCollection<T>(collectionName);

        var filterDefinition = GetFilterDefinition(filter, excludeDeleted);
        return collection.AsQueryable().Where(x => filterDefinition.Inject());
    }


    public IAggregateFluent<T> GetItems<T>(BsonDocument filter, bool isPlural = true, bool excludeDeleted = true)
        where T : EntityBase
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);
        return GetAggregateFluent(filter, excludeDeleted, collection);
    }

    public IAggregateFluent<TProjection> GetItems<TCollection, TProjection>(BsonDocument filter,
        BsonDocument projection = null, bool isPlural = true, bool excludeDeleted = true)
    {
        var type = typeof(TCollection);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<TCollection>(collectionName);
        return GetAggregateFluent(filter, excludeDeleted, collection).Project<TProjection>(projection);
    }

    public IAggregateFluent<TGroup> GetItemsGroup<TCollection, TGroup>(FilterDefinition<TCollection> filter,
        BsonDocument group, bool isPlural = true, bool excludeDeleted = true)
    {
        var type = typeof(TCollection);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<TCollection>(collectionName);
        return GetAggregateFluent(filter, excludeDeleted, collection).Group<TGroup>(group);
    }

    public List<T> GetItemsList<T>(BsonDocument filter, bool isPlural = true, bool excludeDeleted = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);
        return GetAggregateFluent(filter, excludeDeleted, collection).ToList();
    }

    public T GetItem<T>(BsonDocument filter, bool isPlural = true, bool excludeDeleted = true)
    {
        var type = typeof(T);
        var collectionName = isPlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);
        return GetAggregateFluent(filter, excludeDeleted, collection).FirstOrDefault();
    }

    private static IAggregateFluent<T> GetAggregateFluent<T>(BsonDocument filter, bool excludeDeleted,
        IMongoCollection<T> collection)
        => excludeDeleted
            ? collection.Aggregate().Match(filter)
                .Match(Builders<T>.Filter.Ne(nameof(EntityBase.IsMarkedToDelete), true))
            : collection.Aggregate().Match(filter);

    private static IAggregateFluent<T> GetAggregateFluent<T>(FilterDefinition<T> filter, bool excludeDeleted,
        IMongoCollection<T> collection)
        => excludeDeleted
            ? collection.Aggregate().Match(filter)
                .Match(Builders<T>.Filter.Ne(nameof(EntityBase.IsMarkedToDelete), true))
            : collection.Aggregate().Match(filter);

    private static FilterDefinition<T> GetFilterDefinition<T>(Expression<Func<T, bool>> filter, bool excludeDeleted)
        => Builders<T>.Filter.Where(filter) & (!excludeDeleted
            ? Builders<T>.Filter.Empty
            : Builders<T>.Filter.Ne(nameof(EntityBase.IsMarkedToDelete), true));

    private static FilterDefinition<TBsonDocument> GetFilterDefinition<TBsonDocument>(
        FilterDefinition<TBsonDocument> filter, bool excludeDeleted)
        => filter &
           (!excludeDeleted
               ? Builders<TBsonDocument>.Filter.Empty
               : Builders<TBsonDocument>.Filter.Ne(nameof(EntityBase.IsMarkedToDelete), true));

    private static FilterDefinition<T> GetFilterDefinition<T>(string _id, bool excludeDeleted)
        => excludeDeleted
            ? Builders<T>.Filter.Eq("_id", _id) & Builders<T>.Filter.Ne(nameof(EntityBase.IsMarkedToDelete), true)
            : Builders<T>.Filter.Eq("_id", _id);

    #endregion

    #region Get Item Count


    public async Task<long> GetItemCountAsync<T>(Expression<Func<T, bool>> filter)
    {
        var collection = GetCollection<T>(string.Format("{0}", typeof(T).Name));

        return await collection.CountDocumentsAsync(filter);
    }

    public async Task<long> GetItemCountAsync<T>(FilterDefinition<T> filter)
    {
        var collection = GetCollection<T>(string.Format("{0}", typeof(T).Name));

        return await collection.CountDocumentsAsync(filter);
    }


    #endregion


    #region Check if exist

    public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> filter
            , string tenantId = "")
    {
        var collection = GetCollection<T>(string.Format("{0}", typeof(T).Name), tenantId);
        var find = await collection.FindAsync<T>(filter);
        return find.Any();
    }

    public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> filter
            , bool isPlural , string tenantId = "" )
    {
        var collection = GetCollection<T>(string.Format("{0}s", typeof(T).Name), tenantId, isPlural);
        var find = await collection.FindAsync<T>(filter);
        return find.Any();
    }


    #endregion

    #region Delete

    public void DeleteMany<T>(Expression<Func<T, bool>> filter)
    {
        var type = typeof(T);
        //var collectionName = isPlural ? type.Name + "s" : type.Name;
        var collectionName = type.Name + "s";
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);
        var deleteResult = collection.DeleteMany<T>(filter);
        //var data = find.FirstOrDefault();
    }

    public async Task DeleteManyAsync<T>(Expression<Func<T, bool>> filter)
    {
        var type = typeof(T);
        //var collectionName = isPlural ? type.Name + "s" : type.Name;
        var collectionName = type.Name + "s";
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);
        var deleteResult = await collection.DeleteManyAsync<T>(filter);
    }

    public async Task<DeleteResult> DeleteManyAsync<T>(FilterDefinition<T> filter)
    {
        var type = typeof(T);
        var collectionName = type.Name + "s";
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);

        return await collection.DeleteManyAsync(filter);
    }

    #endregion

    #region Upsert

    public void Upsert<T>(T entity, bool isCollectionNamePlural = true) where T : EntityBase
    {
        var type = typeof(T);
        var collectionName = isCollectionNamePlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);

        var updates = new List<WriteModel<T>>();
        var securityContext = _securityContextProvider.GetSecurityContext();
        if (string.IsNullOrWhiteSpace(entity.ItemId))
        {
            return;
        }

        var filter = Builders<T>.Filter.Eq("_id", entity.ItemId);
        entity.LastUpdateDate = DateTime.UtcNow;
        entity.LastUpdatedBy = securityContext.UserId;

        var response = collection.ReplaceOne<T>(x => x.ItemId == entity.ItemId, entity,
            new ReplaceOptions { IsUpsert = true });
    }

    public async Task<string> UpsertAsync<T>(T entity, bool isCollectionNamePlural = true) where T : EntityBase
    {
        var type = typeof(T);
        var collectionName = isCollectionNamePlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);

        var updates = new List<WriteModel<T>>();
        var securityContext = _securityContextProvider.GetSecurityContext();

        if (string.IsNullOrWhiteSpace(entity.ItemId)) entity.ItemId = Guid.NewGuid().ToString();

        var filter = Builders<T>.Filter.Eq("_id", entity.ItemId);
        entity.LastUpdateDate = DateTime.UtcNow;
        entity.LastUpdatedBy = securityContext.UserId;

        var response = await collection.ReplaceOneAsync<T>(x => x.ItemId == entity.ItemId, entity,
            new ReplaceOptions { IsUpsert = true });

        return entity.ItemId;
    }

    public void UpsertMany<T>(IEnumerable<T> entities, bool isCollectionNamePlural = true) where T : EntityBase
    {
        var type = typeof(T);
        var collectionName = isCollectionNamePlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);

        var updates = new List<WriteModel<T>>();
        var securityContext = _securityContextProvider.GetSecurityContext();
        foreach (var doc in entities)
        {
            var filter = Builders<T>.Filter.Eq("_id", doc.ItemId);
            doc.LastUpdateDate = DateTime.UtcNow;
            doc.LastUpdatedBy = securityContext.UserId;
            var updateModel = new ReplaceOneModel<T>(filter, doc) { IsUpsert = true };
            updates.Add(updateModel);
        }

        if (updates.Count > 0)
        {
            var response = collection.BulkWrite(updates);
        }
    }

    public async Task UpsertManyAsync<T>(IEnumerable<T> entities, bool isCollectionNamePlural = true)
        where T : EntityBase
    {
        var type = typeof(T);
        var collectionName = isCollectionNamePlural ? type.Name + "s" : type.Name;
        var mongoDb = _ecapMongoDbDataContextProvider.GetTenantDataContext();
        var collection = mongoDb.GetCollection<T>(collectionName);

        var updates = new List<WriteModel<T>>();
        var securityContext = _securityContextProvider.GetSecurityContext();
        foreach (var doc in entities)
        {
            var filter = Builders<T>.Filter.Eq("_id", doc.ItemId);
            doc.LastUpdateDate = DateTime.UtcNow;
            doc.LastUpdatedBy = securityContext.UserId;
            var updateModel = new ReplaceOneModel<T>(filter, doc) { IsUpsert = true };
            updates.Add(updateModel);
        }

        if (updates.Count > 0)
        {
            var response = await collection.BulkWriteAsync(updates);
        }
    }
    
    public async Task UpsertManyAsync<T>(
        Expression<Func<T, bool>> filterExp, 
        IEnumerable<T> entities)
        where T : EntityBase
    {
        var collectionName = typeof(T).Name + "s";
        var collection = _ecapMongoDbDataContextProvider
            .GetTenantDataContext()
            .GetCollection<T>(collectionName);

        var updates = new List<WriteModel<T>>();
        var securityContext = _securityContextProvider.GetSecurityContext();
        
        foreach (var doc in entities)
        {
            doc.LastUpdateDate = DateTime.UtcNow;
            doc.LastUpdatedBy = securityContext.UserId;
            var updateModel = new ReplaceOneModel<T>(filterExp, doc) { IsUpsert = true };
            updates.Add(updateModel);
        }

        if (updates.Count > 0)
        {
            var response = await collection.BulkWriteAsync(updates);
        }
    }

    #endregion

    #endregion
}