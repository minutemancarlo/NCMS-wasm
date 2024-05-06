using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class UserDto
    {
        public string User_Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Blocked { get; set; }
        public bool? Verified { get; set; }
        public DateTime? Last_Login { get; set; }
        public string Provider { get; set; }
        public string Picture { get; set; }
    }
}
