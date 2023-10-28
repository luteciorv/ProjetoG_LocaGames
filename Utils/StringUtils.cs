using System.Text.RegularExpressions;

namespace ProjetoG_LocaGames.Utils
{
    public static class StringUtils
    {
        public static bool IsPasswordStrong(string password)
        {
            string pattern = "^(?=.*[A-Za-z])(?=.*\\d)(?=.*[$!%*@@#?&])[A-Za-z\\d$!%*@#?&]";
            return Regex.IsMatch(password, pattern);
        }
    }
}
