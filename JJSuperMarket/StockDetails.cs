using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJSuperMarket
{
    public class StockDetails
    {
        // private static JJSuperMarketEntities db = new JJSuperMarketEntities(); //Update data on close and open a form
        public static ObservableCollection<StockDetails> toList = new ObservableCollection<StockDetails>();

        public string ProductName { get; set; }
        public decimal OpQty { get; set; }
        public decimal ReOrderLevel { get; set; }
        public decimal Inwards { get; set; }
        public decimal Outwards { get; set; }
        public decimal ClStock { get; set; }

        static StockDetails()
        {
            toList = GetStockDetails();
        }
        public static void UpdateStockDetails()
        {
            toList = GetStockDetails();
        }

        public static ObservableCollection<StockDetails> GetStockDetails(String ProductName = null, DateTime? dtFrom = null, DateTime? dtTo = null, decimal Group = 0)
        {
            //db = new JJSuperMarketEntities(); // Update data automatically
            JJSuperMarketEntities db = new JJSuperMarketEntities();

            var lstproduct = db.Products.ToList();
            if (!string.IsNullOrEmpty(ProductName))
            {
                lstproduct = lstproduct.Where(x => x.ProductName.ToLower().Contains(ProductName.ToLower())).ToList();
            }
            if (Group != null)
            {
                if (Group != 0)

                    lstproduct = lstproduct.Where(x => x.GroupCode == Group).ToList();
            }
            if (dtFrom == null)
            {

                dtFrom = DateTime.Today.Date;
            }
            if (dtTo == null)
            {
                dtTo = DateTime.Today.AddDays(+1);
            }

            ObservableCollection<StockDetails> list = new ObservableCollection<StockDetails>();
            foreach (var data in lstproduct)
            {
                StockDetails s = new StockDetails
                {
                    ProductName = data.ProductName,
                    OpQty = (
                                        (
                                            Convert.ToDecimal(data.OpQty) +
                                            Convert.ToDecimal(data.PurchaseDetails.Where(x => x.Purchase == null ? false : x.Purchase.PurchaseDate.Value.Date < dtFrom.Value.Date).Sum(x => x.Quantity)) +
                                            Convert.ToDecimal(data.SalesReturnDetails.Where(x => x.SalesReturn == null ? false : x.SalesReturn.SRDate.Value.Date < dtFrom.Value.Date).Sum(x => x.Quantity))
                                        ) -
                                        (
                                            Convert.ToDecimal(data.SalesDetails.Where(x => x.Sale == null ? false : x.Sale.SalesDate.Value.Date < dtFrom.Value.Date).Sum(x => x.Quantity)) +
                                            Convert.ToDecimal(data.PurchaseReturnDetails.Where(x => x.PurchaseReturn == null ? false : x.PurchaseReturn.PRDate.Value.Date < dtFrom.Value.Date).Sum(x => x.Quantity))
                                        )
                                    ),
                    Inwards = (
                                        Convert.ToDecimal(data.PurchaseDetails.Where(x => x.Purchase == null ? false : x.Purchase.PurchaseDate.Value.Date >= dtFrom.Value.Date && x.Purchase == null ? false : x.Purchase.PurchaseDate.Value.Date <= dtTo.Value.Date).Sum(x => x.Quantity)) +
                                        Convert.ToDecimal(data.SalesReturnDetails.Where(x => x.SalesReturn == null ? false : x.SalesReturn.SRDate.Value.Date >= dtFrom.Value.Date && x.SalesReturn == null ? false : x.SalesReturn.SRDate.Value.Date <= dtTo.Value.Date).Sum(x => x.Quantity))
                                    ),
                    Outwards = (
                                        Convert.ToDecimal(data.SalesDetails.Where(x => x.Sale == null ? false : x.Sale.SalesDate.Value.Date >= dtFrom.Value.Date && x.Sale == null ? false : x.Sale.SalesDate.Value.Date <= dtTo.Value.Date).Sum(x => x.Quantity)) +
                                        Convert.ToDecimal(data.PurchaseReturnDetails.Where(x => x.PurchaseReturn == null ? false : x.PurchaseReturn.PRDate.Value.Date >= dtFrom.Value.Date && x.PurchaseReturn == null ? false : x.PurchaseReturn.PRDate.Value.Date <= dtTo.Value.Date).Sum(x => x.Quantity))
                                    ),
                    ReOrderLevel = Convert.ToDecimal(data.ReOrderLevel)
                };
                s.ClStock = s.OpQty + s.Inwards - s.Outwards;
                list.Add(s);
            }
            return list;
        }

        
    }
}
