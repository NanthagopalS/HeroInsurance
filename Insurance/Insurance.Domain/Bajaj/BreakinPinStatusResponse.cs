using Newtonsoft.Json;

namespace Insurance.Domain.Bajaj.BreakinPinStatusXMLResponse
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class EnvBody
    {
        [JsonProperty("m:pinStatusWsResponse")]
        public MPinStatusWsResponse mpinStatusWsResponse { get; set; }
    }

    public class EnvEnvelope
    {
        [JsonProperty("@xmlns:env")]
        public string xmlnsenv { get; set; }

        [JsonProperty("@xmlns:xsi")]
        public string xmlnsxsi { get; set; }

        [JsonProperty("env:Header")]
        public object envHeader { get; set; }

        [JsonProperty("env:Body")]
        public EnvBody envBody { get; set; }
    }

    public class MPinStatusWsResponse
    {
        [JsonProperty("@xmlns:m")]
        public string xmlnsm { get; set; }
        public PPinListOut pPinList_out { get; set; }
        public string pErrorMessage_out { get; set; }
        public string pErrorCode_out { get; set; }
    }

    public class PPinListOut
    {
        [JsonProperty("typ:WeoRecStrings10User")]
        public TypWeoRecStrings10User typWeoRecStrings10User { get; set; }
    }

    public class BreakInPinStatusXMLResponse
    {
        [JsonProperty("env:Envelope")]
        public EnvEnvelope envEnvelope { get; set; }
    }

    public class TypStringval10
    {
        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class TypStringval4
    {
        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class TypStringval7
    {
        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class TypStringval8
    {
        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class TypStringval9
    {
        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class TypWeoRecStrings10User
    {
        [JsonProperty("@xmlns:typ")]
        public string xmlnstyp { get; set; }

        [JsonProperty("typ:stringval2")]
        public string typstringval2 { get; set; }

        [JsonProperty("typ:stringval3")]
        public string typstringval3 { get; set; }

        [JsonProperty("typ:stringval1")]
        public string typstringval1 { get; set; }

        [JsonProperty("typ:stringval6")]
        public string typstringval6 { get; set; }

        [JsonProperty("typ:stringval7")]
        public TypStringval7 typstringval7 { get; set; }

        [JsonProperty("typ:stringval4")]
        public TypStringval4 typstringval4 { get; set; }

        [JsonProperty("typ:stringval5")]
        public string typstringval5 { get; set; }

        [JsonProperty("typ:stringval8")]
        public TypStringval8 typstringval8 { get; set; }

        [JsonProperty("typ:stringval10")]
        public TypStringval10 typstringval10 { get; set; }

        [JsonProperty("typ:stringval9")]
        public TypStringval9 typstringval9 { get; set; }
    }


}
