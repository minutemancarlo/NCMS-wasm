using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class HotelRoom : BaseModel
    {
        public int? RoomNumber { get; set; }
        public string? RoomDescription { get; set; }
        public RoomType Type { get; set; }
        public decimal PricePerNight { get; set; }
        public RoomStatus Status { get; set; }
        public int MaxGuest { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class RoomInfo
    {
        public int RoomId { get; set; } = 0;
        public RoomType Type { get; set; } = RoomType.Twin;
        public decimal PricePerNight { get; set; } = 0.00M;
        public int MaxGuest { get; set; } = 0;
        public string? Image { get; set; }
        public string? Thumbnail { get; set; }
        public string? Features { get; set; }
        public int Rating { get; set; } = 0;
    }

    public enum RoomType
    {
        Twin,
        Double_Twin,
        Triple,
        Deluxe_Double,
        Superior_Queen,
        Suite,
        King_Suite
    }
    public enum RoomStatus
    {
        Available,
        Occupied,
        Reserved,
        Under_Maintenance,
        Not_Available
    }     
}
