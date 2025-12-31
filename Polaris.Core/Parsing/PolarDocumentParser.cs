using System.Xml;
using Polaris.Core.Document;
using Polaris.Core.Document.Elements;
using Polaris.Core.Document.InlineElements;
using Polaris.Core.Document.Metadata;

namespace Polaris.Core.Parsing;

public static class PolarDocumentParser
{
    public static PolarDocument Load(Stream stream)
    {
        using var reader = XmlReader.Create(
            stream,
            new XmlReaderSettings { IgnoreWhitespace = true }
        );

        reader.MoveToContent();

        if (reader.NodeType != XmlNodeType.Element || reader.Name != "polar")
            throw new InvalidDataException("Root element must be <polar>");

        var doc = new PolarDocument
        {
            Version = reader.GetAttribute("version") ?? "0.1",
            Id = reader.GetAttribute("id"),
            Style = reader.GetAttribute("style"),
        };

        if (!reader.IsEmptyElement)
        {
            reader.ReadStartElement("polar");

            while (reader.NodeType == XmlNodeType.Element)
            {
                Console.WriteLine($"reader at: {reader.NodeType}, name: {reader.Name}");

                if (reader.Name == "metadata")
                {
                    doc.Metadata = ParseMetadata(reader);
                }
                else
                {
                    var block = ParseBlockElement(reader);
                    if (block != null)
                        doc.Blocks.Add(block);
                }
            }

            reader.ReadEndElement();
        }
        else
        {
            reader.Read();
        }

        return doc;
    }

    // helper: inline elements (basic impl, can be expanded)
    private static List<InlineElement> ParseInlineElements(XmlReader reader)
    {
        var inlines = new List<InlineElement>();
        if (reader.IsEmptyElement)
        {
            reader.Read();
            return inlines;
        }

        reader.Read();

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Text)
            {
                inlines.Add(new TextRun { Text = reader.Value });
                reader.Read();
            }
            else if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "strong":
                        inlines.Add(new Strong { Children = ParseInlineElements(reader) });
                        reader.ReadEndElement();
                        break;

                    case "em":
                        inlines.Add(new Emphasis { Children = ParseInlineElements(reader) });
                        reader.ReadEndElement();
                        break;

                    case "a":
                        inlines.Add(ParseLink(reader));
                        break;

                    case "code":
                        inlines.Add(new InlineCode { Code = reader.ReadElementContentAsString() });
                        break;

                    case "img":
                        inlines.Add(ParseImage(reader));
                        break;

                    case "br":
                        reader.Read();
                        inlines.Add(new LineBreak());
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        return inlines;
    }

    private static Metadata ParseMetadata(XmlReader reader)
    {
        var metadata = new Metadata();

        if (reader.IsEmptyElement)
        {
            reader.Read(); // skip empty <metadata/>
            return metadata;
        }

        reader.ReadStartElement("metadata");

        while (reader.NodeType == XmlNodeType.Element)
        {
            switch (reader.Name)
            {
                case "title":
                    metadata.Title = reader.ReadElementContentAsString();
                    break;

                case "author":
                    metadata.Authors.Add(
                        new Author
                        {
                            Name = reader.ReadElementContentAsString(),
                            Id = reader.GetAttribute("id"),
                        }
                    );
                    break;

                case "date":
                    metadata.Date = new PolarDate
                    {
                        Created = ParseDate(reader.GetAttribute("created")),
                        Modified = ParseDate(reader.GetAttribute("modified")),
                    };
                    reader.Read();
                    break;

                case "custom":
                {
                    var key = reader.GetAttribute("key");
                    var value = reader.ReadElementContentAsString();

                    if (!string.IsNullOrWhiteSpace(key))
                        metadata.Custom[key] = value;
                    break;
                }

                default:
                    reader.Skip();
                    break;
            }
        }

        reader.ReadEndElement();
        return metadata;
    }

    private static BlockElement? ParseBlockElement(XmlReader reader)
    {
        switch (reader.Name)
        {
            case "heading":
                return ParseHeading(reader);
            case "p":
                return ParseParagraph(reader);
            case "list":
                return ParseList(reader);
            case "code":
                return ParseCodeBlock(reader);
            case "hr":
                reader.ReadStartElement("hr");
                return new HorizontalRule();
            case "blank":
                return ParseBlank(reader);

            default:
                reader.Skip();
                return null;
        }
    }

    private static Heading ParseHeading(XmlReader reader)
    {
        var levelStr = reader.GetAttribute("level") ?? "1";

        if (!int.TryParse(levelStr, out var level))
            throw new InvalidDataException($"Invalid heading level: {levelStr}");

        var heading = new Heading
        {
            Level = level,
            Id = reader.GetAttribute("id"),
            Style = reader.GetAttribute("style"),
            Inlines = ParseInlineElements(reader),
        };

        reader.ReadEndElement();
        return heading;
    }

    private static Paragraph ParseParagraph(XmlReader reader)
    {
        var para = new Paragraph
        {
            Id = reader.GetAttribute("id"),
            Style = reader.GetAttribute("style"),
        };

        if (!reader.IsEmptyElement)
        {
            para.Inlines = ParseInlineElements(reader);
            reader.ReadEndElement(); // </p>
        }
        else
        {
            reader.Read(); // skip empty <p/>
        }

        return para;
    }

    private static ListBlock ParseList(XmlReader reader)
    {
        var typeStr = reader.GetAttribute("type") ?? "bullet";
        var list = new ListBlock
        {
            Type = typeStr == "numbered" ? ListType.Ordered : ListType.Bullet,
            Id = reader.GetAttribute("id"),
            Style = reader.GetAttribute("style"),
        };

        reader.ReadStartElement("list");

        while (reader is { NodeType: XmlNodeType.Element, Name: "item" })
        {
            var item = new ListItem { Inlines = ParseInlineElements(reader) };

            reader.ReadEndElement(); // </item>
            list.Items.Add(item);
        }

        reader.ReadEndElement(); // </list>
        return list;
    }

    private static CodeBlock ParseCodeBlock(XmlReader reader)
    {
        var code = new CodeBlock
        {
            Language = reader.GetAttribute("language"),
            Id = reader.GetAttribute("id"),
            Style = reader.GetAttribute("style"),
            Code = reader.ReadElementContentAsString(),
        };

        return code;
    }

    private static Blank ParseBlank(XmlReader reader)
    {
        var count = reader.GetAttribute("count");
        reader.ReadStartElement("blank");
        return new Blank { Count = int.TryParse(count, out var c) ? c : 1 };
    }

    private static Link ParseLink(XmlReader reader)
    {
        var href = reader.GetAttribute("href") ?? string.Empty;
        var title = reader.GetAttribute("title");
        var link = new Link
        {
            Href = href,
            Title = title,
            Children = ParseInlineElements(reader),
        };
        reader.ReadEndElement(); // </a>

        return link;
    }

    private static Image ParseImage(XmlReader reader)
    {
        var src = reader.GetAttribute("src") ?? string.Empty;
        var alt = reader.GetAttribute("alt") ?? string.Empty;
        var originalPath = reader.GetAttribute("original-path");
        var title = reader.GetAttribute("title");
        reader.ReadStartElement("img");
        return new Image
        {
            Src = src,
            OriginalPath = originalPath,
            Alt = alt,
            Title = title,
        };
    }

    private static DateTime? ParseDate(string? value)
    {
        if (DateTime.TryParse(value, out var date))
            return date;

        return null;
    }
}
