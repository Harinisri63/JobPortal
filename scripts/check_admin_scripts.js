const fs = require('fs');
const content = fs.readFileSync('D:/Synergech/Project/UIUX/features/Admin/admin.html', 'utf8');
const matches = content.match(/<script.*?src=["'](.*?)["']/g);
if (matches) {
  matches.forEach(m => console.log(m));
}
