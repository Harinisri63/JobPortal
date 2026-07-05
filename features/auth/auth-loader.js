
function runAuthLoader(destinationUrl, isSignup) {
  // Hide auth container
  const authContainer = document.querySelector('.auth-container');
  if (authContainer) authContainer.style.display = 'none';

  // Create loader HTML
  const loaderHtml = `
    <div id="auth-transition-loader" class="active">
      <div class="loader-content">
        <div class="loader-counter" id="loaderCounter">0%</div>
        <div class="loader-track">
          <div class="loader-progress" id="loaderProgress"></div>
        </div>
        <div class="loader-status" id="loaderStatus"></div>
      </div>
      <div class="curtain-panel" id="loaderCurtain"></div>
    </div>
  `;
  document.body.insertAdjacentHTML('beforeend', loaderHtml);

  const counterEl = document.getElementById('loaderCounter');
  const progressEl = document.getElementById('loaderProgress');
  const statusEl = document.getElementById('loaderStatus');
  const curtainEl = document.getElementById('loaderCurtain');

  let pct = 0;
  
  function getStatusText(pctValue) {
    if (isSignup) {
      if (pctValue < 30) return "Creating your account";
      if (pctValue < 65) return "Setting things up";
      if (pctValue < 90) return "Preparing your dashboard";
      return "Almost there";
    } else {
      if (pctValue < 30) return "Signing in";
      if (pctValue < 65) return "Verifying credentials";
      if (pctValue < 90) return "Loading your dashboard";
      return "Almost there";
    }
  }

  statusEl.textContent = getStatusText(0);

  function step() {
    // randomize increment between 2 and 6
    const inc = Math.floor(Math.random() * 5) + 2;
    pct += inc;
    if (pct >= 100) pct = 100;

    counterEl.textContent = pct + '%';
    progressEl.style.width = pct + '%';
    statusEl.textContent = getStatusText(pct);

    if (pct < 100) {
      // randomize delay between 30ms and 90ms
      setTimeout(step, Math.floor(Math.random() * 60) + 30);
    } else {
      setTimeout(() => {
        curtainEl.classList.add('slide-up'); // Wait, the curtain is at bottom (100%), if it slides to 0 it covers everything, but loader content is behind it (z-index 2 vs 3). 
        // Actually, the requirements say: "curtain panel ... slides upward off-screen to reveal the destination page underneath".
        // So the curtain should start at 0, covering everything, then slide to -100%.
        // Let's modify the curtain behavior.
        
        // Hide the content first, then slide the loader background up? 
        // "a curtain panel ... slides upward off-screen to reveal the destination page underneath"
        // Let's just slide the whole loader itself up!
        const loaderObj = document.getElementById('auth-transition-loader');
        loaderObj.style.transform = 'translateY(-100%)';
        loaderObj.style.transition = 'transform 0.6s cubic-bezier(0.77, 0, 0.175, 1)';
        
        setTimeout(() => {
          window.location.href = destinationUrl;
        }, 600);
      }, 250);
    }
  }

  setTimeout(step, 50);
}
