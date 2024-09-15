
# InvoiceExtractor

## Overview
InvoiceExtractor is a .NET application to extract invoice details from PDF documents using templates. It supports extracting fields like InvoiceNumber, InvoiceDate, Vendor, Description, and Amount.

### Main Application GUI
![Alt text](https://github.com/adilsezer/InvoiceExtractor/blob/master/InvoiceExtractor/Assets/invoice_extractor_screenshot.png?raw=true "InvoiceExtractor")

## Features
- Extracts multiple invoices, treating each PDF page as a separate invoice.
- Text extraction via **PdfPig**.
- PDF to Image conversion using **PDFtoImage**.
- Field extraction by keywords or X/Y coordinates if regex fails.

## Installation
1. Clone the repo:
   ```
   git clone https://github.com/adilsezer/InvoiceExtractor.git
   ```
2. Install dependencies:
   ```
   Install-Package UglyToad.PdfPig
   Install-Package PDFtoImage
   ```

## Usage
- Define extraction templates with fields and keywords.
- Upload PDF to extract invoices.
- If regex fails, use coordinates for field extraction.

## License
This project is licensed under MIT License.
