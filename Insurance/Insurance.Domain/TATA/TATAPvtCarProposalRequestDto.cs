using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.TATA
{
    public class TATAPvtCarProposalRequestDto
    {
        public string proposer_salutation { get; set; }
        public string proposer_fname { get; set; }
        public string proposer_lname { get; set; }
        public string proposer_dob { get; set; }
        public string proposer_gender { get; set; }
        public string proposer_mobile { get; set; }
        public string proposer_email { get; set; }
        public string proposer_marital { get; set; }
        public string proposer_occupation { get; set; }
        public string proposer_pan { get; set; }
        public string proposer_add1 { get; set; }
        public string proposer_add2 { get; set; }
        public string proposer_add3 { get; set; }
        public string proposer_annual { get; set; }
        public string proposer_gstin { get; set; }
        public string vehicle_puc_expiry { get; set; }
        public string vehicle_puc { get; set; }
        public string vehicle_puc_declaration { get; set; }
        public string pre_insurer_name { get; set; }
        public string pre_insurer_no { get; set; }
        public string pre_insurer_address { get; set; }
        public string financier_type { get; set; }
        public string financier_name { get; set; }
        public string financier_address { get; set; }
        public string nominee_name { get; set; }
        public string nominee_relation { get; set; }
        public int nominee_age { get; set; }
        public string appointee_name { get; set; }
        public string appointee_relation { get; set; }
        public string product_id { get; set; }
        public string declaration { get; set; }
        public string vehicle_chassis { get; set; }
        public string vehicle_engine { get; set; }
        public string proposer_fullname { get; set; }
        public string proposer_pincode { get; set; }
        public string proposal_id { get; set; }
        public string quote_no { get; set; }
        public string carriedOutBy { get; set; }
        public int ble_tp_tenure { get; set; }
        public string ble_tp_type { get; set; }
        public string ble_tp_no { get; set; }
        public string ble_tp_start { get; set; }
        public string ble_tp_end { get; set; }
        public string ble_tp_name { get; set; }
        public string ble_od_start { get; set; }
        public string ble_od_end { get; set; }
        public string ble_saod_prev_no { get; set; }
        public string od_pre_insurer_name { get; set; }
        public string od_pre_insurer_no { get; set; }
        public string od_pre_insurer_address { get; set; }
        public string __finalize { get; set; }

    }
}
