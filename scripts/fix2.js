const fs = require('fs');

const path = 'D:/Synergech/Project/UIUX/features/reports/reports.js';
let content = fs.readFileSync(path, 'utf8');

content = content.replace(/Priya Ma'am/g, "Priya Maam");

fs.writeFileSync(path, content, 'utf8');
console.log("Done");
