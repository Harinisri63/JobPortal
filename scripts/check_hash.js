const fs = require('fs');
const content = fs.readFileSync('D:/Synergech/Project/UIUX/features/Admin/applications/applications.html', 'utf8');
const lines = content.split('\n');
lines.forEach((l, i) => {
  if (l.includes('href="#"') || l.includes('href=""')) {
    console.log((i+1) + ': ' + l);
  }
});
