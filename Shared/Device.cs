using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class Device
    {
        //public string DeviceId { get; set; }
        public string DeviceName { get; set; } 
        public string MacAddress { get; set; }  
        public string LocalIP { get; set; }
        public bool IsConnected { get; set; }
    }
}
