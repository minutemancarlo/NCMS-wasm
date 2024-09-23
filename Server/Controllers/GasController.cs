using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GasController : ControllerBase
    {
        private readonly ILogger<GasController> _logger;
        private readonly GasRepository _gasRepository;

        public GasController(ILogger<GasController> logger, GasRepository gasRepository)
        {
            _logger = logger;
            _gasRepository = gasRepository;
        }

        [HttpGet("GetGasPrices")]
        public async Task<ActionResult<List<GasPrice>>> GetGasPrices()
        {
            try
            {
                var prices = await _gasRepository.GetAllGasPricesAsync();
                _logger.LogInformation("Gas Prices retrieved successfully.");
                return Ok(prices);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while retrievinggas prices: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving gas prices: {ex.Message}");
            }
        }


    }
}
