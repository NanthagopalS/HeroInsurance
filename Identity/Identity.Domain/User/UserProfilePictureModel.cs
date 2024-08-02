using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User
{
    public class UserProfilePictureModel
    {        
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }


        /// <summary>
        /// ProfilePictureID
        /// </summary>
        public string ProfilePictureID { get; set; }

        /// <summary>
        /// ProfilePictureFileName
        /// </summary>
        public string ProfilePictureFileName { get; set; }

        /// <summary>
        /// ProfilePictureStoragePath
        /// </summary>
        public string ProfilePictureStoragePath { get; set; }


        /// <summary>
        /// ImageStream
        /// </summary>
        public byte[] ImageStream { get; set; }

        public string DocumentId { get; set; }

        public string Image64 { get; set; }
    }
}
