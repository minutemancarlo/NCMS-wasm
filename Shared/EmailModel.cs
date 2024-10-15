using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class EmailModel
    {
        public int? Id { get; set; }
        public string ToAddress { get; set; }        
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime? SentDate { get; set; }
        public EmailStatus EmailStatus { get; set; }
    }

    public enum EmailStatus
    {
        OnQueue,
        Success,
        Failed
    }

}
