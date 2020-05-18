using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aco.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public Dictionary<string, string> Test()
        {
            return new Dictionary<string, string>()
            {
                { "1","selamun" },
                { "2","aleykum" },
                { "3","dunya" }
            };
        }
    }
}