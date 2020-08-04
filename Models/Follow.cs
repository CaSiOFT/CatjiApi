using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Follow
    {
        public int Usid { get; set; }
        public int FollowUsid { get; set; }

        public Users FollowUs { get; set; }
        public Users Us { get; set; }
    }
}
