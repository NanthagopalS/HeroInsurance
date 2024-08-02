using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.ICICI
{
    public class ICICIIdvResponseModel
{   
        public string vehiclemodelstatus { get; set; }
        public float vehicleage { get; set; }
        public float idvdepreciationpercent { get; set; }
        public float minexshowroomdeviationlimit { get; set; }
        public float maxexshowroomdeviationlimit { get; set; }
        public float maximumprice { get; set; }
        public float minimumprice { get; set; }
        public float maxidv { get; set; }
        public float minidv { get; set; }
        public float vehiclesellingprice { get; set; }
        public bool status { get; set; }
        public string statusmessage { get; set; }
        public object errorMessage { get; set; }
        public string correlationId { get; set; }
        public int StatusCode { get; set; }
    }
}
