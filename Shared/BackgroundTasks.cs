using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class PayslipUpload : BaseModel
    {
        public string UploadId { get; set; }
        public string FileName { get; set; }  
        public PayslipFileStatus PayslipFileStatus { get; set; }
        public DateTime UploadedOn { get; set; }
    }

    public enum PayslipFileStatus
    {
        OnQueue,
        Processing,
        Success,
        Failed
    }
}
