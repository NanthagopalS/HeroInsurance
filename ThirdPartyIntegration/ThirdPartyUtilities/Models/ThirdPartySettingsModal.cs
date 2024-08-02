namespace ThirdPartyUtilities.Models
{
    public class ThirdPartySettingsModal
    {
        public PasswordSettings PasswordSettings { get; set; }
    }
    public class PasswordSettings
    {
        public string passPhrase { get; set; }
        public string saltValue { get; set; }
        public int passwordIterations { get; set; }
    }
}
