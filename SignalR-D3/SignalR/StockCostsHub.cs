using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalR_D3.SignalR
{
    public class StockCostsHub : Hub
    {
        public void GetInitialStockPrices(int companyId)
        {
            Clients.Caller.initiateChart(StockMarket.Instance.PublishInitialStockPrices(companyId));
        }
    }
}