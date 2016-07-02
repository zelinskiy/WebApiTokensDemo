using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TokenCalc.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TokenCalc
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;

        public TokenProviderMiddleware(
            RequestDelegate next,
            IOptions<TokenProviderOptions> options)
        {
            _next = next;
            _options = options.Value;
        }
        


        public Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return _next(context);
            }

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST")
               || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request *777*");
            }

            return GenerateToken(context);
        }



        /// <summary>
        /// Generates new token from username=user&password=pass in request body
        /// </summary>
        /// <param name="context">Http request context</param>
        /// <returns>JSON with token and its expiration</returns>
        private async Task GenerateToken(HttpContext context)
        {

            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];

            //TODO:proper dependency injection
            var signInManager = (SignInManager<ApplicationUser>)context.RequestServices.GetService(typeof(SignInManager<ApplicationUser>));
            var userManager = (UserManager<ApplicationUser>)context.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
            
            var identity = await GetIdentity(username, password, signInManager);
            var myclaims = new List<Claim>();
            if (identity == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }
            else
            {
                var user = await userManager.FindByNameAsync(username);
                //Try to add claims from database
                if(user != null)
                {
                    foreach (var c in user.Claims)
                    {
                        myclaims.Add(new Claim(c.ClaimType, c.ClaimValue));
                    }
                }
                
            }

            var now = DateTime.UtcNow;
            
            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64),
                new Claim("myClaimType", "myClaimValue")
            };
            claims.AddRange(myclaims);

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);                        

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds,
                valid_to = jwt.ValidTo
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }


        /// <summary>
        /// Getting identity from database by email and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="signInManager"></param>
        /// <returns>speified ClaimSidentity OR empty Identity</returns>
        private async Task<ClaimsIdentity> GetIdentity(string username, string password, SignInManager<ApplicationUser> signInManager)
        {
            var result = await signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false);
            

            // DON'T do this in production, obviously!
            if ((username == "TEST" && password == "TEST123") || result.Succeeded)
            {
                return await Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(username, "Token"), new Claim[] { }));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }
        

        public static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);


    }
}
