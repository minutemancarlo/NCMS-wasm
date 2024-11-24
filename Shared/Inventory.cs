using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class InventoryItems: BaseModel
    {
        public int ItemId { get; set; } = 0;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; } = 0;
        public string? Unit { get; set; }
        public HotelInventoryType ItemType { get; set; } = HotelInventoryType.All;
    
    }

    public class InventoryRequest
    {
        public InventoryItems Items { get; set; }
        public InventoryItems? PreviousItems { get; set; }
    }

    public enum HotelInventoryType
    {
        All,
        Linens,
        Toiletries,
        Furniture,
        Electronics,
        Cleaning_Supplies,
        Kitchenware,
        Decorations,
        Office_Supplies,
        Miscellaneous
    }


}
