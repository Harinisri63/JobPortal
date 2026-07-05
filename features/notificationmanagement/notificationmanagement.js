// ════════════════════════════════════════════════════════════════════
  //  DATA
  // ════════════════════════════════════════════════════════════════════
  const storedNotifications = localStorage.getItem('admin_notifications');
  let notifications = storedNotifications ? JSON.parse(storedNotifications) : [
    { id:'NTF-1021', title:'Schedule Change - Technical Interview',  type:'Job Alert',           target:'All Candidates',     channel:'Email',           delivery:'Delivered', date:'2025-05-18' },
    { id:'NTF-1020', title:'Application Update - Shortlisted',      type:'Application Update',  target:'Shortlisted Candidates', channel:'Email',       delivery:'Delivered', date:'2025-05-17' },
    { id:'NTF-1019', title:'Interview Remainder',                   type:'Interview',           target:'Interview Scheduled', channel:'Email',          delivery:'Delivered', date:'2025-05-16' },
    { id:'NTF-1018', title:'Offer Letter Released',                 type:'Offer',               target:'Selected Candidates', channel:'Email',          delivery:'Delivered', date:'2025-05-15' },
    { id:'NTF-1017', title:'New Job Alert - React Developer',       type:'Job Alert',           target:'All Candidates',     channel:'Email, In-App',   delivery:'Delivered', date:'2025-05-14' },
    { id:'NTF-1016', title:'System Maintenance Notice',             type:'System',              target:'All Candidates',     channel:'In-App',          delivery:'Delivered', date:'2025-05-13' },
    { id:'NTF-1015', title:'Reminder - Complete Your Profile',      type:'Reminder',            target:'All Candidates',     channel:'Email, SMS',      delivery:'Pending',   date:'2025-05-12' },
    { id:'NTF-1014', title:'Application Rejected - Data Analyst',   type:'Application Update',  target:'Specific User',      channel:'Email',          delivery:'Failed',    date:'2025-05-11' },
    { id:'NTF-1013', title:'Interview Schedule - PM Role',          type:'Interview',           target:'Interview Scheduled', channel:'Email, SMS',     delivery:'Delivered', date:'2025-05-10' },
    { id:'NTF-1012', title:'Job Posting Live - UI/UX Designer',     type:'Job Alert',           target:'All Candidates',     channel:'Email, SMS, In-App', delivery:'Delivered', date:'2025-05-09' },
  ];

  const storedTemplates = localStorage.getItem('admin_templates');
  let templates = storedTemplates ? JSON.parse(storedTemplates) : [
    { id: 'T-1', name: 'Interview Reminder',  channels: 'Email, In-App',       status: 'Active', type: 'Interview', target: 'Interview Scheduled', message: 'Dear Candidate,\nThis is a reminder that your technical interview is scheduled for [Date] at [Time]. Please be ready 5 minutes before the scheduled time.\nBest Regards,\nSynergech HR Team' },
    { id: 'T-2', name: 'Application Update',  channels: 'Email, SMS',            status: 'Active', type: 'Application Update', target: 'Shortlisted Candidates', message: 'Hello,\nWe are pleased to inform you that your application has been shortlisted for further rounds. We will contact you soon with the interview details.\nBest,\nSynergech Recruitment' },
    { id: 'T-3', name: 'Job Alert',           channels: 'Email, SMS, In-App',  status: 'Active', type: 'Job Alert', target: 'All Candidates', message: 'Hi!\nNew jobs matching your profile have been posted at Synergech. Check them out on the dashboard!\nRegards,\nProPath Alerts' },
    { id: 'T-4', name: 'Offer Letter',        channels: 'Email',                 status: 'Active', type: 'Offer', target: 'Selected Candidates', message: 'Congratulations!\nWe are delighted to extend you an offer to join Synergech. Please review the attached official offer letter details.\nWelcome Aboard,\nSynergech Team' },
  ];

  function saveData() {
    localStorage.setItem('admin_notifications', JSON.stringify(notifications));
    localStorage.setItem('admin_templates', JSON.stringify(templates));
  }

  let activeTabFilter = '';

  // ════════════════════════════════════════════════════════════════════
  //  RENDER TABLE
  // ════════════════════════════════════════════════════════════════════
  function deliveryClass(d) {
    return d === 'Delivered' ? 'delivery-delivered' : d === 'Pending' ? 'delivery-pending' : 'delivery-failed';
  }

  let currentPage = 1;
  const itemsPerPage = 8;

  function renderTable(data) {
    const tbody = document.getElementById('ntf-tbody');
    if (!data.length) {
      tbody.innerHTML = `<tr><td colspan="7" style="text-align:center;padding:40px;color:#CBD5E1;">No notifications found.</td></tr>`;
      document.getElementById('pagination-container').innerHTML = '';
      updateMetrics(data);
      return;
    }

    const totalPages = Math.ceil(data.length / itemsPerPage);
    if(currentPage > totalPages) currentPage = totalPages;
    if(currentPage < 1) currentPage = 1;

    const startIdx = (currentPage - 1) * itemsPerPage;
    const pageData = data.slice(startIdx, startIdx + itemsPerPage);

    tbody.innerHTML = pageData.map(n => `
      <tr>
        <td class="ntf-id">${n.id}</td>
        <td class="ntf-title-cell">${n.title}</td>
        <td style="font-size:0.7rem;">${n.type}</td>
        <td style="font-size:0.7rem;">${n.target}</td>
        <td style="font-size:0.7rem;">${n.channel}</td>
        <td><span class="delivery-badge ${deliveryClass(n.delivery)}">${n.delivery}</span></td>
        <td>
          <div class="action-icons">
            <button title="View" onclick="viewNotification('${n.id}')"><i class="fa-regular fa-eye"></i></button>
            <button title="Edit" onclick="editNotification('${n.id}')"><i class="fa-regular fa-pen-to-square"></i></button>
            <button title="Resend" onclick="resendNotification('${n.id}')" style="color: ${(n.timestamp && Date.now() - n.timestamp <= 5*60*1000) ? '#0A4BD2' : '#94A3B8'}"><i class="fa-solid fa-rotate-right"></i></button>
            <button title="More"><i class="fa-solid fa-ellipsis-vertical"></i></button>
          </div>
        </td>
      </tr>`).join('');
    
    updateMetrics(data);
    renderPagination(totalPages);
  }

  function updateMetrics(data) {
    const total = data.length;
    let delivered = 0, failed = 0, pending = 0;
    data.forEach(n => {
      if(n.delivery === 'Delivered') delivered++;
      else if(n.delivery === 'Failed') failed++;
      else if(n.delivery === 'Pending') pending++;
    });

    const elTotal = document.getElementById('metric-total');
    const elSent = document.getElementById('metric-sent');
    const elDelivered = document.getElementById('metric-delivered');
    const elFailed = document.getElementById('metric-failed');

    if(elTotal) elTotal.textContent = total;
    if(elSent) elSent.textContent = pending;
    if(elDelivered) elDelivered.textContent = delivered;
    if(elFailed) elFailed.textContent = failed;
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
    applyFilters(false);
  }

  // ════════════════════════════════════════════════════════════════════
  //  RENDER TEMPLATES
  // ════════════════════════════════════════════════════════════════════
  function renderTemplates() {
    const body = document.getElementById('tp-body');
    body.innerHTML = templates.map((t, idx) => `
      <div class="tp-card" id="t-card-${t.id}">
        <div class="tp-card-title">${t.name}</div>
        <div class="tp-card-meta">
          <span class="tp-card-channels">${t.channels}</span>
          <span class="tp-card-status ${t.status === 'Active' ? 'tp-active' : 'tp-inactive'}" id="t-status-badge-${t.id}">${t.status}</span>
        </div>
        <div class="tp-actions">
          <button class="btn-tp-edit" onclick="editTemplate('${t.id}')"><i class="fa-regular fa-pen-to-square"></i> Edit &amp; Send</button>
          <button class="btn-tp-close" onclick="toggleTemplateStatus('${t.id}')"><i class="fa-regular fa-circle-xmark"></i> ${t.status === 'Active' ? 'Close' : 'Activate'}</button>
        </div>
      </div>`).join('');
  }

  function editTemplate(id) {
    const t = templates.find(x => x.id === id);
    if (!t) return;

    openCreateModal();
    document.getElementById('modal-title-label').textContent = 'Edit Template & Send Offer';
    document.getElementById('inp-editing-index').value = id;
    document.getElementById('inp-title').value = t.name;
    document.getElementById('inp-type').value = t.type;
    document.getElementById('inp-target').value = t.target;
    document.getElementById('inp-message').value = t.message;
    document.getElementById('inp-channel').value = t.channels.replace('In-App', 'In - App').replace('In-App', 'In - App'); // matches dropdown format
  }

  function toggleTemplateStatus(id) {
    const t = templates.find(x => x.id === id);
    if (!t) return;
    t.status = t.status === 'Active' ? 'Inactive' : 'Active';
    saveData();
    renderTemplates();
    showToast(`Template ${t.name} set to ${t.status}`, t.status === 'Active' ? '#15803D' : '#EF4444');
  }

  // ════════════════════════════════════════════════════════════════════
  //  TABS + FILTERS
  // ════════════════════════════════════════════════════════════════════
  function switchTab(btn, filter) {
    document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
    btn.classList.add('active');
    activeTabFilter = filter;
    applyFilters();
  }

  function applyFilters(resetPage = true) {
    if(resetPage) currentPage = 1;
    const q       = document.getElementById('ntf-search').value.toLowerCase();
    const status  = document.getElementById('fil-status').value;
    const type    = document.getElementById('fil-type').value;
    const channel = document.getElementById('fil-channel').value;

    let data = notifications.filter(n =>
      (!q       || n.title.toLowerCase().includes(q) || n.type.toLowerCase().includes(q) || n.id.toLowerCase().includes(q)) &&
      (!status  || n.delivery === status) &&
      (!type    || n.type === type) &&
      (!channel || n.channel.includes(channel)) &&
      (!activeTabFilter || n.type.includes(activeTabFilter))
    );
    renderTable(data);
  }

  function resetFilters() {
    currentPage = 1;
    document.getElementById('ntf-search').value = '';
    document.getElementById('fil-status').value = '';
    document.getElementById('fil-type').value   = '';
    document.getElementById('fil-channel').value= '';
    document.getElementById('fil-date').value   = '';
    activeTabFilter = '';
    document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
    document.querySelector('.tab-btn').classList.add('active');
    renderTable(notifications);
  }

  // ════════════════════════════════════════════════════════════════════
  //  CREATE MODAL
  // ════════════════════════════════════════════════════════════════════
  function openCreateModal() {
    document.getElementById('create-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
    document.getElementById('modal-title-label').textContent = 'Create Notification Alert';
    document.getElementById('inp-editing-index').value = '';
    
    const badge = document.getElementById('modal-edit-badge');
    if(badge) badge.style.display = 'none';

    // Reset form
    ['inp-title','inp-message'].forEach(id => document.getElementById(id).value = '');
    ['inp-type','inp-target','inp-channel'].forEach(id => document.getElementById(id).selectedIndex = 0);
    document.getElementById('inp-date').value = '';
    document.getElementById('inp-time').value = '';
  }

  function closeCreateModal() {
    document.getElementById('create-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }

  function handleBd(e) {
    if (e.target === document.getElementById('create-backdrop')) closeCreateModal();
  }

  document.addEventListener('keydown', e => { if (e.key === 'Escape') closeCreateModal(); });

  function submitNotification(action) {
    const title   = document.getElementById('inp-title').value.trim();
    const type    = document.getElementById('inp-type').value;
    const target  = document.getElementById('inp-target').value;
    const message = document.getElementById('inp-message').value.trim();
    const channel = document.getElementById('inp-channel').value;
    const editingId = document.getElementById('inp-editing-index').value;

    if (!title || !type || !target || !message || !channel) {
      showToast('Please fill all required fields.', '#EF4444');
      return;
    }

    if (editingId) {
      // Save changes back to template
      const t = templates.find(x => x.id === editingId);
      if (t) {
        t.name = title;
        t.type = type;
        t.target = target;
        t.message = message;
        t.channels = channel;
        renderTemplates();
      }
    }

    // Generate new notification log entry
    const maxNum = Math.max(...notifications.map(n => parseInt(n.id.split('-')[1])));
    const newId  = 'NTF-' + (maxNum + 1);

    const delivery = action === 'send' ? 'Delivered' : action === 'schedule' ? 'Pending' : 'Pending';

    notifications.unshift({
      id: newId,
      title: title,
      type: type,
      target: target,
      channel: channel,
      delivery: delivery,
      date: new Date().toISOString().slice(0, 10),
      timestamp: Date.now(),
      message: message
    });

    saveData();
    applyFilters(false);
    closeCreateModal();

    const msgs = {
      send:     editingId ? 'Offer/Alert dispatched to candidate successfully!' : 'Notification sent successfully!',
      schedule: 'Notification scheduled successfully!',
      draft:    'Draft saved successfully!'
    };
    showToast(msgs[action] || 'Done!', '#15803D');
  }

  function showToast(msg, bg) {
    const el = document.getElementById('toast');
    el.textContent = msg;
    el.style.background = bg || '#15803D';
    el.classList.add('show');
    setTimeout(() => el.classList.remove('show'), 3000);
  }

  function viewNotification(id) {
    const n = notifications.find(x => x.id === id);
    if(!n) return;
    
    document.getElementById('view-ntf-title').textContent = n.title;
    document.getElementById('view-ntf-type').textContent = n.type;
    document.getElementById('view-ntf-target').textContent = n.target;
    document.getElementById('view-ntf-channel').textContent = n.channel;
    document.getElementById('view-ntf-status').textContent = n.delivery;
    document.getElementById('view-ntf-date').textContent = n.date;
    
    document.getElementById('btn-view-edit').onclick = () => {
      closeViewModal();
      editNotification(id);
    };

    document.getElementById('view-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  }

  function closeViewModal() {
    document.getElementById('view-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }

  function handleViewBd(e) {
    if (e.target === document.getElementById('view-backdrop')) closeViewModal();
  }

  function editNotification(id) {
    const n = notifications.find(x => x.id === id);
    if(!n) return;

    openCreateModal();
    document.getElementById('modal-title-label').textContent = 'Edit Notification Alert';
    document.getElementById('inp-editing-index').value = id;
    document.getElementById('inp-title').value = n.title;
    document.getElementById('inp-type').value = n.type;
    document.getElementById('inp-target').value = n.target;
    document.getElementById('inp-channel').value = n.channel;
    document.getElementById('inp-message').value = n.message || '';
    
    const badge = document.getElementById('modal-edit-badge');
    badge.style.display = 'inline-block';
    
    // Editable badge based on delivery status
    if (n.delivery === 'Delivered') {
      badge.textContent = 'Non-Editable';
      badge.style.background = '#FEE2E2';
      badge.style.color = '#991B1B';
    } else {
      badge.textContent = 'Editable';
      badge.style.background = '#DCFCE7';
      badge.style.color = '#166534';
    }
  }

  function resendNotification(id) {
    const n = notifications.find(x => x.id === id);
    if(!n) return;

    if (n.timestamp && Date.now() - n.timestamp <= 5*60*1000) {
      showToast(`Notification ${id} resent successfully!`, '#0A4BD2');
    } else {
      showToast(`Cannot resend ${id}. The 5-minute window has expired.`, '#EF4444');
    }
  }

  // ════════════════════════════════════════════════════════════════════
  //  INIT
  // ════════════════════════════════════════════════════════════════════
  renderTable(notifications);
  renderTemplates();