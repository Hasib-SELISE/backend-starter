using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class DbActionModel
    {
        public bool IsSucess { get; private set; }
        public long DeletedCount { get; private set; }
        public long InsertedCount { get; private set; }
        public bool IsAcknowledged { get; private set; }
        public bool IsModifiedCountAvailable { get; private set; }
        public long MatchedCount { get; private set; }
        public long ModifiedCount { get; private set; }
        public int RequestCount { get; private set; }
        public IEnumerable<string> AffectedIds { get; private set; }
        public DbActionModel()
        {
            AffectedIds = new List<string>();
        }
        public void SetActionData(ReplaceOneResult result, string affectedId = "")
        {
            var affectedIds = new List<string>();
            if (!string.IsNullOrWhiteSpace(affectedId))
            {
                affectedIds.Add(affectedId);
            }
            SetActionData(result, affectedIds);
        }
        public void SetActionData(ReplaceOneResult result, IEnumerable<string> affectedIds)
        {
            MatchedCount = result.MatchedCount;
            ModifiedCount = result.ModifiedCount;
            AffectedIds = affectedIds;
            var upsertedId = result.UpsertedId?.ToString();

            if (ModifiedCount > 0 || !string.IsNullOrWhiteSpace(upsertedId))
            {
                IsSucess = true;
            }
        }
        public void SetActionData(BulkWriteResult result, string affectedId = "")
        {
            var affectedIds = new List<string>();
            if (!string.IsNullOrWhiteSpace(affectedId))
            {
                affectedIds.Add(affectedId);
            }
            SetActionData(result, affectedIds);
        }
        public void SetActionData(BulkWriteResult result, IEnumerable<string> affectedIds)
        {
            MatchedCount = result.MatchedCount;
            ModifiedCount = result.ModifiedCount;
            InsertedCount = result.InsertedCount;
            DeletedCount = result.DeletedCount;
            RequestCount = result.RequestCount;
            AffectedIds = affectedIds;
            if (ModifiedCount > 0 || InsertedCount > 0 || DeletedCount > 0)
            {
                IsSucess = true;
            }
        }
        public void SetActionData(UpdateResult result, string affectedId = "")
        {
            var affectedIds = new List<string>();
            if (!string.IsNullOrWhiteSpace(affectedId))
            {
                affectedIds.Add(affectedId);
            }
            SetActionData(result, affectedIds);
        }
        public void SetActionData(UpdateResult result, IEnumerable<string> affectedIds)
        {
            MatchedCount = result.MatchedCount;
            ModifiedCount = result.ModifiedCount;
            AffectedIds = affectedIds;
            var upsertedId = result.UpsertedId?.ToString();

            if (ModifiedCount > 0 || !string.IsNullOrWhiteSpace(upsertedId))
            {
                IsSucess = true;
            }
        }
        public void SetActionData(DeleteResult result, string affectedId = "")
        {
            var affectedIds = new List<string>();
            if (!string.IsNullOrWhiteSpace(affectedId))
            {
                affectedIds.Add(affectedId);
            }
            SetActionData(result, affectedIds);
        }
        public void SetActionData(DeleteResult result, IEnumerable<string> affectedIds)
        {
            DeletedCount = result.DeletedCount;
            AffectedIds = affectedIds;
            if (DeletedCount > 0)
            {
                IsSucess = true;
            }
        }

    }
}
