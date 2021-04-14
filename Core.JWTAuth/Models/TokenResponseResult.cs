using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.JWTAuth.Models
{
    public class TokenResponseResult
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
