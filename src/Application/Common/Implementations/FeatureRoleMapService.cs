using Application.Common.Abstractions;
using Application.Common.Mappings;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Common.Implementations;

public class FeatureRoleMapService : IFeatureRoleMapService
    {
        private readonly IRmwRepository _repo;

        public FeatureRoleMapService(IRmwRepository repo)
        {
            _repo = repo;
        }

        public FeatureRoleMap SaveFeatureRoleMap(FeatureRoleMap featureRoleMap)
        {
            var bson = featureRoleMap.MapToBson();
            _repo.Update(bson, "FeatureRoleMap");
            return featureRoleMap;
        }
        public void SaveFeatureRoleMaps(List<FeatureRoleMap> featureRoleMaps)
        {
            var bsonDocuments = featureRoleMaps.Select(item => item.MapToBson());

            _repo.UpdateMany(bsonDocuments, "FeatureRoleMap");
        }
        public void DeleteFeatureRoleMaps(IEnumerable<FeatureRoleMap> featureRoleMaps)
        {
            var ids = featureRoleMaps.Select(x => x.ItemId).ToList();
            if (ids.Any())
            {
                DeleteFeatureRoleMaps(ids);
            }
        }
        public void DeleteFeatureRoleMaps(IEnumerable<string> featureRoleMaps)
        {
            var objectIds = new List<ObjectId>();
            var guidIds = new List<string>();
            foreach (var item in featureRoleMaps)
            {
                if (Guid.TryParse(item, out _))
                {
                    guidIds.Add(item);
                }
                else
                {
                    objectIds.Add(ObjectId.Parse(item));
                    //new BsonDocument("_id", ObjectId.Parse(item.ItemId));
                }
            }

            if (guidIds.Count > 0)
            {
                var filter = new BsonDocument { { "_id", new BsonDocument { { "$in", new BsonArray(guidIds) } } } };
                _repo.DeleteMany(filter, "FeatureRoleMap");
            }

            if (guidIds.Count > 0)
            {
                var filter = new BsonDocument { { "_id", new BsonDocument { { "$in", new BsonArray(objectIds) } } } };
                _repo.DeleteMany(filter, "FeatureRoleMap");
            }
        }

        public FeatureRoleMap GetFeatureRoleMap(FilterDefinition<BsonDocument> filter)
        {
            FeatureRoleMap roleMap = null;
            var projection = new Dictionary<string, object>
                                        {
                                            { "ItemId", "$_id" },
                                            { "AppType", 1 },
                                            { "AppName", 1 },
                                            { "FeatureId", 1 },
                                            { "FeatureName", 1 },
                                            { "RoleName", 1 }
                                        };
            var data = _repo.GetBsonItem("FeatureRoleMap", filter, new BsonDocument(projection));
            if (data != null)
            {
                //custom Mapper
                roleMap = data.MapToFeatureRoleMap();//MongoDB.Bson.Serialization.BsonSerializer.Deserialize<FeatureRoleMap>(data);
            }
            return roleMap;
        }
        public FeatureRoleMap GetFeatureRoleMap(BsonDocument filter)
        {
            FeatureRoleMap roleMap = null;
            var projection = new Dictionary<string, object>
                                        {
                                            { "ItemId", "$_id" },
                                            { "AppType", 1 },
                                            { "AppName", 1 },
                                            { "FeatureId", 1 },
                                            { "FeatureName", 1 },
                                            { "RoleName", 1 }
                                        };
            var data = _repo.GetBsonItem("FeatureRoleMap", filter, new BsonDocument(projection));
            if (data != null)
            {
                //custom Mapper
                roleMap = data.MapToFeatureRoleMap();
                //MongoDB.Bson.Serialization.BsonSerializer.Deserialize<FeatureRoleMap>(data);
            }
            return roleMap;
        }

        public BsonArray GetFeatureIds(IEnumerable<string> ids)
        {
            var arr = new BsonArray();
            foreach (var id in ids)
            {
                if (Guid.TryParse(id, out _))
                {
                    arr.Add(id);
                }
                else
                {
                    arr.Add(ObjectId.Parse(id));
                }
            }
            return arr;
        }
        public List<FeatureRoleMap> GetFeatureRoleMaps(FilterDefinition<BsonDocument> filter)
        {
            var roleMaps = new List<FeatureRoleMap>();
            var projection = new Dictionary<string, object>
                                        {
                                            { "ItemId", "$_id" },
                                            { "AppType", 1 },
                                            { "AppName", 1 },
                                            { "FeatureId", 1 },
                                            { "FeatureName", 1 },
                                            { "RoleName", 1 }
                                        };
            var data = _repo.GetBsonItems("FeatureRoleMap", filter, new BsonDocument(projection));
            if (data != null)
            {
                //custom Mapper
                roleMaps.AddRange(data.Select(item => item.MapToFeatureRoleMap()));
                //MongoDB.Bson.Serialization.BsonSerializer.Deserialize<FeatureRoleMap>(data);
            }
            return roleMaps;
        }
        public List<FeatureRoleMap> GetFeatureRoleMaps(BsonDocument filter)
        {
            var roleMaps = new List<FeatureRoleMap>();
            var projection = new Dictionary<string, object>
                                        {
                                            { "ItemId", "$_id" },
                                            { "AppType", 1 },
                                            { "AppName", 1 },
                                            { "FeatureId", 1 },
                                            { "FeatureName", 1 },
                                            { "RoleName", 1 }
                                        };
            var data = _repo.GetBsonItems("FeatureRoleMap", filter, new BsonDocument(projection));
            if (data != null)
            {
                //custom Mapper
                roleMaps.AddRange(data.Select(item => item.MapToFeatureRoleMap()));
            }
            return roleMaps;
        }
    }