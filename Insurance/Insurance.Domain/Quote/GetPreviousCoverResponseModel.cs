using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
	public class GetPreviousCoverResponseModel
	{
		public IEnumerable<GetPreviousCoverRecords>	getPreviousCoverRecords { get; set; }
	}
	public record GetPreviousCoverRecords
	{

		public string CoverId { get; set; }
		public string CoverName { get; set; }
		public string CoverCode { get; set; }
        public string Text { get; set; }
        public string Flag { get; set; }
    }
}
