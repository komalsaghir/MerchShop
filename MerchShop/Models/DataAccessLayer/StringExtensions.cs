using System.Text;

namespace MerchShop.Models
{
    public static class StringExtensions
    {
        public static string Slug(this string s)
        {
            var sb = new StringBuilder();
            foreach(char c in s)
            {
                if(!char.IsPunctuation(c) || c =='-')
                {
                    sb.Append(c);   
                }
            }
            return sb.ToString().Replace(" ", "-");
        }
        public static bool EqualsNoCase(this string str, string toCompare) => str?.ToLower() == toCompare?.ToLower();

        public static int ToInt(this string str)
        {
            int.TryParse(str, out int value); //returns 0 if no value found
            return value;
        }

        public static string Captialize(this string str) => str?[..1]?.ToUpper() + str?.Substring(1).ToLower();
        
    }
}
