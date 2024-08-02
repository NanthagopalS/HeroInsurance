using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
	public class TATAPolicyDocumentResponseModel
	{
		public int InsurerStatusCode { get; set; }
		public string PolicyDocumentBase64 { get; set; }
	}
}
