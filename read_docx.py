import zipfile
import xml.etree.ElementTree as ET
import os
import sys

def read_docx(file_path):
    try:
        with zipfile.ZipFile(file_path) as z:
            xml_content = z.read('word/document.xml')
            root = ET.fromstring(xml_content)
            
            # Find all text elements
            texts = []
            for paragraph in root.iter('{http://schemas.openxmlformats.org/wordprocessingml/2006/main}p'):
                p_text = []
                for text_el in paragraph.iter('{http://schemas.openxmlformats.org/wordprocessingml/2006/main}t'):
                    if text_el.text:
                        p_text.append(text_el.text)
                if p_text:
                    texts.append(''.join(p_text))
            return '\n'.join(texts)
    except Exception as e:
        return f"Error reading {file_path}: {e}"

docs = [
    "Mob App Work flow.docx",
    "api work flow.docx",
    "sdk work flow.docx",
    "wORK FLOW OF PVC CARD.docx"
]

base_dir = r"c:\Users\hp\OneDrive\Desktop\pmc pvc card work flow"
out_path = os.path.join(base_dir, "extracted_docs.md")

with open(out_path, "w", encoding="utf-8") as f:
    for doc in docs:
        path = os.path.join(base_dir, doc)
        if os.path.exists(path):
            f.write("# " + doc + "\n\n")
            f.write(read_docx(path))
            f.write("\n\n---\n\n")

print("Successfully written extracted text to extracted_docs.md")
