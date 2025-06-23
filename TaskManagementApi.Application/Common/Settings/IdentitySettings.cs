using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.Common.Settings
{
    public class IdentitySettings
    {
        public List<string> AdminEmails { get; set; } = new();
    }
}
