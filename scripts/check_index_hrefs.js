const fs = require('fs');
const file = 'D:/Synergech/Project/UIUX/features/index.html';
const c = fs.readFileSync(file, 'utf8');
const matches = c.match(/href=["'][^"']*["']/g);
if (matches) {
  matches.forEach(m => console.log(m));
}
