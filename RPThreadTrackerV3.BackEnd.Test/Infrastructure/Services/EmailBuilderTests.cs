// <copyright file="EmailBuilderTests.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Test.Infrastructure.Services
{
    using BackEnd.Infrastructure.Services;
    using FluentAssertions;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using Xunit;

    [Trait("Class", "EmailBuilder")]
    public class EmailBuilderTests
    {
        private readonly EmailBuilder _emailBuilder;
        private readonly Mock<IConfigurationService> _mockConfig;

        public EmailBuilderTests()
        {
            _mockConfig = new Mock<IConfigurationService>();
            _emailBuilder = new EmailBuilder();
        }

        public class BuildForgotPasswordEmail : EmailBuilderTests
        {
            [Fact]
            public void BuildsEmailWithCode()
            {
                // Arrange
                var user = new IdentityUser
                {
                    Email = "test@test.com"
                };
                _mockConfig.SetupGet(x => x.ForgotPasswordEmailFromAddress).Returns("forgotpassword@email.com");

                var dto = _emailBuilder.BuildForgotPasswordEmail(user, "http://www.rpthreadtracker.com", "12345", _mockConfig.Object);

                dto.Subject.Should().Be("RPThreadTracker Password Reset");
                dto.RecipientEmail.Should().Be("test@test.com");
                dto.SenderEmail.Should().Be("forgotpassword@email.com");
                dto.SenderName.Should().Be("RPThreadTracker");
                dto.Body.Should().Be(EmailSnapshotHolder.FORGOT_PASSWORD_HTML_BODY_SNAPSHOT);
                dto.PlainTextBody.Should().Be(EmailSnapshotHolder.FORGOT_PASSWORD_TEXT_BODY_SNAPSHOT);
            }
        }

        public class BuildContactEmail : EmailBuilderTests
        {
            [Fact]
            public void BuildsEmailWithMessage()
            {
                // Arrange
                _mockConfig.SetupGet(x => x.ContactFormEmailToAddress).Returns("contact@email.com");

                var dto = _emailBuilder.BuildContactEmail("user@email.com", "my-username", "This is my message.\r\nThis is the second line.", _mockConfig.Object);

                dto.Subject.Should().Be("RPThreadTracker Contact Form Submission");
                dto.RecipientEmail.Should().Be("contact@email.com");
                dto.SenderEmail.Should().Be("user@email.com");
                dto.SenderName.Should().Be("my-username");
                dto.Body.Should().Be(EmailSnapshotHolder.CONTACT_HTML_BODY_SNAPSHOT);
                dto.PlainTextBody.Should().Be(EmailSnapshotHolder.CONTACT_TEXT_BODY_SNAPSHOT);
            }
        }

        private static class EmailSnapshotHolder
        {
            public const string FORGOT_PASSWORD_HTML_BODY_SNAPSHOT = "<p>Hello,</p><p>You are receiving this message because you requested to reset your password for <a href=\"http://www.rpthreadtracker.com\">rpthreadtracker.com</a>.</p><p>Please use the link below to perform a password reset.</p><p>http://www.rpthreadtracker.com/resetpassword?email=test%40test.com&code=12345</p><p>Thanks, and have a great day!</p><p>~Tracker-mun</p>";

            public const string FORGOT_PASSWORD_TEXT_BODY_SNAPSHOT = "Hello,\n\nYou are receiving this message because you requested to reset your password for rpthreadtracker.com.\n\nPlease use the link below to perform a password reset.\n\nhttp://www.rpthreadtracker.com/resetpassword?email=test%40test.com&code=12345\n\nThanks, and have a great day!\n\n~Tracker-mun";

            public const string CONTACT_HTML_BODY_SNAPSHOT = "<p>You have received a message via RPThreadTracker's contact form:</p>This is my message.<br />This is the second line.";

            public const string CONTACT_TEXT_BODY_SNAPSHOT = "You have received a message via RPThreadTracker's contact form:\nThis is my message.\r\nThis is the second line.";
        }
    }
}
