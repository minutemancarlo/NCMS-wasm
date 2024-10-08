using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using NCMS_wasm.Shared;
using System.Data;
using System.Text;

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

    //    public async Task<int> AddGuestAsync(Accomodations info)
    //    {
    //        var parameters = new DynamicParameters();
    //        parameters.Add("@FirstName", info.GuestsInfo.FirstName);

    //        @FirstName nvarchar(255),
    //@MiddleName nvarchar(255),
    //@LastName nvarchar(255),
    //@Email nvarchar(255),
    //@Phone nvarchar(255),
    //@CheckInDate datetime,
    //@CheckOutDate datetime,
    //@ArrivalDate datetime,
    //@BookingType int,
    //@CreatedBy nvarchar(255),
    //@UpdatedBy nvarchar(255)
    //        // Execute the stored procedure
    //        return await _dbConnection.ExecuteScalarAsync<int>("AddBooking", parameters, commandType: CommandType.StoredProcedure);
    //    }

        public async Task<int> AddRoomsAsync(RoomInfo room)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RoomNumber", room.RoomNumber);
            parameters.Add("@RoomDescription", room.RoomDescription);
            parameters.Add("@Type", room.Type);
            parameters.Add("@PricePerNight", room.PricePerNight);
            parameters.Add("@Status", room.Status);
            parameters.Add("@MaxGuest", room.MaxGuest);
            parameters.Add("@Image", room.Image);
            parameters.Add("@Thumbnail", room.Thumbnail);
            parameters.Add("@Features", room.Features);
            parameters.Add("@CreatedBy", room.CreatedBy);
            parameters.Add("@Rating", room.Rating);

            return await _dbConnection.ExecuteScalarAsync<int>("AddRooms", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdatePriceAndStatusAsync(RoomInfo room)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RoomId", room.RoomId);
            parameters.Add("@RoomNumber", room.RoomNumber);      
            parameters.Add("@PricePerNight", room.PricePerNight);
            parameters.Add("@Status", room.Status);
            parameters.Add("@UpdatedBy", room.UpdatedBy);
       

            return await _dbConnection.ExecuteScalarAsync<int>("UpdatePriceAndStatus", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<RoomInfo>> GetAllRoomsAsync()
        {
            string query = @"SELECT a.RoomId,
a.RoomNumber,
a.RoomDescription,
a.Status,
ISNULL(c.Name,'N/A') as CreatedBy,
a.CreatedOn,
ISNULL(d.NAME,'N/A') as UpdatedBy,
a.UpdatedOn,
b.Type,
b.PricePerNight,
b.MaxGuest,
b.Thumbnail,
b.Image,
b.Features,
b.Rating
FROM Rooms a inner join roominfo b on a.roomId = b.roomId left join employee c on a.createdBy = c.Auth0_Id left join employee d on a.updatedby = d.Auth0_Id ORDER BY a.CreatedOn DESC";
            return await _dbConnection.QueryAsync<RoomInfo>(query);
        }

        public async Task<IEnumerable<RoomInfo>> GetAllRoomsInfoAsync()
        {
            string query = "SELECT * FROM RoomInfo ORDER BY RoomId DESC";
            return await _dbConnection.QueryAsync<RoomInfo>(query);
        }

        public async Task<bool> GetRoomsNumberExistAsync(int roomNumber)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@RoomNumber", roomNumber);

            string query = "SELECT COUNT(*) FROM Rooms WHERE RoomNumber = @RoomNumber";
            
            var result = await _dbConnection.ExecuteScalarAsync<int>(query, parameter);
            return result > 0;
        }


    }
}

