using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
	public class TataCKYCRequest
	{
		public string dateOfBirth { get; set; }
		public string pan { get; set; }
		public string aadharNumber { get; set; }
		public string gender { get; set; }
	}
}
