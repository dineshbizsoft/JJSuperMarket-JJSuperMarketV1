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
using MahApps.Metro.Controls;

namespace AccountsBuddy.Reports
{
    /// <summary>
    /// Interaction logic for frmPurchaseOrderSearch.xaml
    /// </summary>
    public partial class frmPurchaseOrderSearch : Window
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        public decimal POID = 0;
        public frmPurchaseOrderSearch()
        {
            InitializeComponent();
            dtpFromDate.SelectedDate = DateTime.Today;
            dtpToDate.SelectedDate = DateTime.Today;
          
            dgvDetails.ItemsSource = db.PurchaseOrders.ToList();
            LoadWindow();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadWindow()
        {
           

            var v = db.Suppliers.ToList();
            cmbSupplier.ItemsSource = v;
            cmbSupplier.DisplayMemberPath = "SupplierName";
            cmbSupplier.SelectedValuePath = "SupplierName";
            var p = db.PurchaseOrders.ToList();

            if (dtpFromDate.Text != "")
            {
                DateTime d = Convert.ToDateTime(dtpFromDate.Text);
               p = db.PurchaseOrders.Where(x => x.PODate >= d ).ToList();
            }
            if (dtpToDate.Text != "")
            {
                DateTime d = Convert.ToDateTime(dtpToDate.Text);
               p = p.Where(x => x.PODate <= d).ToList();
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
                decimal d=Convert.ToDecimal(txtInvoiceNo.Text.ToString());
                p = p.Where(x => x.InvoiceNo == d).ToList();
            }
            if (cmbSupplier.Text != "")
            {
                p = p.Where(x => x.Supplier.SupplierName==cmbSupplier.Text).ToList();
            }
           
                dgvDetails.ItemsSource = p;
            


        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PurchaseOrder p = dgvDetails.SelectedItem as PurchaseOrder;

            POID = p.POId;            
            this.Close();
        }

        
    }
}
