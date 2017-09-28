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
    /// Interaction logic for frmSalesSearch.xaml
    /// </summary>
    public partial class frmSalesSearch : Window
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        public decimal SID = 0;

        public frmSalesSearch()
        {
            InitializeComponent();
            dtpFromDate.SelectedDate = DateTime.Today;
            dtpToDate.SelectedDate = DateTime.Today;

            dgvDetails.ItemsSource = db.SalesReturns.ToList();
            
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


            var v = db.Customers.ToList();
            cmbSupplier.ItemsSource = v;
            cmbSupplier.DisplayMemberPath = "CustomerName";
            cmbSupplier.SelectedValuePath = "CustomerName";
            var p = db.Sales.ToList();

            if (dtpFromDate.Text != "")
            {
                DateTime d = Convert.ToDateTime(dtpFromDate.Text);
                p = db.Sales.Where(x => x.SalesDate >= d).ToList();
            }
            if (dtpToDate.Text != "")
            {
                DateTime d = Convert.ToDateTime(dtpToDate.Text);
                p = p.Where(x => x.SalesDate <= d).ToList();
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

            if (cmbSalesType.Text != null)
            {
                    p = p.Where(x => x.SalesType == cmbSalesType.Text).ToList();
             }
            if (cmbSupplier.Text != "")
            {
                p = p.Where(x => (x.Customer==null?"" : x.Customer.CustomerName) == cmbSupplier.Text).ToList();
            }

            dgvDetails.ItemsSource = p;

            txtTotalAmount.Text = string.Format("{0:N2}", p.Sum(x => x.ItemAmount));

            if (p.Count == 0)
            {
                if (txtInvoiceNumber.Text != "")
                {
                    decimal BillNo = Convert.ToDecimal(txtInvoiceNumber.Text.ToString());
                    var p1 = db.Sales.Where(x => x.InvoiceNo == BillNo).ToList();
                    dgvDetails.ItemsSource = p1;
                    txtTotalAmount.Text = string.Format("{0:N2}", p1.Sum(x => x.ItemAmount));
                }
                else if (cmbSupplier.Text != "")
                {
                    var p2 = db.Sales.Where(x => (x.Customer == null ? "" : x.Customer.CustomerName) == cmbSupplier.Text).OrderBy(x => x.SalesDate).ToList();
                    dgvDetails.ItemsSource = p2;
                    txtTotalAmount.Text = string.Format("{0:N2}", p2.Sum(x => x.ItemAmount));
                }

            }
            else if (txtInvoiceNumber.Text != "")
            {
                decimal BillNo1 = txtInvoiceNumber.Text == "" ? 0 : Convert.ToDecimal(txtInvoiceNumber.Text.ToString());
                var p2 = db.Sales.Where(x => x.InvoiceNo == BillNo1).ToList();
                dgvDetails.ItemsSource = p2;
                txtTotalAmount.Text = string.Format("{0:N2}", p2.Sum(x => x.ItemAmount));
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
            txtInvoiceNumber.Clear();
           // txtTotalAmount.Clear();
            cmbSupplier.Text = "";
            txtBillAmtFrom.Clear();
            txtBillAmtTo.Clear();
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Sale p = dgvDetails.SelectedItem as Sale;

                SID = p.SalesId;
                this.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
