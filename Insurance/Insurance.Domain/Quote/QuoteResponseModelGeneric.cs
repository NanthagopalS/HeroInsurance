﻿using Insurance.Domain.GoDigit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class QuoteResponseModelGeneric
    {
        public QuoteResponseModel QuoteResponseModel { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
    }
}