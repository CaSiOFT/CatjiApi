using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Video
    {
        public Video()
        {
            Favorite = new HashSet<Favorite>();
            Likevideo = new HashSet<Likevideo>();
            Reportvideo = new HashSet<Reportvideo>();
            Videocomment = new HashSet<Videocomment>();
            Videotag = new HashSet<Videotag>();
            Watchhistory = new HashSet<Watchhistory>();
        }

        public int Vid { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Cover { get; set; }
        public DateTime CreateTime { get; set; }
        public int? Time { get; set; }
        public int Usid { get; set; }
        public int LikeNum { get; set; }
        public int FavoriteNum { get; set; }
        public int WatchNum { get; set; }
        public decimal? IsBanned { get; set; }
        public int CommentNum { get; set; }

        public Users Us { get; set; }
        public ICollection<Favorite> Favorite { get; set; }
        public ICollection<Likevideo> Likevideo { get; set; }
        public ICollection<Reportvideo> Reportvideo { get; set; }
        public ICollection<Videocomment> Videocomment { get; set; }
        public ICollection<Videotag> Videotag { get; set; }
        public ICollection<Watchhistory> Watchhistory { get; set; }
    }
}
