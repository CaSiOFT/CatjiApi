using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Videocomment
    {
        public Videocomment()
        {
            InverseParentVc = new HashSet<Videocomment>();
            InverseReplyVc = new HashSet<Videocomment>();
            Likevideocomment = new HashSet<Likevideocomment>();
        }

        public int Vcid { get; set; }
        public int Usid { get; set; }
        public int Vid { get; set; }
        public string Content { get; set; }
        public int? ReplyVcid { get; set; }
        public DateTime CreateTime { get; set; }
        public int LikeNum { get; set; }
        public int? ParentVcid { get; set; }

        public Videocomment ParentVc { get; set; }
        public Videocomment ReplyVc { get; set; }
        public Users Us { get; set; }
        public Video V { get; set; }
        public ICollection<Videocomment> InverseParentVc { get; set; }
        public ICollection<Videocomment> InverseReplyVc { get; set; }
        public ICollection<Likevideocomment> Likevideocomment { get; set; }
    }
}
