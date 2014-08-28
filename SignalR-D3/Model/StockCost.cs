using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SignalR_D3.Model
{
    public class StockCost
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public double Cost { get; set; }
        public DateTime Time { get; set; }

        public Company Company;
    }
}
