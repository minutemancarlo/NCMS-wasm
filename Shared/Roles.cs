using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    
    public partial class RolesProperty
    {
        public string id { get; set; }
        public string name { get; set; }    
        public string description { get; set; }
    }

    public partial class RoleAssignment
    {
        public string Id { get; set; }
        public string Role_Id { get; set; }
    }

}
