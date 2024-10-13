using ArduinoWebAPI.Models;
using Dapper;
using System.Data;

namespace ArduinoWebAPI.Repository
{
    public class DeviceRepository
    {
        private readonly IDbConnection _dbConnection;
        public DeviceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> SaveHandshakeDetails(HandshakeDetails handshakeDetails)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@DeviceName", handshakeDetails.DeviceName);
            parameters.Add("@LocalIP", handshakeDetails.LocalIp);
            parameters.Add("@MacAddress", handshakeDetails.MacAddress);
            parameters.Add("@IsConnected", handshakeDetails.IsConnected);

            // Execute the stored procedure
            return await _dbConnection.ExecuteScalarAsync<int>("AddUpdateDevice", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
