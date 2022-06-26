namespace RazorToStringServices.Attributes
{
    /// <summary>
    /// use this on Razor pages that you want to work as email content but not allow view directly in the browser
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AuthorizeEmailAttribute : Attribute
    {
    }
}
