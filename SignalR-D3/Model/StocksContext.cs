using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SignalR_D3.Model
{
    public class StocksContext: DbContext
    {
        public StocksContext()
            : base("stocksConn")
        {

        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<StockCost> StockCosts { get; set; }
    }
}