using Polaris.Core.Document;
using Polaris.Core.Export;

namespace Polaris.Pdf;

public interface IPdfGenerator
{
    public byte[] Generate(PolarDocument document, PdfOptions options);
}
