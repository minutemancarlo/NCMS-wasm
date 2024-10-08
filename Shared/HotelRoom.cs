using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{  
    public class RoomInfo : BaseModel
    {
        public int? RoomId { get; set; }
        public int RoomNumber { get; set; }
        public RoomType Type { get; set; } = RoomType.Twin;
        public decimal PricePerNight { get; set; } = 0.00M;
        public int MaxGuest { get; set; } = 0;
        public string? Image { get; set; }
        public string? Thumbnail { get; set; }
        public string? Features { get; set; }
        public int Rating { get; set; } = 0;
        public string? RoomDescription { get; set; }
        public RoomStatus Status { get; set; }
        public bool Selected { get; set;} = false;
    
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
        Not_Available,
        Reservation
    }     
}
