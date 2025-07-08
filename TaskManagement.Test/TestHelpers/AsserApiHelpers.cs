using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Domains.Wrapper;
using Xunit.Abstractions;

namespace TaskManagement.Test.HelperTest
{
    /// <summary>
    /// Create Testing reusable result
    /// </summary>
    using Xunit.Abstractions;

    public class AsserApiHelpers(ITestOutputHelper _output)
    {
        /// <summary>
        /// Asserts that the response indicates success, optionally checks the message and data.
        /// </summary>
        /// <typeparam name="T">Type of the response data.</typeparam>
        /// <param name="result">The response to assert.</param>
        /// <param name="expectedMessage">Optional expected message.</param>
        /// <param name="expectedData">Optional expected data.</param>
        public void ShouldSucceed<T>(ResponseType<T> result, string expectedMessage = null, T expectedData = default)
        {
            try
            {
                Assert.True(result.Success, "Expected Success=true but got false.");
                _output.WriteLine($"Success: {result.Message}");
    
                if (!string.IsNullOrWhiteSpace(expectedMessage))
                    Assert.Equal(expectedMessage, result.Message);
    
                if (expectedData is not null)
                    Assert.Equal(expectedData, result.Data);
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Failed Assertion in ShouldSucceed:\n{Serialize(result)}\nException: {ex.Message}");
                throw;
            }
        }
    
        /// <summary>
        /// Asserts that the response indicates failure, optionally checks the message and error.
        /// </summary>
        /// <typeparam name="T">Type of the response data.</typeparam>
        /// <param name="result">The response to assert.</param>
        /// <param name="expectedMessage">Optional expected message.</param>
        /// <param name="expectedError">Optional expected error string to be found in Errors.</param>
        public void ShouldFail<T>(ResponseType<T> result, string expectedMessage = null, string expectedError = null)
        {
            try
            {
                Assert.False(result.Success, "Expected Success=false but got true.");
                _output.WriteLine($"Failure (Expected): {result.Message}");
    
                if (!string.IsNullOrWhiteSpace(expectedMessage))
                    Assert.Equal(expectedMessage, result.Message);
    
                if (!string.IsNullOrWhiteSpace(expectedError))
                {
                    Assert.NotNull(result.Errors);
                    Assert.Contains(expectedError, result.Errors);
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Failed Assertion in ShouldFail:\n{Serialize(result)}\nException: {ex.Message}");
                throw;
            }
        }
    
        /// <summary>
        /// Asserts that all responses in the collection indicate success, optionally checks the message.
        /// </summary>
        /// <typeparam name="T">Type of the response data.</typeparam>
        /// <param name="results">The collection of responses to assert.</param>
        /// <param name="expectedMessage">Optional expected message for all responses.</param>
        public void ShouldAllSucceed<T>(IEnumerable<ResponseType<T>> results, string expectedMessage = null)
        {
            foreach (var res in results)
            {
                try
                {
                    Assert.True(res.Success, "One or more responses failed.");
                    if (!string.IsNullOrWhiteSpace(expectedMessage))
                        Assert.Equal(expectedMessage, res.Message);
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"One of the responses failed:\n{Serialize(res)}\nException: {ex.Message}");
                    throw;
                }
            }
    
            _output.WriteLine($"All {results.Count()} responses succeeded.");
        }
    
        /// <summary>
        /// Asserts that the response data matches a given predicate.
        /// </summary>
        /// <typeparam name="T">Type of the response data.</typeparam>
        /// <param name="result">The response to assert.</param>
        /// <param name="predicate">Predicate to test the data.</param>
        /// <param name="reason">Reason message if the predicate fails.</param>
        public void ShouldMatch<T>(ResponseType<T> result, Func<T, bool> predicate, string reason = "Data didn't match the expected condition.")
        {
            try
            {
                Assert.True(result.Data is not null && predicate(result.Data), reason);
                _output.WriteLine($"Data matched expected condition.");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Failed match:\n{Serialize(result)}\nReason: {reason}\nException: {ex.Message}");
                throw;
            }
        }
    
        /// <summary>
        /// Serializes the response to a JSON string for output/logging.
        /// </summary>
        /// <typeparam name="T">Type of the response data.</typeparam>
        /// <param name="result">The response to serialize.</param>
        /// <returns>JSON string representation of the response.</returns>
        private string Serialize<T>(ResponseType<T> result)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch
            {
                return "Could not serialize response.";
            }
        }
    }



}
