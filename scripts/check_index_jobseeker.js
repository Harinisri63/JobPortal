const fs = require('fs');
const path = require('path');
const jobSeekerDir = 'D:/Synergech/Project/UIUX/features/JobSeeker';
const files = fs.readdirSync(jobSeekerDir).filter(f => f.endsWith('.html'));

files.forEach(f => {
  const content = fs.readFileSync(path.join(jobSeekerDir, f), 'utf8');
  const matches = content.match(/href=["']index\.html["']/g);
  if (matches) {
    console.log(f, matches);
  }
});
