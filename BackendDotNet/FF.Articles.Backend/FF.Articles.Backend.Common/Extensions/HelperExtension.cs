namespace FF.Articles.Backend.Common.Extensions;
public static class HelperExtension
{
    /// <summary>
    /// Normalize a string to lowercase and trim it
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Normalize(this string str)
    {
        return str.ToLower().Trim();
    }
}