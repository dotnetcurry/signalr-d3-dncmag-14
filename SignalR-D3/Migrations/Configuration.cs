namespace SignalR_D3.Migrations
{
    using SignalR_D3.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<StocksContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(StocksContext context)
        {
            if (!context.StockCosts.Any())
            {
                context.Companies.AddRange(new List<Company>() { 
                    new Company(){CompanyName="Microsoft"},
                    new Company(){CompanyName="Google"},
                    new Company(){CompanyName="Apple"},
                    new Company(){CompanyName="IBM"},
                    new Company(){CompanyName="Samsung"}
                });
                var randomGenerator = new Random();

                for (int companyId = 1; companyId <= 5; companyId++)
                {
                    double stockCost = 100 * companyId;

                    for (int count = 0; count < 10; count++)
                    {
                        context.StockCosts.Add(new StockCost() 
                        { 
                            CompanyId = companyId, 
                            Cost = stockCost, 
                            Time = DateTime.Now - new TimeSpan(0, count, 0) 
                        });

                        if (count % 2 == 0)
                        {
                            stockCost = stockCost + randomGenerator.NextDouble();
                        }
                        else
                        {
                            stockCost = stockCost - randomGenerator.NextDouble();
                        }
                    }
                }
            }
        }
    }
}
