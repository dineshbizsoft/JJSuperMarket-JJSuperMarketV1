using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJSuperMarket
{
   public class Points
    {
        class CustomerPoints
        {
            public DateTime Date { get; set; }
            public string CustomerName { get; set; }
            public decimal ItemAmount { get; set; }
            public decimal Points { get; set; }
        }
        public  void points(string Name)
        {
            JJSuperMarketEntities db = new JJSuperMarketEntities();
            List<CustomerPoints> CusPoint = new List<CustomerPoints>();
             
            foreach (var Cus in Name == "" ? db.Customers.ToList() : db.Customers.Where(x => x.CustomerName == Name).ToList())
            {



                CustomerPoints c1 = new CustomerPoints();
                // c1.Date =  Convert.ToDateTime( Cus.Sales.Select(x=>x.SalesDate.Value));
                c1.CustomerName = Cus.CustomerName;
                c1.ItemAmount = Convert.ToDecimal(string.Format("{0:N2}", Cus.Sales.Sum(x => x.ItemAmount)));
                c1.Points = Convert.ToDecimal(string.Format("{0:N2}", Cus.Sales.Sum(x => x.ItemAmount) * 0.01));
                CusPoint.Add(c1);


            }
           
          
        }
    }
}
