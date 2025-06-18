using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Infrastructure.Identity
{
    public class Role :IdentityRole<Guid>
    {
        //provide roles
        public string Description { get; set; } = string.Empty;

        //used for time stamp
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
