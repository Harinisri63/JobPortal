const fs = require('fs');
const path = require('path');

function walk(dir) {
  let results = [];
  const list = fs.readdirSync(dir);
  list.forEach(file => {
    file = path.join(dir, file);
    const stat = fs.statSync(file);
    if (stat && stat.isDirectory()) {
      results = results.concat(walk(file));
    } else if (file.endsWith('.html') || file.endsWith('.js')) {
      results.push(file);
    }
  });
  return results;
}

walk('D:/Synergech/Project/UIUX/features').forEach(f => {
  const c = fs.readFileSync(f, 'utf8');
  const m = c.match(/window\.location\.href\s*=\s*['"][^'"]*['"]/g);
  if (m) console.log(f.replace(/\\/g, '/'), '->', m);
});
