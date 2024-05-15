using Dapper;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Repository
{
    public class HotelRepository
    {
        private readonly IDbConnection _dbConnection;
        public HotelRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> AddAccomodationAsync(Accomodations info)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FirstName", info.GuestsInfo.FirstName);
            parameters.Add("@Middle", info.GuestsInfo.MiddleName);
            parameters.Add("@LastName", info.GuestsInfo.LastName);
            parameters.Add("@Email", info.GuestsInfo.Email);
            parameters.Add("@Phone", info.GuestsInfo.Phone);
            parameters.Add("@CheckInDate", info.GuestsInfo.CheckInDate);
            parameters.Add("@CheckOutDate", info.GuestsInfo.CheckOutDate);
            parameters.Add("@CheckOutDate", info.GuestsInfo.CheckOutDate);
            parameters.Add("@ArrivalDate", info.GuestsInfo.ArrivalDate);
            parameters.Add("@Rooms", info.GuestsInfo.Rooms);
            parameters.Add("@Children", info.GuestsInfo.Children);
            parameters.Add("@Adults", info.GuestsInfo.Adults);

            // Execute the stored procedure
            return await _dbConnection.ExecuteScalarAsync<int>("AddAccomodations", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}

