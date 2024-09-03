using AspNetCore.Reporting;
using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Client.Pages.HR;
using NCMS_wasm.Server.Services;
using System.Data;
using System.Text;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly EmployeeService _employeeService = new EmployeeService();
        public ReportController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [HttpGet("GetReport")]        
        public IActionResult GetReport(int reportType)
        {
            var dt = new DataTable();
            dt = _employeeService.GetEmployees();
            string mimeType = "";
            int extenstion = 1;
            //var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\Report1.rdlc";
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "Report1.rdlc");
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
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("param", "Sample Report");
            LocalReport localReport = new(path);            
            localReport.AddDataSource(dataSetName:"dsEmployee", dt);

            if(reportType == 1)
            {
                var result = localReport.Execute(RenderType.Pdf, extenstion, parameter, mimeType);
                return File(result.MainStream, "application/pdf");
            }
            else
            {
                var result = localReport.Execute(RenderType.Excel, extenstion, parameter, mimeType);
                return File(result.MainStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }

        }

    }
}
