using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors.Infrastructure;
using TokenCalc.Data;
using Microsoft.EntityFrameworkCore;
using TokenCalc.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TokenCalc
{
    public class Startup
    {

        //Our secred key which MUST be stored separately
        private static readonly string secretKey = "veryverysecretkey";

        private static readonly string myIssuer = "ExampleIssuer";
        private static readonly string myAudience = "ExampleAudience";
        private static readonly TimeSpan expirationTime = TimeSpan.FromSeconds(60);



        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            { 
                builder.AddUserSecrets();
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoleManager<ApplicationRoleManager>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var policy = new CorsPolicy();
            policy.Headers.Add("*");
            policy.Methods.Add("*");
            policy.Origins.Add("*");
            policy.SupportsCredentials = true;

            services.AddCors(options => options.AddPolicy("MyPolicy", policy));

            services.AddMvc();
        }        



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            var logger = loggerFactory.CreateLogger("Tokens");

            app.UseStaticFiles();
            app.UseIdentity();

            
            //Setting up our "token dispenser"
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var options = new TokenProviderOptions
            {
                Audience = myAudience,
                Issuer = myIssuer,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                Expiration = expirationTime,
                Path = "/api/token"
            };
            
            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));

            //Setting our tokens validator
            var tokenparams = new TokenValidationParameters()
            {
                IssuerSigningKey = signingKey,
                ValidIssuer = myIssuer,
                ValidateLifetime = true,
                SaveSigninToken = false,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero,
                LifetimeValidator = ((DateTime? notBefore,
                    DateTime? expires,
                    SecurityToken securityToken,
                    TokenValidationParameters validationParameters) =>
                !(expires.Value < DateTime.UtcNow)),                
            };        


            var opts = new JwtBearerOptions()
            {
                TokenValidationParameters = tokenparams,
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                Audience = myAudience,                 
                
                Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        logger.LogError("Authentication failed.", context.Exception);
                        return Task.FromResult(0);
                    },
                    OnTokenValidated = context =>
                    {

                        if(context.SecurityToken.ValidTo < DateTime.UtcNow)
                        {
                            return Task.FromResult(0);                            
                        }
                        var claimsIdentity = context.Ticket.Principal.Identity as ClaimsIdentity;
                        claimsIdentity.AddClaim(new Claim("id_token",
                            context.Request.Headers["Authorization"][0].Substring(context.Ticket.AuthenticationScheme.Length + 1)));

                        // OPTIONAL: you can read/modify the claims that are populated based on the JWT
                        // claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, claimsIdentity.FindFirst("name").Value));
                        return Task.FromResult(0);
                    }
                }
            };
            
            app.UseJwtBearerAuthentication(opts);

            app.UseCors("MyPolicy");

            app.UseMvc();
            
        }
    }
}
