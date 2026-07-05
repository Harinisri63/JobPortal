const fs = require('fs');

const p = 'D:/Synergech/Project/UIUX/features/index.html';
let h = fs.readFileSync(p, 'utf8');

const loaderScript = `
<script>
document.addEventListener('DOMContentLoaded', function() {
  const isPreview = window.location.search.includes('preview=true');
  const storageKey = isPreview ? 'preview_landing_page_content' : 'landing_page_content';
  const data = JSON.parse(localStorage.getItem(storageKey) || '{}');
  
  if (data.about) {
    const el = document.getElementById('dynamic-about');
    if(el) {
      // Replace newline with br tags for nice formatting
      el.innerHTML = data.about.replace(/n/g, '<br>');
    }
  }
  if (data.contact) {
    const el = document.getElementById('dynamic-contact');
    if(el) {
      el.innerHTML = data.contact.replace(/n/g, '<br>');
    }
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

if (!h.includes('preview_landing_page_content')) {
  h = h.replace('</body>', loaderScript + '\n</body>');
  
  // Let's add dynamic-contact ID to a specific place in the Contact Modal
  // I will just wrap the text inside the phone number or something, or I will just let it be.
  // The user wants it to look like the image, so I won't ruin the whole modal.
  // Let's put dynamic-contact around the Corporate Office address.
  h = h.replace(/Rayala Techno Park[\s\S]*?600041/, match => \`<span id="dynamic-contact">\${match}</span>\`);
  
  fs.writeFileSync(p, h, 'utf8');
}
