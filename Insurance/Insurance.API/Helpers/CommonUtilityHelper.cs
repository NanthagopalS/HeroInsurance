using System.Text;

namespace Insurance.API.Helpers
{
    public static class CommonUtilityHelper
    {
        public static List<string> VehicleNumberSplit(string vehicleNumber)
        {
            var words = new List<StringBuilder> { new StringBuilder() };
            for (var i = 0; i < vehicleNumber.Length; i++)
            {
                words[words.Count - 1].Append(vehicleNumber[i]);
                if (i + 1 < vehicleNumber.Length && char.IsLetter(vehicleNumber[i]) != char.IsLetter(vehicleNumber[i + 1]))
                {
                    words.Add(new StringBuilder());
                }
            }
            return words.Select(x => x.ToString()).ToList();
        }
    }
}
