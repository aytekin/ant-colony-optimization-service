using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aco.Business.Abstract;
using Aco.Entity;
using Aco.Entity.Dto;
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
        public IActionResult Calculate(AcoInput acoInput)
        {
            var result = acoService.Calculate(cities: acoInput.cities, acoOptions: acoInput.acoOptions);

            return Ok(result);
        }
    }
}