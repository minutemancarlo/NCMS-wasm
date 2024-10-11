using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using NCMS_wasm.Shared;
using Nextended.Core.Extensions;
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

        public async Task<string> InsertBookingAsync(Booking booking,int guestId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Total", booking.Billing.Total);
            parameters.Add("@Change", booking.Billing.Change);
            parameters.Add("@CardTransactionId", booking.Billing.CardTransactionId);
            parameters.Add("@IsCard", booking.Billing.IsCard);
            parameters.Add("@VAT", booking.Billing.VAT);
            parameters.Add("@CashReceived", booking.Billing.CashReceived);            
            parameters.Add("@CreatedBy", booking.CreatedBy);     
            parameters.Add("@GuestId", guestId);
            parameters.Add("@Vatable", booking.Billing.Vatable);            
            return await _dbConnection.ExecuteScalarAsync<string>("InsertBooking", parameters, commandType: CommandType.StoredProcedure);

        }

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

        public async Task<int> UpdateRoomStatusAsync(int? roomNumber, RoomStatus roomStatus, string updatedBy,string invoiceNo)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RoomNumber", roomNumber);
            parameters.Add("@Status", roomStatus);
            parameters.Add("@UpdatedBy", updatedBy);
            parameters.Add("@InvoiceNo", invoiceNo);

            string query = "UPDATE Rooms SET Status = @Status,BookingNo = (Select Top 1 BookingNo from Booking where InvoiceNo= @InvoiceNo) ,UpdatedBy = @UpdatedBy WHERE RoomNumber = @RoomNumber";

            return await _dbConnection.ExecuteAsync(query, parameters);
        }

        public async Task<int> UpdateBookingStatusAsync(GuestsInfo guestsInfo)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@BookingNo", guestsInfo.BookingNo);
            parameters.Add("@BookingType", guestsInfo.BookingType);
            parameters.Add("@UpdatedBy", guestsInfo.UpdatedBy);
            parameters.Add("@RoomNumber", guestsInfo.RoomNumber);


            string sp = "UpdateBookingStatus";

            return await _dbConnection.ExecuteAsync(sp, parameters,commandType: CommandType.StoredProcedure);
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
            string query = "select DISTINCT (b.type) as t,* from rooms a inner join roominfo b on a.roomId=b.roomId where a.status = 1 ORDER BY a.RoomId DESC";
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

        public async Task<List<DashboardValueSales>> GetDashboardSalesByMonthAsync(int year)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Year", year);

            string query = @"
        WITH Months AS (
            SELECT 1 AS Month
            UNION ALL
            SELECT 2
            UNION ALL
            SELECT 3
            UNION ALL
            SELECT 4
            UNION ALL
            SELECT 5
            UNION ALL
            SELECT 6
            UNION ALL
            SELECT 7
            UNION ALL
            SELECT 8
            UNION ALL
            SELECT 9
            UNION ALL
            SELECT 10
            UNION ALL
            SELECT 11
            UNION ALL
            SELECT 12
        )
        SELECT 
            m.Month,
            ISNULL(SUM(b.Total), 0) AS Sales
        FROM 
            Months m
        LEFT JOIN 
            Booking b ON m.Month = MONTH(b.CreatedOn) AND YEAR(b.CreatedOn) = @Year
        GROUP BY 
            m.Month
        ORDER BY 
            m.Month;";

            var result = await _dbConnection.QueryAsync<DashboardValueSales>(query, parameter, commandType: CommandType.Text);
            return result.ToList();
        }



        public async Task<IEnumerable<GuestsInfo>> GetCalendarDisplayAsync()
        {
          
            string sp = "GetBookingCalendar";
            return await _dbConnection.QueryAsync<GuestsInfo>(sp, commandType: CommandType.StoredProcedure);
        }


    }
}

