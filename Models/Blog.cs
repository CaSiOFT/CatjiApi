using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Blog
    {
        public Blog()
        {
            Blogcomment = new HashSet<Blogcomment>();
            Likeblog = new HashSet<Likeblog>();
            Reportblog = new HashSet<Reportblog>();
        }

        public int Bid { get; set; }
        public string Content { get; set; }
        public int Usid { get; set; }
        public DateTime CreateTime { get; set; }
        public int LikeNum { get; set; }
        public decimal? IsPublic { get; set; }

        public Users Us { get; set; }
        public ICollection<Blogcomment> Blogcomment { get; set; }
        public ICollection<Likeblog> Likeblog { get; set; }
        public ICollection<Reportblog> Reportblog { get; set; }
    }
}
