using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Repositories.Enums
{
    public enum InventoryMovementType
    {
        StockIn = 1, // add stock

        StockOut = 2, // delete stock

        Reserve = 3, // keep stock for order
 
        Release = 4, // release stock when fail

        Sold = 5 // release stock for success
    }
    
}
