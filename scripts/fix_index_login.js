const fs = require('fs');
const file = 'D:/Synergech/Project/UIUX/features/index.html';
let c = fs.readFileSync(file, 'utf8');

c = c.replace(/href=["']features\/auth\/login\.html["']/g, 'href="../features/auth/login.html"');
c = c.replace(/href=["']\.\.\/featues\/auth\/signup\.html["']/g, 'href="../features/auth/signup.html"');
c = c.replace(/href=["']auth\/login\.html["']/g, 'href="../features/auth/login.html"');
c = c.replace(/href=["']auth\/signup\.html["']/g, 'href="../features/auth/signup.html"');
// Fix any other references to auth
c = c.replace(/href=["'](?:.*?\/)?auth\/login\.html["']/g, 'href="../features/auth/login.html"');
c = c.replace(/href=["'](?:.*?\/)?auth\/signup\.html["']/g, 'href="../features/auth/signup.html"');

fs.writeFileSync(file, c, 'utf8');
console.log('Fixed auth links in index.html');
