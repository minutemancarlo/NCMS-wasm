using Dapper;
using NCMS_wasm.Client.Pages.Hotel;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Repository
{
    public class GuestRepository
    {
        private readonly IDbConnection _dbConnection;
        public GuestRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> AddInquiriesAsync(Inquiries inquiries)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Name", inquiries.Name);
            parameters.Add("@Email", inquiries.Email);
            parameters.Add("@Message", inquiries.Message);


            string query = "INSERT INTO Inquiries (Name,Email,Message,CreatedOn,isRead) VALUES (@Name,@Email,@Message,GETDATE(),0)";

            return await _dbConnection.ExecuteAsync(query, parameters);
        }

        public async Task<int> InsertGuestAsync(GuestsInfo guest)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FirstName", guest.FirstName);
            parameters.Add("@MiddleName", guest.MiddleName);
            parameters.Add("@LastName", guest.LastName);
            parameters.Add("@Email", guest.Email);
            parameters.Add("@Phone", guest.Phone);
            parameters.Add("@CheckInDate", guest.CheckInDate);
            parameters.Add("@CheckOutDate", guest.CheckOutDate);
            parameters.Add("@ArrivalDate", guest.ArrivalDate);
            parameters.Add("@CreatedBy", guest.CreatedBy);                  
            parameters.Add("@BookingType", guest.BookingType);
            parameters.Add("@IDType", guest.IDType);
            parameters.Add("@IDNumber", guest.IDNumber);
            

            return await _dbConnection.ExecuteScalarAsync<int>("InsertGuest", parameters, commandType: CommandType.StoredProcedure);

        }

    }
}
