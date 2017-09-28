using System;
using System.Collections.Generic;
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

namespace JJSuperMarket.Reports.Transaction
{
    /// <summary>
    /// Interaction logic for frmPurchaseReturnSearch.xaml
    /// </summary>
    public partial class frmPurchaseReturnSearch : Window
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        public decimal PRID = 0;
        public frmPurchaseReturnSearch()
        {
            InitializeComponent();
            dtpFromDate.SelectedDate = DateTime.Today;
            dtpToDate.SelectedDate = DateTime.Today;

            dgvDetails.ItemsSource = db.PurchaseReturns .ToList();
            LoadWindow();
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
        private void LoadWindow()
        {


            var v = db.Suppliers .ToList();
            cmbSupplier.ItemsSource = v;
            cmbSupplier.DisplayMemberPath = "SupplierName";
            cmbSupplier.SelectedValuePath = "SupplierName";
            var p = db.PurchaseReturns .ToList();

            if (dtpFromDate.Text != "")
            {
                DateTime d = Convert.ToDateTime(dtpFromDate.Text);
                p = db.PurchaseReturns .Where(x => x.PRDate >= d).ToList();
            }
            if (dtpToDate.Text != "")
            {
                DateTime d = Convert.ToDateTime(dtpToDate.Text);
                p = p.Where(x => x.PRDate <= d).ToList();
            }
            if (txtBillAmtFrom.Text != "")
            {
                double bill = Convert.ToDouble(txtBillAmtFrom.Text.ToString());
                p = p.Where(x => x.ItemAmount >= bill).ToList();
            }
            if (txtBillAmtTo.Text != "")
            {
                double bill = Convert.ToDouble(txtBillAmtTo.Text.ToString());
                p = p.Where(x => x.ItemAmount <= bill).ToList();
            }

            if (txtInvoiceNo.Text != "")
            {
                decimal d = Convert.ToDecimal(txtInvoiceNo.Text.ToString());
                p = p.Where(x => x.InvoiceNo== d).ToList();
            }
            if (cmbSupplier.Text != "")
            {
                p = p.Where(x => x.Supplier.SupplierName.ToString() == cmbSupplier.Text).ToList();
            }

            dgvDetails.ItemsSource = p;

            if (p.Count == 0)
            {
                if (txtInvoiceNo.Text != "")
                {
                    decimal BillNo = Convert.ToDecimal(txtInvoiceNo.Text.ToString());
                    var p1 = db.PurchaseReturns.Where(x => x.InvoiceNo == BillNo).ToList();
                    dgvDetails.ItemsSource = p1;
                }
                else if (cmbSupplier.Text != "")
                {
                    var p2 = db.PurchaseReturns.Where(x => (x.Supplier == null ? "" : x.Supplier.SupplierName) == cmbSupplier.Text).OrderBy(x => x.PRDate).ToList();
                    dgvDetails.ItemsSource = p2;
                }

            }
            else if (txtInvoiceNo.Text != "")
            {
                decimal BillNo1 = txtInvoiceNo.Text == "" ? 0 : Convert.ToDecimal(txtInvoiceNo.Text.ToString());
                var p2 = db.PurchaseReturns.Where(x => x.InvoiceNo == BillNo1).ToList();
                dgvDetails.ItemsSource = p2;
            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
            cmbSupplier.Text = "";
            txtInvoiceNo.Clear();
            txtBillAmtFrom.Clear();
            txtBillAmtTo.Clear();
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PurchaseReturn p = dgvDetails.SelectedItem as PurchaseReturn;

                PRID = p.PRId;
                this.Close();
            }
            catch(Exception ex)
            { }
        }
    }
}
