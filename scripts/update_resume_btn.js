const fs = require('fs');
const p = 'D:/Synergech/Project/UIUX/features/index.html';
let h = fs.readFileSync(p, 'utf8');

// Replace the Download button
const oldButton = '<button type="submit" class="btn btn-primary btn-sm w-100 py-2"><i class="fa-solid fa-download me-1"></i> Download PDF Resume</button>';
const newButton = '<a href="https://useresume.ai/account/resumes" target="_blank" class="btn btn-primary btn-sm w-100 py-2"><i class="fa-solid fa-arrow-up-right-from-square me-1"></i> Build Resume</a>';

h = h.replace(oldButton, newButton);

fs.writeFileSync(p, h, 'utf8');
console.log('Button updated!');
