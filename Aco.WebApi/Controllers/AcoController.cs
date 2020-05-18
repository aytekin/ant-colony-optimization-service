using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aco.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aco.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcoController : ControllerBase
    {



        [HttpPost]
        public IActionResult Calculate(List<City> cities)
        {

        }
    }
}