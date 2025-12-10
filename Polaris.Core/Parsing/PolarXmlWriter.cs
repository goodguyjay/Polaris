using System.Xml;
using Polaris.Core.Document;
using Polaris.Core.Document.Elements;
using Polaris.Core.Document.InlineElements;
using Polaris.Core.Document.Metadata;

namespace Polaris.Core.Parsing;

public static class PolarXmlWriter
{
    public static void Save(PolarDocument doc, Stream output)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineOnAttributes = false,
            Encoding = System.Text.Encoding.UTF8,
        };

        using var writer = XmlWriter.Create(output, settings);

        writer.WriteStartDocument();
        writer.WriteStartElement("polar");
        writer.WriteAttributeString("version", doc.Version ?? "0.1");
        if (!string.IsNullOrWhiteSpace(doc.Id))
            writer.WriteAttributeString("id", doc.Id);
        if (!string.IsNullOrWhiteSpace(doc.Style))
            writer.WriteAttributeString("style", doc.Style);

        WriteMetadata(doc.Metadata, writer);

        foreach (var block in doc.Blocks)
        {
            WriteBlock(block, writer);
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();
    }

    private static void WriteMetadata(Metadata metadata, XmlWriter writer)
    {
        writer.WriteStartElement("metadata");
        if (!string.IsNullOrWhiteSpace(metadata.Title))
            writer.WriteElementString("title", metadata.Title);

        foreach (var author in metadata.Authors)
        {
            writer.WriteStartElement("author");
            if (!string.IsNullOrWhiteSpace(author.Id))
                writer.WriteElementString("id", author.Id);
            writer.WriteString(author.Name);
            writer.WriteEndElement();
        }

        if (metadata.Date != null)
        {
            writer.WriteStartElement("date");
            if (metadata.Date.Created.HasValue)
                writer.WriteAttributeString(
                    "created",
                    metadata.Date.Created.Value.ToString("yyyy-MM-dd")
                );
            if (metadata.Date.Modified.HasValue)
                writer.WriteAttributeString(
                    "modified",
                    metadata.Date.Modified.Value.ToString("yyyy-MM-dd")
                );
            writer.WriteEndElement();
        }

        foreach (var kv in metadata.Custom)
        {
            writer.WriteStartElement("custom");
            writer.WriteAttributeString("key", kv.Key);
            writer.WriteString(kv.Value);
            writer.WriteEndElement();
        }

        writer.WriteEndElement();
    }

    private static void WriteBlock(BlockElement block, XmlWriter writer)
    {
        switch (block)
        {
            case Heading h:
            {
                writer.WriteStartElement("heading");
                writer.WriteAttributeString("level", h.Level.ToString());
                WriteCommonBlockAttributes(h, writer);
                WriteInlines(h.Inlines, writer);
                break;
            }
            case Paragraph p:
            {
                writer.WriteStartElement("p");
                WriteCommonBlockAttributes(p, writer);
                WriteInlines(p.Inlines, writer);
                break;
            }
            case CodeBlock cb:
            {
                writer.WriteStartElement("code");
                if (!string.IsNullOrWhiteSpace(cb.Language))
                    writer.WriteAttributeString("language", cb.Language);
                WriteCommonBlockAttributes(cb, writer);
                writer.WriteString(cb.Code);
                break;
            }
            case HorizontalRule hr:
            {
                writer.WriteStartElement("hr");
                WriteCommonBlockAttributes(hr, writer);
                // self closing, no content
                break;
            }
            case ListBlock lb:
            {
                writer.WriteStartElement("list");
                writer.WriteAttributeString("type", lb.Type.ToString().ToLowerInvariant()); // "bullet" or "ordered"
                WriteCommonBlockAttributes(lb, writer);

                foreach (var item in lb.Items)
                {
                    writer.WriteStartElement("item");
                    WriteInlines(item.Inlines, writer);
                    writer.WriteEndElement();
                }

                break;
            }
            case Blank b:
            {
                writer.WriteStartElement("blank");
                if (b.Count > 1)
                    writer.WriteAttributeString("count", b.Count.ToString());
                WriteCommonBlockAttributes(b, writer);
                // self closing, no content
                break;
            }
        }

        writer.WriteEndElement();
    }

    private static void WriteCommonBlockAttributes(BlockElement block, XmlWriter writer)
    {
        if (!string.IsNullOrWhiteSpace(block.Id))
            writer.WriteAttributeString("id", block.Id);
        if (!string.IsNullOrWhiteSpace(block.Style))
            writer.WriteAttributeString("style", block.Style);
    }

    private static void WriteInlines(List<InlineElement> inlines, XmlWriter writer)
    {
        foreach (var inline in inlines)
        {
            switch (inline)
            {
                case TextRun t:
                    writer.WriteString(t.Text ?? string.Empty);
                    break;

                case Strong s:
                    writer.WriteStartElement("strong");
                    WriteInlines(s.Children, writer);
                    writer.WriteEndElement();
                    break;

                case Emphasis e:
                    writer.WriteStartElement("em");
                    WriteInlines(e.Children, writer);
                    writer.WriteEndElement();
                    break;

                case Link l:
                    writer.WriteStartElement("a");
                    writer.WriteAttributeString("href", l.Href);
                    if (!string.IsNullOrWhiteSpace(l.Title))
                        writer.WriteAttributeString("title", l.Title);
                    WriteInlines(l.Children, writer);
                    writer.WriteEndElement();
                    break;

                case InlineCode code:
                    writer.WriteStartElement("code");
                    writer.WriteString(code.Code);
                    writer.WriteEndElement();
                    break;

                case Image img:
                    writer.WriteStartElement("img");
                    writer.WriteAttributeString("src", img.Src);
                    writer.WriteAttributeString("alt", img.Alt);
                    if (!string.IsNullOrWhiteSpace(img.Title))
                        writer.WriteAttributeString("title", img.Title);
                    writer.WriteEndElement();
                    break;
            }
        }
    }
}
