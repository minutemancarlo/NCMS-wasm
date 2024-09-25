using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    /// <summary>
    /// Represents a gas transaction model.
    /// </summary>
    public class GasModel : BaseModel
    {
       
        /// <summary>
        /// Gets or sets the transaction ID. Automatically generated in database
        /// </summary>
        public string? TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the type of transaction. Default to Cash
        /// </summary>
        public TransactionType TransactionType { get; set; } = TransactionType.Cash;

        /// <summary>
        /// Gets or sets the Gas Amount Value. Gas Value e.g 1 Liter. Gas Amount. Value * Price
        /// </summary>
        public GasTransaction GasValues { get; set; } = new();

        /// <summary>
        /// Gets or sets the card transaction ID, if applicable.
        /// </summary>
        public string? CardTransactionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction is void.
        /// </summary>
        public bool IsVoid { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the transaction is a card transaction.
        /// </summary>
        public bool IsCard { get; set; } = false;

        /// <summary>
        /// Gets or sets the points earned from the transaction.
        /// </summary>
        public decimal Points { get; set; } = 0.00M;

        /// <summary>
        /// Gets or sets the discounts applied to the transaction.
        /// </summary>
        public decimal Discounts { get; set; } = 0.00M;

        /// <summary>
        /// Gets or sets the VAT applied to the transaction.
        /// </summary>
        public decimal VAT { get; set; } = 0.00M;

        /// <summary>
        /// Gets or sets the total amount of the transaction.
        /// </summary>
        public decimal Total { get; set; } = 0.00M;

        /// <summary>
        /// Gets or sets the cash received
        /// </summary>
        public decimal CashReceived { get; set; } = 0.00M;

        /// <summary>
        /// Gets or sets the change.
        /// </summary>
        public decimal Change { get; set; } = 0.00M;

        /// <summary>
        /// Gets or sets the scanned loyalty ID Card.
        /// </summary>
        public string? LoyaltyCardId { get; set; }

        /// <summary>
        /// Gets or sets the Invoice No of the Transaction
        /// </summary>
        public string? InvoiceNo { get; set; }

    }


    /// <summary>
    /// Gets or sets the GasTransaction that will be returned to Parent
    /// </summary>
    public class GasTransaction
    {
        /// <summary>
        /// Gets or sets the value of the gas in the transaction.
        /// </summary>
        public decimal GasValue { get; set; } = 0.00M;

        /// <summary>
        /// Gets or sets the Gas Amount Value .
        /// </summary>
        public decimal GasAmount { get; set; } = 0.00M;
    }
    /// <summary>
    /// Gets or sets the Prices per Gas Type
    /// </summary>
    public class GasPrices
    {
        public decimal Unleaded { get; set; } = 0.00M;
        public decimal Regular { get; set; } = 0.00M;
        public decimal Premium { get; set; } = 0.00M;
        public decimal Diesel { get; set; } = 0.00M;
    }

    public class GasPrice : BaseModel
    {
        public int ID { get; set; }
        public GasType GasType { get; set; }
        public decimal Price { get; set; }
        public decimal CapacityRemaining { get; set; }
    }


    /// <summary>
    /// Gets or sets the Payment [For Computation Only for Cash] , [For saving card Transaction Id, if applicable]
    /// </summary>
    public class Payment
    {

        /// <summary>
        /// Gets or sets the card transaction ID, if applicable.
        /// </summary>
        public string? CardTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the change amount of the transaction.
        /// </summary>
        public decimal Change { get; set; } = 0.00M;
        /// <summary>
        /// Gets or sets the cash received by the teller
        /// </summary>
        public decimal CashReceived { get; set; } = 0.00M;

       
    }

    /// <summary>
    /// Gets or sets the Items per Transaction 
    /// </summary>
    public class SubTransaction : BaseModel
    {
        public int SubTransactionId { get; set; } = 0;
        public string? TransactionId { get; set; }
        public GasType GasType { get; set; }
        public decimal Price { get; set; } = 0.00M;
        public decimal Value { get; set; } = 0.00M;
        public decimal VAT { get; set; } = 0.00M;
        public decimal NetAmount { get; set; } = 0.00M;
        public decimal Amount { get; set; } = 0.00M;
        public decimal SubTotal { get; set; } = 0.00M;

        public string? InvoiceNo { get; set; }

    }

    /// <summary>
    /// Used to Pass the parameter to endpoint
    /// </summary>
    public class TransactionRequest
    {
        public GasModel Transaction { get; set; }
        public List<SubTransaction> SubTransactions { get; set; }
    }


    /// <summary>
    /// Cash = 1, Card = 2, Points = 3
    /// </summary>
    public enum TransactionType
    {
        Cash =1,
        Card =2,
        Points =3
    }

    /// <summary>
    /// Gas Types: Regular = 1, Diesel =2, Premium =3, Unleaded = 4
    /// </summary>
    public enum GasType
    {
        Regular = 1,
        Diesel = 2,
        Premium =3,
        Unleaded = 4
    }
}
