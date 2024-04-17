using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace StudentManagementSystem.Utilities;

/// <summary>
/// Provides cryptographic functions.
/// </summary>
public class Cryptography
{
    private Cryptography() { }

    /// <summary>
    /// Hashes a string using Microsoft's Identity PasswordHasher.
    /// </summary>
    /// 
    /// <param name="original">The original string.</param>
    /// <param name="salt">The salt to use in the hash.</param>
    /// 
    /// <returns>The new hashed string.</returns>
    public static string Hash(string original, string salt)
    {
        PasswordHasher<string> hasher = new();
        return hasher.HashPassword(salt, original);
    }

    /// <summary>
    /// Verifies a password against a hashed password.
    /// </summary>
    /// 
    /// <param name="userId">The user ID to salt the hash.</param>
    /// <param name="hashedPw">The hashed password.</param>
    /// <param name="pw">The password to verify.</param>
    /// 
    /// <returns>True if the password is correct, false otherwise.</returns>
    public static bool Verify(string salt, string hashedPw, string pw)
    {
        PasswordHasher<string> hasher = new();
        return hasher.VerifyHashedPassword(salt, hashedPw, pw) == PasswordVerificationResult.Success;
    }

    /// <summary>
    /// Generates a random 128-bit salt for hashing.
    /// </summary>
    /// 
    /// <returns>The salt.</returns>
    public static string GenerateSalt()
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return Encoding.UTF8.GetString(salt);
    }
}