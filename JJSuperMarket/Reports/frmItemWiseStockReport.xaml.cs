using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
using JJSuperMarket.Domain;
using Microsoft.Reporting.WinForms;
using MaterialDesignThemes.Wpf;
using System.Text.RegularExpressions;

namespace JJSuperMarket.Reports
{
    /// <summary>
    /// Interaction logic for frmItemWiseStockReport.xaml
    /// </summary>
    public partial class frmItemWiseStockReport : UserControl
    {
       JJSuperMarketEntities db = new JJSuperMarketEntities();
        List<Product> lstProduct = new List<Product>();
        decimal GId;
        public frmItemWiseStockReport()
        {
            InitializeComponent();
            dtpFromDate.SelectedDate = DateTime.Today.AddDays (-1);
            dtpToDate.SelectedDate = DateTime.Today;
           
        }
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            GId = db.StockGroups.Where(x => x.GroupName.ToLower() == cmbGroupUnder.Text.ToLower()).Select(x => x.StockGroupId).FirstOrDefault();
            LoadReport();
            txtProductNAme.Clear();
            cmbItemName.Text = "";
            txtItemCode.Clear();
            cmbGroupUnder.Text = "";
        }
        private void LoadReport()
        {
            if (cmbGroupUnder.Text == "")
            {
                dgvStockDetails.ItemsSource = StockDetails.GetStockDetails(cmbItemName.Text, dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
            }
            if (cmbGroupUnder.Text != null)
            {
                dgvStockDetails.ItemsSource = StockDetails.GetStockDetails(cmbItemName.Text, dtpFromDate.SelectedDate, dtpToDate.SelectedDate, cmbGroupUnder.Text == "" ? 0 : GId);
            }
            if (!string.IsNullOrEmpty(txtProductNAme.Text))
            {
                dgvStockDetails.ItemsSource = StockDetails.GetStockDetails(txtProductNAme.Text, dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
            }

        }

        private void cmbItemName_TextChanged(object sender, RoutedEventArgs e)
        {
            
                try
                {
                    cmbItemName.ItemsSource = lstProduct.Where(p => p.ProductName.ToLower().Contains(cmbItemName.Text.ToLower())).ToList();
                    //txtItemCode.Text = lstProduct.Where(p => p.ProductName.ToLower() == cmbItemName.Text.ToLower()).Select(x=>x.ItemCode).FirstOrDefault();
                }
                catch (Exception ex)
                {
                }
            
          
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            db = new JJSuperMarketEntities();
             
            lstProduct = db.Products.OrderBy(x => x.ProductName).ToList();
             
            cmbItemName.ItemsSource = lstProduct.ToList();

           // LoadReport();
        }

        private async  void txtItemCode_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                var b1 = lstProduct.Where(x => x.ItemCode == txtItemCode.Text).FirstOrDefault();
                if (b1 == null)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Barcode Not Found.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");

                    txtItemCode.Focus();
                }
                else
                {
                    if (b1 != null)
                    {
                        cmbItemName.Text = b1 .ProductName;   
                    }
                  
                }
            }
        }

        private void PackIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            JJSuperMarket.Transaction.frmProductDetails frm = new JJSuperMarket.Transaction.frmProductDetails();
            frm.ShowDialog();
            ProductName(frm.ProName);

        }
        public void ProductName(string Name)
        {
            if (Name != null)
            {
                var data = lstProduct.Where(x => x.ProductName == Name).FirstOrDefault();

                cmbItemName.Text = Name;
                txtItemCode.Text = data.ItemCode == "" ? "" : data.ItemCode.ToString();
            }

        }

        private void txtProductNAme_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void cmbGroupUnder_DropDownOpened(object sender, EventArgs e)
        {
            cmbGroupUnder.ItemsSource = db.StockGroups.ToList();
            cmbGroupUnder.DisplayMemberPath = "GroupName";
            cmbGroupUnder.SelectedValuePath = "StockGroupId";
        }
    }
}
