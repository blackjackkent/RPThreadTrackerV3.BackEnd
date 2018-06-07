// <copyright file="RegisterRequestTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Models.RequestModels
{
    using System;
    using BackEnd.Infrastructure.Exceptions.Account;
    using BackEnd.Models.RequestModels;
    using FluentAssertions;
    using Xunit;

    [Trait("Class", "RegisterRequest")]
    public class RegisterRequestTests
    {
        private readonly RegisterRequest _request;

        public RegisterRequestTests()
        {
            _request = new RegisterRequest
            {
                Username = "blackjackkent",
                Email = "rosalind@blackjack-software.com",
                Password = "mypassword",
                ConfirmPassword = "mypassword",
            };
        }

        public class AssertIsValid : RegisterRequestTests
        {
            [Fact]
            public void ThrowsNoErrorWhenValid()
            {
                // Act
                _request.AssertIsValid();
            }

            [Fact]
            public void ThrowsErrorWhenUsernameMissing()
            {
                // Arrange
                _request.Username = string.Empty;

                // Act
                Exception ex = Assert.Throws<InvalidRegistrationException>(() => _request.AssertIsValid());
                ((InvalidRegistrationException)ex).Errors.Should().HaveCount(1);
            }

            [Fact]
            public void ThrowsErrorWhenEmailMissing()
            {
                // Arrange
                _request.Email = string.Empty;

                // Act
                Exception ex = Assert.Throws<InvalidRegistrationException>(() => _request.AssertIsValid());
                ((InvalidRegistrationException)ex).Errors.Should().HaveCount(1);
            }

            [Fact]
            public void ThrowsErrorWhenPasswordAndConfirmPasswordMissing()
            {
                // Arrange
                _request.Password = string.Empty;
                _request.ConfirmPassword = string.Empty;

                // Act
                Exception ex = Assert.Throws<InvalidRegistrationException>(() => _request.AssertIsValid());
                ((InvalidRegistrationException)ex).Errors.Should().HaveCount(1);
            }

            [Fact]
            public void ThrowsErrorWhenPasswordsDoNotMatch()
            {
                // Arrange
                _request.Password = "blah";
                _request.ConfirmPassword = "blah2";

                // Act
                Exception ex = Assert.Throws<InvalidRegistrationException>(() => _request.AssertIsValid());
                ((InvalidRegistrationException)ex).Errors.Should().HaveCount(1);
            }
        }
    }
}
