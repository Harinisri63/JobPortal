// ── MOCK DATA ──
  const storedComplaints = localStorage.getItem('admin_complaints');
  let complaints = storedComplaints ? JSON.parse(storedComplaints) : [
    { id: 'CMP-1021', name: 'Jane Cooper', topic: 'Offer letter link not showing in candidate profile dashboard', severity: 'critical', date: 'June 28, 2026', status: 'pending', desc: 'I received an email confirmation for the job offer but when I log into my candidate dashboard, the offer letter button says "Unavailable". Please assist immediately.', response: '' },
    { id: 'CMP-1015', name: 'Albert Flores', topic: 'Resume upload failing continually with 403 Forbidden error', severity: 'high', date: 'June 29, 2026', status: 'pending', desc: 'Whenever I try to drop my resume PDF, the system loading wheel spins for 10 seconds and returns a server forbidden payload. Tried with 3 different PDFs under 2MB.', response: '' },
    { id: 'CMP-1008', name: 'Theresa Webb', topic: 'Technical round meeting link expired before interview time', severity: 'high', date: 'June 29, 2026', status: 'progress', desc: 'The scheduled meeting link was supposed to be live at 10:00 AM, but upon clicking, the meet portal returned an "Expired Event" notification.', response: 'Interviewer notified. Fresh meeting coordinates generated and sent.' },
    { id: 'CMP-0994', name: 'Jerome Bell', topic: 'Recruiter did not show up for scheduled intro phone screening', severity: 'medium', date: 'June 25, 2026', status: 'resolved', desc: 'I waited for 30 minutes on the calls list but the recruiter did not connect. No response received via email.', response: 'Rescheduled candidate screening with Senior HR Partner.' },
    { id: 'CMP-0987', name: 'Wade Warren', topic: 'Incorrect job description listed under Product Designer position', severity: 'low', date: 'June 24, 2026', status: 'resolved', desc: 'The JD description references developer requirements instead of UIUX wireframing competencies.', response: 'Modified job description assets via CMS interface.' },
    { id: 'CMP-1025', name: 'Robert Fox', topic: 'Incorrect OTP authentication codes sent on login portal', severity: 'critical', date: 'June 30, 2026', status: 'pending', desc: 'I tried signing in multiple times but every time the SMS verification code takes 20 minutes to deliver, causing it to expire.', response: '' },
    { id: 'CMP-1029', name: 'Esther Howard', topic: 'Password reset links returning expired token code errors', severity: 'medium', date: 'June 30, 2026', status: 'pending', desc: 'I clicked the reset password option but the URL links sent are immediately expired upon opening.', response: '' },
    { id: 'CMP-1011', name: 'Cody Fisher', topic: 'Incorrect profile picture rendering in candidate workspace', severity: 'low', date: 'June 29, 2026', status: 'progress', desc: 'Uploaded PNG shows up compressed or stretched.', response: 'Adjusted CSS object-fit rules on image display components.' },
    { id: 'CMP-1002', name: 'Bessie Cooper', topic: 'Missing technical documentation link for developer assessments', severity: 'medium', date: 'June 27, 2026', status: 'progress', desc: 'The invitation test references guidelines that are not present in the instructions page.', response: 'Shared reference documents via admin email portal.' },
    { id: 'CMP-0975', name: 'Guy Hawkins', topic: 'Incorrect test scores compiled in grading summary email', severity: 'high', date: 'June 23, 2026', status: 'resolved', desc: 'Grading algorithm evaluated optional tasks as failed attempts.', response: 'Overrode dashboard scores manually to match grading standards.' },
  ];

  let activeFilter = 'all';

  let currentPage = 1;
  const itemsPerPage = 8;

  // ── INIT ──
  document.addEventListener('DOMContentLoaded', () => {
    renderTable();
    updateMetrics();

    // Live search input
    document.getElementById('searchInput').addEventListener('input', (e) => {
      currentPage = 1;
      renderTable(e.target.value.trim());
    });
  });

  // ── RENDER ──
  function renderTable(searchTerm = '') {
    const tbody = document.getElementById('complaintsTableBody');
    tbody.innerHTML = '';

    const filtered = complaints.filter(item => {
      // Filter status tab
      if (activeFilter !== 'all' && item.status !== activeFilter) return false;
      
      // Filter search
      if (searchTerm) {
        const term = searchTerm.toLowerCase();
        return item.id.toLowerCase().includes(term) ||
               item.name.toLowerCase().includes(term) ||
               item.topic.toLowerCase().includes(term) ||
               item.desc.toLowerCase().includes(term);
      }
      return true;
    });

    if (filtered.length === 0) {
      tbody.innerHTML = `<tr><td colspan="7" class="text-center text-muted py-4" style="font-size:0.75rem;">No complaints match the criteria.</td></tr>`;
      document.getElementById('pagination-container').innerHTML = '';
      return;
    }

    const totalPages = Math.ceil(filtered.length / itemsPerPage);
    if(currentPage > totalPages) currentPage = totalPages;
    if(currentPage < 1) currentPage = 1;

    const startIdx = (currentPage - 1) * itemsPerPage;
    const pageData = filtered.slice(startIdx, startIdx + itemsPerPage);

    pageData.forEach((item) => {
      const idx = complaints.indexOf(item);
      const isResolved = item.status === 'resolved';
      
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td style="font-weight: 700;">${item.id}</td>
        <td style="font-weight: 600;">${item.name}</td>
        <td>${item.topic}</td>
        <td><span class="priority-badge ${item.severity}">${item.severity}</span></td>
        <td>${item.date}</td>
        <td><span class="status-badge ${item.status}">${item.status === 'progress' ? 'In Progress' : item.status}</span></td>
        <td>
          <button class="action-btn ${isResolved ? 'view-only' : ''}" onclick="openResolveModal(${idx})">
            ${isResolved ? 'View Details' : 'Resolve'}
          </button>
        </td>
      `;
      tbody.appendChild(tr);
    });
    
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
    renderTable(document.getElementById('searchInput').value.trim());
  }

  function updateMetrics() {
    const total = complaints.length;
    const pending = complaints.filter(c => c.status === 'pending').length;
    const progress = complaints.filter(c => c.status === 'progress').length;
    const resolved = complaints.filter(c => c.status === 'resolved').length;

    document.getElementById('countTotal').textContent = total;
    document.getElementById('countPending').textContent = pending;
    document.getElementById('countProgress').textContent = progress;
    document.getElementById('countResolved').textContent = resolved;
  }

  // ── FILTER TABS ──
  window.filterComplaints = function(filterVal) {
    currentPage = 1;
    activeFilter = filterVal;
    
    // Update active class
    const buttons = document.querySelectorAll('.filter-btn');
    buttons.forEach(btn => {
      btn.classList.remove('active');
      if (btn.getAttribute('onclick').includes(filterVal)) {
        btn.classList.add('active');
      }
    });

    renderTable(document.getElementById('searchInput').value.trim());
  };

  // ── RESOLUTION WORKFLOW ──
  let resolveModalInstance;
  window.openResolveModal = function(index) {
    const item = complaints[index];
    document.getElementById('modalIndex').value = index;
    document.getElementById('modalTitle').textContent = `Manage Complaint ${item.id}`;
    document.getElementById('modalCandidate').textContent = item.name;
    document.getElementById('modalTopic').textContent = item.topic;
    document.getElementById('modalDescription').textContent = item.desc;
    document.getElementById('modalStatusSelect').value = item.status;
    document.getElementById('modalResponseText').value = item.response;

    resolveModalInstance = new bootstrap.Modal(document.getElementById('resolveModal'));
    resolveModalInstance.show();
  };

  window.submitResolution = function() {
    const index = parseInt(document.getElementById('modalIndex').value, 10);
    const newStatus = document.getElementById('modalStatusSelect').value;
    const responseText = document.getElementById('modalResponseText').value;

    complaints[index].status = newStatus;
    complaints[index].response = responseText;

    localStorage.setItem('admin_complaints', JSON.stringify(complaints));

    // Refresh layout
    renderTable(document.getElementById('searchInput').value.trim());
    updateMetrics();

    resolveModalInstance.hide();
    showToast(`Complaint ${complaints[index].id} has been updated successfully!`, 'success');
  };

  // ── TOAST ──
  function showToast(message, type = 'success') {
    let container = document.querySelector('.toast-container');
    if (!container) {
      container = document.createElement('div');
      container.className = 'toast-container';
      document.body.appendChild(container);
    }

    const id = 'toast_' + Date.now();
    const bgClass = type === 'success' ? 'bg-primary' : 'bg-secondary';
    
    const toastHTML = `
      <div id="${id}" class="toast align-items-center text-white ${bgClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
        <div class="d-flex">
          <div class="toast-body" style="font-size: 0.75rem; font-weight: 600;">
            ${message}
          </div>
          <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
      </div>
    `;
    
    container.insertAdjacentHTML('beforeend', toastHTML);
    const toastEl = document.getElementById(id);
    const toast = new bootstrap.Toast(toastEl, { delay: 3000 });
    toast.show();
    
    toastEl.addEventListener('hidden.bs.toast', () => {
      toastEl.remove();
    });
  }