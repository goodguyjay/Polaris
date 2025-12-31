# Polaris Editor

**Polaris** is an open-source document editor focused on *user experience, offline-first workflows, and clean, deterministic export*.

- **Native `.polaris` format:** Like `.docx` for Markdown-style documents. Rich editing, perfect previews, no ugly exports.
- **Offline-first:** Works seamlessly without an internet connection. Your documents are always accessible.
- **Export to PDF:** Clean, deterministic exports that look great and are easy to share.
- **Open-source:** Built with love by the community. Contributions are welcome!

---

## What Makes Polaris Different?

- **User-First Design:**  
  Intuitive, distraction-free editing. Live preview is always accurate. What you see is truly what you get.


- **Native Format, Clean Exports:**  
  Edits are saved in `.polaris` files (ZIP archives) with a custom `.polar` XML for perfect round-trip and deterministic Markdown/PDF export.  
  No more “mystery formatting” or messy conversions.


- **Private & Portable:**  
  All documents and assets are stored locally. Take your work anywhere. Zero dependencies.

---

## Why Not Just Markdown?

Markdown is great, but it can’t store all the structure, styles, and metadata you actually use in a real editor.  
Polaris’s `.polar` format lets you edit richly, then *export clean, readable Markdown or PDF*.

---

## File Format

A Polaris file (`.polaris`) is a portable workspace, just like `.docx` or `.odt`.  
Inside, it stores your document (`main.polar`), all assets, and any future features (history, previews, etc).

---

## Status

**Polaris is in early development.**  
We’re working on the core editor, file format, and preview engine.

---

## License

This project is licensed under the **GNU AGPLv3** — all forks, modifications, and hosted services derived from this code must remain open and free.

**Third-party libraries:**
- This project uses [QuestPDF](https://www.questpdf.com/) under the QuestPDF Community License (MIT).