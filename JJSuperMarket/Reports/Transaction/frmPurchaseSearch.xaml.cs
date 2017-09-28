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
using System.Windows.Shapes;

namespace JJSuperMarket.Reports.Transaction
{
    /// <summary>
    /// Interaction logic for frmPurchaseSearch.xaml
    /// </summary>
    public partial class frmPurchaseSearch : Window
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        public decimal PID = 0;
        public frmPurchaseSearch()
        {
            InitializeComponent();
            dtpFromDate.SelectedDate = DateTime.Today;
            dtpToDate.SelectedDate = DateTime.Today;

            dgvDetails.ItemsSource = db.PurchaseOrders.ToList();
            LoadWindow();
        }
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadWindow()
        {


            var v = db.Suppliers.ToList();
            cmbSupplier.ItemsSource = v;
            cmbSupplier.DisplayMemberPath = "LedgerName";
            cmbSupplier.SelectedValuePath = "LedgerName";
            var p = db.Purchases.ToList();

            if (dtpFromDate.Text != "")
            {
                DateTime d = Convert.ToDateTime(dtpFromDate.Text);
                p = db.Purchases.Where(x => x.PurchaseDate >= d).ToList();
            }
            if (dtpToDate.Text != "")
            {
                DateTime d = Convert.ToDateTime(dtpToDate.Text);
                p = p.Where(x => x.PurchaseDate <= d).ToList();
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

            
            if (cmbSupplier.Text != "")
            {
                p = p.Where(x => x.Supplier.LedgerName == cmbSupplier.Text).ToList();
            }

            dgvDetails.ItemsSource = p;

            if (p.Count == 0)
            {
                if (txtInvoiceNo .Text != "")
                {
                    decimal BillNo = Convert.ToDecimal(txtInvoiceNo.Text.ToString());
                    var p1 = db.Purchases.Where(x => x.InvoiceNo == BillNo).ToList();
                    dgvDetails.ItemsSource = p1;
                }
                else if (cmbSupplier.Text != "")
                {
                    var p2 = db.Purchases.Where(x => (x.Supplier == null ? "" : x.Supplier.LedgerName) == cmbSupplier.Text).OrderBy(x => x.PurchaseDate).ToList();
                    dgvDetails.ItemsSource = p2;
                }

            }
            else if (txtInvoiceNo.Text != "")
            {
                decimal BillNo1 = txtInvoiceNo.Text == "" ? 0 : Convert.ToDecimal(txtInvoiceNo.Text.ToString());
                var p2 = db.Purchases.Where(x => x.InvoiceNo == BillNo1).ToList();
                dgvDetails.ItemsSource = p2;
            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
            txtInvoiceNo.Clear();
            cmbSupplier.Text = "";
            txtBillAmtFrom.Clear();
            txtBillAmtTo.Clear();
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Purchase p = dgvDetails.SelectedItem as Purchase;
                PID = p.PurchaseId;
                this.Close();
            }
            catch(Exception ex)
            {
            }
        }


    }
}
