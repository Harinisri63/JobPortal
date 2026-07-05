const fs = require('fs');
const path = require('path');

const rootDir = 'D:/Synergech/Project/UIUX';
const featuresDir = path.join(rootDir, 'features');
const adminDir = path.join(featuresDir, 'Admin');
const seekerDir = path.join(featuresDir, 'JobSeeker');

function walk(dir) {
  let results = [];
  const list = fs.readdirSync(dir);
  list.forEach(file => {
    file = path.join(dir, file);
    const stat = fs.statSync(file);
    if (stat && stat.isDirectory()) {
      results = results.concat(walk(file));
    } else if (file.endsWith('.html') || file.endsWith('.css') || file.endsWith('.js')) {
      results.push(file);
    }
  });
  return results;
}

const files = walk(featuresDir);

files.forEach(file => {
  let content = fs.readFileSync(file, 'utf8');
  let originalContent = content;

  // Determine file depth from features/
  // e.g. features/Admin/admin.html -> Admin/admin.html (depth 2 from features)
  // features/Admin/auth/login.html -> Admin/auth/login.html (depth 3 from features)
  const relPath = path.relative(featuresDir, file).replace(/\\/g, '/');
  const depth = relPath.split('/').length; 

  // Fix assets and shared links
  // Previously, Admin/admin.html was at features/admin.html (depth 1). It used ../assets/
  // Now it's at features/Admin/admin.html (depth 2). It needs ../../assets/
  
  if (depth === 2) {
    // files like features/Admin/admin.html or features/JobSeeker/seeker.html
    // Replace ../assets/ with ../../assets/
    content = content.replace(/(["'])\.\.\/assets\//g, '$1../../assets/');
    content = content.replace(/(["'])\.\.\/shared\//g, '$1../../shared/');
    // Link to features/index.html
    content = content.replace(/(["'])\.\.\/index\.html(["'])/g, '$1../../index.html$2');
  } else if (depth === 3) {
    // files like features/Admin/auth/login.html
    // Replace ../../assets/ with ../../../assets/
    content = content.replace(/(["'])\.\.\/\.\.\/assets\//g, '$1../../../assets/');
    content = content.replace(/(["'])\.\.\/\.\.\/shared\//g, '$1../../../shared/');
    // Link to features/index.html
    content = content.replace(/(["'])\.\.\/index\.html(["'])/g, '$1../../index.html$2');
    
    // Cross-module fixes
    // login.html points to ../seeker.html -> should be ../../JobSeeker/seeker.html
    content = content.replace(/(["'])\.\.\/seeker\.html(["'])/g, '$1../../JobSeeker/seeker.html$2');
    // anything pointing to ../JobSeeker/seeker.html that was mistakenly fixed
    
    // what if seeker points to admin? It doesn't usually.
  }
  
  // Specific fix for JS window.location
  if (file.endsWith('.js') || file.endsWith('.html')) {
     content = content.replace(/window\.location\.href\s*=\s*(["'])\.\.\/seeker\.html(["'])/g, 'window.location.href = $1../../JobSeeker/seeker.html$2');
     content = content.replace(/runAuthLoader\((["'])\.\.\/seeker\.html(["'])/g, 'runAuthLoader($1../../JobSeeker/seeker.html$2');
  }

  if (content !== originalContent) {
    fs.writeFileSync(file, content, 'utf8');
    console.log('Fixed paths in:', file);
  }
});
console.log('Path update complete.');
