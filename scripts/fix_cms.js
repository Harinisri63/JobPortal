const fs = require('fs');

const htmlPath = 'D:/Synergech/Project/UIUX/features/contentmanagement/contentmanagement.html';
const jsPath = 'D:/Synergech/Project/UIUX/features/contentmanagement/contentmanagement.js';

let html = fs.readFileSync(htmlPath, 'utf8');

if (!html.includes('id="landing-pages-view"')) {
  html = html.replace('<!-- EDITOR MODAL -->', `<!-- LANDING PAGES VIEW -->
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

<!-- EDITOR MODAL -->`);
  fs.writeFileSync(htmlPath, html, 'utf8');
}

let js = fs.readFileSync(jsPath, 'utf8');

// Ensure we hide footer-grids when LandingPages is open
if (!js.includes("document.querySelector('.footer-grids').style.display = 'none';")) {
  js = js.replace(/if \(filter === 'LandingPages'\) {[\s\S]*?document.getElementById\('landing-pages-view'\).style.display = 'block';/, 
    `if (filter === 'LandingPages') {
    document.getElementById('cms-main-view').style.display = 'none';
    const fg = document.querySelector('.footer-grids');
    if (fg) fg.style.display = 'none';
    document.getElementById('landing-pages-view').style.display = 'block';`);
    
  js = js.replace(/} else {[\s\S]*?document.getElementById\('cms-main-view'\).style.display = 'block';/,
    `} else {
    document.getElementById('cms-main-view').style.display = 'block';
    const fg = document.querySelector('.footer-grids');
    if (fg) fg.style.display = 'flex';`);
    
  fs.writeFileSync(jsPath, js, 'utf8');
}
