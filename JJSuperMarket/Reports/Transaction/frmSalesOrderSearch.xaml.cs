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
using System.Windows.Shapes;

namespace AccountsBuddy.Reports
{
    /// <summary>
    /// Interaction logic for frmSalesOrderSearch.xaml
    /// </summary>
    public partial class frmSalesOrderSearch : Window
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        public decimal SOID = 0;
        public frmSalesOrderSearch()
        {
            InitializeComponent();
            dtpFromDate.SelectedDate = DateTime.Today;
            dtpToDate.SelectedDate = DateTime.Today;

            dgvDetails.ItemsSource = db.SalesOrders.ToList();
            LoadWindow();
        }
        private void LoadWindow()
        {


            var v = db.Customers.ToList();
            cmbCustomer.ItemsSource = v;
            cmbCustomer.DisplayMemberPath = "CustomerName";
            cmbCustomer.SelectedValuePath = "CustomerName";
            var p = db.SalesOrders.ToList();

            if (dtpFromDate.Text != "")
            {
                DateTime d = Convert.ToDateTime(dtpFromDate.Text);
                p = db.SalesOrders.Where(x => x.SODate >= d).ToList();
            }
            if (dtpToDate.Text != "")
            {
                DateTime d = Convert.ToDateTime(dtpToDate.Text);
                p = p.Where(x => x.SODate <= d).ToList();
            }
            if (txtBillAmtFrom.Text != "")
            {
                double bill = Convert.ToDouble(txtBillAmtFrom.Text.ToString());
                p = p.Where(x => x.ItemAmount == bill).ToList();
            }
            if (txtBillAmtTo.Text != "")
            {
                double bill = Convert.ToDouble(txtBillAmtTo.Text.ToString());
                p = p.Where(x => x.ItemAmount == bill).ToList();
            }

            if (txtInvoiceNo.Text != "")
            {
                decimal d = Convert.ToDecimal(txtInvoiceNo.Text.ToString());
                p = p.Where(x => x.InvoiceNo == d).ToList();
            }
            if (cmbCustomer.Text != "")
            {
                p = p.Where(x => x.Customer.CustomerName == cmbCustomer.Text).ToList();
            }

            dgvDetails.ItemsSource = p;



        }

        

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SalesOrder p = dgvDetails.SelectedItem as SalesOrder;

            SOID = p.SOId;
            this.Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}

