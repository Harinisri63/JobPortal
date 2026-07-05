const fs = require('fs');

const p = 'D:/Synergech/Project/UIUX/features/index.html';
let h = fs.readFileSync(p, 'utf8');

// 1. Re-apply UseResume link
const resumeRegex = /<div class="tab-pane fade show active" id="v-pills-resume" role="tabpanel" aria-labelledby="v-pills-resume-tab">[\s\S]*?<!-- End PDF generation -->/;
const newResumeContent = `<div class="tab-pane fade show active" id="v-pills-resume" role="tabpanel" aria-labelledby="v-pills-resume-tab">
<div class="text-center py-5">
<i class="fa-solid fa-file-invoice fa-4x text-primary mb-4"></i>
<h4 class="fw-bold mb-3">Ready to build a professional resume?</h4>
<p class="text-muted mb-4">Create an outstanding resume in minutes using our advanced AI-powered platform.</p>
<a href="https://useresume.ai/account/resumes" target="_blank" class="btn btn-primary btn-lg fw-bold px-5 py-3 rounded-pill shadow-sm mt-3" style="background: linear-gradient(135deg, #4F46E5 0%, #7C3AED 100%); border: none; transition: transform 0.2s;">Open UseResume Platform <i class="fa-solid fa-arrow-up-right-from-square ms-2"></i></a>
</div>
<!-- End PDF generation -->`;
h = h.replace(resumeRegex, newResumeContent);

// 2. Add IDs for About Us and Contact without breaking layout
// Instead of replacing the modal-body, replace the text inside specific tags.
// For About, let's wrap the text inside "Our Story & Mission" paragraph.
h = h.replace(/ProPath is a premium, state-of-the-art job board built by[\s\S]*?growth start-ups\./, match => `<span id="dynamic-about">${match}</span>`);

// For Contact, let's replace the recruitment support email maybe? Or just add the ID to the whole modal body again?
// Wait, the user said "contat and about content should be same as the image i was attached".
// This means they want the original layout to be PRESERVED.
// BUT they also want to edit it!
// If they edit the entire HTML, they could break the layout. If they edit just the text, we have to know WHICH text.
// Let's just restore the loaderScript to ONLY update elements if they exist, AND to inject the entire original HTML as the default in contentmanagement.js!
// Actually, earlier I changed the defaults in contentmanagement.js to plain sentences.
// If the user's default is a plain sentence, and we inject it into the `modal-body`, it will replace the entire modal.
// To fix this, I will add specific IDs to the textual paragraphs ONLY.

// For About Us: 
// It's the paragraph under "Our Story & Mission".
// Already did it above: <span id="dynamic-about">...</span>

// For Contact:
// The Contact modal has:
// "Phone Inquiry": "+91 (44) 4344 9000"
// "Recruitment Support": "support@synergech.com"
// "Business Relations": "info@synergech.com"
// Let's just wrap the phone number and emails? No, the text area has "Email: support@... Phone: +1 555".
// If I use the text area for the ENTIRE modal, it will break the layout.
// I'll just change the loader script so it does NOT touch `dynamic-about` or `dynamic-contact`!
// I'll just remove `dynamic-about` and `dynamic-contact` from index.html entirely, so the loader script does nothing for them!
// Wait, then the user's edits in Content Management for About and Contact won't reflect on the landing page!
// Let's just put `dynamic-about` on the main paragraph in About, and `dynamic-contact` on the main phone number or email?
// Or better yet, let's change `loaderScript` so that it doesn't break the layout.

fs.writeFileSync(p, h, 'utf8');
console.log('Restored UseResume link and added dynamic-about');
