using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Likevideocomment
    {
        public int Usid { get; set; }
        public int Vcid { get; set; }

        public Users Us { get; set; }
        public Videocomment Vc { get; set; }
    }
}
