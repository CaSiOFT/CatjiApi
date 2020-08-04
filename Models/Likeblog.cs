using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Likeblog
    {
        public int Usid { get; set; }
        public int Bid { get; set; }

        public Blog B { get; set; }
        public Users Us { get; set; }
    }
}
