﻿using Dapper;
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

        public async Task<IEnumerable<LoyaltyCardInfo>> GetAllCardsAsync(int cardType)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CardType", cardType);

            string sp = "SelectAllCards";
            return await _dbConnection.QueryAsync<LoyaltyCardInfo>(sp, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
