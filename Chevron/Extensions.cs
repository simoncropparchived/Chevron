namespace Chevron
{
    static class Extensions
    {
        public static string AsJs(this bool b)
        {
            return b.ToString().ToLower();
        }
    }
}