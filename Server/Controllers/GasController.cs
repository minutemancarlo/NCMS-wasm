using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using NCMS_wasm.Server.Logger;
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
        private readonly FileLogger _fileLogger;
        private readonly string ModuleName; 
        public GasController(ILogger<GasController> logger, GasRepository gasRepository, IConfiguration configuration)
        {
            _logger = logger;
            _gasRepository = gasRepository;
            _fileLogger = new FileLogger(configuration); 
            ModuleName = "GasController";
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
                _fileLogger.Log($"Exception Occured in Endpoint [GetGasPrices]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                _logger.LogError($"Exception occurred while retrieving gas prices: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving gas prices: {ex.Message}");
            }
        }

        [HttpGet("GetSubTransactionsByDate")]
        public async Task<ActionResult<List<SubTransaction>>> GetSubTransactionsByDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var prices = await _gasRepository.GetSubTransactions(startDate, endDate);
                _logger.LogInformation("Gas Sub Transactions retrieved successfully.");
                return Ok(prices);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetSubTransactionsByDate]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                _logger.LogError($"Exception occurred while retrieving gas sub transactions: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving gas sub transactions: {ex.Message}");
            }
        }


        [HttpGet("GetGasTransactions")]
        public async Task<ActionResult<List<GasModel>>> GetGasTransactions()
        {
            try
            {
                var prices = await _gasRepository.GetTransactions();
                _logger.LogInformation("Gas Transactions retrieved successfully.");
                return Ok(prices);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetGasTransactions]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                _logger.LogError($"Exception occurred while retrieving gas transactions: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving gas transactions: {ex.Message}");
            }
        }

        [HttpGet("GetCardInfo")]
        public async Task<ActionResult<LoyaltyCardInfo>> GetCardInfo(string cardReference)
        {
            try
            {
                var cardinfo = await _gasRepository.GetCardInfoAsync(cardReference);
                cardinfo = cardinfo == null? new LoyaltyCardInfo() : cardinfo;
                _logger.LogInformation("Gas Transactions retrieved successfully.");
                return Ok(cardinfo);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetCardInfo]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                _logger.LogError($"Exception occurred while retrieving gas transactions: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving gas transactions: {ex.Message}");
            }
        }

        [HttpGet("GetDashboardValuePerSales")]
        public async Task<ActionResult<List<DashboardValuePerSales>>> GetDashboardValuePerSales(int year)
        {
            try
            {
                var sales = await _gasRepository.GetDashboardValuePerSalesAsync(year);
                _logger.LogInformation("Dashboard Value Per Sales retrieved successfully.");
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetDashboardValuePerSales]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                _logger.LogError($"Exception occurred while retrieving Dashboard Value Per Sales: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving Dashboard Value Per Sales: {ex.Message}");
            }
        }

        [HttpPost("InsertTransaction")]
        public async Task<ActionResult<string>> InsertTransaction(TransactionRequest request)
        {
            try
            {
                var invoiceNo = await _gasRepository.InsertTransactionAsync(request.Transaction);
                request.SubTransactions.ForEach(subTransaction => subTransaction.InvoiceNo = invoiceNo);
                foreach (var subTransaction in request.SubTransactions)
                {                 
                    await _gasRepository.InsertSubTransactionAsync(subTransaction);
                }



                _logger.LogInformation("Transaction inserted successfully with InvoiceNo: {InvoiceNo}", invoiceNo);
                return Ok(invoiceNo);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [InsertTransaction]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                _logger.LogError($"Exception occurred while inserting transaction: {ex.Message}");
                return BadRequest($"Exception occurred while inserting transaction: {ex.Message}");
            }
        }

        [HttpPost("UpdateGasPrice")]
        public async Task<ActionResult<int>> UpdateGasPrice(GasPrice gasPrice)
        {
            try
            {
                var id = await _gasRepository.UpdateGasPriceAsync(gasPrice);             
                _logger.LogInformation("Gas Info Updated!", id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [UpdateGasPrice]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                _logger.LogError($"Exception occurred while updating gas price: {ex.Message}");
                return BadRequest($"Exception occurred while updating gas price: {ex.Message}");
            }
        }

        [HttpPost("AddLoyaltyCardInfo")]
        public async Task<ActionResult<string>> AddLoyaltyCardInfo(LoyaltyCardInfo request)
        {
            try
            {
                var id = await _gasRepository.InsertLoyaltyCardInfoAsync(request);
                
                _logger.LogInformation("Loyalty card added", id);
                return Ok(id.ToString());
                
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [AddLoyaltyCardInfo]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                _logger.LogError($"Exception occurred while adding loyalty card: {ex.Message}");
                return BadRequest($"Exception occurred while adding loyalty card: {ex.Message}");
            }
        }


    }
}
