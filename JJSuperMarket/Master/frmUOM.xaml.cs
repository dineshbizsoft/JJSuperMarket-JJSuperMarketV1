using JJSuperMarket.Domain;
using MaterialDesignThemes.Wpf;
using Microsoft.Reporting.WinForms;
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

namespace JJSuperMarket.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmUOM.xaml
    /// </summary>
    public partial class frmUOM : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        decimal ID = 0;
        public frmUOM()
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
                if (txtSymbol.Text == "")
                {

                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Symbol Name.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtSymbol.Focus();
                }



                else if (ID != 0)
                {
                    bool r = true;
                    string name = db.UnitsOfMeasurements.Where(x => x.UOMId == ID).Select(x => x.UOMSymbol).FirstOrDefault().ToString();
                    if (txtSymbol.Text != name)

                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }

                    }

                    else if (r == true)
                    {
                        UnitsOfMeasurement c = db.UnitsOfMeasurements.Where(x => x.UOMId == ID).FirstOrDefault();
                        c.UOMSymbol = txtSymbol.Text;
                        c.formalname = txtFormalName.Text;



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
                        UnitsOfMeasurement c = new UnitsOfMeasurement();
                        c.UOMSymbol = txtSymbol.Text;
                        c.formalname = txtFormalName.Text;

                        db.UnitsOfMeasurements.Add(c);
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
                        Message = { Text = "Do you want to delete this record?.." }
                    };

                    var result = Convert.ToBoolean(await DialogHost.Show(sampleDialog, "RootDialog"));
                    if (result == true)
                    {
                        UnitsOfMeasurement c = db.UnitsOfMeasurements.Where(x => x.UOMId == ID).FirstOrDefault();
                        db.UnitsOfMeasurements.Remove(c);
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
        private void dgvUOM_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                UnitsOfMeasurement c = dgvUOM.SelectedItem as UnitsOfMeasurement;
                ID = c.UOMId;
                txtFormalName.Text = c.formalname;
                txtSymbol.Text = c.UOMSymbol;
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
                var c = db.UnitsOfMeasurements.ToList();
                cmbUOMDetail.ItemsSource = c;
                cmbUOMDetail.SelectedValuePath = "UOMSymbol";
                cmbUOMDetail.DisplayMemberPath = "UOMSymbol";

                cmbUOMReport.ItemsSource = c;
                cmbUOMReport.SelectedValuePath = "UOMSymbol";
                cmbUOMReport.DisplayMemberPath = "UOMSymbol";

                if (cmbUOMDetail.Text != "")
                {
                    dgvUOM.ItemsSource = db.UnitsOfMeasurements.Where(x => x.UOMSymbol == cmbUOMDetail.Text).ToList();
                }
                else
                {
                    dgvUOM.ItemsSource = db.UnitsOfMeasurements.ToList();
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
                ReportDataSource Data = new ReportDataSource("UOM", dt);

                Reportviewer.LocalReport.DataSources.Add(Data);
                Reportviewer.LocalReport.ReportEmbeddedResource = "JJSuperMarket.Reports.Master.rptUomReport.rdlc";

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
                if (cmbUOMReport.Text != "")
                {
                    string qry = string.Format("Select * from UnitsOfMeasurement  where UOMSymbol='{0}'", cmbUOMReport.Text);
                    cmd = new SqlCommand(qry, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
                else
                {
                    string qry = string.Format("Select * from UnitsOfMeasurement  order by UOMSymbol");
                    cmd = new SqlCommand(qry, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }


            }
            return dt;
        }
        private async Task<bool> validation()
        {

            var b = db.UnitsOfMeasurements.Where(x => x.UOMSymbol == txtSymbol.Text).Count();

            if (b != 0)
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = "" + txtSymbol.Text + ", Already Exist.Enter New One.. " }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");
                txtSymbol.Focus();
                return false;
            }

            else
            {
                return true;
            }

        }
        private void FormClear()
        {

            txtSymbol.Text = "";
            txtFormalName.Text = "";
            ID = 0;
        }

        #endregion

        private void cmbUOMDetail_DropDownOpened(object sender, EventArgs e)
        {
            var c = db.UnitsOfMeasurements.ToList();
            cmbUOMDetail.ItemsSource = c;
            cmbUOMDetail.SelectedValuePath = "UOMSymbol";
            cmbUOMDetail.DisplayMemberPath = "UOMSymbol";

            cmbUOMReport.ItemsSource = c;
            cmbUOMReport.SelectedValuePath = "UOMSymbol";
            cmbUOMReport.DisplayMemberPath = "UOMSymbol";
        }
    }
}
