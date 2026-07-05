const fs = require('fs');

const jsPath = 'D:/Synergech/Project/UIUX/features/contentmanagement/contentmanagement.js';

let js = fs.readFileSync(jsPath, 'utf8');

if (!js.includes('LP_KEY')) {
  const content = `
// ── LANDING PAGE CONTENT MANAGEMENT LOGIC ──
const LP_KEY = 'landing_page_content';

function loadLandingPageContent() {
  const data = JSON.parse(localStorage.getItem(LP_KEY) || '{}');
  document.getElementById('lp-about').value = data.about || 'Welcome to ProPath & Synergech. We connect top talent with the best opportunities.';
  document.getElementById('lp-contact').value = data.contact || 'Email: support@propath.com\\nPhone: +1 (555) 123-4567';
  document.getElementById('lp-careers').value = data.careers || 'Join our dynamic team! Explore open positions across engineering, product, and sales.';
  document.getElementById('lp-resources').value = data.resources || 'Access our resume builder, salary predictor, and interview prep tools to accelerate your career.';
  document.getElementById('lp-how-we-hire').value = data.howWeHire || '<div class="col-lg-2 col-md-4 col-sm-6 offset-lg-1"><div class="timeline-step active"><div class="step-number">1</div><h3 class="step-title">Apply Online</h3><p class="step-desc">Submit your application and resume through our web portal.</p></div></div><div class="col-lg-2 col-md-4 col-sm-6"><div class="timeline-step"><div class="step-number">2</div><h3 class="step-title">Initial Screening</h3><p class="step-desc">Our team reviews your profile against role requirements.</p></div></div><div class="col-lg-2 col-md-4 col-sm-6"><div class="timeline-step"><div class="step-number">3</div><h3 class="step-title">Technical Assessment</h3><p class="step-desc">Complete a skill-specific challenge or coding test.</p></div></div><div class="col-lg-2 col-md-4 col-sm-6"><div class="timeline-step"><div class="step-number">4</div><h3 class="step-title">Final Interview</h3><p class="step-desc">Meet with hiring managers and team leaders.</p></div></div><div class="col-lg-2 col-md-4 col-sm-6"><div class="timeline-step"><div class="step-number">5</div><h3 class="step-title">Offer</h3><p class="step-desc">Receive an offer and begin your onboarding journey.</p></div></div>';
  document.getElementById('lp-why-join').value = data.whyJoin || '<div class="col-lg-3 col-sm-6"><div class="why-card soft-shadow"><div class="icon-box" style="background-color: rgba(79, 70, 229, 0.08); color: #4F46E5;"><i class="fa-solid fa-chart-line"></i></div><h4>Career Growth</h4><p>Our algorithms proactively connect developers with mentors and key career moves.</p></div></div><div class="col-lg-3 col-sm-6"><div class="why-card soft-shadow"><div class="icon-box" style="background-color: rgba(16, 185, 129, 0.08); color: #10B981;"><i class="fa-solid fa-laptop-code"></i></div><h4>Cutting-Edge Tech</h4><p>Work on next-generation platforms utilizing AI, advanced architectures, and best practices.</p></div></div><div class="col-lg-3 col-sm-6"><div class="why-card soft-shadow"><div class="icon-box" style="background-color: rgba(245, 158, 11, 0.08); color: #F59E0B;"><i class="fa-solid fa-users"></i></div><h4>Vibrant Culture</h4><p>Join a community of passionate individuals who value collaboration, diversity, and innovation.</p></div></div><div class="col-lg-3 col-sm-6"><div class="why-card soft-shadow"><div class="icon-box" style="background-color: rgba(236, 72, 153, 0.08); color: #EC4899;"><i class="fa-solid fa-heart-pulse"></i></div><h4>Wellness & Benefits</h4><p>Comprehensive health coverage, flexible remote work, and mental health support programs.</p></div></div>';
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
  setTimeout(() => { window.open('../../index.html?preview=true', '_blank'); }, 500);
}
`;
  
  fs.appendFileSync(jsPath, content, 'utf8');
}
