using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using TokenCalc.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text;


namespace TokenCalc.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<CalculatorController>();
            _roleManager = roleManager;
            //_roleManager = (RoleManager<IdentityRole>)serviceProvider.GetService(typeof(ApplicationRoleManager));
        }



        // GET: api/account
        [HttpGet]
        public string Get()
        {
            return "POST /api/token \n"+
                "GET api/account/register/{login}/{password}\n" +
                "GET /api/account/authenticated\n" +
                "GET /api/account/claims\n"+
                "GET /api/account/allusers\n";
        }

        // GET api/account/authenticated
        [HttpGet("authenticated")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public string IsAuthenticated()
        {
            return User.Identity.IsAuthenticated.ToString();
        }

        // GET api/account/claims
        [HttpGet("claims")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public string MyClaims()
        {
            string answer = "";
            foreach (var c in User.Claims)
            {
                answer += $"Type:{c.Type}, Value:{c.Value} \n";
            }
            return answer;
        }

        // GET api/account/allusers
        [HttpGet("allusers")]
        public string AllUsers()
        {
            string answer = "";
            foreach (var u in _userManager.Users)
            {
                answer += $"User Name:{u.UserName} Email: {u.Email}\n";
            }
            return answer;
        }

        // GET api/account/register/{login}/{password}
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
                foreach (var e in result.Errors)
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
