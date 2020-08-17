using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Blogcomment
    {
        public Blogcomment()
        {
            InverseParentBc = new HashSet<Blogcomment>();
            InverseReplyBc = new HashSet<Blogcomment>();
            Likeblogcomment = new HashSet<Likeblogcomment>();
        }

        public int Bcid { get; set; }
        public int Usid { get; set; }
        public int Bid { get; set; }
        public string Content { get; set; }
        public int? ReplyBcid { get; set; }
        public DateTime CreateTime { get; set; }
        public int LikeNum { get; set; }
        public int? ParentBcid { get; set; }

        public Blog B { get; set; }
        public Blogcomment ParentBc { get; set; }
        public Blogcomment ReplyBc { get; set; }
        public Users Us { get; set; }
        public ICollection<Blogcomment> InverseParentBc { get; set; }
        public ICollection<Blogcomment> InverseReplyBc { get; set; }
        public ICollection<Likeblogcomment> Likeblogcomment { get; set; }
    }
}
