const fs = require('fs');
const path = require('path');

const projectRoot = 'D:/Synergech/Project/UIUX';

// 1. Collect all files in the project
const allFiles = [];
function walk(dir) {
  const list = fs.readdirSync(dir);
  list.forEach(file => {
    const fullPath = path.join(dir, file);
    if (fs.statSync(fullPath).isDirectory()) {
      if (!fullPath.includes('.vs') && !fullPath.includes('node_modules') && !fullPath.includes('.git')) {
        walk(fullPath);
      }
    } else {
      allFiles.push(fullPath);
    }
  });
}
walk(projectRoot);

// Build a map of filename -> absolute path
// For duplicates (like index.html), we might need to handle carefully, but for most files like admin.html, it's unique.
const fileMap = {};
allFiles.forEach(f => {
  const basename = path.basename(f);
  if (!fileMap[basename]) fileMap[basename] = [];
  fileMap[basename].push(f);
});

// Helper to compute new relative path
function getNewRelativePath(currentFilePath, targetFilePath) {
  let rel = path.relative(path.dirname(currentFilePath), targetFilePath);
  rel = rel.replace(/\\/g, '/');
  if (!rel.startsWith('.') && !rel.startsWith('/')) {
    rel = './' + rel; // ensure it's a relative path format, though usually it's fine without ./
  }
  // Remove leading ./ for cleaner paths if desired, but let's just leave it or clean it:
  if (rel.startsWith('./') && rel.indexOf('/') === 1) {
    rel = rel.substring(2);
  }
  return rel;
}

const fileExtensionsToProcess = ['.html', '.js', '.css'];

allFiles.forEach(currentFile => {
  if (!fileExtensionsToProcess.includes(path.extname(currentFile).toLowerCase())) return;

  let content = fs.readFileSync(currentFile, 'utf8');
  let modified = false;

  // Regex patterns
  const patterns = [
    /href=["']([^"']+\.(?:html|css))([#?][^"']*)?["']/gi,
    /src=["']([^"']+\.(?:png|jpg|jpeg|gif|svg|js))(["'])/gi,
    /url\(['"]?([^'"\)]+\.(?:png|jpg|jpeg|gif|svg))['"]?\)/gi,
    /window\.location\.href\s*=\s*['"]([^'"]+\.html)([#?][^'"]*)?['"]/gi
  ];

  patterns.forEach(regex => {
    content = content.replace(regex, (match, urlPath, suffixOrQuote, offset) => {
      // Ignore absolute URLs
      if (urlPath.startsWith('http://') || urlPath.startsWith('https://') || urlPath.startsWith('//') || urlPath.startsWith('data:')) {
        return match;
      }
      
      const resolvedPath = path.resolve(path.dirname(currentFile), urlPath);
      
      // If path still exists, no need to change
      if (fs.existsSync(resolvedPath)) {
        return match;
      }

      // Path is broken! Let's find the file.
      const basename = path.basename(urlPath);
      const possibleTargets = fileMap[basename];

      if (possibleTargets && possibleTargets.length > 0) {
        let bestTarget = possibleTargets[0];
        
        // If there are multiple, try to guess based on folder matching
        if (possibleTargets.length > 1) {
           const urlParts = urlPath.split(/[\\/]/);
           if (urlParts.length > 1) {
             const parentDir = urlParts[urlParts.length - 2];
             const betterMatch = possibleTargets.find(t => path.basename(path.dirname(t)) === parentDir);
             if (betterMatch) bestTarget = betterMatch;
           }
        }

        const newRelPath = getNewRelativePath(currentFile, bestTarget);
        console.log(`Fixing in ${path.basename(currentFile)}: ${urlPath} -> ${newRelPath}`);
        modified = true;

        // Reconstruct the match
        if (match.startsWith('href=')) {
          return `href="${newRelPath}${suffixOrQuote || ''}"`;
        } else if (match.startsWith('src=')) {
          return `src="${newRelPath}"`;
        } else if (match.startsWith('url(')) {
          return `url('${newRelPath}')`;
        } else if (match.includes('window.location.href')) {
          return `window.location.href = '${newRelPath}${suffixOrQuote || ''}'`;
        }
      }

      return match; // fallback
    });
  });

  if (modified) {
    fs.writeFileSync(currentFile, content, 'utf8');
  }
});
