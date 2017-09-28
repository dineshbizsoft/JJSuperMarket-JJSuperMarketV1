using JJSuperMarket.Domain;
using MaterialDesignThemes.Wpf;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace JJSuperMarket.Transaction
{
    /// <summary>
    /// Interaction logic for frmPayment.xaml
    /// </summary>
    public partial class frmPayment : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        string UOM;
        
        decimal ID = 0;
        string TextToPrint = "";
        decimal PayToC = 0;
        decimal PayToS = 0;
        decimal GivenAmount;
        public frmPayment()
        {
            InitializeComponent();
            dtpP.SelectedDate = DateTime.Today;
            LoadWindow();
          
        }
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

        #region PredefineTools

        private void cmbNameDr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double? Amt1 = null;
            decimal LedgerId;
            try
            {
                if (rbtCustomerMaster.IsChecked == true)
                {
                    var Cus = cmbNameDr.SelectedItem as Customer;
                    List<CustomerDueReport> Cuslist = new List<CustomerDueReport>();

                    foreach (var cust in db.SalesReturns.Where(x => x.LedgerCode == Cus.CustomerId && x.SRType == "Credit").ToList())
                    {
                        var Pay = db.PaymentMasters.Where(x => x.CustomerId == cust.Customer.CustomerId).ToList();

                        CustomerDueReport c1 = new CustomerDueReport();
                        c1.CustomerName = cust.Customer.CustomerName;
                        // c1.DueDate = String.Format("{0:dd-MM-yyyy}", (cust.Date.Value == null ? DateTime.Today : cust.Date.Value.AddDays(cust.Supplier.CreditDays == null ? 0 : (double)cust.Supplier.CreditDays.Value)));

                        c1.Amount = Convert.ToDecimal(string.Format("{0:N2}", cust.ItemAmount.Value));
                        c1.PaidAmount = Pay == null ? 0 : Convert.ToDecimal(string.Format("{0:N2}", Pay.Where(x => x.SalesRId == cust.InvoiceNo).Sum(x => x.PayAmount).Value));
                        c1.Balance = Convert.ToDecimal(string.Format("{0:N2}", c1.Amount - c1.PaidAmount));

                        c1.Date = string.Format("{0:dd-MM-yyyy}", cust.SRDate);
                        c1.SRInvoiceNo = String.Format("SRINV {0}", cust.InvoiceNo);
                        //c1.IsOverdue = (DateTime.Now - cust.Date.Value.AddDays((double)(cust.Supplier.CreditDays == null ? 0 : cust.Supplier.CreditDays.Value))).TotalDays > 0; ;
                        if (c1.Balance > 0) Cuslist.Add(c1);

                    }
                    dgvPayment.ItemsSource = Cuslist;

                    txtBalance.Text = Cuslist.Sum(x => x.Balance).ToString();


                }
                else if (rbtSupplierMaster.IsChecked == true)
                {
                    var sup = cmbNameDr.SelectedItem as Supplier;
                    List<SupplierDueReport> Suplist = new List<SupplierDueReport>();
                    if (sup != null)
                    {
                        foreach (var supl in db.Purchases.Where(x => x.LedgerCode == sup.SupplierId && x.PurchaseType == "Credit").ToList())
                        {
                            var Pay = db.PaymentMasters.Where(x => x.SupplierId == supl.Supplier.SupplierId).ToList();

                            SupplierDueReport c1 = new SupplierDueReport();
                            c1.LedgerName = supl.Supplier.LedgerName;
                            // c1.DueDate = String.Format("{0:dd-MM-yyyy}", (cust.Date.Value == null ? DateTime.Today : cust.Date.Value.AddDays(cust.Supplier.CreditDays == null ? 0 : (double)cust.Supplier.CreditDays.Value)));

                            c1.Amount = Convert.ToDecimal(string.Format("{0:N2}", supl.ItemAmount.Value));
                            c1.PaidAmount = Pay == null ? 0 : Convert.ToDecimal(string.Format("{0:N2}", Pay.Where(x => x.PurchaseId == supl.InvoiceNo).Sum(x => x.PayAmount).Value));
                            c1.Balance = Convert.ToDecimal(string.Format("{0:N2}", c1.Amount - c1.PaidAmount));

                            c1.Date = string.Format("{0:dd-MM-yyyy}", supl.PurchaseDate);
                            c1.PInvoiceNo = String.Format("PINV {0}", supl.InvoiceNo);
                            //c1.IsOverdue = (DateTime.Now - cust.Date.Value.AddDays((double)(cust.Supplier.CreditDays == null ? 0 : cust.Supplier.CreditDays.Value))).TotalDays > 0; ;
                            if (c1.Balance > 0) Suplist.Add(c1);

                        }
                        foreach (var s1 in db.PurchaseMasters.Where(x => x.LedgerCode == sup.SupplierId && x.PurchaseType == "Credit").ToList())
                        {
                            var Pay = db.PaymentMasters.Where(x => x.SupplierId == s1.Supplier.SupplierId).ToList();

                            SupplierDueReport c1 = new SupplierDueReport();
                            c1.LedgerName = s1.Supplier.LedgerName;
                            // c1.DueDate = String.Format("{0:dd-MM-yyyy}", (cust.Date.Value == null ? DateTime.Today : cust.Date.Value.AddDays(cust.Supplier.CreditDays == null ? 0 : (double)cust.Supplier.CreditDays.Value)));

                            c1.Amount = Convert.ToDecimal(string.Format("{0:N2}", s1.ItemAmount.Value));
                            c1.PaidAmount = Pay == null ? 0 : Convert.ToDecimal(string.Format("{0:N2}", Pay.Where(x => x.PurchaseId == s1.InvoiceNo).Sum(x => x.PayAmount).Value));
                            c1.Balance = Convert.ToDecimal(string.Format("{0:N2}", c1.Amount - c1.PaidAmount));

                            c1.Date = string.Format("{0:dd-MM-yyyy}", s1.PurchaseDate);
                            c1.PInvoiceNo = String.Format("PINV {0}", s1.InvoiceNo);
                            //c1.IsOverdue = (DateTime.Now - cust.Date.Value.AddDays((double)(cust.Supplier.CreditDays == null ? 0 : cust.Supplier.CreditDays.Value))).TotalDays > 0; ;
                            if (c1.Balance > 0) Suplist.Add(c1);

                        }
                    }

                    dgvPayment.ItemsSource = Suplist;

                    txtBalance.Text = Suplist.Sum(x => x.Balance).ToString();
                }


            }
            catch (Exception ex)
            {


            }

        }

        private void cmbPaymentTo_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void rbtCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (rbtCustomer.IsChecked == true)
            {
                cmbCompanySrch.Text = "";
                List<CustomerPayList> culist = new List<CustomerPayList>();
                foreach (var pay in db.PaymentMasters.ToList())
                {
                    foreach (var cus in db.Customers.Where(x => x.CustomerId == pay.CustomerId).ToList())
                    {
                        CustomerPayList c1 = new CustomerPayList();
                        c1.Id = pay.PaymentId;
                        c1.Date = pay.PaymentDate.Value;
                        c1.Name = cus.CustomerName;
                        c1.Amount = (decimal)pay.PayAmount;
                        c1.Description = pay.Description;
                        culist.Add(c1);
                    }

                }
                dgvCustomer.ItemsSource = culist;
            }
        }

        private void rbtSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (rbtSupplier.IsChecked == true)
            {
                cmbCompanySrch.Text = "";
                List<SuplierPayList> splist = new List<SuplierPayList>();
                foreach (var pay in db.PaymentMasters.ToList())
                {
                    foreach (var sup in db.Suppliers.Where(x => x.SupplierId == pay.SupplierId).ToList())
                    {
                        SuplierPayList s1 = new SuplierPayList();
                        s1.Id = pay.PaymentId;
                        s1.Date = pay.PaymentDate.Value;
                        s1.Name = sup.LedgerName;
                        s1.Amount = (decimal)pay.PayAmount;
                        s1.Description = pay.Description;
                        splist.Add(s1);
                    }

                }
                dgvCustomer.ItemsSource = splist;
            }
        }

        #region PayList
        class SuplierPayList
        {
            public decimal Id { get; set; }
            public DateTime Date { get; set; }
            public string Name { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }

        }
        class CustomerPayList
        {
            public decimal Id { get; set; }
            public DateTime Date { get; set; }
            public string Name { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }

        }

        #endregion
        private void rbtCustomer1_Click(object sender, RoutedEventArgs e)
        {
            // LoadReport("Customer");

        }

        private void rbtSupplier1_Click(object sender, RoutedEventArgs e)
        {
            // LoadReport("Supplier");

        }
        #endregion

        #region ButtonEvents
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool acc = true;
                decimal fkyc = 0;
                decimal fkys = 0;
                var cus = cmbNameDr.SelectedItem as Customer;
                var sup = cmbNameDr.SelectedItem as Supplier;
                decimal PaidAmount = txtAmount.Text==""?0: Convert.ToDecimal(txtAmount.Text);
                if (rbtCustomerMaster.IsChecked == true)
                {

                    if (cus == null)
                    {
                        MessageBox.Show("Please select the name.."); cmbNameDr.Focus();
                        acc = false;
                    }
                    else if (PayToC == 0)
                    {
                        MessageBox.Show("Select Invoice"); acc = false;
                    }
                    else if (txtAmount.Text == "")
                    {

                        MessageBox.Show("Enter Amount");
                        txtAmount.Focus();
                        acc = false;
                    }
                    else if (PaidAmount > GivenAmount)
                    {
                        MessageBox.Show("Amount Mismatch", "Alert");
                        acc = false;
                        txtAmount.Focus();
                    }
                    fkyc = cus.CustomerId;
                }
                else if (rbtSupplierMaster.IsChecked == true)
                {

                    if (sup == null)
                    {
                        MessageBox.Show("Please select the name.."); cmbNameDr.Focus();
                        acc = false;
                    }
                    else if (PayToS == 0)
                    {
                        MessageBox.Show("Select Invoice"); acc = false;
                    }
                    else if (txtAmount.Text == "")
                    {

                        MessageBox.Show("Enter Amount");
                        txtAmount.Focus();
                        acc = false;
                    }
                    else if (PaidAmount > GivenAmount)
                    {
                        MessageBox.Show("Amount Mismatch","Alert");
                        acc = false;
                        txtAmount.Focus();
                    }
                    fkys = sup.SupplierId;
                }

               

                if (acc == true)
                {
                    if (ID != 0)
                    {
                        PaymentMaster c = db.PaymentMasters.Where(x => x.PaymentId == ID).FirstOrDefault();

                        c.PaymentDate = dtpP.SelectedDate.Value;
                        c.PurchaseId = PayToS;
                        c.SupplierId = fkys;
                        c.SalesRId = PayToC;
                        c.CustomerId = fkyc;
                        c.PaymentMode = "Cash";
                        c.PayAmount = Convert.ToDecimal(txtAmount.Text);
                        c.Description = txtNarration.Text;

                        db.SaveChanges();

                        MessageBox.Show("Updated Sucessfully");

                        FormClear();
                        rbtCustomer_Click(sender, e);
                        rbtSupplier_Click(sender, e);
                    }
                    else
                    {

                        if (await validation() == true)
                        {

                            PaymentMaster p = new PaymentMaster();
                            p.PaymentDate = dtpP.SelectedDate.Value;

                            p.PurchaseId = PayToS;
                            p.SupplierId = fkys;
                            p.SalesRId = PayToC;
                            p.CustomerId = fkyc;
                            p.PaymentMode = "Cash";

                            p.PayAmount = Convert.ToDecimal(txtAmount.Text);
                            p.Description = txtNarration.Text;

                            db.PaymentMasters.Add(p);

                            db.SaveChanges();
                            MessageBox.Show("Saved Sucessfully");
                            FormClear();
                            rbtCustomer_Click(sender, e);
                            rbtSupplier_Click(sender, e);
                        }


                    }
                }


            }
            catch (Exception ex)
            {

            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FormClear();
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ID == 0)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "No Records to Delete.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                }
                else
                {
                    var sampleDialog = new SampleDialog
                    {
                        Message = { Text = "Do you want to delete tis record?.." }
                    };

                    var result = Convert.ToBoolean(await DialogHost.Show(sampleDialog, "RootDialog"));
                    if (result == true)
                    {
                        PaymentMaster p = db.PaymentMasters.Where(x => x.PaymentId == ID).FirstOrDefault();
                        db.PaymentMasters.Remove(p);

                        //Ledger l = db.Ledgers.Where(x => x.LedgerName == txtCompanyName.Text.ToString()).FirstOrDefault();
                        //db.Ledgers.Remove(l);

                        db.SaveChanges();
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = { Text = "Deleted Successfully.." }
                        };

                        await DialogHost.Show(sampleMessageDialog, "RootDialog");
                        FormClear();
                        LoadWindow();
                        //   LoadReport("Customer");
                    }
                }
            }

            catch (Exception ex)
            {

            }


        }

        private void btnSearch1_Click(object sender, RoutedEventArgs e)
        {
            // LoadReport();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCompanySrch.Text != "")
            {
                var cid = db.Customers.Where(x => x.CustomerName == cmbCompanySrch.Text).FirstOrDefault();
                if (cid != null) dgvCustomer.ItemsSource = db.PaymentMasters.Where(x => x.CustomerId == cid.CustomerId).Select(x => new { Id = x.PaymentId, Date = x.PaymentDate, Name = cid.CustomerName, Amount = x.PayAmount, Description = x.Description }).ToList();

                var sid = db.Suppliers.Where(x => x.LedgerName == cmbCompanySrch.Text).FirstOrDefault();
                if (sid != null) dgvCustomer.ItemsSource = db.PaymentMasters.Where(x => x.SupplierId == sid.SupplierId).Select(x => new { Id = x.PaymentId, Date = x.PaymentDate, Name = sid.LedgerName, Amount = x.PayAmount, Description = x.Description }).ToList();


            }

        }

        private void btnSearch1_Click_1(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region UserDefine
        private async Task<bool> validation()
        {
            if (txtAmount.Text.Trim() == null)
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = " Please Enter the Amount" }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");
                txtAmount.Focus();
                return false;
            }
            else { return true; }

        }

        private void FormClear()
        {
            cmbNameDr.Text = "";
            txtAmount.Clear();
            txtBalance.Clear();
            txtNarration.Clear();
            dtpP.SelectedDate = DateTime.Today;
            var list1 = new List<CustomerDueReport>();
            dgvPayment.ItemsSource = list1;
            dgvCustomer.Items.Refresh();
            // dgvCustomer.ItemsSource = db.PaymentMasters.ToList();
            // cmbCompanySr.Text = "";
            cmbCompanySrch.Text = "";
            ID = 0;
            GivenAmount = 0;
        }

        //private void LoadReport(string Payment)
        //{
        //    try
        //    {
        //        Reportviewer.Reset();
        //        DataTable dt = getData(Payment);
        //        ReportDataSource Data = new ReportDataSource("PaymentMaster", dt);

        //        Reportviewer.LocalReport.DataSources.Add(Data);
        //        Reportviewer.LocalReport.ReportEmbeddedResource = "POS.Reports.Transaction.rptPaymentReport.rdlc";

        //        Reportviewer.RefreshReport();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }

        //}

        private void LoadWindow()
        {

            List<CustomerPayList> culist = new List<CustomerPayList>();
            foreach (var pay in db.PaymentMasters.ToList())
            {
                foreach (var cus in db.Customers.Where(x => x.CustomerId == pay.CustomerId).ToList())
                {
                    CustomerPayList c1 = new CustomerPayList();
                    c1.Id = pay.PaymentId;
                    c1.Date = pay.PaymentDate.Value;
                    c1.Name = cus.CustomerName;
                    c1.Amount = (decimal)pay.PayAmount;
                    c1.Description = pay.Description;
                    culist.Add(c1);
                }

            }
            dgvCustomer.ItemsSource = culist;

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        //private DataTable getData(String Payment)
        //{

        //    DataTable dt = new DataTable();
        //    using (SqlConnection con = new SqlConnection(AppLib.conStr ))
        //    {
        //        SqlCommand cmd;
        //        if (cmbCompanySr.Text != "")
        //        {
        //            string qry = string.Format("Select * from PaymentMaster  where LedgerCode='{0}'", cmbCompanySr.Text);
        //            cmd = new SqlCommand(qry, con);
        //            SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //            adp.Fill(dt);
        //        }
        //        else
        //        {
        //            string qry = string.Format("Select * from PaymentMaster where PaymentTo='"+Payment .ToString () +"'");
        //            cmd = new SqlCommand(qry, con);
        //            SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //            adp.Fill(dt);
        //        }


        //    }
        //    return dt;

        //}

        private void rbtCustomerMaster_Click(object sender, RoutedEventArgs e)
        {
            dgvPayment.Columns.Clear();
            cmbNameDr.Text = "";
        }

        private void rbtSupplierMaster_Click(object sender, RoutedEventArgs e)
        {
            dgvPayment.Columns.Clear();
            cmbNameDr.Text = "";
        }

        private void cmbNameDr_DropDownOpened(object sender, EventArgs e)
        {
            if (rbtCustomerMaster.IsChecked == true)
            {
                var v = db.Customers.ToList();
                cmbNameDr.ItemsSource = v;
                cmbNameDr.DisplayMemberPath = "CustomerName";
                cmbNameDr.SelectedValuePath = "CustomerId";
                //cmbNameDr.ItemsSource = db.Customers.Select(x => x.CustomerName).ToList();
            }
            else if (rbtSupplierMaster.IsChecked == true)
            {
                var v = db.Suppliers.ToList();
                cmbNameDr.ItemsSource = v;
                cmbNameDr.DisplayMemberPath = "LedgerName";
                cmbNameDr.SelectedValuePath = "SupplierId";

                //cmbNameDr.ItemsSource = db.Suppliers.Select(x => x.LedgerName).ToList();
            }
        }

        private void dgvPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (rbtCustomerMaster.IsChecked == true)
            {
                var pData = dgvPayment.SelectedItem as CustomerDueReport;
                if (pData != null)
                {
                    PayToC = decimal.Parse(pData.SRInvoiceNo.Substring(6));
                    PayToS = 0;
                    txtNarration.Text = "For " + pData.SRInvoiceNo;
                    GivenAmount = pData.Balance;
                }
            }
            else if (rbtSupplierMaster.IsChecked == true)
            {
                var pData = dgvPayment.SelectedItem as SupplierDueReport;
                if (pData != null)
                {
                    PayToC = 0;
                    PayToS = decimal.Parse(pData.PInvoiceNo.Substring(5));
                    txtNarration.Text = "For " + pData.PInvoiceNo;
                    GivenAmount = pData.Balance;
                }
            }


        }

        private void dgvCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //  var data = dgvCustomer.SelectedItem as PaymentMaster;

            if (rbtCustomer.IsChecked == true)
            {
                var data = dgvCustomer.SelectedItem as CustomerPayList;
                if (data != null)
                {

                    // var cname = db.Customers.Where(x => x.CustomerId == data.CustomerId).FirstOrDefault();
                    cmbNameDr.Text = data.Name;
                    txtAmount.Text = data.Amount.ToString();
                    txtNarration.Text = data.Description;
                    dtpP.SelectedDate = data.Date.Date;
                    //  PayToC = data.SalesRId.Value;
                    var list1 = new List<CustomerDueReport>();
                    //  var pay = db.PaymentMasters.Where(x => x.PaymentId == data.Id).ToList();

                    dgvPayment.ItemsSource = list1;
                    ID = data.Id;
                }
            }
            else if (rbtSupplier.IsChecked == true)
            {
                var data = dgvCustomer.SelectedItem as SuplierPayList;
                if (data != null)
                {
                    //var sname = db.Suppliers.Where(x => x.SupplierId == data.SupplierId).FirstOrDefault();
                    cmbNameDr.Text = data.Name;
                    txtAmount.Text = data.Amount.ToString();
                    txtNarration.Text = data.Description;
                    dtpP.SelectedDate = data.Date.Date;
                    // PayToS = data.PurchaseId.Value;
                    var list1 = new List<SupplierDueReport>();


                    dgvPayment.ItemsSource = list1;
                    ID = data.Id;
                }
            }

        }

        private void cmbCompanySrch_DropDownOpened(object sender, EventArgs e)
        {
            if (rbtCustomer.IsChecked == true)
            {
                var v = db.Customers.ToList();
                cmbCompanySrch.ItemsSource = v;
                cmbCompanySrch.DisplayMemberPath = "CustomerName";
                cmbCompanySrch.SelectedValuePath = "CustomerId";
                //cmbCompanySrch.ItemsSource = db.Customers.Select(x => x.CustomerName).ToList();
            }
            else if (rbtSupplier.IsChecked == true)
            {
                var v = db.Suppliers.ToList();
                cmbCompanySrch.ItemsSource = v;
                cmbCompanySrch.DisplayMemberPath = "LedgerName";
                cmbCompanySrch.SelectedValuePath = "SupplierId";
                // cmbCompanySrch.ItemsSource = db.Suppliers.Select(x => x.LedgerName).ToList();
            }
        }
        private void rbtSupplierMaster_Click_1(object sender, RoutedEventArgs e)
        {

        }



        #endregion


    }
    class CustomerDueReport
    {
        public string SRInvoiceNo { get; set; }
        public string Date { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
    }

    class SupplierDueReport
    {
        public string PInvoiceNo { get; set; }
        public string Date { get; set; }
        public string LedgerName { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
    }

}
