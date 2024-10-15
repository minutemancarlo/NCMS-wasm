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

        public async Task<int> SaveHandshakeDetailsAsync(HandshakeDetails handshakeDetails)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@DeviceName", handshakeDetails.DeviceName);
            parameters.Add("@LocalIP", handshakeDetails.LocalIp);
            parameters.Add("@MacAddress", handshakeDetails.MacAddress);
            parameters.Add("@IsConnected", handshakeDetails.IsConnected);

            // Execute the stored procedure
            return await _dbConnection.ExecuteScalarAsync<int>("AddUpdateDevice", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> PostCheckCardAsync(HandshakeDetails handshakeDetails)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CardReference", handshakeDetails.CardReference);
            parameters.Add("@DeviceName", handshakeDetails.DeviceName);

            string query = "Select count(*) from RFIDCards r inner join Devices d on r.RoomNumber = d.RoomNumber inner JOIN Rooms r2 on r.RoomNumber =r2.RoomNumber inner join Booking b on r2.BookingNo = b.BookingNo INNER JOIN Guests g on b.GuestId =g.Id WHERE r2.Status = 1 and r.[Type] = 2 and r.CardReference = @CardReference AND d.DeviceName = @DeviceName";

            // Execute the stored procedure
               int result = await _dbConnection.ExecuteScalarAsync<int>(query, parameters, commandType: CommandType.Text);
            return result > 0;
        }

    }
}
