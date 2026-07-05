const fs = require('fs');
const exceljs = require('exceljs');
const { Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell, WidthType, BorderStyle, HeadingLevel, ShadingType } = require('docx');
const PDFDocument = require('pdfkit');

const candidates = [
  { id: 'CND-10234', name: 'Arav Kumar', title: 'Senior DevOps Engineer', exp: '5-8 years', loc: 'Bangalore', status: 'Shortlisted', date: '12 May 2025' },
  { id: 'CND-10235', name: 'Priya Sharma', title: 'React Developer', exp: '3-5 years', loc: 'Pune', status: 'Interview', date: '11 May 2025' },
  { id: 'CND-10236', name: 'Rohan Joshi', title: 'Product Manager', exp: '4-7 years', loc: 'Hyderabad', status: 'Applied', date: '10 May 2025' },
  { id: 'CND-10237', name: 'Neha Singh', title: 'UI/UX Designer', exp: '2-4 years', loc: 'Delhi', status: 'Shortlisted', date: '09 May 2025' },
];

async function createFiles() {
  // ─────────────────────────────────────────────────────────────────────────────
  // EXCEL
  // ─────────────────────────────────────────────────────────────────────────────
  const workbook = new exceljs.Workbook();
  const sheet = workbook.addWorksheet('Candidates');
  
  sheet.columns = [
    { header: 'CANDIDATE ID', key: 'id', width: 15 },
    { header: 'NAME', key: 'name', width: 20 },
    { header: 'JOB TITLE', key: 'title', width: 25 },
    { header: 'EXPERIENCE', key: 'exp', width: 15 },
    { header: 'LOCATION', key: 'loc', width: 15 },
    { header: 'STATUS', key: 'status', width: 15 },
    { header: 'APPLIED ON', key: 'date', width: 15 },
  ];

  const headerRow = sheet.getRow(1);
  headerRow.eachCell((cell) => {
    cell.font = { bold: true, color: { argb: 'FFFFFFFF' } };
    cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF0A4BD2' } };
    cell.alignment = { vertical: 'middle', horizontal: 'center' };
  });

  candidates.forEach(c => {
    sheet.addRow(c);
  });
  
  await workbook.xlsx.writeFile('candidates_export.xlsx');
  
  // ─────────────────────────────────────────────────────────────────────────────
  // DOCX
  // ─────────────────────────────────────────────────────────────────────────────
  const tableRows = [];
  
  // Header
  tableRows.push(
    new TableRow({
      children: [
        'CANDIDATE ID', 'NAME', 'JOB TITLE', 'EXPERIENCE', 'LOCATION', 'STATUS', 'APPLIED DATE'
      ].map(text => new TableCell({
        children: [new Paragraph({ children: [new TextRun({ text, bold: true, color: "FFFFFF" })], alignment: 'center' })],
        shading: { fill: "0A4BD2", type: ShadingType.CLEAR, color: "auto" },
        margins: { top: 100, bottom: 100, left: 100, right: 100 }
      }))
    })
  );

  // Body
  candidates.forEach(c => {
    tableRows.push(
      new TableRow({
        children: Object.values(c).map((val, idx) => {
           const isBold = idx === 1 || idx === 2; // Name and Job Title
           return new TableCell({
             children: [new Paragraph({ children: [new TextRun({ text: val, bold: isBold })], alignment: idx===0?'center':'left' })],
             margins: { top: 100, bottom: 100, left: 100, right: 100 }
           });
        })
      })
    );
  });

  const doc = new Document({
    sections: [{
      properties: {},
      children: [
        new Paragraph({
          text: "ProPath Candidates Export",
          heading: HeadingLevel.HEADING_1,
        }),
        new Paragraph(""), // spacer
        new Table({
          rows: tableRows,
          width: { size: 100, type: WidthType.PERCENTAGE },
        })
      ]
    }]
  });
  const buffer = await Packer.toBuffer(doc);
  fs.writeFileSync('candidates_export.docx', buffer);
  
  // ─────────────────────────────────────────────────────────────────────────────
  // PDF
  // ─────────────────────────────────────────────────────────────────────────────
  const pdfDoc = new PDFDocument({ margin: 30, size: 'A4', layout: 'landscape' });
  pdfDoc.pipe(fs.createWriteStream('candidates_export.pdf'));
  
  pdfDoc.fontSize(20).font('Helvetica-Bold').text('ProPath Candidates Export', { align: 'left' });
  pdfDoc.moveDown(1);
  
  const headers = ['ID', 'NAME', 'JOB TITLE', 'EXPERIENCE', 'LOCATION', 'STATUS', 'APPLIED ON'];
  const colWidths = [70, 100, 150, 90, 90, 90, 100];
  
  let currentY = pdfDoc.y;
  let currentX = 30;
  
  // Header Row
  pdfDoc.rect(currentX, currentY, 780, 25).fill('#0A4BD2');
  pdfDoc.fill('#FFFFFF').fontSize(10).font('Helvetica-Bold');
  
  headers.forEach((h, i) => {
    pdfDoc.text(h, currentX + 5, currentY + 7, { width: colWidths[i], align: 'left' });
    currentX += colWidths[i];
  });
  
  currentY += 25;
  
  // Body Rows
  candidates.forEach((c, index) => {
    currentX = 30;
    
    // borders
    pdfDoc.rect(currentX, currentY, 780, 25).stroke('#CCCCCC');
    pdfDoc.fill('#000000').fontSize(10);
    
    const vals = Object.values(c);
    vals.forEach((v, i) => {
       pdfDoc.font(i === 1 || i === 2 ? 'Helvetica-Bold' : 'Helvetica');
       pdfDoc.text(v, currentX + 5, currentY + 7, { width: colWidths[i], align: 'left' });
       currentX += colWidths[i];
    });
    currentY += 25;
  });
  
  pdfDoc.end();
}

createFiles().catch(console.error);
