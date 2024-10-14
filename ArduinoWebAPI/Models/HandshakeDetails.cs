namespace ArduinoWebAPI.Models
{
    public class HandshakeDetails : AccessUsage
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
        public int DeviceValue { get; set; }

    }

    public enum DeviceType
    {
        Door_Access,
        Leak_Detection
    }
}
