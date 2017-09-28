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
using AccountsBuddy.Domain;
using MaterialDesignThemes.Wpf;
using Microsoft.Reporting.WinForms;

namespace AccountsBuddy.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmAccountGroups.xaml
    /// </summary>
    public partial class frmAccountGroups : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        decimal ID = 0;
        public frmAccountGroups()
        {
            InitializeComponent();

            //var v = db.AccountGroups.Select(x=>new { GroupName=x.GroupName,Under=x.AccountGroup1.GroupName}). ToList();
            //trvAccount.Items.Clear();
            //trvAccount.ItemsSource = v;

           

            LoadReport();
            LoadWindow();
        }

        #region Button Events



        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (txtGroupName.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Group Name.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtGroupName.Focus();
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
                    string name = db.AccountGroups.Where(x => x.AccountGroupId == ID).Select(x => x.GroupName).FirstOrDefault().ToString();
                    //string name1 = db.AccountGroups.Where(x => x.AccountGroupId == ID).Select(x => x.).FirstOrDefault().ToString();
                    if (txtGroupName.Text != name)

                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }

                    }
                    //else if (txtGroupCode.Text != name1)
                    //{
                    //    if (await validation() == false)
                    //    {
                    //        r = false;
                    //    }
                    //}
                    else if (r == true)
                    {
                        AccountGroup c = db.AccountGroups.Where(x => x.AccountGroupId == ID).FirstOrDefault();
                        c.AccountGroupCode = txtGroupCode.Text;
                        c.GroupName = txtGroupName.Text;
                        c.Under = (cmbGroupName.Text == null ? 0 : Convert.ToDecimal(cmbGroupName.SelectedValue));

                        db.SaveChanges();
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = { Text = "Saved Successfully.." }
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
                        AccountGroup c = new AccountGroup();
                        c.AccountGroupCode = txtGroupCode.Text;
                        c.GroupName = txtGroupName.Text;
                        c.Under = (cmbGroupName.Text == null ? 0 : Convert.ToDecimal(cmbGroupName.SelectedValue));

                        db.AccountGroups.Add(c);
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
            catch(Exception ex)
            {

            }
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
                        AccountGroup c = db.AccountGroups.Where(x => x.AccountGroupId == ID).FirstOrDefault();
                        db.AccountGroups.Remove(c);
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
            catch(Exception ex)
            {

            }
        }

        private void btnSearchReport_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void dgvAccount_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AccountGroup c = dgvAccountGroup.SelectedItem as AccountGroup;
                ID = c.AccountGroupId;
                txtGroupCode.Text = c.AccountGroupCode;
                txtGroupName.Text = c.GroupName;
                cmbGroupName.Text = c.AccountGroup1.GroupName;
            }
            catch(Exception ex)
            {

            }
        }
        #endregion

        #region User Defined
        private async Task<bool> validation()
        {
            
                var b = db.AccountGroups.Where(x => x.GroupName == txtGroupName.Text).Count();
                var b1 = db.AccountGroups.Where(x => x.GroupCode == txtGroupCode.Text).Count();

                if (b != 0)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "" + txtGroupName.Text + ", Already Exist.Enter New One " }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtGroupName.Focus();
                    return false;
                }
                else if (b1 != 0)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "" + txtGroupCode.Text + ",Already Exist!.Enter new code.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtGroupCode.Focus();
                    return false;

                }
                else
                {
                    return true;
                }
            }
           
        

        private void FormClear()
        {
            txtGroupCode.Clear();
            txtGroupName.Clear();
            cmbGroupName.Text = "";
            ID = 0;
        }

        private void LoadWindow()
        {
            try
            {
                var c = db.AccountGroups.ToList();
                cmbGroupName.ItemsSource = c;
                cmbGroupName.SelectedValuePath = "AccountGroupId";
                cmbGroupName.DisplayMemberPath = "GroupName";

                var group = db.AccountGroups.ToList();
                cmbGroupName.ItemsSource = group;
                cmbGroupName.DisplayMemberPath = "GroupName";
                cmbGroupName.SelectedValuePath = "AccountGroupId";


                var c1 = db.AccountGroups.ToList();
                cmbGroupNameSrch.ItemsSource = c;
                cmbGroupNameSrch.SelectedValuePath = "GroupName";
                cmbGroupNameSrch.DisplayMemberPath = "GroupName";

                cmbGroupNameRepSrch.ItemsSource = c;
                cmbGroupNameRepSrch.SelectedValuePath = "GroupName";
                cmbGroupNameRepSrch.DisplayMemberPath = "GroupName";

                if (cmbGroupNameSrch.Text != "")
                {
                    dgvAccountGroup.ItemsSource = db.AccountGroups.Where(x => x.GroupName == cmbGroupNameSrch.Text).ToList();
                }
                else
                {
                    dgvAccountGroup.ItemsSource = db.AccountGroups.ToList();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void LoadReport()
        {
            try
            {
                Reportviewer.Reset();
                DataTable dt = getData();
                ReportDataSource Data = new ReportDataSource("AccountGroup", dt);

                Reportviewer.LocalReport.DataSources.Add(Data);
                Reportviewer.LocalReport.ReportEmbeddedResource = "AccountsBuddy.Reports.Master.rptAccountGroupReport.rdlc";

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
                    if (cmbGroupNameRepSrch.Text != "")
                    {
                        string qry = string.Format("SELECT SG.AccountGroupId, SG.AccountGroupCode, SG.GroupName, SG.GROUPCODE, ST.GROUPNAME UNDER FROM AccountGroups SG(nolock) JOIN AccountGroups ST(nolock) ON SG.UNDER = ST.AccountGroupId WHERE SG.AccountGroupId<> SG.UNDER and SG.GroupName='{0}'", cmbGroupNameRepSrch.Text);
                        cmd = new SqlCommand(qry, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dt);
                    }
                    else
                    {
                        string qry = string.Format("SELECT AccountGroupId,AccountGroupCode,GROUPNAME,GROUPCODE,GROUPNAME UNDER FROM AccountGroups(nolock) WHERE AccountGroupId=UNDER UNION SELECT SG.AccountGroupId, SG.AccountGroupCode, SG.GROUPNAME, SG.GROUPCODE, ST.GROUPNAME UNDER FROM AccountGroups SG(nolock) JOIN AccountGroups ST(nolock) ON SG.UNDER = ST.AccountGroupId WHERE SG.AccountGroupId <> SG.UNDER  order by GroupName");
                        cmd = new SqlCommand(qry, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dt);
                    }


                }
                return dt;

            }
            
            }
           

    }

#endregion

