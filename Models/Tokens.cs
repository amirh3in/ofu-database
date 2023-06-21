using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace reservationApi.Models
{
    public class Tokens
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}
