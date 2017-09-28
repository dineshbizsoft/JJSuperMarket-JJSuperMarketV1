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
    /// Interaction logic for frmSales.xaml
    /// </summary>
    public partial class frmSales : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();

        List<Product> lstProduct = new List<Product>();
        int i = 0;

        ObservableCollection<ItemsDetails> lstSalesDetails = new ObservableCollection<ItemsDetails>();
        decimal ID = 0;
        string TextToPrint = "";
        List c;
        Double points;
        public frmSales()
        {
            InitializeComponent();

            txtExtras.Text = "0";

        }

        public void UserControl_Loaded(object sender, RoutedEventArgs e)
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
            if (lstSalesDetails.Count == 0)
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

                Sale p = new Sale();
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
                else if (lstSalesDetails.Count == 0)
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

                else if (cmbSalesType.Text == "Redeem" && paid > Convert.ToDouble(lblPoint.Content.ToString()))
                {

                    var Information = new SampleMessageDialog
                    {
                        Message = { Text = string.Format("Available Points {0}", lblPoint.Content) }
                    };
                    txtPaidAmount.Focus();
                    await DialogHost.Show(Information, "RootDialog");


                }



                else
                {

                    if (ID == 0)
                    {
                        db.Sales.Add(p);

                    }
                    else
                    {
                        p = db.Sales.Where(x => x.SalesId == ID).FirstOrDefault();
                    }
                    p.DiscountAmount = Convert.ToDouble(txtDiscount.Text.ToString());
                    p.Extra = Convert.ToDouble(txtExtras.Text.ToString());
                    p.InvoiceNo = Convert.ToDecimal(txtInNo.Text.ToString());

                    // p.LedgerCode = Convert.ToDecimal(cmbCustomer.SelectedValue.ToString());
                    p.LedgerCode = db.Customers.Where(x => x.CustomerName == cmbCustomer.Text).Select(x => x.CustomerId).FirstOrDefault();
                    p.Narration = txtNarration.Text;
                    p.SalesDate = dtpS.SelectedDate;
                    p.ItemAmount = Convert.ToDouble(txtTotal.Text.ToString());
                    p.SalesType = cmbSalesType.Text;
                    p.Narration = txtPaidAmount.Text;
                    var sods = p.SalesDetails.ToList();

                    if (cmbSalesType.SelectedIndex == 1)
                    {
                        if (ID == 0)
                        {
                            ReceiptMaster r = new ReceiptMaster();
                            r.ReceiptDate = dtpS.SelectedDate.Value;
                            r.SalesId = Convert.ToDecimal(txtInNo.Text.ToString());
                            r.CustomerId = db.Customers.Where(x => x.CustomerName == cmbCustomer.Text).Select(x => x.CustomerId).FirstOrDefault();
                            r.PurchaseRId = 0;
                            r.SupplierId = 0;
                            r.ReceiptMode = "Cash";
                            r.ReceiptAmount = Convert.ToDecimal(txtPaidAmount.Text);
                            r.Description = "For INV " + txtInNo.Text + "From Sale";
                            db.ReceiptMasters.Add(r);
                            db.SaveChanges();
                        }
                    }
                    foreach (var data in sods)
                    {
                        var sod = lstSalesDetails.Where(x => x.SDId == data.SDId).FirstOrDefault();
                        if (sod == null)
                        {
                            p.SalesDetails.Remove(data);
                        }
                    }


                    db.SaveChanges();

                    foreach (ItemsDetails data in lstSalesDetails)
                    {
                        SalesDetail tbd = new SalesDetail();
                        if (data.SDId == 0)
                        {
                            db.SalesDetails.Add(tbd);
                        }
                        else
                        {
                            tbd = db.SalesDetails.Where(x => x.SDId == data.SDId).FirstOrDefault();
                        }

                        //  tbd.SDId = ID;
                        tbd.SalesId = p.SalesId;
                        tbd.ProductCode = data.ProductCode;
                        tbd.DisPer = data.DisPer == 0 ? data.Rate : data.DisPer;  //SellingRate
                        tbd.Quantity = data.Quantity;
                        tbd.Rate = data.Rate;  //MRP
                        tbd.TaxPer = (((data.Rate == 0 ? data.DisPer : data.Rate) - data.DisPer) * data.Quantity).ToString();  //SaveingAmount
                        tbd.UOM = data.UOM;


                        db.SaveChanges();

                    }
                    // StockDetails.UpdateStockDetails();
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Saved Successfully.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");

                    //if(cmbSalesType.Text=="Credit" || cmbSalesType.Text=="Card")
                    //{
                    //    frmReceipt f = new frmReceipt();
                    //    App.frmHome.ShowForm(f);
                    //    System.Windows.Forms.Application.DoEvents();
                    //    var d =db.Customers.Where(x=>x.CustomerName==cmbCustomer.Text).ToList();
                    //    f.cmbNameDr.ItemsSource = d;
                    //    f.cmbNameDr.DisplayMemberPath = "CustomerName";
                    //    f.cmbNameDr.SelectedValuePath = "CustomerId";
                    //    f.cmbNameDr.SelectedIndex = 0;
                    //        f.setBalance();
                    //    System.Windows.Forms.Application.DoEvents();

                    //}
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
            Reports.Transaction.frmSalesSearch frm = new Reports.Transaction.frmSalesSearch();
            frm.ShowDialog();
            ViewDetails(frm.SID);
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
                        var sal = db.Sales.Where(x => x.SalesId == ID).FirstOrDefault();
                        var rec = db.ReceiptMasters.Where(x => x.SalesId == sal.InvoiceNo).Count();
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
                            db.SalesDetails.RemoveRange(db.SalesDetails.Where(x => x.SalesId == ID));
                            db.SaveChanges();

                            Sale p1 = db.Sales.Where(x => x.SalesId == ID).FirstOrDefault();
                            db.Sales.Remove(p1);
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
        private void btnPoint_Click(object sender, RoutedEventArgs e)
        {
            Reports.Transaction.frmCustomerPoint frm = new Reports.Transaction.frmCustomerPoint();
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
            //else if(txtQty.Text== StockDetails.toList.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault().ClStock.ToString())
            //{
            //    var sampleMessageDialog = new SampleMessageDialog
            //    {
            //        Message = { Text = string.Format("Available Stock {0}", StockDetails.toList.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault().ClStock.ToString()) }
            //    };

            //    await DialogHost.Show(sampleMessageDialog, "RootDialog");
            //    txtQty.Focus();
            //}
            else
            {
               try
                {
                    ItemsDetails tbd = lstSalesDetails.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault();
                 
                    if (tbd == null)
                    {
                        tbd = new ItemsDetails();
                        lstSalesDetails.Add(tbd);                     
                    }
                    Double Amt = 0;
                    Double Total = 0; Double Dis = 0;
                  
                    tbd.ProductCode = Pro.ProductId;
                    //tbd.SDId = Convert.ToInt16(ID);
                    tbd.SNo = lstSalesDetails.Count;

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

                    dgvDetails.ItemsSource = lstSalesDetails;
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
            lstSalesDetails.Remove(tbd);
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
                txtTotItemAmount.Text = string.Format("{0:N2}", lstSalesDetails.Sum(x => x.Amount));
                //txtDiscount.Text = string.Format("{0:N2}", lstSalesDetails.Sum(x => x.DisPer));
                double dis = Convert.ToDouble(txtTotItemAmount.Text.ToString());

                if (dis > c.MinAmount)
                {
                    total = ((dis - ((dis * (double)c.Amount) / 100)) + Convert.ToDouble(txtExtras.Text.ToString()));
                    double r = Math.Round(Convert.ToDouble(string.Format("{0:N2}", total)), MidpointRounding.AwayFromZero);
                    txtDiscount.Text = ((dis * (double)c.Amount) / 100).ToString();
                    txtRound.Text = string.Format("{0:N2}", r);
                    txtTotal.Text = string.Format("{0:N2}", total);
                }
                else
                {
                    txtDiscount.Text = "0";
                    txtTotal.Text = string.Format("{0:N2}", dis);
                    txtRound.Text = Math.Round(dis).ToString();
                }



                int test;
                test = Convert.ToInt32(total);
                lblAmount.Text = "RS " + string.Format("{0:N2}", test.ToString());
                lblAmountInWords.Text = AppLib.NumberToWords(test).ToUpper();
                lblAmountInWords.Text += " ONLY.";
                // PaidAmount();
            }
            catch (Exception ex) { }
        }

        public void LoadWindow()
        {
            // JJSuperMarketEntities db1 = new JJSuperMarketEntities();


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
            var c = lstProduct;
            cmbItem.ItemsSource = c.ToList();

            var inv = (db.Sales.DefaultIfEmpty().Max(p => p == null ? 0 : p.InvoiceNo)) + 1;
            txtInNo.Text = inv.ToString();

            txtID.Text = (db.Sales.DefaultIfEmpty().Max(p => p == null ? 0 : p.SalesId) + 1).ToString();
            dgvDetails.ItemsSource = lstSalesDetails;
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
                Sale p = db.Sales.Where(x => x.SalesId == id).FirstOrDefault();
                ID = p.SalesId;
                txtID.Text = p.SalesId.ToString();
                txtInNo.Text = p.InvoiceNo.ToString();
                dtpS.SelectedDate = p.SalesDate;
                cmbSalesType.Text = p.SalesType;
                txtPaidAmount.Text = p.Narration;
                txtDiscount.Text = p.DiscountAmount.ToString();
                txtExtras.Text = p.Extra.ToString();
                cmbCustomer.Text = p.Customer.CustomerName;

                App.LogWriter("Sales master View");

                lstSalesDetails.Clear();
                int s = 0;
                foreach (var data in p.SalesDetails.ToList())
                {
                    App.LogWriter(p.SalesDetails.Count().ToString());
                    ItemsDetails i = new ItemsDetails();
                    i.DisPer = data.DisPer == 0 ? (double)data.Rate : Convert.ToDouble(data.DisPer);
                    s=s + 1;
                    i.SNo = s;
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
                    i.SDId = Convert.ToInt32(data.SDId);

                    lstSalesDetails.Add(i);
                }
                FindTotalAmount();

            }
            catch (Exception ex)
            {
                App.LogWriter("Sales master View");

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
                    var data = lstSalesDetails.Where(x => x.Itemcode == txtItemCode.Text).FirstOrDefault();
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
                //StockDetails.UpdateStockDetails();
                //var stock = StockDetails.toList.Where(x => x.ProductName == cmbItem.Text).FirstOrDefault();
                //if (Convert.ToDecimal(txtQty.Text) <= stock.ClStock)
                //{
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
                //}
                //else
                //{
                //    var sampleMessageDialog = new SampleMessageDialog
                //    {
                //        Message = { Text = "Available Stock " + stock.ClStock }
                //    };

                //    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                //    txtQty.Focus();
                //}
            }
        }
        private void Showpoints(Customer cust)
        {
            lblPoint.Content = "";
            try
            {

                var Amt1 = (double)cust.Sales.Where(x => x.SalesType != "Redeem").Sum(x => x.ItemAmount) * 0.01;
                var Amt2 = (double)cust.Sales.Where(x => x.SalesType == "Redeem").Sum(x => x.ItemAmount);

                lblPoint.Content = string.Format("{0:N2}", Math.Abs(Amt1 - Amt2));
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
                double qty = 1;
                var data = lstSalesDetails.Where(x => x.Itemcode == txtItemCode.Text).FirstOrDefault();
                if (data != null)
                {
                    qty = data.Quantity + 1;
                }
                txtQty.Text = qty.ToString();
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
            lstSalesDetails.Clear();
            dgvDetails.ItemsSource = lstSalesDetails;
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
            lblPoint.Content = "";
            ID = 0;
            chkNewEntry.IsChecked = false;
            LoadWindow();
            dtpS.SelectedDate = DateTime.Today;
            cmbSalesType.SelectedIndex = 0;
        }
        #endregion
      
        #region class
        partial class ItemsDetails : INotifyPropertyChanged
        {
            private int _SNo;
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
            private ObservableCollection<ItemsDetails> _Items;


            private ObservableCollection<ItemsDetails> items = new ObservableCollection<ItemsDetails>();

            public ObservableCollection<ItemsDetails> Items
            {
                get { return items; }
                set { items = value; NotifyPropertyChanged("Items"); NotifyPropertyChanged("ItemCount"); }
            }
            public int SDId { get { return _SDId; } set { if (_SDId != value) { _SDId = value; NotifyPropertyChanged("SDId"); } } }
            public int SNo { get { return _SNo; } set { if (_SNo != value) { _SNo = value; NotifyPropertyChanged("SNo"); } } }
            public int ItemCount
            {
                get
                {
                    return Items.Count();
                }
            }


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
                        NotifyPropertyChanged("ItemCount");
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

        #region PrintBill
        enum PrintTextAlignType
        {
            Left,
            Center,
            Right
        }
        int PrintNoOfCharPerLine = 40;//27
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
        private async void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cmbMobileNumber.Text.Trim()))


                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Mobile Number.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    cmbMobileNumber.Focus();

                }
                else if (string.IsNullOrEmpty(txtPaidAmount.Text.Trim()))


                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Given Amount.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtPaidAmount.Focus();

                }
                else
                {
                    System.Drawing.Printing.PrintDocument prnPurchaseOrder = new System.Drawing.Printing.PrintDocument();
                    prnPurchaseOrder.PrintPage += PrnPurchaseOrder_PrintPage;
                    var c = db.CompanyDetails.FirstOrDefault();
                    prnPurchaseOrder.DefaultPageSettings.PrinterSettings.PrinterName = c.PhNo == "" ? "Microsoft XPS Document Writer" : c.PhNo;
                    prnPurchaseOrder.PrintController = new System.Drawing.Printing.StandardPrintController();


                    // adds.Split()
                    TextBlock txt = new TextBlock();
                    txt.Text = PrintLine(c.CompanyName, PrintTextAlignType.Center);
                    txt.FontWeight = FontWeights.UltraBold;
                    TextToPrint = PrintLine(c.CompanyName, PrintTextAlignType.Center);
                    //TextToPrint += PrintLine(c.Address1, PrintTextAlignType.Center);
                    // TextToPrint += PrintLine(c.Address2, PrintTextAlignType.Center);
                    //  TextToPrint += PrintLine(c.Address3, PrintTextAlignType.Center);
                    TextToPrint += PrintLine(c.Address4, PrintTextAlignType.Center);
                    //TextToPrint += PrintLine("Ambattur, Kalikuppam", PrintTextAlignType.Center);
                    TextToPrint += PrintLine(c.Pincode, PrintTextAlignType.Center);
                    TextToPrint += PrintLine(string.Format("Mob No:{0},Tin No:{1}", c.MobileNo, c.Tin), PrintTextAlignType.Center);
                    // TextToPrint += PrintLine(string.Format("Tin No:{0}", c.Tin), PrintTextAlignType.Center);

                    TextToPrint += PrintLine(string.Format("{0}", new string('-', PrintNoOfCharPerLine)), PrintTextAlignType.Center);
                    TextToPrint += PrintLine(string.Format("Member Name  :{0},", cmbCustomer.Text), PrintTextAlignType.Left);
                    TextToPrint += PrintLine(string.Format("Member Mob No:{0},", cmbMobileNumber.Text), PrintTextAlignType.Left);
                    TextToPrint += PrintLine(string.Format("Your Points  :{0}.", lblPoint.Content), PrintTextAlignType.Left);

                    TextToPrint += PrintLine(string.Format("{0}CASH BILL{0}", new string('-', 15)), PrintTextAlignType.Center);

                    TextToPrint += PrintLine(string.Format("Date :{0:dd/MM/yyyy , hh:mm tt,}", DateTime.Now), PrintTextAlignType.Left);
                    TextToPrint += PrintLine(string.Format("Bill No:{0}", txtInNo.Text), PrintTextAlignType.Left);


                    TextToPrint += PrintLine(string.Format("{0}", new string('-', PrintNoOfCharPerLine)), PrintTextAlignType.Center);
                    TextToPrint += PrintLine(string.Format("{0,3} {1,14} {2,20}", "SNo", "Particulars", "Amount"), PrintTextAlignType.Left);
                    TextToPrint += PrintLine(string.Format("{0}", new string('-', PrintNoOfCharPerLine)), PrintTextAlignType.Center);

                    int sno = 0;

                    foreach (var data in lstSalesDetails)
                    {
                        TextToPrint += PrintLine(string.Format("{0,3} {1,-10} ", ++sno + ") ", data.ProductName), PrintTextAlignType.Left);
                        // TextToPrint += PrintLine(string.Format("{0}"," MRP:"+data.Rate ), PrintTextAlignType.Left); //Data SubString{data.ProductName.Length>20?data.ProductName.Substring(0,20):}
                        //TextToPrint += PrintLine(string.Format("{0} [Rs. {1} x {2} {3}] {4,8:0.00} ", "", data.Rate, data.Quantity, data.UOMSymbol, data.Rate * data.Quantity), PrintTextAlignType.Left);
                        TextToPrint += PrintLine(string.Format("MRP:{0:N2}{1}[Rate:{2:N2} x {3} {4}] {5,7:0.00}", data.Rate, "", data.DisPer, data.Quantity, data.UOMSymbol, (data.DisPer * data.Quantity)), PrintTextAlignType.Right);
                    }

                    TextToPrint += PrintLine(string.Format("{0}", new string('-', PrintNoOfCharPerLine)), PrintTextAlignType.Center);
                    TextToPrint += PrintLine(string.Format("Total : {0,10:0.00}", Convert.ToDouble(txtTotItemAmount.Text)), PrintTextAlignType.Right);
                    TextToPrint += PrintLine(string.Format("Your Savings : {0,10:0.00}", lstSalesDetails.Sum(x => x.Saveing)), PrintTextAlignType.Right);
                    TextToPrint += PrintLine("", PrintTextAlignType.Left);
                    TextToPrint += PrintLine(string.Format("    Bill Amount :RS.{0,7:0.00}", txtRound.Text == "" ? 0 : Convert.ToDouble(txtRound.Text)), PrintTextAlignType.Right);
                    TextToPrint += PrintLine(string.Format("Received Amount :RS.{0,7:0.00}", txtPaidAmount.Text == "" ? 0 : Convert.ToDouble(txtPaidAmount.Text)), PrintTextAlignType.Right);
                    TextToPrint += PrintLine(string.Format(" Balance Amount :RS.{0,7:0.00}", txtBalAmount.Text == "" ? 0 : Convert.ToDouble(txtBalAmount.Text)), PrintTextAlignType.Right);
                    TextToPrint += PrintLine("", PrintTextAlignType.Left);

                    TextToPrint += PrintLine("*** Thank You! Visit Again ***", PrintTextAlignType.Center);
                    TextToPrint += PrintLine("", PrintTextAlignType.Left);



                    prnPurchaseOrder.Print();
                    btnSave_Click(sender, e);
                }
            }
            catch (Exception ex) { }
        }
        private void PrnPurchaseOrder_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
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

        #region ProductSearch
        private void PackIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            frmProductDetails frm = new Transaction.frmProductDetails();
            frm.ShowDialog();
            ProductName(frm.ProName);

        }
        public void ProductName(string Name)
        {
            txtQty.Focus();
            double qty = 1;
            var data = lstProduct.Where(x => x.ProductName == Name).FirstOrDefault();
            var data1 = lstSalesDetails.Where(x => x.ProductName == Name).FirstOrDefault();
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

        #region AddNewCustomer
        private void chkNewEntry_Checked(object sender, RoutedEventArgs e)
        {
            gbxNewEntry.Visibility = Visibility.Visible;
        }
        private void chkNewEntry_Unchecked(object sender, RoutedEventArgs e)
        {
            gbxNewEntry.Visibility = Visibility.Hidden;
        }

        private void cmbSalesType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbSalesType.SelectedIndex == 1)
            {
                txtPaidAmount.Text = "0.00";
            }

        }

        private void dgvDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            cu = db.Customers.Where(x => x.CustomerName == txtCustomerNameNew.Text).Count();
            var m = db.Customers.Where(x => x.MobileNo == txtMobileNumberNew.Text).Count();
            if (string.IsNullOrWhiteSpace(txtCustomerNameNew.Text))
            {
                if (txtCustomerNameNew.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Please Enter The New CustomerName" }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtCustomerNameNew.Focus();
                }

            }
            else if (string.IsNullOrWhiteSpace(txtMobileNumberNew.Text))
            {

                if (txtMobileNumberNew.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Please Enter The New Customer Mobile Number" }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtMobileNumberNew.Focus();
                }

            }
            else if (cu != 0)
            {
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = txtCustomerNameNew.Text + " is Already Exist..! Enter New One" }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtCustomerNameNew.Focus();
                }
            }
            else if (m != 0)
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = { Text = txtMobileNumberNew.Text + " is Already Exist..! Enter New MobileNumber.." }
                };

                await DialogHost.Show(sampleMessageDialog, "RootDialog");
                txtMobileNumberNew.Focus();
            }
            else
            {
                Customer r = new Customer();
                r.CustomerName = txtCustomerNameNew.Text.ToString();
                r.MobileNo = txtMobileNumberNew.Text.ToString();
                r.LedgerName = "JJSuperCus";
                r.CreditDays = 0;
                r.CreditLimits = 0;
                db.Customers.Add(r);
                db.SaveChanges();

                cmbCustomer.Text = txtCustomerNameNew.Text;
                cmbMobileNumber.Text = txtMobileNumberNew.Text;
                txtMobileNumberNew.Clear();
                txtCustomerNameNew.Clear();
                gbxNewEntry.Visibility = Visibility.Hidden;
                chkNewEntry.IsChecked = false;

            }
        }
        #endregion 
    }

}


namespace JJSuperMarket
{
    public partial class Product
    {
        public override string ToString()
        {
            return this.ProductName;
        }
    }
}