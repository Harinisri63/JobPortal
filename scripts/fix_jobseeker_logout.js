const fs = require('fs');
const path = require('path');

const jobSeekerDir = 'D:/Synergech/Project/UIUX/features/JobSeeker';

const files = fs.readdirSync(jobSeekerDir).filter(f => f.endsWith('.html'));

files.forEach(f => {
  const file = path.join(jobSeekerDir, f);
  let content = fs.readFileSync(file, 'utf8');
  let original = content;

  // Fix image name
  content = content.replace(/user-image\.png/g, 'profile.png');

  // Fix Sign out link
  // The logout link is usually href="../features/index.html" or href="../features/index.html". 
  // features/index.html is one level up from JobSeeker.
  content = content.replace(/href=["']index\.html["']\s*>([^<]*Sign out)/g, 'href="../features/index.html">$1');

  if (content !== original) {
    fs.writeFileSync(file, content, 'utf8');
    console.log('Fixed', f);
  }
});
