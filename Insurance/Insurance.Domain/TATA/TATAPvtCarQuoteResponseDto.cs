using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATAPvtCarQuoteResponseDto
    {
        public dynamic status { get; set; }
        public string message_txt { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public Data data { get; set; }
        public Pol_Dlts pol_dlts { get; set; }
        public string quote_stage { get; set; }
        public string quote_datetime { get; set; }
    }

    public class Data
    {
        public string policy_id { get; set; }
        public string product_id { get; set; }
        public string product_group_id { get; set; }
        public string par_product_id { get; set; }
        public string quote_id { get; set; }
        public string proposal_id { get; set; }
        public string discount_id { get; set; }
        public string document_id { get; set; }
        public string sol_id { get; set; }
        public string motor_plan_opted_no { get; set; }
        public string quote_no { get; set; }
        public string cibilScore { get; set; }
        public dynamic customerAge { get; set; }
        public dynamic cibilDiscount { get; set; }
        public string prev_pol_end_date { get; set; }
        public string prev_pol_start_date { get; set; }
        public dynamic man_year { get; set; }
        public string q_producer_code { get; set; }
        public string proposer_type { get; set; }
        public string product_code { get; set; }
        public string pol_plan_id { get; set; }
        public string business_type { get; set; }
        public string business_type_no { get; set; }
        public string pol_plan_variant { get; set; }
        public string proposer_pincode { get; set; }
        public string pol_start_date { get; set; }
        public string pol_end_date { get; set; }
        public string dor { get; set; }
        public dynamic vehicle_make_no { get; set; }
        public string vehicle_make { get; set; }
        public dynamic vehicle_model_no { get; set; }
        public string vehicle_model { get; set; }
        public string vehicle_variant_no { get; set; }
        public string vehicle_variant { get; set; }
        public dynamic min_idv { get; set; }
        public dynamic max_idv { get; set; }
        public string regno_1 { get; set; }
        public string regno_2 { get; set; }
        public string regno_3 { get; set; }
        public string regno_4 { get; set; }
        public string place_reg { get; set; }
        public string place_reg_no { get; set; }
        public string BH_regno { get; set; }
        public string special_regno { get; set; }
        public string prev_pol_type { get; set; }
        public string claim_last { get; set; }
        public string pre_pol_ncb { get; set; }
        public string segment { get; set; }
        public dynamic segment_code { get; set; }
        public dynamic cc { get; set; }
        public dynamic sc { get; set; }
        public Premium_Break_Up premium_break_up { get; set; }
    }

    public class Premium_Break_Up
    {
        public Total_Od_Premium total_od_premium { get; set; }
        public Total_Addons total_addOns { get; set; }
        public Total_Tp_Premium total_tp_premium { get; set; }
        public double final_premium { get; set; }
        public double igst_prem { get; set; }
        public dynamic sgst_prem { get; set; }
        public dynamic ugst_prem { get; set; }
        public dynamic cgst_prem { get; set; }
        public double net_premium { get; set; }
    }

    public class Total_Od_Premium
    {
        public Od od { get; set; }
        public Loading_Od loading_od { get; set; }
        public Discount_Od discount_od { get; set; }
        public string total_od { get; set; }
    }

    public class Od
    {
        public string basic_od { get; set; }
        public string non_electrical_prem { get; set; }
        public dynamic electrical_prem { get; set; }
        public dynamic cng_lpg_od_prem { get; set; }
        public dynamic trailer_od_prem { get; set; }
        public dynamic geography_extension_od_prem { get; set; }
        public dynamic add_towing_prem { get; set; }
    }

    public class Loading_Od
    {
        public dynamic load_fibre_prem { get; set; }
        public dynamic load_tuition_prem { get; set; }
        public dynamic load_imported_prem { get; set; }
    }

    public class Discount_Od
    {
        public dynamic aam_disc_prem { get; set; }
        public dynamic atd_disc_prem { get; set; }
        public dynamic vehicle_blind_prem { get; set; }
        public dynamic vdynamicage_car_od_prem { get; set; }
        public dynamic vd_disc_prem { get; set; }
        public dynamic ncb_prem { get; set; }
    }

    public class Total_Addons
    {
        public string dep_reimburse_prem { get; set; }
        public dynamic return_invoice_prem { get; set; }
        public dynamic ncb_protection_prem { get; set; }
        public dynamic personal_loss_prem { get; set; }
        public dynamic key_replace_prem { get; set; }
        public dynamic engine_secure_prem { get; set; }
        public dynamic tyre_secure_prem { get; set; }
        public dynamic consumbale_expense_prem { get; set; }
        public dynamic repair_glass_prem { get; set; }
        public dynamic rsa_prem { get; set; }
        public dynamic emergency_expense_prem { get; set; }
        public dynamic total_addon { get; set; }
    }

    public class Total_Tp_Premium
    {
        public dynamic basic_tp { get; set; }
        public dynamic tppd_prem { get; set; }
        public dynamic vdynamicage_car_tp_prem { get; set; }
        public dynamic own_premises_tp_prem { get; set; }
        public dynamic trailer_tp_prem { get; set; }
        public dynamic cng_lpg_tp_prem { get; set; }
        public dynamic geography_extension_tp_prem { get; set; }
        public dynamic pa_owner_prem { get; set; }
        public dynamic pa_paid_prem { get; set; }
        public string pa_named_no { get; set; }
        public string pa_unnamed_no { get; set; }
        public string pa_paid_no { get; set; }
        public string ll_paid_no { get; set; }
        public string ll_emp_no { get; set; }
        public dynamic ll_soldier_prem { get; set; }
        public string pa_unnamed_prem { get; set; }
        public dynamic ll_paid_prem { get; set; }
        public dynamic total_tp { get; set; }
    }

    public class Pol_Dlts
    {
        public string q_dealer_code { get; set; }
        public string vehicle_fuel { get; set; }
        public dynamic vehicle_cc { get; set; }
        public dynamic vehicle_price { get; set; }
        public dynamic vehicle_idv { get; set; }
        public dynamic vehicle_seating { get; set; }
        public string choice_opted { get; set; }
        public string motor_plan_opted { get; set; }
        public dynamic tppd_prem { get; set; }
        public dynamic cng_lpg_tp_prem { get; set; }
        public dynamic basic_tp { get; set; }
        public dynamic detariff { get; set; }
        public string basic_od { get; set; }
        public string od_rate { get; set; }
        public dynamic cng_lpg_od_prem { get; set; }
        public dynamic electrical_prem { get; set; }
        public string basis_of_rating { get; set; }
        public dynamic non_electrical_prem { get; set; }
        public dynamic pa_unnamed_prem { get; set; }
        public string q_branch_name { get; set; }
        public dynamic ll_paid_prem { get; set; }
        public dynamic pa_paid_prem { get; set; }
        public string q_emp_code { get; set; }
        public dynamic ll_emp_prem { get; set; }
        public dynamic total_tp { get; set; }
        public string q_user_id { get; set; }
        public dynamic pa_owner_prem { get; set; }
        public dynamic total_addon { get; set; }
        public string total_od { get; set; }
        public dynamic aam_disc_prem { get; set; }
        public dynamic vd_disc_prem { get; set; }
        public string od_tenure { get; set; }
        public dynamic atd_disc_prem { get; set; }
        public string tp_tenure { get; set; }
        public dynamic dep_reimburse_prem { get; set; }
        public string service_fault_code { get; set; }
        public dynamic tyre_secure_prem { get; set; }
        public string service_fault_msg { get; set; }
        public dynamic engine_secure_prem { get; set; }
        public string has_break_in { get; set; }
        public dynamic break_in_days { get; set; }
        public dynamic emergency_expense_prem { get; set; }
        public dynamic min_idv { get; set; }
        public dynamic ncb_protection_prem { get; set; }
        public dynamic max_idv { get; set; }
        public dynamic consumbale_expense_prem { get; set; }
        public dynamic return_invoice_prem { get; set; }
        public dynamic veh_age { get; set; }
        public dynamic personal_loss_prem { get; set; }
        public dynamic ll_paid_si { get; set; }
        public dynamic key_replace_prem { get; set; }
        public dynamic repair_glass_prem { get; set; }
        public dynamic total_idv { get; set; }
        public dynamic rsa_prem { get; set; }
        public dynamic cng_lpg_idv2 { get; set; }
        public dynamic net_premium { get; set; }
        public dynamic vehicle_system_idv2 { get; set; }
        public dynamic compulsory_excess { get; set; }
        public dynamic electrical_idv1 { get; set; }
        public dynamic vehicle_user_idv1 { get; set; }
        public dynamic trailer_tp_prem { get; set; }
        public dynamic cng_lpg_idv3 { get; set; }
        public dynamic trailer_od_prem { get; set; }
        public dynamic vehicle_user_idv3 { get; set; }
        public dynamic vehicle_blind_prem { get; set; }
        public dynamic vehicle_system_idv3 { get; set; }
        public dynamic own_premises_od_prem { get; set; }
        public dynamic et_si { get; set; }
        public dynamic own_premises_tp_prem { get; set; }
        public dynamic vehicle_user_idv2 { get; set; }
        public dynamic non_electrical_idv1 { get; set; }
        public dynamic lopb_si { get; set; }
        public dynamic geography_extension_tp_prem { get; set; }
        public dynamic key_si { get; set; }
        public dynamic geography_extension_od_prem { get; set; }
        public dynamic cng_lpg_idv1 { get; set; }
        public dynamic ll_soldier_prem { get; set; }
        public dynamic electrical_idv2 { get; set; }
        public dynamic load_fibre_prem { get; set; }
        public dynamic vehicle_system_idv1 { get; set; }
        public dynamic load_imported_prem { get; set; }
        public dynamic non_electrical_idv2 { get; set; }
        public dynamic load_tuition_prem { get; set; }
        public dynamic electrical_idv3 { get; set; }
        public dynamic pa_named_prem { get; set; }
        public dynamic non_electrical_idv3 { get; set; }
        public float prem_gst { get; set; }
        public dynamic curr_ncb { get; set; }
        public dynamic curr_ncb_perc { get; set; }
        public dynamic sgst_rate { get; set; }
        public dynamic dep_reimburse_si { get; set; }
        public dynamic cgst_rate { get; set; }
        public dynamic tyre_secure_si { get; set; }
        public dynamic igst_rate { get; set; }
        public dynamic engine_secure_si { get; set; }
        public dynamic ugst_rate { get; set; }
        public dynamic sgst_prem { get; set; }
        public dynamic emergency_expense_si { get; set; }
        public dynamic cgst_prem { get; set; }
        public dynamic ncb_protection_si { get; set; }
        public float igst_prem { get; set; }
        public dynamic consumbale_expense_si { get; set; }
        public dynamic ugst_prem { get; set; }
        public dynamic return_invoice_si { get; set; }
        public dynamic kerala_cess_rate { get; set; }
        public dynamic personal_loss_si { get; set; }
        public dynamic kerala_cess_prem { get; set; }
        public dynamic key_replace_si { get; set; }
        public dynamic repair_glass_si { get; set; }
        public dynamic rsa_si { get; set; }
        public string cpa_start { get; set; }
        public string cpa_end { get; set; }
        public dynamic vehicle_wheels { get; set; }
        public string vehicle_body { get; set; }
        public string proposer_district { get; set; }
        public string proposer_city { get; set; }
        public string proposer_state { get; set; }
        public dynamic vehicle_price_no { get; set; }
        public string refferal { get; set; }
        public string refferalMsg { get; set; }
        public string self_inspection_link { get; set; }
        public string inspectionFlag { get; set; }
        public string q_mobile_no { get; set; }
        public dynamic system_discount { get; set; }
        public string banca_product_name { get; set; }
        public string quote_expiry_days { get; set; }
        public dynamic deviation_discount { get; set; }
        public dynamic total_discount { get; set; }
        public string is_posp { get; set; }
        public string q_agent_pan { get; set; }
        public string policy_id { get; set; }
        public string sol_id { get; set; }
    }

}
