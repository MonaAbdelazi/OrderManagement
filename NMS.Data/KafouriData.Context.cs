﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class kafouriEntities : DbContext
    {
        public kafouriEntities()
            : base("name=kafouriEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<VendorItem> VendorItems { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<RequestOrderItem> RequestOrderItems { get; set; }
        public virtual DbSet<RequestOrder> RequestOrders { get; set; }
        public virtual DbSet<RequestedOrderInvoice> RequestedOrderInvoices { get; set; }
        public virtual DbSet<ClosedItem> ClosedItems { get; set; }
        public virtual DbSet<ClosedOrder> ClosedOrders { get; set; }
    }
}
