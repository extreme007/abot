using System;
using System.Collections.Generic;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class ArticleCategory
    {
        public ArticleCategory()
        {
            Articles = new HashSet<Article>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int? ParentId { get; set; }
        public int? Order { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}
