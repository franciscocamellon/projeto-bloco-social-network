using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Domain.Entities
{
    public class UserPhotos
    {
        public int Id { get; set; }
        public string UserPhoto { get; set; }
        public DateTime AddingDate { get; set; }
        public Profile Profile { get; set; }
    }
}
