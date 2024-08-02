using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.GoDigit
{
    public class GodigitCKYCRequest
    {
        public string dateOfBirth { get; set; }
        public string panNumber { get; set; }
        public string customerType { get; set; }
        public string dateOfInsertion { get; set; }
    }
}
