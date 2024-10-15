using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class Device : AccessUsage
    {
        public string? DeviceName { get; set; }
        public string? MacAddress { get; set; }
        public string? LocalIp { get; set; }
        public bool IsConnected { get; set; } = false;
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        public int RoomNumber { get; set; } = 0;
        public bool IsOpen { get; set; } = false;
    }

    public class AccessUsage
    {
        public DateTime? AccessedOn { get; set; }
        public string? CardReference { get; set; }
        public DeviceType DeviceType { get; set; } = DeviceType.Door_Access;
        public int DeviceValue { get; set; } = 0;

    }

    public enum DeviceType
    {
        Door_Access,
        Leak_Detection
    }
}
