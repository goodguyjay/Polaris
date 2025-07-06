using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig.Syntax;

namespace Polaris.UI.Rendering;

public sealed class MarkdownUiVisitor : IMarkdownUiVisitor
{
    public Control Visit(HeadingBlock heading)
    {
        var text = string.Join("", heading.Inline!.Select(i => i.ToString()));

        return new TextBlock
        {
            Text = text,
            FontSize = 16 + (6 - heading.Level) * 2, // Dynamic font size based on heading level
            FontWeight = FontWeight.Bold,
            TextWrapping = TextWrapping.Wrap,
        };
    }

    public Control Visit(ParagraphBlock paragraph)
    {
        var textBlock = new TextBlock
        {
            FontSize = 14,
            TextWrapping = TextWrapping.Wrap,
        };
        
        foreach (var inline in MarkdownRenderer.RenderInlines(paragraph.Inline))
            textBlock.Inlines?.Add(inline);

        return textBlock;
    }

    public Control Visit(ListBlock list)
    {
        var stack = new StackPanel { Margin = new Thickness(0, 4, 0, 4) };

        var itemNumber = 1;
        foreach (var block in list)
        {
            if (block is ListItemBlock item)
            {
                foreach (var child in item)
                {
                    var content = MarkdownRenderer.RenderBlock(child, this);

                    var prefix = list.IsOrdered
                        ? $"{itemNumber}. "
                        : "• ";

                    var panel = new StackPanel { Orientation = Orientation.Horizontal };

                    panel.Children.Add(new TextBlock
                    {
                        Text = prefix,
                        FontWeight = FontWeight.Bold,
                        Margin = new Thickness(0, 0, 4, 0)
                    });

                    panel.Children.Add(content);

                    stack.Children.Add(panel);
                }
            }
            else
            {
                // Handle unexpected blocks
                stack.Children.Add(new TextBlock
                {
                    Text = "Unexpected block type in list.",
                    Foreground = Brushes.Red
                });
            }
            itemNumber++;
        }


        return stack;
    }

    public Control Visit(CodeBlock? code)
    {
        var codeText = string.Empty;

        if (code?.Lines.Lines != null)
            codeText = string.Join(
                Environment.NewLine,
                code.Lines.Lines.Select(l => l.ToString().TrimEnd('\n', '\r'))
            );

        // extract language info if available
        var language = (code as Markdig.Syntax.FencedCodeBlock)?.Info;

        return new Border
        {
            Background = Brushes.LightGray,
            Padding = new Thickness(8),
            Margin = new Thickness(0, 8, 0, 8),
            CornerRadius = new CornerRadius(6),
            Child = new TextBlock
            {
                FontFamily = new FontFamily("Consolas, monospace"),
                FontSize = 13,
                Text = codeText,
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.DarkSlateGray
            }
        };
    }

    public Control Visit(QuoteBlock quote)
    {
        var panel = new StackPanel
        {
            Margin = new Thickness(0, 8, 0, 8)
        };

        foreach (var child in quote)
            panel.Children.Add(MarkdownRenderer.RenderBlock(child, this));

        return new Border
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(3, 0, 0, 0),
            Background = Brushes.Transparent,
            Padding = new Thickness(12, 4, 4, 4),
            Margin = new Thickness(0, 8, 0, 8),
            Child = panel
        };
    }

    public Control Visit(ThematicBreakBlock _)
    {
        return new Separator
        {
            Margin = new Thickness(0, 12, 0, 12),
            Height = 1,
            Background = Brushes.Gray
        };
    }
}