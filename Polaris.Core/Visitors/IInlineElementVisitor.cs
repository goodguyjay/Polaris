using Polaris.Core.Document.InlineElements;

namespace Polaris.Core.Visitors;

public interface IInlineElementVisitor<out T>
{
    T VisitEmphasis(Emphasis emphasis);
    T VisitInlineCode(InlineCode inlineCode);
    T VisitLineBreak(LineBreak lineBreak);
    T VisitLink(Link link);
    T VisitStrong(Strong strong);
    T VisitTextRun(TextRun text);
}
