using System.ComponentModel.DataAnnotations;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.ApplicationHelpers
{
    public class ModelValidation
    {
        public static List<string?> ModelValidationResponse<T>(T instance)
        {
            ResponseType<T> response = new();
            if (instance == null)
            {
                response.Success = false;
                response.Message = "Models are null";
                response.Errors?.Add("Models Cannot be Null");
                return response.Errors ?? new List<string>() {"error"};
            }
            List<ValidationResult> validationResults = new ();

            var validationContext = new ValidationContext(instance);
            Validator.TryValidateObject(instance, validationContext, validationResults, true);
            return validationResults.Select(x => x.ErrorMessage).ToList();

        }
    }
}
