Polaris `.polar` Format Specification

Version 0.1 (DRAFT)

Status: In development

Maintainers: Jay & Community

License: GPL-2.0

---

## 1. Overview

The `.polar` format is an offline‑first, XML‑based document model optimized for the Polaris editor. It provides:

* **Deterministic parsing** and rendering
* **High‑fidelity WYSIWYG preview**
* **Clean, CommonMark‑compliant export**
* **Extensible architecture** for future features

## 2. Goals

* **Simplicity**: Minimal core spec, favoring predictable behavior
* **Determinism**: Fixed serialization order and whitespace rules
* **Extensibility**: Namespaced extensions & reserved x- elements
* **Toolability**: Easily validated via XSD/DTD and programmatically transformed

## 3. XML Namespace & Schema

Define a namespace to avoid collisions and allow versioned extensions:

```xml
<polar xmlns="https://polaris.dev/schema/polar" version="0.1">
  ...
</polar>
```

* **xmlns**: URI for XSD
* **version**: MUST match the XSD version

Publish an XSD alongside the spec describing types for every element and attribute.

## 4. Document Structure

### 4.1 Root Element

```xml
<polar version="0.1" id="doc-123" style="article">
  <!-- metadata, body -->
</polar>
```

* **version** (required)
* **id**, **style** (optional)
* **metadata** child before content (title, authors, dates)

### 4.2 Metadata Section

```xml
<metadata>
  <title>Document Title</title>
  <author id="u1">Jay</author>
  <date created="2025-07-11" modified="2025-07-12"/>
  <custom key="theme">dark</custom>
</metadata>
```

* **title**, **author**, **date**, and arbitrary **custom** entries

## 5. Core Elements

### 5.1 Block Elements

* **Heading**: `<heading level="1">Text</heading>`
* **Paragraph**: `<p>Text</p>`
* **List**: `<list type="bullet"> <item>…</item> </list>`
* **Code Block**: `<code language="csharp">…</code>`
* **Horizontal Rule**: `<hr/>`

### 5.2 Inline Elements

* **Strong**: `<strong>bold</strong>`
* **Emphasis**: `<em>italic</em>`
* **Link**: `<a href="url" title="…">text</a>`
* **Inline Code**: `<code>...</code>`

## 6. Attributes & Styling

* **id**: Global unique identifier
* **style**: CSS‑like styling reference
* **data-**\*: Arbitrary data attributes for plugins
* All custom or future features should use `x-` prefix (e.g., `x-annotation`)

## 7. Deterministic Serialization

* Enforce element ordering in XSD
* Trim extraneous whitespace
* Canonical attribute ordering: alphabetical

## 8. Versioning & Compatibility

* **version** attribute MUST be incremented on breaking changes
* Consumers should reject unknown major versions
* **Deprecated** elements marked in spec and XSD annotations

## 9. Extensions & Namespaces

Use separate namespaces for community plugins:

```xml
<polar xmlns="https://polaris.dev/schema/polar"
       xmlns:task="https://polaris.dev/schema/task"
       version="0.1">
  <task:list>…</task:list>
</polar>
```

## 10. Examples

```xml
<polar version="0.1">
  <metadata>
    <title>Welcome to Polaris</title>
    <author>Jay</author>
    <date created="2025-07-11"/>
  </metadata>

  <heading level="1">Hello, Polaris!</heading>
  <p>This is a <strong>sample</strong> document.</p>
  <list type="bullet">
    <item>First item</item>
    <item>Second item</item>
  </list>
  <code language="csharp">Console.WriteLine("Hello!");</code>
</polar>
```

## 11. Implementation Notes

* **Parser**: Use streaming `XmlReader` to minimize memory
* **Validation**: XSD at load time, custom checks for business rules
* **Export**: Map elements 1:1 to CommonMark AST for Markdown generation

## 12. Roadmap

* Define **inline annotations** (comments, highlights)
* Add **table** support
* Embed **images**, **footnotes**, **citations**
* Collaboration features: `<change-set>`, `<review-comment>`

---

*Feedback Welcome! Submit PRs to the repository & discuss on issues.*
