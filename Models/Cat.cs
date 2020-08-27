using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Cat
    {
        public Cat()
        {
            Tag = new HashSet<Tag>();
            Users = new HashSet<Users>();
        }

        public int CatId { get; set; }
        public int? Usid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Banner { get; set; }

        public Users Us { get; set; }
        public ICollection<Tag> Tag { get; set; }
        public ICollection<Users> Users { get; set; }
    }
}
