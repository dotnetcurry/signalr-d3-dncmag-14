using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR_D3.Model
{
    public class StockCostRepository
    {
        StocksContext context;

        public StockCostRepository()
        {
            context = new StocksContext();
        }

        public List<StockCost> GetRecentCosts(int companyId)
        {
            var count = context.StockCosts.Count();

            if (count > 20)
            {
                var stockCostList = context.StockCosts.Where(sc => sc.CompanyId == companyId)
                    .OrderByDescending(sc=>sc.Id)
                    .Take(20)
                    .ToList();
                stockCostList.Reverse();
                return stockCostList;
            }
            else
            {
                return context.StockCosts.Where(sc => sc.CompanyId == companyId)
                    .ToList();
            }
        }

        public double GetLastStockValue(int companyId)
        {
            return context.StockCosts.Where(sc => sc.CompanyId == companyId)
                    .OrderByDescending(sc => sc.Id)
                    .First()
                    .Cost;
        }

        public StockCost AddStockCost(StockCost newStockCost)
        {
            var addedStockCost = context.StockCosts.Add(newStockCost);
            context.SaveChanges();
            return addedStockCost;
        }

    }
}