using Dapper;
using NCMS_wasm.Client.Pages.Hotel;
using NCMS_wasm.Shared;
using System.Data;

namespace NCMS_wasm.Server.Repository
{
    public class CardRepository
    {
        private readonly IDbConnection _dbConnection;
        public CardRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<LoyaltyCardInfo>> GetAllCardsAsync(RFIDType cardType)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CardType", cardType);
            parameters.Add("@SelectType", 0 );

            string sp = "SelectAllCards";
            return await _dbConnection.QueryAsync<LoyaltyCardInfo>(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> VerifyAccessCardAsync(string cardId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CardNumber", cardId);
            
            string sp = "Select count(*) as count from RFIDCards Where CardReference = @CardNumber and isActive = 1";
            var result = await _dbConnection.ExecuteScalarAsync<int>(sp, parameters, commandType: CommandType.Text);

            return result > 0;
        }
        

        public async Task<int> InsertAccessCardAsync(RFIDCard accessCard)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CardReference", accessCard.CardReference);
            parameters.Add("@Type", accessCard.Type);
            parameters.Add("@IsActive", accessCard.IsActive);
            parameters.Add("@Points", accessCard.Points);
            parameters.Add("@RegisteredTo", accessCard.RegisteredTo);
            parameters.Add("@CreatedBy", accessCard.CreatedBy);       
            parameters.Add("@RoomNumber", accessCard.RoomNumber);

            return await _dbConnection.ExecuteScalarAsync<int>("InsertAccessCard", parameters, commandType: CommandType.StoredProcedure);

        }
    }
}
