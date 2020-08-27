using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public class Users
    {
        public int Usid { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string Avatar { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? Birthday { get; set; }
        public decimal? IsBanned { get; set; }
        public string Signature { get; set; }
        public int FollowerNum { get; set; }
        public int? CatId { get; set; }
        public DateTime ChangedTime { get; set; }

        public Cat CatNavigation { get; set; }
        public ICollection<Block> BlockBlockUs { get; set; } = new HashSet<Block>();
        public ICollection<Block> BlockUs { get; set; } = new HashSet<Block>();
        public ICollection<Blog> Blog { get; set; } = new HashSet<Blog>();
        public ICollection<Blogcomment> Blogcomment { get; set; } = new HashSet<Blogcomment>();
        public ICollection<Cat> Cat { get; set; } = new HashSet<Cat>();
        public ICollection<Favorite> Favorite { get; set; } = new HashSet<Favorite>();
        public ICollection<Follow> FollowFollowUs { get; set; } = new HashSet<Follow>();
        public ICollection<Follow> FollowUs { get; set; } = new HashSet<Follow>();
        public ICollection<Likeblog> Likeblog { get; set; } = new HashSet<Likeblog>();
        public ICollection<Likeblogcomment> Likeblogcomment { get; set; } = new HashSet<Likeblogcomment>();
        public ICollection<Likevideo> Likevideo { get; set; } = new HashSet<Likevideo>();
        public ICollection<Likevideocomment> Likevideocomment { get; set; } = new HashSet<Likevideocomment>();
        public ICollection<Message> MessageToUs { get; set; } = new HashSet<Message>();
        public ICollection<Message> MessageUs { get; set; } = new HashSet<Message>();
        public ICollection<Reportblog> Reportblog { get; set; } = new HashSet<Reportblog>();
        public ICollection<Reportvideo> Reportvideo { get; set; } = new HashSet<Reportvideo>();
        public ICollection<Searchhistory> Searchhistory { get; set; } = new HashSet<Searchhistory>();
        public ICollection<Video> Video { get; set; } = new HashSet<Video>();
        public ICollection<Videocomment> Videocomment { get; set; } = new HashSet<Videocomment>();
        public ICollection<Watchhistory> Watchhistory { get; set; } = new HashSet<Watchhistory>();
    }
}
