//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KOrders.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class RequestOrderItem
    {
        public long ID { get; set; }
        public long RequestId { get; set; }
        public long ItemId { get; set; }
        public long Quantity { get; set; }
        public decimal Price { get; set; }
        public Nullable<decimal> NewPrice { get; set; }
    
        public virtual OrderItem OrderItem { get; set; }
        public virtual RequestOrder RequestOrder { get; set; }
    }
}
