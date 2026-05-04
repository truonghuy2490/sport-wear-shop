using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Repositories.Enums
{
    public enum InventoryMovementType
    {
        StockIn = 1, // When new stock is added to the inventory 
        StockOut = 2, // for non-reserved stock, such as returns or manual adjustments (damaged or lost items or broken items)
        Reserve = 3, // stock is reserved for an order 
        Release = 4, // reserved stock is released -  order is cancelled 
        Deduct = 5, // When reserved stock is deducted - order is completed 
        Adjustment = 6 // When stock is manually adjusted 
    }
    public enum InventoryReferenceType
    {
        Order = 1,
        ManualAdjustment = 2,
        Return = 3,
        System = 4
    }
}
