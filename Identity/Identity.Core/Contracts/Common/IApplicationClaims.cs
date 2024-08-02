using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Contracts.Common;

public interface IApplicationClaims
{
	string GetUserId();
}
