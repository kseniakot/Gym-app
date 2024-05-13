using Gym.Model;
using System.Security.Cryptography;

namespace WebGym.Services
{
    public class TokenGenerator
    {
        public static string GenerateToken()
        {
            using var rng = new RNGCryptoServiceProvider();
            byte[] tokenData = new byte[32];
            rng.GetBytes(tokenData);
            string token = Convert.ToBase64String(tokenData);
            return token.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public static string HashToken(string token)
        {
            token = token.Replace('-', '+').Replace('_', '/') + new string('=', (4 - token.Length % 4) % 4);
            using var sha256 = SHA256.Create();
            try
            {
                byte[] tokenData = Convert.FromBase64String(token);
                byte[] hashedTokenData = sha256.ComputeHash(tokenData);
                return Convert.ToBase64String(hashedTokenData);
            }
            catch (Exception e)
            {
                throw new Exception("Error hashing token", e);
            }
        }

        public static async Task SaveUserToken(User user, string hashedToken, DBContext db)
        {
            user.ResetToken = hashedToken;
            user.ResetTokenCreationTime = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }
}
