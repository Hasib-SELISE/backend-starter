using Application.Services;
using Application.Test.Queries;
using Microsoft.AspNetCore.Mvc;
using Selise.Ecap.Infrastructure;

namespace API.Controllers.Test
{
    public class TestsController : ControllerBase
    {
        private readonly IRmwStatisticalTestService _statisticalTestService;

        public TestsController(IRmwStatisticalTestService statisticalTestService)
        {
            _statisticalTestService = statisticalTestService;
        }



        #region GET

        [HttpGet]
        [AnyonomusEndPoint]
        public async Task<dynamic> GetData([FromBody] GetDataQuery query)
        {
            var response = await _statisticalTestService.GetAnyDataAsync(query);
            return Ok(response);
        }

        #endregion
    }
}
