﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JJSuperMarket
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class JJSuperMarketEntities : DbContext
    {
        public JJSuperMarketEntities()
            : base("name=JJSuperMarketEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccountGroup> AccountGroups { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<CompanyDetail> CompanyDetails { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Journal> Journals { get; set; }
        public virtual DbSet<JournalDetail> JournalDetails { get; set; }
        public virtual DbSet<JournalMaster> JournalMasters { get; set; }
        public virtual DbSet<Ledger> Ledgers { get; set; }
        public virtual DbSet<LedgerReport> LedgerReports { get; set; }
        public virtual DbSet<LogInfo> LogInfoes { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }
        public virtual DbSet<PaymentMaster> PaymentMasters { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual DbSet<PurchaseReturn> PurchaseReturns { get; set; }
        public virtual DbSet<PurchaseReturnDetail> PurchaseReturnDetails { get; set; }
        public virtual DbSet<Receipt> Receipts { get; set; }
        public virtual DbSet<ReceiptDetail> ReceiptDetails { get; set; }
        public virtual DbSet<ReceiptMaster> ReceiptMasters { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SalesDetail> SalesDetails { get; set; }
        public virtual DbSet<SalesOrder> SalesOrders { get; set; }
        public virtual DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public virtual DbSet<SalesReturn> SalesReturns { get; set; }
        public virtual DbSet<SalesReturnDetail> SalesReturnDetails { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<StockGroup> StockGroups { get; set; }
        public virtual DbSet<StockReport> StockReports { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<TaxMaster> TaxMasters { get; set; }
        public virtual DbSet<UnitsOfMeasurement> UnitsOfMeasurements { get; set; }
        public virtual DbSet<UserAccount> UserAccounts { get; set; }
        public virtual DbSet<AutoGeneration> AutoGenerations { get; set; }
        public virtual DbSet<SoftValidation> SoftValidations { get; set; }
        public virtual DbSet<TextTranslation> TextTranslations { get; set; }
        public virtual DbSet<TmpProduct> TmpProducts { get; set; }
        public virtual DbSet<PurchaseMaster> PurchaseMasters { get; set; }
    }
}