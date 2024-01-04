using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class FeatureRoleMap
{
    [BsonId]
    public string ItemId { get; set; }
    public string AppType { get; set; }
    public string AppName { get; set; }
    public string FeatureId { get; set; }
    public string FeatureName { get; set; }
    public string RoleName { get; set; }
}