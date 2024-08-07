using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class Events
    {
        public int EventId { get; set; } = 0;
        public string? EventName { get; set; }
        public EventsType EventType { get; set; } = EventsType.Holiday;
        public DateTime? EventStart { get; set; } = DateTime.Today;
        public DateTime? EventEnd { get; set; } = DateTime.Today;
        public bool IsApproved { get; set; } = false;
    }

    public enum EventsType
    {
        Holiday = 0,
        Leave = 1
    }
}
