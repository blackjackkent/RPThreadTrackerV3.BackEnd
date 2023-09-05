// <copyright file="ExceptionBuilder.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.TestHelpers
{
    using System;
    using System.Net;
    using System.Reflection;
    using Microsoft.Azure.Cosmos;

    public class ExceptionBuilder
    {
        public static CosmosException BuildDocumentClientException(Exception error, HttpStatusCode httpStatusCode)
        {
            return new CosmosException(error.Message, httpStatusCode, 0, string.Empty, 0);

        }
    }
}
