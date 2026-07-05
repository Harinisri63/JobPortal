const fs = require('fs');
const path = require('path');
const jobSeekerDir = 'D:/Synergech/Project/UIUX/features/JobSeeker';
const files = fs.readdirSync(jobSeekerDir).filter(f => f.endsWith('.html'));

files.forEach(f => {
  const file = path.join(jobSeekerDir, f);
  let content = fs.readFileSync(file, 'utf8');
  let original = content;

  // Replace all remaining href="../features/index.html" with href="../features/index.html"
  content = content.replace(/href=["']index\.html["']/g, 'href="../features/index.html"');

  if (content !== original) {
    fs.writeFileSync(file, content, 'utf8');
    console.log('Fixed remaining index links in', f);
  }
});
