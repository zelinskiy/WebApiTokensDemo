using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenCalc.Services
{
    public interface ITokenProviderOptionsService
    {
        IOptions<TokenProviderOptions> GetOptions();
    }
}
