using Polaris.Core.Document.Elements;

namespace Polaris.Core.Visitors;

public interface IBlockElementVisitor<out T>
{
    T VisitCodeBlock(CodeBlock codeBlock);
    T VisitHeading(Heading heading);
    T VisitHorizontalRule(HorizontalRule horizontalRule);
    T VisitListBlock(ListBlock listBlock);
    T VisitParagraph(Paragraph paragraph);
}
