using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class RFIDCard
    {
        public string CardReference { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public string Value { get; set; }
        public int Points { get; set; }
        public string RegisteredTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
