using System.ComponentModel.DataAnnotations;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.ApplicationHelpers
{
    public class ModelValidation
    {
        public static Dictionary<string, List<string>> ModelValidationResponse<T>(T instance)
        {
            var result = new Dictionary<string, List<string>>();

            if (instance == null)
            {
                result.Add("Model", new List<string> { "Model instance is null." });
                return result;
            }

            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(instance);
            Validator.TryValidateObject(instance, context, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    if (!result.ContainsKey(memberName))
                        result[memberName] = new List<string>();

                    result[memberName].Add(validationResult.ErrorMessage ?? "Invalid value.");
                }
            }

            return result;
        }
    }

}
