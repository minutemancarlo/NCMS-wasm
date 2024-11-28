using MudBlazor;
using NCMS_wasm.Server.Logger;
using NCMS_wasm.Server.Repository;
using OfficeOpenXml;
using static MudBlazor.CategoryTypes;

namespace NCMS_wasm.Server.BackgroundServices
{
    public class DTRProcessor : BackgroundService
    {
        private readonly string onQueuePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DTR", "Payslip", "OnQueue");
        private readonly string processingPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DTR", "Payslip", "Processing");
        private readonly string successPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DTR", "Payslip", "Success");
        private readonly string failedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DTR", "Payslip", "Failed");
        private readonly string template = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates");

        private readonly EmployeeRepository _employeeRepository;
        private readonly FileLogger _fileLogger;
        private string LogFileName = String.Empty;
        private string ModuleName = "DTR Processor";

        public DTRProcessor(IConfiguration configuration,EmployeeRepository employeeRepository)
        {
            _fileLogger = new FileLogger(configuration);
            _employeeRepository = employeeRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LogFileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
            _fileLogger.Log("DTR Processor Started.", LogFileName, ModuleName);
            while (!stoppingToken.IsCancellationRequested)
            {
                LogFileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
                CreateDirectories();
                await OnProcessDTR();
                await Task.Delay(10000, stoppingToken); // Runs every 10 seconds
            }
        }

        private async Task OnProcessDTR()
        {
            int? currentTask =0;
            string processingFilePath="", successFilePath="",failedFilePath = "";
            try
            {
                var tasks = await _employeeRepository.GetDTRForProcessAsync();
                if (tasks != null)
                {
                    foreach (var item in tasks)
                    {
                        await _employeeRepository.UpdateDTRTaskStatusAsync(Shared.DTRStatus.Processing, item.TaskId);
                        _fileLogger.Log($"Processing DTR Task Id: {item.TaskId}, Type: {item.TaskType}, Name: {item.TaskName}", LogFileName, ModuleName);
                        currentTask = item.TaskId;
                        // Get Individual DTR with Leave
                        if (item.TaskType.Contains("Individual"))
                        {
                            string[] parts = item.TaskName.Split('_');
                            string cutoffDate = parts[1];
                            string employeeId = parts[2];
                            var dtrList = await _employeeRepository.GetDTRIndividualAsync(cutoffDate, employeeId);

                            if (dtrList != null && dtrList.Any())
                            {
                                string templatePath = Path.Combine(template, "PayslipTemplate.xlsx");
                                processingFilePath = Path.Combine(processingPath, $"{item.TaskName}.xlsx");
                                successFilePath = Path.Combine(successPath, $"{item.TaskName}.xlsx");
                                failedFilePath = Path.Combine(failedPath, $"{item.TaskName}.xlsx");

                                // Copy the template to the processing folder
                                File.Copy(templatePath, processingFilePath, true);

                                var employeeInfo = await _employeeRepository.GetEmployeeInfoSingleAsync(employeeId);

                                // Use EPPlus to edit the Excel file
                                using (var package = new ExcelPackage(new FileInfo(processingFilePath)))
                                {
                                    // Update the first worksheet with employee info
                                    var summaryWorksheet = package.Workbook.Worksheets[0];
                                    summaryWorksheet.Cells["A2"].Value = employeeInfo.IDNumber;
                                    summaryWorksheet.Cells["B2"].Value = employeeInfo.Name;
                                    summaryWorksheet.Cells["C2"].Value = employeeInfo.Position;
                                    summaryWorksheet.Cells["E2"].Value = employeeInfo.Salary;

                                    // Calculate the payroll date
                                    string[] cutoffParts = cutoffDate.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                    string[] dateRange = cutoffParts[1].Split('-');
                                    int endDay = int.Parse(dateRange[1].Replace(",", ""));
                                    int payrollDay = endDay == 30 ? 10 : 25;
                                    DateTime payrollDate = new DateTime(
                                        int.Parse(cutoffParts[2]), // Year
                                        DateTime.ParseExact(cutoffParts[0], "MMMM", null).Month + 1, // Next month
                                        payrollDay);

                                    summaryWorksheet.Cells["D2"].Value = payrollDate.ToString("MM/dd/yyyy");

                                    // Add a new worksheet for individual DTR
                                    var dtrWorksheetName = $"{employeeId} - {employeeInfo.Name}";
                                    var dtrWorksheet = package.Workbook.Worksheets.Add(dtrWorksheetName);

                                    // Create headers for the new worksheet
                                    dtrWorksheet.Cells[1, 1].Value = "Cut Off Date";
                                    dtrWorksheet.Cells[1, 2].Value = "Shift Date";
                                    dtrWorksheet.Cells[1, 3].Value = "Time In";
                                    dtrWorksheet.Cells[1, 4].Value = "Time Out";
                                    dtrWorksheet.Cells[1, 5].Value = "Hours";
                                    dtrWorksheet.Cells[1, 6].Value = "Remarks";

                                    // Populate the rows
                                    int row = 2;
                                    foreach (var dtr in dtrList)
                                    {
                                        dtrWorksheet.Cells[row, 1].Value = cutoffDate;
                                        dtrWorksheet.Cells[row, 2].Value = dtr.ShiftDate?.ToString("MM/dd/yyyy");
                                        dtrWorksheet.Cells[row, 3].Value = dtr.TimeIn?.ToString("hh:mm tt");
                                        dtrWorksheet.Cells[row, 4].Value = dtr.TimeOut?.ToString("hh:mm tt");
                                        dtrWorksheet.Cells[row, 5].Formula = $"IF(AND(C{row}<>\"\",D{row}<>\"\"),(D{row}-C{row})*24,0)";
                                        dtrWorksheet.Cells[row, 5].Style.Numberformat.Format = "0.00";
                                        dtrWorksheet.Cells[row, 6].Value = "";
                                        row++;
                                    }

                                    // Calculate TotalNetPay
                                    dtrWorksheet.Cells[row, 5].Formula = $"SUM(E2:E{row - 1})"; // Total hours
                                    dtrWorksheet.Cells[row, 5].Style.Numberformat.Format = "0.00";
                                    summaryWorksheet.Cells["J2"].Formula = $"E2/240 * '{dtrWorksheetName}'!E{row}";
                                    summaryWorksheet.Cells["J2"].Style.Numberformat.Format = "0.00";
                                    // Format headers and auto-fit
                                    using (var range = dtrWorksheet.Cells[1, 1, 1, 6])
                                    {
                                        range.Style.Font.Bold = true;
                                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                                    }
                                    dtrWorksheet.Cells.AutoFitColumns();

                                    // Save the updated file
                                    package.Save();
                                }

                                // Move the file to the success folder
                                File.Move(processingFilePath, successFilePath, true);
                                await _employeeRepository.UpdateDTRTaskStatusAsync(Shared.DTRStatus.Processed, item.TaskId);

                                _fileLogger.Log($"Successfully processed and moved DTR Task Id: {item.TaskId} to Success folder.", LogFileName, ModuleName);
                            }
                            else
                            {
                                _fileLogger.Log($"No DTR data found for Task Id: {item.TaskId}.", LogFileName, ModuleName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"An Exception Occurred: {ex.Message}", LogFileName, ModuleName);
                File.Move(processingFilePath, failedFilePath, true);
                await _employeeRepository.UpdateDTRTaskStatusAsync(Shared.DTRStatus.Failed, currentTask);
            }
        }


        private void CreateDirectories()
        {
            if (!Directory.Exists(onQueuePath))
            {
                Directory.CreateDirectory(onQueuePath);
            }
            if (!Directory.Exists(processingPath))
            {
                Directory.CreateDirectory(processingPath);
            }
            if (!Directory.Exists(successPath))
            {
                Directory.CreateDirectory(successPath);
            }
            if (!Directory.Exists(failedPath))
            {
                Directory.CreateDirectory(failedPath);
            }
        }

    }
}
