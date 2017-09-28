using System;
using System.Collections.Generic;
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
using JJSuperMarket.Domain;
using MaterialDesignThemes.Wpf;
using System.Text.RegularExpressions;
using System.Data;
using Microsoft.Reporting.WinForms;
using System.Data.SqlClient;
using System.Windows.Controls.Primitives;

namespace JJSuperMarket.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmCustomer.xaml
    /// </summary>
    public partial class frmCustomer : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        decimal ID = 0;

        public frmCustomer()
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
                if (txtPersonIncharge.Text == "")
                {

                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Person Incharge.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtPersonIncharge.Focus();
                }

                else if (txtMobile.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Customer Mobile Number.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtMobile.Focus();
                }

                else if (ID != 0)
                {
                    bool r = true;
                    string name = db.Customers.Where(x => x.CustomerId == ID).Select(x => x.LedgerName).FirstOrDefault().ToString();
                    string name1 = db.Customers.Where(x => x.CustomerId == ID).Select(x => x.CustomerName).FirstOrDefault().ToString();
                    if (txtPersonIncharge.Text != name1)

                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }

                    }

                    if (r == true)
                    {
                        Customer c = db.Customers.Where(x => x.CustomerId == ID).FirstOrDefault();
                        c.CustomerName = txtPersonIncharge.Text;
                        c.LedgerName = "JJSuper";
                        c.MobileNo = txtMobile.Text;
                        c.TelePhoneNo = txtTelephone.Text;// (txtDisAmt.Text == "" ? 0 : Convert.ToDouble(txtDisAmt.Text.ToString()));
                        c.AddressLine = txtAddress.Text;
                        c.EMailId = txtEmail.Text;

                        c.CreditDays = (txtCreditDays.Text == "" ? 0 : Convert.ToDecimal(txtCreditDays.Text)); //  Convert.ToDecimal(txtCreditDays.Text);
                        c.CreditLimits = (txtCreditDays.Text == "" ? 0 : Convert.ToDouble(txtCreditLimit.Text));
                        c.CST = txtGST.Text;
                        c.TinNo = txtTinNo.Text;

                        //Ledger l = db.Ledgers.Where(x => x.BillingName  == txtPersonIncharge .Text.ToString()).FirstOrDefault();
                        //l.LedgerName = txtPersonIncharge.Text;
                        //l.BillingName = txtPersonIncharge.Text;
                        //l.GroupCode = "Sundry Debtors";
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
                        Customer c = new Customer();
                        c.CustomerName = txtPersonIncharge.Text;
                        c.LedgerName = "JJSuper";
                        c.MobileNo = txtMobile.Text;
                        c.TelePhoneNo = txtTelephone.Text;
                        c.AddressLine = txtAddress.Text;
                        c.EMailId = txtEmail.Text;
                        c.CreditDays = (txtCreditDays.Text == "" ? 0 : Convert.ToDecimal(txtCreditDays.Text));
                        c.CreditLimits = (txtCreditLimit.Text == "" ? 0 : Convert.ToDouble(txtCreditLimit.Text));
                        c.CST = txtGST.Text;
                        c.TinNo = txtTinNo.Text;

                        db.Customers.Add(c);
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
                LogInfo er = new JJSuperMarket.LogInfo();
                er.Error = ex.ToString();
                er.LogDate = DateTime.Now;
                db.LogInfoes.Add(er);
                db.SaveChanges();

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
                        Ledger l = db.Ledgers.Where(x => x.BillingName == txtPersonIncharge.Text.ToString()).FirstOrDefault();
                        if (l != null)
                        {
                            db.Ledgers.Remove(l);
                            db.SaveChanges();
                        }


                        Customer c = db.Customers.Where(x => x.CustomerId == ID).FirstOrDefault();
                        db.Customers.Remove(c);
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
                MessageBox.Show(" Can't Delete Contact Admin..");
                db = new JJSuperMarketEntities();
                FormClear();
                LoadWindow();
                LoadReport();
            }


        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void btnSearch1_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void dgvCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CustomerDetail c = dgvCustomer.SelectedItem as CustomerDetail;
                ID = c.CustomerId;
                txtPersonIncharge.Text = c.CustomerName;
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
            {

            }
        }

        #endregion
     
        #region Methods
        private async Task<bool> validation()
        {

            var b = db.Customers.Where(x => x.LedgerName == txtCompanyName.Text).Count();
            var b1 = db.Customers.Where(x => x.CustomerName == txtPersonIncharge.Text).Count();

            if (b1 != 0)
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

        private void LoadWindow()
        {
            try
            {
                Customer_Search(db.Customers.OrderBy(x => x.CustomerName).ToList());
                LoadReport();
            }
            catch (Exception ex)
            {

            }
        }

        private void FormClear()
        {
            txtCompanyName.Clear();
            txtPersonIncharge.Clear();
            txtTelephone.Clear();
            txtMobile.Clear();
            txtCustomerSearch.Clear();
            txtGST.Clear();
            txtEmail.Clear();
            txtCreditLimit.Text = "0";
            txtCreditDays.Text = "0";
            txtAddress.Clear();
            txtTinNo.Clear();
            ID = 0;
        }

        private void LoadReport()
        {
            try
            {
                Reportviewer.Reset();
                DataTable dt = getData();
                ReportDataSource Data = new ReportDataSource("Customer", dt);

                Reportviewer.LocalReport.DataSources.Add(Data);
                Reportviewer.LocalReport.ReportEmbeddedResource = "JJSuperMarket.Reports.Master.rptCustomerReport.rdlc";

                Reportviewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private DataTable getData()
        {

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(AppLib.conStr))
            {
                SqlCommand cmd;
                string qry = string.Format("Select * from Customer  order by CustomerName");
                cmd = new SqlCommand(qry, con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);



            }
            return dt;

        }
        private void Customer_Search(List<Customer> p)
        {
            CustomerDetail pc = new CustomerDetail();
            List<CustomerDetail> p1 = new List<CustomerDetail>();
            var pr = p;
            int n = 0;
            foreach (var p2 in pr)
            {
                pc = new CustomerDetail();
                pc.CustomerId = p2.CustomerId;
                n = n + 1;
                pc.SNo = n;
                pc.CustomerName = p2.CustomerName;
                pc.AddressLine = p2.AddressLine;
                pc.CreditDays = p2.CreditDays;
                pc.CreditLimits = p2.CreditLimits;
                pc.CST = p2.CST;
                pc.CustomerCode = p2.CustomerCode;
                pc.EMailId = p2.EMailId;
                pc.LedgerName = p2.LedgerName;
                pc.MobileNo = p2.MobileNo;
                pc.TelePhoneNo = p2.TelePhoneNo;
                pc.TinNo = p2.TinNo;
                p1.Add(pc);

            }
            dgvCustomer.ItemsSource = p1;
        }

        //Accepts only numbers
        private void txtCreditDays_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        #endregion

        #region Enents
        private void txtCustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCustomerSearch.Text))
            {
                var p = db.Customers.Where(x => x.CustomerName.ToLower().Contains(txtCustomerSearch.Text.ToLower())).OrderBy(x => x.CustomerId).ToList();
                Customer_Search(p);
            }
            else
            {
                Customer_Search( db.Customers.ToList());
            }
        }

        private void txtCompanyName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
        #endregion

        #region Classs
        public class CustomerDetail
        {
            public int SNo { get; set; }
            public decimal CustomerId { get; set; }
            public string CustomerCode { get; set; }
            public string LedgerName { get; set; }
            public string CustomerName { get; set; }
            public string AddressLine { get; set; }
            public string TelePhoneNo { get; set; }
            public string MobileNo { get; set; }
            public string EMailId { get; set; }
            public string CST { get; set; }
            public string TinNo { get; set; }
            public Nullable<decimal> CreditDays { get; set; }
            public Nullable<double> CreditLimits { get; set; }

        }
        #endregion
    }
}
