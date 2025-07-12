namespace Polaris.Core.Document.Metadata;

public sealed class Metadata
{
    public string? Title { get; set; }
    public List<Author> Authors { get; set; } = [];
    public PolarDate? Date { get; set; }
    public Dictionary<string, string> Custom { get; set; } = new();
}