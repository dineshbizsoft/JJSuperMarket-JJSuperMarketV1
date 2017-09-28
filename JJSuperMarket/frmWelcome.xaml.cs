using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace JJSuperMarket
{
    /// <summary>
    /// Interaction logic for frmWelcome.xaml
    /// </summary>
    public partial class frmWelcome : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        public frmWelcome()
        {
            InitializeComponent();
            dtpDate.SelectedDate = DateTime.Now;
            dtpDate.DisplayDateEnd = DateTime.Now;
            dtpCDate.SelectedDate = DateTime.Now;
            LoadMonth();
        }

        private void LoadMonth()
        {
            DateTime m = Convert.ToDateTime("1/1/2017");
            List<GetMonthList> month = new List<GetMonthList>();
            GetMonthList g = new GetMonthList();
            for (int i = 0; i < 12; i++)
            {
                g = new GetMonthList();
                DateTime Nm = m.AddMonths(i);
                g.MonthName = Nm.ToString("MMMM");
                g.Value = Nm.Month;
                g.year = Nm.Year;
                month.Add(g);
            }
            cmbMonths.ItemsSource = month;
            cmbMonths.DisplayMemberPath = "MonthName";
            cmbMonths.SelectedValuePath = "Value";
            cmbMonths.SelectedIndex = DateTime.Now.AddMonths(-1).Month;

            cmbNPMonths.ItemsSource = month;
            cmbNPMonths.DisplayMemberPath = "MonthName";
            cmbNPMonths.SelectedValuePath = "Value";
            cmbNPMonths.SelectedIndex = DateTime.Now.AddMonths(-1).Month;
        }

        private void LoadWindow()
        {

            db = new JJSuperMarketEntities();
            //  StockDetails.UpdateStockDetails();
            DateTime dtFrom = DateTime.Now.AddDays(-7);
            DateTime dtTo = DateTime.Now;

            try
            {
                var list1 = db.Sales.GroupBy(x => x.SalesDate).OrderByDescending(x => x.Key).Take(30).Select(x => new { SalesDate = x.Key, BillAmount = x.Sum(y => y.ItemAmount) }).ToList();
                SalesGrid.ItemsSource = list1.Where(x => x.BillAmount != null).Select(x => new { Date = string.Format("{0:dd/MM/yyyy}", x.SalesDate), Amount = string.Format("{0:N2}", x.BillAmount) }).ToList();
                     
                var count = db.Sales.Where(x => x.SalesDate.Value == dtpDate.SelectedDate.Value).GroupBy(x => x.Customer.CustomerName).OrderBy(x => x.Sum(y => y.ItemAmount)).ToList();
                lblName.Content = count.Count().ToString() + " Customer Visited. [Details]";
                dgvSaleInfo.ItemsSource = count.Select(x => new { CustomerName = x.Key, Amount = string.Format("{0:N2}", x.Sum(y => y.ItemAmount)) }).ToList();

                List<CustomerPoints> CusPoint = new List<CustomerPoints>();

                foreach (var Cus in db.Customers.ToList())
                {
                    CustomerPoints c1 = new CustomerPoints();
                    c1.NoOfVisit = Cus.Sales.Count();
                    c1.CustomerName = Cus.CustomerName;
                    c1.TotalAmount = Convert.ToDecimal(string.Format("{0:N2}", Cus.Sales.Sum(x => x.ItemAmount)));
                    c1.Points = Convert.ToDecimal(string.Format("{0:N2}", Cus.Sales.Sum(x => x.ItemAmount) * 0.01));
                    CusPoint.Add(c1);
                }
                dgvCustomerInfo.ItemsSource = CusPoint.OrderByDescending(x => x.Points).Take(10).ToList();


                salesTypeReport();
                CalculateNetProfit();
                CategoryWiseSales();
            }
            catch (Exception ex) { }
        }

        private void CategoryWiseSales()
        {

            List<CategoryList> lst = new List<CategoryList>();
            CategoryList c = new CategoryList();
            var salesList = db.SalesDetails.Where(x => x.Sale.SalesDate ==dtpCDate.SelectedDate.Value)
                 .GroupBy(x => new { x.Sale.SalesDate, x.Product.StockGroup.GroupName }).ToList();
            foreach (var d in salesList)
            {
                c = new CategoryList();
                c.Category = d.Key.GroupName;
                c.Qty = string.Format("{0:N2}", d.Sum(y => y.Quantity));
                c.Amount = d.Sum(y => y.DisPer * y.Quantity) == null ? 0 : (decimal)d.Sum(y => y.DisPer * y.Quantity);

                if (c.Amount != null || c.Amount != 0) lst.Add(c);

            }
            dgvCatagorWiseSales.ItemsSource = lst;
        }

        private void salesTypeReport()
        {
            try
            {
                var m = cmbMonths.SelectedItem as GetMonthList;
                var sales1 = db.Sales.
                    Where(x => x.SalesDate.Value.Year == DateTime.Now.Year && x.SalesDate.Value.Month == m.Value).GroupBy(x => x.SalesType)
                    .ToList();
                SalesTypeReport st = new SalesTypeReport();
                List<SalesTypeReport> sList = new List<SalesTypeReport>();
                foreach(var  c in sales1)               
                {
                    st = new SalesTypeReport();
                    st.SalesType = c.Key ;
                    st.Amount = (double)c.Sum(y => y.ItemAmount);
                    if(st.Amount!=0)
                    {
                        sList.Add(st);
                    }
                        
                }
                st = new SalesTypeReport();
                st.SalesType = "Total";
                st.Amount = sList.Sum(x => x.Amount);
                sList.Add(st);
                dgvSalesTypeWiseReport.ItemsSource = sList;
            }
            catch (Exception ex)
            { }
        }

        private void PackIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //frmAccountGroups frm = new JJSuperMarket.frmAccountGroups() ;
            //frm.Show();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void dtpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtpDate.SelectedDate.Value != null)
                {
                    var count = db.Sales.Where(x => x.SalesDate.Value == dtpDate.SelectedDate.Value).GroupBy(x => x.Customer.CustomerName).ToList();
                    lblName.Content = count.Count().ToString() + " Customer Visited. [Details]";
                    dgvSaleInfo.ItemsSource = count.Select(x => new { CustomerName = x.Key, Amount = string.Format("{0:N2}", x.Sum(y => y.ItemAmount)) }).OrderBy(x => x.Amount).ToList();
                }
            }
            catch (Exception ex)
            {


            }


        }

        class CustomerPoints
        {
            public int NoOfVisit { get; set; }
            public string CustomerName { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal Points { get; set; }
        }
        class SalesTypeReport
        {
            public string SalesType { get; set; }
            public double Amount { get; set; }

        }
        class CategoryList
        {
           public string Category { get; set; }
            public string Qty { get; set; }
            public decimal? Amount { get; set; }
        }
        class GetMonthList
        {
            public string MonthName { get; set; }
            public int Value { get; set; }
            public int year { get; set; }
        }

        private void dtpCDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CategoryWiseSales();
            }
            catch (Exception ex)
            {

            }


        }

        private void dtpSTDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            salesTypeReport();
        }

        private void dtpPDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateNetProfit();
        }

        private void CalculateNetProfit()
        {
            var d = cmbNPMonths.SelectedItem as GetMonthList;
            DateTime startDate = new DateTime(d.year, d.Value, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            TimeSpan ts = startDate.Subtract(endDate);
            int n = Math.Abs(ts.Days);
            NetProfit np = new NetProfit();
            List<NetProfit> listNp = new List<NetProfit>();

            for (int i = 0; i <= n; i++)
            {
                double? sAmt = 0, pAmt = 0, Qty = 0;
                np = new NetProfit();
                np.Date = string.Format("{0:dd/MM/yyyy}", startDate.AddDays(i).Date);
                DateTime dtFrom = startDate.AddDays(i);
                var l = db.SalesDetails.Where(x => x.Sale.SalesDate == dtFrom).GroupBy(x => x.Product.ProductName);
                foreach (var pro in l)
                {
                    double amt = (double)pro.Sum(x => x.Quantity) * pro.Select(x => x.Product.PurchaseRate).FirstOrDefault().Value;
                    pAmt += amt;
                }
                foreach (var pro in l)
                {
                    double amt = (double)pro.Sum(x => x.Quantity) * pro.Select(x => x.Product.SellingRate).FirstOrDefault().Value;
                    sAmt += amt;
                    Qty += (double)pro.Sum(x => x.Quantity);
                }
                np.Qty = Qty;
                np.PurRate = pAmt == null ? 0 : (double)pAmt;
                sAmt = db.Sales.Where(x => x.SalesDate == dtFrom).Sum(x => x.ItemAmount);
                np.SelRate = sAmt == null ? 0 : (double)sAmt;
                if (np.PurRate != 0 || np.SelRate != 0)
                {
                    np.Profit = Math.Abs(np.SelRate.Value) - (np.PurRate.Value);
                    listNp.Add(np);
                }

            }
            np = new NetProfit();
            np.Date = "";
            np.PurRate = null;
            np.SelRate = null;
            np.Profit = listNp.Sum(x => x.Profit);
            np.Qty = listNp.Sum(x => x.Qty);
            listNp.Add(np);

            dgvProfitReport.ItemsSource = listNp;


        }

        class NetProfit
        {
            public string Date { get; set; }
            public double? PurRate { get; set; }
            public double? SelRate { get; set; }
            public double? Profit { get; set; }
            public double? Qty { get; set; }

        }

        private void dtpSTDate_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {

        }

        private void cmbMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            salesTypeReport();
        }

        private void cmbNPMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateNetProfit();
        }
    }
}
