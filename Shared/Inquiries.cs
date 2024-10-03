using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class Inquiries
    {

        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedOn { get; set; }
        private bool isRead { get; set; } = false;
    }
}
