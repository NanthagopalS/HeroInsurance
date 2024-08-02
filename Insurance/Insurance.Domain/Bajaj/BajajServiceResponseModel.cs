using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Bajaj
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Premiumdetails
    {
        public string ncbamt { get; set; }
        public string addloadprem { get; set; }
        public string totalodpremium { get; set; }
        public string totalactpremium { get; set; }
        public string totalnetpremium { get; set; }
        public string totalpremium { get; set; }
        public string netpremium { get; set; }
        public string finalpremium { get; set; }
        public string spdisc { get; set; }
        public string servicetax { get; set; }
        public string stampduty { get; set; }
        public string collpremium { get; set; }
        public string imtout { get; set; }
        public string totaliev { get; set; }
    }

    public class Premiumsummerylist
    {
        public string paramdesc { get; set; }
        public string paramref { get; set; }
        public string paramtype { get; set; }
        public string od { get; set; }
        public string act { get; set; }
        public string net { get; set; }
    }

    public class BajajServiceResponseModel
    {
        public Premiumdetails premiumdetails { get; set; }
        public List<Premiumsummerylist> premiumsummerylist { get; set; }
        public List<errorlist> errorlist { get; set; }
        public int errorcode { get; set; }
        public string transactionid { get; set; }
    }
    public class errorlist
    {
        public string errnumber { get; set; }
        public string parname { get; set; }
        public string parindex { get; set; }
        public string property { get; set; }
        public string errtext { get; set; }
        public string errlevel { get; set; }
    }
}
