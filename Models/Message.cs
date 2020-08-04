using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Message
    {
        public int Mid { get; set; }
        public int Usid { get; set; }
        public int ToUsid { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }

        public Users ToUs { get; set; }
        public Users Us { get; set; }
    }
}
