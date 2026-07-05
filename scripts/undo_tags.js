const fs = require('fs');
const path = 'D:/Synergech/Project/UIUX/features/index.html';
let html = fs.readFileSync(path, 'utf8');

// Regex to remove the job tags
html = html.replace(/\s*<div class="job-tags">[\s\S]*?<\/div>/g, '');
fs.writeFileSync(path, html, 'utf8');
console.log('Removed job-tags');
