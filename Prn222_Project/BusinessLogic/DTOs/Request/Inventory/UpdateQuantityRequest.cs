using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Request.Inventory {
    public class UpdateQuantityRequest {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
