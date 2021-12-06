using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Events.Something;

namespace YourDomain.Something
{
    /*public class MakeSomethingHappen
    {
        public Guid Id;
        public string What;
    }*/

    public class OpenTab
    {
        public Guid Id;
        public int TableNumber;
        public string Waiter;
    }
    public class PlaceOrder
    {
        public Guid Id;
        public List<OrderedItem> Items;
    }

    public class CloseTab
    {
        public Guid Id;
        public decimal AmountPaid;
    }

}
