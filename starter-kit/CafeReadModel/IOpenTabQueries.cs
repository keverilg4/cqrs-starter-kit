using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeReadModel
{
    public interface IOpenTabQueries
    {
        List<int> ActiveTableNumbers();
        OpenTabs.TabInvoice InvoiceForTable(int table);
        OpenTabs.TabStatus TabForTable(int table);
        Dictionary<int, List<OpenTabs.TabItem>> TodoListForWaiter(string waiter);
    }
}
