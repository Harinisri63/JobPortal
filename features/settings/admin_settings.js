// admin_settings.js - Interaction logic for Admin Settings page

// ──────────────────────────────────────────────────────────────
//  TAB SWITCHING
// ──────────────────────────────────────────────────────────────
function switchTab(btn, tabName) {
  // Deactivate all tabs
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  
  if (btn) {
    btn.classList.add('active');
  } else {
    // Find the button by text content or mapping if needed. For now, match the lowercase text or predefined list
    document.querySelectorAll('.tab-btn').forEach(b => {
      if (b.textContent.toLowerCase() === tabName.toLowerCase()) {
        b.classList.add('active');
      }
    });
  }

  // Hide all tab panels
  const panels = ['general','security','notifications','integrations','storage','backup','services'];
  panels.forEach(p => {
    const el = document.getElementById('tab-' + p);
    if (el) el.style.display = 'none';
  });

  // Show selected tab panel
  const target = document.getElementById('tab-' + tabName);
  if (target) target.style.display = 'block';
}

// ──────────────────────────────────────────────────────────────
//  THEME TOGGLE
// ──────────────────────────────────────────────────────────────
function toggleTheme() {
  document.body.classList.toggle('dark-mode');
  const icon = document.getElementById('theme-toggle');
  if (icon) {
    if (document.body.classList.contains('dark-mode')) {
      icon.classList.remove('fa-regular', 'fa-lightbulb');
      icon.classList.add('fa-solid', 'fa-moon');
      localStorage.setItem('theme', 'dark');
    } else {
      icon.classList.remove('fa-solid', 'fa-moon');
      icon.classList.add('fa-regular', 'fa-lightbulb');
      localStorage.setItem('theme', 'light');
    }
  }
}

// Check saved theme on load
document.addEventListener('DOMContentLoaded', () => {
  if (localStorage.getItem('theme') === 'dark') {
    toggleTheme();
  }
});

// ──────────────────────────────────────────────────────────────
//  MAINTENANCE TOGGLE
// ──────────────────────────────────────────────────────────────
function toggleMaintenance(checkbox) {
  if (checkbox.checked) {
    showToast('⚠️ Maintenance Mode ENABLED. System is now offline for users.', '#D97706');
  } else {
    showToast('✅ Maintenance Mode DISABLED. System is back online.', '#15803D');
  }
}

// ──────────────────────────────────────────────────────────────
//  SAVE / RESET / EXPORT
// ──────────────────────────────────────────────────────────────
function saveSettings() {
  showToast('✅ All settings saved successfully!', '#15803D');
}

function resetSettings() {
  showToast('🔄 Settings reset to last saved values.', '#D97706');
}

// Dropdown click outside listener
document.addEventListener('click', (e) => {
  const exportBtn = document.querySelector('.btn-export');
  const exportDropdown = document.getElementById('export-dropdown');
  if (exportBtn && exportDropdown) {
    if (!exportBtn.contains(e.target) && !exportDropdown.contains(e.target)) {
      exportDropdown.style.display = 'none';
    }
  }
});

function toggleExportMenu() {
  const dropdown = document.getElementById('export-dropdown');
  if (dropdown) {
    dropdown.style.display = dropdown.style.display === 'none' ? 'block' : 'none';
  }
}

function downloadSettings(format) {
  toggleExportMenu();
  showToast(`📥 Exporting system configuration as ${format}...`, '#0A4BD2');
  setTimeout(() => {
    // Create a JSON-like blob to simulate export
    const config = {
      systemName: 'PROPATH Engine v2',
      version: '2.0.71',
      timeZone: 'UTC',
      language: 'English',
      twoFA: true,
      auditLogging: true,
      backupFrequency: 'Daily'
    };
    
    let content = '';
    let type = '';
    let ext = '';
    
    if (format === 'JSON') {
      content = JSON.stringify(config, null, 2);
      type = 'application/json';
      ext = 'json';
    } else if (format === 'CSV') {
      content = "Key,Value\n" + Object.entries(config).map(([k, v]) => `${k},${v}`).join('\n');
      type = 'text/csv';
      ext = 'csv';
    } else if (format === 'PDF') {
      // Just simulate PDF with a text blob since we don't have jsPDF loaded here
      content = "System Configuration Report\n==========================\n\n" + Object.entries(config).map(([k, v]) => `${k}: ${v}`).join('\n');
      type = 'application/pdf'; // Mock type
      ext = 'pdf';
    }
    
    const blob = new Blob([content], { type: type });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `System_Config_Backup.${ext}`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
    showToast(`✅ ${format} Export Complete!`, '#15803D');
  }, 1000);
}

// ──────────────────────────────────────────────────────────────
//  DANGER ZONE
// ──────────────────────────────────────────────────────────────
function confirmDeleteSystemData() {
  const confirmed = window.confirm(
    '⚠️ WARNING: This action is PERMANENT and IRREVERSIBLE.\n\nAll system data will be deleted. Are you sure you want to proceed?'
  );
  if (confirmed) {
    showToast('❌ System data deletion initiated. This cannot be undone.', '#EF4444');
  } else {
    showToast('Action cancelled — no data was deleted.', '#64748B');
  }
}

// ──────────────────────────────────────────────────────────────
//  PERFORMANCE BARS — live simulation
// ──────────────────────────────────────────────────────────────
function animatePerformanceBars() {
  const bars = [
    { fillSelector: '.perf-fill', values: [30, 40, 50, 65] }
  ];
  document.querySelectorAll('.perf-fill').forEach((bar, i) => {
    const targets = [30, 40, 50, 65];
    const target = targets[i] || 30;
    let current = 0;
    const step = target / 40;
    const interval = setInterval(() => {
      current = Math.min(current + step, target);
      bar.style.width = current + '%';
      if (current >= target) clearInterval(interval);
    }, 20);
  });
}

// ──────────────────────────────────────────────────────────────
//  TOAST
// ──────────────────────────────────────────────────────────────
function showToast(msg, bg) {
  const el = document.getElementById('toast');
  if (!el) return;
  el.textContent = msg;
  el.style.background = bg || '#15803D';
  el.classList.add('show');
  setTimeout(() => el.classList.remove('show'), 3500);
}

// ──────────────────────────────────────────────────────────────
//  INIT
// ──────────────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
  // Show General tab by default
  const panels = ['security','notifications','integrations','storage','backup','services'];
  panels.forEach(p => {
    const el = document.getElementById('tab-' + p);
    if (el) el.style.display = 'none';
  });
  document.getElementById('tab-general').style.display = 'block';

  // Start Dynamic Dashboard Telemetry
  initDynamicDashboard();
});

function initDynamicDashboard() {
  // --- 1. Dynamic System Performance Telemetry ---
  const perfRows = document.querySelectorAll('#tab-general .perf-row');
  if (perfRows.length >= 4) {
    setInterval(() => {
      // CPU
      let cpu = Math.floor(Math.random() * 25) + 20; // 20-45%
      let cpuFill = perfRows[0].querySelector('.perf-fill');
      let cpuText = perfRows[0].querySelector('.perf-row-hdr span:nth-child(2)');
      if (cpuFill && cpuText) { cpuFill.style.width = cpu + '%'; cpuText.textContent = cpu + '%'; }
      
      // Memory
      let mem = Math.floor(Math.random() * 15) + 35; // 35-50%
      let memFill = perfRows[1].querySelector('.perf-fill');
      let memText = perfRows[1].querySelector('.perf-row-hdr span:nth-child(2)');
      if (memFill && memText) { memFill.style.width = mem + '%'; memText.textContent = mem + '%'; }
      
      // Disk
      let disk = Math.floor(Math.random() * 30) + 15; // 15-45%
      let diskFill = perfRows[2].querySelector('.perf-fill');
      let diskText = perfRows[2].querySelector('.perf-row-hdr span:nth-child(2)');
      if (diskFill && diskText) { diskFill.style.width = disk + '%'; diskText.textContent = disk + '%'; }
      
      // Sessions
      let sessions = 1100 + Math.floor(Math.random() * 150);
      let sessPct = Math.min(100, (sessions / 2000) * 100);
      let sessFill = perfRows[3].querySelector('.perf-fill');
      let sessText = perfRows[3].querySelector('.perf-row-hdr span:nth-child(2)');
      if (sessFill && sessText) { sessFill.style.width = sessPct + '%'; sessText.textContent = sessions; }
      
      // Response Time
      const rtEl = document.querySelector('#tab-general .s-card:nth-child(5) .setting-row:last-child .setting-val');
      if (rtEl) {
        rtEl.textContent = (180 + Math.floor(Math.random() * 80)) + 'ms';
      }
    }, 2500);
  }
}

// ──────────────────────────────────────────────────────────────
//  CHANGE PASSWORD MODAL
// ──────────────────────────────────────────────────────────────
function openPwdModal() {
  const modal = document.getElementById('pwd-modal');
  if (modal) {
    document.getElementById('pwd-current').value = '';
    document.getElementById('pwd-new').value = '';
    document.getElementById('pwd-confirm').value = '';
    modal.style.display = 'flex';
  }
}

function closePwdModal() {
  const modal = document.getElementById('pwd-modal');
  if (modal) modal.style.display = 'none';
}

function submitChangePassword() {
  const cur = document.getElementById('pwd-current').value;
  const newP = document.getElementById('pwd-new').value;
  const conf = document.getElementById('pwd-confirm').value;
  
  if (!cur || !newP || !conf) {
    showToast('Please fill all password fields.', '#EF4444');
    return;
  }
  if (newP !== conf) {
    showToast('New passwords do not match.', '#EF4444');
    return;
  }
  
  // Mock API Call
  showToast('Password changed successfully!', '#15803D');
  closePwdModal();
}
