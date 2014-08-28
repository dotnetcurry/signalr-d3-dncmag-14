using SignalR_D3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SignalR_D3.WebAPI
{
    [Route("api/companies")]
    public class CompaniesController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get()
        {
            try
            {
                return Ok(new CompanyRepository().GetCompanies());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}