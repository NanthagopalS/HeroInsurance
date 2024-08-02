using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Bajaj
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Accessorieslist
    {
        public string contractid { get; set; }
        public string acccategorycode { get; set; }
        public string acctypecode { get; set; }
        public string accmake { get; set; }
        public string accmodel { get; set; }
        public string acciev { get; set; }
        public string acccount { get; set; }
    }

    public class Detariffobj
    {
        public string vehpurchasetype { get; set; }
        public string vehpurchasedate { get; set; }
        public string monthofmfg { get; set; }
        public string registrationauth { get; set; }
        public string bodytype { get; set; }
        public string goodstranstype { get; set; }
        public string natureofgoods { get; set; }
        public string othergoodsfrequency { get; set; }
        public string permittype { get; set; }
        public string roadtype { get; set; }
        public string vehdrivenby { get; set; }
        public string driverexperience { get; set; }
        public string clmhistcode { get; set; }
        public string incurredclmexpcode { get; set; }
        public string driverqualificationcode { get; set; }
        public string tacmakecode { get; set; }
        public string extcol1 { get; set; }
        public string extcol2 { get; set; }
        public string extcol3 { get; set; }
        public string extcol4 { get; set; }
        public string extcol5 { get; set; }
        public string extcol6 { get; set; }
        public string extcol7 { get; set; }
        public string extcol8 { get; set; }
        public string extcol9 { get; set; }
        public string extcol10 { get; set; }
        public string extcol11 { get; set; }
        public string extcol12 { get; set; }
        public string extcol13 { get; set; }
        public string extcol14 { get; set; }
        public string extcol15 { get; set; }
        public string extcol16 { get; set; }
        public string extcol17 { get; set; }
        public string extcol18 { get; set; }
        public string extcol19 { get; set; }
        public string extcol20 { get; set; }
        public string extcol21 { get; set; }
        public string extcol22 { get; set; }
        public string extcol23 { get; set; }
        public string extcol24 { get; set; }
        public string extcol25 { get; set; }
        public string extcol26 { get; set; }
        public string extcol27 { get; set; }
        public string extcol28 { get; set; }
        public string extcol29 { get; set; }
        public string extcol30 { get; set; }
        public string extcol31 { get; set; }
        public string extcol32 { get; set; }
        public string extcol33 { get; set; }
        public string extcol34 { get; set; }
        public string extcol35 { get; set; }
        public string extcol36 { get; set; }
        public string extcol37 { get; set; }
        public string extcol38 { get; set; }
        public string extcol39 { get; set; }
        public string extcol40 { get; set; }
    }

    public class Motextracover
    {
        public string geogextn { get; set; }
        public int noofpersonspa { get; set; }
        public string suminsuredpa { get; set; }
        public object suminsuredtotalnamedpa { get; set; }
        public string cngvalue { get; set; }
        public string noofemployeeslle { get; set; }
        public string noofpersonsllo { get; set; }
        public string fibreglassvalue { get; set; }
        public string sidecarvalue { get; set; }
        public string nooftrailers { get; set; }
        public string totaltrailervalue { get; set; }
        public string voluntaryexcess { get; set; }
        public string covernoteno { get; set; }
        public string covernotedate { get; set; }
        public string subimdcode { get; set; }
        public string extrafield1 { get; set; }
        public string extrafield2 { get; set; }
        public string extrafield3 { get; set; }
    }

    public class Paddoncoverlist
    {
        public string paramdesc { get; set; }
        public string paramref { get; set; }
    }

    public class Questlist
    {
        public string questionref { get; set; }
        public string contractid { get; set; }
        public string questionval { get; set; }
    }

    public class BajajServiceRequestModel
    {
        public string userid { get; set; }
        public string password { get; set; }
        public string vehiclecode { get; set; }
        public string city { get; set; }
        public Weomotpolicyin weomotpolicyin { get; set; }
        public List<Accessorieslist> accessorieslist { get; set; }
        public List<Paddoncoverlist> paddoncoverlist { get; set; }
        public Motextracover motextracover { get; set; }
        public List<Questlist> questlist { get; set; }
        public Detariffobj detariffobj { get; set; }
        public string transactionid { get; set; }
        public string transactiontype { get; set; }
        public string contactno { get; set; }
    }

    public class Weomotpolicyin
    {
        public string contractid { get; set; }
        public string poltype { get; set; }
        public string product4digitcode { get; set; }
        public string deptcode { get; set; }
        public string branchcode { get; set; }
        public string termstartdate { get; set; }
        public string termenddate { get; set; }
        public string tpfintype { get; set; }
        public string hypo { get; set; }
        public string vehicletypecode { get; set; }
        public string vehicletype { get; set; }
        public string miscvehtype { get; set; }
        public string vehiclemakecode { get; set; }
        public string vehiclemake { get; set; }
        public string vehiclemodelcode { get; set; }
        public string vehiclemodel { get; set; }
        public string vehiclesubtypecode { get; set; }
        public string vehiclesubtype { get; set; }
        public string fuel { get; set; }
        public string zone { get; set; }
        public string engineno { get; set; }
        public string chassisno { get; set; }
        public string registrationno { get; set; }
        public string registrationdate { get; set; }
        public string registrationlocation { get; set; }
        public string regilocother { get; set; }
        public string carryingcapacity { get; set; }
        public string cubiccapacity { get; set; }
        public string yearmanf { get; set; }
        public string color { get; set; }
        public double vehicleidv { get; set; }
        public string ncb { get; set; }
        public string addloading { get; set; }
        public string addloadingon { get; set; }
        public string spdiscrate { get; set; }
        public string elecacctotal { get; set; }
        public string nonelecacctotal { get; set; }
        public string prvpolicyref { get; set; }
        public string prvexpirydate { get; set; }
        public string prvinscompany { get; set; }
        public string prvncb { get; set; }
        public string prvclaimstatus { get; set; }
        public string automembership { get; set; }
        public string partnertype { get; set; }
    }
}
