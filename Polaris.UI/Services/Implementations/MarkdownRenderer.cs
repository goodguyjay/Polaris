using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Markdig.Syntax;
using Polaris.UI.Services.Interfaces;

namespace Polaris.UI.Services.Implementations;

public class MarkdownRenderer : IMarkdownRenderer
{
    public Control RenderMarkdown(string markdownText)
    {
        if (string.IsNullOrWhiteSpace(markdownText))
        {
            return new TextBlock { Text = "No content to display." };
        }

        var pipeline = new Markdig.MarkdownPipelineBuilder().Build();
        var document = Markdig.Markdown.Parse(markdownText, pipeline);

        var panel = new StackPanel();

        foreach (var control in document.Select(RenderBlock).OfType<Control>())
        {
            panel.Children.Add(control);
        }

        return panel;
    }

    private static Control? RenderBlock(MarkdownObject block)
    {
        // To add more block types, add more cases to this switch statement
        return block switch
        {
            ParagraphBlock paragraph => RenderParagraph(paragraph),
            HeadingBlock heading => RenderHeading(heading),
            _ => null,
        };
    }

    private static TextBlock RenderParagraph(ParagraphBlock paragraph)
    {
        var text = string.Join("", paragraph.Inline!.Select(inline => inline.ToString()));

        return new TextBlock
        {
            Text = text,
            FontSize = 14,
            TextWrapping = TextWrapping.Wrap,
        };
    }

    private static TextBlock RenderHeading(HeadingBlock heading)
    {
        var text = string.Join("", heading.Inline!.Select(inline => inline.ToString()));

        return new TextBlock
        {
            Text = text,
            FontSize = 16 + (6 - heading.Level) * 2, // Dynamic font size based on heading level
            FontWeight = FontWeight.Bold,
            TextWrapping = TextWrapping.Wrap,
        };
    }
}
