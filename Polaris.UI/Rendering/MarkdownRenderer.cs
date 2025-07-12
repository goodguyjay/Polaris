using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Polaris.UI.Rendering;

public static class MarkdownRenderer
{
    public static Control RenderDocument(MarkdownDocument doc, IMarkdownUiVisitor visitor)
    {
        var panel = new StackPanel();
        
        foreach (var block in doc)
            panel.Children.Add(RenderBlock(block, visitor));
        
        return panel;
    }
    
    public static Control RenderBlock(MarkdownObject block, IMarkdownUiVisitor visitor)
    {
        return block switch
        {
            HeadingBlock heading => visitor.Visit(heading),
            ParagraphBlock paragraph => visitor.Visit(paragraph),
            ListBlock list => visitor.Visit(list),
            QuoteBlock quote => visitor.Visit(quote),
            CodeBlock code => visitor.Visit(code),
            ThematicBreakBlock thematicBreak => visitor.Visit(thematicBreak),
            _ => new TextBlock { Text = block.ToString() }
        };
    }

    public static IEnumerable<Avalonia.Controls.Documents.Inline> RenderInlines(ContainerInline? container)
    {
        var result = new List<Avalonia.Controls.Documents.Inline>();
        if (container == null) return result;

        foreach (var inline in container)
        {
            switch (inline)
            {
                case LiteralInline literal:
                {
                    var content = literal.Content.ToString();
                    Console.WriteLine(content);
                    var lines = content.Split('\n');
                    for (var i = 0; i < lines.Length; i++)
                    {
                        result.Add(new Run(lines[i]));
                        
                        if (i < lines.Length - 1)
                        {
                            result.Add(new LineBreak());
                        }
                    }
                    
                    break;
                }
                
                case LineBreakInline _:
                    result.Add(new LineBreak());
                    break;

                case EmphasisInline emphasis:
                {
                    var childInlines = RenderInlines(emphasis);
                    var span = new Span();
                    
                    foreach (var child in childInlines)
                        span.Inlines.Add(child);

                    switch (emphasis)
                    {
                        // markdig handles bold as delimiter count == 2, italic as 1
                        // double or triple asterisks/underscores are both possible (***bold+italic***)
                        case { DelimiterCount: >= 2, DelimiterChar: '*' }:
                        {
                            span.FontWeight = FontWeight.Bold;
                        
                            if (emphasis.DelimiterCount == 3)
                                span.FontStyle = FontStyle.Italic; // bold + italic
                            break;
                        }
                        case { DelimiterCount: >= 2, DelimiterChar: '_' }:
                        {
                            span.FontWeight = FontWeight.Bold;
                        
                            if (emphasis.DelimiterCount == 3)
                                span.FontStyle = FontStyle.Italic;
                            break;
                        }
                        default:
                            span.FontStyle = FontStyle.Italic;
                            break;
                    }
                    
                    result.Add(span);
                    break;
                }
                
                case CodeInline codeInline:
                {
                    result.Add(new Run
                    {
                        Text = codeInline.Content,
                        Background = Brushes.DarkGray,
                        FontFamily = new FontFamily("Consolas, monospace"),
                        FontSize = 13
                    });

                    break;
                }

                case LinkInline { IsImage: false } linkInline:
                {
                    var children = RenderInlines(linkInline).ToList();
                    var linkText = string.Concat(children.OfType<Run>().Select(r => r.Text));

                    var button = new HyperlinkButton
                    {
                        Content = linkText,
                        NavigateUri = Uri.TryCreate(linkInline.Url ?? "", UriKind.Absolute, out var uri) ? uri : null,
                        Classes = { "markdown-link" },
                        Margin = new Thickness(0, 0, 2, 0)
                    };
                    
                    result.Add(new InlineUIContainer { Child = button});
                    break;
                }
                
                default:
                    result.Add(new Run { Text = inline.ToString() });
                    break;
                // TODO: add more cases for EmphasisInline, CodeInline, LinkInline, etc
            }
        }
        return result;
    }
}