using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Identity.Models;

namespace TaskManagement.Infrastructures.Data.Seeders
{
    public class IdentitySeeder
    {
         /// <summary>
        /// Seeds predefined roles to the database if they don't exist.
        /// </summary>
        public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            string[] roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                // Check if role exists
                if (!await roleManager.RoleExistsAsync(role))
                {
                    // Create it if not
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }
        }
    }
}
