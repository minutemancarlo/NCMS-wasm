using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Server.Logger;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CardManagementController : ControllerBase
    {
        private readonly ILogger<CardManagementController> _logger;
        private readonly CardRepository _cardRepository;
        private readonly FileLogger _fileLogger;

        public CardManagementController(ILogger<CardManagementController> logger, CardRepository cardRepository, IConfiguration configuration)
        {
            _logger = logger;
            _cardRepository = cardRepository;
            _fileLogger = new FileLogger(configuration);
        }

        [HttpGet("GetLoyaltyCards")]
        public async Task<ActionResult<List<LoyaltyCardInfo>>> GetLoyaltyCards(RFIDType cardType)
        {
            try
            {
                var cards = await _cardRepository.GetAllCardsAsync(cardType);
                _logger.LogInformation("Cards Retrieved");
                return Ok(cards);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetLoyaltyCards]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", "CardManagementController");
                _logger.LogError($"Exception occurred while retrieving cards: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving cards: {ex.Message}");
            }
        }

    }
}
