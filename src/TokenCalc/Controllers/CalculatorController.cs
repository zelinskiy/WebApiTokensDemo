using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TokenCalc.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Text;

namespace TokenCalc.Controllers
{
    [Route("api/[controller]")]
    public class CalculatorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        RoleManager<IdentityRole> _roleManager;


        public CalculatorController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<CalculatorController>();
            _roleManager = (RoleManager<IdentityRole>)serviceProvider.GetService(typeof(ApplicationRoleManager));
        }


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


        // GET api/calculator/authenticated
        [HttpGet("authenticated")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public string MyName()
        {
            return User.Identity.IsAuthenticated.ToString();
        }

        // GET api/calculator/authenticated
        [HttpGet("claims")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public string MyClaims()
        {
            string answer = "";
            foreach(var c in User.Claims)
            {
                answer += $"Type:{c.Type}, Value:{c.Value} \n";
            }
            return answer;
        }

        // GET api/calculator/allusers
        [HttpGet("allusers")]
        public string AllUsers()
        {
            string answer = "";
            foreach (var u in _userManager.Users)
            {
                answer += $"User Name:{u.UserName}, RolesNum:{u.Roles.Count} \n";
            }
            return answer;
        }


        // GET api/calculator/register/{login}/{password}
        [HttpGet("register/{login}/{password}")]        
        public async Task<string> Register(string login, string password)
        {
            var user = new ApplicationUser { UserName = login, Email = login };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return $"User {login} succesfully registered!";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach(var e in result.Errors)
                {
                    sb.AppendLine(e.Code);
                    sb.AppendLine(e.Description);
                    sb.AppendLine("-------------");
                }
                return sb.ToString();
            }            
        }

        


    }
}
