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
using JJSuperMarket.Domain;
using Microsoft.Reporting.WinForms;

namespace JJSuperMarket.Transaction
{
    /// <summary>
    /// Interaction logic for frmSalesReturnReport.xaml
    /// </summary>
    public partial class frmSalesReturnReport : Window
    {
        string qry = "";
        JJSuperMarketEntities db = new JJSuperMarketEntities();

        public frmSalesReturnReport()
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
                SalesReturnReport.Reset();
                DataTable dt = getData();
                SalesReturnReport.Reset();
                ReportDataSource Data = new ReportDataSource("SalesReturn", dt);

                SalesReturnReport.LocalReport.DataSources.Add(Data);
                SalesReturnReport.LocalReport.ReportEmbeddedResource = "JJSuperMarket.Reports.Transaction.rptSalesReturn.rdlc";
                SalesReturnReport.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(PurchaseDetails);

                SalesReturnReport.RefreshReport();
            }
            catch (Exception ex)
            {

            }
        }

        private void PurchaseDetails(object sender, SubreportProcessingEventArgs e)
        {
            int c = int.Parse(e.Parameters["SRId"].Values[0]);
            DataTable dt = GetDetails(c);
            ReportDataSource rs = new ReportDataSource("SalesReturnDetails", dt);
            e.DataSources.Add(rs);
        }

        private DataTable GetDetails(int c)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppLib.conStr))
            {
                string qry = String.Format("select P.ProductName as ProductCode, POD.Quantity, POD.Rate, U.UOMSymbol as UOM,POD.TaxPer, POD.DisPer from SalesReturnDetails as POD left join Products as P on POD.ProductCode = p.ProductId left join UnitsOfMeasurement as U on POD.UOM=U.UOMId where POD.SRId='{0}'", c);
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
                string qry1 = string.Format("select   PO.SRId,s.CustomerName as LedgerCode,PO.SRDate, PO.InvoiceNo,PO.DiscountAmount,PO.Extra,PO.ItemAmount from SalesReturn as PO left join Customer as s on PO.LedgerCode = s.CustomerId where {0}", qry);
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
            qry = String.Format("PO.SRDate>='{0:yyyy-MM-dd}' and PO.SRDate<='{1:yyyy-MM-dd}' and PO.ItemAmount>='{2}' and PO.ItemAmount<='{3}'", fromDate, toDate, billFrom, billTo);
            if (cmbCustomer.Text != "")
            {

                qry = qry + "and S.CustomerName='" + cmbCustomer.Text + "'";

            }
            if (txtInvoiceNo.Text != "")
            {
                qry = qry + "and PO.InvoiceNo='" + txtInvoiceNo.Text + "'";

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