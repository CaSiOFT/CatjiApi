using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Users
    {
        public Users()
        {
            BlockBlockUs = new HashSet<Block>();
            BlockUs = new HashSet<Block>();
            Blog = new HashSet<Blog>();
            Blogcomment = new HashSet<Blogcomment>();
            Cat = new HashSet<Cat>();
            Favorite = new HashSet<Favorite>();
            FollowFollowUs = new HashSet<Follow>();
            FollowUs = new HashSet<Follow>();
            Likeblog = new HashSet<Likeblog>();
            Likeblogcomment = new HashSet<Likeblogcomment>();
            Likevideo = new HashSet<Likevideo>();
            Likevideocomment = new HashSet<Likevideocomment>();
            MessageToUs = new HashSet<Message>();
            MessageUs = new HashSet<Message>();
            Reportblog = new HashSet<Reportblog>();
            Reportvideo = new HashSet<Reportvideo>();
            Searchhistory = new HashSet<Searchhistory>();
            Video = new HashSet<Video>();
            Videocomment = new HashSet<Videocomment>();
            Watchhistory = new HashSet<Watchhistory>();
        }

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
        public ICollection<Block> BlockBlockUs { get; set; }
        public ICollection<Block> BlockUs { get; set; }
        public ICollection<Blog> Blog { get; set; }
        public ICollection<Blogcomment> Blogcomment { get; set; }
        public ICollection<Cat> Cat { get; set; }
        public ICollection<Favorite> Favorite { get; set; }
        public ICollection<Follow> FollowFollowUs { get; set; }
        public ICollection<Follow> FollowUs { get; set; }
        public ICollection<Likeblog> Likeblog { get; set; }
        public ICollection<Likeblogcomment> Likeblogcomment { get; set; }
        public ICollection<Likevideo> Likevideo { get; set; }
        public ICollection<Likevideocomment> Likevideocomment { get; set; }
        public ICollection<Message> MessageToUs { get; set; }
        public ICollection<Message> MessageUs { get; set; }
        public ICollection<Reportblog> Reportblog { get; set; }
        public ICollection<Reportvideo> Reportvideo { get; set; }
        public ICollection<Searchhistory> Searchhistory { get; set; }
        public ICollection<Video> Video { get; set; }
        public ICollection<Videocomment> Videocomment { get; set; }
        public ICollection<Watchhistory> Watchhistory { get; set; }
    }
}
