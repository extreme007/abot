using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class UmbracoExternalLogin
    {
        public UmbracoExternalLogin()
        {
            UmbracoExternalLoginTokens = new HashSet<UmbracoExternalLoginToken>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserData { get; set; }

        public virtual ICollection<UmbracoExternalLoginToken> UmbracoExternalLoginTokens { get; set; }
    }
}
