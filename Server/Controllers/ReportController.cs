using AspNetCore.Reporting;
using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Client.Pages.HR;
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

        public ReportController(IWebHostEnvironment webHostEnvironment, LeaveRequestService leaveRequestService,EmployeeService employeeService,ReceiptService receiptService )
        {
            _webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _leaveRequestService = leaveRequestService;
            _employeeService = employeeService;
            _receiptService = receiptService;
        }

        

        [HttpGet("GetLeaveRequestReport")]
        public async Task<IActionResult> GetLeaveRequestReport([FromQuery] LeaveReportFilter filter)
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

        [HttpGet("GetEmployeeInfoReport")]
        public async Task<IActionResult> GetEmployeeInfoReport([FromQuery] string idNumber)
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

        [HttpGet("GetPayslipReport")]
        public async Task<IActionResult> GetPayslipReport([FromQuery] MyPayslipFilter filter)
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

        [HttpGet("GetReceiptPrint")]
        public async Task<IActionResult> GetReceiptPrint([FromQuery] string invoiceNo)
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

    }
}
