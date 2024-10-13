namespace ArduinoWebAPI.Models
{
    public class HandshakeDetails
    {
        public string? DeviceName { get; set; }
        public string? MacAddress { get; set; }
        public string? LocalIp { get; set; }
        public bool IsConnected { get; set; } = false;
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
