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
    /// Interaction logic for frmPurchaseReturn.xaml
    /// </summary>
    public partial class frmPurchaseReturn : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        string UOM;
        ObservableCollection<ItemsDetails> lstPurchaseReturnDetail = new ObservableCollection<ItemsDetails>();
        decimal ID = 0;
        string TextToPrint = "";

        public frmPurchaseReturn()
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
                else if (lstPurchaseReturnDetail.Count == 0)
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

                    PurchaseReturn  p = new PurchaseReturn ();

                    if (ID == 0)
                    {
                        db.PurchaseReturns .Add(p);

                    }
                    else
                    {
                        p = db.PurchaseReturns .Where(x => x.PRId  == ID).FirstOrDefault();
                    }
                    p.DiscountAmount = Convert.ToDouble(txtDiscount.Text.ToString());
                    p.Extra = Convert.ToDouble(txtExtras.Text.ToString());
                    p.LedgerCode = Convert.ToDecimal(cmbSupplier.SelectedValue.ToString());
                    p.Narration = txtNarration.Text;
                    p.PRDate  = dtpP.SelectedDate;
                    p.ItemAmount  = Convert.ToDouble(txtTotal.Text.ToString());
                    p.PRType  = cmbPurchaseType.Text;

                    var prds = p.PurchaseReturnDetails.ToList();

                    foreach (var data in prds)
                    {
                        var pod = lstPurchaseReturnDetail.Where(x => x.PRDId == data.PRDId).FirstOrDefault();
                        if (pod == null)
                        {
                            p.PurchaseReturnDetails.Remove(data);
                        }
                    }

                    db.SaveChanges();

                    foreach (ItemsDetails data in lstPurchaseReturnDetail)
                    {
                        PurchaseReturnDetail  tbd = new PurchaseReturnDetail();
                        if (data.PRDId == 0)
                        {
                            db.PurchaseReturnDetails.Add(tbd);
                        }
                        else
                        {
                            tbd = db.PurchaseReturnDetails.Where(x => x.PRDId == data.PRDId).FirstOrDefault();
                        }
                        //tbd.PODId = ID;
                        tbd.PRId  = p.PRId ;
                        tbd.ProductCode = data.PRCode;
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
            Reports.Transaction.frmPurchaseReturnSearch  frm = new Reports.Transaction.frmPurchaseReturnSearch();
            frm.ShowDialog();
            ViewDetails(frm.PRID );
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.PurchaseReturnDetails .RemoveRange(db.PurchaseReturnDetails.Where(x => x.PRId  == ID));
                db.SaveChanges();

                PurchaseReturn  p1 = db.PurchaseReturns .Where(x => x.PRId  == ID).FirstOrDefault();
                db.PurchaseReturns .Remove(p1);
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
            Reports.Transaction.frmPurchaseReturnReport frm = new Reports.Transaction.frmPurchaseReturnReport();
            frm.ShowDialog();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                System.Drawing.Printing.PrintDocument prnPurchaseOrder = new System.Drawing.Printing.PrintDocument();
                prnPurchaseOrder.PrintPage += PrnPurchase_PrintPage;

                prnPurchaseOrder.DefaultPageSettings.PrinterSettings.PrinterName = @"\\192.168.0.18\POS-58";
                prnPurchaseOrder.PrintController = new System.Drawing.Printing.StandardPrintController();
                TextToPrint = PrintLine("JJ SUPERMARKET", PrintTextAlignType.Center);
                TextToPrint += PrintLine("Green Garden", PrintTextAlignType.Center);
                TextToPrint += PrintLine("41 Grd. Flr.,Annai Avenue,", PrintTextAlignType.Center);
                TextToPrint += PrintLine("Thiruvalluvar St,", PrintTextAlignType.Center);
                TextToPrint += PrintLine("Om Shakthi Nagar, ", PrintTextAlignType.Center);
                TextToPrint += PrintLine("Ambattur, Kalikuppam", PrintTextAlignType.Center);
                TextToPrint += PrintLine("Chennai-600053", PrintTextAlignType.Center);

                TextToPrint += PrintLine(string.Format("{0}CASH BILL{0}", new string('-', 8)), PrintTextAlignType.Center);

                TextToPrint += PrintLine(string.Format("Dt: {0:dd/MM/yyyy hh:mm tt}", DateTime.Now), PrintTextAlignType.Left);
                TextToPrint += PrintLine(string.Format("Bill No: {0}", txtInNo.Text), PrintTextAlignType.Left);
                TextToPrint += PrintLine("TIN:33886938482", PrintTextAlignType.Left);

                TextToPrint += PrintLine(string.Format("{0}", new string('-', PrintNoOfCharPerLine)), PrintTextAlignType.Center);
                TextToPrint += PrintLine(string.Format("{0,3} {1,-11} {2,10}", "SNo", "Particulars", "Amount"), PrintTextAlignType.Left);
                TextToPrint += PrintLine(string.Format("{0}", new string('-', PrintNoOfCharPerLine)), PrintTextAlignType.Center);

                int sno = 0;

                foreach (var data in lstPurchaseReturnDetail)
                {
                    TextToPrint += PrintLine(string.Format("{0,3} {1,-11} {2,10:0.00}", ++sno, data.ProductName, data.Rate * data.Quantity), PrintTextAlignType.Left);
                    TextToPrint += PrintLine(string.Format("{0,3} [Rs. {1} x {2} {3}]", "", data.Rate, data.Quantity, data.UOMSymbol), PrintTextAlignType.Left);
                }
                TextToPrint += PrintLine(string.Format("{0}", new string('-', PrintNoOfCharPerLine)), PrintTextAlignType.Center);
                TextToPrint += PrintLine(string.Format("Total   : {0,10:0.00}", Convert.ToDouble(txtTotItemAmount.Text)), PrintTextAlignType.Right);
                TextToPrint += PrintLine(string.Format("Discount: {0,10:0.00}", Convert.ToDouble(txtDiscount.Text)), PrintTextAlignType.Right);
                TextToPrint += PrintLine("", PrintTextAlignType.Left);
                TextToPrint += PrintLine(string.Format("Bill Amount : RS.{0:0.00}", Convert.ToDouble(txtTotal.Text)), PrintTextAlignType.Center);
                TextToPrint += PrintLine("", PrintTextAlignType.Left);
                TextToPrint += PrintLine("", PrintTextAlignType.Left);
                TextToPrint += PrintLine("", PrintTextAlignType.Left);


                prnPurchaseOrder.Print();
                MessageBox.Show("Printing..");
            }
            catch(Exception ex)
            {

            }
        }

        #endregion

        #region GridEvents 

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            Product p = cmbItem.SelectedItem as Product;
            var Pro = db.Products.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault();
            if (Pro == null)
            {
                MessageBox.Show("Please Select Product..");
            }
            else
            {
                ItemsDetails tbd = lstPurchaseReturnDetail.Where(x => x.ProductName == p.ProductName).FirstOrDefault();
                if (tbd == null)
                {
                    tbd = new ItemsDetails();
                    lstPurchaseReturnDetail.Add(tbd);
                }

                Double Amt = 0;
                Double Total = 0; Double Dis = 0;

                tbd.PRCode = p.ProductId;
               // tbd.SRDId = Convert.ToInt16(ID);

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

                dgvDetails.ItemsSource = lstPurchaseReturnDetail;
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
            lstPurchaseReturnDetail.Remove(tbd);
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
            txtTotItemAmount.Text = string.Format("{0:0.00}", lstPurchaseReturnDetail.Sum(x => x.Amount));
            txtDiscount.Text = string.Format("{0:0.00}", lstPurchaseReturnDetail.Sum(x => x.DisPer));
            total = ((Convert.ToDouble(txtTotItemAmount.Text.ToString()) - Convert.ToDouble(txtDiscount.Text.ToString())) + Convert.ToDouble(txtExtras.Text.ToString()));
            txtTotal.Text = total.ToString();
            int r = Convert.ToInt16(Math.Round(total));
            txtRound.Text = r.ToString();
            int test;
            test = Convert.ToInt32(total);
            lblAmount.Content =  test;
            lblAmountInWords.Content = AppLib.NumberToWords(test).ToUpper();
            lblAmountInWords.Content += " Only.";
        }

        private void LoadWindow()
        {
            var v = db.Suppliers .ToList();
            cmbSupplier.ItemsSource = v;
            cmbSupplier.DisplayMemberPath = "SupplierName";
            cmbSupplier.SelectedValuePath = "SupplierId";


            var i = db.Products.ToList();
            cmbItem.ItemsSource = i;
            cmbItem.DisplayMemberPath = "ProductName";
            cmbItem.SelectedValuePath = "ProductId";
           
            txtID.Text = (db.PurchaseReturns .DefaultIfEmpty().Max(p => p == null ? 0 : p.PRId ) + 1).ToString();
            txtInNo.Text= (db.PurchaseReturns.DefaultIfEmpty().Max(p => p == null ? 0 : p.InvoiceNo) + 1).ToString();

            var POID = db.Purchases.ToList();
            cmbPID.ItemsSource = POID;
            cmbPID.DisplayMemberPath = "InvoiceNo";
            cmbPID.SelectedValuePath = "PurchaseId";


            dgvDetails.ItemsSource = lstPurchaseReturnDetail;
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
            cmbSupplier.Text = "";
            txtItemCode.Clear();
            cmbItem.Text = "";
            txtRate.Text = "0";
            txtQty.Text = "0";
            txtAmount.Text = "0";
            txtDisAmt.Text = "0";
            lstPurchaseReturnDetail.Clear();
            dgvDetails.ItemsSource = lstPurchaseReturnDetail;
            txtTotItemAmount.Text = "0";
            txtDiscount.Text = "0";
            txtExtras.Text = "0";
            txtRound.Text = "0";
            txtTotal.Text = "0";
            cmbPurchaseType.Text = "";
            ID = 0;
            txtNarration.Text = "";
            lblAmount.Content = "";
            lblAmountInWords.Content = "";
            cmbPID.Text = "";
            dtpP.SelectedDate = DateTime.Today;
            LoadWindow();

        }

        public void ViewDetails(decimal id)
        {
            try
            {
                PurchaseReturn  p = db.PurchaseReturns .Where(x => x.PRId  == id).FirstOrDefault();
                ID = p.PRId ;
                txtID.Text = p.PRId .ToString();
                 txtInNo.Text = p.InvoiceNo.ToString();
                dtpP.SelectedDate = p.PRDate ;

                txtNarration.Text = p.Narration;
                txtDiscount.Text = p.DiscountAmount.ToString();
                 txtExtras.Text = p.Extra.ToString();
                cmbSupplier.Text = p.Supplier.SupplierName.ToString();
                cmbPurchaseType.Text = p.PRType ;

                lstPurchaseReturnDetail.Clear();
                foreach (var data in p.PurchaseReturnDetails .ToList())
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
                    i.PRCode = Convert.ToDecimal(data.ProductCode);
                    i.UOM = Convert.ToDecimal(data.UOM);
                    i.PRDId = Convert.ToInt16(data.PRDId );

                    lstPurchaseReturnDetail.Add(i);
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
                Purchase p = db.Purchases.Where(x => x.PurchaseId == d).FirstOrDefault();
                cmbSupplier.Text = p.Supplier.SupplierName;
                txtNarration.Text = p.Narration;
                lstPurchaseReturnDetail.Clear();
                foreach (var data in p.PurchaseDetails.ToList())
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
                    i.PRCode = Convert.ToDecimal(data.ProductCode);
                    i.UOM = Convert.ToDecimal(data.UOM);

                    lstPurchaseReturnDetail.Add(i);
                }
                FindTotalAmount();
            }
            catch(Exception ex)
            {

            }

        }

        private void cmbPID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = Convert.ToDecimal(cmbPID.SelectedValue);
            orderDetails(d);
        }

        #endregion

        #region class
        partial class ItemsDetails : INotifyPropertyChanged
        {
            private int _PRId;

            private decimal _PRCode;
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


            public int PRDId { get { return _PRId; } set { if (_PRId != value) { _PRId = value; NotifyPropertyChanged("_PRId"); } } }


            public decimal PRCode
            {
                get
                {
                    return _PRCode;
                }
                set
                {
                    if (_PRCode != value)
                    {
                        _PRCode = value;
                        NotifyPropertyChanged("PRCode");
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

        #region Print 
        enum PrintTextAlignType
        {
            Left,
            Center,
            Right
        }
        int PrintNoOfCharPerLine = 27;
        String PrintLine(string Text, PrintTextAlignType AlignType)
        {

            String RValue = "";
            if (AlignType == PrintTextAlignType.Left)
            {
                RValue = Text;
            }
            else if (AlignType == PrintTextAlignType.Center)
            {
                RValue = new string(' ', Math.Abs(PrintNoOfCharPerLine - Text.Length) / 2) + Text;
            }
            else
            {
                RValue = new string(' ', Math.Abs(PrintNoOfCharPerLine - Text.Length)) + Text;
            }
            return RValue + System.Environment.NewLine;

        }

        private void PrnPurchase_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            System.Drawing.Font textfont = new System.Drawing.Font("Courier New", 8, System.Drawing.FontStyle.Regular);

            int currentChar = 0;
            int w = 0, h = 0, left = 0, top = 0;
            System.Drawing.Rectangle b = new System.Drawing.Rectangle(left, top, w, h);
            StringFormat format = new StringFormat(StringFormatFlags.LineLimit);
            int line = 0, chars = 0;

            e.Graphics.MeasureString(TextToPrint, textfont, new System.Drawing.SizeF(0, 0), format, out chars, out line);
            e.Graphics.DrawString(TextToPrint.Substring(currentChar, chars), textfont, System.Drawing.Brushes.Black, b, format);

            currentChar = currentChar + chars;
            if (currentChar < TextToPrint.Length)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
                currentChar = 0;
            }

        }

        #endregion

    }
}
