using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TokenCalc.Controllers
{
    [Route("api/[controller]")]
    public class CalculatorController : Controller
    {
        // GET api/calculator
        [HttpGet]
        public string Get()
        {
            return "POST /token \nGET /calculator/add/4/8 \nGET /calculator/mult/3/5";
        }

        // GET api/calculator/increment/5/2
        [HttpGet("add/{a}/{b}")]
        public string Get(int a, int b)
        {
            return (a+b).ToString();
        }

        // GET api/calculator/mult/5/2
        [HttpGet("mult/{a}/{b}")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public string Multiply(int a, int b)
        {
            return (a*b).ToString();
        }

    }
}
