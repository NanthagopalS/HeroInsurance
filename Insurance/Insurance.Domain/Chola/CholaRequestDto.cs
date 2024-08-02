﻿namespace Insurance.Domain.Chola;

public class CholaRequestDto
{
    public string user_code { get; set; }
    public string make { get; set; }
    public string model_variant { get; set; }
    public string vehicle_model_code { get; set; }
    public string frm_model_variant { get; set; }
    public string rto_location_code { get; set; }
    public string frm_rto { get; set; }
    public decimal ex_show_room { get; set; }
    public string d2cdtd_masterfetched { get; set; }
    public string usr_make { get; set; }
    public string usr_model { get; set; }
    public string usr_variant { get; set; }
    public string usr_name { get; set; }
    public string usr_mobile { get; set; }
    public string first_name { get; set; }
    public string phone_no { get; set; }
    public string mobile_no { get; set; }
    public string email_id { get; set; }
    public bool authorizeChk { get; set; }
    public int YOR { get; set; }
    public int date_of_reg { get; set; }
    public string IMDState { get; set; }
    public string place_of_reg { get; set; }
    public string place_of_reg_short_code { get; set; }
    public string val_claim { get; set; }
    public string claim_year { get; set; }
    public string B2B_NCB_App { get; set; }
    public string Lastyrncb_percentage { get; set; }
    public string PrvyrClaim { get; set; }
    public string D2C_NCB_PERCENTAGE { get; set; }
    public string state { get; set; }
    public string fuel_type { get; set; }
    public string cubic_capacity { get; set; }
    public string PAAddon { get; set; }
    public string pa_cover { get; set; }
    public string unnamed_cover_opted { get; set; }
    public string NilDepselected { get; set; }
    public string intermediary_code { get; set; }
    public string broker_code { get; set; }
    public string partner_name { get; set; }
    public string save_percentage { get; set; }
    public string Customertype { get; set; }
    public string title { get; set; }
    public string fullName { get; set; }
    public string aadhar { get; set; }
    public string cus_mobile_no { get; set; }
    public string email { get; set; }
    public string customer_dob_input { get; set; }
    public string cmp_gst_no { get; set; }
    public string reg_no { get; set; }
    public int YOM { get; set; }
    public string engine_no { get; set; }
    public string vehicle_color { get; set; }
    public string chassis_no { get; set; }
    public string prev_insurer_name { get; set; }
    public string prev_policy_no { get; set; }
    public string od_prev_insurer_name { get; set; }
    public string od_prev_policy_no { get; set; }
    public string city { get; set; }
    public string pincode { get; set; }
    public string reg_pincode { get; set; }
    public string reg_state { get; set; }
    public string reg_city { get; set; }
    public string reg_area { get; set; }
    public string reg_houseno { get; set; }
    public string reg_street { get; set; }
    public bool reg_toggle { get; set; }
    public string address { get; set; }
    public string communi_pincode { get; set; }
    public string communi_state { get; set; }
    public string communi_city { get; set; }
    public string communi_area { get; set; }
    public string communi_houseno { get; set; }
    public string communi_street { get; set; }
    public string commaddress { get; set; }
    public string nominee_name { get; set; }
    public string nominee_relationship { get; set; }
    public string hypothecated { get; set; }
    public string financier_details { get; set; }
    public string financieraddress { get; set; }
    public string contract_no { get; set; }
    public string sel_idv { get; set; }
    public decimal idv_input { get; set; }
    public string product_id { get; set; }
    public string proposal_id { get; set; }
    public string quote_id { get; set; }
    public string sel_policy_type { get; set; }
    public string agree_checbox { get; set; }
    public string city_of_reg { get; set; }
    public string prev_exp_date_comp { get; set; }
    public bool no_previous_insurer_chk { get; set; }
    public string no_prev_ins { get; set; }
    public string user_type { get; set; }
    public string employee_id { get; set; }
    public string branch_code_sol_id { get; set; }
    public string cust_mobile { get; set; }
    public string account_no { get; set; }
    public string enach_reg { get; set; }
    public string utm_details { get; set; }
    public string utm_source { get; set; }
    public string utm_medium { get; set; }
    public string utm_campaign { get; set; }
    public string utm_content { get; set; }
    public string utm_term { get; set; }
    public string covid19_addon { get; set; }
    public string covid19_dcb_addon { get; set; }
    public string covid19_dcb_benefit { get; set; }
    public string covid19_lossofjob_addon { get; set; }
    public string IMDShortcode_Dev { get; set; }
    public string emp_code { get; set; }
    public string sol_id { get; set; }
    public string seo_master_availability { get; set; }
    public string d2cmodel_master_availability { get; set; }
    public string b2brto_master_availability { get; set; }
    public string d2crto_master_availability { get; set; }
    public string seo_vehicle_type { get; set; }
    public string seo_policy_type { get; set; }
    public string seo_preferred_time { get; set; }
    public string ncb_protect_app { get; set; }
    public string vehicle_replacement_advantage_app { get; set; }
    public string reinstatement_value_basis { get; set; }
    public string daily_cash_allowance { get; set; }
    public string sel_allowance { get; set; }
    public string monthly_installment_cover { get; set; }
    public int emi_entered { get; set; }
    public int sel_time_excess { get; set; }
    public string hydrostatic_lock_cover_app { get; set; }
    public string return_to_invoice { get; set; }
    public int registrationcost { get; set; }
    public int roadtaxpaid { get; set; }
    public string chola_value_added_services { get; set; }
    public string Plan_1 { get; set; }
    public string pa_lt_dropdown { get; set; }
    public string tp_rsd { get; set; }
    public string tp_red { get; set; }
    public string od_rsd { get; set; }
    public string od_red { get; set; }
    public string paid_driver_opted { get; set; }
    public string pc_cvas_cover { get; set; }
    public int no_of_unnamed { get; set; }
    public string consumables_cover_app { get; set; }
    public string key_replacement_cover_app { get; set; }
    public string personal_belonging_cover_app { get; set; }
    public string rsa_cover_app { get; set; }
    public string cng_lpg_app { get; set; }
    public string cng_lpg_value { get; set; }
    public string externally_fitted_cng_lpg_opted { get; set; }
    public string non_elec_acc_app { get; set; }
    public string elec_acc_app { get; set; }
    public string externally_fitted_cng_lpg_idv { get; set; }
    public string externally_fitted_cng_lpg_min_idv { get; set; }
    public string externally_fitted_cng_lpg_max_idv { get; set; }
    public string non_elec_acc_idv { get; set; }
    public string non_elec_acc_max_idv { get; set; }
    public string non_elec_acc_desc { get; set; }
    public string elec_acc_idv { get; set; }
    public string elec_acc_max_idv { get; set; }
    public string elec_acc_desc { get; set; }
    public string non_elec_acc_type_1 { get; set; }
    public string non_elec_acc_value_1 { get; set; }
    public string non_elec_acc_type_2 { get; set; }
    public string non_elec_acc_value_2 { get; set; }
    public string non_elec_acc_type_3 { get; set; }
    public string non_elec_acc_value_3 { get; set; }
    public string non_elec_acc_type_4 { get; set; }
    public string non_elec_acc_value_4 { get; set; }
    public string non_elec_acc_type_5 { get; set; }
    public string non_elec_acc_value_5 { get; set; }
    public string non_elec_acc_type_6 { get; set; }
    public string non_elec_acc_value_6 { get; set; }
    public string elec_acc_type_1 { get; set; }
    public string elec_acc_value_1 { get; set; }
    public string elec_acc_type_2 { get; set; }
    public string elec_acc_value_2 { get; set; }
    public string elec_acc_type_3 { get; set; }
    public string elec_acc_value_3 { get; set; }
    public string elec_acc_type_4 { get; set; }
    public string elec_acc_value_4 { get; set; }
    public string elec_acc_type_5 { get; set; }
    public string elec_acc_value_5 { get; set; }
    public string elec_acc_type_6 { get; set; }
    public string elec_acc_value_6 { get; set; }
}
