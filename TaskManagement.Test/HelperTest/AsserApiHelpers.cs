﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Test.HelperTest
{
    /// <summary>
    /// Create Testing reusable result
    /// </summary>
    public class AsserApiHelpers
    {
        /// <summary>
        /// Reusable Result for all testing if Invalid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="expectedMessage"></param>
        /// <param name="expectedError"></param>
        public static void AsserApiError<T>(ResponseType<T> result, string expectedMessage, string expectedError)
        {
            Assert.False(result.Success);
            Assert.Equal(expectedMessage, result.Message);
            Assert.Contains(expectedError, result.Errors);
        }
        /// <summary>
        /// Return Valid Request Result 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="expectedMessage"></param>
        public static void AsserApiSuccess<T>(ResponseType<T> result, string expectedMessage)
        {
            Assert.True(result.Success);
            Assert.Equal(expectedMessage, result.Message);
        }
        public static void AsserApiSuccess<T>(List<ResponseType<T>> result, string expectedMessage)
        {
            // Ensure all responses in the list are successful
            Assert.All(result, res => Assert.True(res.Success));

            // Ensure all messages in the list match the expected message
            Assert.All(result, res => Assert.Equal(expectedMessage, res.Message));
        }
    }
}
