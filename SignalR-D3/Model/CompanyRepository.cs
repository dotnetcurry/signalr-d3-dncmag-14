using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR_D3.Model
{
    public class CompanyRepository
    {
        StocksContext context;

        public CompanyRepository()
        {
            context = new StocksContext();
        }

        public List<Company> GetCompanies()
        {
            return context.Companies.ToList();
        }
    }
}