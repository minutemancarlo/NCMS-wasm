﻿using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Server.Logger;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;
namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PayslipController : ControllerBase
    {
        private readonly ILogger<PayslipController> _logger;
        private readonly PayslipRepository _payslipRepository;
        private readonly FileLogger _fileLogger;
        private readonly string ModuleName;
        public PayslipController(ILogger<PayslipController> logger, PayslipRepository payslipRepository, IConfiguration configuration)
        {
            _logger = logger;
            _payslipRepository = payslipRepository;
            _fileLogger = new FileLogger(configuration);
            ModuleName = "PayslipController";
        }

        [HttpPost("GetPayslipsUploads")]
        public async Task<ActionResult<List<PayslipUpload>>> GetPayslipsUploads()
        {
            try
            {
                var devices = await _payslipRepository.GetPayslipsUploadsAsync();
                _logger.LogInformation("Payslip upload files retrieved successfully.");
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetPayslipsUploads]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                _logger.LogError($"Exception occurred while retrieving payslip upload files: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving payslip upload files: {ex.Message}");
            }
        }


        [HttpPost("GetMyPayslips")]
        public async Task<ActionResult<List<PayslipModel>>> GetMyPayslips([FromBody] string id)
        {
            try
            {
                var payslips = await _payslipRepository.GetMyPayslipsAsync(id);
                _logger.LogInformation("Payslip upload files retrieved successfully.");
                return Ok(payslips);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetMyPayslips]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                _logger.LogError($"Exception occurred while retrieving payslip upload files: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving payslip upload files: {ex.Message}");
            }
        }

        [HttpPost("UploadPayslipFile")]
        public async Task<IActionResult> UploadPayslipFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Payslip", "OnQueue");

            // Ensure the directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            
            var newGuid = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var newFileName = $"payslip_{newGuid}{extension}";            
            var filePath = Path.Combine(uploadPath, newFileName);
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                _ = await _payslipRepository.AddPayslipUploadAsync(newGuid.ToString(), newFileName);
                return Ok("File uploaded successfully.");
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [UploadPayslipFile]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
