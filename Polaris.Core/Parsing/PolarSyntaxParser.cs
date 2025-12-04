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
        var listType = ListType.Bullet;
        var inCodeBlock = false;
        string? codeLang = null;
        var codeLines = new List<string>();
        var blankLineCount = 0;

        foreach (var lineRaw in lines)
        {
            var line = lineRaw.TrimEnd('\r');

            if (line.StartsWith("```"))
            {
                FlushParagraph();
                FlushList();
                FlushBlanks();

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
                // flush active contexts
                if (listItems.Count > 0)
                {
                    FlushList();
                }

                if (paragraphLines.Count > 0)
                {
                    FlushParagraph();
                }

                blankLineCount++;
                continue;
            }

            if (HorizontalRuleRegex().IsMatch(line))
            {
                FlushParagraph();
                FlushList();
                FlushBlanks();
                doc.Blocks.Add(new HorizontalRule());
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

            if (BulletListItemRegex().IsMatch(line))
            {
                FlushParagraph();

                // if switching list types, flush the previous list
                if (listItems.Count > 0 && listType == ListType.Ordered)
                {
                    FlushList();
                }

                listType = ListType.Bullet;
                listItems.Add(line.TrimStart('-', '*', '+', ' '));
                continue;
            }

            if (OrderedListItemRegex().IsMatch(line))
            {
                FlushParagraph();
                FlushBlanks();

                // if switching list types, flush the previous list
                if (listItems.Count > 0 && listType == ListType.Bullet)
                {
                    FlushList();
                }

                listType = ListType.Ordered;
                var match = OrderedListItemRegex().Match(line);
                listItems.Add(match.Groups[2].Value); // capture text after number and dot
                continue;
            }

            if (listItems.Count > 0)
            {
                FlushList();
            }

            FlushBlanks();

            paragraphLines.Add(line);
        }

        FlushParagraph();
        FlushList();
        FlushBlanks();

        return doc;

        void FlushParagraph()
        {
            if (paragraphLines.Count <= 0)
                return;

            var paragraphText = string.Join('\n', paragraphLines);

            if (string.IsNullOrWhiteSpace(paragraphText))
            {
                paragraphLines.Clear();
                return;
            }

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
                    Type = listType,
                    Items = listItems
                        .Select(text => new ListItem { Inlines = ParseInlines(text) })
                        .ToList(),
                }
            );
            listItems.Clear();
        }

        void FlushBlanks()
        {
            if (blankLineCount <= 0)
                return;

            doc.Blocks.Add(new Blank { Count = blankLineCount });
            blankLineCount = 0;
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
            var linkMatch = LinkRegex().Match(remaining);

            var first = new[] { boldMatch, italicMatch, codeMatch, linkMatch }
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
            else if (first == linkMatch)
            {
                inlines.Add(
                    new Link
                    {
                        Href = linkMatch.Groups[2].Value,
                        Title = linkMatch.Groups[3].Success ? linkMatch.Groups[3].Value : null,
                        Children = [new TextRun { Text = linkMatch.Groups[1].Value }],
                    }
                );
            }

            remaining = remaining[(first.Index + first.Length)..];
        }

        return inlines;
    }

    [GeneratedRegex(@"^(#{1,6})\s+(.*)$")]
    private static partial Regex HeadingRegex();

    [GeneratedRegex(@"^---+$|^\*\*\*+$|^___+$")]
    private static partial Regex HorizontalRuleRegex();

    [GeneratedRegex(@"^\s*[-*+]\s")]
    private static partial Regex BulletListItemRegex();

    [GeneratedRegex(@"^\s*(\d+)\.\s+(.*)$")]
    private static partial Regex OrderedListItemRegex();

    [GeneratedRegex(@"\*\*(.+?)\*\*")]
    private static partial Regex BoldRegex();

    [GeneratedRegex(@"\*(.+?)\*")]
    private static partial Regex ItalicRegex();

    [GeneratedRegex("`(.+?)`")]
    private static partial Regex CodeRegex();

    /// <summary>
    ///  Example: [link text](http://example.com "optional title")
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex("""\[([^\]]+)\]\(([^\)]+?)(?:\s+"([^"]*)")?\)""")]
    private static partial Regex LinkRegex();
}
