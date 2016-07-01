using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenCalc.Data;

namespace TokenCalc
{
    public class TokenProviderOptions
    {
        public string Path { get; set; } = "/token";

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public TimeSpan Expiration { get; set; } = TimeSpan.FromSeconds(10);

        public SigningCredentials SigningCredentials { get; set; }

        public DbContextOptions<ApplicationDbContext> ApplicationDbContextOptions { get; set; }

    }

   

}
