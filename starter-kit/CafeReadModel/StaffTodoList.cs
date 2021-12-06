using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Something;
using Edument.CQRS;

namespace CafeReadModel
{
    public class StaffTodoList :
           ISubscribeTo<FoodOrdered>,
           ISubscribeTo<FoodPrepared>
    {
        private Dictionary<Guid, TableToDo> todoByTab = new Dictionary<Guid, TableToDo>();

        public class ItemToDo
        {
            public int MenuNumber;
            public string Description;
        }

        public class TableToDo
        {
            public int TableNumber;
            public string Waiter;
            public List<ItemToDo> Toserve;
            public List<ItemToDo> InPreparation;
        }

        public Dictionary<int, List<ItemToDo>> TodoListForWaiter(string waiter)
        {
            lock (todoByTab)
                return (from tab in todoByTab
                        where tab.Value.Waiter == waiter
                        select new
                        {
                            TableNumber = tab.Value.TableNumber,
                            ToServe = CopyItems(tab.Value)
                        })
                        .Where(t => t.ToServe.Count > 0)
                        .ToDictionary(k => k.TableNumber, v => v.ToServe);
        }

        public List<TodoListGroup> GetTodoList()
        {
            lock (todoList)
                return (from grp in todoList
                        select new TodoListGroup
                        {
                            Tab = grp.Tab,
                            Items = new List<TodoListItem>(grp.Items)
                        }).ToList();
        }

        public void Handle(FoodOrdered e)
        {
            var group = new TodoListGroup
            {
                Tab = e.Id,
                Items = new List<TodoListItem>(
                    e.Items.Select(i => new TodoListItem
                    {
                        MenuNumber = i.MenuNumber,
                        Description = i.Description
                    }))
            };

            lock (todoList)
                todoList.Add(group);
        }

        public void Handle(FoodPrepared e)
        {
            lock (todoList)
            {
                var group = todoList.First(g => g.Tab == e.Id);

                foreach (var num in e.MenuNumbers)
                    group.Items.Remove(
                        group.Items.First(i => i.MenuNumber == num));

                if (group.Items.Count == 0)
                    todoList.Remove(group);
            }
        }
    }
}
