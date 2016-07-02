using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TokenCalc.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TokenCalc.Controllers
{
    [Route("api/[controller]")]
    public class CalculatorController : Controller
    {

        // GET api/calculator
        [HttpGet]
        public string Get()
        {
            return "POST /api/token \nGET /api/calculator/add/4/8 \nGET /api/calculator/mult/3/5";
        }

        // GET api/calculator/add/5/2
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
