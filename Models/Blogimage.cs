using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Blogimage
    {
        public string ImgUrl { get; set; }
        public int Bid { get; set; }

        public Blog B { get; set; }
    }
}
