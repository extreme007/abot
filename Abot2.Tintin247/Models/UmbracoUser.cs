using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoUser
    {
        public UmbracoUser()
        {
            UmbracoContentVersionCultureVariations = new HashSet<UmbracoContentVersionCultureVariation>();
            UmbracoContentVersions = new HashSet<UmbracoContentVersion>();
            UmbracoLogs = new HashSet<UmbracoLog>();
            UmbracoNodes = new HashSet<UmbracoNode>();
            UmbracoUser2NodeNotifies = new HashSet<UmbracoUser2NodeNotify>();
            UmbracoUser2UserGroups = new HashSet<UmbracoUser2UserGroup>();
            UmbracoUserLogins = new HashSet<UmbracoUserLogin>();
            UmbracoUserStartNodes = new HashSet<UmbracoUserStartNode>();
        }

        public int Id { get; set; }
        public bool? UserDisabled { get; set; }
        public bool? UserNoConsole { get; set; }
        public string UserName { get; set; }
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }
        public string PasswordConfig { get; set; }
        public string UserEmail { get; set; }
        public string UserLanguage { get; set; }
        public string SecurityStampToken { get; set; }
        public int? FailedLoginAttempts { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? EmailConfirmedDate { get; set; }
        public DateTime? InvitedDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Avatar { get; set; }
        public string TourData { get; set; }

        public virtual ICollection<UmbracoContentVersionCultureVariation> UmbracoContentVersionCultureVariations { get; set; }
        public virtual ICollection<UmbracoContentVersion> UmbracoContentVersions { get; set; }
        public virtual ICollection<UmbracoLog> UmbracoLogs { get; set; }
        public virtual ICollection<UmbracoNode> UmbracoNodes { get; set; }
        public virtual ICollection<UmbracoUser2NodeNotify> UmbracoUser2NodeNotifies { get; set; }
        public virtual ICollection<UmbracoUser2UserGroup> UmbracoUser2UserGroups { get; set; }
        public virtual ICollection<UmbracoUserLogin> UmbracoUserLogins { get; set; }
        public virtual ICollection<UmbracoUserStartNode> UmbracoUserStartNodes { get; set; }
    }
}
