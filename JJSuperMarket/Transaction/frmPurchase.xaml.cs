using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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
using AccountsBuddy.Domain;
using MaterialDesignThemes.Wpf;

namespace AccountsBuddy.Transaction
{
    /// <summary>
    /// Interaction logic for frmPurchase.xaml
    /// </summary>
    public partial class frmPurchase : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        string UOM;
        ObservableCollection<ItemsDetails> lstPurchaseDetails = new ObservableCollection<ItemsDetails>();
        decimal ID = 0;
        string TextToPrint = "";

        public frmPurchase()
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
                if (cmbSupplier.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Supplier Name.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    cmbSupplier.Focus();
                }
                else if (lstPurchaseDetails.Count == 0)
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

                    Purchase p = new Purchase();

                    if (ID == 0)
                    {
                        db.Purchases.Add(p);

                    }
                    else
                    {
                        p = db.Purchases.Where(x => x.PurchaseId == ID).FirstOrDefault();
                    }
                    p.DiscountAmount = Convert.ToDouble(txtDiscount.Text.ToString());
                    p.Extra = Convert.ToDouble(txtExtras.Text.ToString());
                    p.InvoiceNo = Convert.ToDecimal(txtInNo.Text.ToString());
                    p.LedgerCode = Convert.ToDecimal(cmbSupplier.SelectedValue.ToString());
                    p.Narration = txtNarration.Text;
                    p.PurchaseDate = dtpP.SelectedDate;
                    p.ItemAmount = Convert.ToDouble(txtTotal.Text.ToString());
                    p.PurchaseType = cmbPurchaseType.Text;


                    var pods = p.PurchaseDetails.ToList();

                    foreach (var data in pods)
                    {
                        var pod = lstPurchaseDetails.Where(x => x.PODId == data.PDId).FirstOrDefault();
                        if (pod == null)
                        {
                            p.PurchaseDetails.Remove(data);
                        }
                    }

                    db.SaveChanges();

                    foreach (ItemsDetails data in lstPurchaseDetails)
                    {
                        PurchaseDetail tbd = new PurchaseDetail();
                        if (data.PODId == 0)
                        {
                            db.PurchaseDetails.Add(tbd);
                        }
                        else
                        {
                            tbd = db.PurchaseDetails.Where(x => x.PDId == data.PODId).FirstOrDefault();
                        }

                        //tbd.PODId = ID;
                        tbd.PurchaseId = p.PurchaseId;
                        tbd.ProductCode = data.ProductCode;
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
                    StockDetails.UpdateStockDetails();
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
                LoadWindow();
                FormClear();
            }
            catch (Exception ex)
            {

            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            Reports.Transaction.frmPurchaseReport frm = new Reports.Transaction.frmPurchaseReport();
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
                ItemsDetails tbd = lstPurchaseDetails.Where(x => x.ProductName == p.ProductName).FirstOrDefault();
                if (tbd == null)
                {
                    tbd = new ItemsDetails();
                    lstPurchaseDetails.Add(tbd);
                }

                Double Amt = 0;
                Double Total = 0; Double Dis = 0;

                tbd.ProductCode = p.ProductId;
               // tbd.PODId = Convert.ToInt16(ID);

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

                dgvDetails.ItemsSource = lstPurchaseDetails;
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
            lstPurchaseDetails.Remove(tbd);
            FindTotalAmount();
        }
        private void AddNewItem()
        {
            txtItemCode.Clear();
            cmbItem.Text = "";
            cmbPOID.Text = "";
            cmbPurchaseType.Text = "";
            cmbSupplier.Text = "";
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
            txtTotItemAmount.Text = string.Format("{0:0.00}", lstPurchaseDetails.Sum(x => x.Amount));
            txtDiscount.Text = string.Format("{0:0.00}", lstPurchaseDetails.Sum(x => x.DisPer));
            total = ((Convert.ToDouble(txtTotItemAmount.Text.ToString()) - Convert.ToDouble(txtDiscount.Text.ToString())) + Convert.ToDouble(txtExtras.Text.ToString()));
            txtTotal.Text = total.ToString();
            int r = Convert.ToInt16(Math.Round(total));
            txtRound.Text = r.ToString();
            int test;
            test = Convert.ToInt32(total);
            lblAmountInWords.Text = AppLib.NumberToWords(test) + " ONLY.".ToUpper();
            lblAmount.Text = txtTotal.Text;
        }

        private void LoadWindow()
        {
            var v = db.Suppliers.ToList();
            cmbSupplier.ItemsSource = v;
            cmbSupplier.DisplayMemberPath = "SupplierName";
            cmbSupplier.SelectedValuePath = "SupplierId";


            var i = db.Products.ToList();
            cmbItem.ItemsSource = i;
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "ProductId";
            var inv = (db.Purchases.DefaultIfEmpty().Max(p => p == null ? 0 : p.InvoiceNo)) + 1;
            txtInNo.Text = inv.ToString();
            txtID.Text = (db.Purchases.DefaultIfEmpty().Max(p => p == null ? 0 : p.PurchaseId) + 1).ToString();

            var POID = db.PurchaseOrders.ToList();
            cmbPOID.ItemsSource = POID;
            cmbPOID.DisplayMemberPath = "InvoiceNo";
            cmbPOID.SelectedValuePath = "POId";


            dgvDetails.ItemsSource = lstPurchaseDetails;
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
            cmbSupplier.Text = "";
            txtItemCode.Clear();
            cmbItem.Text = "";
            txtRate.Text = "0";
            txtQty.Text = "0";
            txtAmount.Text = "0";
            txtDisAmt.Text = "0";
            lstPurchaseDetails.Clear();
            dgvDetails.ItemsSource = lstPurchaseDetails;
            txtTotItemAmount.Text = "0";
            txtDiscount.Text = "0";
            txtExtras.Text = "0";
            txtRound.Text = "0";
            txtTotal.Text = "0";
            ID = 0;
            lblAmount.Text = string.Empty;
            lblAmountInWords.Text = string.Empty;
            dtpP.SelectedDate = DateTime.Today;
            LoadWindow();

        }

        public void ViewDetails(decimal id)
        {
            try
            {
                Purchase p = db.Purchases.Where(x => x.PurchaseId == id).FirstOrDefault();
                ID = p.PurchaseId;
                txtID.Text = p.PurchaseId.ToString();
                txtInNo.Text = p.InvoiceNo.ToString();
                dtpP.SelectedDate = p.PurchaseDate  ;

                txtNarration.Text = p.Narration;
                txtDiscount.Text = p.DiscountAmount.ToString();
                 txtExtras.Text = p.Extra.ToString();
                cmbSupplier.Text = p.Supplier.SupplierName;


                lstPurchaseDetails.Clear();
                foreach (var data in p.PurchaseDetails.ToList())
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
                    i.ProductCode = Convert.ToDecimal(data.ProductCode);
                    i.UOM = Convert.ToDecimal(data.UOM);
                    i.PODId = Convert.ToInt16(data.PDId);

                    lstPurchaseDetails.Add(i);
                }
                FindTotalAmount();

            }
            catch (Exception ex)
            {

            }
        }

        public void orderDetails(decimal d)
        {
            PurchaseOrder p = db.PurchaseOrders.Where(x => x.POId == d).FirstOrDefault();
            cmbSupplier.Text = p.Supplier.SupplierName;
            txtNarration.Text = p.Narration;
            foreach (var data in p.PurchaseOrderDetails.ToList())
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
                i.ProductCode = Convert.ToDecimal(data.ProductCode);
                i.UOM = Convert.ToDecimal(data.UOM);
              
                lstPurchaseDetails.Add(i);
            }
            FindTotalAmount();


        }

        private void cmbPOID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = Convert.ToDecimal(cmbPOID.SelectedValue);
            orderDetails(d);
        }

        #endregion

        #region class
        partial class ItemsDetails : INotifyPropertyChanged
        {
            private int _PurchaseId;

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


            public int PODId { get { return _PurchaseId; } set { if (_PurchaseId != value) { _PurchaseId = value; NotifyPropertyChanged("_PurchaseId"); } } }


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

       
        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnView_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }


}