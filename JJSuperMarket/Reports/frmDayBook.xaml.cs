using System;
using System.Collections.Generic;
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

namespace JJSuperMarket.Reports
{
    /// <summary>
    /// Interaction logic for frmDayBook.xaml
    /// </summary>
    public partial class frmDayBook : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        string Wqry = "";
        public frmDayBook()
        {
            InitializeComponent();
            LoadReport();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }
        private void LoadReport()
        {
            var c = db.AccountGroups.ToList();
            cmbAccounts.ItemsSource = c;
            cmbAccounts.DisplayMemberPath = "GroupName";
            cmbAccounts.SelectedValuePath = "GroupName";

            var d = db.Ledgers.ToList();
            cmbLedger.ItemsSource = d;
            cmbLedger.DisplayMemberPath = "LedgerName";
            cmbLedger.SelectedValuePath = "LedgerName";

            dtpFromDate.SelectedDate = DateTime.Today;
            dtpToDate.SelectedDate = DateTime.Today;
            
            try
            {
                ReportViewer.Reset();
                DataTable dt = getData();
                ReportDataSource Data = new ReportDataSource("DayBook", dt);

                ReportViewer.LocalReport.DataSources.Add(Data);
                ReportViewer.LocalReport.ReportEmbeddedResource = "JJSuperMarket.Reports.rptDayBook.rdlc";
                ReportParameter[] rp= new ReportParameter[2];
                rp[0] = new ReportParameter("FromDate", String.Format("{0:MMM-yyyy}", dtpFromDate.SelectedDate.Value));
                rp[1] = new ReportParameter("ToDate", String.Format("{0:MMM-yyyy}", dtpToDate.SelectedDate.Value));
                ReportViewer.LocalReport.SetParameters(rp);
                ReportViewer.RefreshReport();
            }
            catch (Exception ex)
            {

            }
        }

        private DataTable getData()
        {
            WQRY();
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(AppLib.conStr))
            {
                SqlCommand cmd;
                String qry1 = "";
               
                    qry1 = string.Format("Select * from ViewLedgerReport where {0}", Wqry);

                cmd = new SqlCommand(qry1, con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            return dt;

        }
        private string WQRY()
        {
            Wqry = String.Format("LDate>={0:yyyy-MM-dd} and LDate<={1:yyyy-MM-dd}", dtpFromDate.Text, dtpToDate.Text); 
            if (cmbAccounts.Text != "")
            {
                Wqry =Wqry+ string.Format("and AccountGroup ='{0}'", cmbAccounts.Text);

            }
            if (cmbLedger.Text != "")
            {
               
                    Wqry = Wqry + string.Format("and LedgerName ='{0}'", cmbLedger.Text);
               
            }
            return Wqry;
        }

       
    }
}
