using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace TokenCalc.Services
{
    public class TokenProviderOptionsService : ITokenProviderOptionsService
    {
        private TokenProviderOptions options = new TokenProviderOptions();

        public TokenProviderOptionsService() { }
        public TokenProviderOptionsService(Action<TokenProviderOptions> builder)
        {
            builder.Invoke(options);
        }        

        IOptions<TokenProviderOptions> ITokenProviderOptionsService.GetOptions()
        {
            return Options.Create(options);
        }
    }
}
