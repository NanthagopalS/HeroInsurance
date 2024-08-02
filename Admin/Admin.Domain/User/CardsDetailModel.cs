using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class CardsDetailResponseModel
    {
        public IEnumerable<UserCardModel>? userCardModel { get; set; }
        public IEnumerable<ContactCardModel>? contactCardModel { get; set; }
        public IEnumerable<SupportSectionModel>? supportSectionModel { get; set; }

    }

    public class UserCardModel
    {
        //It's hard coded in previous section
        public string? Image { get; set; }
        public string? AgentId { get; set; }
        public string? CardType { get; set; }
        public string? CollectedPremium { get; set; }
    }

    public class ContactCardModel
    {
        public string? RoleLevelName { get; set; }
        public string? RoleName { get; set; }
        public string? RoleTypeID { get; set; }
        public string? Position { get; set; }
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? EmailId { get; set; }
        public string? MobileNo { get; set; }

    }

    public class SupportSectionModel
    {
        public string? ConfigurationKey { get; set; }
        public string? Configurationvalue { get; set; }
    }

}
