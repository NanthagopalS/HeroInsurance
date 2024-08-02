﻿using Insurance.Domain.GoDigit;

namespace Insurance.Domain.Chola
{

    public class CholaServiceResponseModel
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string RequestId { get; set; }
        public string RequestDateTime { get; set; }
        public ResponseModelData Data { get; set; }
        public QuoteResponseModel quoteResponseModel { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
    }

    public class ResponseModelData 
    {
        public string payment_id { get; set; }
        public int amount { get; set; }
        public int Basic_Own_Damage_CNG_Elec_Non_Elec { get; set; }
        public int Own_Damage { get; set; }
        public int Electrical_Accessory_Prem { get; set; }
        public int Non_Electrical_Accessory_Prem { get; set; }
        public int CNG_LPG_Own_Damage { get; set; }
        public int IMT_Cover_Premium { get; set; }
        public int No_Claim_Bonus { get; set; }
        public string NCB_percentage { get; set; }
        public int DTD_Discounts { get; set; }
        public int DTD_Loading { get; set; }
        public string DTD_percentage { get; set; }
        public int GST_Discounts { get; set; }
        public object GST_percentage { get; set; }
        public int Personal_Accident { get; set; }
        public int Consumables_Cover { get; set; }
        public int Hydrostatic_Lock_Cover { get; set; }
        public int Key_Replacement_Cover { get; set; }
        public int Personal_Belonging_Cover { get; set; }
        public int RSA_Cover { get; set; }
        public int NCB_Protection_Cover { get; set; }
        public int Vehicle_Replacement_Cover { get; set; }
        public int Unnamed_Passenger_Cover { get; set; }
        public int Zero_Depreciation { get; set; }
        public int Own_Damage_Premium_after_Discount { get; set; }
        public int Basic_Third_Party_Premium { get; set; }
        public int CNG_LPG_TP { get; set; }
        public int TPPD_Discount_Premium { get; set; }
        public int Legal_Liability_to_paid_driver { get; set; }
        public int Paid_Coolie_Cleaner_Premium { get; set; }
        public int Final_Reinstatement_Cover_Premium { get; set; }
        public int Final_Daily_Cash_Allowance_Cover_Premium { get; set; }
        public int Final_Monthly_Installment_Cover_Premium { get; set; }
        public int Final_Return_To_Invoice_Cover_Premium { get; set; }
        public int Final_chola_value_added_services_Cover_Premium { get; set; }
        public int Net_Premium { get; set; }
        public int GST { get; set; }
        public int Total_Premium { get; set; }
    }

}
