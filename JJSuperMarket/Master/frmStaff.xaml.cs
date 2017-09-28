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
    /// Interaction logic for frmStaff.xaml
    /// </summary>
    public partial class frmStaff : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        decimal ID = 0;
        public frmStaff()
        {
            InitializeComponent();
            dtpDOJ.SelectedDate = DateTime.Today;
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
                if (txtStaffName.Text == "")
                {

                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Staff Name.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtStaffName.Focus();
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
                    string name = db.Staffs.Where(x => x.StaffId == ID).Select(x => x.LedgerName).FirstOrDefault().ToString();
                    string name1 = db.Staffs.Where(x => x.StaffId == ID).Select(x => x.StaffName).FirstOrDefault().ToString();
                    if (txtStaffName.Text != name1)

                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }

                    }
                    else if (txtLedgerName.Text != name)
                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }
                    }
                    else if (r == true)
                    {
                        Staff c = db.Staffs.Where(x => x.StaffId == ID).FirstOrDefault();
                        c.LedgerName = txtLedgerName.Text;
                        c.StaffName = txtStaffName.Text;
                        c.Designation = txtDesignation.Text;
                        c.DOJ = dtpDOJ.SelectedDate.Value;
                        c.Address = txtAddress.Text;
                        c.ContactNum = txtContact.Text;
                        c.MailId = txtEmail.Text;

                        Ledger l = db.Ledgers.Where(x => x.LedgerName == txtLedgerName.Text.ToString()).FirstOrDefault();
                        l.LedgerName = txtLedgerName.Text;
                        l.BillingName = txtStaffName.Text;
                        l.GroupCode = "Sundry Debtors";
                        l.Mobile = txtContact.Text;
                        l.MailId = txtEmail.Text;
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
                        Staff c = new Staff();
                        c.LedgerName = txtLedgerName.Text;
                        c.StaffName = txtStaffName.Text;
                        c.Designation = txtDesignation.Text;
                        c.DOJ = dtpDOJ.SelectedDate.Value;
                        c.Address = txtAddress.Text;
                        c.ContactNum = txtContact.Text;
                        c.MailId = txtEmail.Text;
                        db.Staffs.Add(c);

                        Ledger l = new Ledger();
                        l.LedgerName = txtLedgerName.Text;
                        l.BillingName = txtStaffName.Text;
                        l.GroupCode = "Sundry Debtors";
                        l.Mobile = txtContact.Text;
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
            catch(Exception ex)
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
                        Staff c = db.Staffs.Where(x => x.StaffId == ID).FirstOrDefault();
                        db.Staffs.Remove(c);

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
            catch(Exception ex)
            { }
        }
        private void btnSearchDetail_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }
        private void btnSearchReport_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }
        private void dgvStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Staff c = dgvStaff.SelectedItem as Staff;
                ID = c.StaffId;
                txtLedgerName.Text = c.LedgerName;
                txtStaffName.Text = c.StaffName;
                txtDesignation.Text = c.Designation;
                dtpDOJ.SelectedDate = c.DOJ;
                txtAddress.Text = c.Address;
                txtContact.Text = c.ContactNum;
                txtEmail.Text = c.MailId;
            }
            catch(Exception ex)
            { }
        }
        #endregion

        #region User Define
        private void LoadWindow()
        {
            try
            {
                var c = db.Staffs.ToList();
                cmbStaffDetails.ItemsSource = c;
                cmbStaffDetails.SelectedValuePath = "LedgerName";
                cmbStaffDetails.DisplayMemberPath = "LedgerName";

                cmbStaffReport.ItemsSource = c;
                cmbStaffReport.SelectedValuePath = "LedgerName";
                cmbStaffReport.DisplayMemberPath = "LedgerName";

                if (cmbStaffDetails.Text != "")
                {
                    dgvStaff.ItemsSource = db.Staffs.Where(x => x.LedgerName == cmbStaffDetails.Text).ToList();
                }
                else
                {
                    dgvStaff.ItemsSource = db.Staffs.ToList();
                }
            }
            catch(Exception ex)
            { }
        }
        private void LoadReport()
        {
            try
            {
                Reportviewer.Reset();
                DataTable dt = getData();
                ReportDataSource Data = new ReportDataSource("Staff", dt);

                Reportviewer.LocalReport.DataSources.Add(Data);
                Reportviewer.LocalReport.ReportEmbeddedResource = "AccountsBuddy.Reports.Master.rptStaffReport.rdlc";

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
                if (cmbStaffReport.Text != "")
                {
                    string qry = string.Format("Select * from Staff  where LedgerName='{0}'", cmbStaffReport.Text);
                    cmd = new SqlCommand(qry, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
                else
                {
                    string qry = string.Format("Select * from Staff  order by LedgerName");
                    cmd = new SqlCommand(qry, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }


            }
            return dt;
        }
        private async Task<bool> validation()
        {

            var b = db.Staffs.Where(x => x.LedgerName == txtLedgerName.Text).Count();
            var b1 = db.Staffs.Where(x => x.StaffName == txtStaffName.Text).Count();

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
                    Message = { Text = "" + txtStaffName.Text + ",Already Exist!.Enter new Staff Name.." }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");
                txtStaffName.Focus();
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
            txtStaffName.Clear();
            txtDesignation.Text = "";
            dtpDOJ.SelectedDate = DateTime.Today;
            txtAddress.Clear();
            txtContact.Clear();
            txtEmail.Clear();
            ID = 0;
        }
        #endregion

    }
}
