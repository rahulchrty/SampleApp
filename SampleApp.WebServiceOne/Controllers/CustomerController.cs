using Microsoft.AspNetCore.Mvc;

namespace SampleApp.PollyRetry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private ICounter _counterService;
        private IHttpClientFactory _clientFactory;
        public CustomerController(ICounter counterService, IHttpClientFactory clientFactory)
        {
            _counterService = counterService;
            _clientFactory = clientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                HttpClient selfClient = _clientFactory.CreateClient("self");
                var response = await selfClient.GetAsync("api/Customer/123");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                else
                {
                    throw new Exception("Error");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                _counterService.Increment();
                if (_counterService.GetCounterValue() > 2)
                {
                    await Task.Delay(0);
                    return Ok($"Bruce Wayne {_counterService.GetCounterValue()}");
                }
                else
                {
                    throw new Exception("Internal Server Error");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public interface ICounter
    {
        void Increment();
        int GetCounterValue();
    }
    public class Counter: ICounter
    {
        private int _requestCounter;
        public void Increment()
        {
            _requestCounter++;
        }
        public int GetCounterValue()
        {
            return _requestCounter;
        }
    }
}
