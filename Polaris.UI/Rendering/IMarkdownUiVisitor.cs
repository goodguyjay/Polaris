using Avalonia.Controls;
using Markdig.Syntax;

namespace Polaris.UI.Rendering;

public interface IMarkdownUiVisitor
{
    Control Visit(HeadingBlock heading);
    Control Visit(ParagraphBlock paragraph);
    Control Visit(ListBlock list);
    Control Visit(CodeBlock? code);
    Control Visit(QuoteBlock quote);
    Control Visit(ThematicBreakBlock _);
}