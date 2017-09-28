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

namespace JJSuperMarket.Reports.Transaction
{
    /// <summary>
    /// Interaction logic for frmCustomerPoint.xaml
    /// </summary>
    public partial class frmCustomerPoint : Window
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        public frmCustomerPoint()
        {
            InitializeComponent();
            var v = db.Customers.ToList();
            points();
            cmbSupplier.ItemsSource = v;
            cmbSupplier.DisplayMemberPath = "CustomerName";
            cmbSupplier.SelectedValuePath = "CustomerName";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            points();
        }
        private void points()
        {
            List<CustomerPoints> CusPoint = new List<CustomerPoints>();
            foreach (var Cus in cmbSupplier.Text==""? db.Customers.ToList():db.Customers.Where(x=>x.CustomerName==cmbSupplier.Text).ToList())
            {
                 


                    CustomerPoints c1 = new CustomerPoints();
                // c1.Date =  Convert.ToDateTime( Cus.Sales.Select(x=>x.SalesDate.Value));
                    c1.CustomerName = Cus.CustomerName;
                    c1.ItemAmount = Convert.ToDecimal(string.Format("{0:N2}",  Cus.Sales.Sum(x=>x.ItemAmount)));
                var d = Math.Abs((decimal)(Cus.Sales.Sum(x => x.ItemAmount)- Cus.Sales.Sum(x => x.ItemAmount) * 0.01));
                var Amt1 =(double)Cus.Sales.Where(x => x.SalesType != "Redeem").Sum(x => x.ItemAmount)*0.01 ;
                var Amt2 = (double)Cus.Sales.Where(x => x.SalesType == "Redeem").Sum(x => x.ItemAmount);
                c1.Points = Convert.ToDecimal(string.Format("{0:N2}", Math.Abs(Amt1 - Amt2) ));
                CusPoint.Add(c1);

                
            }
            dgvDetails.ItemsSource = CusPoint.OrderByDescending(x=>x.Points).ToList();
           // txtTotalAmount.Text = CusPoint.Where(x=>x.CustomerName==cmbSupplier.Text).Select(x => x.Points).ToString();
        }
        class CustomerPoints
        {    
            public DateTime Date { get; set; }
            public string CustomerName { get; set; }
            public decimal ItemAmount { get; set; }
            public decimal Points { get; set; }
        }
    }
}
