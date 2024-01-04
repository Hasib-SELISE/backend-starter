using Application.Common.Models;
using Application.Test.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public interface IRmwStatisticalTestService
{
    Task<DynamicQueryResponseModel> GetAnyDataAsync(GetDataQuery query);
}
