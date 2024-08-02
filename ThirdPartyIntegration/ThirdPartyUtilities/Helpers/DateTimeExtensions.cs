using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdPartyUtilities.Helpers
{
    public static class DateTimeExtensions
    {
        public static int ToYear(this TimeSpan span)
        {
            try
            {
                DateTime zeroTime = new DateTime(1, 1, 1);
                return (zeroTime + span).Year;

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int Compare(this DateTime fromDate, DateTime toDate, DateTimeInterval dateTimeInterval)
        {
            try
            {
                if (fromDate > toDate)
                    throw new Exception("From Date should not be greater than To Date");

                var noofmonths = ((toDate.Year - fromDate.Year) * 12) + toDate.Month - fromDate.Month;

                if (dateTimeInterval == DateTimeInterval.Months)
                    return noofmonths;
                else if (dateTimeInterval == DateTimeInterval.Years)
                    return Convert.ToInt32(Math.Round(noofmonths / 12.00));
                else if (dateTimeInterval == DateTimeInterval.Days)
                    return Convert.ToInt32((toDate - fromDate).TotalDays);
            }
            catch (Exception)
            {
                return 0;
            }
            return 0;
        }
        public static DateTime ConvertToDateTime(string dateString)
        {
            string[] formats = { "yyyy-MM-dd", "yyyy/MM/dd", "MM/dd/yyyy", "dd/MM/yyyy", "yyyyMMdd", "dd-MM-yyyy", "dd/MMM/yyyy",
        "MMM/dd/yyyy", "yyyy-MMM-dd", "MMM-dd-yyyy", "dd-MMM-yyyy", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss",
        "dd/MM/yyyy HH:mm:ss", "MMM dd, yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fffZ","MM/dd/yyyy HH:mm:ss" };

            DateTime result;
            if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result))
            {
                return result;
            }
            else
            {
                return DateTime.UtcNow;
            }
        }

    }

    public enum DateTimeInterval
    {
        Months,
        Years,
        Days
    }
}
