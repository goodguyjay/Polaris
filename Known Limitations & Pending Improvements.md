# Critical Performance Issues

## Image Parsing on Every Keystroke
**Problem:** Images are converted to base64 on every character typed, causing unecessary I/O operations and CPU usage.
- Current behavior: `File.ReadAllBytes` called repeatedly for the same image.
- Impact: Noticeable lag with large images (>1MB), multiplied by number of images in document.
- Temporary mitigation: Ostrich Algorithm.

**Potential Solutions (post-MVP)**
1. **Debouncing (300ms-500ms delay).** Simple but may still cause lag with large images.
2. **Image caching.** Cache base64 conversions in memory with path-based lookup.
3. **Background processing.** Offload conversion to background thread, updating UI when done.
4. **Lazy loading.** Only convert images when they enter the viewport.
5. **File system watchers.** Monitor image files for changes to avoid redundant reads.
6. **Hybrid approach.** Combine caching with debouncing for optimal performance.

**Priority**: Medium (acceptable for MVP, but requires urgency in post-MVP).

---

# Code Organization

## Parser Structure
**Problem:** `PolarSyntaxParser` class is growing monolithic with inline helper methods and regex definitions mixed with parsing logic.

**Needed refactoring:**
- Extract inline formatting to separate class (`InlineParser`)
- Move regex patterns to dedicated static class (`RegexPatterns`)
- Separate concerns: block parsing vs inline parsing vs state management

## ViewModels
**Problem:** MainWindowViewModel handles too many responsibilities (parsing, preview, file I/O, document conversion).

**Needed refactoring:**
- Extract `DocumentConverter` service (AST <-> plaintext)
- Extract `PreviewRenderer` service (AST -> UI Controls)
- Keep ViewModel focused on UI state and commands

## Visitor Pattern Implementation
**Problem:** Multiple visitors(`UiVisitor`, future `PdfVisitor`) will duplicate traversal logic.

**Needed refactoring:**
- Create base `BlockElementVisitor<T>` with default traversal
- Individual visitors only override specific element rendering
- Reduces boilerplate when adding new elements

**Priority**: High (tech debt pilling up)

---

# Missing Core Features

## Markdown Elements Not Yet Implemented
- [ ] Tables
- [ ] Blockquotes
- [ ] Nested lists
- [ ] Task lists
- [ ] Strikethrough text
- [ ] Subscript/superscript
- [ ] Footnotes

**Priority**: Tables high, rest low

## Nested Formatting Support
**Problem**: Parser doesn't handle nested inline formatting correctly.

- Example: `**bold and _italic_ text**` may fail
- Current regex approach is fragile for nesting

**Solution:** Proper recursive descent parsing for inline elements (post MVP).

---

# Export & Preview

## PDF Export
**Status:** Not yet implemented (core MVP deliverable).

**Planned approach:**
- QuestPDF for rendering
- Template system (government, academic, generic)
- Need abstraction layer (`IPdfGenerator`) for potential lib migration

## PDF Preview in Editor
**Problem:** No way to preview PDF output without exporting to file.

**Potential solutions:**
1. No preview initially (export → open externally)
2. Static thumbnail of first page (meh)
3. QuestPDF page-by-page image generation (performance concerns)
4. ~~WebView with PDF.js~~ (Avalonia doesn't support WebView well... it is a last resort approach)

**Priority**: Medium (important but not blocking MVP)

---

# File Format & Portability

## Workspace (.polaris) Not Implemented
**Problem:** Single `.polar` file with embedded base64 images becomes huge and hard to manage.

**Future solution:**
- `.polaris` as zip container (like `.docx`)
- Multiple `.polar` files (e.g., `header.polar`, `footer.polar`, `chapter1.polar`)
- Image assets stored separately in `assets/` folder
- Manifest file for metadata and file structure

**Challenges:**
- Asset cleanup
- Corruption handling
- Path resolution between multiple `.polar` files
- Compression strategy for images (preferably lossless)

**Priority**: Medium-high (post-MVP feature, important for real-world use)

## Image Handling Before Workspace
**Current:** Base64 embedded in `.polar`(portable but bloated)

**Problem:** Large documents with multiple images = multi-MB XML files

**Interim solutions:**
- Accept bloat for MVP (really only for testing, I promise)
- ~~External image references~~ (breaks portability)
- ~~Lossless compression of base64~~ (complexity, limited gains)

---

# Testing & Validation

## Missing Test Coverage

- [ ] Unit tests for parser (round-trip tests)
- [ ] Integration tests for file I/O
- [ ] Smoke tests for PDF generation
- [ ] XML schema validation (XSD) - only needed post-workspace

**Priority**: High (critical for maintainability and required for early users)

## Edge Cases not Handled
- Malformed Markdown (incomplete syntax)
- Corrupted XML files
- Missing image files during parsing
- Extremely large documents (>10MB)
- Unicode edge cases (emoji, RTL text, etc.)

---

# UI/UX Improvements

## Editor Experience

- [ ] Syntax highlighting in editor
- [ ] Line numbers
- [ ] Undo/Redo support (currently relies on TextBox default)
- [ ] Search/replace
- [ ] Drag-and-drop image insertion
- [ ] Auto-save functionality
- [ ] Document outline/navigation pane (post-mvp)

**Priority:** Low (MVP focuses on core functionality)

## Preview Fidelity

**Problem:** Preview rendering may not match PDF output exactly (95% fidelity acceptable for MVP)

**Known discrepancies:**
- Font rendering (ContentControl vs QuestPDF)
- Line spacing/margins
- Page breaks (preview has no concept of pages)

**Solution:** Document known differences, improve over time.

---

# Architecture Decisions to Revisit

## Image as InlineElement vs BlockElement
**Current:** `Image` is `InlineElement` (Markdown-compliant)

**Reality:** Most images are block-level (centered, full-width)

**Potential change:** Detect when image is alone on a line and treat as block automatically

## Blank Line Representation
**Current:** `<blank count="4" />` consolidates multiple blank lines

**Trade-off:** More efficient but less granular (what if user wants specific spacing?)

**Decision:** Keep for now, revisit if user feedback indicates need for precise control.

## Parser State Management
**Current:** Mutable state in closure (`FlushParagraph`, `FlushList`, `FlushBlanks`)

**Alternative:** Immutable state machine with explicit transitions

**Decision:** Current approach for MVP, refactor if state bugs become frequent (or whenever I feel like it)

---

# Future Considerations

## Collaborative Editing
Not planned for MVP, but architecture should allow:
- Operational transforms on AST
- Conflict resolution for concurrent edits
- Real-time sync (WebSocket or similar, unlikely to happen)

## Plugin System
Allow users to extend functionality:
- Custom block elements (e.g., diagrams, charts)
- Custom export formats (e.g., HTML, LaTeX)
- Custom templates

**Complexity:** High, not for MVP, but keep extensibility in mind during design.

## Mobile Support
Avalonia supports Android/iOS but:
- Still in beta/experimental
- Significant effort for UI adaptation
- Questionable ROI (desktop-first tool)
- Why?

**Decision:** Desktop only for the foreseeable future.

---

# Documentation Needs
- [ ] User guide (basic usage, file format explanation)
- [ ] Developer guide (architecture overview, code organization)
- [ ] Markdown syntax reference (including custom annotations)
- [ ] Template creation guide
- [ ] Contributing guidelines

---

**Last updated:** 2025-12-22

**Version:** Pre-MVP v0.1