using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Tag
    {
        public Tag()
        {
            Videotag = new HashSet<Videotag>();
        }

        public int TagId { get; set; }
        public string Name { get; set; }
        public int? CatId { get; set; }

        public Cat Cat { get; set; }
        public ICollection<Videotag> Videotag { get; set; }
    }
}
