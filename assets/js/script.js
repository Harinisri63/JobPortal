document.addEventListener('DOMContentLoaded', () => {  
  // Load Shared Navbar dynamically
  const navbarPlaceholder = document.getElementById('navbar-shared');
  if (navbarPlaceholder) {
    const basePath = navbarPlaceholder.getAttribute('data-basepath') || '';
    const navbarUrl = basePath + 'shared/navbar.html';
    
    fetch(navbarUrl)
      .then(response => response.text())
      .then(html => {
        const resolvedHtml = html.replace(/\[BASE_PATH\]/g, basePath);
        navbarPlaceholder.innerHTML = resolvedHtml;
        initNavbarScroll();
      })
      .catch(err => {
        console.warn('Fetch failed (likely local file protocol CORS). Attempting iframe fallback:', err);
        // Fallback using loaded iframe contents (works locally on most browsers)
        const iframe = document.getElementById('navbar-iframe-fallback');
        if (iframe) {
          iframe.addEventListener('load', () => {
            try {
              let iframeDoc = iframe.contentDocument || iframe.contentWindow.document;
              let navbarHtml = iframeDoc.body.innerHTML;
              if (navbarHtml) {
                const resolvedHtml = navbarHtml.replace(/\[BASE_PATH\]/g, basePath);
                navbarPlaceholder.innerHTML = resolvedHtml;
                initNavbarScroll();
              }
            } catch (iframeErr) {
              console.error('Iframe fallback blocked by local security policy. Please run this page via a local web server.', iframeErr);
            }
          });
        }
      });
  }

  function initNavbarScroll() {
    const navbar = navbarPlaceholder.querySelector('.navbar');
    if (navbar) {
      window.addEventListener('scroll', () => {
        if (window.scrollY > 20) {
          navbar.classList.add('scrolled');
        } else {
          navbar.classList.remove('scrolled');
        }
      });
    }
  }

  // Bookmark Toggle logic
  const bookmarkButtons = document.querySelectorAll('.bookmark-btn');
  bookmarkButtons.forEach(button => {
    button.addEventListener('click', (e) => {
      e.preventDefault();
      const icon = button.querySelector('i');
      if (icon.classList.contains('fa-regular')) {
        icon.classList.replace('fa-regular', 'fa-solid');
        icon.style.color = '#4F46E5';
      } else {
        icon.classList.replace('fa-solid', 'fa-regular');
        icon.style.color = '';
      }
    });
  });

  // Apply button alerts
  // Apply button logic removed as it now triggers a login modal in HTML
});
