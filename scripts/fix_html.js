const fs = require('fs');
const p = 'D:/Synergech/Project/UIUX/features/contentmanagement/contentmanagement.html';
let h = fs.readFileSync(p, 'utf8');

const replacement = `  <!-- BODY -->
  <div class="body-wrap">

    <!-- LEFT COLUMN -->
    <div class="left-col">

      <!-- Top Row Subtitle / Banner -->
      <div class="d-flex justify-content-between align-items-center" style="background: linear-gradient(135deg, var(--primary), #3B82F6); border-radius: 12px; padding: 16px 20px; color: #fff; margin-bottom: 5px;">
        <div>
          <div style="font-size: 0.95rem; font-weight: 800; margin-bottom: 2px;">Content Management</div>
          <div style="font-size: 0.72rem; opacity: 0.85;">Manage all portal content, articles, guide, FAQ and settings with full control.</div>
        </div>
        <button class="btn btn-light btn-sm fw-bold" onclick="document.querySelectorAll('.tabs button')[8].click();" style="color: var(--primary); border-radius: 6px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); font-size: 0.8rem;">
          <i class="fa-solid fa-pen-to-square me-1"></i> Edit Landing Page
        </button>
      </div>

      <!-- METRICS -->
      <div class="metrics-row">
        <div class="metric-card">
          <div class="lbl">Total Content</div>`;

h = h.replace('<div class="lbl">Total Content</div>', replacement);
fs.writeFileSync(p, h, 'utf8');
