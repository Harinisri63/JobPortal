// backup_recovery.js - Interaction handler for Backup & Recovery Dashboard

let defaultBackupsData = [
  { name: 'Full System Backup', type: 'Full', included: 'All Databases, Media', schedule: 'Weekly', dateTime: 'May 15, 14:30', size: '12 GB', status: 'Success' },
  { name: 'Database Backup', type: 'Incremental', included: 'Candidate, Jobs Schema', schedule: 'Daily', dateTime: 'May 15, 12:00', size: '2 GB', status: 'Success' },
  { name: 'Files & Media Backup', type: 'Differential', included: 'Resumes, Media Files', schedule: 'Weekly', dateTime: 'May 14, 02:00', size: '14 GB', status: 'Success' },
  { name: 'Incremental Backup', type: 'Incremental', included: 'Logs, Session Storage', schedule: 'Hourly', dateTime: 'May 15, 15:00', size: '1 GB', status: 'In Progress' },
  { name: 'Full System Backup', type: 'Full', included: 'All Databases, Media', schedule: 'Weekly', dateTime: 'May 08, 14:30', size: '12 GB', status: 'Failed' }
];

let backupsData = JSON.parse(localStorage.getItem('backupsData'));

// Auto-reset legacy large data to fit within D drive capacity
let needsReset = false;
if (backupsData) {
  let totalTestSize = 0;
  backupsData.forEach(b => {
    let s = parseFloat(b.size) || 0;
    if (b.size.includes('TB')) s *= 1024;
    totalTestSize += s;
  });
  if (totalTestSize > 163) needsReset = true;
}

if (!backupsData || needsReset) {
  backupsData = defaultBackupsData;
  localStorage.setItem('backupsData', JSON.stringify(backupsData));
}

let defaultSchedulesData = [
  { name: 'Daily Full Backup', frequency: 'Every 24h', source: 'All Databases, Media', nextRun: 'Today, 23:00', status: 'Active' },
  { name: 'Database Backup', frequency: 'Every 6h', source: 'Candidate, Jobs Schema', nextRun: 'Today, 18:00', status: 'Active' },
  { name: 'Files & Media Backup', frequency: 'Every Sunday', source: 'Resumes, Media Files', nextRun: 'May 19, 02:00', status: 'Active' },
  { name: 'Incremental Backup', frequency: 'Every 1h', source: 'Logs, Session Storage', nextRun: 'Today, 16:00', status: 'Active' }
];

let activeSchedulesData = JSON.parse(localStorage.getItem('activeSchedulesData')) || defaultSchedulesData;

let healthData = [
  { period: 'Last 7 Days', score: '100%' },
  { period: 'Last 30 Days', score: '96.7%' },
  { period: 'Last 90 Days', score: '97.8%' }
];

let defaultRestoreActivityData = [
  { name: 'Database Restore', status: 'Success', date: 'May 12, 10:00' },
  { name: 'Files Restore', status: 'Success', date: 'May 10, 08:30' },
  { name: 'System Restore', status: 'Failed', date: 'May 08, 14:15' },
  { name: 'Database Restore', status: 'Success', date: 'May 03, 11:20' }
];

let restoreActivityData = JSON.parse(localStorage.getItem('restoreActivityData')) || defaultRestoreActivityData;

let defaultRetentionData = [
  { type: 'Daily Backups', period: '7 Days', count: '7 / 7', size: '294 GB', next: 'Tomorrow, 00:00' },
  { type: 'Weekly Backups', period: '4 Weeks', count: '4 / 4', size: '2.04 TB', next: 'May 20, 00:00' },
  { type: 'Monthly Backups', period: '12 Months', count: '12 / 12', size: '6.12 TB', next: 'Jan 01, 00:00' },
  { type: 'Yearly Backups', period: '3 Years', count: '2 / 3', size: '3.56 TB', next: 'Dec 31, 00:00' }
];

let retentionData = JSON.parse(localStorage.getItem('retentionData')) || defaultRetentionData;

let currentPage = 1;
const itemsPerPage = 8;

function statusBadge(status) {
  if (status === 'Success') return `<span class="badge-success">Success</span>`;
  if (status === 'In Progress') return `<span class="badge-progress">In Progress</span>`;
  return `<span class="badge-failed">Failed</span>`;
}

function renderSummaryChart(data) {
  const total = data.length;
  let succ = 0, fail = 0, prog = 0;
  
  data.forEach(b => {
    if (b.status === 'Success') succ++;
    else if (b.status === 'Failed') fail++;
    else if (b.status === 'In Progress') prog++;
  });
  
  document.getElementById('summary-total').textContent = total;
  document.getElementById('summary-success').textContent = succ;
  document.getElementById('summary-failed').textContent = fail;
  document.getElementById('summary-progress').textContent = prog;
  
  const circle = document.getElementById('summary-circle');
  if (circle && total > 0) {
    const succPct = (succ / total) * 100;
    const failPct = (fail / total) * 100;
    const progPct = (prog / total) * 100;
    
    // Conic gradient: var(--primary) for success, #EF4444 for fail, #F59E0B for progress
    const p1 = succPct;
    const p2 = p1 + failPct;
    
    circle.style.background = `conic-gradient(
      var(--primary) 0% ${p1}%, 
      #EF4444 ${p1}% ${p2}%, 
      #F59E0B ${p2}% 100%
    )`;
  } else if (circle) {
    circle.style.background = '#e5e7eb';
  }
}

function renderBackups(data) {
  renderSummaryChart(data);
  const tbody = document.getElementById('backup-tbody');
  if (!tbody) return;

  if (data.length === 0) {
    tbody.innerHTML = `<tr><td colspan="8" class="text-center text-muted py-4">No backups found.</td></tr>`;
    return;
  }

  const totalPages = Math.ceil(data.length / itemsPerPage);
  if(currentPage > totalPages) currentPage = Math.max(1, totalPages);
  if(currentPage < 1) currentPage = 1;

  const startIdx = (currentPage - 1) * itemsPerPage;
  const pageData = data.slice(startIdx, startIdx + itemsPerPage);

  tbody.innerHTML = pageData.map(b => `
    <tr>
      <td><strong>${b.name}</strong></td>
      <td>${b.type}</td>
      <td>${b.included}</td>
      <td>${b.schedule}</td>
      <td style="white-space:nowrap;">${b.dateTime}</td>
      <td>${b.size}</td>
      <td>${statusBadge(b.status)}</td>
      <td>
        <a class="action-link" onclick="handleRestore('${b.name}')">View Restore</a>
      </td>
    </tr>
  `).join('');
  
  updateMetrics(data);
  renderPagination(totalPages);
}

function renderPagination(totalPages) {
  const container = document.getElementById('pagination-container');
  if(!container) return;
  
  if (totalPages <= 1) {
    container.innerHTML = '';
    return;
  }
  
  let html = `<button onclick="goToPage(${currentPage - 1})" ${currentPage === 1 ? 'disabled style="opacity:0.5;cursor:not-allowed;"' : ''}>← Previous</button>`;
  
  for(let i=1; i<=totalPages; i++) {
    if (i === currentPage) {
      html += `<button class="pg-active">${i}</button>`;
    } else {
      html += `<button onclick="goToPage(${i})">${i}</button>`;
    }
  }
  
  html += `<button onclick="goToPage(${currentPage + 1})" ${currentPage === totalPages ? 'disabled style="opacity:0.5;cursor:not-allowed;"' : ''}>Next →</button>`;
  container.innerHTML = html;
}

window.goToPage = function(page) {
  currentPage = page;
  renderBackups(backupsData); // Assuming backupsData is the master list. We don't have filters in backup yet.
}

function updateMetrics(data) {
  const total = data.length;
  if(total === 0) return;
  
  const success = data.filter(b => b.status === 'Success').length;
  const failed = data.filter(b => b.status === 'Failed').length;
  
  const successRate = total > 0 ? ((success / total) * 100).toFixed(1) : 0;
  
  // Calculate total size
  let totalGB = 0;
  data.forEach(b => {
    let size = parseFloat(b.size);
    if(b.size.includes('TB')) size *= 1024;
    else if(b.size.includes('MB')) size /= 1024;
    totalGB += size || 0;
  });
  
  const sizeFormatted = totalGB > 1000 ? (totalGB / 1024).toFixed(2) + ' TB' : totalGB.toFixed(0) + ' GB';
  
  // Last backup
  const lastBackup = data.length > 0 ? data[0].dateTime : '-';
  
  document.getElementById('metric-total').textContent = total;
  document.getElementById('metric-success').textContent = success;
  document.getElementById('metric-success-rate').textContent = `${successRate}% Success Rate`;
  document.getElementById('metric-failed').textContent = failed;
  document.getElementById('metric-size').textContent = sizeFormatted;
  document.getElementById('metric-last').textContent = lastBackup;
  document.getElementById('metric-health').textContent = `${successRate}%`;
  
  // Update Total Storage Used Card (Based on D: drive real capacity)
  const storageCapacityGB = 163; // User's D: drive is exactly ~163 GB total
  const availableGB = Math.max(0, storageCapacityGB - totalGB);
  const fillPct = Math.min(100, (totalGB / storageCapacityGB) * 100);
  
  const storageUsedEl = document.getElementById('storage-used');
  const storageFreeEl = document.getElementById('storage-free');
  const storageFillEl = document.getElementById('storage-fill');
  
  if (storageUsedEl && storageFreeEl && storageFillEl) {
    storageUsedEl.textContent = `Backup Size: ${sizeFormatted}`;
    storageFreeEl.textContent = `Available Space: ${availableGB.toFixed(0)} GB`;
    storageFillEl.style.width = `${fillPct}%`;
  }
}

function switchTab(btn, tabName) {
  if (btn) {
    document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
    btn.classList.add('active');
  } else {
    // If triggered from a quick action link
    document.querySelectorAll('.tab-btn').forEach(b => {
      if (b.textContent.includes(tabName)) {
        document.querySelectorAll('.tab-btn').forEach(x => x.classList.remove('active'));
        b.classList.add('active');
      }
    });
  }

  // Toggle views
  document.querySelectorAll('.tab-view').forEach(view => {
    view.style.display = 'none';
  });
  
  const targetView = document.getElementById(`view-${tabName}`);
  if (targetView) {
    targetView.style.display = 'block';
  }
}

function openBackupModal() {
  const backdrop = document.getElementById('backup-modal-backdrop');
  if (backdrop) {
    document.getElementById('bkp-name').value = '';
    document.getElementById('bkp-type').value = 'Full';
    document.getElementById('bkp-included').value = '';
    backdrop.classList.add('open');
  }
}

function closeBackupModal() {
  const backdrop = document.getElementById('backup-modal-backdrop');
  if (backdrop) backdrop.classList.remove('open');
}

function openReportsModal() {
  const backdrop = document.getElementById('reports-modal-backdrop');
  if (backdrop) {
    const total = backupsData.length;
    let successCount = 0;
    let failedCount = 0;
    let totalGB = 0;
    let issuesHtml = '';
    
    backupsData.forEach(b => {
      if (b.status === 'Success') successCount++;
      if (b.status === 'Failed') {
        failedCount++;
        issuesHtml += `<li style="margin-bottom:6px;"><strong>${b.dateTime}:</strong> ${b.name} failed (${b.included}).</li>`;
      }
      if (b.size && b.size.includes('GB')) {
        totalGB += parseInt(b.size) || 0;
      }
    });
    
    const rate = total === 0 ? 0 : ((successCount / total) * 100).toFixed(1);
    
    if (issuesHtml === '') {
      issuesHtml = '<li>No recent backup failures detected.</li>';
    }
    
    document.getElementById('rep-total').textContent = total;
    document.getElementById('rep-rate').textContent = rate + '%';
    document.getElementById('rep-data').textContent = (totalGB / 1000).toFixed(2) + ' TB';
    document.getElementById('rep-failed').textContent = failedCount;
    document.getElementById('rep-issues').innerHTML = issuesHtml;
    
    backdrop.classList.add('open');
  }
}

function closeReportsModal() {
  const backdrop = document.getElementById('reports-modal-backdrop');
  if (backdrop) backdrop.classList.remove('open');
}

function downloadReport() {
  const { jsPDF } = window.jspdf;
  const doc = new jsPDF();
  
  doc.setFontSize(18);
  doc.text('Comprehensive Backup Report', 14, 22);
  
  const tableData = backupsData.map(b => [b.name, b.type, b.included, b.schedule, b.dateTime, b.size, b.status]);
  
  doc.autoTable({
    startY: 30,
    head: [['Backup Name', 'Type', 'Data Included', 'Schedule', 'Date & Time', 'Size', 'Status']],
    body: tableData,
    theme: 'grid',
    headStyles: { fillColor: [37, 99, 235] }
  });
  
  doc.save('Backup_Report.pdf');
  closeReportsModal();
  showToast('Report generated and downloaded as PDF!', '#15803D');
}

function saveManualBackup() {
  const name = document.getElementById('bkp-name').value.trim();
  const type = document.getElementById('bkp-type').value;
  const included = document.getElementById('bkp-included').value.trim();

  if (!name || !included) {
    showToast('Please provide a Backup Name and Data Included', '#EF4444');
    return;
  }

  closeBackupModal();
  openRestoreProgressModal('Backup', name, (Math.random() * 5 + 2).toFixed(1));
}

function saveRetentionPolicies() {
  const daily = document.getElementById('ret-daily')?.value;
  const weekly = document.getElementById('ret-weekly')?.value;
  const monthly = document.getElementById('ret-monthly')?.value;
  
  if (daily) retentionData[0].period = daily.replace('Keep for ', '');
  if (weekly) retentionData[1].period = weekly.replace('Keep for ', '');
  if (monthly) retentionData[2].period = monthly.replace('Keep for ', '');
  
  localStorage.setItem('retentionData', JSON.stringify(retentionData));
  renderOverviewCards();
  showToast('Retention policies updated successfully!', '#22C55E');
}

function handleRestore(backupName) {
  const backup = backupsData.find(b => b.name === backupName);
  if (backup && backup.status !== 'Success') {
    showToast(`⚠️ Cannot restore '${backupName}' because it is still ${backup.status}.`, '#EF4444');
    return;
  }

  let sizeGB = null;
  if (backup && backup.size && backup.size.includes('GB')) {
    sizeGB = parseFloat(backup.size);
  }
  openRestoreProgressModal('Restore', backupName, sizeGB);
}

function showToast(msg, bg) {
  const el = document.getElementById('toast');
  if (!el) return;
  el.textContent = msg;
  el.style.background = bg || '#15803D';
  el.classList.add('show');
  setTimeout(() => el.classList.remove('show'), 3000);
}

function renderOverviewCards() {
  // Render Health
  const healthEl = document.getElementById('health-content');
  if (healthEl) {
    healthEl.innerHTML = healthData.map(h => `<div><strong>${h.period}</strong> - ${h.score}</div>`).join('');
  }

  // Render Restore Activity Card
  const restoreEl = document.getElementById('restore-activity-content');
  if (restoreEl) {
    restoreEl.innerHTML = restoreActivityData.slice(0,4).map(r => `<div><strong>${r.name}</strong> - <span class="${r.status === 'Success' ? 'badge-success' : 'badge-failed'}">${r.status}</span> (${r.date})</div>`).join('');
  }
  
  // Render Restore Table (in Restore Tab)
  const restoreTbody = document.getElementById('restore-tbody');
  if (restoreTbody) {
    restoreTbody.innerHTML = restoreActivityData.map((r, i) => `
      <tr>
        <td><strong>RST-88${90 - i}</strong></td>
        <td>${r.name}</td>
        <td>Primary Environment</td>
        <td>Admin</td>
        <td>${r.date}</td>
        <td><span class="${r.status === 'Success' ? 'badge-success' : 'badge-failed'}">${r.status}</span></td>
      </tr>
    `).join('');
  }

  // Render Retention
  const retentionTbody = document.getElementById('retention-tbody');
  if (retentionTbody) {
    retentionTbody.innerHTML = retentionData.map(r => `
      <tr>
        <td><strong>${r.type}</strong></td>
        <td>${r.period}</td>
        <td>${r.count}</td>
        <td>${r.size}</td>
        <td>${r.next}</td>
      </tr>`).join('');
  }
}

function renderActiveSchedules() {
  const tbody = document.getElementById('schedules-tbody');
  if (!tbody) return;
  tbody.innerHTML = activeSchedulesData.map((s, idx) => `
    <tr>
      <td><strong>${s.name}</strong></td>
      <td>${s.frequency}</td>
      <td>${s.source}</td>
      <td>${s.nextRun}</td>
      <td><span style="color:${s.status === 'Active' ? '#22C55E' : '#EF4444'};font-weight:700;">${s.status}</span></td>
      <td>
        <a class="action-link" style="cursor:pointer;" onclick="openScheduleModal(${idx})">Edit</a> | 
        <a class="action-link" style="color:${s.status === 'Active' ? '#EF4444' : '#22C55E'};cursor:pointer;" onclick="toggleSchedulePause(${idx})">${s.status === 'Active' ? 'Pause' : 'Play'}</a>
      </td>
    </tr>
  `).join('');
}

let editingScheduleIdx = null;

function openScheduleModal(idx = null) {
  const backdrop = document.getElementById('schedule-modal-backdrop');
  if (!backdrop) return;
  
  editingScheduleIdx = idx;
  
  const title = document.getElementById('sch-modal-title');
  const nameInput = document.getElementById('sch-name');
  const freqInput = document.getElementById('sch-freq');
  const srcInput = document.getElementById('sch-source');
  const statusBox = document.getElementById('sch-status-box');
  const statusBadge = document.getElementById('sch-status-badge');
  const btnSave = document.getElementById('sch-btn-save');

  if (idx !== null && activeSchedulesData[idx]) {
    const s = activeSchedulesData[idx];
    title.textContent = 'Edit Schedule';
    nameInput.value = s.name;
    freqInput.value = s.frequency;
    srcInput.value = s.source;
    
    statusBox.style.display = 'block';
    statusBadge.textContent = s.status;
    statusBadge.style.color = s.status === 'Active' ? '#22C55E' : '#EF4444';
    
    btnSave.textContent = 'Save';
  } else {
    title.textContent = 'Create Schedule';
    nameInput.value = '';
    freqInput.value = 'Every 24h';
    srcInput.value = '';
    
    statusBox.style.display = 'none';
    btnSave.textContent = 'Create';
  }
  
  backdrop.classList.add('open');
}

function closeScheduleModal() {
  const backdrop = document.getElementById('schedule-modal-backdrop');
  if (backdrop) backdrop.classList.remove('open');
}

function saveSchedule() {
  const name = document.getElementById('sch-name').value.trim();
  const freq = document.getElementById('sch-freq').value;
  const source = document.getElementById('sch-source').value.trim();
  
  if(!name || !source) {
    showToast('Please fill out all fields', '#EF4444');
    return;
  }
  
  if (editingScheduleIdx !== null && activeSchedulesData[editingScheduleIdx]) {
    activeSchedulesData[editingScheduleIdx].name = name;
    activeSchedulesData[editingScheduleIdx].frequency = freq;
    activeSchedulesData[editingScheduleIdx].source = source;
    showToast(`Schedule '${name}' updated!`, '#15803D');
  } else {
    // Determine next run text dynamically based on freq
    let nextRun = 'Tomorrow, 00:00';
    if(freq.includes('1h')) nextRun = 'Today, 14:00';
    else if(freq.includes('6h')) nextRun = 'Today, 18:00';
    
    activeSchedulesData.push({
      name: name,
      frequency: freq,
      source: source,
      nextRun: nextRun,
      status: 'Active'
    });
    showToast(`Schedule '${name}' created!`, '#15803D');
  }
  
  localStorage.setItem('activeSchedulesData', JSON.stringify(activeSchedulesData));
  closeScheduleModal();
  renderActiveSchedules();
}

function toggleSchedulePause(idx) {
  const s = activeSchedulesData[idx];
  if (!s) return;
  
  if (s.status === 'Active') {
    s.status = 'Paused';
    showToast(`⏸️ Schedule '${s.name}' paused.`, '#EF4444');
  } else {
    s.status = 'Active';
    showToast(`▶️ Schedule '${s.name}' resumed.`, '#22C55E');
  }
  
  localStorage.setItem('activeSchedulesData', JSON.stringify(activeSchedulesData));
  renderActiveSchedules();
}

document.addEventListener('DOMContentLoaded', () => {
  renderBackups(backupsData);
  renderOverviewCards();
  renderActiveSchedules();
  
  // Background poller to complete backups after 2 minutes
  setInterval(() => {
    let changed = false;
    const now = Date.now();
    backupsData.forEach(b => {
      if (b.status === 'In Progress') {
        if (!b.timestamp || (now - b.timestamp >= 120000)) { // 2 minutes or missing timestamp
          b.status = 'Success';
          b.size = Math.floor(Math.random() * 200 + 50) + ' GB';
          showToast(`🎉 Backup '${b.name}' has completed successfully!`, '#15803D');
          changed = true;
        }
      }
    });
    
    if (changed) {
      localStorage.setItem('backupsData', JSON.stringify(backupsData));
      renderBackups(backupsData); // This will re-render everything correctly
    }
  }, 5000);
});

// Dropdown click outside listener
document.addEventListener('click', (e) => {
  const filterBtn = document.querySelector('.filter-btn');
  const filterDropdown = document.getElementById('filter-dropdown');
  if (filterBtn && filterDropdown) {
    if (!filterBtn.contains(e.target) && !filterDropdown.contains(e.target)) {
      filterDropdown.style.display = 'none';
    }
  }
});

function toggleFilterMenu() {
  const dropdown = document.getElementById('filter-dropdown');
  if (dropdown) {
    dropdown.style.display = dropdown.style.display === 'none' ? 'block' : 'none';
  }
}

function applyFilter(status) {
  toggleFilterMenu();
  
  let filteredData = backupsData;
  if (status !== 'All') {
    filteredData = backupsData.filter(b => b.status === status);
  }
  
  renderBackups(filteredData);
  
  // Update the button text to show active filter
  const btn = document.querySelector('.filter-btn');
  if (btn) {
    btn.innerHTML = `<i class="fa-solid fa-filter me-1"></i> ${status === 'All' ? 'Filter Logs' : status + ' Logs'}`;
  }
}

/* ── PROGRESS MEGA MODAL LOGIC ── */
let progressInterval = null;
let currentProgress = 0;
let totalDataGB = 0;
let currentDataGB = 0;
let progressActionType = 'Restore';
let progressBackupName = '';

function openRestoreProgressModal(actionType, backupName, sizeGB) {
  progressActionType = actionType;
  progressBackupName = backupName;
  totalDataGB = sizeGB || (Math.random() * 5 + 2).toFixed(1);
  currentDataGB = 0;
  currentProgress = 0;
  
  document.getElementById('pm-title').textContent = actionType === 'Backup' ? 'Backing up Data' : 'Restoring Backup';
  document.getElementById('pm-subtitle').textContent = backupName;
  document.getElementById('pm-icon-i').className = actionType === 'Backup' ? 'fa-solid fa-cloud-arrow-up' : 'fa-solid fa-cloud-arrow-down';
  
  document.getElementById('pm-pct-text').textContent = '0%';
  document.getElementById('pm-circle-progress').style.strokeDashoffset = '377';
  document.getElementById('pm-bar-fill').style.width = '0%';
  document.getElementById('pm-stat-data').textContent = `0.0 GB / ${totalDataGB} GB`;
  document.getElementById('pm-stat-speed').textContent = '0 MB/s';
  document.getElementById('pm-stat-elapsed').textContent = '00:00:00';
  document.getElementById('pm-stat-eta').textContent = 'Calculating...';
  
  document.getElementById('pm-circle-wrap').classList.remove('success');
  
  for(let i=1; i<=5; i++) {
    const st = document.getElementById('pm-stage-'+i);
    st.classList.remove('active', 'completed');
  }
  if (actionType === 'Backup') {
    document.getElementById('pm-stage-1-text').textContent = 'Connecting to cloud storage';
    document.getElementById('pm-stage-2-text').textContent = 'Preparing files for backup';
    document.getElementById('pm-stage-3-text').textContent = 'Transferring data to cloud';
    document.getElementById('pm-stage-4-text').textContent = 'Verifying backup integrity';
    document.getElementById('pm-stage-5-text').textContent = 'Finalizing';
  } else {
    document.getElementById('pm-stage-1-text').textContent = 'Connecting to cloud storage';
    document.getElementById('pm-stage-2-text').textContent = 'Verifying backup integrity';
    document.getElementById('pm-stage-3-text').textContent = 'Downloading data';
    document.getElementById('pm-stage-4-text').textContent = 'Restoring database & files';
    document.getElementById('pm-stage-5-text').textContent = 'Finalizing & cleanup';
  }
  
  document.getElementById('progress-modal-backdrop').classList.add('show');
  
  let startTime = Date.now();
  
  progressInterval = setInterval(() => {
    let inc = Math.random() * 1.5 + 0.3; // slower organic feel
    currentProgress += inc;
    if (currentProgress >= 100) currentProgress = 100;
    
    let offset = 377 - (currentProgress / 100) * 377;
    document.getElementById('pm-circle-progress').style.strokeDashoffset = offset;
    document.getElementById('pm-pct-text').textContent = Math.floor(currentProgress) + '%';
    document.getElementById('pm-bar-fill').style.width = currentProgress + '%';
    
    currentDataGB = (currentProgress / 100) * totalDataGB;
    document.getElementById('pm-stat-data').textContent = `${currentDataGB.toFixed(1)} GB / ${totalDataGB} GB`;
    
    let speed = Math.floor(Math.random() * 20 + 30);
    document.getElementById('pm-stat-speed').textContent = speed + ' MB/s';
    
    let elapsedMs = Date.now() - startTime;
    let elapsedSec = Math.floor(elapsedMs / 1000);
    let eh = Math.floor(elapsedSec / 3600);
    let em = Math.floor((elapsedSec % 3600) / 60);
    let es = elapsedSec % 60;
    document.getElementById('pm-stat-elapsed').textContent = 
      `${eh.toString().padStart(2,'0')}:${em.toString().padStart(2,'0')}:${es.toString().padStart(2,'0')}`;
      
    let remainingGB = totalDataGB - currentDataGB;
    if (remainingGB > 0) {
      let remainingMB = remainingGB * 1024;
      let remainingSec = Math.floor(remainingMB / speed);
      let rm = Math.floor(remainingSec / 60);
      let rs = remainingSec % 60;
      document.getElementById('pm-stat-eta').textContent = `00:${rm.toString().padStart(2,'0')}:${rs.toString().padStart(2,'0')}`;
    } else {
      document.getElementById('pm-stat-eta').textContent = '00:00:00';
    }
    
    updateStages(currentProgress);
    
    if (currentProgress >= 100) {
      clearInterval(progressInterval);
      document.getElementById('pm-circle-wrap').classList.add('success');
      setTimeout(() => {
        closeRestoreProgressModal();
        if (progressActionType === 'Restore') {
          showToast(`✅ Backup Restored Successfully: ${progressBackupName}`, '#15803D');
          appendRestoreHistory(progressBackupName);
          switchTab(null, 'Restore');
        } else {
          showToast(`✅ Backup Created Successfully: ${progressBackupName}`, '#15803D');
          appendBackupHistory(progressBackupName);
          switchTab(null, 'Overview');
        }
      }, 1000);
    }
  }, 100); // 100ms for smooth animation
}

function updateStages(pct) {
  let activeStage = 1;
  if (pct > 20) activeStage = 2;
  if (pct > 40) activeStage = 3;
  if (pct > 70) activeStage = 4;
  if (pct > 95) activeStage = 5;
  
  for(let i=1; i<=5; i++) {
    const st = document.getElementById('pm-stage-'+i);
    if (i < activeStage) {
      st.classList.remove('active');
      st.classList.add('completed');
    } else if (i === activeStage) {
      st.classList.remove('completed');
      st.classList.add('active');
    } else {
      st.classList.remove('active', 'completed');
    }
  }
}

function closeRestoreProgressModal() {
  if (progressInterval) clearInterval(progressInterval);
  document.getElementById('progress-modal-backdrop').classList.remove('show');
}

function appendRestoreHistory(name) {
  const newRestore = {
    id: 'RST-' + Math.floor(1000 + Math.random() * 9000),
    name: name,
    target: 'Production Cluster',
    user: 'Admin User',
    date: new Date().toLocaleDateString('en-US', { month:'short', day:'2-digit', hour:'2-digit', minute:'2-digit' }),
    status: 'Success'
  };
  restoreActivityData.unshift(newRestore);
  localStorage.setItem('restoreActivityData', JSON.stringify(restoreActivityData));
  
  const restoreEl = document.getElementById('restore-activity-content');
  if (restoreEl) {
    restoreEl.innerHTML = restoreActivityData.slice(0,4).map(r => `<div><strong>${r.name}</strong> - <span class="${r.status === 'Success' ? 'badge-success' : 'badge-failed'}">${r.status}</span> (${r.date})</div>`).join('');
  }
  const restoreTbody = document.getElementById('restore-tbody');
  if (restoreTbody) {
    restoreTbody.innerHTML = restoreActivityData.map((r, i) => `
      <tr>
        <td>${r.id || 'RST-000'}</td>
        <td><strong>${r.name}</strong></td>
        <td>${r.target || 'Production Cluster'}</td>
        <td>${r.user || 'Admin User'}</td>
        <td>${r.date}</td>
        <td><span class="status-badge ${r.status === 'Success' ? 'badge-success' : 'badge-failed'}">${r.status}</span></td>
      </tr>
    `).join('');
  }
}

function appendBackupHistory(name) {
  const btype = document.getElementById('bkp-type') ? document.getElementById('bkp-type').value : 'Manual';
  const binc = document.getElementById('bkp-included') ? document.getElementById('bkp-included').value : 'All';
  const newBackup = {
    name: name,
    type: btype + ' System',
    included: binc,
    schedule: 'Manual',
    date: new Date().toLocaleDateString('en-US', { month:'short', day:'2-digit', hour:'2-digit', minute:'2-digit' }),
    size: (Math.random() * 5 + 2).toFixed(1) + ' GB',
    status: 'Success'
  };
  backupData.unshift(newBackup);
  renderTable();
}
