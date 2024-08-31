using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class BaseModel
    {
        public string? CreatedBy { get; set; } = null;
        public DateTime? CreatedOn { get; set; } = null;
        public string? UpdatedBy { get; set; } = null;
        public DateTime? UpdatedOn { get; set; } = null;
    }
}
