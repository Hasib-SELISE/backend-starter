using Domain.Entities;
using MongoDB.Bson;

namespace Application.Common.Mappings;

public static class FeatureRoleMapper
{
    public static FeatureRoleMap MapToFeatureRoleMap(this BsonDocument source)
        => new FeatureRoleMap
        {
            AppName = source["AppName"].AsString,
            AppType = source["AppType"].AsString,
            FeatureId = source["FeatureId"].AsString,
            FeatureName = source["FeatureName"].AsString,
            RoleName = source["RoleName"].AsString,
            ItemId = source["ItemId"].IsObjectId
                ? source["ItemId"].AsObjectId.ToString()
                : source["ItemId"].AsString
        };

    public static BsonDocument MapToBson(this FeatureRoleMap source)
    {
        var dest = new BsonDocument
        {
            {"AppName", source.AppName},
            {"AppType", source.AppType},
            {"FeatureId", source.FeatureId},
            {"FeatureName", source.FeatureName},
            {"RoleName", source.RoleName}
        };

        if (Guid.TryParse(source.ItemId, out _))
        {
            dest.Add("_id", source.ItemId);
        }
        else
        {
            dest.Add("_id", ObjectId.Parse(source.ItemId));
        }

        return dest;
    }

}