const fs = require('fs');
const html = fs.readFileSync('D:/Synergech/Project/UIUX/features/index.html', 'utf8');
const opens = (html.match(/<div/g) || []).length;
const closes = (html.match(/<\/div>/g) || []).length;
console.log('Div balance:', opens - closes);
