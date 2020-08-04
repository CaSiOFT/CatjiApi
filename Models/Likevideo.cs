using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Likevideo
    {
        public int Usid { get; set; }
        public int Vid { get; set; }

        public Users Us { get; set; }
        public Video V { get; set; }
    }
}
