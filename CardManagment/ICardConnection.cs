using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardManagment
{
    public interface ICardConnection
    {
        int ExecuteQuery(string query, string? connectionString = null);
        Card SelectQuery(string query, string? connectionString = null);

    }
}
