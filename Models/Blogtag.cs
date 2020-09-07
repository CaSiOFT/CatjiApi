using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Blogtag
    {
        public int Bid { get; set; }
        public int TagId { get; set; }

        public Blog B { get; set; }
        public Tag Tag { get; set; }
    }
}
