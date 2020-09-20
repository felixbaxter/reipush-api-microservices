using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reipush.Api.Entities.User
{
    public class RefreshAccessToken
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}
