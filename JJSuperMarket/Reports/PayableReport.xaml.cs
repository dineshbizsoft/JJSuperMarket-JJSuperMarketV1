using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JJSuperMarket.Reports
{
    /// <summary>
    /// Interaction logic for PayableReport.xaml
    /// </summary>
    public partial class PayableReport : UserControl
    {
        public PayableReport()
        {
            InitializeComponent();
            LoadReport();
             }
        public void LoadReport()
        {
            //PayableDetails.UpdatePayableDetails();
            //dgvPayableSupplier.ItemsSource = PayableDetails.toList.Where(x => x.Type == "Supplier").Select(x => new { PayName = x.PayName, Amount = x.Amount }).ToList();
            //dgvPayableCustomer.ItemsSource = PayableDetails.toList.Where(x => x.Type == "Customer").Select(x => new { PayName = x.PayName, Amount = x.Amount }).ToList();
            JJSuperMarketEntities db = new JJSuperMarketEntities();
            try
            {
                List<SupplierDueReport> Suplist = new List<SupplierDueReport>();

                foreach (var sam in db.Suppliers.ToList())
                {
                    foreach (var supl in db.Purchases.Where(x => x.LedgerCode == sam.SupplierId && x.PurchaseType == "Credit").ToList())
                    {
                        var Pay = db.PaymentMasters.Where(x => x.SupplierId == supl.Supplier.SupplierId).ToList();

                        SupplierDueReport c1 = new SupplierDueReport();
                        c1.SupplierName = supl.Supplier.SupplierName;
                        // c1.DueDate = String.Format("{0:dd-MM-yyyy}", (cust.Date.Value == null ? DateTime.Today : cust.Date.Value.AddDays(cust.Supplier.CreditDays == null ? 0 : (double)cust.Supplier.CreditDays.Value)));

                        c1.Amount = Convert.ToDecimal(string.Format("{0:N2}", supl.ItemAmount.Value));
                        c1.PaidAmount = Pay == null ? 0 : Convert.ToDecimal(string.Format("{0:N2}", Pay.Where(x => x.PurchaseId == supl.InvoiceNo).Sum(x => x.PayAmount).Value));
                        c1.Balance = Convert.ToDecimal(string.Format("{0:N2}", c1.Amount - c1.PaidAmount));

                        c1.PDate = string.Format("{0:dd-MM-yyyy}", supl.PurchaseDate);
                        c1.PInvoiceNo = String.Format("PINV {0}", supl.InvoiceNo);
                        //c1.IsOverdue = (DateTime.Now - cust.Date.Value.AddDays((double)(cust.Supplier.CreditDays == null ? 0 : cust.Supplier.CreditDays.Value))).TotalDays > 0; ;
                        if (c1.Balance > 0) Suplist.Add(c1);

                    }
                }

                //dgvReceivableCustomer.ItemsSource = ReceivableDetails.toList.Where(x => x.Type == "Customer").Select(x => new { PayName = x.PayName, Amount = x.Amount }).ToList();
                dgvPayableSupplier.ItemsSource = Suplist;

                List<CustomerDueReport> Cuslist = new List<CustomerDueReport>();

                foreach (var Cus in db.Customers.ToList())
                {
                    foreach (var cust in db.SalesReturns.Where(x => x.LedgerCode == Cus.CustomerId && x.SRType == "Credit").ToList())
                    {
                        var Pay = db.PaymentMasters.Where(x => x.CustomerId == cust.Customer.CustomerId).ToList();

                        CustomerDueReport c1 = new CustomerDueReport();
                        c1.CustomerName = cust.Customer.CustomerName;
                        // c1.DueDate = String.Format("{0:dd-MM-yyyy}", (cust.Date.Value == null ? DateTime.Today : cust.Date.Value.AddDays(cust.Supplier.CreditDays == null ? 0 : (double)cust.Supplier.CreditDays.Value)));

                        c1.Amount = Convert.ToDecimal(string.Format("{0:N2}", cust.ItemAmount.Value));
                        c1.PaidAmount = Pay == null ? 0 : Convert.ToDecimal(string.Format("{0:N2}", Pay.Where(x => x.SalesRId == cust.InvoiceNo).Sum(x => x.PayAmount).Value));
                        c1.Balance = Convert.ToDecimal(string.Format("{0:N2}", c1.Amount - c1.PaidAmount));

                        c1.InDate = string.Format("{0:dd-MM-yyyy}", cust.SRDate);
                        c1.InvoiceNo = String.Format("SRINV {0}", cust.InvoiceNo);
                        //c1.IsOverdue = (DateTime.Now - cust.Date.Value.AddDays((double)(cust.Supplier.CreditDays == null ? 0 : cust.Supplier.CreditDays.Value))).TotalDays > 0; ;
                        if (c1.Balance > 0) Cuslist.Add(c1);

                    }
                }
                dgvPayableCustomer.ItemsSource = Cuslist;
            }
            catch (Exception ex)
            {


            }
        }
        class CustomerDueReport
        {
            public string InvoiceNo { get; set; }
            public string InDate { get; set; }
            public string CustomerName { get; set; }
            public decimal Amount { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal Balance { get; set; }
        }

        class SupplierDueReport
        {
            public string PInvoiceNo { get; set; }
            public string PDate { get; set; }
            public string SupplierName { get; set; }
            public decimal Amount { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal Balance { get; set; }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }
    }
}
