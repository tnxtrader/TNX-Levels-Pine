"""Extract everything from the research PDFs.

For each PDF writes research/extracted/<slug>/:
  text.md        - layout-aware markdown (pymupdf4llm)
  text_raw.txt   - plain text per page (pymupdf), fallback/cross-check
  images/        - every embedded raster image (photos, plots stored as images)
  pages/         - 110-dpi render of every page that holds a figure/vector drawing
                   (vector plots are not "images", so rendering is the only way to keep them)
  tables.md      - tables detected by pymupdf find_tables()
  meta.json      - metadata, page count, image/figure counts, OCR need flag
"""
import json, re, sys, pathlib
import pymupdf, pymupdf4llm

ROOT = pathlib.Path(__file__).resolve().parents[1]
RAW, OUT = ROOT / "raw", ROOT / "extracted"

def slug(p):
    s = re.sub(r"#U[0-9a-fA-F]{4}", "-", p.stem)
    s = re.sub(r"[^A-Za-z0-9]+", "_", s).strip("_")
    return s[:80]

summary = []
for pdf in sorted(RAW.rglob("*.pdf")):
    d = OUT / slug(pdf)
    (d / "images").mkdir(parents=True, exist_ok=True)
    (d / "pages").mkdir(parents=True, exist_ok=True)
    doc = pymupdf.open(pdf)
    raw_pages, n_img, fig_pages, tables_md, chars = [], 0, [], [], 0
    for i, page in enumerate(doc):
        t = page.get_text("text")
        chars += len(t.strip())
        raw_pages.append(f"\n\n===== PAGE {i+1} =====\n{t}")
        # embedded raster images
        for j, img in enumerate(page.get_images(full=True)):
            xref = img[0]
            try:
                pix = pymupdf.Pixmap(doc, xref)
                if pix.width < 60 or pix.height < 60:
                    continue
                if pix.n - pix.alpha >= 4:
                    pix = pymupdf.Pixmap(pymupdf.csRGB, pix)
                pix.save(d / "images" / f"p{i+1:03d}_img{j+1}.png")
                n_img += 1
            except Exception as e:
                print("img fail", pdf.name, i, e, file=sys.stderr)
        # vector drawings (charts) -> render page
        has_vec = len(page.get_drawings()) > 40
        if page.get_images() or has_vec:
            fig_pages.append(i + 1)
            page.get_pixmap(dpi=110).save(d / "pages" / f"page_{i+1:03d}.png")
        # tables
        try:
            for k, tb in enumerate(page.find_tables().tables):
                md = tb.to_markdown()
                if md and md.count("|") > 6:
                    tables_md.append(f"### Page {i+1} table {k+1}\n\n{md}\n")
        except Exception as e:
            print("table fail", pdf.name, i, e, file=sys.stderr)
    (d / "text_raw.txt").write_text("".join(raw_pages))
    try:
        md = pymupdf4llm.to_markdown(str(pdf), show_progress=False)
    except Exception as e:
        md = f"(pymupdf4llm failed: {e})\n" + "".join(raw_pages)
    (d / "text.md").write_text(md)
    (d / "tables.md").write_text("\n".join(tables_md) or "(no tables detected)\n")
    meta = dict(file=str(pdf.relative_to(RAW)), slug=d.name, pages=doc.page_count,
                metadata=doc.metadata, embedded_images=n_img, figure_pages=fig_pages,
                tables=len(tables_md), text_chars=chars,
                needs_ocr=chars < 200 * doc.page_count)
    (d / "meta.json").write_text(json.dumps(meta, indent=2, default=str))
    summary.append(meta)
    print(f"{d.name}: {doc.page_count}p, {chars} chars, {n_img} imgs, {len(fig_pages)} fig pages, {len(tables_md)} tables, ocr={meta['needs_ocr']}")
(OUT / "_index.json").write_text(json.dumps(summary, indent=2, default=str))
