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
            var parameters = new DynamicParameters();
            parameters.Add("@DeviceName", device.DeviceName);
            parameters.Add("@LocalIP", device.LocalIp);
            parameters.Add("@MacAddress", device.MacAddress);
            parameters.Add("@IsConnected", device.IsConnected);

            // Execute the stored procedure
            return await _dbConnection.ExecuteScalarAsync<int>("AddUpdateDevice", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateDeviceRoom(Device device)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RoomNumber", device.RoomNumber);
            parameters.Add("@DeviceName", device.DeviceName);
            parameters.Add("@MacAddress", device.MacAddress);



            string query = "Update Devices Set RoomNumber = @RoomNumber Where DeviceName = @DeviceName and MacAddress = @MacAddress";
            // Execute the stored procedure
            return await _dbConnection.ExecuteScalarAsync<int>(query, parameters, commandType: CommandType.Text);
        }

    }
}
