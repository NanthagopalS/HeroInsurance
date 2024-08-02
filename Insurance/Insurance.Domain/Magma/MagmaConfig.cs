using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Magma
{
    public class MagmaConfig
    {
        public string BaseURL { get; set; }
        public string QuoteURL { get; set; }
        public string TokenURL { get; set; }
        public string InsurerId { get; set; }
        public TokenData Token { get; set; }
    }
    public class TokenData
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string CompanyName { get; set;}
    }
}
