using Microsoft.AspNetCore.Mvc;
using Sample.ElasticApm.Domain.Concrete;

namespace Sample.ElasticApm.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class SampleController : Controller
    {
        private readonly ISampleApplication _sampleApplication;

        public SampleController(ISampleApplication sampleApplication)
        {
            _sampleApplication = sampleApplication;
        }

        [HttpPost("elastic")]
        public IActionResult PostSampleElastic()
        {
            _sampleApplication.PostSampleElastic();

            return Ok(new { Result = "Data successfully registered with Elasticsearch" });
        }

        [HttpPost("exception")]
        public IActionResult PostException()
        {
            _sampleApplication.PostSampleException();

            return BadRequest();
        }

        [HttpPost("sql")]
        public IActionResult PostSampleSql()
        {
            _sampleApplication.PostDataSql();

            return Ok();
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            var result = _sampleApplication.GetAll();

            return Json(result);
        }

        [HttpGet("term")]
        public IActionResult GetByAllCondictions([FromQuery] string term)
        {
            var result = _sampleApplication.GetActorsAllCondition(term);

            return Json(result);
        }

        [HttpGet("aggregation")]
        public IActionResult GetActorsAggregation()
        {
            var result = _sampleApplication.GetActorsAggregation();

            return Json(result);
        }

        [HttpGet("call-google")]
        public IActionResult GetGoogle()
        {
            _sampleApplication.GetGoogle();

            return Ok();
        }

        [HttpGet("call-api")]
        public IActionResult GetApiTest()
        {
            _sampleApplication.GetApiTest();

            return Ok();
        }
    }
}
