using Dapper;
using System.Data;
using NCMS_wasm.Shared;

namespace NCMS_wasm.Server.Repository
{
    public class DeviceRepository
    {
        private readonly IDbConnection _dbConnection;
        public DeviceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Device>> GetAllDevicesAsync()
        {
            string query = "SELECT * FROM Devices";
            return await _dbConnection.QueryAsync<Device>(query);
        }

        public async Task<int> AddDeviceAsync(Device device)
        {
            var parameters = new
            {
                DeviceName = device.DeviceName,
                IsConnected = device.IsConnected
            };

            // Execute the stored procedure
            return await _dbConnection.ExecuteScalarAsync<int>("AddUpdateDevice", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
