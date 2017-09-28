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
    /// Interaction logic for frmSalesReturnNew.xaml
    /// </summary>
    public partial class frmSalesReturnNew : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();

        List<Product> lstProduct = new List<Product>();

        ObservableCollection<ItemsDetails> lstSalesReturnDetails = new ObservableCollection<ItemsDetails>();
        

        decimal ID = 0;
        decimal SID = 0;
        string TextToPrint = "";
        List c;
        public frmSalesReturnNew()
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
            if (lstSalesReturnDetails.Count == 0)
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

                SalesReturn  p = new SalesReturn();
                double tot, paid;
                tot = Convert.ToDouble(txtRound.Text);
                paid = Convert.ToDouble(txtPaidAmount.Text == "" ? "0.00" : txtPaidAmount.Text);
                if (cmbCustomer.Text == "")
                {
                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Customer..." }
                    };
                    cmbCustomer.Focus();
                    await DialogHost.Show(Information, "RootDialog");
                }
                else if (cmbMobileNumber.Text == "")
                {
                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Customer Mobile Number..." }
                    };
                    cmbMobileNumber.Focus();
                    await DialogHost.Show(Information, "RootDialog");
                }
                else if (txtPaidAmount.Text == "" || txtPaidAmount.Text == "0.00" && cmbSalesType.SelectedIndex != 1)
                {
                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = "Please Enter the Paid Amount..." }
                    };
                    txtPaidAmount.Focus();
                    await DialogHost.Show(Information, "RootDialog");
                }
                else if (lstSalesReturnDetails.Count == 0)
                {
                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = "Please Enter the Product Details..." }
                    };
                    txtPaidAmount.Focus();
                    await DialogHost.Show(Information, "RootDialog");
                }
                else if (paid < tot && cmbSalesType.SelectedIndex != 1)
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
                    txtPaidAmount.Text = cmbSalesType.SelectedIndex == 1 ? "0.00" : txtPaidAmount.Text;
                    if (ID == 0)
                    {
                        db.SalesReturns .Add(p);

                    }
                    else
                    {
                        p = db.SalesReturns .Where(x => x.SRId  == ID).FirstOrDefault();
                    }
                    p.DiscountAmount =txtDiscount.Text==""?0: Convert.ToDouble(txtDiscount.Text.ToString());
                    p.Extra = txtExtras.Text==""?0: Convert.ToDouble(txtExtras.Text.ToString());
                    p.InvoiceNo = Convert.ToDecimal(txtInNo.Text.ToString());

                    // p.LedgerCode = Convert.ToDecimal(cmbCustomer.SelectedValue.ToString());
                    p.LedgerCode = db.Customers.Where(x => x.CustomerName == cmbCustomer.Text).Select(x => x.CustomerId).FirstOrDefault();
                    p.Narration = txtPaidAmount.Text;
                    p.SRDate  = dtpS.SelectedDate;
                    p.ItemAmount = Convert.ToDouble(txtTotal.Text.ToString());
                    p.SRType  = cmbSalesType.Text;
                    var sods = p.SalesReturnDetails.ToList();

                    foreach (var data in sods)
                    {
                        var sod = lstSalesReturnDetails.Where(x => x.SDId == data.SRDId).FirstOrDefault();
                        if (sod == null)
                        {
                            p.SalesReturnDetails.Remove(data);
                        }
                    }


                    db.SaveChanges();

                    foreach (ItemsDetails data in lstSalesReturnDetails)
                    {
                        SalesReturnDetail tbd = new SalesReturnDetail();
                        if (data.SDId == 0)
                        {
                            db.SalesReturnDetails.Add(tbd);
                        }
                        else
                        {
                            tbd = db.SalesReturnDetails.Where(x => x.SRDId == data.SDId).FirstOrDefault();
                        }

                        //  tbd.SDId = ID;
                        tbd.SRId   = p.SRId;
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
                    //StockDetails.UpdateStockDetails();
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
            ViewDetails(frm.SRID);
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
                        var srin = db.SalesReturns.Where(x => x.SRId == ID).FirstOrDefault();
                        var rec = db.PaymentMasters.Where(x => x.SalesRId == srin.InvoiceNo).Count();
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
                            db.SalesReturnDetails.RemoveRange(db.SalesReturnDetails.Where(x => x.SRId == ID));
                            db.SaveChanges();

                            SalesReturn p1 = db.SalesReturns.Where(x => x.SRId == ID).FirstOrDefault();
                            db.SalesReturns.Remove(p1);
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
            Transaction.frmSalesReturnReport frm = new frmSalesReturnReport();
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
                    ItemsDetails tbd = lstSalesReturnDetails.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault();
                    if (tbd == null)
                    {
                        tbd = new ItemsDetails();
                        lstSalesReturnDetails.Add(tbd);
                    }
                    Double Amt = 0;
                    Double Total = 0; Double Dis = 0;

                    tbd.ProductCode = Pro.ProductId;
                    //tbd.SDId = Convert.ToInt16(ID);

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

                    dgvDetails.ItemsSource = lstSalesReturnDetails;
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
            lstSalesReturnDetails.Remove(tbd);
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
                var c = db.CompanyDetails.FirstOrDefault();
                double total = 0;
                txtTotItemAmount.Text = string.Format("{0:N2}", lstSalesReturnDetails.Sum(x => x.Amount));
                //txtDiscount.Text = string.Format("{0:N2}", lstSalesDetails.Sum(x => x.DisPer));
                double dis = Convert.ToDouble(txtTotItemAmount.Text.ToString());

                
                    txtDiscount.Text = "0";
                    txtTotal.Text = string.Format("{0:N2}", dis );
                    txtRound.Text = string.Format("{0:N2}", Math.Round(dis,MidpointRounding.AwayFromZero));
                



                int test;
                test = Convert.ToInt32(dis );
                lblAmount.Text = "RS " + string.Format("{0:N2}", test.ToString());
                lblAmountInWords.Text = AppLib.NumberToWords(test).ToUpper();
                lblAmountInWords.Text += " ONLY.";
                // PaidAmount();
            }
            catch (Exception ex) { }
        }

        private void LoadWindow()
        {


            var v = db.Customers.Where(x => !string.IsNullOrEmpty(x.CustomerName)).OrderBy(x => x.CustomerName).ToList();
            cmbCustomer.ItemsSource = v;
            cmbCustomer.DisplayMemberPath = "CustomerName";
            cmbCustomer.SelectedValuePath = "CustomerId";

            var m = db.Customers.Where(x => !string.IsNullOrEmpty(x.MobileNo)).OrderBy(x => x.MobileNo).ToList();
            cmbMobileNumber.ItemsSource = m;
            cmbMobileNumber.DisplayMemberPath = "MobileNo";
            cmbMobileNumber.SelectedValuePath = "CustomerId";
            db = new JJSuperMarketEntities();
            lstProduct = db.Products.OrderBy(x => x.ProductName).ToList();
           
            cmbItem.ItemsSource =lstProduct.ToList();

            var inv = (db.SalesReturns .DefaultIfEmpty().Max(p => p == null ? 0 : p.InvoiceNo)) + 1;
            txtInNo.Text = inv.ToString();

            txtID.Text = (db.SalesReturns .DefaultIfEmpty().Max(p => p == null ? 0 : p.SRId) + 1).ToString();
            dgvDetails.ItemsSource = lstSalesReturnDetails;
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
               // UpdateAvailableStock();
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
                SalesReturn p = db.SalesReturns .Where(x => x.SRId == id).FirstOrDefault();
                ID = p.SRId;
                txtID.Text = p.SRId.ToString();
                txtInNo.Text = p.InvoiceNo.ToString();
                dtpS.SelectedDate = p.SRDate;

                txtPaidAmount.Text = p.Narration;
                txtDiscount.Text = p.DiscountAmount.ToString();
                txtExtras.Text = p.Extra.ToString();
                cmbCustomer.Text = p.Customer.CustomerName;


                lstSalesReturnDetails.Clear();

                foreach (var data in p.SalesReturnDetails.ToList())
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
                    i.SDId = Convert.ToInt16(data.SRDId);

                    lstSalesReturnDetails.Add(i);
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
                    var data = lstSalesReturnDetails.Where(x => x.Itemcode == txtItemCode.Text).FirstOrDefault();
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
            var cust = cmbMobileNumber.SelectedItem as Customer;
            if (cust != null)
            {
                cmbCustomer.SelectedItem = cust;
            }
        }
        private void cmbCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cust = cmbCustomer.SelectedItem as Customer;
            cmbMobileNumber.SelectedIndex = -1;
            if (cust != null)
            {
                cmbMobileNumber.SelectedItem = cust;
                Showpoints(cust);
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
                txtBalAmount.Text = string.Format("{0:N2}", (paid - total) );
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
            cmbCustomer.Text = "";
            txtItemCode.Clear();
            cmbItem.Text = "";
            txtMRP.Clear();
            txtQty.Clear();
            txtAmount.Clear();
            txtDisRATE.Clear();
            lstSalesReturnDetails.Clear();
            dgvDetails.ItemsSource = lstSalesReturnDetails;
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
            private int _SDId;
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



            public int SDId { get { return _SDId; } set { if (_SDId != value) { _SDId = value; NotifyPropertyChanged("SDId"); } } }


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
            ViewSalesDetails(frm.SID);
        }

        public void ViewSalesDetails(decimal id)
        {
            db = new JJSuperMarketEntities();
            try
            {
                Sale p =  db.Sales.Where(x => x.SalesId == id).FirstOrDefault();
               // SID = p.SalesId;
                txtID.Text = p.SalesId.ToString();
                txtInNo.Text = p.InvoiceNo.ToString();
               // dtpS.SelectedDate = p.SalesDate;

                txtPaidAmount.Text = p.Narration;
                txtDiscount.Text = p.DiscountAmount.ToString();
                txtExtras.Text = p.Extra.ToString();
                cmbCustomer.Text = p.Customer.CustomerName;


                lstSalesReturnDetails.Clear();

                foreach (var data in p.SalesDetails.ToList())
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

                    lstSalesReturnDetails.Add(i);
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
            var data1 = lstSalesReturnDetails.Where(x => x.ProductName == Name).FirstOrDefault();
            if (Name != null)
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
