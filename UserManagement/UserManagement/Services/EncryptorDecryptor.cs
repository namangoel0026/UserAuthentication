using Microsoft.AspNetCore.Identity;

namespace UserManagement.Services
{
    public static class EncryptorDecryptor
    {
        private static readonly PasswordHasher<object> _passwordHasher = new();

        public static string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(new object(), password);
        }

        public static bool VerifyPassword(string hashedPassword, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(new object(), hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
