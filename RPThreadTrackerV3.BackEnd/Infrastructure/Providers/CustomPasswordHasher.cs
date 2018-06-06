// <copyright file="CustomPasswordHasher.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Providers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Custom class to check passwords for users whose accounts have not been migrated from the
    /// System.Web.Helpers.Crypto hashing system.
    /// </summary>
    /// <seealso cref="PasswordHasher{IdentityUser}" />
    public class CustomPasswordHasher : PasswordHasher<IdentityUser>
	{
        /// <summary>
        /// Returns a <see cref="PasswordVerificationResult" /> indicating the result of a password hash
        /// comparison. Indicates rehash needed if user's account has not been migrated from the
        /// System.Web.Helpers.Crypto hashing system.
        /// </summary>
        /// <param name="user">The user whose password should be verified.</param>
        /// <param name="hashedPassword">The hash value for a user's stored password.</param>
        /// <param name="providedPassword">The password supplied for comparison.</param>
        /// <returns>
        /// A <see cref="PasswordVerificationResult" /> indicating the result of a password hash comparison.
        /// </returns>
        /// <remarks>
        /// Implementations of this method should be time consistent.
        /// </remarks>
        public override PasswordVerificationResult VerifyHashedPassword(IdentityUser user, string hashedPassword, string providedPassword)
		{
			var isValidPasswordWithLegacyHash = VerifyHashedPassword(hashedPassword, providedPassword);
			return isValidPasswordWithLegacyHash
				? PasswordVerificationResult.SuccessRehashNeeded
				: base.VerifyHashedPassword(user, hashedPassword, providedPassword);
		}

		private const int PBKDF2_ITER_COUNT = 1000;
		private const int PBKDF2_SUBKEY_LENGTH = 256 / 8;
		private const int SALT_SIZE = 128 / 8;

        /// <summary>
        /// Verifies the hashed password.
        /// </summary>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="password">The password.</param>
        /// <returns><c>true</c> if password is valid, otherwise <c>false</c>.</returns>
        public static bool VerifyHashedPassword(string hashedPassword, string password)
		{
			// Checks password using legacy hashing from System.Web.Helpers.Crypto
			var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);
			if (hashedPasswordBytes.Length != (1 + SALT_SIZE + PBKDF2_SUBKEY_LENGTH) || hashedPasswordBytes[0] != 0x00)
			{
				return false;
			}
			var salt = new byte[SALT_SIZE];
			Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SALT_SIZE);
			var storedSubkey = new byte[PBKDF2_SUBKEY_LENGTH];
			Buffer.BlockCopy(hashedPasswordBytes, 1 + SALT_SIZE, storedSubkey, 0, PBKDF2_SUBKEY_LENGTH);
			byte[] generatedSubkey;
			using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, PBKDF2_ITER_COUNT))
			{
				generatedSubkey = deriveBytes.GetBytes(PBKDF2_SUBKEY_LENGTH);
			}
			return ByteArraysEqual(storedSubkey, generatedSubkey);
		}

		private static string BinaryToHex(byte[] data)
		{
			var hex = new char[data.Length * 2];
			for (var iter = 0; iter < data.Length; iter++)
			{
				var hexChar = (byte)(data[iter] >> 4);
				hex[iter * 2] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
				hexChar = (byte)(data[iter] & 0xF);
				hex[(iter * 2) + 1] = (char)(hexChar > 9 ? hexChar + 0x37 : hexChar + 0x30);
			}
			return new string(hex);
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		private static bool ByteArraysEqual(byte[] a, byte[] b)
		{
			if (ReferenceEquals(a, b))
			{
				return true;
			}
			if (a == null || b == null || a.Length != b.Length)
			{
				return false;
			}
			var areSame = true;
			for (var i = 0; i < a.Length; i++)
			{
				areSame &= a[i] == b[i];
			}
			return areSame;
		}
	}
}
