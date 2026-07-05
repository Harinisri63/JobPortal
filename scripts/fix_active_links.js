const fs = require('fs');
const path = require('path');

const adminDir = 'D:/Synergech/Project/UIUX/features/Admin';

function walk(dir) {
  let results = [];
  const list = fs.readdirSync(dir);
  list.forEach(file => {
    file = path.join(dir, file);
    if (fs.statSync(file).isDirectory()) {
      results = results.concat(walk(file));
    } else if (file.endsWith('.html')) {
      results.push(file);
    }
  });
  return results;
}

const files = walk(adminDir);

files.forEach(file => {
  let content = fs.readFileSync(file, 'utf8');
  let original = content;

  // Find all <a href="...">...</a> class="active" or similar
  // We can just use regex to replace the href of the active sidebar link
  content = content.replace(/href=["'][^"']*?["']([^>]*class=["'][^"']*?active[^"']*?["'])/g, 'href="javascript:void(0);"$1');
  
  if (content !== original) {
    fs.writeFileSync(file, content, 'utf8');
    console.log('Fixed active link in', file);
  }
});
