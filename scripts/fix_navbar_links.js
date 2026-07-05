const fs = require('fs');
const p = 'D:/Synergech/Project/UIUX/features/index.html';
let h = fs.readFileSync(p, 'utf8');

// 1. Change Navbar links to anchors
h = h.replace('<li class="nav-item"><a class="nav-link" href="#" data-bs-toggle="modal" data-bs-target="#resourcesModal">Career Resources</a></li>', '<li class="nav-item"><a class="nav-link" href="#resources">Career Resources</a></li>');
h = h.replace('<li class="nav-item"><a class="nav-link" href="#" data-bs-toggle="modal" data-bs-target="#contactModal">Contact</a></li>', '<li class="nav-item"><a class="nav-link" href="#contact">Contact</a></li>');
h = h.replace('<li class="nav-item"><a class="nav-link" href="#" data-bs-toggle="modal" data-bs-target="#aboutModal">About Us</a></li>', '<li class="nav-item"><a class="nav-link" href="#about-us">About Us</a></li>');

// 2. Add About Us Section
if (!h.includes('id="about-us"')) {
  const aboutSection = `
  <section id="about-us" class="py-5 bg-white" data-aos="fade-up" data-aos-duration="800">
    <div class="container">
      <div class="row align-items-center">
        <div class="col-lg-6 mb-4 mb-lg-0">
          <h2 class="section-title mb-4">About ProPath & Synergech</h2>
          <h3 class="fw-bold mb-3 h4">Our Story & Mission</h3>
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
        </div>
        <div class="col-lg-5 offset-lg-1">
          <img src="../assets/Image/landing-image.jpg" onerror="this.src='https://images.unsplash.com/photo-1522071820081-009f0129c71c?auto=format&fit=crop&q=80&w=800'" alt="About Us" class="img-fluid rounded-4 shadow-lg" />
        </div>
      </div>
    </div>
  </section>
  `;
  // Insert before the footer
  h = h.replace(/<footer id="contact"/, aboutSection + '\n<footer id="contact"');
}

fs.writeFileSync(p, h, 'utf8');
console.log('Fixed navbar links and added About Us section!');
