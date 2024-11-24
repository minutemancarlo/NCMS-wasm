using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class InventoryAuditTrail
    {
        public int AuditId { get; set; }
        public int ItemId { get; set; }
        public string? PreviousName { get; set; }
        public string? PreviousDescription { get; set; }
        public int? PreviousQuantity { get; set; }
        public string? PreviousItemType { get; set; }
        public string? PreviousUnit { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public string? ItemType { get; set; }
        public string? Unit { get; set; }
        public AuditAction AuditAction { get; set; } = AuditAction.Update;
        public string? AuditBy { get; set; }
        public DateTime? AuditOn { get; set; }
    }

    public enum AuditAction
    {
        Insert,
        Update,
        Delete
    }

}
