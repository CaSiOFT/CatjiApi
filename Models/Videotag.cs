using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Videotag
    {
        public int Vid { get; set; }
        public int TagId { get; set; }

        public Tag Tag { get; set; }
        public Video V { get; set; }
    }
}
