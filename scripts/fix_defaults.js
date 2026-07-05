const fs = require('fs');
const p = 'D:/Synergech/Project/UIUX/features/index.html';
let h = fs.readFileSync(p, 'utf8');

// 1. Careers
const careersText = 'We are actively recruiting software developers, product owners, and growth specialists across our Chennai, Bangalore, and Pune centers. Check out our careers section to learn more.';
if (h.includes(careersText) && !h.includes('id="dynamic-careers"')) {
  h = h.replace(careersText, `<span id="dynamic-careers">${careersText}</span>`);
}

// 2. Resources (CTA description)
const resourcesText = 'Explore opportunities matching your professional criteria and connect with premium recruiters instantly.';
if (h.includes(resourcesText) && !h.includes('id="dynamic-resources"')) {
  h = h.replace(resourcesText, `<span id="dynamic-resources">${resourcesText}</span>`);
}

// Write HTML
fs.writeFileSync(p, h, 'utf8');

// Now update contentmanagement.js defaults
const jsPath = 'D:/Synergech/Project/UIUX/features/contentmanagement/contentmanagement.js';
let js = fs.readFileSync(jsPath, 'utf8');

const aboutText = 'ProPath is a premium, state-of-the-art job board built by Synergech to empower high-potential design, product, and tech professionals. Our mission is to accelerate engineering careers by bridging connections to top-tier organizations and high-growth start-ups.';
const contactText = 'Rayala Techno Park, 1st Floor, 144, Rajiv Gandhi Salai (OMR), Kottivakkam, Chennai, Tamil Nadu 600041';

js = js.replace(/data\.about \|\| '.*?'/g, `data.about || '${aboutText}'`);
js = js.replace(/data\.contact \|\| '.*?'/g, `data.contact || '${contactText}'`);
js = js.replace(/data\.careers \|\| '.*?'/g, `data.careers || '${careersText}'`);
js = js.replace(/data\.resources \|\| '.*?'/g, `data.resources || '${resourcesText}'`);

fs.writeFileSync(jsPath, js, 'utf8');

console.log('Fixed IDs and JS defaults');
