using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ProductWiseSaleReport.xaml
    /// </summary>
    public partial class ProductWiseSaleReport : UserControl
    {
        string qry = "";
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        public List ll = new List();
        public ProductWiseSaleReport()
        {
            InitializeComponent();
            LoadWindow();
            dtpFromDate.SelectedDate = DateTime.Now.AddDays(-1);
            dtpToDate.SelectedDate = DateTime.Now;
        }

        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
        private void dtpFromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dtpToDate.DisplayDateStart = dtpFromDate.SelectedDate.Value;
        }

        private void dtpToDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dtpFromDate.DisplayDateEnd = dtpToDate.SelectedDate.Value;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
            txtItem.Clear();
            cmbProduct.Text = "";
        }
        List<ProductReport> PRP = new List<ProductReport>();
        public void LoadWindow()
        {
           db = new JJSuperMarketEntities();

          
            PRP.Clear();
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            if (string.IsNullOrEmpty(cmbProduct.Text))
            {

                var lstSD = db.SalesDetails.Where(x => x.Sale.SalesDate >= dtpFromDate.SelectedDate.Value && x.Sale.SalesDate <= dtpToDate.SelectedDate.Value).ToList();
                foreach (var lst in lstSD)
                {
                    ProductReport p1 = new ProductReport();
                    var pro = db.Products.Where(x => x.ProductId == lst.ProductCode).ToList();
                    p1.ProductName = lst.Product.ProductName;
                    p1.SaleDate = string.Format("{0:dd/MM/yyyy}", lst.Sale.SalesDate.Value);
                    p1.Invoice = (decimal)lst.Sale.InvoiceNo;
                    p1.Qty = (double)lst.Quantity;
                    p1.Amount = (double)lst.Quantity * (double)lst.DisPer;
                    p1.PurchasaRate = (double)lst.Product.PurchaseRate;
                    p1.SellingRate = (double)lst.Product.SellingRate;
                    p1.ProfitAmount = (p1.SellingRate - p1.PurchasaRate) * p1.Qty;
                    PRP.Add(p1);
                }
                  dgvProductSale.ItemsSource = PRP.OrderBy(x=>x.Invoice) ;
               // dgvProductSale.ItemsSource = PRP.OrderBy(x => x.Invoice).GroupBy(x=>x.SaleDate ).Select(x=> new {  Name=x.Sum(y=>y.Qty) });
                dgvProductSale.Items.Refresh();

            }
            else

            {
                var s = cmbProduct.SelectedItem as Product;
                var lstSD = db.SalesDetails.Where(x => x.Sale.SalesDate >= dtpFromDate.SelectedDate.Value && x.Sale.SalesDate <= dtpToDate.SelectedDate.Value && x.Product.ProductName == s.ProductName).ToList();
                foreach (var lst1 in lstSD)
                {
                    ProductReport p1 = new ProductReport();
                    var pro = db.Products.Where(x => x.ProductId == lst1.ProductCode).ToList();
                    p1.ProductName = lst1.Product.ProductName;
                    p1.SaleDate = string.Format("{0:dd/MM/yyyy}", lst1.Sale.SalesDate.Value);
                    p1.Invoice = (decimal)lst1.Sale.InvoiceNo;
                    p1.Qty = (double)lst1.Quantity;
                    p1.Amount = (double)lst1.Quantity * (double)lst1.DisPer;
                    p1.PurchasaRate = (double)lst1.Product.PurchaseRate;
                    p1.SellingRate = (double)lst1.Product.SellingRate;
                    p1.ProfitAmount = (p1.SellingRate - p1.PurchasaRate) * p1.Qty;
                    PRP.Add(p1);
                }
                dgvProductSale.ItemsSource = PRP.OrderBy(x => x.Invoice) ; 

                dgvProductSale.Items.Refresh();
            }
            txtProfitAmount.Text = string.Format("{0:N2}", PRP.Sum(x => x.ProfitAmount));
            txtSaleAmount.Text = string.Format("{0:N2}", PRP.Sum(x => x.Amount));

            List<KeyValuePair<string, int>> MyValue1 = new List<KeyValuePair<string, int>>();
            MyValue1 = PRP.Select(x => new KeyValuePair<string, int>(string.Format("{0}", x.ProductName), (int)double.Parse(x.ProfitAmount.ToString()))).ToList();
            BarChart.DataContext = MyValue1;

            LoadReportData();
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
           
        }
        public void LoadReportData()
        {
            SalesReport.Reset();
            DataTable dt1 = GetData();
            ReportDataSource masterData = new ReportDataSource("ProductSale", dt1);

            SalesReport.LocalReport.DataSources.Add(masterData);
            SalesReport.LocalReport.ReportEmbeddedResource = "JJSuperMarket.Reports.rptProductSaleReport.rdlc";

          //  SalesReport.SetDisplayMode(DisplayMode.PrintLayout);
            SalesReport.RefreshReport();


        }
        private DataTable GetData()
        {
            // DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            //dt = ds.Tables.Add("ProductSale");
            dt.Columns.Add("ProductName");
            dt.Columns.Add("SaleDate");
            dt.Columns.Add("Invoice");
            dt.Columns.Add("Qty");
            dt.Columns.Add("Amount");
            dt.Columns.Add("PurchasaRate");
            dt.Columns.Add("SellingRate");
            dt.Columns.Add("ProfitAmount");

            int n = 0;
            foreach (var ll in PRP)
            {
                var nrow = dt.NewRow();
                nrow["ProductName"] = ll.ProductName;
                nrow["SaleDate"] = ll.SaleDate;
                nrow["Qty"] = ll.Qty;
                nrow["Amount"] = string.Format("{0:N2}", ll.Amount);
                nrow["PurchasaRate"] = string.Format("{0:N2}", ll.PurchasaRate);
                nrow["SellingRate"] = string.Format("{0:N2}", ll.SellingRate);
                nrow["ProfitAmount"] = string.Format("{0:N2}", ll.ProfitAmount);
                nrow["Invoice"] = ll.Invoice;
                dt.Rows.Add(nrow);
            }
            return dt;
        }
        class ProductReport
        {
            public string ProductName { get; set; }
            public string SaleDate { get; set; }
            public double Qty { get; set; }
            public double Amount { get; set; }
            public double PurchasaRate { get; set; }
            public double SellingRate { get; set; }
            public double ProfitAmount { get; set; }
            public decimal Invoice { get; set; }
        }

        private void txtItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            //cmbProduct.Text="";
            //if (string.IsNullOrEmpty(txtItem.Text))
            //{
            //    dgvProductSale.ItemsSource = db.Products.ToList().OrderBy(x => x.ProductName);
            //}
        }

        private void txtItem_KeyDown(object sender, KeyEventArgs e)
        {
           // JJSuperMarketEntities db = new JJSuperMarketEntities();
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtItem.Text))
                {
                    cmbProduct.Text = db.Products.Where(x => x.ItemCode == txtItem.Text).Select(x => x.ProductName).FirstOrDefault();
                }
                btnSearch.Focus();
            }
        }
        

        private void cmbProduct_TextChanged(object sender, RoutedEventArgs e)
        {
          //  JJSuperMarketEntities db = new JJSuperMarketEntities();
            cmbProduct.ItemsSource = db.Products.Where(p => p.ProductName.ToLower().Contains(cmbProduct.Text.ToLower())).ToList();
        }

        private void cmbProduct_DropDownOpened(object sender, EventArgs e)
        {
            var v = db.Products.ToList();
            cmbProduct.ItemsSource = v;
            cmbProduct.DisplayMemberPath = "ProductName";
            cmbProduct.SelectedValuePath = "ProductId";
        }
    }
}
