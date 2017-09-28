using AccountsBuddy.Domain;
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

namespace AccountsBuddy.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmBankSetup.xaml
    /// </summary>
    public partial class frmBankSetup : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        decimal ID = 0;
        public frmBankSetup()
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
                if (txtBank.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Bank Name.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtBank.Focus();
                }
                else if (txtLedgerName.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Ledger Name.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtLedgerName.Focus();
                }


                else if (ID != 0)
                {
                    bool r = true;
                    string name = db.Banks.Where(x => x.BankId == ID).Select(x => x.LedgerName).FirstOrDefault().ToString();
                    string name1 = db.Banks.Where(x => x.BankId == ID).Select(x => x.BankName).FirstOrDefault().ToString();
                    if (txtBank.Text != name)

                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }

                    }
                    else if (txtLedgerName.Text != name1)
                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }
                    }
                    else if (r == true)
                    {
                        Bank c = db.Banks.Where(x => x.BankId == ID).FirstOrDefault();
                        c.LedgerName = txtLedgerName.Text;
                        c.BankName = txtBank.Text;
                        c.AcType = cmbAccounttype.Text;
                        c.MCRCode = txtBranch.Text;
                        c.Address = txtAddress.Text;
                        c.Phone = txtPhone.Text;
                        c.CPerson1 = txtContactPerson1.Text;
                        c.MobileNo = txtMobile1.Text;
                        c.CPerson2 = txtContactPerson2.Text;
                        c.MobileNo2 = txtMobile2.Text;

                        Ledger l = db.Ledgers.Where(x => x.LedgerName == txtBank.ToString()).FirstOrDefault();
                        l.LedgerName = txtLedgerName.Text;
                        l.BillingName = txtBank.Text;
                        l.GroupCode = "Sundry Creditors";
                        l.Mobile = txtMobile1.Text;
                        l.Phone = txtPhone.Text;
                        l.Address = txtAddress.Text;
                        l.CrAmt = 0;
                        l.DrAmt = 0;

                        db.SaveChanges();
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = { Text = "Saved Successfully.." }
                        };

                        await DialogHost.Show(sampleMessageDialog, "RootDialog");
                        FormClear();
                        LoadWindow();

                    }


                }
                else
                {

                    if (await validation() == true)
                    {
                        Bank c = new Bank();
                        c.LedgerName = txtLedgerName.Text;
                        c.BankName = txtBank.Text;
                        c.AcType = cmbAccounttype.Text;
                        c.MCRCode = txtBranch.Text;
                        c.Address = txtAddress.Text;
                        c.Phone = txtPhone.Text;
                        c.CPerson1 = txtContactPerson1.Text;
                        c.MobileNo = txtMobile1.Text;
                        c.CPerson2 = txtContactPerson2.Text;
                        c.MobileNo2 = txtMobile2.Text;

                        db.Banks.Add(c);

                        Ledger l = new Ledger();
                        l.LedgerName = txtLedgerName.Text;
                        l.BillingName = txtBank.Text;
                        l.GroupCode = "Sundry Creditors";
                        l.Mobile = txtMobile1.Text;
                        l.Phone = txtPhone.Text;
                        l.Address = txtAddress.Text;
                        l.CrAmt = 0;
                        l.DrAmt = 0;

                        db.Ledgers.Add(l);
                        db.SaveChanges();
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = { Text = "Saved successfully.." }
                        };

                        await DialogHost.Show(sampleMessageDialog, "RootDialog");
                        FormClear();
                        LoadWindow();
                    }


                }
            }
            catch (Exception ex)
            { }
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FormClear();
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
                        Message = { Text = "Do you want to delete tis record?.." }
                    };

                    var result = Convert.ToBoolean(await DialogHost.Show(sampleDialog, "RootDialog"));
                    if (result == true)
                    {
                        Bank c = db.Banks.Where(x => x.BankId == ID).FirstOrDefault();
                        db.Banks.Remove(c);

                        Ledger l = db.Ledgers.Where(x => x.LedgerName == txtLedgerName.Text.ToString()).FirstOrDefault();
                        db.Ledgers.Remove(l);


                        db.SaveChanges();
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = { Text = "Deleted Successfully.." }
                        };

                        await DialogHost.Show(sampleMessageDialog, "RootDialog");
                        FormClear();
                        LoadWindow();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void btnSearchDetail_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }
        private void btnSearchReport_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }
        private void dgvBank_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                Bank c = dgvBank.SelectedItem as Bank;
                ID = c.BankId;
                txtBank.Text = c.LedgerName;
                txtLedgerName.Text = c.BankName;
                cmbAccounttype.Text = c.AcType;
                txtBranch.Text = c.MCRCode;
                txtPhone.Text = c.Phone;
                txtAddress.Text = c.Address;
                txtContactPerson1.Text = c.CPerson1;
                txtMobile1.Text = c.MobileNo.ToString();
                txtContactPerson2.Text = c.CPerson2;
                txtMobile2.Text = c.MobileNo2.ToString();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region User Define
        private void LoadWindow()
        {
            try
            {
                var c = db.Banks.ToList();
                cmbBankDetailSrch.ItemsSource = c;
                cmbBankDetailSrch.SelectedValuePath = "LedgerName";
                cmbBankDetailSrch.DisplayMemberPath = "LedgerName";

                cmbBankReportSrch.ItemsSource = c;
                cmbBankReportSrch.SelectedValuePath = "LedgerName";
                cmbBankReportSrch.DisplayMemberPath = "LedgerName";

                if (cmbBankDetailSrch.Text != "")
                {
                    dgvBank.ItemsSource = db.Banks.Where(x => x.LedgerName == cmbBankDetailSrch.Text).ToList();
                }
                else
                {
                    dgvBank.ItemsSource = db.Banks.ToList();
                }
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
                ReportDataSource Data = new ReportDataSource("Bank", dt);

                Reportviewer.LocalReport.DataSources.Add(Data);
                Reportviewer.LocalReport.ReportEmbeddedResource = "AccountsBuddy.Reports.Master.rptBankReport.rdlc";

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
                if (cmbBankReportSrch.Text != "")
                {
                    string qry = string.Format("Select * from Banks  where LedgerName='{0}'", cmbBankReportSrch.Text);
                    cmd = new SqlCommand(qry, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
                else
                {
                    string qry = string.Format("Select * from Banks  order by LedgerName");
                    cmd = new SqlCommand(qry, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }


            }
            return dt;
        }
        private async Task<bool> validation()
        {

            var b = db.Banks.Where(x => x.LedgerName == txtLedgerName.Text).Count();
            var b1 = db.Banks.Where(x => x.BankName == txtBank.Text).Count();

            if (b != 0)
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = "" + txtLedgerName.Text + ", Already Exist.Enter New One " }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");
                txtLedgerName.Focus();
                return false;
            }
            else if (b1 != 0)
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = "" + txtBank.Text + ",Already Exist!.Enter new Bank Name.." }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");
                txtBank.Focus();
                return false;

            }
            else
            {
                return true;
            }

        }
        private void FormClear()
        {
            txtBank.Clear();
            txtLedgerName.Clear();
            txtBranch.Clear();
            cmbAccounttype.Text = "";
            txtMobile1.Clear();
            txtMobile2.Clear();
            txtPhone.Clear();
            txtContactPerson1.Clear();
            txtAddress.Clear();
            txtContactPerson2.Clear();
            ID = 0;
        }

        #endregion


    }
}
