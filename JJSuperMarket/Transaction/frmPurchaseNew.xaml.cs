using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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

namespace JJSuperMarket.Transaction
{
    /// <summary>
    /// Interaction logic for frmPurchaseNew.     
    /// </summary>
    public partial class frmPurchaseNew : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();

        List<Product> lstProduct = new List<Product>();

        ObservableCollection<ItemsDetails> lstPurchase = new ObservableCollection<ItemsDetails>();


        decimal ID = 0;
        decimal SID = 0;
        string TextToPrint = "";
        List c;
        public frmPurchaseNew()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dtpS.SelectedDate = DateTime.Today;
            LoadWindow();

        }
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void NumericOnly1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #region Button Events

        async void FormValidate()
        {
            bool RValue = true;
            if (lstPurchase.Count == 0)
            {
                var Info = new SampleMessageDialog
                {
                    Message = { Text = "Enter Products..." }
                };
                cmbItem.Focus();
                await DialogHost.Show(Info, "RootDialog");
            }

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Purchase p = new Purchase();
                double tot, paid;
                tot = Convert.ToDouble(txtRound.Text);
                paid = Convert.ToDouble(txtPaidAmount.Text == "" ? "0.00" : txtPaidAmount.Text);
                var d = cmbSupplier.SelectedItem as Supplier;
                if (d.SupplierId == 0)
                {
                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Supplier Name..." }
                    };
                    cmbSupplier.Focus();
                    await DialogHost.Show(Information, "RootDialog");
                }
                else if (cmbMobileNumber.Text == "")
                {
                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Supplier Mobile Number..." }
                    };
                    cmbMobileNumber.Focus();
                    await DialogHost.Show(Information, "RootDialog");
                }
                else if (txtPaidAmount.Text == "" || txtPaidAmount.Text == "0.00" && cmbPurchaseType.SelectedIndex != 1)
                {
                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = "Please Enter the Paid Amount..." }
                    };
                    txtPaidAmount.Focus();
                    await DialogHost.Show(Information, "RootDialog");
                }
                else if (lstPurchase.Count == 0)
                {
                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = "Please Enter the Product Details..." }
                    };
                    txtPaidAmount.Focus();
                    await DialogHost.Show(Information, "RootDialog");
                }
                else if (paid < tot && cmbPurchaseType.SelectedIndex != 1)
                {

                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = "Amount Mismatch " }
                    };
                    txtPaidAmount.Focus();
                    await DialogHost.Show(Information, "RootDialog");


                }
                else
                {
                    txtPaidAmount.Text = cmbPurchaseType.SelectedIndex == 1 ? "0.00" : txtPaidAmount.Text;
                    if (ID == 0)
                    {
                        db.Purchases.Add(p);

                    }
                    else
                    {
                        p = db.Purchases.Where(x => x.PurchaseId == ID).FirstOrDefault();
                    }
                    p.DiscountAmount = txtDiscount.Text == "" ? 0 : Convert.ToDouble(txtDiscount.Text.ToString());
                    p.Extra = txtExtras.Text == "" ? 0 : Convert.ToDouble(txtExtras.Text.ToString());
                    p.InvoiceNo = Convert.ToDecimal(txtInNo.Text.ToString());

                    // p.LedgerCode = Convert.ToDecimal(cmbCustomer.SelectedValue.ToString());
                    p.LedgerCode = db.Suppliers.Where(x => x.SupplierId==d.SupplierId).Select(x => x.SupplierId).FirstOrDefault();
                    p.Narration = txtPaidAmount.Text;
                    p.PurchaseDate = dtpS.SelectedDate;
                    p.ItemAmount = Convert.ToDouble(txtTotal.Text.ToString());
                    p.PurchaseType = cmbPurchaseType.Text;
                    var pds = p.PurchaseDetails .ToList();

                    foreach (var data in pds)
                    {
                        var sod = lstPurchase.Where(x => x.PDId == data.PDId).FirstOrDefault();
                        if (sod == null)
                        {
                            p.PurchaseDetails.Remove(data);
                        }
                    }


                    db.SaveChanges();

                    foreach (ItemsDetails data in lstPurchase)
                    {
                        PurchaseDetail tbd = new PurchaseDetail();
                        if (data.PDId == 0)
                        {
                            db.PurchaseDetails.Add(tbd);
                        }
                        else
                        {
                            tbd = db.PurchaseDetails.Where(x => x.PDId == data.PDId).FirstOrDefault();
                        }

                        //  tbd.SDId = ID;
                        tbd.PurchaseId  = p.PurchaseId;
                        tbd.ProductCode = data.ProductCode;
                        tbd.DisPer = data.DisPer == 0 ? data.Rate : data.DisPer;  //SellingRate
                        tbd.Quantity = data.Quantity;
                        tbd.Rate = data.Rate;  //MRP
                        tbd.TaxPer = (((data.Rate == 0 ? data.DisPer : data.Rate) - data.DisPer) * data.Quantity).ToString();  //SaveingAmount
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
                   // StockDetails.UpdateStockDetails();
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
            Reports.Transaction.frmPurchaseSearch frm = new Reports.Transaction.frmPurchaseSearch();
            frm.ShowDialog();
            ViewDetails(frm.PID);
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
                        var pur = db.Purchases.Where(x => x.PurchaseId == ID).FirstOrDefault();
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
                            db.PurchaseDetails.RemoveRange(db.PurchaseDetails.Where(x => x.PurchaseId == ID));
                db.SaveChanges();

                Purchase p1 = db.Purchases.Where(x => x.PurchaseId == ID).FirstOrDefault();
                db.Purchases.Remove(p1);
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

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
           Reports.Transaction.frmPurchaseReport  frm = new  Reports.Transaction.frmPurchaseReport();
            frm.ShowDialog();
        }
        #endregion

        #region GridEvents 

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var Pro = lstProduct.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault();

            if (Pro == null)
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = "Select Product.." }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");
                cmbItem.Focus();
            }
            else
            {
                try
                {
                    ItemsDetails tbd = lstPurchase.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault();
                    if (tbd == null)
                    {
                        tbd = new ItemsDetails();
                        lstPurchase.Add(tbd);
                    }
                    Double Amt = 0;
                    Double Total = 0; Double Dis = 0;

                    tbd.ProductCode = Pro.ProductId;
                    //tbd.SDId = Convert.ToInt16(ID);
                    tbd.SNo = lstPurchase.Count;

                    tbd.Rate = txtMRP.Text == "0" ? 0 : Convert.ToDouble(txtMRP.Text.ToString());
                    tbd.Quantity = txtQty.Text == "" ? 1 : Convert.ToDouble(txtQty.Text.ToString());
                    tbd.UOM = Pro.UnitsOfMeasurement.UOMId;
                    Dis = Convert.ToDouble(txtDisRATE.Text.ToString());
                    tbd.DisPer = Dis;
                    tbd.ProductName = Pro.ProductName;
                    tbd.UOMSymbol = Pro.UnitsOfMeasurement.UOMSymbol;
                    Amt = Convert.ToDouble(txtDisRATE.Text.ToString()) * (txtQty.Text == "" ? 1 : Convert.ToDouble(txtQty.Text.ToString()));
                    tbd.Amount = Amt;
                    tbd.Saveing = (((txtMRP.Text == "0" ? Convert.ToDouble(txtDisRATE.Text) : Convert.ToDouble(txtMRP.Text)) - Convert.ToDouble(txtDisRATE.Text)) * Convert.ToDouble(txtQty.Text.ToString()));
                    Total = Convert.ToDouble(Amt - Dis);
                    tbd.Total = Total;
                    tbd.Itemcode = Pro.ItemCode;

                    dgvDetails.ItemsSource = lstPurchase;
                    FindTotalAmount();
                    AddNewItem();

                }
                catch (Exception ex)
                {


                }

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
                txtMRP.Text = p.Rate.ToString();
                txtQty.Text = p.Quantity.ToString();
                //amt = (Convert.ToDouble(txtRate.Text.ToString()) * Convert.ToDouble(txtQty.Text.ToString()));
                txtAmount.Text = p.Amount.ToString();

                txtDisRATE.Text = p.DisPer.ToString();
                //UOM = p.UnitsOfMeasurement.UOMSymbol;                                

            }
            catch (Exception ex)
            { }

        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ItemsDetails tbd = btn.Tag as ItemsDetails;
            lstPurchase.Remove(tbd);
            FindTotalAmount();
        }
        private void AddNewItem()
        {
            txtItemCode.Clear();
            cmbItem.Text = "";
            txtMRP.Clear();
            txtQty.Clear();
            txtAmount.Clear();
            txtDisRATE.Clear();

        }
        #endregion

        #region userdefined

        private void FindTotalAmount()
        {
            try
            {
                
                double total = 0;
                txtTotItemAmount.Text = string.Format("{0:N2}", lstPurchase.Sum(x => x.Amount));
                //txtDiscount.Text = string.Format("{0:N2}", lstSalesDetails.Sum(x => x.DisPer));
                double dis = Convert.ToDouble(txtTotItemAmount.Text.ToString());

                txtDiscount.Text = "0";
                txtTotal.Text = string.Format("{0:N2}", dis);
                txtRound.Text = string.Format("{0:N2}", Math.Round(dis,MidpointRounding.AwayFromZero ));

                int test;
                test = Convert.ToInt32(dis);
                lblAmount.Text = "RS " + string.Format("{0:N2}", test.ToString());
                lblAmountInWords.Text = AppLib.NumberToWords(test).ToUpper();
                lblAmountInWords.Text += " ONLY.";
                // PaidAmount();
            }
            catch (Exception ex) { }
        }

        private void LoadWindow()
        {
            var v = db.Suppliers.Where(x => !string.IsNullOrEmpty(x.SupplierName)).OrderBy(x => x.LedgerName).ToList();
            cmbSupplier.ItemsSource = v;
            cmbSupplier.DisplayMemberPath = "LedgerName";
            cmbSupplier.SelectedValuePath = "SupplierId";

            var m = db.Suppliers.Where(x => !string.IsNullOrEmpty(x.MobileNo)).OrderBy(x => x.MobileNo).ToList();
            cmbMobileNumber.ItemsSource = m;
            cmbMobileNumber.DisplayMemberPath = "MobileNo";
            cmbMobileNumber.SelectedValuePath = "SupplierId";
            db = new JJSuperMarketEntities();
            lstProduct = db.Products.OrderBy(x => x.ProductName).ToList();
            
            cmbItem.ItemsSource = lstProduct.ToList();

            var inv = (db.Purchases.DefaultIfEmpty().Max(p => p == null ? 0 : p.InvoiceNo)) + 1;
            txtInNo.Text = inv.ToString();

            txtID.Text = (db.Purchases.DefaultIfEmpty().Max(p => p == null ? 0 : p.PurchaseId) + 1).ToString();
            dgvDetails.ItemsSource = lstPurchase;
        }

        private void LoadReport()
        {

        }

        private void txtItemCode_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var v = lstProduct.Where(x => x.ItemCode == txtItemCode.Text).FirstOrDefault();
                cmbItem.Text = v.ProductName;

                txtMRP.Text = v.MRP.ToString();
              //  UpdateAvailableStock();
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
                v = (Convert.ToDouble(txtMRP.Text.ToString())) * (Convert.ToDouble(txtQty.Text.ToString()));
                txtAmount.Text = v.ToString();
            }
            catch (Exception ex)
            {

            }
        }

        void UpdateAvailableStock()
        {
            var stock = StockDetails.toList.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault();
            if (stock == null)
            {
                txtAvailStk.Text = string.Empty;
            }
            else
            {
                txtAvailStk.Text = stock.ClStock.ToString();
            }
        }

        private void txtExtras_LostFocus(object sender, RoutedEventArgs e)
        {
            FindTotalAmount();
        }

        public void ViewDetails(decimal id)
        {
            try
            {
                Purchase p = db.Purchases.Where(x => x.PurchaseId == id).FirstOrDefault();
                ID = p.PurchaseId;
                txtID.Text = p.PurchaseId.ToString();
                txtInNo.Text = p.InvoiceNo.ToString();
                dtpS.SelectedDate = p.PurchaseDate;

                txtPaidAmount.Text = p.Narration;
                txtDiscount.Text = p.DiscountAmount.ToString();
                txtExtras.Text = p.Extra.ToString();
                cmbSupplier.Text = p.Supplier.SupplierName;


                lstPurchase.Clear();
                int s = 0;
                foreach (var data in p.PurchaseDetails.ToList())
                {
                    ItemsDetails i = new ItemsDetails();
                    i.DisPer = data.DisPer == 0 ? (double)data.Rate : Convert.ToDouble(data.DisPer);
                    s = s + 1;
                    i.SNo = s; i.UOMSymbol = data.UnitsOfMeasurement.UOMSymbol;
                    i.Itemcode = data.Product.ItemCode;
                    i.ProductName = data.Product.ProductName;
                    i.Rate = Convert.ToDouble(data.Rate);
                    i.Quantity = Convert.ToDouble(data.Quantity);
                    Double d = (i.DisPer * i.Quantity);
                    i.Saveing = Convert.ToDouble(data.TaxPer);
                    i.Amount = d;
                    double t = d - i.DisPer;

                    i.Total = t;
                    i.ProductCode = Convert.ToDecimal(data.ProductCode);
                    i.UOM = Convert.ToDecimal(data.UOM);
                    i.PDId = Convert.ToInt16(data.PDId);

                    lstPurchase.Add(i);
                }
                FindTotalAmount();

            }
            catch (Exception ex)
            {

            }
        }

        private async void txtItemCode_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                var b1 = lstProduct.Where(x => x.ItemCode == txtItemCode.Text).FirstOrDefault();
                if (b1 == null)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Barcode Not Found.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");

                    txtItemCode.Focus();
                }
                else
                {
                    txtQty.Focus();
                    double qty = 1;
                    var data = lstPurchase.Where(x => x.Itemcode == txtItemCode.Text).FirstOrDefault();
                    if (data != null)
                    {
                        qty = data.Quantity + 1;
                    }
                    txtQty.Text = qty.ToString();
                    var v = lstProduct.Where(x => x.ItemCode == txtItemCode.Text).FirstOrDefault();

                    txtDisRATE.Text = v.SellingRate.ToString();
                    txtMRP.Text = v.MRP.ToString();

                }
            }


        }
        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {

                try
                {
                    if (!string.IsNullOrEmpty(txtItemCode.Text))
                    {
                        var v = lstProduct.Where(x => x.ItemCode == txtItemCode.Text).FirstOrDefault();
                        cmbItem.Text = v.ProductName;
                        txtMRP.Text = v.MRP.ToString();
                        txtDisRATE.Text = v.SellingRate.ToString();
                        txtAmount.Text = v.SellingRate.ToString();
                        btnAdd_Click(sender, e);
                        txtItemCode.Focus();
                    }
                    else if (!string.IsNullOrEmpty(cmbItem.Text))
                    {
                        var v = lstProduct.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault();
                        cmbItem.Text = v.ProductName;
                        txtMRP.Text = v.SellingRate.ToString();
                        txtAmount.Text = v.SellingRate.ToString();
                        btnAdd_Click(sender, e);
                        txtItemCode.Focus();
                    }
                }
                catch (Exception ex)
                {

                }

            }
        }
        private void Showpoints(Customer cust)
        {
            lblPoint.Content = "";
            try
            {
                lblPoint.Content = (cust.Sales.Sum(x => x.ItemAmount) * 0.01);
            }
            catch (Exception ex) { }

        }
        private void cmbMobileNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cust = cmbMobileNumber.SelectedItem as Supplier;
            if (cust != null)
            {
                cmbSupplier.SelectedItem = cust;
            }
        }
        private void cmbCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cust = cmbSupplier.SelectedItem as Supplier;
            cmbMobileNumber.SelectedIndex = -1;
            if (cust != null)
            {
                cmbMobileNumber.SelectedItem = cust;
                //Showpoints(cust);
            }
        }
        private void PaidAmount()
        {
            try
            {
                FindTotalAmount();
                double total = 0, paid = 0;
                total = Convert.ToDouble(txtRound.Text);
                paid = Convert.ToDouble(txtPaidAmount.Text);
                txtBalAmount.Text = string.Format("{0:N2}", (paid - total));
            }
            catch (Exception ex) { }
        }
        private void txtPaidAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            PaidAmount();
        }

        private void txtRound_TextChanged(object sender, TextChangedEventArgs e)
        {
            PaidAmount();
        }
        private void cmbItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            var pro = cmbItem.SelectedItem as Product;
            // Product pr = cmbItem.SelectedValue  as Product;
            if (pro != null)
            {

                txtItemCode.Text = pro.ItemCode;
                txtQty.Text = "1";
                txtMRP.Text = pro.MRP.ToString();
                txtDisRATE.Text = pro.SellingRate.ToString();
                // cmbItem.SelectedItem = pro;
            }

        }
        private void cmbItem_TextChanged(object sender, RoutedEventArgs e)
        {
            cmbItem.ItemsSource = lstProduct.Where(p => p.ProductName.ToLower().Contains(cmbItem.Text.ToLower())).ToList();
        }
        public int m, cu;
        #endregion

        #region FormClear
        private void FormClear()
        {
            cmbMobileNumber.Text = "";
            cmbSupplier.Text = "";
            txtItemCode.Clear();
            cmbItem.Text = "";
            txtMRP.Clear();
            txtQty.Clear();
            txtAmount.Clear();
            txtDisRATE.Clear();
            lstPurchase.Clear();
            dgvDetails.ItemsSource = lstPurchase;
            txtTotItemAmount.Text = "0";
            txtDiscount.Text = "0";
            txtExtras.Text = "0";
            txtRound.Text = "0";
            txtTotal.Text = "0";
            txtNarration.Text = "";
            lblAmount.Text = string.Empty;
            lblAmountInWords.Text = string.Empty;
            txtPaidAmount.Clear();
            txtBalAmount.Clear();

            ID = 0;

            LoadWindow();
            dtpS.SelectedDate = DateTime.Today;
        }
        #endregion

        #region class
        partial class ItemsDetails : INotifyPropertyChanged
        {
            private int _PDId;
            private decimal _ProductCode;
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
            private int _SNo;


            public int SNo { get { return _SNo; } set { if (_SNo != value) { _SNo = value; NotifyPropertyChanged("SNo"); } } }

            public int PDId { get { return _PDId; } set { if (_PDId != value) { _PDId = value; NotifyPropertyChanged("PDId"); } } }


            public decimal ProductCode
            {
                get
                {
                    return _ProductCode;
                }
                set
                {
                    if (_ProductCode != value)
                    {
                        _ProductCode = value;
                        NotifyPropertyChanged("ProductCode");
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
            public double Saveing
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
                        NotifyPropertyChanged("Saveing");
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
        
        #region ProductSearch
        private void PackIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            frmProductDetails frm = new Transaction.frmProductDetails();
            frm.ShowDialog();
            ProductName(frm.ProName);

        }

        private void btnPSearch_Click(object sender, RoutedEventArgs e)
        {
            Reports.Transaction.frmSalesReturnSearchNew frm = new Reports.Transaction.frmSalesReturnSearchNew();
            frm.ShowDialog();
            ViewPurchaseDetails(frm.SID);
        }

        public void ViewPurchaseDetails(decimal id)
        {
            try
            {
                Purchase p = db.Purchases.Where(x => x.PurchaseId == id).FirstOrDefault();
                // SID = p.SalesId;
                txtID.Text = p.PurchaseId.ToString();
                txtInNo.Text = p.InvoiceNo.ToString();
                // dtpS.SelectedDate = p.SalesDate;

                txtPaidAmount.Text = p.Narration;
                txtDiscount.Text = p.DiscountAmount.ToString();
                txtExtras.Text = p.Extra.ToString();
                cmbSupplier.Text = p.Supplier.SupplierName;


                lstPurchase.Clear();

                foreach (var data in p.PurchaseDetails.ToList())
                {
                    ItemsDetails i = new ItemsDetails();
                    i.DisPer = data.DisPer == 0 ? (double)data.Rate : Convert.ToDouble(data.DisPer);
                    i.UOMSymbol = data.UnitsOfMeasurement.UOMSymbol;
                    i.Itemcode = data.Product.ItemCode;
                    i.ProductName = data.Product.ProductName;
                    i.Rate = Convert.ToDouble(data.Rate);
                    i.Quantity = Convert.ToDouble(data.Quantity);
                    Double d = (i.DisPer * i.Quantity);
                    i.Saveing = Convert.ToDouble(data.TaxPer);
                    i.Amount = d;
                    double t = d - i.DisPer;

                    i.Total = t;
                    i.ProductCode = Convert.ToDecimal(data.ProductCode);
                    i.UOM = Convert.ToDecimal(data.UOM);
                    //i.SDId = Convert.ToInt16(data.SDId);

                    lstPurchase.Add(i);
                }
                FindTotalAmount();

            }
            catch (Exception ex)
            {

            }
        }

        public void ProductName(string Name)
        {
            txtQty.Focus();
            double qty = 1;
            var data = lstProduct.Where(x => x.ProductName == Name).FirstOrDefault();
            var data1 = lstPurchase.Where(x => x.ProductName == Name).FirstOrDefault();
            if(Name != null)
            {
                cmbItem.Text = Name;
                txtItemCode.Text = data.ItemCode == "" ? "" : data.ItemCode.ToString();
                txtDisRATE.Text = data.SellingRate.ToString();
                txtMRP.Text = data.MRP.ToString();

                if (data1 != null)
                {
                    qty = data1.Quantity + 1;
                }
                txtQty.Text = qty.ToString();
            }
           

        }
        #endregion
    }
}
