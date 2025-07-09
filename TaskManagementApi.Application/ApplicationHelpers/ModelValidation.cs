using System.ComponentModel.DataAnnotations;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.ApplicationHelpers
{
    public class ModelValidation
    {
        public static List<string?> ModelValidationResponse<T>(T instance)
        {
            if (instance == null)
            {
                return new List<string?> { "Models cannot be null" };
            }
        
            List<ValidationResult> validationResults = new();
            var validationContext = new ValidationContext(instance);
            Validator.TryValidateObject(instance, validationContext, validationResults, validateAllProperties: true);
        
            return validationResults.Select(x => x.ErrorMessage).ToList();
        }

    }
}
