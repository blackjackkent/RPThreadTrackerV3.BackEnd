// <copyright file="BaseController.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Controllers
{
	using System.Linq;
	using System.Security.Claims;
	using Infrastructure.Providers;
	using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Base controller class from which all API controllers inherit.
    /// Provides hook for <see cref="GlobalExceptionHandlerAttribute"/>
    /// and <see cref="DisableDuringMaintenanceFilterAttribute"/>
    /// </summary>
    /// <seealso cref="Controller" />
    [ServiceFilter(typeof(GlobalExceptionHandlerAttribute))]
    [ServiceFilter(typeof(DisableDuringMaintenanceFilterAttribute))]
	public class BaseController : Controller
	{
        /// <summary>
        /// Gets the ID of the currently logged in user, if applicable
        /// </summary>
        /// <value>
        /// The ID of the currently logged in user, or <c>null</c> if no user is logged in.
        /// </value>
        protected string UserId => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
	}
}
