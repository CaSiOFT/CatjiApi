using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Block
    {
        public int Usid { get; set; }
        public int BlockUsid { get; set; }

        public Users BlockUs { get; set; }
        public Users Us { get; set; }
    }
}
