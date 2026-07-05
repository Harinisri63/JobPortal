const fs = require('fs');
const path = require('path');

const adminDir = 'D:/Synergech/Project/UIUX/features/Admin';

function walk(dir) {
  let results = [];
  const list = fs.readdirSync(dir);
  list.forEach(file => {
    file = path.join(dir, file);
    const stat = fs.statSync(file);
    if (stat && stat.isDirectory()) {
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

  const relPath = path.relative(adminDir, file).replace(/\\/g, '/');
  const depth = relPath.split('/').length; 
  // admin.html is depth 1 inside Admin -> features/Admin/admin.html
  // usermanagement/usermanagement.html is depth 2 inside Admin

  // The link to features/index.html
  let target = '';
  if (depth === 1) {
    target = '../index.html';
  } else if (depth === 2) {
    target = '../../index.html';
  } else if (depth === 3) {
    target = '../../../index.html';
  }

  // Replace <a href="../features/index.html">Logout</a> or <a href="../features/index.html">Logout</a> with target
  content = content.replace(/href=["'][^"']*index\.html["']\s*>([^<]*Logout)/g, `href="${target}">$1`);
  
  if (content !== original) {
    fs.writeFileSync(file, content, 'utf8');
    console.log('Fixed Logout link in', file);
  }
});
