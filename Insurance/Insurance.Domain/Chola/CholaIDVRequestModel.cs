namespace Insurance.Domain.Chola;

public class CholaIDVRequestModel
{
    public string user_code { get; set; }
    public string make { get; set; }
    public string model_variant { get; set; }
    public string vehicle_model_code { get; set; }
    public string frm_model_variant { get; set; }
    public string rto_location_code { get; set; }
    public string frm_rto { get; set; }
    public decimal ex_show_room { get; set; }
    public string mobile_no { get; set; }
    public int date_of_reg { get; set; }
    public string product_id { get; set; }
    public string sel_policy_type { get; set; }
    public bool no_previous_insurer_chk { get; set; }
    public string no_prev_ins { get; set; }
    public string IMDShortcode_Dev { get; set; }
    public int tp_rsd { get; set; }
    public int tp_red { get; set; }
    public int od_rsd { get; set; }
    public int od_red { get; set; }
}
