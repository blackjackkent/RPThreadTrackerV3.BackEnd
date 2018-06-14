// <copyright file="ExceptionBuilder.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.TestHelpers
{
    using System.Net;
    using System.Net.Http.Headers;
    using System.Reflection;
    using Microsoft.Azure.Documents;

    public class ExceptionBuilder
    {
        public static DocumentClientException BuildDocumentClientException(Error error, HttpStatusCode httpStatusCode)
        {
            var type = typeof(DocumentClientException);

            var documentClientExceptionInstance = type.Assembly.CreateInstance(
                type.FullName,
                false,
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { error, (HttpResponseHeaders)null, httpStatusCode },
                null,
                null);

            return (DocumentClientException)documentClientExceptionInstance;
        }
    }
}
