const fs = require('fs');
const p = 'D:/Synergech/Project/UIUX/features/index.html';
let h = fs.readFileSync(p, 'utf8');

const modalsHTML = `
<!-- About Us Modal -->
<div class="modal fade" id="aboutModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-lg">
    <div class="modal-content">
      <div class="modal-header" style="background-color: #059669; color: white;">
        <h5 class="modal-title"><i class="fa-solid fa-circle-info me-2"></i> About ProPath & Synergech</h5>
        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body p-4">
        <h3 class="fw-bold mb-3">Our Story & Mission</h3>
        <p class="text-muted mb-4"><span id="dynamic-about">ProPath is a premium, state-of-the-art job board built by Synergech to empower high-potential design, product, and tech professionals. Our mission is to accelerate engineering careers by bridging connections to top-tier organizations and high-growth start-ups.</span></p>
        <div class="row g-4 mb-4">
          <div class="col-md-6">
            <div class="p-4 text-center rounded soft-shadow" style="background-color: #F8FAFC;">
              <h2 class="fw-bold text-success mb-2">100%</h2>
              <p class="text-muted mb-0">Verified Job Listings</p>
            </div>
          </div>
          <div class="col-md-6">
            <div class="p-4 text-center rounded soft-shadow" style="background-color: #F8FAFC;">
              <h2 class="fw-bold text-success mb-2">45+</h2>
              <p class="text-muted mb-0">Corporate Partners</p>
            </div>
          </div>
        </div>
        <h5 class="fw-bold mb-2">Join Our Team</h5>
        <p class="text-muted mb-0">We are actively recruiting software developers, product owners, and growth specialists across our Chennai, Bangalore, and Pune centers. Check out our careers section to learn more.</p>
      </div>
    </div>
  </div>
</div>

<!-- Contact Modal -->
<div class="modal fade" id="contactModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-xl">
    <div class="modal-content">
      <div class="modal-header" style="background-color: #6366F1; color: white;">
        <h5 class="modal-title"><i class="fa-solid fa-envelope-open me-2"></i> Contact ProPath</h5>
        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body p-0">
        <div class="row g-0">
          <div class="col-md-5 p-4 p-lg-5 border-end">
            <h5 class="text-primary mb-4 fw-bold">Communication Channels</h5>
            <div class="d-flex align-items-center mb-4">
              <div class="icon-box me-3" style="background-color: #EFF6FF; color: #3B82F6; width: 48px; height: 48px; border-radius: 8px; display: flex; align-items: center; justify-content: center;"><i class="fa-solid fa-phone"></i></div>
              <div>
                <p class="text-muted mb-1 small">Phone Inquiry</p>
                <h6 class="mb-0">+91 (44) 4344 9000</h6>
              </div>
            </div>
            <div class="d-flex align-items-center mb-4">
              <div class="icon-box me-3" style="background-color: #EFF6FF; color: #3B82F6; width: 48px; height: 48px; border-radius: 8px; display: flex; align-items: center; justify-content: center;"><i class="fa-solid fa-envelope"></i></div>
              <div>
                <p class="text-muted mb-1 small">Recruitment Support</p>
                <h6 class="mb-0">support@synergech.com</h6>
              </div>
            </div>
            <div class="d-flex align-items-center mb-4">
              <div class="icon-box me-3" style="background-color: #EFF6FF; color: #3B82F6; width: 48px; height: 48px; border-radius: 8px; display: flex; align-items: center; justify-content: center;"><i class="fa-solid fa-briefcase"></i></div>
              <div>
                <p class="text-muted mb-1 small">Business Relations</p>
                <h6 class="mb-0">info@synergech.com</h6>
              </div>
            </div>
            <hr class="my-4">
            <h5 class="text-primary mb-3 fw-bold">Corporate Office</h5>
            <p class="text-muted fw-bold mb-1">Synergech Chennai Office:</p>
            <p class="text-muted mb-0"><span id="dynamic-contact">Rayala Techno Park, 1st Floor, 144, Rajiv Gandhi Salai (OMR), Kottivakkam, Chennai, Tamil Nadu 600041</span></p>
          </div>
          <div class="col-md-7">
            <iframe src="https://maps.google.com/maps?q=Synergech+Technology+Solutions+Inc,+Chennai,+Tamil+Nadu,+India&output=embed&z=16" width="100%" height="100%" style="border:0; min-height: 400px;" allowfullscreen="" loading="lazy"></iframe>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>

<!-- Career Resources Modal -->
<div class="modal fade" id="resourcesModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-lg">
    <div class="modal-content">
      <div class="modal-header" style="background-color: #4F46E5; color: white;">
        <h5 class="modal-title"><i class="fa-solid fa-book-open me-2"></i> Career Resources</h5>
        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body p-4 text-center">
        <i class="fa-solid fa-laptop-code fa-4x text-primary mb-3"></i>
        <h4 class="fw-bold">Level Up Your Career</h4>
        <p class="text-muted"><span id="dynamic-resources">Explore opportunities matching your professional criteria and connect with premium recruiters instantly.</span></p>
        <div class="row g-3 mt-4 text-start">
          <div class="col-md-6">
            <div class="p-3 border rounded">
              <h6 class="fw-bold"><i class="fa-solid fa-file-pdf text-danger me-2"></i> Resume Guides</h6>
              <p class="text-muted small mb-0">Learn how to build ATS-friendly resumes.</p>
            </div>
          </div>
          <div class="col-md-6">
            <div class="p-3 border rounded">
              <h6 class="fw-bold"><i class="fa-solid fa-video text-success me-2"></i> Interview Prep</h6>
              <p class="text-muted small mb-0">Top behavioral and technical questions.</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>
`;

if (!h.includes('id="contactModal"')) {
  h = h.replace('</body>', modalsHTML + '\n</body>');
  fs.writeFileSync(p, h, 'utf8');
  console.log('Modals added!');
} else {
  console.log('Modals already exist.');
}
