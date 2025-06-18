using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Domains.Entities
{
    public class Users
    {
        [Key]
        public Guid Id { get; set; }
       
        //Audit when it was created and when it was updated
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

         // Navigation properties to other domain entities  that this user owns related to it
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
