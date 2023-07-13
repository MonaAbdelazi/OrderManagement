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
    
    public partial class OrderItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderItem()
        {
            this.RequestOrderItems = new HashSet<RequestOrderItem>();
            this.ClosedItems = new HashSet<ClosedItem>();
        }
    
        public long ID { get; set; }
        public long OrderId { get; set; }
        public long vendorItemId { get; set; }
        public string Price { get; set; }
        public string ItemPrice { get; set; }
        public Nullable<decimal> NewPrice { get; set; }
        public Nullable<decimal> NewItemPrice { get; set; }
        public string Status { get; set; }
    
        public virtual VendorItem VendorItem { get; set; }
        public virtual Order Order { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequestOrderItem> RequestOrderItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClosedItem> ClosedItems { get; set; }
    }
}
