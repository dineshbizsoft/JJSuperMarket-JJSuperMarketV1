using AccountsBuddy.Domain;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Drawing;

namespace AccountsBuddy.Transaction
{
    /// <summary>
    /// Interaction logic for frmSalesReturn.xaml
    /// </summary>
    public partial class frmSalesReturn : UserControl 
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        string UOM;
        ObservableCollection<ItemsDetails> lstSalesReturnDetail = new ObservableCollection<ItemsDetails>();
        decimal ID = 0;
        string TextToPrint = "";

        public frmSalesReturn()
        {
            InitializeComponent();
            dtpP.SelectedDate = DateTime.Today;
            txtExtras.Text = "0";
            LoadWindow();
        }
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

        #region Button Events

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbCustomer.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Customer Name.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    cmbCustomer.Focus();
                }
                else if (lstSalesReturnDetail.Count == 0)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Products.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    cmbItem.Focus();
                }
                else
                {

                    SalesReturn  p = new SalesReturn();

                    if (ID == 0)
                    {
                        db.SalesReturns.Add(p);

                    }
                    else
                    {
                        p = db.SalesReturns.Where(x => x.SRId  == ID).FirstOrDefault();
                    }
                    p.DiscountAmount = Convert.ToDouble(txtDiscount.Text.ToString());
                    p.Extra = Convert.ToDouble(txtExtras.Text.ToString());
                    p.LedgerCode = Convert.ToDecimal(cmbCustomer.SelectedValue.ToString());
                    p.SRDate = dtpP.SelectedDate;
                    p.ItemAmount  = Convert.ToDouble(txtTotal.Text.ToString());
                    p.SRType = cmbSalesRType.Text;

                    var sods = p.SalesReturnDetails.ToList();

                    foreach (var data in sods)
                    {
                        var sod = lstSalesReturnDetail.Where(x => x.SRDId == data.SRDId).FirstOrDefault();
                        if (sod == null)
                        {
                            p.SalesReturnDetails.Remove(data);
                        }
                    }


                    db.SaveChanges();

                    foreach (ItemsDetails data in lstSalesReturnDetail)
                    {
                        SalesReturnDetail tbd = new SalesReturnDetail();
                        if (data.SRDId == 0)
                        {
                            db.SalesReturnDetails.Add(tbd);
                        }

                        else
                        {
                            tbd = db.SalesReturnDetails.Where(x => x.SRDId == data.SRDId).FirstOrDefault();
                        }
                        //tbd.PODId = ID;
                        tbd.SRId  = p.SRId ;
                        tbd.ProductCode = data.SRCode ;
                        tbd.DisPer = data.DisPer;
                        tbd.Quantity = data.Quantity;
                        tbd.Rate = data.Rate;
                        tbd.TaxPer = data.TaxPer.ToString();
                        tbd.UOM = data.UOM;

                        db.SaveChanges();
                    }

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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FormClear();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Reports.Transaction.frmSalesReturnSearch frm = new Reports.Transaction.frmSalesReturnSearch();
            frm.ShowDialog();
            ViewDetails(frm.SRID );
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.SalesReturnDetails.RemoveRange(db.SalesReturnDetails.Where(x => x.SRId  == ID));
                db.SaveChanges();

                SalesReturn  p1 = db.SalesReturns .Where(x => x.SRId  == ID).FirstOrDefault();
                db.SalesReturns .Remove(p1);
                db.SaveChanges();

                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = "Deleted Successfully.." }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");
                LoadWindow();
                FormClear();
            }
            catch (Exception ex)
            {

            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            Transaction.frmSalesReturnReport frm = new frmSalesReturnReport();
            frm.ShowDialog();
        }

        

        #endregion

        #region GridEvents 

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            Product p = cmbItem.SelectedItem as Product;
            if (p == null)
            {
                MessageBox.Show("Please Select Product..");
            }
            else
            {
                ItemsDetails tbd = lstSalesReturnDetail .Where(x => x.ProductName  == p.ProductName).FirstOrDefault();
                if (tbd == null)
                {
                    tbd = new ItemsDetails();
                    lstSalesReturnDetail.Add(tbd);
                }

                Double Amt = 0;
                Double Total = 0; Double Dis = 0;

                tbd.SRCode  = p.ProductId;
              //  tbd.SRDId = Convert.ToInt16(ID);

                tbd.Rate = Convert.ToDouble(txtRate.Text.ToString());
                tbd.Quantity = Convert.ToDouble(txtQty.Text.ToString());
                tbd.UOM = p.UnitsOfMeasurement.UOMId;
                Dis = (txtDisAmt.Text == "" ? 0 : Convert.ToDouble(txtDisAmt.Text.ToString()));
                tbd.DisPer = Dis;
                tbd.ProductName = p.ProductName;
                tbd.UOMSymbol = p.UnitsOfMeasurement.UOMSymbol;
                Amt = (Convert.ToDouble(txtRate.Text.ToString()) * Convert.ToDouble(txtQty.Text.ToString()));
                tbd.Amount = Amt;
                Total = Convert.ToDouble(Amt - Dis);
                tbd.Total = Total;
                tbd.Itemcode = p.ItemCode;

                dgvDetails.ItemsSource = lstSalesReturnDetail;
                FindTotalAmount();
                AddNewItem();
            }
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                double amt = 0, Total = 0, DisPer = 0;
                ItemsDetails p = (ItemsDetails)dgvDetails.SelectedItem;
                txtItemCode.Text = p.Itemcode;
                cmbItem.Text = p.ProductName;
                txtRate.Text = p.Rate.ToString();
                txtQty.Text = p.Quantity.ToString();
                //amt = (Convert.ToDouble(txtRate.Text.ToString()) * Convert.ToDouble(txtQty.Text.ToString()));
                txtAmount.Text = p.Amount.ToString();

                txtDisAmt.Text = p.DisPer.ToString();
                //UOM = p.UnitsOfMeasurement.UOMSymbol;
                UOM = p.UOMSymbol;

            }
            catch (Exception ex)
            { }

        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ItemsDetails tbd = btn.Tag as ItemsDetails;
            lstSalesReturnDetail.Remove(tbd);
            FindTotalAmount();
        }
        private void AddNewItem()
        {
            txtItemCode.Clear();
            cmbItem.Text = "";
            txtRate.Clear();
            txtQty.Clear();
            txtAmount.Clear();
            txtDisAmt.Clear();
        }
        #endregion

        #region userdefined

        private void FindTotalAmount()
        {
            double total = 0;
            txtTotItemAmount.Text = string.Format("{0:0.00}", lstSalesReturnDetail.Sum(x => x.Amount));
            txtDiscount.Text = string.Format("{0:0.00}", lstSalesReturnDetail.Sum(x => x.DisPer));
            total = ((Convert.ToDouble(txtTotItemAmount.Text.ToString()) - Convert.ToDouble(txtDiscount.Text.ToString())) + Convert.ToDouble(txtExtras.Text.ToString()));
            txtTotal.Text = total.ToString();
            int r = Convert.ToInt16(Math.Round(total));
            txtRound.Text = r.ToString();
            int test;
            test = Convert.ToInt32(total);
            lblAmount.Content =   test;
            lblAmountInWords.Content = AppLib.NumberToWords(test).ToUpper();
            lblAmountInWords.Content += " Only.";
        }

        private void LoadWindow()
        {
            var v = db.Customers.ToList();
            cmbCustomer.ItemsSource = v;
            cmbCustomer.DisplayMemberPath = "CustomerName";
            cmbCustomer.SelectedValuePath = "CustomerId";


            var i = db.Products.ToList();
            cmbItem.ItemsSource = i;
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "ProductId";
            txtID.Text = (db.SalesReturns .DefaultIfEmpty().Max(p => p == null ? 0 : p.SRId) + 1).ToString();
            txtInNo.Text = (db.SalesReturns.DefaultIfEmpty().Max(p => p == null ? 0 : p.InvoiceNo) + 1).ToString();
            var SalesId = db.Sales.ToList();
            cmbSalesId.ItemsSource = SalesId;
            cmbSalesId.DisplayMemberPath = "InvoiceNo";
            cmbSalesId.SelectedValuePath = "SalesId";


            dgvDetails.ItemsSource = lstSalesReturnDetail;
        }

        private void txtItemCode_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var v = db.Products.Where(x => x.ItemCode == txtItemCode.Text).FirstOrDefault();
                cmbItem.Text = v.ProductName;

                txtRate.Text = v.PurchaseRate.ToString();
                UOM = v.UnitsOfMeasurement.UOMSymbol;
            }
            catch (Exception ex)
            {

            }
        }

        private void txtQty_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                double v = 0;
                v = (Convert.ToDouble(txtRate.Text.ToString())) * (Convert.ToDouble(txtQty.Text.ToString()));
                txtAmount.Text = v.ToString();
            }
            catch (Exception ex)
            {

            }
        }
        private void cmbItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var v = db.Products.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault();
                txtItemCode.Text = v.ItemCode;

                txtRate.Text = v.PurchaseRate.ToString();
                UOM = v.UnitsOfMeasurement.UOMSymbol;
            }
            catch (Exception ex)
            {

            }
        }
        private void cmbItem_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var v = db.Products.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault();
                txtItemCode.Text = v.ItemCode;

                txtRate.Text = v.PurchaseRate.ToString();
                UOM = v.UnitsOfMeasurement.UOMSymbol;
            }
            catch (Exception ex)
            {

            }
        }

        private void txtExtras_LostFocus(object sender, RoutedEventArgs e)
        {
            FindTotalAmount();
        }

        private void FormClear()
        {
            cmbCustomer.Text = "";
            txtItemCode.Clear();
            cmbSalesId.Text = "";
            cmbCustomer.Text = "";
            cmbItem.Text = "";
            txtRate.Text = "0";
            txtQty.Text = "0";
            txtAmount.Text = "0";
            txtDisAmt.Text = "0";
            lstSalesReturnDetail.Clear();
            dgvDetails.ItemsSource = lstSalesReturnDetail;
            txtTotItemAmount.Text = "0";
            txtDiscount.Text = "0";
            txtExtras.Text = "0";
            txtRound.Text = "0";
            txtTotal.Text = "0";
            ID = 0;
            lblAmount.Content = "";
            lblAmountInWords.Content = "";
            cmbSalesRType.Text = "";
            dtpP.SelectedDate = DateTime.Today;
            LoadWindow();
            
        }

        public void ViewDetails(decimal id)
        {
            try
            {
                SalesReturn  p = db.SalesReturns .Where(x => x.SRId  == id).FirstOrDefault();
                ID = p.SRId ;
                txtID.Text = p.SRId.ToString();
                txtInNo.Text = p.InvoiceNo.ToString();
                dtpP.SelectedDate = p.SRDate;

               
                txtDiscount.Text = p.DiscountAmount.ToString();
                txtExtras.Text = p.Extra.ToString();
                cmbCustomer.Text = p.Customer.CustomerName.ToString ();
                cmbSalesRType.Text = p.SRType ;

                lstSalesReturnDetail.Clear();
                foreach (var data in p.SalesReturnDetails .ToList())
                {
                    ItemsDetails i = new ItemsDetails();
                    i.DisPer = Convert.ToDouble(data.DisPer);
                    i.UOMSymbol = data.UnitsOfMeasurement.UOMSymbol;
                    i.Itemcode = data.Product.ItemCode;
                    i.ProductName = data.Product.ProductName;
                    i.Rate = Convert.ToDouble(data.Rate);
                    i.Quantity = Convert.ToDouble(data.Quantity);
                    Double d = (i.Rate * i.Quantity);
                    i.Amount = d;
                    double t = d - i.DisPer;
                    i.Total = t;
                    i.SRCode  = Convert.ToDecimal(data.ProductCode);
                    i.UOM = Convert.ToDecimal(data.UOM);
                    i.SRDId = Convert.ToInt16(data.SRDId );

                    lstSalesReturnDetail.Add(i);
                }
                FindTotalAmount();

            }
            catch (Exception ex)
            {

            }
        }

        public void orderDetails(decimal d)
        {
            try
            {
                Sale p = db.Sales.Where(x => x.SalesId == d).FirstOrDefault();
                cmbCustomer.Text = p.Customer.CustomerName;
               
                foreach (var data in p.SalesDetails.ToList())
                {
                    ItemsDetails i = new ItemsDetails();
                    i.DisPer = Convert.ToDouble(data.DisPer);
                    i.UOMSymbol = data.UnitsOfMeasurement.UOMSymbol;
                    i.Itemcode = data.Product.ItemCode;
                    i.ProductName = data.Product.ProductName;
                    i.Rate = Convert.ToDouble(data.Rate);
                    i.Quantity = Convert.ToDouble(data.Quantity);
                    Double amt = (i.Rate * i.Quantity);
                    i.Amount = amt;
                    double t = amt - i.DisPer;
                    i.Total = t;
                    i.SRCode = Convert.ToDecimal(data.ProductCode);
                    i.UOM = Convert.ToDecimal(data.UOM);

                    lstSalesReturnDetail.Add(i);
                }
                FindTotalAmount();
            }
            catch(Exception ex)
            {

            }

        }

        private void cmbPOID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = Convert.ToDecimal(cmbSalesId.SelectedValue);
            orderDetails(d);
        }

        #endregion

        #region class
        partial class ItemsDetails : INotifyPropertyChanged
        {
            private int _SRId;

            private decimal _SRCode;
            private double _Rate;
            private double _Quantity;
            private decimal _UOM;
            private double _TaxPer;
            private double _DisPer;
            private string _ProductName;
            private string _UOMSymbol;
            private double _Amount;
            private double _Total;
            private string _Itemcode;


            public int SRDId { get { return _SRId; } set { if (_SRId != value) { _SRId = value; NotifyPropertyChanged("_SRId"); } } }


            public decimal SRCode
            {
                get
                {
                    return _SRCode;
                }
                set
                {
                    if (_SRCode != value)
                    {
                        _SRCode = value;
                        NotifyPropertyChanged("SRCode");
                    }
                }
            }

            public double Rate
            {
                get
                {
                    return _Rate;
                }
                set
                {
                    if (_Rate != value)
                    {
                        _Rate = value;
                        NotifyPropertyChanged("Rate");
                    }
                }
            }
            public double Quantity
            {
                get
                {
                    return _Quantity;
                }
                set
                {
                    if (_Quantity != value)
                    {
                        _Quantity = value;
                        NotifyPropertyChanged("Quantity");
                    }
                }
            }
            public decimal UOM
            {
                get
                {
                    return _UOM;
                }
                set
                {
                    if (_UOM != value)
                    {
                        _UOM = value;
                        NotifyPropertyChanged("UOM");
                    }
                }
            }
            public double TaxPer
            {
                get
                {
                    return _TaxPer;
                }
                set
                {
                    if (_TaxPer != value)
                    {
                        _TaxPer = value;
                        NotifyPropertyChanged("TaxPer");
                    }
                }
            }
            public double DisPer
            {
                get
                {
                    return _DisPer;
                }
                set
                {
                    if (_DisPer != value)
                    {
                        _DisPer = value;
                        NotifyPropertyChanged("DisPer");
                    }
                }
            }
            public string ProductName
            {
                get
                {
                    return _ProductName;
                }
                set
                {
                    if (_ProductName != value)
                    {
                        _ProductName = value;
                        NotifyPropertyChanged("ProductName");
                    }
                }
            }
            public string UOMSymbol
            {
                get
                {
                    return _UOMSymbol;
                }
                set
                {
                    if (_UOMSymbol != value)
                    {
                        _UOMSymbol = value;
                        NotifyPropertyChanged("UOMSymbol");
                    }
                }
            }
            public double Amount
            {
                get
                {
                    return _Amount;
                }
                set
                {
                    if (_Amount != value)
                    {
                        _Amount = value;
                        NotifyPropertyChanged("Amount");
                    }
                }
            }
            public double Total
            {
                get
                {
                    return _Total;
                }
                set
                {
                    if (_Total != value)
                    {
                        _Total = value;
                        NotifyPropertyChanged("Total");
                    }
                }
            }
            public string Itemcode
            {
                get
                {
                    return _Itemcode;
                }
                set
                {
                    if (_Itemcode != value)
                    {
                        _Itemcode = value;
                        NotifyPropertyChanged("Itemcode");
                    }
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string propName)
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }



        }

        #endregion

        
    }
}
