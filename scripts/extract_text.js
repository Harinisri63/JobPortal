const fs = require('fs');
const content = fs.readFileSync('D:/Synergech/Project/UIUX/features/index.html', 'utf8');
const lines = content.split('\n');
lines.forEach((l, i) => {
  if (l.includes('Our Story & Mission')) { console.log('About:', lines[i+1].trim()); }
  if (l.includes('Rayala Techno Park')) { console.log('Contact:', l.trim()); }
});
