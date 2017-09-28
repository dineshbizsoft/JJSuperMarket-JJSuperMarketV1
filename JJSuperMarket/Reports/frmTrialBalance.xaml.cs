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

namespace JJSuperMarket.Reports
{
    /// <summary>
    /// Interaction logic for frmTrialBalance.xaml
    /// </summary>
    public partial class frmTrialBalance : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        public  ObservableCollection<TrialBalance> toList = new ObservableCollection<TrialBalance>();
           
        public frmTrialBalance()
        {
            InitializeComponent();
          //dgvTrialBalance.ItemsSource = toList;
            LoadWindow();
        }

        private void LoadWindow()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(AppLib.conStr))
            {
                con.Open();

                String Qry = String.Format("Select Particulars, CRAmount, DRAmount from ViewLedgerReport");
                SqlCommand cmd = new SqlCommand(Qry, con);
                SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                sdp.Fill(dt);
            }
            var datas = (from DataRow row in dt.Rows
                         select new
                         {
                             Particulars = (string)(row["Particulars"] ?? ""),
                             CRAmount = (double)(row["CRAmount"] ?? 0),
                             DRAmount = (double)(row["DRAmount"] ?? 0)
                         } 
                         ).ToList();
            
            var datas1 = datas.Select(x => new TrialBalance
            {
                Particulars = x.Particulars,
                CrAmt=x.CRAmount, 
                DrAmt=x.DRAmount, 
               
                 
            })
            ;
            txtCrAmount.Text = datas.Sum(x => x.CRAmount).ToString();
            txtDrAmount.Text = datas.Sum(x => x.DRAmount).ToString();
            txtDifference.Text = datas.Sum(x => x.CRAmount - x.DRAmount).ToString();
            dgvTrialBalance.ItemsSource=datas1.ToList();
        }
        public  class TrialBalance
        {
            public string Particulars;
            public double CrAmt;
            public double DrAmt;
            public double TotalCrAmt;
            public double TotalDrAmt;
        }


    }
}
