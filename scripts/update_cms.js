const fs = require('fs');

const htmlPath = 'D:/Synergech/Project/UIUX/features/contentmanagement/contentmanagement.html';
const jsPath = 'D:/Synergech/Project/UIUX/features/contentmanagement/contentmanagement.js';
const indexPath = 'D:/Synergech/Project/UIUX/features/index.html';

// 1. Update contentmanagement.html
let html = fs.readFileSync(htmlPath, 'utf8');

// Add Tab
if (!html.includes('LandingPages')) {
  html = html.replace('<button class="tab-btn" onclick="switchTab(this, \'Settings\')">Settings</button>',
    `<button class="tab-btn" onclick="switchTab(this, 'LandingPages')">Landing Page</button>\n<button class="tab-btn" onclick="switchTab(this, 'Settings')">Settings</button>`);
}

// Wrap main content
if (!html.includes('id="cms-main-view"')) {
  html = html.replace('<!-- FILTER BAR -->', '<div id="cms-main-view">\n<!-- FILTER BAR -->');
  
  // Find the closing of recent content activity card
  const endMain = html.indexOf('<!-- MODALS -->');
  if (endMain === -1) {
    const endMain2 = html.indexOf('<!-- Toast Notification -->');
    if (endMain2 !== -1) {
       html = html.slice(0, endMain2) + '</div>\n' + html.slice(endMain2);
    }
  } else {
    html = html.slice(0, endMain) + '</div>\n' + html.slice(endMain);
  }
}

// Add the new landing pages view
const landingPagesView = `
<!-- LANDING PAGES VIEW -->
<div id="landing-pages-view" style="display:none; padding-top: 20px;">
  <div class="card" style="margin-bottom: 20px;">
    <div class="card-hdr">
      <div class="card-title">Manage Landing Page Sections</div>
      <button class="btn-create" onclick="saveLandingPageContent()"><i class="fa-solid fa-floppy-disk"></i> Publish Changes</button>
    </div>
    <div style="padding: 20px;">
      <p style="color: var(--text-muted); font-size: 0.85rem; margin-bottom: 20px;">
        Edit the content for various sections of the landing page below. Basic HTML tags (like &lt;br&gt;, &lt;b&gt;, &lt;p&gt;, &lt;ul&gt;, &lt;li&gt;) are supported.
      </p>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">About Us</label>
        <textarea id="lp-about" style="width: 100%; height: 100px; padding: 10px; border: 1px solid var(--border); border-radius: 6px;"></textarea>
      </div>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">Contact</label>
        <textarea id="lp-contact" style="width: 100%; height: 100px; padding: 10px; border: 1px solid var(--border); border-radius: 6px;"></textarea>
      </div>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">Careers</label>
        <textarea id="lp-careers" style="width: 100%; height: 100px; padding: 10px; border: 1px solid var(--border); border-radius: 6px;"></textarea>
      </div>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">Resources</label>
        <textarea id="lp-resources" style="width: 100%; height: 100px; padding: 10px; border: 1px solid var(--border); border-radius: 6px;"></textarea>
      </div>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">How We Hire</label>
        <textarea id="lp-how-we-hire" style="width: 100%; height: 100px; padding: 10px; border: 1px solid var(--border); border-radius: 6px;"></textarea>
      </div>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">Why Join Synergech</label>
        <textarea id="lp-why-join" style="width: 100%; height: 100px; padding: 10px; border: 1px solid var(--border); border-radius: 6px;"></textarea>
      </div>

    </div>
  </div>
</div>
`;

if (!html.includes('landing-pages-view')) {
  // Insert it right after closing cms-main-view
  html = html.replace('</div>\n<!-- MODALS -->', '</div>\n' + landingPagesView + '<!-- MODALS -->');
  html = html.replace('</div>\n<!-- Toast Notification -->', '</div>\n' + landingPagesView + '<!-- Toast Notification -->');
}

fs.writeFileSync(htmlPath, html, 'utf8');

// 2. Update contentmanagement.js
let js = fs.readFileSync(jsPath, 'utf8');

const newSwitchTab = `
function switchTab(btn, filter) {
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  btn.classList.add('active');
  
  if (filter === 'LandingPages') {
    document.getElementById('cms-main-view').style.display = 'none';
    document.getElementById('landing-pages-view').style.display = 'block';
    loadLandingPageContent();
  } else {
    document.getElementById('cms-main-view').style.display = 'block';
    if(document.getElementById('landing-pages-view')) {
      document.getElementById('landing-pages-view').style.display = 'none';
    }
    selectedTabFilter = filter;
    applyFilters();
  }
}

// LANDING PAGE CONTENT MANAGEMENT LOGIC
const LP_KEY = 'landing_page_content';

function loadLandingPageContent() {
  const data = JSON.parse(localStorage.getItem(LP_KEY) || '{}');
  
  document.getElementById('lp-about').value = data.about || 'Welcome to ProPath & Synergech. We connect top talent with the best opportunities.';
  document.getElementById('lp-contact').value = data.contact || 'Email: support@propath.com\\nPhone: +1 (555) 123-4567';
  document.getElementById('lp-careers').value = data.careers || 'Join our dynamic team! Explore open positions across engineering, product, and sales.';
  document.getElementById('lp-resources').value = data.resources || 'Access our resume builder, salary predictor, and interview prep tools to accelerate your career.';
  document.getElementById('lp-how-we-hire').value = data.howWeHire || 'Our hiring process is designed to be transparent, fair, and fast. We evaluate skills over pedigree.';
  document.getElementById('lp-why-join').value = data.whyJoin || 'We offer competitive compensation, remote flexibility, and a culture of continuous learning.';
}

function saveLandingPageContent() {
  const data = {
    about: document.getElementById('lp-about').value,
    contact: document.getElementById('lp-contact').value,
    careers: document.getElementById('lp-careers').value,
    resources: document.getElementById('lp-resources').value,
    howWeHire: document.getElementById('lp-how-we-hire').value,
    whyJoin: document.getElementById('lp-why-join').value
  };
  
  localStorage.setItem(LP_KEY, JSON.stringify(data));
  showToast('✅ Landing Page content saved & published!', '#15803D');
}
`;

if (!js.includes('loadLandingPageContent')) {
  js = js.replace(/function switchTab\(btn, filter\) \{[\s\S]*?applyFilters\(\);\s*\}/, newSwitchTab);
  fs.writeFileSync(jsPath, js, 'utf8');
}

// 3. Update index.html
let indexHtml = fs.readFileSync(indexPath, 'utf8');

// Insert the dynamic loading script
const loaderScript = `
<script>
document.addEventListener('DOMContentLoaded', function() {
  const data = JSON.parse(localStorage.getItem('landing_page_content') || '{}');
  
  if (data.about) {
    const el = document.getElementById('dynamic-about');
    if(el) el.innerHTML = data.about;
  }
  if (data.contact) {
    const el = document.getElementById('dynamic-contact');
    if(el) el.innerHTML = data.contact;
  }
  if (data.careers) {
    const el = document.getElementById('dynamic-careers');
    if(el) el.innerHTML = data.careers;
  }
  if (data.resources) {
    const el = document.getElementById('dynamic-resources');
    if(el) el.innerHTML = data.resources;
  }
  if (data.howWeHire) {
    const el = document.getElementById('dynamic-how-we-hire');
    if(el) el.innerHTML = data.howWeHire;
  }
  if (data.whyJoin) {
    const el = document.getElementById('dynamic-why-join');
    if(el) el.innerHTML = data.whyJoin;
  }
});
</script>
`;

if (!indexHtml.includes('landing_page_content')) {
  indexHtml = indexHtml.replace('</body>', loaderScript + '\n</body>');
  fs.writeFileSync(indexPath, indexHtml, 'utf8');
}

console.log("Done.");
