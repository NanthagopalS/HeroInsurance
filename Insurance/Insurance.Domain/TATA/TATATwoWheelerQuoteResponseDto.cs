using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATATwoWheelerQuoteResponseDto
    {
        public int status { get; set; }
        public string message_txt { get; set; }
        public TW_Data data { get; set; }
    }

    public class TW_Data
    {
        public string quote_id { get; set; }
        public string proposal_id { get; set; }
        public string policy_id { get; set; }
        public string quote_no { get; set; }
        public string pol_start_date { get; set; }
        public string pol_end_date { get; set; }
        public int status { get; set; }
        public string business_type { get; set; }
        public string proposer_type { get; set; }
        public string plan_type { get; set; }
        public string dor { get; set; }
        public string man_year { get; set; }
        public string vehicle_make { get; set; }
        public string vehicle_model { get; set; }
        public string vehicle_variant { get; set; }
        public string make_code { get; set; }
        public string model_code { get; set; }
        public string variant_code { get; set; }
        public string proposer_pincode { get; set; }
        public string place_reg { get; set; }
        public string place_reg_no { get; set; }
        public string quote_datetime { get; set; }
        public string q_producer_code { get; set; }
        public string q_producer_email { get; set; }
        public string is_posp { get; set; }
        public string q_agent_pan { get; set; }
        public int q_office_location { get; set; }
        public string sol_id { get; set; }
        public string regno_1 { get; set; }
        public string regno_2 { get; set; }
        public string regno_3 { get; set; }
        public string regno_4 { get; set; }
        public string BH_regno { get; set; }
        public string special_regno { get; set; }
        public TW_Data1 data { get; set; }
        public TW_Premium_Break_Up premium_break_up { get; set; }
        public TW_Pol_Dlts pol_dlts { get; set; }
        public string quote_stage { get; set; }
    }

    public class TW_Data1
    {
        public int vehicleAge { get; set; }
        public string hasBreakIn { get; set; }
        public int breakInDays { get; set; }
        public bool error { get; set; }
        public int currentYearNCB { get; set; }
        public int baseIDV { get; set; }
        public int maximumIDV { get; set; }
        public int minimumIDV { get; set; }
        public dynamic netPremium { get; set; }
        public Declines declines { get; set; }
        public Referrals referrals { get; set; }
        public Underwritingmessages underwritingMessages { get; set; }
        public Exceptionrules exceptionRules { get; set; }
        public Vehicleinspections vehicleInspections { get; set; }
        public bool vehicleInspectionFlag { get; set; }
        public string inspectionRights { get; set; }
        public int inspectionValidity { get; set; }
        public string noOfPhotographs { get; set; }
        public int rsdDays { get; set; }
        public bool overrideInspection { get; set; }
        public bool selfInspection { get; set; }
        public bool thirdPartyInspection { get; set; }
        public dynamic genderDiscount { get; set; }
        public string moratoriumApprovalAuthority { get; set; }
        public string moratoriumCode { get; set; }
        public string ex_showroom_price { get; set; }
        public string vehicle_make_no { get; set; }
        public string vehicle_model_no { get; set; }
        public string vehicle_variant_no { get; set; }
        public int max_idv { get; set; }
        public int min_idv { get; set; }
        public dynamic vehicle_idv { get; set; }
        public dynamic net_premium { get; set; }
        public string refferal { get; set; }
    }

    public class Declines
    {
    }

    public class Referrals
    {
    }

    public class Underwritingmessages
    {
    }

    public class Exceptionrules
    {
    }

    public class Vehicleinspections
    {
    }

    public class TW_Premium_Break_Up
    {
        public TW_Total_Od_Premium total_od_premium { get; set; }
        public TW_Total_Addons total_addOns { get; set; }
        public TW_Total_Tp_Premium total_tp_premium { get; set; }
        public dynamic igst_prem { get; set; }
        public dynamic sgst_prem { get; set; }
        public dynamic ugst_prem { get; set; }
        public dynamic cgst_prem { get; set; }
        public dynamic net_premium { get; set; }
        public dynamic final_premium { get; set; }
    }

    public class TW_Total_Od_Premium
    {
        public TW_Od od { get; set; }
        public TW_Loading_Od loading_od { get; set; }
        public TW_Discount_Od discount_od { get; set; }
        public dynamic total_od { get; set; }
    }

    public class TW_Od
    {
        public dynamic basic_od { get; set; }
        public dynamic non_electrical_prem { get; set; }
        public dynamic electrical_prem { get; set; }
        public dynamic cng_lpg_od_prem { get; set; }
        public dynamic side_car_prem { get; set; }
        public dynamic geography_extension_od_prem { get; set; }
        public dynamic reliability_trials_od_prem { get; set; }
        public dynamic loss_access_prem { get; set; }
        public dynamic vehicle_fitted_fuel_prem { get; set; }
        public dynamic imported_vehicle_prem { get; set; }
        public dynamic driving_tution_od_prem { get; set; }
        public dynamic add_towing_prem { get; set; }
    }

    public class TW_Loading_Od
    {
    }

    public class TW_Discount_Od
    {
        public dynamic disc_auto_asso_prem { get; set; }
        public dynamic disc_blind_handi_prem { get; set; }
        public dynamic disc_antitheft_prem { get; set; }
        public dynamic limited_own_od_prem { get; set; }
        public dynamic voluntary_prem { get; set; }
        public dynamic disc_side_car { get; set; }
    }

    public class TW_Total_Addons
    {
        public dynamic dep_reimburse_prem { get; set; }
        public dynamic return_invoice_prem { get; set; }
        public dynamic consumbale_expense_prem { get; set; }
        public dynamic emergency_medical_expense_prem { get; set; }
        public dynamic rsa_prem { get; set; }
        public dynamic ncb_prem { get; set; }
        public dynamic total_addon { get; set; }
    }

    public class TW_Total_Tp_Premium
    {
        public dynamic basic_tp_prem { get; set; }
        public dynamic disc_limited_own_tp_prem { get; set; }
        public dynamic pa_cover_prem { get; set; }
        public dynamic disc_tppd_prem { get; set; }
        public dynamic cng_lpg_tp_prem { get; set; }
        public dynamic reliability_trials_tp_prem { get; set; }
        public dynamic geo_extension_tp_prem { get; set; }
        public dynamic pa_paid_driver_prem { get; set; }
        public dynamic pa_unnamed_prem { get; set; }
        public dynamic addi_pa_own_drive_prem { get; set; }
        public dynamic addi_pa_unnamed_prem { get; set; }
        public dynamic ll_paid_driver_prem { get; set; }
        public dynamic ll_paid_employee_prem { get; set; }
        public dynamic total_tp_prem { get; set; }
    }

    public class TW_Pol_Dlts
    {
        public string vehicleInspectionFlag { get; set; }
        public string tp_tenure { get; set; }
        public int pp_curr_ncb { get; set; }
        public dynamic total_discount { get; set; }
        public string basic_od { get; set; }
        public string refferal { get; set; }
    }

}
