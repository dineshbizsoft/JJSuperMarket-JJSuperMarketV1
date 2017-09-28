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
using AccountsBuddy.Domain;
using MaterialDesignThemes.Wpf;
using Microsoft.Reporting.WinForms;

namespace AccountsBuddy.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmLedger.xaml
    /// </summary>
    public partial class frmLedger : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        decimal ID = 0;
        public frmLedger()
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
                if (txtLedgerName.Text == "")
                {

                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Ledger Name.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtLedgerName.Focus();
                }
                else if (cmbGroupName.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Group Name.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    cmbGroupName.Focus();
                }


                else if (ID != 0)
                {
                    bool r = true;
                    string name = db.Ledgers.Where(x => x.LedgerId == ID).Select(x => x.LedgerName).FirstOrDefault().ToString();
                    if (txtLedgerName.Text != name)

                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }

                    }
                    
                    else if (r == true)
                    {
                        Ledger c = db.Ledgers.Where(x => x.LedgerId == ID).FirstOrDefault();
                        c.LedgerName = txtLedgerName.Text;
                        c.BillingName = txtBillingName.Text;
                        c.GroupCode = cmbGroupName.SelectedValue.ToString();
                        c.Mobile = txtMobile.Text;
                        c.Phone = txtTelephone.Text;
                        c.MailId = txtEmail.Text;
                        c.Address = txtAddress.Text;
                        c.CreditDays =txtCreditDays.Text;
                        c.CreditLimit =txtCreditLimit.Text;
                        c.CrAmt = Convert.ToDouble(txtCrAmt.Text);
                        c.DrAmt = Convert.ToDouble(txtDrAmt.Text);

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
                        Ledger c = new Ledger();
                        c.LedgerName = txtLedgerName.Text;
                        c.BillingName = txtBillingName.Text;
                        c.GroupCode = cmbGroupName.SelectedValue.ToString();
                        c.Mobile = txtMobile.Text;
                        c.Phone = txtTelephone.Text;
                        c.MailId = txtEmail.Text;
                        c.Address = txtAddress.Text;
                        c.CreditDays = txtCreditDays.Text;
                        c.CreditLimit = txtCreditLimit.Text;
                        c.CrAmt =  Convert.ToDouble(txtCrAmt.Text);
                        c.DrAmt =  Convert.ToDouble(txtDrAmt.Text);

                        db.Ledgers.Add(c);
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
        private void btnSearchRep_Click(object sender, RoutedEventArgs e)
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
                        Message = { Text = "Do you want to delete tis record?.." }
                    };

                    var result = Convert.ToBoolean(await DialogHost.Show(sampleDialog, "RootDialog"));
                    if (result == true)
                    {
                        Ledger c = db.Ledgers.Where(x => x.LedgerId == ID).FirstOrDefault();
                        db.Ledgers.Remove(c);
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
            catch (Exception EX)
            {

            }

        }
        private void dgvLedger_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Ledger c = dgvLedger.SelectedItem as Ledger;
                ID = c.LedgerId;
                txtLedgerName.Text = c.LedgerName;
                txtBillingName.Text = c.BillingName;
                cmbGroupName.Text = c.GroupCode;
                txtTelephone.Text = c.Phone;
                txtMobile.Text = c.Mobile;
                txtAddress.Text = c.Address;
                txtCreditDays.Text = c.CreditDays.ToString();
                txtCreditLimit.Text = c.CreditLimit.ToString();
                txtDrAmt.Text = c.DrAmt.ToString();
                txtCrAmt.Text = c.CrAmt.ToString();
            }
            catch (Exception ex)
            { }
        }
        #endregion

        #region User Define
        private async Task<bool> validation()
        {

            var b = db.Ledgers.Where(x => x.LedgerName == txtLedgerName.Text).Count();
          
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
           
            else
            {
                return true;
            }


        }
        private void FormClear()
        {
            txtLedgerName.Clear();
            txtBillingName.Clear();
            cmbGroupName.Text = "";
            txtMobile.Clear();
            txtTelephone.Clear();
            txtEmail.Clear();
            txtCreditLimit.Clear();
            txtCreditDays.Clear();
            txtAddress.Clear();
            txtDrAmt.Text="0";
            txtCrAmt.Text = "0";
            ID = 0;
        }
        private void LoadWindow()
        {
            try
            {
                var c = db.AccountGroups.ToList();
                cmbGroupName.ItemsSource = c;
                cmbGroupName.SelectedValuePath = "GroupCode";
                cmbGroupName.DisplayMemberPath = "GroupCode";

                var c1 = db.Ledgers.ToList();
                cmbLedgerSrch.ItemsSource = c1;
                cmbLedgerSrch.SelectedValuePath = "LedgerName";
                cmbLedgerSrch.DisplayMemberPath = "LedgerName";

                cmbLedger.ItemsSource = c1;
                cmbLedger.SelectedValuePath = "LedgerName";
                cmbLedger.DisplayMemberPath = "LedgerName";

                if (cmbLedgerSrch.Text != "")
                {
                    dgvLedger.ItemsSource = db.Ledgers.Where(x => x.LedgerName == cmbLedger.Text).ToList();
                }
                else
                {
                    dgvLedger.ItemsSource = db.Ledgers.ToList();
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
                ReportDataSource Data = new ReportDataSource("Ledger", dt);

                Reportviewer.LocalReport.DataSources.Add(Data);
                Reportviewer.LocalReport.ReportEmbeddedResource = "AccountsBuddy.Reports.Master.rptLedger.rdlc";

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
                if (cmbLedgerSrch.Text != "")
                {
                    string qry = string.Format("Select * from Ledger  where LedgerName='{0}'", cmbLedgerSrch.Text);
                    cmd = new SqlCommand(qry, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
                else
                {
                    string qry = string.Format("Select * from Ledger  order by LedgerName");
                    cmd = new SqlCommand(qry, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }


            }
            return dt;
        }

    }

    #endregion
}

