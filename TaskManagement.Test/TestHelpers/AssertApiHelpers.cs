using System.Text.Json;
using TaskManagementApi.Domains.Wrapper;
using Xunit;
using Xunit.Abstractions;

namespace TaskManagement.Test.HelperTest
{
    /// <summary>
    /// Enhanced API response assertion helper with comprehensive testing capabilities
    /// </summary>
    public class AssertApiHelpers(ITestOutputHelper output)
    {
        private readonly ITestOutputHelper _output = output;

        #region Core Assertions

        /// <summary>
        /// Asserts that the response indicates success with optional message and data validation
        /// </summary>
        public void ShouldSucceed<T>(ResponseType<T> result, string? expectedMessage = null, T? expectedData = default)
        {
            try
            {
                Assert.True(result.Success, "Expected Success=true but got false.");
                _output.WriteLine($"SUCCESS: {result.Message}");

                if (!string.IsNullOrWhiteSpace(expectedMessage))
                    Assert.Equal(expectedMessage, result.Message);

                if (expectedData != null)
                    Assert.Equal(expectedData, result.Data);
            }
            catch (Exception ex)
            {
                _output.WriteLine($"FAILED ASSERTION:\n{Serialize(result)}\nEXCEPTION: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Asserts that the response indicates failure with optional message and error validation
        /// </summary>
        public void ShouldFail<T>(ResponseType<T> result, string? expectedMessage = null, List<string>? expectedErrors  = null)
        {
            try
            {
                Assert.False(result.Success, "Expected Success=false but got true.");
                _output.WriteLine($"EXPECTED FAILURE: {result.Message}");

                if (!string.IsNullOrWhiteSpace(expectedMessage))
                Assert.Equal(expectedMessage, result.Message);

                if (expectedErrors != null && expectedErrors.Any())
                {
                    Assert.NotNull(result.Errors);
                    // Assert that all expected errors are contained in the result.Errors
                    foreach (var expectedError in expectedErrors)
                    {
                        Assert.Contains(expectedError, result.Errors);
                    }
                    // Optional: If you want to ensure the *exact* set of errors (no more, no less)
                    // Assert.Equal(expectedErrors.OrderBy(e => e), result.Errors.OrderBy(e => e));
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"FAILED ASSERTION:\n{Serialize(result)}\nEXCEPTION: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Collection Assertions

        /// <summary>
        /// Asserts all responses in a collection succeeded
        /// </summary>
        public void ShouldAllSucceed<T>(IEnumerable<ResponseType<T>> results, string? expectedMessage = null)
        {
            foreach (var res in results)
            {
                try
                {
                    ShouldSucceed(res, expectedMessage);
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"FAILED RESPONSE:\n{Serialize(res)}\nEXCEPTION: {ex.Message}");
                    throw;
                }
            }
            _output.WriteLine($"All {results.Count()} responses succeeded.");
        }

        /// <summary>
        /// Asserts a paginated response is valid
        /// </summary>
        public void ShouldBePaginated<T>(ResponseType<PagedList<T>> result, 
            int expectedPage, int expectedPageSize, int? expectedTotalCount = null)
        {
            ShouldSucceed(result);
            Assert.Equal(expectedPage, result.Data.PageNumber);
            Assert.Equal(expectedPageSize, result.Data.PageSize);
            
            if (expectedTotalCount.HasValue)
                Assert.Equal(expectedTotalCount.Value, result.Data.TotalCount);
        }

        #endregion

        #region Data Assertions

        /// <summary>
        /// Asserts response data matches a predicate
        /// </summary>
        public void ShouldMatch<T>(ResponseType<T> result, Func<T, bool> predicate, 
            string reason = "Data didn't match expected condition")
        {
            ShouldSucceed(result);
            Assert.True(predicate(result.Data!), reason);
            _output.WriteLine($"Data matched: {reason}");
        }

        /// <summary>
        /// Asserts response data equals expected value
        /// </summary>
        public void ShouldEqual<T>(ResponseType<T> result, T expected, 
            string? message = null) where T : IEquatable<T>
        {
            ShouldSucceed(result);
            Assert.Equal(expected, result.Data);
            _output.WriteLine(message ?? "Data matched expected value");
        }

        /// <summary>
        /// Asserts response contains specific validation error
        /// </summary>
        public void ShouldHaveValidationError<T>(ResponseType<T> result, 
            string fieldName, string? errorFragment = null)
        {
            ShouldFail(result);
            Assert.NotNull(result.Errors);
            
            var fieldErrors = result.Errors.Where(e => e.Contains(fieldName));
            Assert.True(fieldErrors.Any(), $"No errors found for field '{fieldName}'");

            if (!string.IsNullOrWhiteSpace(errorFragment))
                Assert.True(fieldErrors.Any(e => e.Contains(errorFragment)),
                    $"Field '{fieldName}' errors didn't contain '{errorFragment}'");
        }

        #endregion

        #region Async Support

        /// <summary>
        /// Async version of ShouldSucceed
        /// </summary>
        public async Task ShouldSucceedAsync<T>(Task<ResponseType<T>> taskResult, 
            string? expectedMessage = null)
        {
            var result = await taskResult;
            ShouldSucceed(result, expectedMessage);
        }

        /// <summary>
        /// Async version of ShouldFail
        /// </summary>
        public async Task ShouldFailAsync<T>(Task<ResponseType<T>> taskResult, 
            string? expectedMessage = null, List<string>? expectedError = null)
        {
            var result = await taskResult;
            ShouldFail(result, expectedMessage, expectedError);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Serializes response for logging
        /// </summary>
        private string Serialize<T>(ResponseType<T> result)
        {
            try
            {
                return JsonSerializer.Serialize(result, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            catch
            {
                return "Could not serialize response";
            }
        }

        /// <summary>
        /// Creates a test entity with default values
        /// </summary>
        public T CreateTestEntity<T>(Action<T>? configure = null) where T : new()
        {
            var entity = new T();
            configure?.Invoke(entity);
            return entity;
        }

        #endregion

        #region Specialized Assertions

        /// <summary>
        /// Asserts the response has a not found error
        /// </summary>
        public void ShouldBeNotFound<T>(ResponseType<T> result)
        {
            ShouldFail(result, "Resource not found");
        }

        /// <summary>
        /// Asserts the response has an unauthorized error
        /// </summary>
        public void ShouldBeUnauthorized<T>(ResponseType<T> result)
        {
            ShouldFail(result, "Unauthorized access");
        }

        /// <summary>
        /// Asserts the response has a validation error
        /// </summary>
        public void ShouldBeValidationError<T>(ResponseType<T> result)
        {
            ShouldFail(result, "Validation failed");
            Assert.NotNull(result.Errors);
            Assert.NotEmpty(result.Errors);
        }

        #endregion
    }

    // Supporting classes
    public class PagedList<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; } = new();
    }
}