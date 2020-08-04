using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Reportvideo
    {
        public int Vrid { get; set; }
        public int Usid { get; set; }
        public int Vid { get; set; }
        public string Reason { get; set; }
        public DateTime CreateTime { get; set; }
        public string Reply { get; set; }
        public int? Admin { get; set; }
        public DateTime? ReplyTime { get; set; }

        public Admin AdminNavigation { get; set; }
        public Users Us { get; set; }
        public Video V { get; set; }
    }
}
