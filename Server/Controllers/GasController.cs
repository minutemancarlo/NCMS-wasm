﻿using Microsoft.AspNetCore.Mvc;
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
                _logger.LogError($"Exception occurred while inserting transaction: {ex.Message}");
                return BadRequest($"Exception occurred while inserting transaction: {ex.Message}");
            }
        }


    }
}
