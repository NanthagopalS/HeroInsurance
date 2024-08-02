using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Authentication
{
    public class AuthenticationAdminResponse
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }

        public string EmailId { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// RoleId
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// RoleName
        /// </summary>
        public string RoleName { get; set; }

    }

    public class TestMobilePAN
    {
        public bool IsExists { get; set; }
    }
}
