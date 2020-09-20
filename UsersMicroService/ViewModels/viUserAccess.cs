using Reipush.Api.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.ViewModels
{
    public class viUserAccess
    {
        public viUserAccess()
        {
            refreshAccesToken = new RefreshAccessToken();
        }
        public int UserId { get; set; }
        public RefreshAccessToken refreshAccesToken { get; set; }
    }
}
