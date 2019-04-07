namespace Exchaggle.Helpers
{
    public static class StringHelper
    {
        public static string LimitText(this string str, int charallowed)
        {
            if (str.Length > charallowed)
            {
                return str.Substring(0, charallowed) + " ...";
            }
            return str;
        }
    }
}