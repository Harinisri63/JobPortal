const fs = require('fs');
const path = 'D:/Synergech/Project/UIUX/features/index.html';
let html = fs.readFileSync(path, 'utf8');

// Only add if not already present
if (!html.includes('job-tags')) {
    const tagsHTML = `
              <div class="job-tags">
                <span class="job-tag">Remote</span>
                <span class="job-tag">Full-Time</span>
                <span class="job-tag">Tech</span>
              </div>`;
    
    // Replace all job-details-meta closing divs with the div + tagsHTML
    html = html.replace(/(<div class="job-details-meta mt-3">[\s\S]*?<\/div>)/g, `$1${tagsHTML}`);
    fs.writeFileSync(path, html, 'utf8');
    console.log('Added job-tags to index.html');
} else {
    console.log('job-tags already present');
}
