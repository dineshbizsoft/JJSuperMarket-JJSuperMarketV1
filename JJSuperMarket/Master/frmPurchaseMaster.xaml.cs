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
using JJSuperMarket.Domain;
using MaterialDesignThemes.Wpf;

namespace JJSuperMarket.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmPurchaseMaster.xaml
    /// </summary>
    public partial class frmPurchaseMaster : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        decimal ID = 0;
        decimal SID = 0;
        public frmPurchaseMaster()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dtpDate.SelectedDate = DateTime.Today;
            LoadWindow();
        }
        private void LoadWindow()
        {
            var v = db.Suppliers.Where(x => !string.IsNullOrEmpty(x.SupplierName)).OrderBy(x => x.LedgerName).ToList();
            cmbSupplier.ItemsSource = v;
            cmbSupplier.DisplayMemberPath = "LedgerName";
            cmbSupplier.SelectedValuePath = "SupplierId";
           
            var inv = (db.PurchaseMasters.DefaultIfEmpty().Max(p => p == null ? 0 : p.InvoiceNo)) + 1;
            txtInNo.Text = inv.ToString();

            txtID.Text = (db.PurchaseMasters.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id) + 1).ToString();
           
        }

        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #region button Events
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PurchaseMaster pm = new PurchaseMaster() ;
                if (cmbSupplier.Text == "")
                {
                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Supplier Name..." }
                    };
                    cmbSupplier.Focus();
                    await DialogHost.Show(Information, "RootDialog");
                }
                else if (txtItemAmount.Text == "")
                {
                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Bill Amount..." }
                    };
                    txtTotalAmount.Focus();
                    await DialogHost.Show(Information, "RootDialog");
                }                              
                else
                {
                    if (ID == 0)
                    {
                        db.PurchaseMasters.Add(pm);

                    }
                    else
                    {
                        pm = db.PurchaseMasters.Where(x => x.Id == ID).FirstOrDefault();
                    }
                    var LCode = cmbSupplier.SelectedItem as Supplier;
                    pm.PurchaseDate = dtpDate.SelectedDate;
                    pm.LedgerCode = db.Suppliers.Where(x => x.SupplierId == LCode.SupplierId).Select(x => x.SupplierId).FirstOrDefault();
                    pm.NoOfProducts = Convert.ToDecimal(txtNoOfProducts.Text);
                    pm.Narration = txtNarration.Text;
                    pm.PurchaseType = cmbPurchaseType.Text;

                    pm.DiscountAmount = txtDiscountAmount.Text == "" ? 0 : Convert.ToDecimal(txtDiscountAmount.Text.ToString());
                    pm.Extra = txtExtraAmount.Text == "" ? 0 : Convert.ToDecimal(txtExtraAmount.Text.ToString());
                    pm.InvoiceNo = Convert.ToDecimal(txtInNo.Text.ToString());
                    pm.ItemAmount = Convert.ToDouble(txtTotalAmount.Text.ToString());
                    pm.PurchaseType = cmbPurchaseType.Text;
                    pm.InvoiceNo = Convert.ToDecimal(txtInNo.Text.ToString());
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
            catch (Exception ex)
            { }
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
                        var pur = db.PurchaseMasters.Where(x => x.Id == ID).FirstOrDefault();
                        var rec = db.PaymentMasters.Where(x => x.PurchaseId == pur.InvoiceNo).Count();
                        if (rec > 0)
                        {
                            var sampleMessageDialog = new SampleMessageDialog
                            {
                                Message = { Text = "Can't Delete.Contact Admin.." }
                            };

                            await DialogHost.Show(sampleMessageDialog, "RootDialog");
                        }
                        else
                        {
                          
                            PurchaseMaster p1 = db.PurchaseMasters.Where(x => x.Id == ID).FirstOrDefault();
                            db.PurchaseMasters.Remove(p1);
                            db.SaveChanges();

                            var sampleMessageDialog = new SampleMessageDialog
                            {
                                Message = { Text = "Deleted Successfully.." }
                            };

                            await DialogHost.Show(sampleMessageDialog, "RootDialog");
                            FormClear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Can't Delete Contact Admin..");
                db = new JJSuperMarketEntities();
                FormClear();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FormClear();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Reports.Transaction.frmPurchaseMasterSearch frm = new Reports.Transaction.frmPurchaseMasterSearch();
            frm.ShowDialog();
            ViewDetails(frm.PID);
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            Reports.Transaction.frmPurchaseMasterReport frm = new Reports.Transaction.frmPurchaseMasterReport();
            frm.ShowDialog();
        }

        #endregion

        #region user defined
        private void FormClear()
        {
            cmbSupplier.Text = "";
            dtpDate.SelectedDate = DateTime.Now;
            cmbPurchaseType.Text = "";
            txtNoOfProducts.Text = "";
            txtNarration.Text = "";

            txtItemAmount.Text = "";
            txtDiscountAmount.Text = "";
            txtExtraAmount.Text = "";
            txtTotalAmount.Text = "";

            txtRoundOff.Text = "";
            lblAmount.Text = "";
            lblAmountInWords.Text = "";
            ID = 0;
            LoadWindow();
           
        }
        public void ViewDetails(decimal id)
        {
            try
            {
                PurchaseMaster p = db.PurchaseMasters.Where(x => x.Id == id).FirstOrDefault();
                ID = p.Id;
                txtID.Text = p.Id.ToString();
                txtInNo.Text = p.InvoiceNo.ToString();
                dtpDate.SelectedDate = p.PurchaseDate;
                cmbSupplier.Text = p.Supplier.SupplierName;
                txtNoOfProducts.Text = p.NoOfProducts.ToString() ;
                txtNarration.Text = p.Narration;
                cmbPurchaseType.Text = p.PurchaseType;
                txtDiscountAmount.Text = p.DiscountAmount.ToString();
                txtExtraAmount.Text = p.Extra.ToString();
                txtTotalAmount.Text = p.ItemAmount.ToString();
                txtItemAmount.Text =( (Convert.ToDouble(p.ItemAmount.Value) + (double)p.Extra.Value) - (double)p.DiscountAmount).ToString();
              

            }
            catch (Exception ex)
            {

            }
        }
        private void FindTotalAmount()
        {
            try
            {
                decimal Total;
                decimal RoundOff;
                Total = (Convert.ToDecimal(txtItemAmount.Text))+ (txtExtraAmount.Text == "" ? 0 : Convert.ToDecimal(txtExtraAmount.Text));
                Total -= (txtDiscountAmount.Text)==""?0:Convert.ToDecimal(txtDiscountAmount.Text); 
                txtTotalAmount.Text = string.Format("{0:N2}",Total.ToString());

                RoundOff =  Math.Round(Total, MidpointRounding.AwayFromZero);
                txtRoundOff.Text = string.Format("{0:N2}", RoundOff.ToString());
                lblAmount.Text = "RS " + string.Format("{0:N2}", Total.ToString());
                lblAmountInWords.Text = AppLib.NumberToWords(Convert.ToInt32(RoundOff)).ToUpper();
                lblAmountInWords.Text += " ONLY.";
                
            }
            catch (Exception ex) { }
        }
        private void LoadReport()
        {

        }

        #endregion

        private void txtItemAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            FindTotalAmount();
        }

        private void txtExtraAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            FindTotalAmount();
        }

        private void txtDiscountAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            FindTotalAmount();
        }
    }
}

