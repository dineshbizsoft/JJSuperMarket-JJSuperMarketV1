using JJSuperMarket.Domain;
using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for frmCateogeryWiseSalesReport.xaml
    /// </summary>
    public partial class frmCateogeryWiseSalesReport : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        decimal GId;
        public frmCateogeryWiseSalesReport()
        {
            InitializeComponent();
            dtpFromDate.SelectedDate = DateTime.Today.AddDays(-1);
            dtpToDate.SelectedDate = DateTime.Today;
        }
        private void cmbGroupUnder_DropDownOpened(object sender, EventArgs e)
        {
            cmbGroupUnder.ItemsSource = db.StockGroups.ToList();
            cmbGroupUnder.DisplayMemberPath = "GroupName";
            cmbGroupUnder.SelectedValuePath = "StockGroupId";
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            GId = db.StockGroups.Where(x => x.GroupName.ToLower() == cmbGroupUnder.Text.ToLower()).Select(x => x.StockGroupId).FirstOrDefault();
            LoadReport();
           
            cmbGroupUnder.Text = ""; cmbProduct.Text = ""; 
        }
        private async void LoadReport()
        {
            db = new JJSuperMarketEntities();
            if (!string.IsNullOrWhiteSpace (cmbGroupUnder.Text))
            {
                decimal code;
                decimal Pcode;

               if (cmbGroupUnder.SelectedValue == null)
                {
                    code = db.Products.Where(x => x.StockGroup.GroupName == cmbGroupUnder.Text).Select(x => x.StockGroup.StockGroupId).FirstOrDefault(); 
                }
                else
                {
                    code = (decimal)cmbGroupUnder.SelectedValue;
                }
                if (cmbProduct.SelectedValue == null)
                {
                    Pcode = db.Products.Where(x => x.ProductName  == cmbProduct .Text).Select(x => x.ProductId ).FirstOrDefault();
                }
                else
                {
                    Pcode = (decimal)cmbProduct.SelectedValue;
                }
                
                if(code != null || code != 0)
                {
                   
                    var sinfo = db.SalesDetails.Where(x => x.Sale.SalesDate >= dtpFromDate.SelectedDate.Value && x.Sale.SalesDate.Value <= dtpToDate.SelectedDate.Value && x.Product.GroupCode == code  ).ToList();
                    if(Pcode != 0)
                    {
                        sinfo = sinfo.Where(x => x.Product.ProductId == Pcode).ToList();  
                    }

                    dgvSaleDetail.ItemsSource = sinfo.Select(x => new { Date = string.Format("{0:dd/MM/yy}", x.Sale.SalesDate), ProductName = x.Product.ProductName, Qty = x.Quantity, Amount = string.Format("{0:N2}", (x.DisPer * x.Quantity)) }).OrderByDescending(x=>x.Date).ToList();
                }
                //dgvStockDetails.ItemsSource = StockDetails.GetStockDetails("", dtpFromDate.SelectedDate, dtpToDate.SelectedDate, cmbGroupUnder.Text == "" ? 0 : GId); 
            }
            else
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = "Please select the Categoty.." }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");

                cmbGroupUnder.Focus();
            }
        }

        private void cmbProduct_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(cmbGroupUnder.Text))
                {
                    cmbProduct.ItemsSource = db.Products.Where(x => x.GroupCode == (decimal)cmbGroupUnder.SelectedValue).ToList();
                    cmbProduct.DisplayMemberPath = "ProductName";
                    cmbProduct.SelectedValuePath = "ProductId";
                }
                else
                {
                    cmbProduct.ItemsSource = db.Products.ToList();
                    cmbProduct.DisplayMemberPath = "ProductName";
                    cmbProduct.SelectedValuePath = "ProductId";
                }

            }
            catch (Exception ex)
            {

            }
          
        }

        private void cmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var p = cmbProduct.SelectedItem as Product;
                cmbGroupUnder.Text = p.StockGroup.GroupName;
            }
            catch (Exception ex)
            {


            }
         
        }

        private void txtItemCode_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                var res = db.Products.Where(x => x.ItemCode == txtItemCode.Text).ToList();
                cmbProduct.Text = res.Select(x => x.ProductName).FirstOrDefault();
                cmbGroupUnder.Text = res.Select(x => x.StockGroup.GroupName).FirstOrDefault();
            }
        }
    }
}
