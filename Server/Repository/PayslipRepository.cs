﻿using Dapper;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Repository
{
    public class PayslipRepository
    {
        private readonly IDbConnection _dbConnection;
        public PayslipRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }


        public async Task<int> AddPayslipData(PayslipModel info)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UploadId", info.UploadId);
            parameters.Add("@EmployeeId", info.EmployeeId);
            parameters.Add("@EmployeeName", info.EmployeeName);
            parameters.Add("@Position", info.Position);
            parameters.Add("@PayrollDate", info.PayrollDate);
            parameters.Add("@BasicSalary", info.BasicSalary);
            parameters.Add("@SSS", info.SSS);
            parameters.Add("@PagIbig", info.PagIbig);
            parameters.Add("@PHIC", info.PHIC);
            parameters.Add("@Tax", info.Tax);
            parameters.Add("@TotalNetPay", info.TotalNetPay);

            return await _dbConnection.ExecuteScalarAsync<int>("InsertPayslip", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<PayslipUpload>> GetPayslipsUploadsAsync()
        {
            string query = "SELECT * FROM PayslipUpload order by UploadedOn desc";
            return await _dbConnection.QueryAsync<PayslipUpload>(query);
        }

        public async Task<int> AddPayslipUploadAsync(string id, string filename)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UploadId", id);
            parameters.Add("@FileName", filename);


            string query = "INSERT into PayslipUpload (UploadId, FileName, PayslipFileStatus, UploadedOn) Values (@UploadId,@FileName,0,GETDATE());";

            return await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
        }

        public async Task<int> UpdatePayslipUploadAsync(string id,PayslipFileStatus PayslipFileStatus)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UploadId", id);
            parameters.Add("@PayslipFileStatus", PayslipFileStatus);


            string query = "Update PayslipUpload set PayslipFileStatus=@PayslipFileStatus where UploadId = @UploadId";

            return await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
        }

    }
}
