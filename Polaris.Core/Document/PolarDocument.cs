using Polaris.Core.Document.Elements;

namespace Polaris.Core.Document;

public sealed class PolarDocument
{
    public string? Version { get; set; } = "0.1";
    public string? Id { get; set; }
    public string? Style { get; set; }
    public Metadata.Metadata Metadata { get; set; } = new Metadata.Metadata();
    public List<BlockElement> Blocks { get; set; } = [];
}