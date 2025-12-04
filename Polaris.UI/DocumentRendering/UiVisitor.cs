using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using Polaris.Core.Document.Elements;
using Polaris.Core.Document.InlineElements;
using Polaris.Core.Visitors;
using LineBreak = Polaris.Core.Document.InlineElements.LineBreak;

namespace Polaris.UI.DocumentRendering;

public sealed class UiVisitor : IBlockElementVisitor<Control>, IInlineElementVisitor<Inline>
{
    public Control VisitHeading(Heading heading)
    {
        var tb = new TextBlock
        {
            FontSize = heading.Level switch
            {
                1 => 28,
                2 => 22,
                3 => 18,
                _ => 16,
            },
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0, 12, 0, 4),
        };

        foreach (var inline in heading.Inlines)
            tb.Inlines?.Add(inline.Accept(this));
        return tb;
    }

    public Control VisitParagraph(Paragraph paragraph)
    {
        var tb = new TextBlock
        {
            Margin = new Thickness(0, 0, 0, 8),
            TextWrapping = TextWrapping.Wrap,
        };

        foreach (var inline in paragraph.Inlines)
            tb.Inlines?.Add(inline.Accept(this));
        return tb;
    }

    public Control VisitListBlock(ListBlock listBlock)
    {
        var panel = new StackPanel { Orientation = Orientation.Vertical };
        var idx = 1;

        foreach (var item in listBlock.Items)
        {
            var tb = new TextBlock { Margin = new Thickness(16, 0, 0, 0) };
            var prefix = listBlock.Type == ListType.Bullet ? "• " : $"{idx++}. ";
            tb.Inlines?.Add(new Run { Text = prefix });

            foreach (var inline in item.Inlines)
                tb.Inlines?.Add(inline.Accept(this));

            panel.Children.Add(tb);
        }

        return panel;
    }

    public Control VisitCodeBlock(CodeBlock codeBlock)
    {
        return new TextBox
        {
            Text = codeBlock.Code,
            FontFamily = "Consolas, JetBrains Mono, Cascadia Code, monospace",
            Background = Brushes.DarkGray,
            Margin = new Thickness(0, 8, 0, 8),
            IsReadOnly = true,
        };
    }

    public Control VisitHorizontalRule(HorizontalRule _) =>
        new Separator { Margin = new Thickness(0, 8, 0, 8) };

    public Control VisitBlank(Blank blank) => new Border { Height = 20 * blank.Count };

    public Inline VisitTextRun(TextRun text) => new Run { Text = text.Text };

    public Inline VisitStrong(Strong strong)
    {
        var span = new Bold();
        foreach (var child in strong.Children)
            span.Inlines.Add(child.Accept(this));
        return span;
    }

    public Inline VisitEmphasis(Emphasis emphasis)
    {
        var span = new Italic();
        foreach (var child in emphasis.Children)
            span.Inlines.Add(child.Accept(this));
        return span;
    }

    public Inline VisitInlineCode(InlineCode inlineCode) =>
        new Run
        {
            Text = inlineCode.Code,
            FontFamily = "Consolas, JetBrains Mono, Cascadia Code, monospace",
        };

    public Inline VisitLineBreak(LineBreak lineBreak)
    {
        return new Avalonia.Controls.Documents.LineBreak();
    }

    public Inline VisitLink(Link link)
    {
        // mvp will just render link text, not clickable
        var span = new Span
        {
            Foreground = Brushes.Blue,
            TextDecorations = TextDecorations.Underline,
        };
        foreach (var child in link.Children)
            span.Inlines.Add(child.Accept(this));

        // todo: add tooltip with href/title and make clickable
        return span;
    }
}
