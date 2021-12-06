using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using YourDomain.Something;
using Edument.CQRS;
using Events.Something;

namespace CafeTests
{
    /*
    [TestFixture]
    public class SomethingTests : BDDTest<SomethingAggregate>
    {
        private Guid testId2;

        [SetUp]
        public void Setup()
        {
            testId2 = Guid.NewGuid();
        }

        [Test]
        public void SomethingCanHappen()
        {
            Test(
                Given(),
                When(new MakeSomethingHappen
                {
                    Id = testId2,
                    What = "Boom!"
                }),
                Then(new SomethingHappened
                {
                    Id = testId2,
                    What = "Boom!"
                }));
        }

        [Test]
        public void SomethingCanHappenOnlyOnce()
        {
            Test(
                Given(new SomethingHappened
                {
                    Id = testId2,
                    What = "Boom!"
                }),
                When(new MakeSomethingHappen
                {
                    Id = testId2,
                    What = "Boom!"
                }),
                ThenFailWith<SomethingCanOnlyHappenOnce>());
        }
    }*/

    [TestFixture]
    public class TabTests : BDDTest<TabAggregate>
    {
        private Guid testId;
        private int testTable;
        private string testWaiter;
        private OrderedItem testDrink1 = new OrderedItem();
        private OrderedItem testDrink2 = new OrderedItem();
        private OrderedItem testFood1 = new OrderedItem();
        private OrderedItem testFood2 = new OrderedItem();

        [SetUp]
        public void Setup()
        {
            testId = Guid.NewGuid();
            testTable = 42;
            testWaiter = "Derek";
            testDrink1.Description = "d1";
            testDrink1.MenuNumber = 1;
            testDrink1.IsDrink = true;
            testDrink1.Price = 1;
            testDrink2.Description = "d2";
            testDrink2.MenuNumber = 2;
            testDrink2.IsDrink = true;
            testDrink2.Price = 2;
            testFood1.Description = "f1";
            testFood1.MenuNumber = 3;
            testFood1.IsDrink = false;
            testFood1.Price = 3;
            testFood2.Description = "f2";
            testFood2.MenuNumber = 4;
            testFood2.IsDrink = false;
            testFood2.Price = 4;
        }

        [Test]
        public void CanOpenANewTab()
        {
            Test(
                Given(),
                When(new OpenTab
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                }),
                Then(new TabOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter

                }));
        }

        [Test]
        public void CanNotOrderWithUnopenedTab()
        {
            Test(
                Given(),
                When(new PlaceOrder
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1 }
                }),
                ThenFailWith<TabNotOpen>());

        }

        [Test]
        public void CanPlaceDrinksOrder()
        {
            Test(
                Given(new TabOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                }),
                When(new PlaceOrder
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1, testDrink2 }
                }),
                Then(new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1, testDrink2 }
                }));
        }

        [Test]
        public void CanPlaceFoodOrder()
        {
            Test(
                Given(new TabOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                }),
                When(new PlaceOrder
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1, testFood2 }
                }),
                Then(new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1, testFood2 }
                }));
        }

        [Test]
        public void CanPlaceFoodAndDrinkOrder()
        {
            Test(
                Given(new TabOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                }),
                When(new PlaceOrder
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1, testDrink2 }
                }),
                Then(new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink2 }
                },
                new FoodOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testFood1 }
                }));
        }

        [Test]
        public void OrderedDrinksCanBeServed()
        {

            Test(
                Given(new TabOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1, testDrink2 }
                }),
                When(new MarkDrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> 
                        { testDrink1.MenuNumber, testDrink2.MenuNumber }
                }),
                Then(new DrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> 
                        { testDrink1.MenuNumber, testDrink2.MenuNumber }
                }));
        }

        [Test]
        public void CanNotServeAnUnorderedDrink()
        {
            Test(
                Given(new TabOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new DrinksOrdered {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1 }
                }),
                When(new MarkDrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testDrink2.MenuNumber }
                }),
                ThenFailWith<DrinksNotOutstanding>());
        }

        [Test]
        public void CanNotServeAnOrderedDrinkTwice()
        {
            Test(
                Given(new TabOpened
                {
                    Id = testId,
                    TableNumber = testTable,
                    Waiter = testWaiter
                },
                new DrinksOrdered
                {
                    Id = testId,
                    Items = new List<OrderedItem> { testDrink1 }
                },
                new DrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testDrink1.MenuNumber }
                }),
                When(new MarkDrinksServed
                {
                    Id = testId,
                    MenuNumbers = new List<int> { testDrink1.MenuNumber }
                }),
                ThenFailWith<DrinksNotOutstanding>());
        }

        [Test]
        public void CanCloseTabWithTip()
        {
            Test(Given(new TabOpened
            {
                Id = testId,
                TableNumber = testTable,
                Waiter = testWaiter
            },
            new DrinksOrdered
            {
                Id = testId,
                Items = new List<OrderedItem>() { testDrink2 }
            },
            new DrinksServed
            { 
                Id = testId,
                MenuNumbers = new List<int> { testDrink2.MenuNumber }
            }),
            When(new CloseTab
            {
                Id = testId,
                AmountPaid = testDrink2.Price + 0.50M
            }),
            Then(new TabClosed 
            { 
                Id = testId,
                AmountPaid = testDrink2.Price + 0.50M,
                OrderValue = testDrink2.Price,
                TipValue = 0.50M
            }));
        }
    }
}
