// <copyright file="GlobalSuppressions.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "It is safe to suppress a warning from this rule when the interface is used to identify a set of types at compile time.", Scope = "type", Target = "~T:RPThreadTrackerV3.Interfaces.Data.IDocument")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "It is safe to suppress a warning from this rule when the interface is used to identify a set of types at compile time.", Scope = "type", Target = "~T:RPThreadTrackerV3.Interfaces.Data.IEntity")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Program cannot be static.", Scope = "type", Target = "~T:RPThreadTrackerV3.Program")]
