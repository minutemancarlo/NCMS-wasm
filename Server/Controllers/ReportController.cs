using AspNetCore.Reporting;
using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Client.Pages.HR;
using NCMS_wasm.Server.Logger;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Server.Services;
using NCMS_wasm.Shared;
using System.Data;
using System.Text;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly EmployeeService _employeeService;
        private readonly LeaveRequestService _leaveRequestService;
        private readonly ReceiptService _receiptService;
        private readonly FileLogger _fileLogger;
        private readonly string ModuleName;
        public ReportController(IWebHostEnvironment webHostEnvironment, LeaveRequestService leaveRequestService,EmployeeService employeeService,ReceiptService receiptService, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _leaveRequestService = leaveRequestService;
            _employeeService = employeeService;
            _receiptService = receiptService;
            _fileLogger = new FileLogger(configuration);
            ModuleName = "ReportController";
        }

        

        [HttpGet("GetLeaveRequestReport")]
        public async Task<IActionResult> GetLeaveRequestReport([FromQuery] LeaveReportFilter filter)
        {
            try
            {
                var dt = new DataTable();
                dt = await _leaveRequestService.GetLeaveRequestsAsync(filter);

                string mimeType = "";
                int extenstion = 1;
                //var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\Report1.rdlc";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "LeaveRequest.rdlc");
                //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "Reports", "Report1.rdlc");

                if (!System.IO.File.Exists(path))
                {
                    Console.WriteLine("File not found.");
                    return NotFound("Report file not found.");
                }
                else
                {
                    Console.WriteLine("File exists");
                }
                LocalReport localReport = new(path);
                localReport.AddDataSource(dataSetName: "dsLeaveRequest", dt);
                var result = localReport.Execute(RenderType.Pdf);

                var pdfStream = new MemoryStream(result.MainStream);
                var timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
                var fileName = $"LeaveRequestReport_{timestamp}.pdf";
                mimeType = "application/pdf";

                return new FileStreamResult(pdfStream, mimeType)
                {
                    FileDownloadName = fileName
                };
            }
            catch(Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetLeaveRequestReport]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return StatusCode(500, "An error occurred while generating the report.");
            }          
        }

        [HttpGet("GetEmployeeInfoReport")]
        public async Task<IActionResult> GetEmployeeInfoReport([FromQuery] string idNumber)
        {
            try
            {
                var dt = new DataTable();
                dt = await _employeeService.GetEmployeeInfoAsync(idNumber);

                string mimeType = "";
                int extenstion = 1;
                //var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\Report1.rdlc";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "EmployeeInfoSingle.rdlc");
                //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "Reports", "Report1.rdlc");

                if (!System.IO.File.Exists(path))
                {
                    Console.WriteLine("File not found.");
                    return NotFound("Report file not found.");
                }
                else
                {
                    Console.WriteLine("File exists");
                }
                LocalReport localReport = new(path);
                localReport.AddDataSource(dataSetName: "EmployeeInfoSingle", dt);
                var result = localReport.Execute(RenderType.Pdf);

                var pdfStream = new MemoryStream(result.MainStream);
                var fileName = $"EmployeeInfoReport_{idNumber}.pdf";
                mimeType = "application/pdf";

                return new FileStreamResult(pdfStream, mimeType)
                {
                    FileDownloadName = fileName
                };
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetEmployeeInfoReport]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return StatusCode(500, "An error occurred while generating the report.");
            }
        }

        [HttpGet("GetPayslipReport")]
        public async Task<IActionResult> GetPayslipReport([FromQuery] MyPayslipFilter filter)
        {
            try
            {
                var dt = new DataTable();
                dt = await _employeeService.GetMyPayslipAsync(filter.PayslipId);

                string mimeType = "";
                int extenstion = 1;

                var path = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "Payslip.rdlc");

                if (!System.IO.File.Exists(path))
                {
                    Console.WriteLine("File not found.");
                    return NotFound("Report file not found.");
                }
                else
                {
                    Console.WriteLine("File exists");
                }
                LocalReport localReport = new(path);
                localReport.AddDataSource(dataSetName: "Payslip", dt);
                var result = localReport.Execute(RenderType.Pdf);

                var pdfStream = new MemoryStream(result.MainStream);
                var timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
                var fileName = $"Payslip_{filter.EmployeeId}_{timestamp}.pdf";
                mimeType = "application/pdf";

                return new FileStreamResult(pdfStream, mimeType)
                {
                    FileDownloadName = fileName
                };
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetPayslipReport]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return StatusCode(500, "An error occurred while generating the report.");
            }
           
        }

        [HttpGet("GetReceiptPrint")]
        public async Task<IActionResult> GetReceiptPrint([FromQuery] string invoiceNo)
        {
            try
            {
                var dt = new DataTable();
                dt = await _receiptService.GetReceiptTransactionAsync(invoiceNo);
                string mimeType = "";
                int extenstion = 1;
                //var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\Report1.rdlc";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "Receipt.rdlc");
                //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "Reports", "Report1.rdlc");

                if (!System.IO.File.Exists(path))
                {
                    Console.WriteLine("File not found.");
                    return NotFound("Report file not found.");
                }
                else
                {
                    Console.WriteLine("File exists");
                }
                LocalReport localReport = new(path);
                localReport.AddDataSource(dataSetName: "dsReceipt", dt);
                var result = localReport.Execute(RenderType.Pdf);

                var pdfStream = new MemoryStream(result.MainStream);
                var timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
                var fileName = $"Receipt_{timestamp}.pdf";
                mimeType = "application/pdf";

                return new FileStreamResult(pdfStream, mimeType)
                {
                    FileDownloadName = fileName
                };
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetReceiptPrint]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return StatusCode(500, "An error occurred while generating the report.");
            }           
        }

        [HttpGet("GetReceiptPrintBooking")]
        public async Task<IActionResult> GetReceiptPrintBooking([FromQuery] string invoiceNo)
        {
            try
            {
                var dt = new DataTable();
                dt = await _receiptService.GetReceiptBookingAsync(invoiceNo);
                string mimeType = "";
                int extenstion = 1;
                //var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\Report1.rdlc";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "ReceiptBooking.rdlc");
                //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "Reports", "Report1.rdlc");

                if (!System.IO.File.Exists(path))
                {
                    Console.WriteLine("File not found.");
                    return NotFound("Report file not found.");
                }
                else
                {
                    Console.WriteLine("File exists");
                }
                LocalReport localReport = new(path);
                localReport.AddDataSource(dataSetName: "dsReceiptBooking", dt);
                var result = localReport.Execute(RenderType.Pdf);

                var pdfStream = new MemoryStream(result.MainStream);
                var timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
                var fileName = $"Receipt_{invoiceNo}_{timestamp}.pdf";
                mimeType = "application/pdf";

                return new FileStreamResult(pdfStream, mimeType)
                {
                    FileDownloadName = fileName
                };
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetReceiptPrintBooking]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", ModuleName);
                return StatusCode(500, "An error occurred while generating the report.");
            }
        }

    }
}
