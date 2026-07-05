const fs = require('fs');
const indexPath = 'D:/Synergech/Project/UIUX/features/index.html';

let html = fs.readFileSync(indexPath, 'utf8');

// 1. Contact Modal Body
html = html.replace(
  /<div class="modal fade" id="contactModal"[\s\S]*?<div class="modal-body p-4"/,
  match => match.replace('class="modal-body p-4"', 'id="dynamic-contact" class="modal-body p-4"')
);

// 2. About Us Modal Body
html = html.replace(
  /<div class="modal fade" id="aboutModal"[\s\S]*?<div class="modal-body p-4"/,
  match => match.replace('class="modal-body p-4"', 'id="dynamic-about" class="modal-body p-4"')
);

// 3. Resources Modal Body
// Wait, the resources modal doesn't have a regular body, it has the tab system.
// I'll add the ID to the tab content wrapper or something.
// Actually, the user asked to manage the "Resources" section. I will add an ID to a new div inside the modal body.
html = html.replace(
  /<div class="modal fade" id="resourcesModal"[\s\S]*?<div class="modal-body p-0"/,
  match => match.replace('class="modal-body p-0"', 'class="modal-body p-0" id="dynamic-resources"')
);

// 4. How We Hire
html = html.replace(
  /<h2 class="section-title">How We Hire<\/h2>\s*<\/div>\s*<div class="hiring-timeline-wrapper">/,
  '<h2 class="section-title">How We Hire</h2>\n</div>\n<div id="dynamic-how-we-hire" class="hiring-timeline-wrapper">'
);

// 5. Why Join Synergech
html = html.replace(
  /<h2 class="section-title">Why Join Synergech<\/h2>[\s\S]*?<\/div>\s*<div class="row g-4">/,
  match => match.replace('<div class="row g-4">', '<div id="dynamic-why-join" class="row g-4">')
);

// 6. Careers
// I will create an empty div for dynamic careers inside the about modal just in case.
html = html.replace(
  /<div id="dynamic-about" class="modal-body p-4"[\s\S]*?>/,
  match => match + '\n<div id="dynamic-careers"></div>'
);

fs.writeFileSync(indexPath, html, 'utf8');
console.log("IDs injected into index.html");
