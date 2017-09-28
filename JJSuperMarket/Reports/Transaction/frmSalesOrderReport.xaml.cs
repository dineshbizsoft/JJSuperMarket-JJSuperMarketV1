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
using System.Windows.Shapes;
using AccountsBuddy.Domain;
using Microsoft.Reporting.WinForms;

namespace AccountsBuddy.Reports.Transaction
{
    /// <summary>
    /// Interaction logic for frmSalesOrderReport.xaml
    /// </summary>
    public partial class frmSalesOrderReport : Window
    {
        string qry = "";
        JJSuperMarketEntities db = new JJSuperMarketEntities();

        public frmSalesOrderReport()
        {
            InitializeComponent();
            LoadReport();
            LoadWindow();

        }

        private void LoadWindow()
        {
            dtpFromDate.SelectedDate = DateTime.Today;
            dtpToDate.SelectedDate = DateTime.Today;
            txtBillAmtFrom.Text = "0";
            txtBillAmtTo.Text = "99999999";
            var v = db.Customers.ToList();
            cmbCustomer.ItemsSource = v;
            cmbCustomer.DisplayMemberPath = "CustomerName";
            cmbCustomer.SelectedValuePath = "CustomerName";
        }

        private void LoadReport()
        {
            try
            {
                PurchaseReport.Reset();
                DataTable dt = getData();
                //DataTable dt = GetDetails(9);
                ReportDataSource Data = new ReportDataSource("SalesOrder", dt);

                PurchaseReport.LocalReport.DataSources.Add(Data);
                PurchaseReport.LocalReport.ReportEmbeddedResource = "AccountsBuddy.Reports.Transaction.rptSalesOrderReport.rdlc";
                PurchaseReport.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SalesDetails);

                PurchaseReport.RefreshReport();
            }
            catch (Exception ex)
            {

            }
        }

        private void SalesDetails(object sender, SubreportProcessingEventArgs e)
        {
            int c = int.Parse(e.Parameters["SOId"].Values[0]);
            DataTable dt = GetDetails(c);
            ReportDataSource rs = new ReportDataSource("SalesOrderDetails", dt);
            e.DataSources.Add(rs);
        }

        private DataTable GetDetails(int c)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppLib.conStr))
            {
                string qry = String.Format("select P.ProductName as ProductCode, SOD.Quantity, SOD.Rate,SOD.TaxPer,U.UOMSymbol as UOM, SOD.DisPer from SalesOrderDetails as SOD left join Products as P on SOD.ProductCode = p.ProductId left join UnitsOfMeasurement as U on SOD.UOM=U.UOMId where SOD.SOId='{0}'", c);
                SqlCommand cmd = new SqlCommand(qry, conn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            return dt;
        }

        private DataTable getData()
        {
            Wqry();
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(AppLib.conStr))
            {
                SqlCommand cmd;
                string qry1 = string.Format("select SO.SOId,s.CustomerName as LedgerCode,SO.SODate, SO.InvoiceNo,SO.DiscountAmount,SO.Extra,SO.ItemAmount from SalesOrder as SO left join Customer as s on So.LedgerCode = s.CustomerId where {0}", qry);
                cmd = new SqlCommand(qry1, con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            return dt;

        }

        public string Wqry()
        {
            DateTime fromDate = Convert.ToDateTime(dtpFromDate.SelectedDate);
            DateTime toDate = Convert.ToDateTime(dtpToDate.SelectedDate);
            Double billFrom = Convert.ToDouble(txtBillAmtFrom.Text);
            Double billTo = Convert.ToDouble(txtBillAmtTo.Text);
            qry = String.Format("SO.SODate>='{0:yyyy-MM-dd}' and SO.SODate<='{1:yyyy-MM-dd}' and SO.ItemAmount>='{2}' and SO.ItemAmount<='{3}'", fromDate, toDate, billFrom, billTo);
            if (cmbCustomer.Text != "")
            {

                qry = qry + "and S.CustomerName='" + cmbCustomer.Text + "'";

            }
            if (txtInvoiceNo.Text != "")
            {
                qry = qry + "and SO.InvoiceNo='" + txtInvoiceNo.Text + "'";

            }

            return qry;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }
    }
}
