using JJSuperMarket.Domain;
using MaterialDesignThemes.Wpf;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace JJSuperMarket.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmSupplier.xaml
    /// </summary>
    public partial class frmSupplier : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        decimal ID = 0;
        public frmSupplier()
        {
            InitializeComponent();
            LoadWindow();
            LoadReport();
        }
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #region Button Events
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtPersonIncharge.Text == "")
                {

                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Person Incharge.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtPersonIncharge.Focus();
                }
                else if (txtCompanyName.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Company Name.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtCompanyName.Focus();
                }


                else if (ID != 0)
                {
                    bool r = true;
                    string name = db.Suppliers.Where(x => x.SupplierId == ID).Select(x => x.LedgerName).FirstOrDefault().ToString();
                    string name1 = db.Suppliers.Where(x => x.SupplierId == ID).Select(x => x.SupplierName).FirstOrDefault().ToString();
                    if (txtCompanyName.Text != name)

                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }

                    }
                    if (txtPersonIncharge.Text != name1)
                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }
                    }
                    if (r == true)
                    {
                        Supplier c = db.Suppliers.Where(x => x.SupplierId == ID).FirstOrDefault();
                        c.SupplierName = txtPersonIncharge.Text;
                        c.LedgerName = txtCompanyName.Text;
                        c.MobileNo = txtMobile.Text;
                        c.TelePhoneNo = txtTelephone.Text;
                        c.AddressLine = txtAddress.Text;
                        c.EMailId = txtEmail.Text;
                        c.CreditDays = (txtCreditDays.Text == "" ? 0 : Convert.ToDecimal(txtCreditDays.Text));
                        c.CreditLimits = (txtCreditLimit.Text == "" ? 0 : Convert.ToDouble(txtCreditLimit.Text));
                        c.CST = txtGST.Text;
                        c.TinNo = txtTinNo.Text;

                        //Ledger l = db.Ledgers.Where(x => x.BillingName  == txtCompanyName.Text.ToString()).FirstOrDefault();
                        //l.LedgerName = txtCompanyName.Text;
                        //l.BillingName = txtPersonIncharge.Text;
                        //l.GroupCode = "Sundry Creditors";
                        //l.Mobile = txtMobile.Text;
                        //l.Phone = txtTelephone.Text;
                        //l.MailId = txtEmail.Text;
                        //l.Address = txtAddress.Text;
                        //l.CreditDays = txtCreditDays.Text;
                        //l.CreditLimit = txtCreditLimit.Text;
                        //l.CrAmt = 0;
                        //l.DrAmt = 0;


                        db.SaveChanges();
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = { Text = "Updated Successfully.." }
                        };

                        await DialogHost.Show(sampleMessageDialog, "RootDialog");
                        FormClear();
                        LoadWindow();
                        LoadReport();

                    }


                }
                else
                {

                    if (await validation() == true)
                    {
                        Supplier c = new Supplier();
                        c.SupplierName = txtPersonIncharge.Text;
                        c.LedgerName = txtCompanyName.Text;
                        c.MobileNo = txtMobile.Text;
                        c.TelePhoneNo = txtTelephone.Text;
                        c.AddressLine = txtAddress.Text;
                        c.EMailId = txtEmail.Text;
                        c.CreditDays = (txtCreditDays.Text == "" ? 0 : Convert.ToDecimal(txtCreditDays.Text));
                        c.CreditLimits = (txtCreditLimit.Text == "" ? 0 : Convert.ToDouble(txtCreditLimit.Text));
                        c.CST = txtGST.Text;
                        c.TinNo = txtTinNo.Text;

                        db.Suppliers.Add(c);

                        //Ledger l = new Ledger();
                        //l.LedgerName = txtCompanyName.Text;
                        //l.BillingName = txtPersonIncharge.Text;
                        //l.GroupCode = "Sundry Creditors";
                        //l.Mobile = txtMobile.Text;
                        //l.Phone = txtTelephone.Text;
                        //l.MailId = txtEmail.Text;
                        //l.Address = txtAddress.Text;
                        //l.CreditDays = txtCreditDays.Text;
                        //l.CreditLimit = txtCreditLimit.Text;
                        //l.CrAmt = 0;
                        //l.DrAmt = 0;

                        //db.Ledgers.Add(l);

                        db.SaveChanges();
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = { Text = "Saved successfully.." }
                        };

                        await DialogHost.Show(sampleMessageDialog, "RootDialog");
                        FormClear();
                        LoadWindow();
                        LoadReport();
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FormClear();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }
        private void btnSearch1_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ID == 0)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "No Records to Delete.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                }
                else
                {
                    var sampleDialog = new SampleDialog
                    {
                        Message = { Text = "Do you want to delete this record?.." }
                    };

                    var result = Convert.ToBoolean(await DialogHost.Show(sampleDialog, "RootDialog"));
                    if (result == true)
                    {
                        Ledger l = db.Ledgers.Where(x => x.BillingName == txtCompanyName.Text.ToString()).FirstOrDefault();
                        if (l != null)
                        {
                            db.Ledgers.Remove(l);
                            db.SaveChanges();
                        }


                        Supplier c = db.Suppliers.Where(x => x.SupplierId == ID).FirstOrDefault();
                        db.Suppliers.Remove(c);
                        db.SaveChanges();
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = { Text = "Deleted Successfully.." }
                        };

                        await DialogHost.Show(sampleMessageDialog, "RootDialog");
                        FormClear();
                        LoadWindow();
                        LoadReport();
                    }
                }

            }
            catch (Exception EX)
            {
                MessageBox.Show(" Can't Delete Contact Admin..");
                db = new JJSuperMarketEntities();
                FormClear();
                LoadWindow();
                LoadReport();
            }

        }
        private void dgvSupplier_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SupplierDetail c = dgvSupplier.SelectedItem as SupplierDetail;
                ID = c.SupplierId;
                txtPersonIncharge.Text = c.SupplierName;
                txtCompanyName.Text = c.LedgerName;
                txtMobile.Text = c.MobileNo;
                txtTelephone.Text = c.TelePhoneNo;
                txtEmail.Text = c.EMailId;
                txtAddress.Text = c.AddressLine;
                txtCreditDays.Text = c.CreditDays.ToString();
                txtCreditLimit.Text = c.CreditLimits.ToString();
                txtTinNo.Text = c.TinNo;
            }
            catch (Exception ex)
            { }
        }
        #endregion

        #region User Define
        private async Task<bool> validation()
        {

            var b = db.Suppliers.Where(x => x.LedgerName == txtCompanyName.Text).Count();
            var b1 = db.Suppliers.Where(x => x.SupplierName == txtPersonIncharge.Text).Count();

            if (b != 0)
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = "" + txtCompanyName.Text + ", Already Exist.Enter New One " }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");
                txtCompanyName.Focus();
                return false;
            }
            else if (b1 != 0)
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = "" + txtPersonIncharge.Text + ",Already Exist!.Enter new Person Incharge.." }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");
                txtPersonIncharge.Focus();
                return false;

            }
            else
            {
                return true;
            }


        }
        private void FormClear()
        {
            txtCompanyName.Clear();
            txtPersonIncharge.Clear();
            txtTelephone.Clear();
            txtMobile.Clear();
            txtGST.Clear();
            txtEmail.Clear();
            txtCreditLimit.Clear();
            txtCreditDays.Clear();
            txtAddress.Clear();
            txtTinNo.Clear();
            ID = 0;
        }
        private void LoadWindow()
        {
            try
            {
                Supplier_Search(db.Suppliers.ToList());
            }
            catch (Exception ex)
            {

            }
        }
        private void LoadReport()
        {
            try
            {
                Reportviewer.Reset();
                DataTable dt = getData();
                ReportDataSource Data = new ReportDataSource("Supplier", dt);

                Reportviewer.LocalReport.DataSources.Add(Data);
                Reportviewer.LocalReport.ReportEmbeddedResource = "JJSuperMarket.Reports.Master.rptSupplierReport.rdlc";

                Reportviewer.RefreshReport();
            }
            catch (Exception ex)
            {

            }

        }
        private DataTable getData()
        {

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(AppLib.conStr))
            {
                SqlCommand cmd;


                string qry = string.Format("Select * from Supplier  order by LedgerName");
                cmd = new SqlCommand(qry, con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);



            }
            return dt;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }


        private void txtSupplierSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSupplierSearch.Text))
            {
                Supplier_Search(db.Suppliers.Where(x => x.SupplierName.ToLower().Contains(txtSupplierSearch.Text.ToLower())).OrderBy(x => x.SupplierId).ToList());
            }
            else
            {
                Supplier_Search(db.Suppliers.ToList());
            }
        }

        private void Supplier_Search(List<Supplier> p)
        {
            SupplierDetail pc = new SupplierDetail();
            List<SupplierDetail> p1 = new List<SupplierDetail>();
            var pr = p;
            int n = 0;
            foreach (var p2 in pr)
            {
                pc = new SupplierDetail();
                pc.SupplierId = p2.SupplierId;
                n = n + 1;
                pc.SNo = n;
                pc.SupplierName = p2.SupplierName;
                pc.AddressLine = p2.AddressLine;
                pc.CreditDays = p2.CreditDays;
                pc.CreditLimits = p2.CreditLimits;
                pc.CST = p2.CST;
                pc.SupplierCode = p2.SupplierCode;
                pc.EMailId = p2.EMailId;
                pc.LedgerName = p2.LedgerName;
                pc.MobileNo = p2.MobileNo;
                pc.TelePhoneNo = p2.TelePhoneNo;
                pc.TinNo = p2.TinNo;
                p1.Add(pc);

            }
            dgvSupplier.ItemsSource = p1;
        }
    }
    #endregion

    public class SupplierDetail
    {
        public int SNo { get; set; }
        public decimal SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string LedgerName { get; set; }
        public string SupplierName { get; set; }
        public string AddressLine { get; set; }
        public string TelePhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string EMailId { get; set; }
        public string TinNo { get; set; }
        public string CST { get; set; }
        public Nullable<decimal> CreditDays { get; set; }
        public Nullable<double> CreditLimits { get; set; }
    }
        
}

