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

namespace JJSuperMarket.Reports
{
    /// <summary>
    /// Interaction logic for frmReOrderLevel.xaml
    /// </summary>
    public partial class frmReOrderLevel : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        
     
        public frmReOrderLevel()
        {
            InitializeComponent();
             
        }
        
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        private void LoadReport()
        {

            dgvStockDetails.ItemsSource = StockDetails.toList.Where(x => x.ClStock <= x.ReOrderLevel).ToList();
        }

        private void txtItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtItem.Text))
            {
                dgvStockDetails.ItemsSource = db.Products.ToList().OrderBy(x => x.ProductName);
            }

        }
        private void txtItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dgvStockDetails.ItemsSource = db.Products.Where(x => x.ItemCode == txtItem.Text).ToList();
            }
        }
        private void cmbProductSrch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cmbProductSrch.Text))
            {
                dgvStockDetails.ItemsSource = db.Products.Where(x => x.ProductName.ToLower().Contains(cmbProductSrch.Text.ToLower())).OrderBy(x => x.ProductName).ToList();
            }
            else
            {
                dgvStockDetails.ItemsSource = db.Products.ToList();
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            LoadReport();

        }
    }
}
