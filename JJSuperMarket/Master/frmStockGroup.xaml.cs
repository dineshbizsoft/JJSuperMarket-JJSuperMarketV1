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
using JJSuperMarket.Domain;
using MaterialDesignThemes.Wpf;
using Microsoft.Reporting.WinForms;



namespace JJSuperMarket.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmStockGroup.xaml
    /// </summary>
    public partial class frmStockGroup : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        decimal ID = 0;
        public frmStockGroup()
        {
            InitializeComponent();
            LoadWindow();
            LoadReport();
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
                        Message = { Text = "Enter Under Group.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    cmbGroupName.Focus();
                }
               
                if (ID != 0)
                {
                    bool r = true;
                    //string name = db.StockGroups.Where(x => x.StockGroupId == ID  ).Select(x => x.GroupName).FirstOrDefault().ToString();
                    //// string name1 = db.StockGroups.Where(x => x.StockGroupId == ID).Select(x => x.GroupCode).FirstOrDefault().ToString();
                   
                        if (await validation() == false)
                        {
                            r = false;
                        }
                        else if (r == true)
                        {
                            StockGroup c = db.StockGroups.Where(x => x.StockGroupId == ID).FirstOrDefault();
                            c.StockGroupCode = "0";
                            c.GroupName = txtGroupName.Text;
                            c.Under = (cmbGroupName.Text == null ? 0 : Convert.ToDecimal(cmbGroupName.SelectedValue));

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
                        StockGroup c = new StockGroup();
                        c.StockGroupCode ="0";
                        c.GroupName = txtGroupName.Text;
                        c.Under = (cmbGroupName.Text == null ? 0 : Convert.ToDecimal(cmbGroupName.SelectedValue));



                        db.StockGroups.Add(c);
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
                        StockGroup c = db.StockGroups.Where(x => x.StockGroupId == ID).FirstOrDefault();
                        db.StockGroups.Remove(c);
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
            catch (Exception ex)
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

        private void dgvStock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                StockGroup c = dgvStock.SelectedItem as StockGroup;
                ID = c.StockGroupId;
                
                txtGroupName.Text = c.GroupName;
                cmbGroupName.Text = c.StockGroup1.GroupName;
            }
            catch(Exception ex)
            {

            }
        }
        #endregion

        #region User Define
        private async Task<bool> validation()
        {

            var b = db.StockGroups.Where(x => x.StockGroupId != ID && x.GroupName.ToLower()==txtGroupName.Text.ToLower());
            //var b1 = db.StockGroups.Where(x => x.GroupName == txtGroupCode.Text).Count();

            if (b.Count() != 0)
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = "" + txtGroupName.Text + ", Already Exist.Enter New One " }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");
                txtGroupName.Focus();
                return false;
            }
            //else if (b1 != 0)
            //{
            //    var sampleMessageDialog = new SampleMessageDialog
            //    {
            //        Message = { Text = "" + txtGroupCode.Text + ",Already Exist!.Enter new code.." }
            //    };

            //    await DialogHost.Show(sampleMessageDialog, "RootDialog");
            //    txtGroupCode.Focus();
            //    return false;

            //}
            else
            {
                return true;
            }

        }
        private void FormClear()
        {
            
            txtGroupName.Clear();
            cmbGroupName.Text = "";
            ID = 0;
        }
        private void LoadWindow()
        {
            try
            {
                var c = db.StockGroups.OrderBy(x => x.GroupName).ToList();
                cmbGroupName.ItemsSource = c;
                cmbGroupName.SelectedValuePath = "StockGroupId";
                cmbGroupName.DisplayMemberPath = "GroupName";

                var c1 = db.StockGroups.OrderBy(x=>x.GroupName).ToList();
                cmbGroupNameSrch.ItemsSource = c;
                cmbGroupNameSrch.SelectedValuePath = "GroupName";
                cmbGroupNameSrch.DisplayMemberPath = "GroupName";

                cmbGroupNameRepSrch.ItemsSource = c;
                cmbGroupNameRepSrch.SelectedValuePath = "GroupName";
                cmbGroupNameRepSrch.DisplayMemberPath = "GroupName";

                if (cmbGroupNameSrch.Text != "")
                {
                    dgvStock.ItemsSource = db.StockGroups.Where(x => x.GroupName == cmbGroupNameSrch.Text).OrderBy(x=>x.GroupName).ToList();
                }
                else
                {
                    dgvStock.ItemsSource = db.StockGroups.OrderBy(x => x.GroupName).ToList();
                }
            }
            catch (Exception ex)
            { }
        }
        private void LoadReport()
        {
            try
            {
                Reportviewer.Reset();
                DataTable dt = getData();
                ReportDataSource Data = new ReportDataSource("StockGroup", dt);

                Reportviewer.LocalReport.DataSources.Add(Data);
                Reportviewer.LocalReport.ReportEmbeddedResource = "JJSuperMarket.Reports.Master.rptStockReport.rdlc";



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
                    string qry = string.Format("SELECT SG.STOCKGROUPID, SG.STOCKGROUPCODE, SG.GROUPNAME, SG.GROUPCODE, ST.GROUPNAME UNDER FROM STOCKGROUPS SG(nolock) JOIN STOCKGROUPS ST(nolock) ON SG.UNDER = ST.STOCKGROUPID WHERE SG.STOCKGROUPID <> SG.UNDER or ST.GROUPNAME='{0}'", cmbGroupNameRepSrch.Text);
                    cmd = new SqlCommand(qry, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
                else
                {
                    string qry = "SELECT STOCKGROUPID,STOCKGROUPCODE,GROUPNAME,GROUPCODE,GROUPNAME UNDER FROM STOCKGROUPS(nolock) WHERE STOCKGROUPID=UNDER UNION SELECT SG.STOCKGROUPID,SG.STOCKGROUPCODE,SG.GROUPNAME,SG.GROUPCODE,ST.GROUPNAME UNDER FROM STOCKGROUPS SG(nolock) JOIN STOCKGROUPS ST(nolock) ON SG.UNDER = ST.STOCKGROUPID WHERE SG.STOCKGROUPID<> SG.UNDER";
                    cmd = new SqlCommand(qry, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }


            }
            return dt;

        }

        #endregion

    }
}