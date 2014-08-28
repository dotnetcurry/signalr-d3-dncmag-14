using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalR_D3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SignalR_D3.SignalR
{
    public class StockMarket
    {
        //Singleton instance created lazily
        public static readonly Lazy<StockMarket> market = new Lazy<StockMarket>(() => new StockMarket());

        //Flag to be used as a toggler in calculations
        public static bool toggler = true;

        //Flag holding current state of the market
        //public static bool marketStarted = false;

        //A dictionary holding list of last inserted stock prices, captured to ease calculation
        public static Dictionary<int, double> lastStockPrices;

        //To be used to prevent multiple insert calls to DB at the same time
        private readonly object stockInsertingLock = new Object();

        //Repositories and data
        CompanyRepository companyRepository;
        StockCostRepository stockCostRepository;
        List<Company> companies;

        //Timer to be used to run market operations automatically
        Timer _timer;

        public StockMarket()
        {
            companyRepository = new CompanyRepository();
            stockCostRepository = new StockCostRepository();
            companies = companyRepository.GetCompanies();
        }

        static StockMarket()
        {
            lastStockPrices = new Dictionary<int, double>();
        }

        public static StockMarket Instance
        {
            get
            {
                return market.Value;
            }
        }

        public void GenerateNextStockValue(object state)
        {
            lock (stockInsertingLock)
            {
                Random randomGenerator = new Random();
                int changeInCost = randomGenerator.Next(100, 110);
                double lastStockCost, newStockCost;
                List<StockCost> stockCosts = new List<StockCost>();

                foreach (var company in companies)
                {
                    if (!lastStockPrices.TryGetValue(company.CompanyId, out lastStockCost))
                    {
                        lastStockCost = stockCostRepository.GetLastStockValue(company.CompanyId);
                    }

                    if (toggler)
                    {
                        newStockCost = lastStockCost + randomGenerator.NextDouble();
                    }
                    else
                    {
                        newStockCost = lastStockCost - randomGenerator.NextDouble();
                    }

                    var newStockAdded = stockCostRepository.AddStockCost(new StockCost() { Cost = newStockCost, Time = DateTime.Now, CompanyId = company.CompanyId });
                    stockCosts.Add(newStockAdded);
                    lastStockPrices[company.CompanyId] = newStockCost;
                }
                toggler = !toggler;
                GetClients().All.updateNewStockCosts(stockCosts);
            }
        }

        public IHubConnectionContext<dynamic> GetClients()
        {
            return GlobalHost.ConnectionManager.GetHubContext<StockCostsHub>().Clients;
        }

        public IEnumerable<StockCost> PublishInitialStockPrices(int companyId)
        {
            //Get the last 20 costs of the requested company using the repository
            var recentStockCosts = stockCostRepository.GetRecentCosts(companyId);

            //If timer is null, the market is not started
            //Following condition is true only for the first time when the application runs
            if (_timer == null)
            {
                _timer = new Timer(GenerateNextStockValue, null, 10000, 10000);
            }

            return recentStockCosts;
        }
    }
}