const fs = require('fs');
const lines = fs.readFileSync('D:/Synergech/Project/UIUX/features/index.html', 'utf8').split('\n');
lines.forEach((l, i) => {
  if (l.includes('<ul class="navbar-nav')) {
    for (let j = i; j < i + 10; j++) console.log((j + 1) + ': ' + lines[j]?.trim());
  }
});
