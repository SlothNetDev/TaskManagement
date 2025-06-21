using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Infrastructures.Data
{
    internal class TaskManagementDbContextFactory : IDesignTimeDbContextFactory<TaskManagementDbContext>
    {
        public TaskManagementDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)        // optional for test
            .AddUserSecrets<TaskManagementDbContextFactory>()       // this enables user secrets
            .Build();

            var connectionString = config.GetConnectionString("TaskDbConnection");

            var optionsBuilder = new DbContextOptionsBuilder<TaskManagementDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new TaskManagementDbContext(optionsBuilder.Options);
        }
    }
}
