using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.ApplicationHelpers.CostumeValidation
{
    internal class NotDefaultGuidAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is Guid guid)
                return guid != Guid.Empty;
            return false;
        }
    }
}
