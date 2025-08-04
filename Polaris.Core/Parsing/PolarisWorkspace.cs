using System.IO.Compression;
using Polaris.Core.Document;

namespace Polaris.Core.Parsing;

public static class PolarisWorkspace
{
    public static PolarDocument OpenPolaris(string path)
    {
        using var zip = ZipFile.OpenRead(path);
        
        var entry = zip.GetEntry("main.polar");
        if (entry == null)
            throw new InvalidDataException("main.polar entry not found in the zip file.");
        
        using var stream = entry.Open();
        var document = PolarDocumentParser.Load(stream);
        
        if (document == null)
            throw new InvalidDataException("Failed to load Polaris document from main.polar.");
        
        if (zip.GetEntry(".private/.keep") == null)
            zip.CreateEntry(".private/.keep");

        return document;
    }

    public static void SavePolaris(string path, PolarDocument doc, Dictionary<string, byte[]>? assets = null)
    {
        using var zip = ZipFile.Open(path, ZipArchiveMode.Create);
        
        // write main.polar
        var entry = zip.CreateEntry("main.polar");
        using (var stream = entry.Open())
        {
            PolarXmlWriter.Save(doc, stream);
        }

        if (assets != null)
        {
            foreach (var (filename, data) in assets)
            {
                var assetEntry = zip.CreateEntry($"assets/{filename}");
                using var assetStream = assetEntry.Open();
                assetStream.Write(data, 0, data.Length);
            }
        }

        zip.CreateEntry(".private/.keep");
    }

    public static Stream? GetAssetStream(ZipArchive zip, string assetPath)
    {
        throw new NotImplementedException("Asset loading is not implemented yet.");
    }
}