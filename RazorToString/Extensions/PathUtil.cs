namespace RazorToStringServices.Extensions
{
    public static class PathUtil
    {
        /// <summary>
        /// ensures ther's a single slash between two path elements
        /// </summary>
        public static string Combine(string path1, string path2) =>
            path1.TrimEnd("/") + "/" + path2.TrimStart("/");
        
        internal static string TrimEnd(this string input, string trim) => input.EndsWith(trim) ? input.Substring(0, input.Length - trim.Length) : input;

        internal static string TrimStart(this string input, string trim) => input.StartsWith(trim) ? input.Substring(trim.Length) : input;
    }
}
