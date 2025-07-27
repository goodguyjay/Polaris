using System.Text.RegularExpressions;
using Polaris.Core.Document;
using Polaris.Core.Document.Elements;
using Polaris.Core.Document.InlineElements;

namespace Polaris.Core.Parsing;

public sealed partial class PolarSyntaxParser : IPolarSyntaxParser
{
    public PolarDocument Parse(string input)
    {
        var doc = new PolarDocument();
        var lines = input.Split('\n');
        var paragraphLines = new List<string>();
        var listItems = new List<string>();
        var inCodeBlock = false;
        string? codeLang = null;
        var codeLines = new List<string>();

        foreach (var lineRaw in lines)
        {
            var line = lineRaw.TrimEnd('\r');

            if (line.StartsWith("```"))
            {
                FlushParagraph();
                FlushList();

                if (!inCodeBlock)
                {
                    inCodeBlock = true;
                    codeLang = line.Length > 3 ? line[3..].Trim() : null;
                    codeLines.Clear();
                }
                else
                {
                    doc.Blocks.Add(
                        new CodeBlock { Language = codeLang, Code = string.Join('\n', codeLines) }
                    );
                    inCodeBlock = false;
                    codeLang = null;
                }

                continue;
            }

            if (inCodeBlock)
            {
                codeLines.Add(line);
                continue;
            }

            if (string.IsNullOrEmpty(line))
            {
                paragraphLines.Add(string.Empty);
                continue;
            }

            if (HeadingRegex().IsMatch(line))
            {
                FlushParagraph();
                FlushList();
                var match = HeadingRegex().Match(line);
                var level = match.Groups[1].Value.Length;
                var text = match.Groups[2].Value;

                doc.Blocks.Add(new Heading { Level = level, Inlines = ParseInlines(text) });
                continue;
            }

            if (ListItemRegex().IsMatch(line))
            {
                FlushParagraph();
                listItems.Add(line.TrimStart('-', ' '));

                continue;
            }

            if (listItems.Count > 0)
            {
                FlushList();
            }

            paragraphLines.Add(line);
        }

        FlushParagraph();
        FlushList();

        return doc;

        void FlushParagraph()
        {
            if (paragraphLines.Count <= 0)
                return;

            var paragraphText = string.Join('\n', paragraphLines);
            doc.Blocks.Add(new Paragraph { Inlines = ParseInlines(paragraphText) });
            paragraphLines.Clear();
        }

        void FlushList()
        {
            if (listItems.Count <= 0)
                return;

            doc.Blocks.Add(
                new ListBlock
                {
                    Type = ListType.Bullet,
                    Items = listItems
                        .Select(text => new ListItem { Inlines = ParseInlines(text) })
                        .ToList(),
                }
            );
            listItems.Clear();
        }
    }

    private static List<InlineElement> ParseInlines(string text)
    {
        var inlines = new List<InlineElement>();
        var lines = text.Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            inlines.AddRange(ParseInlineFormatting(lines[i]));

            if (i < lines.Length - 1)
                inlines.Add(new LineBreak());
        }

        return inlines;
    }

    private static List<InlineElement> ParseInlineFormatting(string text)
    {
        var inlines = new List<InlineElement>();

        // can be vastly improved later
        var remaining = text;

        while (remaining.Length > 0)
        {
            var boldMatch = BoldRegex().Match(remaining);
            var italicMatch = ItalicRegex().Match(remaining);
            var codeMatch = CodeRegex().Match(remaining);

            var first = new[] { boldMatch, italicMatch, codeMatch }
                .Where(m => m.Success)
                .OrderBy(m => m.Index)
                .FirstOrDefault();

            if (first == null)
            {
                inlines.Add(new TextRun { Text = remaining });
                break;
            }

            if (first.Index > 0)
            {
                inlines.Add(new TextRun { Text = remaining[..first.Index] });
            }

            if (first == boldMatch)
            {
                inlines.Add(
                    new Strong { Children = [new TextRun { Text = boldMatch.Groups[1].Value }] }
                );
            }
            else if (first == italicMatch)
            {
                inlines.Add(
                    new Emphasis { Children = [new TextRun { Text = italicMatch.Groups[1].Value }] }
                );
            }
            else if (first == codeMatch)
            {
                inlines.Add(new InlineCode { Code = codeMatch.Groups[1].Value });
            }

            remaining = remaining[(first.Index + first.Length)..];
        }

        return inlines;
    }

    [GeneratedRegex(@"^(#{1,6})\s+(.*)$")]
    private static partial Regex HeadingRegex();

    [GeneratedRegex(@"^\s*-\s")]
    private static partial Regex ListItemRegex();

    [GeneratedRegex(@"\*\*(.+?)\*\*")]
    private static partial Regex BoldRegex();

    [GeneratedRegex(@"\*(.+?)\*")]
    private static partial Regex ItalicRegex();

    [GeneratedRegex("`(.+?)`")]
    private static partial Regex CodeRegex();
}
