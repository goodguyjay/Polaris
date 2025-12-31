using Polaris.Core.Document;

namespace Polaris.Core.Parsing;

public interface IPolarSyntaxParser
{
    PolarDocument Parse(string input, string? baseDirectory = null);
}
