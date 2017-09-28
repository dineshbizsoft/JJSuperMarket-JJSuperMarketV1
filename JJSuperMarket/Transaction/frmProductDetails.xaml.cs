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
using MahApps.Metro.Controls;

namespace JJSuperMarket.Transaction
{
    /// <summary>
    /// Interaction logic for frmProductDetails.xaml
    /// </summary>
    public partial class frmProductDetails :Window
    {
        List<Product> lstProduct = new List<Product>();
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        public string ProName;
        public frmProductDetails()
        {
            InitializeComponent();
            lstProduct = db.Products.OrderBy(x => x.ProductName).ToList();
        }
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
        private void cmbProductSrch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cmbProductSrch.Text))
            {
                
                var p = lstProduct.Where(x => x.ProductName.ToLower().Contains(cmbProductSrch.Text.ToLower())).ToList();
                ProductDetails pc = new ProductDetails();
                List<ProductDetails> p1 = new List<ProductDetails>();
                int n = 0;
                foreach (var p2 in p)
                {
                    pc = new ProductDetails();
                    pc.ProductName = p2.ProductName;
                    n = n + 1;
                    pc.SNo = n;
                    pc.Under = p2.StockGroup.GroupName;
                    pc.PurchaseRate = p2.PurchaseRate.Value;
                    pc.SellingRate = p2.SellingRate.Value;
                    pc.MRP = p2.MRP.Value;

                    p1.Add(pc);
                }
                dgvProduct.ItemsSource = p1;
            }
        }

        private void dgvProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ProductDetails  p = dgvProduct .SelectedItem as ProductDetails;

                ProName  = p.ProductName;
                this.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtItem.Focus();
            ProductDetails pc = new ProductDetails();
            List<ProductDetails> p1 = new List<ProductDetails>();
            int n = 0;
            foreach (var p2 in lstProduct.ToList())
            {
                pc = new ProductDetails();
                pc.ProductName = p2.ProductName;
                n = n + 1;
                pc.SNo = n;
                pc.Under = p2.StockGroup.GroupName;
                pc.PurchaseRate = p2.PurchaseRate.Value;
                pc.SellingRate = p2.SellingRate.Value;
                pc.MRP = p2.MRP.Value;

                p1.Add(pc);
            }
            dgvProduct.ItemsSource = p1; 
        }

        private void txtItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
            
                var p = db.Products.Where(x => x.ItemCode == txtItem.Text).ToList();
                ProductDetails pc = new ProductDetails();
                List<ProductDetails> p1 = new List<ProductDetails>();
                int n = 0;
                foreach (var p2 in p)
                {
                    pc = new ProductDetails();
                    pc.ProductName = p2.ProductName;
                    n = n + 1;
                    pc.SNo = n;
                    pc.Under = p2.StockGroup.GroupName;
                    pc.PurchaseRate = p2.PurchaseRate.Value;
                    pc.SellingRate = p2.SellingRate.Value;
                    pc.MRP = p2.MRP.Value;

                    p1.Add(pc);
                }
                dgvProduct.ItemsSource = p1;
            }
        }

        private void txtItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtItem.Text))
           {
                 var p = lstProduct.ToList();
                ProductDetails pc = new ProductDetails();
                List<ProductDetails> p1 = new List<ProductDetails>();
                int n = 0;
                foreach (var p2 in p)
                {
                    pc = new ProductDetails();
                    pc.ProductName = p2.ProductName;
                    n = n + 1;
                    pc.SNo = n;
                    pc.Under = p2.StockGroup.GroupName;
                    pc.PurchaseRate = p2.PurchaseRate.Value;
                    pc.SellingRate = p2.SellingRate.Value;
                    pc.MRP = p2.MRP.Value;

                    p1.Add(pc);
                }
                dgvProduct.ItemsSource = p1;
            }
        }
    }
}
