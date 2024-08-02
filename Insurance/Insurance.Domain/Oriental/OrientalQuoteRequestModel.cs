using System.Xml.Serialization;

namespace Insurance.Domain.Oriental;


[XmlRoot(ElementName = "objGetQuoteMotorETT")]
public class ObjGetQuoteMotorEtt
{

    [XmlElement(ElementName = "LOGIN_ID")]
    public string LOGINID { get; set; }

    [XmlElement(ElementName = "DLR_INV_NO")]
    public string DLRINVNO { get; set; }

    [XmlElement(ElementName = "DLR_INV_DT")]
    public string DLRINVDT { get; set; }

    [XmlElement(ElementName = "PRODUCT_CODE")]
    public string PRODUCTCODE { get; set; }

    [XmlElement(ElementName = "POLICY_TYPE")]
    public string POLICYTYPE { get; set; }

    [XmlElement(ElementName = "START_DATE")]
    public string STARTDATE { get; set; }

    [XmlElement(ElementName = "END_DATE")]
    public string ENDDATE { get; set; }

    [XmlElement(ElementName = "INSURED_NAME")]
    public string INSUREDNAME { get; set; }

    [XmlElement(ElementName = "ADDR_01")]
    public string ADDR01 { get; set; }

    [XmlElement(ElementName = "ADDR_02")]
    public string ADDR02 { get; set; }

    [XmlElement(ElementName = "ADDR_03")]
    public string ADDR03 { get; set; }

    [XmlElement(ElementName = "CITY")]
    public string CITY { get; set; }

    [XmlElement(ElementName = "STATE")]
    public string STATE { get; set; }

    [XmlElement(ElementName = "PINCODE")]
    public string PINCODE { get; set; }

    [XmlElement(ElementName = "COUNTRY")]
    public string COUNTRY { get; set; }

    [XmlElement(ElementName = "EMAIL_ID")]
    public string EMAILID { get; set; }

    [XmlElement(ElementName = "MOBILE_NO")]
    public string MOBILENO { get; set; }

    [XmlElement(ElementName = "TEL_NO")]
    public string TELNO { get; set; }

    [XmlElement(ElementName = "FAX_NO")]
    public string FAXNO { get; set; }

    [XmlElement(ElementName = "ROAD_TRANSPORT_YN")]
    public int ROADTRANSPORTYN { get; set; }

    [XmlElement(ElementName = "INSURED_KYC_VERIFIED")]
    public int INSUREDKYCVERIFIED { get; set; }

    [XmlElement(ElementName = "MOU_ORG_MEM_ID")]
    public string MOUORGMEMID { get; set; }

    [XmlElement(ElementName = "MOU_ORG_MEM_VALI")]
    public string MOUORGMEMVALI { get; set; }

    [XmlElement(ElementName = "MANUF_VEHICLE_CODE")]
    public string MANUFVEHICLECODE { get; set; }

    [XmlElement(ElementName = "VEHICLE_CODE")]
    public string VEHICLECODE { get; set; }

    [XmlElement(ElementName = "VEHICLE_TYPE_CODE")]
    public string VEHICLETYPECODE { get; set; }

    [XmlElement(ElementName = "VEHICLE_CLASS_CODE")]
    public string VEHICLECLASSCODE { get; set; }

    [XmlElement(ElementName = "MANUF_CODE")]
    public string MANUFCODE { get; set; }

    [XmlElement(ElementName = "VEHICLE_MODEL_CODE")]
    public string VEHICLEMODELCODE { get; set; }

    [XmlElement(ElementName = "TYPE_OF_BODY_CODE")]
    public string TYPEOFBODYCODE { get; set; }

    [XmlElement(ElementName = "VEHICLE_COLOR")]
    public string VEHICLECOLOR { get; set; }

    [XmlElement(ElementName = "VEHICLE_REG_NUMBER")]
    public string VEHICLEREGNUMBER { get; set; }

    [XmlElement(ElementName = "FIRST_REG_DATE")]
    public string FIRSTREGDATE { get; set; }

    [XmlElement(ElementName = "ENGINE_NUMBER")]
    public string ENGINENUMBER { get; set; }

    [XmlElement(ElementName = "CHASSIS_NUMBER")]
    public string CHASSISNUMBER { get; set; }

    [XmlElement(ElementName = "VEH_IDV")]
    public string VEHIDV { get; set; }

    [XmlElement(ElementName = "CUBIC_CAPACITY")]
    public string CUBICCAPACITY { get; set; }

    [XmlElement(ElementName = "THREEWHEELER_YN")]
    public int THREEWHEELERYN { get; set; }

    [XmlElement(ElementName = "SEATING_CAPACITY")]
    public string SEATINGCAPACITY { get; set; }

    [XmlElement(ElementName = "VEHICLE_GVW")]
    public string  VEHICLEGVW { get; set; }

    [XmlElement(ElementName = "NO_OF_DRIVERS")]
    public int NOOFDRIVERS { get; set; }

    [XmlElement(ElementName = "FUEL_TYPE_CODE")]
    public string FUELTYPECODE { get; set; }

    [XmlElement(ElementName = "RTO_CODE")]
    public string RTOCODE { get; set; }

    [XmlElement(ElementName = "ZONE_CODE")]
    public int ZONECODE { get; set; }

    [XmlElement(ElementName = "GEO_EXT_CODE")]
    public string GEOEXTCODE { get; set; }

    [XmlElement(ElementName = "VOLUNTARY_EXCESS")]
    public string VOLUNTARYEXCESS { get; set; }

    [XmlElement(ElementName = "MEMBER_OF_AAI")]
    public int MEMBEROFAAI { get; set; }

    [XmlElement(ElementName = "ANTITHEFT_DEVICE_DESC")]
    public int ANTITHEFTDEVICEDESC { get; set; }

    [XmlElement(ElementName = "NON_ELEC_ACCESS_DESC")]
    public string NONELECACCESSDESC { get; set; }

    [XmlElement(ElementName = "NON_ELEC_ACCESS_VALUE")]
    public int NONELECACCESSVALUE { get; set; }

    [XmlElement(ElementName = "ELEC_ACCESS_DESC")]
    public string ELECACCESSDESC { get; set; }

    [XmlElement(ElementName = "ELEC_ACCESS_VALUE")]
    public int ELECACCESSVALUE { get; set; }

    [XmlElement(ElementName = "SIDE_CAR_ACCESS_DESC")]
    public string SIDECARACCESSDESC { get; set; }

    [XmlElement(ElementName = "SIDE_CARS_VALUE")]
    public string SIDECARSVALUE { get; set; }

    [XmlElement(ElementName = "TRAILER_DESC")]
    public string TRAILERDESC { get; set; }

    [XmlElement(ElementName = "TRAILER_VALUE")]
    public string TRAILERVALUE { get; set; }

    [XmlElement(ElementName = "ARTI_TRAILER_DESC")]
    public string ARTITRAILERDESC { get; set; }

    [XmlElement(ElementName = "ARTI_TRAILER_VALUE")]
    public string ARTITRAILERVALUE { get; set; }

    [XmlElement(ElementName = "PREV_YR_ICR")]
    public string PREVYRICR { get; set; }

    [XmlElement(ElementName = "NCB_DECL_SUBMIT_YN")]
    public int NCBDECLSUBMITYN { get; set; }

    [XmlElement(ElementName = "LIMITED_TPPD_YN")]
    public int LIMITEDTPPDYN { get; set; }

    [XmlElement(ElementName = "RALLY_COVER_YN")]
    public int RALLYCOVERYN { get; set; }

    [XmlElement(ElementName = "RALLY_DAYS")]
    public int RALLYDAYS { get; set; }

    [XmlElement(ElementName = "NIL_DEP_YN")]
    public int NILDEPYN { get; set; }

    [XmlElement(ElementName = "CNG_KIT_VALUE")]
    public string CNGKITVALUE { get; set; }

    [XmlElement(ElementName = "FIBRE_TANK_VALUE")]
    public int FIBRETANKVALUE { get; set; }

    [XmlElement(ElementName = "ALT_CAR_BENEFIT")]
    public string ALTCARBENEFIT { get; set; }

    [XmlElement(ElementName = "PERS_EFF_COVER")]
    public string PERSEFFCOVER { get; set; }

    [XmlElement(ElementName = "NO_OF_PA_OWNER_DRIVER")]
    public int NOOFPAOWNERDRIVER { get; set; }

    [XmlElement(ElementName = "NO_OF_PA_NAMED_PERSONS")]
    public int NOOFPANAMEDPERSONS { get; set; }

    [XmlElement(ElementName = "PA_NAMED_PERSONS_SI")]
    public int PANAMEDPERSONSSI { get; set; }

    [XmlElement(ElementName = "NO_OF_PA_UNNAMED_PERSONS")]
    public int NOOFPAUNNAMEDPERSONS { get; set; }

    [XmlElement(ElementName = "PA_UNNAMED_PERSONS_SI")]
    public int PAUNNAMEDPERSONSSI { get; set; }

    [XmlElement(ElementName = "NO_OF_PA_UNNAMED_HIRER")]
    public string NOOFPAUNNAMEDHIRER { get; set; }

    [XmlElement(ElementName = "NO_OF_LL_EMPLOYEES")]
    public int NOOFLLEMPLOYEES { get; set; }

    [XmlElement(ElementName = "NO_OF_LL_PAID_DRIVER")]
    public int NOOFLLPAIDDRIVER { get; set; }

    [XmlElement(ElementName = "NO_OF_LL_SOLDIERS")]
    public string NOOFLLSOLDIERS { get; set; }

    [XmlElement(ElementName = "OTH_SINGLE_FUEL_CVR")]
    public int OTHSINGLEFUELCVR { get; set; }

    [XmlElement(ElementName = "IMP_CAR_WO_CUSTOMS_CVR")]
    public int IMPCARWOCUSTOMSCVR { get; set; }

    [XmlElement(ElementName = "DRIVING_TUITION_EXT_CVR")]
    public int DRIVINGTUITIONEXTCVR { get; set; }

    [XmlElement(ElementName = "NO_OF_COOLIES")]
    public int NOOFCOOLIES { get; set; }

    [XmlElement(ElementName = "NO_OF_CONDUCTORS")]
    public int NOOFCONDUCTORS { get; set; }

    [XmlElement(ElementName = "NO_OF_CLEANERS")]
    public int NOOFCLEANERS { get; set; }

    [XmlElement(ElementName = "TOWING_TYPE")]
    public string TOWINGTYPE { get; set; }

    [XmlElement(ElementName = "NO_OF_TRAILERS_TOWED")]
    public string NOOFTRAILERSTOWED { get; set; }

    [XmlElement(ElementName = "NO_OF_NFPP_EMPL")]
    public string NOOFNFPPEMPL { get; set; }

    [XmlElement(ElementName = "NO_OF_NFPP_OTH_THAN_EMPL")]
    public string NOOFNFPPOTHTHANEMPL { get; set; }

    [XmlElement(ElementName = "DLR_PA_NOMINEE_NAME")]
    public string DLRPANOMINEENAME { get; set; }

    [XmlElement(ElementName = "DLR_PA_NOMINEE_DOB")]
    public string DLRPANOMINEEDOB { get; set; }

    [XmlElement(ElementName = "DLR_PA_NOMINEE_RELATION")]
    public string DLRPANOMINEERELATION { get; set; }

    [XmlElement(ElementName = "RETN_TO_INVOICE")]
    public int RETNTOINVOICE { get; set; }

    [XmlElement(ElementName = "HYPO_TYPE")]
    public string HYPOTYPE { get; set; }

    [XmlElement(ElementName = "HYPO_COMP_NAME")]
    public string HYPOCOMPNAME { get; set; }

    [XmlElement(ElementName = "HYPO_COMP_ADDR_01")]
    public string HYPOCOMPADDR01 { get; set; }

    [XmlElement(ElementName = "HYPO_COMP_ADDR_02")]
    public string HYPOCOMPADDR02 { get; set; }

    [XmlElement(ElementName = "HYPO_COMP_ADDR_03")]
    public string HYPOCOMPADDR03 { get; set; }

    [XmlElement(ElementName = "HYPO_COMP_CITY")]
    public string HYPOCOMPCITY { get; set; }

    [XmlElement(ElementName = "HYPO_COMP_STATE")]
    public string HYPOCOMPSTATE { get; set; }

    [XmlElement(ElementName = "HYPO_COMP_PINCODE")]
    public string HYPOCOMPPINCODE { get; set; }

    [XmlElement(ElementName = "PAYMENT_TYPE")]
    public string PAYMENTTYPE { get; set; }

    [XmlElement(ElementName = "NCB_PERCENTAGE")]
    public string NCBPERCENTAGE { get; set; }

    [XmlElement(ElementName = "PREV_INSU_COMPANY")]
    public string PREVINSUCOMPANY { get; set; }

    [XmlElement(ElementName = "PREV_POL_NUMBER")]
    public string PREVPOLNUMBER { get; set; }

    [XmlElement(ElementName = "PREV_POL_START_DATE")]
    public string PREVPOLSTARTDATE { get; set; }

    [XmlElement(ElementName = "PREV_POL_END_DATE")]
    public string PREVPOLENDDATE { get; set; }

    [XmlElement(ElementName = "EXIS_POL_FM_OTHER_INSR")]
    public int EXISPOLFMOTHERINSR { get; set; }

    [XmlElement(ElementName = "IP_ADDRESS")]
    public string IPADDRESS { get; set; }

    [XmlElement(ElementName = "MAC_ADDRESS")]
    public string MACADDRESS { get; set; }

    [XmlElement(ElementName = "WIN_USER_ID")]
    public string WINUSERID { get; set; }

    [XmlElement(ElementName = "WIN_MACHINE_ID")]
    public string WINMACHINEID { get; set; }

    [XmlElement(ElementName = "DISCOUNT_PERC")]
    public string DISCOUNTPERC { get; set; }

    [XmlElement(ElementName = "FLEX_01")]
    public string FLEX01 { get; set; }

    [XmlElement(ElementName = "FLEX_02")]
    public string FLEX02 { get; set; }

    [XmlElement(ElementName = "FLEX_03")]
    public string FLEX03 { get; set; }

    [XmlElement(ElementName = "FLEX_04")]
    public string FLEX04 { get; set; }

    [XmlElement(ElementName = "FLEX_05")]
    public string FLEX05 { get; set; }

    [XmlElement(ElementName = "FLEX_06")]
    public string FLEX06 { get; set; }

    [XmlElement(ElementName = "FLEX_07")]
    public string FLEX07 { get; set; }

    [XmlElement(ElementName = "FLEX_08")]
    public string FLEX08 { get; set; }

    [XmlElement(ElementName = "FLEX_09")]
    public string FLEX09 { get; set; }
    [XmlElement(ElementName = "FLEX_12")]
    public string FLEX12 { get; set; }

    [XmlElement(ElementName = "FLEX_10")]
    public string FLEX10 { get; set; }
    [XmlElement(ElementName = "FLEX_19")]
    public string FLEX19 { get; set; }

    [XmlElement(ElementName = "FLEX_20")]
    public string FLEX20 { get; set; }

    [XmlElement(ElementName = "FLEX_21")]
    public string FLEX21 { get; set; }
    [XmlElement(ElementName = "FLEX_24")]
    public string FLEX24 { get; set; }

    [XmlElement(ElementName = "FLEX_22")]
    public int FLEX22 { get; set; }

    [XmlElement(ElementName = "FLEX_25")]
    public string FLEX25 { get; set; }

    [XmlElement(ElementName = "FLEX_32")]
    public string FLEX32 { get; set; }
}

[XmlRoot(ElementName = "GetQuoteMotor")]
public class GetQuoteMotor
{

    [XmlElement(ElementName = "objGetQuoteMotorETT")]
    public ObjGetQuoteMotorEtt ObjGetQuoteMotorETT { get; set; }

    [XmlAttribute(AttributeName = "xmlns")]
    public string Xmlns { get; set; }

    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "Body")]
public class OrientalBody
{

    [XmlElement(ElementName = "GetQuoteMotor", Namespace = "http://MotorService/")]
    public GetQuoteMotor GetQuoteMotor { get; set; }
}

[XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class OrientalEnvelope
{

    [XmlElement(ElementName = "Body")]
    public OrientalBody Body { get; set; }

    [XmlAttribute(AttributeName = "soap")]
    public string Soap { get; set; }

    [XmlAttribute(AttributeName = "xsi")]
    public string Xsi { get; set; }

    [XmlAttribute(AttributeName = "xsd")]
    public string Xsd { get; set; }

    [XmlText]
    public string Text { get; set; }
}
