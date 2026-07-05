const fs = require('fs');

const htmlPath = 'D:/Synergech/Project/UIUX/features/contentmanagement/contentmanagement.html';
const jsPath = 'D:/Synergech/Project/UIUX/features/contentmanagement/contentmanagement.js';
const indexPath = 'D:/Synergech/Project/UIUX/features/index.html';

let html = fs.readFileSync(htmlPath, 'utf8');

// 1. Add Tab
if (!html.includes('switchTab(this, \'LandingPages\')')) {
  html = html.replace('<button class="tab-btn" onclick="switchTab(this, \'Settings\')">Settings</button>',
    `<button class="tab-btn" onclick="switchTab(this, 'LandingPages')">Landing Page</button>\n<button class="tab-btn" onclick="switchTab(this, 'Settings')">Settings</button>`);
}

// 2. Wrap main dashboard in cms-main-view
if (!html.includes('id="cms-main-view"')) {
  html = html.replace('<!-- FILTER BAR -->', '<div id="cms-main-view">\n<!-- FILTER BAR -->');
  html = html.replace('<!-- EDITOR MODAL -->', '</div>\n\n<!-- EDITOR MODAL -->');
}

// 3. Add Landing Pages View (with Preview button)
const landingPagesView = `
<!-- LANDING PAGES VIEW -->
<div id="landing-pages-view" style="display:none; padding-top: 20px; padding-bottom: 40px; padding-left: 20px; padding-right: 20px;">
  <div class="card" style="margin-bottom: 20px; max-width: 900px; margin-left: auto; margin-right: auto;">
    <div class="card-hdr">
      <div class="card-title">Manage Landing Page Sections</div>
      <div>
        <button class="btn-ga" onclick="previewLandingPageContent()" style="margin-right: 10px;"><i class="fa-solid fa-eye"></i> Preview</button>
        <button class="btn-create" onclick="saveLandingPageContent()"><i class="fa-solid fa-floppy-disk"></i> Publish Changes</button>
      </div>
    </div>
    <div style="padding: 20px;">
      <p style="color: var(--text-muted); font-size: 0.85rem; margin-bottom: 20px;">
        Edit the content for various sections of the landing page below. Basic HTML tags (like &lt;br&gt;, &lt;b&gt;, &lt;p&gt;, &lt;ul&gt;, &lt;li&gt;) are supported.
      </p>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">About Us</label>
        <textarea id="lp-about" style="width: 100%; height: 100px; padding: 10px; border: 1px solid var(--border); border-radius: 6px; font-family: inherit;"></textarea>
      </div>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">Contact</label>
        <textarea id="lp-contact" style="width: 100%; height: 100px; padding: 10px; border: 1px solid var(--border); border-radius: 6px; font-family: inherit;"></textarea>
      </div>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">Careers</label>
        <textarea id="lp-careers" style="width: 100%; height: 100px; padding: 10px; border: 1px solid var(--border); border-radius: 6px; font-family: inherit;"></textarea>
      </div>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">Resources</label>
        <textarea id="lp-resources" style="width: 100%; height: 100px; padding: 10px; border: 1px solid var(--border); border-radius: 6px; font-family: inherit;"></textarea>
      </div>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">How We Hire</label>
        <textarea id="lp-how-we-hire" style="width: 100%; height: 150px; padding: 10px; border: 1px solid var(--border); border-radius: 6px; font-family: inherit;"></textarea>
      </div>

      <div style="margin-bottom: 20px;">
        <label style="font-weight: 600; display: block; margin-bottom: 8px;">Why Join Synergech</label>
        <textarea id="lp-why-join" style="width: 100%; height: 150px; padding: 10px; border: 1px solid var(--border); border-radius: 6px; font-family: inherit;"></textarea>
      </div>
    </div>
  </div>
</div>
`;

if (!html.includes('id="landing-pages-view"')) {
  html = html.replace('<!-- EDITOR MODAL -->', landingPagesView + '\n<!-- EDITOR MODAL -->');
}

fs.writeFileSync(htmlPath, html, 'utf8');

// 4. Update contentmanagement.js SAFELY
let js = fs.readFileSync(jsPath, 'utf8');

const oldSwitchTab = `function switchTab(btn, filter) {
document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
btn.classList.add('active');
selectedTabFilter = filter;
applyFilters();
}`;

const newSwitchTab = `function switchTab(btn, filter) {
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  btn.classList.add('active');
  
  if (filter === 'LandingPages') {
    const mainView = document.getElementById('cms-main-view');
    if (mainView) mainView.style.display = 'none';
    const lpView = document.getElementById('landing-pages-view');
    if (lpView) lpView.style.display = 'block';
    loadLandingPageContent();
  } else {
    const mainView = document.getElementById('cms-main-view');
    if (mainView) mainView.style.display = 'block';
    const lpView = document.getElementById('landing-pages-view');
    if (lpView) lpView.style.display = 'none';
    
    selectedTabFilter = filter;
    applyFilters();
  }
}

// ── LANDING PAGE CONTENT MANAGEMENT LOGIC ──
const LP_KEY = 'landing_page_content';

function loadLandingPageContent() {
  const data = JSON.parse(localStorage.getItem(LP_KEY) || '{}');
  
  document.getElementById('lp-about').value = data.about || 'Welcome to ProPath & Synergech. We connect top talent with the best opportunities.';
  document.getElementById('lp-contact').value = data.contact || 'Email: support@propath.com\\nPhone: +1 (555) 123-4567';
  document.getElementById('lp-careers').value = data.careers || 'Join our dynamic team! Explore open positions across engineering, product, and sales.';
  document.getElementById('lp-resources').value = data.resources || 'Access our resume builder, salary predictor, and interview prep tools to accelerate your career.';
  
  document.getElementById('lp-how-we-hire').value = data.howWeHire || 
    '<div class="col-lg-2 col-md-4 col-sm-6 offset-lg-1"><div class="timeline-step active"><div class="step-number">1</div><h3 class="step-title">Apply Online</h3><p class="step-desc">Submit your application and resume through our web portal.</p></div></div>' +
    '<div class="col-lg-2 col-md-4 col-sm-6"><div class="timeline-step"><div class="step-number">2</div><h3 class="step-title">Initial Screening</h3><p class="step-desc">Our team reviews your profile against role requirements.</p></div></div>' +
    '<div class="col-lg-2 col-md-4 col-sm-6"><div class="timeline-step"><div class="step-number">3</div><h3 class="step-title">Technical Assessment</h3><p class="step-desc">Complete a skill-specific challenge or coding test.</p></div></div>' +
    '<div class="col-lg-2 col-md-4 col-sm-6"><div class="timeline-step"><div class="step-number">4</div><h3 class="step-title">Final Interview</h3><p class="step-desc">Meet with hiring managers and team leaders.</p></div></div>' +
    '<div class="col-lg-2 col-md-4 col-sm-6"><div class="timeline-step"><div class="step-number">5</div><h3 class="step-title">Offer</h3><p class="step-desc">Receive an offer and begin your onboarding journey.</p></div></div>';
    
  document.getElementById('lp-why-join').value = data.whyJoin || 
    '<div class="col-lg-3 col-sm-6"><div class="why-card soft-shadow"><div class="icon-box" style="background-color: rgba(79, 70, 229, 0.08); color: #4F46E5;"><i class="fa-solid fa-chart-line"></i></div><h4>Career Growth</h4><p>Our algorithms proactively connect developers with mentors and key career moves.</p></div></div>' +
    '<div class="col-lg-3 col-sm-6"><div class="why-card soft-shadow"><div class="icon-box" style="background-color: rgba(16, 185, 129, 0.08); color: #10B981;"><i class="fa-solid fa-laptop-code"></i></div><h4>Cutting-Edge Tech</h4><p>Work on next-generation platforms utilizing AI, advanced architectures, and best practices.</p></div></div>' +
    '<div class="col-lg-3 col-sm-6"><div class="why-card soft-shadow"><div class="icon-box" style="background-color: rgba(245, 158, 11, 0.08); color: #F59E0B;"><i class="fa-solid fa-users"></i></div><h4>Vibrant Culture</h4><p>Join a community of passionate individuals who value collaboration, diversity, and innovation.</p></div></div>' +
    '<div class="col-lg-3 col-sm-6"><div class="why-card soft-shadow"><div class="icon-box" style="background-color: rgba(236, 72, 153, 0.08); color: #EC4899;"><i class="fa-solid fa-heart-pulse"></i></div><h4>Wellness & Benefits</h4><p>Comprehensive health coverage, flexible remote work, and mental health support programs.</p></div></div>';
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

function previewLandingPageContent() {
  // Save as preview draft
  const data = {
    about: document.getElementById('lp-about').value,
    contact: document.getElementById('lp-contact').value,
    careers: document.getElementById('lp-careers').value,
    resources: document.getElementById('lp-resources').value,
    howWeHire: document.getElementById('lp-how-we-hire').value,
    whyJoin: document.getElementById('lp-why-join').value
  };
  localStorage.setItem('preview_landing_page_content', JSON.stringify(data));
  showToast('Opening preview...', '#0A4BD2');
  
  // Open landing page with preview flag
  setTimeout(() => {
    window.open('../../index.html?preview=true', '_blank');
  }, 500);
}
`;

if (js.includes(oldSwitchTab)) {
  js = js.replace(oldSwitchTab, newSwitchTab);
  fs.writeFileSync(jsPath, js, 'utf8');
}

// 5. Update index.html for Preview Mode
let indexHtml = fs.readFileSync(indexPath, 'utf8');

const loaderScript = `
<script>
document.addEventListener('DOMContentLoaded', function() {
  const isPreview = window.location.search.includes('preview=true');
  const storageKey = isPreview ? 'preview_landing_page_content' : 'landing_page_content';
  const data = JSON.parse(localStorage.getItem(storageKey) || '{}');
  
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
  
  if (isPreview) {
    // Add a banner at the top indicating preview mode
    const banner = document.createElement('div');
    banner.style.cssText = 'position:fixed;top:0;left:0;width:100%;background:#EF4444;color:white;text-align:center;padding:8px;font-weight:bold;z-index:99999;font-family:sans-serif;box-shadow:0 2px 10px rgba(0,0,0,0.2);';
    banner.innerHTML = '<i class="fa-solid fa-eye"></i> PREVIEW MODE <span style="font-weight:normal;margin-left:10px;font-size:0.9em;">(Close this tab to return to editor)</span>';
    document.body.prepend(banner);
    document.body.style.marginTop = '40px';
  }
});
</script>
`;

if (indexHtml.includes('<script>\ndocument.addEventListener(\'DOMContentLoaded\', function() {\n  const data = JSON.parse(localStorage.getItem(\'landing_page_content\')')) {
  // Replace the old injected script with the new one
  const oldScriptRegex = /<script>\s*document\.addEventListener\('DOMContentLoaded',\s*function\(\)\s*{\s*const data = JSON\.parse\(localStorage\.getItem\('landing_page_content'\)[\s\S]*?<\/script>/;
  indexHtml = indexHtml.replace(oldScriptRegex, loaderScript.trim());
  fs.writeFileSync(indexPath, indexHtml, 'utf8');
} else if (!indexHtml.includes('preview_landing_page_content')) {
  indexHtml = indexHtml.replace('</body>', loaderScript + '\n</body>');
  fs.writeFileSync(indexPath, indexHtml, 'utf8');
}

console.log("Installation complete.");
