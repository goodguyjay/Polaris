using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
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
                    result.Add(new Run { Text = literal.Content.ToString() });
                    break;
                case LineBreakInline _:
                    result.Add(new LineBreak());
                    break;
                // TODO: add more cases for EmphasisInline, CodeInline, LinkInline, etc
            }
        }
        return result;
    }
}