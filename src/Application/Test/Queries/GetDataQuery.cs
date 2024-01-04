using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Test.Queries
{
    public class GetDataQuery
    {
        public string TenantId { get; set; } = "986A938D-81CA-4D87-8DAC-7AB1470BBC48";
        public string Match { get; set; }
        public string CollectionName { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int ThresholdValue { get; set; } = 40;
        public int SortOrder { get; set; }
        public string SortKey { get; set; }

        public GetDataQuery()
        {
            this.PageSize = 10;
        }
    }
}
