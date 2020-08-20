using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Searchhistory
    {
        public int Usid { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public int SearchNum { get; set; }

        public Users Us { get; set; }
    }
}
