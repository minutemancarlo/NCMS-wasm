using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class UserInfo : SetUserRole
    {
        public string Email { get; set; }

        public bool Blocked { get; set; } = false;

        public bool EmailVerified { get; set; } = false;

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string Connection { get; set; } = "con_V6mgoPIOAel8SAtD";

        public string Password { get; set; }

        public bool VerifyEmail { get; set; } = true;
    }

}
