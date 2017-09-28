using JJSuperMarket.Domain;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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

namespace JJSuperMarket.Master
{
    /// <summary>
    /// Interaction logic for frmCompanyDetails.xaml
    /// </summary>
    public partial class frmCompanyDetails : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarket.JJSuperMarketEntities();
        Boolean result = false ;
        string pass;
        public frmCompanyDetails()
        {
            InitializeComponent();
            LoadWindow();
        }
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion



        public void LoadWindow()
        {
            
            var v = db.CompanyDetails.ToList().FirstOrDefault() ;
            txtCompanyName.Text = v.CompanyName;
            txtAddress.Text = v.Address1;
            txtAddress2.Text = v.Address2 ;
            txtAddress3.Text = v.Address3;
            txtAddress4.Text = v.Address4;
            txtPinCode.Text = v.Pincode;
            txtUserName.Text =v.UserName ;
            txtPassWord1 .Text    =v.PassWord ;
            
            txtMobileNo.Text = v.MobileNo;
            txtTINNo.Text = v.Tin;
            txtPrinterName.Text = v.PhNo;
            txtGST.Text = v.Cst;
           // txtMinAmount.Text = v.MinAmount.ToString ();
            //txtMinPersentage.Text = v.Amount.ToString(); 
            result = false;           
        }

        
        private async  void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty (txtCompanyName.Text.Trim()))
            {
                var Information = new SampleMessageDialog
                {
                    Message = { Text = "Enter Company Name..." }
                };
                txtCompanyName .Focus();
                await DialogHost.Show(Information, "RootDialog");
                result = false;
            }
            else if(string.IsNullOrEmpty (txtAddress.Text.Trim()))
            {
                var Information = new SampleMessageDialog
                {
                    Message = { Text = "Enter Company Address..." }
                };
                txtAddress .Focus();
                await DialogHost.Show(Information, "RootDialog");
                result = false;
            }
            else if (string.IsNullOrEmpty(txtMobileNo .Text.Trim()))
            {
                var Information = new SampleMessageDialog
                {
                    Message = { Text = "Enter Mobile Number..." }
                };
                txtMobileNo .Focus();
                await DialogHost.Show(Information, "RootDialog");
                result = false;
            }
            else if (string.IsNullOrEmpty(txtUserName .Text.Trim()))
            {
                var Information = new SampleMessageDialog
                {
                    Message = { Text = "Enter User Name..." }
                };
                txtUserName .Focus();
                await DialogHost.Show(Information, "RootDialog");
                result = false;
            }
            else if (string.IsNullOrEmpty(txtPassWord1 .Text  .Trim()))
            {
                var Information = new SampleMessageDialog
                {
                    Message = { Text = "Enter Password..." }
                };
                txtPassWord1 .Focus();
                await DialogHost.Show(Information, "RootDialog");
                result = false;
            }
            else { result = true; }
           

            //txtPassWord.Password = txtPassWord1.Text;
            if (result == true)
            {
                var cmp = db.CompanyDetails.FirstOrDefault();
                cmp.CompanyName = txtCompanyName.Text;
                cmp.Address1 = txtAddress.Text;
                cmp.Address2  = txtAddress2.Text;
                cmp.Address3 = txtAddress3.Text;
                cmp.Address4 = txtAddress4.Text;
                cmp.Pincode = txtPinCode.Text;
                cmp.MobileNo = txtMobileNo.Text;
                cmp.PhNo = txtPrinterName.Text;
                cmp.Tin = txtTINNo.Text;
                cmp.Cst = txtGST.Text;
                cmp.UserName = txtUserName.Text;
                cmp.PassWord = txtPassWord1.Text   ;
               // cmp.MinAmount =Convert.ToDouble ( txtMinAmount.Text);
               // cmp.Amount = Convert.ToDouble(txtMinPersentage.Text);  
                db.SaveChanges();
              
                var Information = new SampleMessageDialog
                {
                    Message = { Text = " Save SucessFully " }
                };
                
                await DialogHost.Show(Information, "RootDialog");
                LoadWindow();
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWindow(); 
        }
    }
}
