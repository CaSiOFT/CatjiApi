using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Likeblogcomment
    {
        public int Usid { get; set; }
        public int Bcid { get; set; }

        public Blogcomment Bc { get; set; }
        public Users Us { get; set; }
    }
}
