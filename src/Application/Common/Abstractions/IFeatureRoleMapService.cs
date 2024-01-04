using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Common.Abstractions;

public interface IFeatureRoleMapService
{
    FeatureRoleMap GetFeatureRoleMap(FilterDefinition<BsonDocument> filter);
    FeatureRoleMap GetFeatureRoleMap(BsonDocument filter);
    List<FeatureRoleMap> GetFeatureRoleMaps(FilterDefinition<BsonDocument> filter);
    List<FeatureRoleMap> GetFeatureRoleMaps(BsonDocument filter);
    FeatureRoleMap SaveFeatureRoleMap(FeatureRoleMap featureRoleMap);
    void SaveFeatureRoleMaps(List<FeatureRoleMap> featureRoleMaps);
    BsonArray GetFeatureIds(IEnumerable<string> ids);
    void DeleteFeatureRoleMaps(IEnumerable<FeatureRoleMap> featureRoleMaps);
    void DeleteFeatureRoleMaps(IEnumerable<string> featureRoleMaps);
}