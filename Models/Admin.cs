using System;
using System.Collections.Generic;

namespace CatjiApi.Models
{
    public partial class Admin
    {
        public Admin()
        {
            Reportblog = new HashSet<Reportblog>();
            Reportvideo = new HashSet<Reportvideo>();
        }

        public int AdminId { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string Avatar { get; set; }
        public DateTime CreateTime { get; set; }

        public ICollection<Reportblog> Reportblog { get; set; }
        public ICollection<Reportvideo> Reportvideo { get; set; }
    }
}
