using System;
using System.Linq;
using System.Text;

public static class StringExtensions
{
    // Thanks StackOverflow
    
    public static string ToSnakeCase(this string text)
    {
        if(text == null) {
            throw new ArgumentNullException(nameof(text));
        }
        if(text.Length < 2) {
            return text.ToLowerInvariant();
        }
        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));
        for(int i = 1; i < text.Length; ++i) {
            char c = text[i];
            if(char.IsUpper(c)) {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            } else {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    public static string ToUrl(this string text)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));

        return text
            .Aggregate("", (current, c) => current + (c is ' '
                ? '_'
                : c is '\''
                    ? ""
                    : c.ToString().ToLowerInvariant()));
    }
}