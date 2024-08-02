using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class ErrorCodeModel
    {
        public string Id { get; set; }
        public string ErrorType { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorKey { get; set; }
        public string ErrorMessage { get; set; }
        
    }
}
