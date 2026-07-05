const fs = require('fs');
const content = fs.readFileSync('D:/Synergech/Project/UIUX/features/index.html', 'utf8');
const lines = content.split('\n');
lines.forEach((l, i) => {
  if (l.includes('id="contactModal"') || l.includes('id="aboutModal"') || l.includes('class="footer"') || l.includes('id="contact"') || l.includes('id="about"')) {
    console.log((i+1) + ': ' + l);
  }
});
