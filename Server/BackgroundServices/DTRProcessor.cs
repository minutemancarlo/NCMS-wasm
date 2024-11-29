using MudBlazor;
using NCMS_wasm.Client.Pages.HR;
using NCMS_wasm.Server.Logger;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;
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
            int? currentTask = 0;
            string processingFilePath = "", successFilePath = "", failedFilePath = "";
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

                        // Get Individual DTR
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
                                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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

                                    // Parse the cutoff date range
                                    string[] cutoffPartsForDTR = cutoffDate.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                    string[] dateRangeForDTR = cutoffPartsForDTR[1].Split('-');
                                    int cutOffstartDay = int.Parse(dateRangeForDTR[0]);
                                    int cutOffendDay = int.Parse(dateRangeForDTR[1].Replace(",", ""));
                                    int year = int.Parse(cutoffPartsForDTR[2]);
                                    int month = DateTime.ParseExact(cutoffPartsForDTR[0], "MMMM", null).Month;

                                    // Loop through the date range and populate the ShiftDate
                                    DateTime startDate = new DateTime(year, month, cutOffstartDay);
                                    DateTime endDate = new DateTime(year, month, cutOffendDay);

                                    int row = 2; // Start from the second row in the worksheet
                                    while (startDate <= endDate)
                                    {
                                        // Check if the ShiftDate exists in the dtrList
                                        var dtrForDay = dtrList.FirstOrDefault(dtr => dtr.ShiftDate?.Date == startDate.Date);
                                        var leave = await _employeeRepository.GetLeavesAsync(startDate, EventsType.Leave, employeeId);
                                        var holidays = await _employeeRepository.GetLeavesAsync(startDate, EventsType.Holiday);
                                        // If ShiftDate exists in dtrList, populate the data
                                        if (dtrForDay != null)
                                        {
                                            dtrWorksheet.Cells[row, 1].Value = cutoffDate; // Set Cut Off Date
                                            dtrWorksheet.Cells[row, 2].Value = startDate.ToString("MM/dd/yyyy"); // Set Shift Date
                                            dtrWorksheet.Cells[row, 3].Value = dtrForDay.TimeIn?.ToString("hh:mm tt"); // Time In
                                            dtrWorksheet.Cells[row, 4].Value = dtrForDay.TimeOut?.ToString("hh:mm tt"); // Time Out
                                            dtrWorksheet.Cells[row, 5].Formula = $"IF(OR(C{row}=\"\", D{row}=\"\"), 0, IF(AND(TEXT(C{row},\"HH:mm\")<TEXT(\"12:00 PM\",\"HH:mm\"), TEXT(D{row},\"HH:mm\")>TEXT(\"01:00 PM\",\"HH:mm\")), (D{row}-C{row})*24-1, (D{row}-C{row})*24))";
                                            dtrWorksheet.Cells[row, 5].Style.Numberformat.Format = "0.00";                                            
                                        }
                                        else
                                        {
                                            // If ShiftDate does not exist in dtrList, just populate the date with empty values
                                            dtrWorksheet.Cells[row, 1].Value = cutoffDate; // Set Cut Off Date
                                            dtrWorksheet.Cells[row, 2].Value = startDate.ToString("MM/dd/yyyy"); // Set Shift Date
                                            dtrWorksheet.Cells[row, 3].Value = ""; // Time In
                                            dtrWorksheet.Cells[row, 4].Value = ""; // Time Out
                                            dtrWorksheet.Cells[row, 5].Formula = $"IF(OR(C{row}=\"\", D{row}=\"\"), 0, IF(AND(TEXT(C{row},\"HH:mm\")<TEXT(\"12:00 PM\",\"HH:mm\"), TEXT(D{row},\"HH:mm\")>TEXT(\"01:00 PM\",\"HH:mm\")), (D{row}-C{row})*24-1, (D{row}-C{row})*24))";
                                            dtrWorksheet.Cells[row, 5].Style.Numberformat.Format = "0.00";                                            
                                        }

                                        if(leave is not null)
                                        {
                                            dtrWorksheet.Cells[row, 3].Value = "8:00 AM"; // Time In
                                            dtrWorksheet.Cells[row, 4].Value = "5:00 PM"; // Time Out
                                            dtrWorksheet.Cells[row, 6].Value = leave.LeaveType is null ? "" : leave.LeaveType;
                                            if (holidays is not null)
                                            {
                                                dtrWorksheet.Cells[row, 3].Value = "8:00 AM"; // Time In
                                                dtrWorksheet.Cells[row, 4].Value = "5:00 PM"; // Time Out
                                                dtrWorksheet.Cells[row, 6].Value = holidays.LeaveType is null ? "" : holidays.LeaveType;
                                            }                                                                                        
                                        }
                                        else
                                        {
                                            dtrWorksheet.Cells[row, 6].Value = "";
                                            if (holidays is not null)
                                            {
                                                dtrWorksheet.Cells[row, 3].Value = "8:00 AM"; // Time In
                                                dtrWorksheet.Cells[row, 4].Value = "5:00 PM"; // Time Out
                                                dtrWorksheet.Cells[row, 6].Value = holidays.LeaveType is null ? "" : holidays.LeaveType;
                                            }
                                        }

                                        // Move to the next row
                                        row++;

                                        // Increment the date by 1 day
                                        startDate = startDate.AddDays(1);
                                    }

                                    // Calculate TotalNetPay
                                    dtrWorksheet.Cells[row, 5].Formula = $"SUM(E2:E{row - 1})"; // Total hours
                                    dtrWorksheet.Cells[row, 5].Style.Numberformat.Format = "0.00";
                                    summaryWorksheet.Cells["J2"].Formula = $"E2/240 * '{dtrWorksheetName}'!E{row} - F2 - G2 - H2 - I2"; // Subtract F to I Rows for TotalNetPay
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
                        else
                        {
                            // Process All DTR
                            string[] parts = item.TaskName.Split('_');
                            string cutoffDate = parts[1];

                            // Get Grouped employee id
                            var dtrList = await _employeeRepository.GetDTRAllAsync(cutoffDate);

                            string? currentEmployee = "";
                            int summaryRow = 2;

                            if (dtrList != null && dtrList.Any())
                            {
                                string templatePath = Path.Combine(template, "PayslipTemplate.xlsx");
                                processingFilePath = Path.Combine(processingPath, $"{item.TaskName}.xlsx");
                                successFilePath = Path.Combine(successPath, $"{item.TaskName}.xlsx");

                                File.Copy(templatePath, processingFilePath, true);
                                Employee? employeeInfo = new();
                                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                using (var package = new ExcelPackage(new FileInfo(processingFilePath)))
                                {
                                    var summaryWorksheet = package.Workbook.Worksheets[0]; // Summary worksheet

                                    foreach (var dtrItem in dtrList)
                                    {
                                        if (currentEmployee == "" || currentEmployee != dtrItem.IDNumber)
                                        {
                                            employeeInfo = await _employeeRepository.GetEmployeeInfoSingleAsync(dtrItem.IDNumber);
                                            currentEmployee = employeeInfo?.IDNumber ?? "";
                                            if (employeeInfo is not null)
                                            {
                                                // Update summary worksheet for current employee
                                                summaryWorksheet.Cells[summaryRow, 1].Value = employeeInfo.IDNumber;
                                                summaryWorksheet.Cells[summaryRow, 2].Value = employeeInfo.Name;
                                                summaryWorksheet.Cells[summaryRow, 3].Value = employeeInfo.Position;
                                                summaryWorksheet.Cells[summaryRow, 4].Value = employeeInfo.Salary;

                                                // Calculate payroll date
                                                string[] cutoffParts = cutoffDate.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                                string[] dateRange = cutoffParts[1].Split('-');
                                                int endDay = int.Parse(dateRange[1].Replace(",", ""));
                                                int payrollDay = endDay == 30 ? 10 : 25;
                                                DateTime payrollDate = new DateTime(
                                                    int.Parse(cutoffParts[2]),
                                                    DateTime.ParseExact(cutoffParts[0], "MMMM", null).Month + 1,
                                                    payrollDay);
                                                summaryWorksheet.Cells[summaryRow, 5].Value = payrollDate.ToString("MM/dd/yyyy");

                                                // Create new worksheet for this employee's DTR
                                                var dtrWorksheetName = $"{currentEmployee} - {employeeInfo.Name}";
                                                var dtrWorksheet = package.Workbook.Worksheets.Add(dtrWorksheetName);

                                                // Create headers for the DTR worksheet
                                                dtrWorksheet.Cells[1, 1].Value = "Cut Off Date";
                                                dtrWorksheet.Cells[1, 2].Value = "Shift Date";
                                                dtrWorksheet.Cells[1, 3].Value = "Time In";
                                                dtrWorksheet.Cells[1, 4].Value = "Time Out";
                                                dtrWorksheet.Cells[1, 5].Value = "Hours";
                                                dtrWorksheet.Cells[1, 6].Value = "Remarks";

                                                // Parse cutoff date range
                                                string[] dateRangeForDTR = cutoffParts[1].Split('-');
                                                int cutOffStartDay = int.Parse(dateRangeForDTR[0]);
                                                int cutOffEndDay = int.Parse(dateRangeForDTR[1].Replace(",", ""));
                                                int year = int.Parse(cutoffParts[2]);
                                                int month = DateTime.ParseExact(cutoffParts[0], "MMMM", null).Month;

                                                DateTime startDate = new DateTime(year, month, cutOffStartDay);
                                                DateTime endDate = new DateTime(year, month, cutOffEndDay);

                                                int row = 2; // Start from the second row in the DTR worksheet
                                                while (startDate <= endDate)
                                                {
                                                    var dtrForDay = dtrList.FirstOrDefault(dtr => dtr.IDNumber == currentEmployee && dtr.ShiftDate?.Date == startDate.Date);
                                                    var leave = await _employeeRepository.GetLeavesAsync(startDate, EventsType.Leave, currentEmployee);
                                                    var holidays = await _employeeRepository.GetLeavesAsync(startDate, EventsType.Holiday);

                                                    if (dtrForDay != null)
                                                    {
                                                        dtrWorksheet.Cells[row, 1].Value = cutoffDate;
                                                        dtrWorksheet.Cells[row, 2].Value = startDate.ToString("MM/dd/yyyy");
                                                        dtrWorksheet.Cells[row, 3].Value = dtrForDay.TimeIn?.ToString("hh:mm tt");
                                                        dtrWorksheet.Cells[row, 4].Value = dtrForDay.TimeOut?.ToString("hh:mm tt");
                                                        dtrWorksheet.Cells[row, 5].Formula = $"IF(OR(C{row}=\"\", D{row}=\"\"), 0, IF(AND(TEXT(C{row},\"HH:mm\")<TEXT(\"12:00 PM\",\"HH:mm\"), TEXT(D{row},\"HH:mm\")>TEXT(\"01:00 PM\",\"HH:mm\")), (D{row}-C{row})*24-1, (D{row}-C{row})*24))";
                                                        dtrWorksheet.Cells[row, 5].Style.Numberformat.Format = "0.00";
                                                    }
                                                    else
                                                    {
                                                        dtrWorksheet.Cells[row, 1].Value = cutoffDate;
                                                        dtrWorksheet.Cells[row, 2].Value = startDate.ToString("MM/dd/yyyy");
                                                        dtrWorksheet.Cells[row, 3].Value = "";
                                                        dtrWorksheet.Cells[row, 4].Value = "";
                                                        dtrWorksheet.Cells[row, 5].Formula = $"IF(OR(C{row}=\"\", D{row}=\"\"), 0, IF(AND(TEXT(C{row},\"HH:mm\")<TEXT(\"12:00 PM\",\"HH:mm\"), TEXT(D{row},\"HH:mm\")>TEXT(\"01:00 PM\",\"HH:mm\")), (D{row}-C{row})*24-1, (D{row}-C{row})*24))";
                                                        dtrWorksheet.Cells[row, 5].Style.Numberformat.Format = "0.00";
                                                    }

                                                    if (leave is not null)
                                                    {
                                                        dtrWorksheet.Cells[row, 3].Value = "8:00 AM"; // Time In
                                                        dtrWorksheet.Cells[row, 4].Value = "5:00 PM"; // Time Out
                                                        dtrWorksheet.Cells[row, 6].Value = leave.LeaveType is null ? "" : leave.LeaveType;
                                                        if (holidays is not null)
                                                        {
                                                            dtrWorksheet.Cells[row, 3].Value = "8:00 AM"; // Time In
                                                            dtrWorksheet.Cells[row, 4].Value = "5:00 PM"; // Time Out
                                                            dtrWorksheet.Cells[row, 6].Value = holidays.LeaveType is null ? "" : holidays.LeaveType;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dtrWorksheet.Cells[row, 6].Value = "";
                                                        if (holidays is not null)
                                                        {
                                                            dtrWorksheet.Cells[row, 3].Value = "8:00 AM"; // Time In
                                                            dtrWorksheet.Cells[row, 4].Value = "5:00 PM"; // Time Out
                                                            dtrWorksheet.Cells[row, 6].Value = holidays.LeaveType is null ? "" : holidays.LeaveType;
                                                        }
                                                    }

                                                    row++;
                                                    startDate = startDate.AddDays(1);
                                                }

                                                dtrWorksheet.Cells[row, 5].Formula = $"SUM(E2:E{row - 1})"; // Total hours
                                                dtrWorksheet.Cells[row, 5].Style.Numberformat.Format = "0.00";

                                                // Update summary formula for Net Pay with corrected references
                                                summaryWorksheet.Cells[summaryRow, 10].Formula = $"{employeeInfo.Salary}/240 * '{dtrWorksheetName}'!E{row} - SUM(F{summaryRow}:I{summaryRow}) ";
                                                summaryWorksheet.Cells[summaryRow, 10].Style.Numberformat.Format = "0.00";

                                                using (var range = dtrWorksheet.Cells[1, 1, 1, 6])
                                                {
                                                    range.Style.Font.Bold = true;
                                                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                                                }
                                                dtrWorksheet.Cells.AutoFitColumns();
                                            }
                                            
                                            summaryRow++;
                                        }
                                    }

                                    package.Save();
                                }

                                File.Move(processingFilePath, successFilePath, true);
                                await _employeeRepository.UpdateDTRTaskStatusAsync(Shared.DTRStatus.Processed, item.TaskId);
                                _fileLogger.Log($"Successfully processed and moved DTR Task Id: {item.TaskId} to Success folder.", LogFileName, ModuleName);
                            }


                        } //End else process all

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
