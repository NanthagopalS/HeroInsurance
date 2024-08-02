using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
	public class TATABreakInResponseModel
	{
		public int InsurerStatusCode { get; set; }
		public TATABreakInPesponseDto TATABreakInPesponseDto { get; set; }
    }
}
