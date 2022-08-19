using System.Text.RegularExpressions;

namespace ImgGoCli.Utils;

public static class RegexUtil
{
    private static readonly Regex ImgRegex = new(@"!\[.*?\]\((.*?)\)", RegexOptions.Compiled);

    public static IEnumerable<string> ExtractorImgFromMarkdown(string content)
    {
        foreach (Match match in ImgRegex.Matches(content))
        {
            yield return match.Groups[1].Value;
        }
    }
}