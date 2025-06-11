using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class RFIDCard
    {
        public string? CardReference { get; set; } = string.Empty;
        public RFIDType Type { get; set; } = RFIDType.Loyalty;
        public bool IsActive { get; set; } = true;
        public decimal Points { get; set; } = 0.00M;
        public string? RegisteredTo { get; set; }
        public int? RoomNumber { get; set; }
        public string? EmployeeID { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public enum RFIDType
    {
        Loyalty = 1,
        Access = 2,
        EmployeeId =3
    }

}
