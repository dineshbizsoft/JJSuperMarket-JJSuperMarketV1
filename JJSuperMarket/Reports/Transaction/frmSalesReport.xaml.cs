﻿using System;
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
using System.Text.RegularExpressions;
using System.Data.Entity.Core.Objects;


namespace JJSuperMarket.Reports.Transaction
{
    /// <summary>
    /// Interaction logic for frmSalesReport.xaml
    /// </summary>
    public partial class frmSalesReport : UserControl 
    {
        string qry = "";
        string qry1 = "";
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        double AmtFrom = 0; double AmtTo = 10000;
        public frmSalesReport()
        {
            InitializeComponent();
            LoadReport();
            LoadWindow();

        }
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
       
        #endregion
        private void LoadWindow()
        {
            dtpFromDate.SelectedDate = DateTime.Today.AddDays(-30) ;
            dtpToDate.SelectedDate = DateTime.Today;
            txtBillAmtFrom.Text = AmtFrom.ToString() ;
            txtBillAmtTo.Text = AmtTo.ToString() ;
            var v = db.Customers.ToList();
            cmbCustomer.ItemsSource = v;
            cmbCustomer.DisplayMemberPath = "CustomerName";
            cmbCustomer.SelectedValuePath = "CustomerName";
           
        }

        private void LoadReport()
        {
            try
            {
                SalesReport.Reset();
              //  DataTable dt = getData();

                var cName = cmbCustomer.SelectedValue;
                var InNo = txtInvoiceNo.Text;

               AmtFrom= Convert.ToDouble(txtBillAmtFrom.Text.ToString());
              AmtTo= Convert.ToDouble(txtBillAmtTo.Text.ToString());

                var s = db.Sales.Where(x => EntityFunctions.TruncateTime(x.SalesDate) >= dtpFromDate.SelectedDate&& 
                                        EntityFunctions.TruncateTime(x.SalesDate) <= dtpToDate.SelectedDate).ToList();
                

            
                s = s.Where(x => x.ItemAmount >= Convert.ToDouble(txtBillAmtFrom.Text.ToString())
                        && x.ItemAmount <= Convert.ToDouble(txtBillAmtTo.Text.ToString())).ToList();

                s = s.Where(x => x.SalesType == cmbSalesType.Text.ToString() 
                        && (cName==null||x.Customer.CustomerName==cName.ToString())
                        && (InNo == ""|| x.InvoiceNo == Convert.ToDecimal(InNo))
                        ).ToList();



               ReportDataSource Data = new ReportDataSource("Sales", 
                   s.Select(x=>new {SalesId=x.SalesId,SalesCode=x.SalesCode, x.LedgerCode,x.Narration ,  CustomerName = x.Customer.CustomerName,
                       SalesDate = x.SalesDate, InvoiceNo = x.InvoiceNo, DiscountAmount = x.DiscountAmount, Extra = x.Extra, ItemAmount = x.ItemAmount, SalesType = x.SalesType }).ToList());
               // ReportDataSource Data = new ReportDataSource("Sales", s);

                SalesReport.LocalReport.DataSources.Add(Data);
                SalesReport.LocalReport.ReportEmbeddedResource = "JJSuperMarket.Reports.Transaction.rptSales.rdlc";
                SalesReport.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(PurchaseDetails);

                SalesReport.RefreshReport();
             
                OverAllReportLoad();

               
            }
            catch (Exception ex)
            {

            }
        }

        private void OverAllReportLoad()
        {
            OverAllReport.Reset();

            var v2 = db.Sales.Where(x => EntityFunctions.TruncateTime(x.SalesDate) >= dtpFromDate.SelectedDate && EntityFunctions.TruncateTime(x.SalesDate) <= dtpToDate.SelectedDate && x.ItemAmount >= AmtFrom && x.ItemAmount <= AmtTo)
                .Select(x => new { CustomerName = x.Customer.CustomerName, PhoneNumber = x.Customer.MobileNo, Amount = x.ItemAmount }).ToList();

            //DataTable dt1 = getOverAllData();
            ReportDataSource Data1 = new ReportDataSource("SalesReport", v2);
            OverAllReport.LocalReport.DataSources.Add(Data1);
            OverAllReport.LocalReport.ReportEmbeddedResource = "JJSuperMarket.Reports.Transaction.rptOverAllSalesReport.rdlc";
            OverAllReport.RefreshReport();
        }

        private void PurchaseDetails(object sender, SubreportProcessingEventArgs e)
        {
            int c = int.Parse(e.Parameters["SalesId"].Values[0]);
            
            var v2 = db.SalesDetails.Where(x => x.SalesId==c)
                .Select(x => new { ProductName = x.Product.ProductName,Rate=x.Rate,  Quantity = x.Quantity, UOMSymbol=x.Product.UnitsOfMeasurement.UOMSymbol, TaxPer =x.TaxPer, DisPer=x.DisPer, Amount=(x.DisPer*x.Quantity)}).ToList();
            ReportDataSource rs = new ReportDataSource("SalesDetails", v2);
            e.DataSources.Add(rs);
        }

        //private DataTable GetDetails(int c)
        //{
        //    DataTable dt = new DataTable();
        //    using (SqlConnection conn = new SqlConnection(AppLib.conStr))
        //    {
        //        string qry = String.Format("select P.ProductName as ProductCode, POD.Quantity,U.UOMSymbol as UOM, POD.Rate,POD.TaxPer, POD.DisPer from SalesDetails as POD left join Products as P on POD.ProductCode = p.ProductId left join UnitsOfMeasurement as U on POD.UOM=U.UOMId where POD.SalesId='{0}'", c);
        //        SqlCommand cmd = new SqlCommand(qry, conn);
        //        SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //        adp.Fill(dt);
        //    }
        //    return dt;
        //}

        //private DataTable getData()
        //{
        //    Wqry();
        //    DateTime fromDate = Convert.ToDateTime(dtpFromDate.SelectedDate);
        //    DateTime toDate = Convert.ToDateTime(dtpToDate.SelectedDate);
        //    Double billFrom = Convert.ToDouble(txtBillAmtFrom.Text);
        //    Double billTo = Convert.ToDouble(txtBillAmtTo.Text);
        //    DataTable dt = new DataTable();
        //    using (SqlConnection con = new SqlConnection(AppLib.conStr))
        //    {
        //        SqlCommand cmd;
        //        var s = db.SalesDetails.Where(x => EntityFunctions.TruncateTime(x.Sale.SalesDate) >= fromDate && EntityFunctions.TruncateTime(x.Sale.SalesDate) <= toDate && x.Sale.ItemAmount >= billFrom && x.Sale.ItemAmount >= billTo).ToList();

        //        string qry1 = string.Format("select   PO.SalesId,s.CustomerName as LedgerCode,PO.SalesDate, PO.InvoiceNo,PO.DiscountAmount,PO.Extra,PO.ItemAmount,po.salestype from Sales as PO left join Customer as s on PO.LedgerCode = s.CustomerId where {0}", qry);
        //        cmd = new SqlCommand(qry1, con);
        //        SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //        adp.Fill(dt);
        //    }
        //    return dt;

        //}
        //private DataTable getOverAllData()
        //{
        //    Wqry1();
        //    DataTable dt = new DataTable();
        //    using (SqlConnection con = new SqlConnection(AppLib.conStr))
        //    {
        //        SqlCommand cmd;
        //        string Sqry1 = string.Format("select c.CustomerName as CustomerName ,c.MobileNo as PhoneNumber ,SUM(s.ItemAmount) as Amount from Sales as S join Customer as C on c.CustomerId = S.LedgerCode where {0} Group by CustomerName, c.MobileNo order by CustomerName", qry1);
        //        cmd = new SqlCommand(Sqry1, con);
        //        SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //        adp.Fill(dt);
        //    }
        //    return dt;

        //}
        //public string Wqry()
        //{
        //    DateTime fromDate = Convert.ToDateTime(dtpFromDate.SelectedDate);
        //    DateTime toDate = Convert.ToDateTime(dtpToDate.SelectedDate);
        //    Double billFrom = Convert.ToDouble(txtBillAmtFrom.Text);
        //    Double billTo = Convert.ToDouble(txtBillAmtTo.Text);
        //    qry = String.Format("PO.SalesDate>='{0:yyyy-MM-dd}' and PO.SalesDate<='{1:yyyy-MM-dd}' and PO.ItemAmount>='{2}' and PO.ItemAmount<='{3}'", fromDate, toDate, billFrom, billTo);
        //    if (cmbCustomer.Text != "")
        //    {

        //        qry = qry + "and S.CustomerName='" + cmbCustomer.Text + "'";

        //    }
        //    if (txtInvoiceNo.Text != "")
        //    {
        //        qry = qry + "and PO.InvoiceNo='" + txtInvoiceNo.Text + "'";

        //    }
        //    if (cmbSalesType.Text != "")
        //    {
        //        qry=qry+"and po.salestype='"+cmbSalesType.Text+"'";
        //    }

        //   return qry;
        //}

        //public string Wqry1()
        //{
        //    DateTime fromDate = Convert.ToDateTime(dtpFromDate.SelectedDate);
        //    DateTime toDate = Convert.ToDateTime(dtpToDate.SelectedDate);
        //    Double billFrom = Convert.ToDouble(txtBillAmtFrom.Text);
        //    Double billTo = Convert.ToDouble(txtBillAmtTo.Text);
        //    qry1 = String.Format("S.SalesDate>='{0:yyyy-MM-dd}' and S.SalesDate<='{1:yyyy-MM-dd}' and S.ItemAmount>='{2}' and S.ItemAmount<='{3}'", fromDate, toDate, billFrom, billTo);

        //    return qry1;
        //}

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadReport();
            }
            catch (Exception ex)
            { }
        }
    }
}
