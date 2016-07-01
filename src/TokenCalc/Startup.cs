using System;
using System.Collections.Generic;
using System.Linq;
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

        private static DbContextOptions<ApplicationDbContext> applicationDbContextOptions;
        

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                applicationDbContextOptions = (DbContextOptions<ApplicationDbContext>)options.Options;
            });

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
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                ApplicationDbContextOptions = applicationDbContextOptions,
            };

            
            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));


            //Setting our tokens validator
            var tokenparams = new TokenValidationParameters();
            tokenparams.IssuerSigningKey = signingKey;
            tokenparams.ValidIssuer = "ExampleIssuer";
            tokenparams.ValidateLifetime = true;
            tokenparams.SaveSigninToken = false;
            tokenparams.RequireExpirationTime = true;
            tokenparams.ClockSkew = TimeSpan.Zero;
            tokenparams.LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
            {
                if (expires.Value < DateTime.UtcNow)
                {
                    return false;
                }
                return true;
            };


            var opts = new JwtBearerOptions()
            {
                TokenValidationParameters = tokenparams,
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                Audience = "ExampleAudience",
                
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
                        //not identified kind of magic goes here
                        var claimsIdentity = context.Ticket.Principal.Identity as ClaimsIdentity;
                        claimsIdentity.AddClaim(new Claim("id_token",
                            context.Request.Headers["Authorization"][0].Substring(context.Ticket.AuthenticationScheme.Length + 1)));

                        // OPTIONAL: you can read/modify the claims that are populated based on the JWT
                        // claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, claimsIdentity.FindFirst("name").Value));
                        return Task.FromResult(0);
                    }
                }
            };

            //Adding a jwt service so we can use it like 
            //[Authorize(ActiveAuthenticationSchemes = "Bearer")]
            app.UseJwtBearerAuthentication(opts);

            app.UseCors("MyPolicy");

            app.UseMvc();
            
        }
    }
}
