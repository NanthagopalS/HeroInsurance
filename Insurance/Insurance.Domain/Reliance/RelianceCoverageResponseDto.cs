using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Reliance;
public class RelianceCoverageResponseDto
{
    public ListCovers ListCovers { get; set; }
}

public class AddonCovers
{
    public string CoverageName { get; set; }
    public string CoverageID { get; set; }
    public string TypeofCover { get; set; }
    public string rate { get; set; }
    public EMIProtection EMIProtection { get; set; }
    public LossofpersonalbelongingCover LossofpersonalbelongingCover { get; set; }
}

public class CoverageList
{
    public List<CoverDetail> CoverDetail { get; set; }
}

public class CoverDetail
{
    public string CoverageName { get; set; }
    public string CoverageID { get; set; }
    public string Ismandatory { get; set; }
    public ListValues ListValues { get; set; }
}

public class EMIProtection
{
    public string EMIPlan1Rate { get; set; }
    public string EMIPlan2Rate { get; set; }
    public string EMIPlan3Rate { get; set; }
}

public class ListCovers
{
    [JsonProperty("@xmlns:xsd")]
    public string xmlnsxsd { get; set; }

    [JsonProperty("@xmlns:xsi")]
    public string xmlnsxsi { get; set; }
    public CoverageList CoverageList { get; set; }
    public string TraceID { get; set; }
    public LstAddonCovers LstAddonCovers { get; set; }
    public string lstPACoverBenefits { get; set; }
    public string ErrorMessages { get; set; }
}

public class ListValues
{
    public object Values { get; set; }
}

public class Lossofpersonalbelonging
{
    public string minSumInsured { get; set; }
    public string maxSumInsured { get; set; }
    public string coverPremium { get; set; }
    public string isOptedByCustomer { get; set; }
}

public class LossofpersonalbelongingCover
{
    public Lossofpersonalbelonging Lossofpersonalbelonging { get; set; }
}

public class LstAddonCovers
{
    public List<AddonCovers> AddonCovers { get; set; }
}




