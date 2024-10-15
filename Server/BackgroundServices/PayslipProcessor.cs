using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using OfficeOpenXml; 
using NCMS_wasm.Shared;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Server.Logger;

public class PayslipProcessor : BackgroundService
{
    private readonly string onQueuePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Payslip", "OnQueue");
    private readonly string processingPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Payslip", "Processing");
    private readonly string successPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Payslip", "Success");
    private readonly string failedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Payslip", "Failed");
    private readonly PayslipRepository _payslipRepository;
    private readonly FileLogger _fileLogger;
    private string LogFileName = String.Empty;
    private string ModuleName = "Payslip Processor";
    public PayslipProcessor(PayslipRepository payslipRepository, IConfiguration configuration)
    {
        _payslipRepository = payslipRepository;
        _fileLogger = new FileLogger(configuration);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       LogFileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
        _fileLogger.Log("Payslip Processor Started.", LogFileName,ModuleName);
        while (!stoppingToken.IsCancellationRequested)
        {
            LogFileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
            CreateDirectories();
            await ProcessFilesAsync();            
            await Task.Delay(10000, stoppingToken); // Runs every 10 seconds
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


    private async Task ProcessFilesAsync()
    {
        var uploadId = string.Empty;
        try
        {
            // Get the first file in the OnQueue folder
            var files = Directory.GetFiles(onQueuePath, "*.xlsx");
            if (files.Length == 0) return;
            var file = files[0]; // Pick the first file
            var fileName = Path.GetFileName(file);
            var destinationPath = Path.Combine(processingPath, fileName);
            uploadId = fileName.Split('_')[1];
            uploadId = uploadId.Split('.')[0];

            // Move the file to Processing
            File.Move(file, destinationPath);
            _fileLogger.Log($"File {fileName} moved to {destinationPath}", LogFileName,ModuleName);
            await _payslipRepository.UpdatePayslipUploadAsync(uploadId, PayslipFileStatus.Processing);
            _fileLogger.Log($"Upload ID: {uploadId} status changed to Processing", LogFileName,ModuleName);
            // Read the file and process it
            var payslipData = await ReadExcelFileAsync(destinationPath);
            int count = 0;          
            foreach (var payslip in payslipData)
            {
                payslip.UploadId = uploadId;
                await _payslipRepository.AddPayslipData(payslip);
                count++;
            }
            _fileLogger.Log($"Inserted {count} rows in file {fileName}", LogFileName,ModuleName);
            files = Directory.GetFiles(processingPath, "*.xlsx");            
            if (files.Length == 0) return;

             file = files[0];
             fileName = Path.GetFileName(file);
             destinationPath = Path.Combine(successPath, fileName);
            File.Move(file, destinationPath);
            _fileLogger.Log($"File {fileName} moved to {destinationPath}", LogFileName,ModuleName);

            await _payslipRepository.UpdatePayslipUploadAsync(uploadId, PayslipFileStatus.Success);
            _fileLogger.Log($"Upload ID: {uploadId} status changed to Success", LogFileName,ModuleName);
        }
        catch (Exception ex)
        {
            var files = Directory.GetFiles(processingPath, "*.xlsx");
            if (files.Length == 0) return;

            var file = files[0]; 
            var fileName = Path.GetFileName(file);
            var destinationPath = Path.Combine(failedPath, fileName);
            // Move the file to Processing
            File.Move(file, destinationPath);
            _fileLogger.Log($"File {fileName} moved to {destinationPath}", LogFileName,ModuleName);
            await _payslipRepository.UpdatePayslipUploadAsync(uploadId, PayslipFileStatus.Failed);
            _fileLogger.Log($"Upload ID: {uploadId} status changed to Failed", LogFileName,ModuleName);
            _fileLogger.Log($"An Exception Occured: {ex.Message}", LogFileName,ModuleName);
        }
        
    }

    private async Task<List<PayslipModel>> ReadExcelFileAsync(string filePath)
    {
        var payslips = new List<PayslipModel>();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        // Load the Excel file using EPPlus
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet

            // Iterate through the rows, skipping the header row
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var payslip = new PayslipModel
                {
                    EmployeeId = worksheet.Cells[row, 1].Text,
                    EmployeeName = worksheet.Cells[row, 2].Text,
                    Position = worksheet.Cells[row, 3].Text,
                    PayrollDate = DateTime.Parse(worksheet.Cells[row, 4].Text),
                    BasicSalary = decimal.Parse(worksheet.Cells[row, 5].Text),
                    SSS = decimal.Parse(worksheet.Cells[row, 6].Text),
                    PagIbig = decimal.Parse(worksheet.Cells[row, 7].Text),
                    PHIC = decimal.Parse(worksheet.Cells[row, 8].Text),
                    Tax = decimal.Parse(worksheet.Cells[row, 9].Text),
                    TotalNetPay = decimal.Parse(worksheet.Cells[row, 10].Text)
                };
                payslips.Add(payslip);
            }
        }

        return payslips;
    }
}
