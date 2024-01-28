namespace API.Utils
{
    public class Password
    {
        public static string Hash(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hashedPassword;
        }

        public static bool Verify(string password, string hashedPassword)
        {
            bool passwordMatches = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

            return passwordMatches;
        }
    }
}
