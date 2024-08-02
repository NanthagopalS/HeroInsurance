using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class ButtonResponseModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string? ButtonType { get; set; }


        /// <summary>
        /// UserId
        /// </summary>
        public string? ButtonValue { get; set; }

        public string? ButtonStatus { get; set; }
    }
}
