using Newtonsoft.Json;

namespace Identity.Domain.User
{
    public class ResetPasswordResponseModel
    {
        [JsonIgnore]
        public string UserId { get; set; }

        [JsonIgnore]
        public bool IsValidate { get; set; }
        public string Message { get; set; }

    }
}
