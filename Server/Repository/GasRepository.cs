using Dapper;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Repository
{
    public class GasRepository
    {
        private readonly IDbConnection _dbConnection;
        public GasRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<GasPrice>> GetAllGasPricesAsync()
        {                                          
            return await _dbConnection.QueryAsync<GasPrice>("Select * from GasPrices", null, commandType: CommandType.Text);
        }


    }
}
