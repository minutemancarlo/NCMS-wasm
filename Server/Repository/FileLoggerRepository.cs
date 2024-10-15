using Dapper;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Repository
{
    public class FileLoggerRepository
    {
        private readonly IDbConnection _dbConnection;
        public FileLoggerRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> AddLog(FileLog log)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ModuleName", log.ModuleName);
            parameters.Add("@Message", log.Message);
            
            // Execute the stored procedure
            return await _dbConnection.ExecuteScalarAsync<int>("AddLog", parameters, commandType: CommandType.StoredProcedure);
        }

    }
}
