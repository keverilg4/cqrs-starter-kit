using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edument.CQRS;
using Events.Something;
using System.Collections;

namespace YourDomain.Something
{
    /*public class SomethingAggregate : Aggregate,
        IHandleCommand<MakeSomethingHappen>,
        IApplyEvent<SomethingHappened>
    {
        private bool alreadyHappened;

        public IEnumerable Handle(MakeSomethingHappen c)
        {
            if (alreadyHappened)
                throw new SomethingCanOnlyHappenOnce();

            yield return new SomethingHappened
            {
                Id = c.Id,
                What = c.What
            };
        }

        public void Apply(SomethingHappened e)
        {
            alreadyHappened = true;
        }
    }*/

    public class TabAggregate : Aggregate,
        IHandleCommand<OpenTab>,
        IHandleCommand<PlaceOrder>,
        IApplyEvent<TabOpened>,
        IHandleCommand<MarkDrinksServed>,
        IApplyEvent<DrinksOrdered>,
        IApplyEvent<DrinksServed>,
        IHandleCommand<CloseTab>
    {
        private bool open = false;
        private List<OrderedItem> outstandingDrinks = new List<OrderedItem>();
        private List<OrderedItem> outstandingFood = new List<OrderedItem>();
        private List<OrderedItem> preparedFood = new List<OrderedItem>();
        private decimal servedItemsValue = 0M;

        public IEnumerable Handle(OpenTab c)
        {
            yield return new TabOpened
            {
                Id = c.Id,
                TableNumber = c.TableNumber,
                Waiter = c.Waiter
            };
        }

        public IEnumerable Handle(PlaceOrder c)
        {
            if (!open)
                throw new TabNotOpen();

            var drink = c.Items.Where(i => i.IsDrink).ToList();
            if (drink.Any())
                yield return new DrinksOrdered
                {
                    Id = c.Id,
                    Items = drink
                };

            var food = c.Items.Where(i => !i.IsDrink).ToList();
            if (food.Any())
                yield return new FoodOrdered
                {
                    Id = c.Id,
                    Items = food
                };
        }

        public IEnumerable Handle (MarkDrinksServed c)
        {
            if (!AreDrinksOutstanding(c.MenuNumbers))
                throw new DrinksNotOutstanding();

            yield return new DrinksServed
            {
                Id = c.Id,
                MenuNumbers = c.MenuNumbers
            };
        }

        public void Apply(TabOpened e)
        {
            open = true;
        }

        public void Apply(DrinksOrdered e)
        {
            outstandingDrinks.AddRange(e.Items);
        }

        private bool AreDrinksOutstanding(List<int> menuNumbers)
        {
            if (outstandingDrinks.Count == 0)
                return false;

            var curOutstanding = new List<OrderedItem>(outstandingDrinks);
            foreach (var num in menuNumbers)
            {
                foreach(var item in curOutstanding)
                {
                    if (item.MenuNumber == num)
                    {
                        curOutstanding.Remove(item);
                        break;
                    }
                    else
                        return false;
                }

            }

            return true;
        }

        public void Apply(DrinksServed e)
        {
            foreach (var num in e.MenuNumbers)
            {
                var item = outstandingDrinks.First(d => d.MenuNumber == num);
                outstandingDrinks.Remove(item);
                servedItemsValue += item.Price;
            }
        }

        public IEnumerable Handle(CloseTab c)
        {
            yield return new TabClosed
            {
                Id = c.Id,
                AmountPaid = c.AmountPaid,
                OrderValue = servedItemsValue,
                TipValue = c.AmountPaid - servedItemsValue
            };

        }
    }
}
