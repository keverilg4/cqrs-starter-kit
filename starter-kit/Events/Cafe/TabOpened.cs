using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Events.Something
{
    /*public class SomethingHappened
    {
        public Guid Id;
        public string What;
    }*/

    public class TabOpened
    {
        public Guid Id;
        public int TableNumber;
        public string Waiter;
    }

    public class OrderedItem
    {
        public int MenuNumber;
        public string Description;
        public bool IsDrink;
        public decimal Price;
    }

    public class DrinksOrdered
    {
        public Guid Id;
        public List<OrderedItem> Items;

    }

    public class FoodOrdered
    {
        public Guid Id;
        public List<OrderedItem> Items;
    }

    public class DrinksServed
    {
        public Guid Id;
        public List<int> MenuNumbers;
    }

    public class MarkDrinksServed
    {
        public Guid Id;
        public List<int> MenuNumbers;
    }

    public class TabClosed
    {
        public Guid Id;
        public decimal AmountPaid;
        public decimal OrderValue;
        public decimal TipValue;
    }
}
