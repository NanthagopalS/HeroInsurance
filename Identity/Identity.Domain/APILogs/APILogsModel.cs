using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.APILogs;

public class APILogsModel
{
	public string RequestMethod { get; set; }
	public string RequestPath { get; set; }
	public string RequestUrl { get; set; }
	public string RequestBody { get; set; }
	public string ResponseStatusCode { get; set; }
	public string ResponseBody { get; set; }
	public string UserId { get; set; }
}
