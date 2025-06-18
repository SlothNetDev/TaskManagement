using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Domains.Entities;

namespace TaskManagement.Infrastructures.Data
{
    public class TaskManagementDbContext : IdentityDbContext<ApplicationUsers,ApplicationRole,Guid>
    {
    }
}
