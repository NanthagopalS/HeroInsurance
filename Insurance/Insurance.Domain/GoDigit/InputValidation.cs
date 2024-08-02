using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.GoDigit
{
    public class InputValidation
    {
        public bool IsAddonIdValid { get; set; }
        public bool IsDiscountIdValid { get; set; }
        public bool ISPacoverIdValid { get; set; }
        public bool IsAccessoryIdValid { get; set; }
        public bool IsVarientIdValid { get; set; }
        public bool IsVehicleTypeIdValid { get; set; }
        public bool IsPolicyTypeIdValid { get; set; }
        public bool IsRTOIdValid { get; set; }
    }
}
