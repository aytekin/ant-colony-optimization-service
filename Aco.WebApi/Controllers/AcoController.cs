using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aco.Business.Abstract;
using Aco.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aco.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcoController : ControllerBase
    {
        private IAcoService acoService;

        public AcoController(IAcoService acoService)
        {
            this.acoService = acoService;
        }


        [HttpPost("calculate")]
        public IActionResult Calculate( List<City> cities)
        {
            var result = acoService.Calculate(cities: cities);

            return Ok(result);
        }
    }
}