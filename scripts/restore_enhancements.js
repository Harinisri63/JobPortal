const fs = require('fs');
const p = 'D:/Synergech/Project/UIUX/features/index.html';
let h = fs.readFileSync(p, 'utf8');

// 1. Job tags
if (!h.includes('job-tags')) {
  const tagsHTML = `
              <div class="job-tags">
                <span class="job-tag">Remote</span>
                <span class="job-tag">Full-Time</span>
                <span class="job-tag">Tech</span>
              </div>`;
  h = h.replace(/(<div class="job-details-meta mt-3">[\s\S]*?<\/div>)/g, `$1${tagsHTML}`);
}

// 2. AOS CSS & JS
if (!h.includes('aos.css')) {
  h = h.replace(/<link href="..\/assets\/css\/style.css"/, `<link href="https://unpkg.com/aos@2.3.1/dist/aos.css" rel="stylesheet" />\n  <link href="../assets/css/style.css"`);
}
if (!h.includes('aos.js')) {
  h = h.replace(/<script src="..\/assets\/bootstrap-5.1.3-dist\/js\/bootstrap.bundle.min.js"><\/script>/, `<script src="../assets/bootstrap-5.1.3-dist/js/bootstrap.bundle.min.js"></script>\n<script src="https://unpkg.com/aos@2.3.1/dist/aos.js"></script>\n<script>AOS.init();</script>`);
}

// 3. Particle Canvas CSS and Canvas
if (!h.includes('hero-particle-canvas')) {
  const particleCSS = `
  <style>
  #hero-particle-canvas {
    position: absolute;
    top: 0; left: 0;
    width: 100%; height: 100%;
    pointer-events: none;
    z-index: 0;
  }
  .hero-section { position: relative; }
  .hero-section .container { position: relative; z-index: 1; }
  </style>`;
  h = h.replace(/<\/head>/, `${particleCSS}\n</head>`);
  h = h.replace(/<section class="hero-section">/, `<section class="hero-section">\n    <canvas id="hero-particle-canvas"></canvas>`);
  
  // Particle JS
  const particleJS = `
<script>
  const canvas = document.getElementById('hero-particle-canvas');
  if (canvas) {
    const ctx = canvas.getContext('2d');
    canvas.width = window.innerWidth;
    canvas.height = canvas.parentElement.offsetHeight;
    const particles = [];
    for(let i=0; i<50; i++) {
      particles.push({
        x: Math.random() * canvas.width,
        y: Math.random() * canvas.height,
        r: Math.random() * 2 + 1,
        dx: (Math.random() - 0.5) * 0.5,
        dy: (Math.random() - 0.5) * 0.5
      });
    }
    function draw() {
      ctx.clearRect(0,0,canvas.width,canvas.height);
      ctx.fillStyle = 'rgba(79, 70, 229, 0.3)';
      particles.forEach(p => {
        ctx.beginPath();
        ctx.arc(p.x, p.y, p.r, 0, Math.PI*2);
        ctx.fill();
        p.x += p.dx;
        p.y += p.dy;
        if(p.x < 0 || p.x > canvas.width) p.dx *= -1;
        if(p.y < 0 || p.y > canvas.height) p.dy *= -1;
      });
      requestAnimationFrame(draw);
    }
    draw();
  }
</script>`;
  h = h.replace(/<\/body>/, `${particleJS}\n</body>`);
}

// 4. AOS Attributes (just a few on major sections)
h = h.replace(/<section class="featured-opportunities bg-white">/, '<section class="featured-opportunities bg-white" data-aos="fade-up" data-aos-duration="800">');
h = h.replace(/<div class="hero-content">/, '<div class="hero-content" data-aos="fade-up" data-aos-duration="1000">');

// 5. Login Prompt Modal and apply button changes
if (!h.includes('loginPromptModal')) {
  h = h.replace(/(<button class="apply-btn">Apply Now<\/button>)/g, '<button class="apply-btn" data-bs-toggle="modal" data-bs-target="#loginPromptModal">Apply Now</button>');
  
  const loginModal = `
<!-- Login Prompt Modal -->
<div class="modal fade" id="loginPromptModal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content text-center p-4">
      <div class="mb-3">
        <i class="fa-solid fa-lock fa-3x text-primary"></i>
      </div>
      <h4 class="fw-bold">Sign In Required</h4>
      <p class="text-muted">Please sign in or create an account to apply for jobs and access all features.</p>
      <div class="d-grid gap-2 mt-3">
        <a href="auth/login.html" class="btn btn-primary fw-bold py-2">Sign In</a>
        <button type="button" class="btn btn-light text-muted" data-bs-dismiss="modal">Cancel</button>
      </div>
    </div>
  </div>
</div>`;
  h = h.replace(/<\/body>/, `${loginModal}\n</body>`);
}

// 6. Google maps iframe
const newIframe = '<iframe src="https://maps.google.com/maps?q=Synergech+Technology+Solutions+Inc,+Chennai,+Tamil+Nadu,+India&output=embed&z=16" style="border:0;" allowfullscreen="" loading="lazy" referrerpolicy="no-referrer-when-downgrade"></iframe>';
h = h.replace(/<iframe src="https:\/\/www\.google\.com\/maps\/embed[\s\S]*?<\/iframe>/g, newIframe);

fs.writeFileSync(p, h, 'utf8');
console.log('Restored all enhancements!');
