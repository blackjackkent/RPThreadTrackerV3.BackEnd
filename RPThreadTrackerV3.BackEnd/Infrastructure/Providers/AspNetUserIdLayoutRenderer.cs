// <copyright file="AspNetUserIdLayoutRenderer.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Providers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using NLog;
    using NLog.LayoutRenderers;
    using NLog.Web.LayoutRenderers;

    /// <summary>
    /// NLog layout renderer for logging the session user ID associated with a log message.
    /// </summary>
    [LayoutRenderer("aspnet-user-id")]
    [ExcludeFromCodeCoverage]
    public class AspNetUserIdLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc />
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContextAccessor.HttpContext;

            if (context.User?.Claims == null || !context.User.Claims.Any())
            {
                return;
            }

            builder.Append(context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
