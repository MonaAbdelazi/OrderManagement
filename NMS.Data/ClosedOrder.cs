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
    
    public partial class ClosedOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClosedOrder()
        {
            this.ClosedItems = new HashSet<ClosedItem>();
        }
    
        public long ID { get; set; }
        public string totalPrice { get; set; }
        public long OrderId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClosedItem> ClosedItems { get; set; }
        public virtual Order Order { get; set; }
    }
}
