using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class GuestsInfo : BaseModel
    {
        public string? Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? IDType { get; set; }
        public string? IDNumber { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public int Rooms { get; set; } = 1;
        public int Children { get; set; } = 0;
        public int Adults { get; set; } = 1;
        public BookingType BookingType { get; set; } = BookingType.Walkin_Booking;

    }

    public class Booking
    {
        public GuestsInfo Guests { get; set; } = new();
        public RoomInfo Room { get; set; } = new();
        public Billing Billing { get; set; } = new();
        public RFIDCard AccessCard { get; set; } = new();
    }

    public class Billing : BaseModel
    {
        public int BillId { get; set; }
        public string InvoiceNo { get; set; }
        public decimal Total { get; set; }
        public decimal VAT { get; set; }
        public decimal CashReceived { get; set; }
        public decimal Change { get; set; }
        public bool IsCard { get; set; } = false;
        public string? CardTransactionId { get; set; }
    }


    public class Availability
    {
        public DateTime? dateFrom { get; set; } = DateTime.Now;
        public DateTime? dateTo { get; set; }
        public int Rooms { get; set; } = 1;
        public int Children { get; set; } = 0;
        public int Adults { get; set; } = 1;
    }

    public enum BookingType
    {
        Cancelled,
        Reservation,
        Walkin_Booking,
        Booked,
        Checked_Out
    }
}
