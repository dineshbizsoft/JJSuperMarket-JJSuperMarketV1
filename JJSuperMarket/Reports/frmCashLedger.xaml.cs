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
    /// Interaction logic for frmCashLedger.xaml
    /// </summary>
    public partial class frmCashLedger : UserControl
    {
        string CrAmt, DrAmt;
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        DataTable dt = new DataTable();
        public frmCashLedger()
        {
            InitializeComponent();
            dtpFromDate.SelectedDate = DateTime.Today;
            dtpToDate.SelectedDate = DateTime.Today;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();

        }
        private void LoadReport()
        {
            

            try
            {
                ReportViewer.Reset();
                DataTable dt = getData();
                ReportDataSource Data = new ReportDataSource("LedgerReport", dt);

                ReportViewer.LocalReport.DataSources.Add(Data);
                ReportViewer.LocalReport.ReportEmbeddedResource = "JJSuperMarket.Reports.rptCashLedger.rdlc";
                ReportParameter[] rp = new ReportParameter[4];
                rp[0] = new ReportParameter("FromDate", String.Format("{0:dd-MM-yyyy}", dtpFromDate.SelectedDate.Value));
                rp[1] = new ReportParameter("ToDate", String.Format("{0:dd-MM-yyyy}", dtpToDate.SelectedDate.Value));
                rp[2] = new ReportParameter("CrAmt", String.Format("{0:00}", dtpFromDate.SelectedDate.Value));
                rp[3] = new ReportParameter("DrAmt", String.Format("{0:00}", dtpToDate.SelectedDate.Value));

                ReportViewer.LocalReport.SetParameters(rp);
                ReportViewer.RefreshReport();
            }
            catch (Exception ex)
            {

            }
        }

        private DataTable getData()
        {

            
            using (SqlConnection con = new SqlConnection(AppLib.conStr))
            {
                SqlCommand cmd;
                String qry1 = "";

                qry1 = string.Format("Select * from ViewLedgerReport where LDate>='{0:yyyy-MM-dd}' and LDate<='{1:yyyy-MM-dd}'", dtpFromDate.SelectedDate.Value, dtpToDate.SelectedDate.Value);
                
                cmd = new SqlCommand(qry1, con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

                CrAmt = db.LedgerReports.Where(x=>x.LDate>=dtpFromDate.SelectedDate.Value && x.LDate<=dtpToDate.SelectedDate.Value).Sum(x => x.CRAmount).ToString();
                DrAmt = db.LedgerReports.Where(x => x.LDate >= dtpFromDate.SelectedDate.Value && x.LDate <= dtpToDate.SelectedDate.Value).Sum(x => x.DRAmount).ToString();

            }
            return dt;

        }
    }
}
