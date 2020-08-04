using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Favorite
    {
        public int Usid { get; set; }
        public int Vid { get; set; }

        public Users Us { get; set; }
        public Video V { get; set; }
    }
}
