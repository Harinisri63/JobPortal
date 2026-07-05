// ── DATA MODEL ──
  let storedArticles = null;
try {
  storedArticles = localStorage.getItem('cms_articles');
} catch (e) {
  console.warn('localStorage not accessible due to iframe file:// security policies');
}
  let articles = storedArticles ? JSON.parse(storedArticles) : [
    { id: 1, title: 'How to prepare for a technical interview', type: 'Article', category: 'Career Tips', status: 'Published', date: '12 May 2025', views: '2,450', author: 'Admin', desc: 'A comprehensive guide on cracking coding tests and design rounds.', body: 'Start preparing early. Build project portfolios...' },
    { id: 2, title: 'Top 10 In-Demand skills in 2025', type: 'Article', category: 'Skills', status: 'Published', date: '10 May 2025', views: '1,990', author: 'Tech Lead', desc: 'An insight into what companies look for.', body: 'AI/ML engineering, advanced DevOps pipelines...' },
    { id: 3, title: 'Resume Building Guide', type: 'Guide', category: 'Job Search', status: 'Published', date: '09 May 2025', views: '3,210', author: 'HR Team', desc: 'Crafting the ultimate standard ATS friendly resume.', body: 'Always list quantifiable contributions...' },
    { id: 4, title: 'Job search Strategies for freshers', type: 'Article', category: 'Career Tips', status: 'Pending Review', date: '08 May 2025', views: '980', author: 'HR Team', desc: 'Leveraging portals, references, and platforms.', body: 'Optimize profiles, connect with key hiring managers...' },
    { id: 5, title: 'Salary Trends in IT Industry', type: 'Case Study', category: 'Insights', status: 'Draft', date: '06 May 2025', views: '-', author: 'Admin', desc: 'A compensation analysis across different tech hubs.', body: 'Remote roles show salary convergence...' },
    { id: 6, title: 'Interview Questions - Frontend Developer', type: 'Article', category: 'Interview Preparation', status: 'Scheduled', date: '15 May 2025', views: '-', author: 'Tech Lead', desc: 'Common React, JS, and HTML5 questions.', body: 'Cover closures, hooks, Virtual DOM...' },
  ];
  let selectedTabFilter = '';
  let currentPage = 1;
  const itemsPerPage = 5;
  // ── RENDER FUNCTION ──
  function getStatusClass(s) {
    if (s === 'Published') return 'status-published';
    if (s === 'Pending Review') return 'status-pending';
    if (s === 'Scheduled') return 'status-scheduled';
    return 'status-draft';
  }
  function renderTable(data) {
    const tbody = document.getElementById('cms-tbody');
    if (!data.length) {
      tbody.innerHTML = `<tr><td colspan="7" style="text-align:center;padding:40px;color:var(--text-muted);">No contents found matching filters.</td></tr>`;
      renderPagination(0);
      return;
    }

    const totalPages = Math.ceil(data.length / itemsPerPage);
    if (currentPage > totalPages && totalPages > 0) currentPage = totalPages;

    const startIdx = (currentPage - 1) * itemsPerPage;
    const pageData = data.slice(startIdx, startIdx + itemsPerPage);

    tbody.innerHTML = pageData.map(a => `
      <tr onclick="loadArticleToEditor(${a.id})">
        <td class="cms-title-cell">${a.title}</td>
        <td>${a.type}</td>
        <td>${a.category}</td>
        <td><span class="status-badge ${getStatusClass(a.status)}">${a.status}</span></td>
        <td style="white-space:nowrap;">${a.date}</td>
        <td>${a.views}</td>
        <td>
          <div class="action-icons" onclick="event.stopPropagation()">
            <button title="View" onclick="viewArticle(${a.id})"><i class="fa-regular fa-eye"></i></button>
            <button title="Edit" onclick="loadArticleToEditor(${a.id})"><i class="fa-regular fa-pen-to-square"></i></button>
            <button title="Delete" onclick="deleteArticle(${a.id})"><i class="fa-regular fa-trash-can"></i></button>
            <button title="More"><i class="fa-solid fa-ellipsis-vertical"></i></button>
          </div>
        </td>
      </tr>
    `).join('');
    
    renderPagination(totalPages);
  }

  function renderPagination(totalPages) {
    const container = document.getElementById('pagination-container');
    if (!container) return;
    
    if (totalPages <= 1) {
      container.innerHTML = '';
      return;
    }

    let html = '';
    
    // Prev
    html += `<button onclick="goToPage(${currentPage - 1})" ${currentPage === 1 ? 'disabled style="opacity:0.5;cursor:not-allowed;"' : ''}>&larr; Previous</button>`;
    
    // Pages
    for (let i = 1; i <= totalPages; i++) {
      if (
        i === 1 || 
        i === totalPages || 
        (i >= currentPage - 1 && i <= currentPage + 1)
      ) {
        if (i === currentPage) {
          html += `<button class="pg-active">${i}</button>`;
        } else {
          html += `<button onclick="goToPage(${i})">${i}</button>`;
        }
      } else if (i === currentPage - 2 || i === currentPage + 2) {
        html += `<span class="pg-dots">&hellip;</span>`;
      }
    }
    
    // Next
    html += `<button onclick="goToPage(${currentPage + 1})" ${currentPage === totalPages ? 'disabled style="opacity:0.5;cursor:not-allowed;"' : ''}>Next &rarr;</button>`;
    
    container.innerHTML = html;
  }

  window.goToPage = function(page) {
    currentPage = page;
    applyFilters(false);
  };

  function renderDashboard() {
    const total = articles.length || 1;
    let counts = { 'Published': 0, 'Pending Review': 0, 'Scheduled': 0, 'Draft': 0 };
    articles.forEach(a => { if (counts[a.status] !== undefined) counts[a.status]++; else counts['Draft']++; });
    
    const pPub = (counts['Published'] / total) * 100;
    const pPen = (counts['Pending Review'] / total) * 100;
    const pSch = (counts['Scheduled'] / total) * 100;
    
    const p1 = pPub; const p2 = p1 + pPen; const p3 = p2 + pSch;
    
    const statusChart = document.getElementById('status-chart-placeholder');
    if (statusChart) {
      statusChart.innerHTML = `
        <div style="display:flex; align-items:center; gap:20px; width:100%; justify-content:center;">
          <div style="width:110px;height:110px;border-radius:50%;background:conic-gradient(#0A4BD2 ${p1}%, #FEB900 ${p1}% ${p2}%, #6366F1 ${p2}% ${p3}%, #CBD5E1 ${p3}%); flex-shrink:0;"></div>
          <div style="display:flex; flex-direction:column; gap:6px;">
            <div style="display:flex; align-items:center; gap:6px;"><span style="width:10px;height:10px;background:#0A4BD2;border-radius:2px;"></span> Published (${counts['Published']})</div>
            <div style="display:flex; align-items:center; gap:6px;"><span style="width:10px;height:10px;background:#FEB900;border-radius:2px;"></span> Pending (${counts['Pending Review']})</div>
            <div style="display:flex; align-items:center; gap:6px;"><span style="width:10px;height:10px;background:#6366F1;border-radius:2px;"></span> Scheduled (${counts['Scheduled']})</div>
            <div style="display:flex; align-items:center; gap:6px;"><span style="width:10px;height:10px;background:#CBD5E1;border-radius:2px;"></span> Draft (${counts['Draft']})</div>
          </div>
        </div>
      `;
    }

    let tCounts = { 'Article': 0, 'Guide': 0, 'Case Study': 0 };
    articles.forEach(a => { if (tCounts[a.type] !== undefined) tCounts[a.type]++; else tCounts['Article']++; });
    
    const tArt = (tCounts['Article'] / total) * 100;
    const tGui = (tCounts['Guide'] / total) * 100;
    const t1 = tArt; const t2 = t1 + tGui;
    
    const typeChart = document.getElementById('type-chart-placeholder');
    if (typeChart) {
      typeChart.innerHTML = `
        <div style="display:flex; align-items:center; gap:20px; width:100%; justify-content:center;">
          <div style="width:110px;height:110px;border-radius:50%;background:conic-gradient(#10B981 ${t1}%, #065F46 ${t1}% ${t2}%, #0EA5E9 ${t2}%); flex-shrink:0;"></div>
          <div style="display:flex; flex-direction:column; gap:6px;">
            <div style="display:flex; align-items:center; gap:6px;"><span style="width:10px;height:10px;background:#10B981;border-radius:2px;"></span> Article (${tCounts['Article']})</div>
            <div style="display:flex; align-items:center; gap:6px;"><span style="width:10px;height:10px;background:#065F46;border-radius:2px;"></span> Guide (${tCounts['Guide']})</div>
            <div style="display:flex; align-items:center; gap:6px;"><span style="width:10px;height:10px;background:#0EA5E9;border-radius:2px;"></span> Case Study (${tCounts['Case Study']})</div>
          </div>
        </div>
      `;
    }

    const activityList = document.getElementById('recent-activity-list');
    if (activityList) {
      const sorted = [...articles].sort((a,b) => b.id - a.id).slice(0, 4);
      if(sorted.length === 0) {
        activityList.innerHTML = `<li class="activity-item"><span class="activity-txt">No recent activity.</span></li>`;
      } else {
        activityList.innerHTML = sorted.map(a => `
          <li class="activity-item">
            <span class="activity-txt">&bull; "${a.title}" (${a.type}) - ${a.status}</span>
            <span class="activity-time">${a.date}</span>
          </li>
        `).join('');
      }
    }
  }

  // ── LOAD / EDIT ──
  function loadArticleToEditor(id) {
    const a = articles.find(x => x.id === id);
    if (!a) return;

    document.getElementById('editor-action-title').textContent = 'Edit CMS Article or Post';
    
    // Badge Logic
    const badge = document.getElementById('editor-badge');
    badge.style.display = 'inline-block';
    if(a.status === 'Published' || a.status === 'Pending Review') {
      badge.innerText = 'Non-Editable';
      badge.style.background = '#FEE2E2';
      badge.style.color = '#EF4444';
    } else {
      badge.innerText = 'Editable';
      badge.style.background = '#D1FAE5';
      badge.style.color = '#10B981';
    }

    document.getElementById('edit-id').value = a.id;
    document.getElementById('cms-title').value = a.title;
    document.getElementById('cms-type').value = a.type;
    document.getElementById('cms-category').value = a.category;
    document.getElementById('cms-author').value = a.author;
    document.getElementById('cms-status').value = a.status;
    document.getElementById('cms-desc').value = a.desc;
    document.getElementById('cms-body-content').innerText = a.body;

    document.getElementById('editor-backdrop').classList.add('open');
    document.getElementById('editor-panel').classList.add('open');
    document.body.style.overflow = 'hidden';
  }

  function clearEditor() {
    document.getElementById('editor-action-title').textContent = 'Create CMS Article or Post';
    document.getElementById('editor-badge').style.display = 'none';
    
    document.getElementById('edit-id').value = '';
    document.getElementById('cms-title').value = '';
    document.getElementById('cms-type').selectedIndex = 0;
    document.getElementById('cms-category').selectedIndex = 0;
    document.getElementById('cms-author').selectedIndex = 0;
    document.getElementById('cms-status').selectedIndex = 0;
    document.getElementById('cms-desc').value = '';
    document.getElementById('cms-body-content').innerText = 'Write your content here...';

    document.getElementById('editor-backdrop').classList.remove('open');
    document.getElementById('editor-panel').classList.remove('open');
    document.body.style.overflow = '';
  }

  function openCreateEditor() {
    clearEditor();
    document.getElementById('editor-backdrop').classList.add('open');
    document.getElementById('editor-panel').classList.add('open');
    document.body.style.overflow = 'hidden';
  }

  // ── SAVE ──
  function saveContent(isDraft = false) {
    const editId = document.getElementById('edit-id').value;
    const title = document.getElementById('cms-title').value.trim();
    const type = document.getElementById('cms-type').value;
    const category = document.getElementById('cms-category').value;
    const author = document.getElementById('cms-author').value;
    const status = isDraft ? 'Draft' : document.getElementById('cms-status').value;
    const desc = document.getElementById('cms-desc').value.trim();
    const body = document.getElementById('cms-body-content').innerText.trim();

    if (!title || !type || !category || !author || !body) {
      showToast('Please fill out all required fields marked with *', '#EF4444');
      return;
    }

    if (editId) {
      // Update existing
      const a = articles.find(x => x.id == editId);
      if (a) {
        a.title = title;
        a.type = type;
        a.category = category;
        a.author = author;
        a.status = status;
        a.desc = desc;
        a.body = body;
        a.date = new Date().toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
        showToast('Article updated successfully!', '#15803D');
      }
    } else {
      // Create new
      articles.unshift({
        id: Date.now(),
        title, type, category, author, status,
        date: new Date().toLocaleDateString('en-GB', { day:'2-digit', month:'short', year:'numeric'}),
        views: '-',
        desc, body
      });
      showToast('New article created successfully!', '#15803D');
    }

    saveData();
    applyFilters();
    clearEditor();
  }

  // ── DELETE ──
  const storedDeleted = localStorage.getItem('cms_deletedArticles');
  let deletedArticles = storedDeleted ? JSON.parse(storedDeleted) : [];

  function saveData() {
    localStorage.setItem('cms_articles', JSON.stringify(articles));
    localStorage.setItem('cms_deletedArticles', JSON.stringify(deletedArticles));
    renderDashboard();
  }

  function deleteArticle(id) {
    if (confirm('Are you sure you want to move this article to the Recycle Bin?')) {
      const idx = articles.findIndex(x => x.id === id);
      if(idx !== -1) {
        deletedArticles.push(articles[idx]);
        articles.splice(idx, 1);
        saveData();
        applyFilters();
        showToast('Article moved to Recycle Bin!', '#EF4444');
      }
    }
  }

  // ── VIEW ──
  // ── VIEW ──
  function viewArticle(id) {
    const a = articles.find(x => x.id === id);
    if (!a) return;
    
    document.getElementById('view-title').innerText = a.title;
    document.getElementById('view-type').innerText = a.type;
    document.getElementById('view-category').innerText = a.category;
    document.getElementById('view-author').innerText = a.author;
    document.getElementById('view-desc').innerText = a.desc;
    document.getElementById('view-body').innerText = a.body;
    
    const statusEl = document.getElementById('view-status');
    statusEl.innerText = a.status;
    if (a.status === 'Published') {
      statusEl.style.background = '#D1FAE5'; statusEl.style.color = '#10B981';
    } else if (a.status === 'Draft' || a.status === 'Pending Review') {
      statusEl.style.background = '#FEF3C7'; statusEl.style.color = '#F59E0B';
    } else {
      statusEl.style.background = '#F1F5F9'; statusEl.style.color = '#475569';
    }

    document.getElementById('view-article-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  }

  function closeViewArticleModal() {
    document.getElementById('view-article-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }

  function handleViewArticleBackdropClick(e) {
    if (e.target === document.getElementById('view-article-backdrop')) closeViewArticleModal();
  }

  // ── FILTERS & TABS ──
  function switchTab(btn, filter) {
    document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
    btn.classList.add('active');
    
    if (filter === 'LandingPages') {
      const mainView = document.getElementById('cms-main-view');
      if (mainView) mainView.style.display = 'none';
      const lpView = document.getElementById('landing-pages-view');
      if (lpView) lpView.style.display = 'block';
      loadLandingPageContent();
    } else {
      const mainView = document.getElementById('cms-main-view');
      if (mainView) mainView.style.display = 'block';
      const lpView = document.getElementById('landing-pages-view');
      if (lpView) lpView.style.display = 'none';
      
      selectedTabFilter = filter;
      applyFilters();
    }
  }

  function applyFilters(resetPage = true) {
    if (resetPage) currentPage = 1;
    const q = document.getElementById('cms-search').value.toLowerCase();
    const type = document.getElementById('fil-type').value;
    const cat = document.getElementById('fil-category').value;
    const status = document.getElementById('fil-status').value;
    const author = document.getElementById('fil-author').value;

    const data = articles.filter(a =>
      (!q || a.title.toLowerCase().includes(q) || a.type.toLowerCase().includes(q)) &&
      (!type || a.type === type) &&
      (!cat || a.category === cat) &&
      (!status || a.status === status) &&
      (!author || a.author === author) &&
      (!selectedTabFilter || a.type === selectedTabFilter)
    );
    renderTable(data);
  }

  function resetFilters() {
    document.getElementById('cms-search').value = '';
    document.getElementById('fil-type').selectedIndex = 0;
    document.getElementById('fil-category').selectedIndex = 0;
    document.getElementById('fil-status').selectedIndex = 0;
    document.getElementById('fil-author').selectedIndex = 0;
    selectedTabFilter = '';
    document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
    document.querySelector('.tab-btn').classList.add('active');
    renderTable(articles);
  }

  function showToast(msg, bg) {
    const el = document.getElementById('toast');
    el.textContent = msg;
    el.style.background = bg;
    el.style.display = 'block';
    setTimeout(() => { el.style.display = 'none'; }, 3000);
  }

  // ── BULK UPLOAD ──
  function handleBulkUpload() {
    document.getElementById('bulk-upload-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
    updateTemplateLink();
  }

  function closeBulkUploadModal() {
    document.getElementById('bulk-upload-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }

  function handleBulkUploadBackdropClick(e) {
    if (e.target === document.getElementById('bulk-upload-backdrop')) closeBulkUploadModal();
  }

  function updateTemplateLink() {
    const isJob = document.getElementById('up-job').checked;
    const link = document.getElementById('template-link');
    if (isJob) {
      link.innerHTML = '<i class="fa-solid fa-download"></i> Download Job List Template';
    } else {
      link.innerHTML = '<i class="fa-solid fa-download"></i> Download Candidate List Template';
    }
  }

  function triggerBulkUploadFile() {
    const isJob = document.getElementById('up-job').checked;
    const typeStr = isJob ? 'Job List' : 'Candidate List';

    const input = document.createElement('input');
    input.type = 'file';
    input.multiple = true;
    input.accept = '.zip,.csv,.xlsx';
    input.onchange = e => {
      if(e.target.files.length > 0) {
        showToast('✅ Successfully uploaded ' + e.target.files.length + ' file(s) for ' + typeStr + '.', '#10B981');
        closeBulkUploadModal();
      }
    };
    input.click();
  }

  // ── BULK DELETE ──
  function handleBulkDelete() {
    document.getElementById('bulk-delete-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  }

  function closeBulkDeleteModal() {
    document.getElementById('bulk-delete-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }

  function handleBulkDeleteBackdropClick(e) {
    if (e.target === document.getElementById('bulk-delete-backdrop')) closeBulkDeleteModal();
  }

  function processBulkDelete() {
    const isJob = document.getElementById('del-job').checked;
    const typeStr = isJob ? 'Job List' : 'Candidate List';

    if(confirm('⚠️ Are you sure you want to permanently delete selected data from ' + typeStr + '? This action cannot be undone.')) {
      showToast('🗑️ Bulk delete for ' + typeStr + ' completed successfully.', '#EF4444');
      closeBulkDeleteModal();
    }
  }

  function handleExportContent() {
    document.getElementById('export-modal-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  }

  function closeExportModal() {
    document.getElementById('export-modal-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }

  function handleExportBackdropClick(e) {
    if (e.target === document.getElementById('export-modal-backdrop')) {
      closeExportModal();
    }
  }

  function processExport() {
    // 1. Determine list type
    const isJob = document.getElementById('exp-job').checked;
    const baseName = isJob ? 'jobs_export' : 'candidates_export';

    // 2. Determine file format
    let selectedFmt = 'pdf';
    if (document.getElementById('fmt-docx').checked) selectedFmt = 'docx';
    if (document.getElementById('fmt-excel').checked) selectedFmt = 'excel';

    const ext = selectedFmt === 'excel' ? 'xlsx' : selectedFmt;
    const filename = baseName + '.' + ext;
    const filepath = '../../Report/' + filename;

    // 3. Trigger download
    const link = document.createElement('a');
    link.href = filepath;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    showToast('✅ Exported file "' + filename + '" downloaded successfully!', '#15803D');
    closeExportModal();
  }


  // ── RECYCLE BIN ──
  function handleRecycleBin() {
    renderRecycleBin();
    document.getElementById('recycle-bin-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  }

  function closeRecycleBinModal() {
    document.getElementById('recycle-bin-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }

  function handleRecycleBinBackdropClick(e) {
    if (e.target === document.getElementById('recycle-bin-backdrop')) closeRecycleBinModal();
  }

  function renderRecycleBin() {
    const tbody = document.getElementById('recycle-bin-tbody');
    if (!deletedArticles.length) {
      tbody.innerHTML = `<tr><td colspan="4" style="text-align:center;padding:30px;color:var(--text-muted);">Recycle bin is empty.</td></tr>`;
      return;
    }
    
    tbody.innerHTML = deletedArticles.map(a => `
      <tr>
        <td class="cms-title-cell">${a.title}</td>
        <td>${a.type}</td>
        <td>${new Date().toLocaleDateString('en-GB')}</td>
        <td>
          <div class="action-icons" style="justify-content: flex-start; gap: 12px;">
            <button title="View" onclick="viewDeletedArticle(${a.id})"><i class="fa-regular fa-eye"></i></button>
            <button title="Restore" onclick="restoreArticle(${a.id})" style="color: #10B981;"><i class="fa-solid fa-rotate-left"></i></button>
            <button title="Permanently Delete" onclick="hardDeleteArticle(${a.id})" style="color: #EF4444;"><i class="fa-regular fa-trash-can"></i></button>
          </div>
        </td>
      </tr>
    `).join('');
  }

  function viewDeletedArticle(id) {
    const a = deletedArticles.find(x => x.id === id);
    if (!a) return;
    
    document.getElementById('view-title').innerText = a.title;
    document.getElementById('view-type').innerText = a.type;
    document.getElementById('view-category').innerText = a.category;
    document.getElementById('view-author').innerText = a.author;
    document.getElementById('view-desc').innerText = a.desc;
    document.getElementById('view-body').innerText = a.body;
    
    const statusEl = document.getElementById('view-status');
    statusEl.innerText = 'Deleted';
    statusEl.style.background = '#FEE2E2';
    statusEl.style.color = '#EF4444';

    document.getElementById('view-article-backdrop').classList.add('open');
    // Ensure overflow remains hidden since it's already hidden by recycle bin modal
  }

  function restoreArticle(id) {
    const idx = deletedArticles.findIndex(x => x.id === id);
    if(idx !== -1) {
      articles.unshift(deletedArticles[idx]);
      deletedArticles.splice(idx, 1);
      saveData();
      applyFilters();
      renderRecycleBin();
      showToast('Article restored successfully!', '#10B981');
    }
  }

  function hardDeleteArticle(id) {
    if(confirm('Are you sure you want to PERMANENTLY delete this article? This action cannot be undone.')) {
      deletedArticles = deletedArticles.filter(x => x.id !== id);
      saveData();
      renderRecycleBin();
      showToast('Article permanently deleted.', '#EF4444');
    }
  }

  // ── BACKUP & RECOVERY ──
  function handleBackupNow() {
    document.getElementById('backup-now-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  }
  function closeBackupNowModal() {
    document.getElementById('backup-now-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }
  function handleBackupNowBackdropClick(e) {
    if (e.target === document.getElementById('backup-now-backdrop')) closeBackupNowModal();
  }
  function processBackupNow() {
    const from = document.getElementById('backup-from-date').value;
    const to = document.getElementById('backup-to-date').value;
    if(!from || !to) {
      showToast('⚠️ Please select both From and To dates.', '#F59E0B');
      return;
    }

    let selectedData = 'full';
    const dataRadios = document.getElementsByName('bk-data');
    for (const radio of dataRadios) {
      if (radio.checked) selectedData = radio.value;
    }
    
    closeBackupNowModal();

    const widget = document.getElementById('task-progress-widget');
    const bar = document.getElementById('task-progress-bar');
    const pct = document.getElementById('task-progress-percent');
    const title = document.getElementById('task-progress-title');
    const desc = document.getElementById('task-progress-desc');
    
    widget.style.display = 'flex';
    title.innerText = 'Portal Backup in progress...';
    desc.innerText = 'Preparing data...';
    bar.style.width = '0%';
    pct.innerText = '0%';
    bar.style.background = '#0A4BD2';

    let progress = 0;
    const interval = setInterval(() => {
      progress += Math.floor(Math.random() * 15) + 5;
      if(progress > 100) progress = 100;
      
      bar.style.width = progress + '%';
      pct.innerText = progress + '%';
      
      if(progress > 30) desc.innerText = 'Compressing Candidate & Job Lists...';
      if(progress > 70) desc.innerText = 'Finalizing backup archive...';
      
      if(progress === 100) {
        clearInterval(interval);
        title.innerText = 'Backup Complete!';
        desc.innerText = 'Downloading backup file...';
        bar.style.background = '#10B981';
        pct.style.color = '#10B981';
        showToast('✅ Backup finished successfully.', '#15803D');

        const filename = 'backup_' + selectedData + '.zip';
        const filepath = '../../Report/' + filename;
        const link = document.createElement('a');
        link.href = filepath;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        setTimeout(() => {
          widget.style.display = 'none';
          pct.style.color = '#0A4BD2';
        }, 3500);
      }
    }, 600);
  }

  function handleConfigureBackup() {
    document.getElementById('configure-backup-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  }
  function closeConfigureBackupModal() {
    document.getElementById('configure-backup-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }
  function handleConfigureBackupBackdropClick(e) {
    if (e.target === document.getElementById('configure-backup-backdrop')) closeConfigureBackupModal();
  }
  function processConfigureBackup() {
    const freq = document.getElementById('backup-frequency').value;
    showToast(`✅ Backup schedule configured to run ${freq}.`, '#15803D');
    closeConfigureBackupModal();
  }

  function handleRestoreBackup() {
    document.getElementById('restore-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  }
  function closeRestoreModal() {
    document.getElementById('restore-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }
  function handleRestoreBackdropClick(e) {
    if (e.target === document.getElementById('restore-backdrop')) closeRestoreModal();
  }
  function processRestore() {
    const file = document.getElementById('restore-file').value;
    showToast(`✅ Data successfully restored from backup ${file}.`, '#15803D');
    closeRestoreModal();
  }

  // ── HELP MODAL ──
  function openHelpModal(type) {
    const title = document.getElementById('help-modal-title');
    const body = document.getElementById('help-modal-body');
    
    if(type === 'articles') {
      title.innerHTML = '<i class="fa-solid fa-file-lines" style="color:#0A4BD2; margin-right:8px;"></i> Manage Help Articles';
      body.innerHTML = `
        <h3 style="color:#1E293B; margin-top:0; font-size:1rem;">Steps to Manage Help Articles:</h3>
        <ol style="padding-left:20px; display:flex; flex-direction:column; gap:12px; margin-bottom:0;">
          <li><strong>Create an Article:</strong> Click the "+ Create Article" button at the top right of the Content Management table.</li>
          <li><strong>Fill Details:</strong> Provide the Title, Type, Category, Author, Description, and the full body content.</li>
          <li><strong>Publish:</strong> Set the status to "Published" to make it live instantly.</li>
          <li><strong>Edit/Update:</strong> Find the article in the table and click the <i class="fa-regular fa-pen-to-square"></i> (Edit) icon to make changes.</li>
          <li><strong>Delete:</strong> Click the <i class="fa-regular fa-trash-can"></i> (Delete) icon to move the article to the Recycle Bin.</li>
        </ol>
      `;
    } else if (type === 'faqs') {
      title.innerHTML = '<i class="fa-solid fa-comments" style="color:#0A4BD2; margin-right:8px;"></i> Manage FAQs';
      body.innerHTML = `
        <h3 style="color:#1E293B; margin-top:0; font-size:1rem;">Steps to Manage FAQs:</h3>
        <ol style="padding-left:20px; display:flex; flex-direction:column; gap:12px; margin-bottom:0;">
          <li><strong>Identify common questions:</strong> Collect recurring questions from users and support tickets.</li>
          <li><strong>Create FAQ Content:</strong> Click "+ Create Article", and classify it under the relevant category.</li>
          <li><strong>Write Clear Answers:</strong> Ensure the question is the Title, and the answer is provided clearly in the body.</li>
          <li><strong>Regular Updates:</strong> Audit your FAQs monthly to ensure all steps and product references remain accurate.</li>
        </ol>
      `;
    }
    
    document.getElementById('help-modal-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  }

  function closeHelpModal() {
    document.getElementById('help-modal-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }

  function handleHelpBackdropClick(e) {
    if (e.target === document.getElementById('help-modal-backdrop')) closeHelpModal();
  }

  // ── INITIALIZE ──function loadLandingPageContent() {
  let dataStr = '{}';
  try { dataStr = localStorage.getItem(LP_KEY) || '{}'; } catch(e) {}
  const data = JSON.parse(dataStr);
  
  const aboutEl = document.getElementById('lp-about');
  if (aboutEl) aboutEl.value = data.about || 'ProPath is a premium, state-of-the-art job board built by Synergech to empower high-potential design, product, and tech professionals. Our mission is to accelerate engineering careers by bridging connections to top-tier organizations and high-growth start-ups.';
  
  const contactEl = document.getElementById('lp-contact');
  if (contactEl) contactEl.value = data.contact || 'Rayala Techno Park, 1st Floor, 144, Rajiv Gandhi Salai (OMR), Kottivakkam, Chennai, Tamil Nadu 600041';
  
  const careersEl = document.getElementById('lp-careers');
  if (careersEl) careersEl.value = data.careers || 'We are actively recruiting software developers, product owners, and growth specialists across our Chennai, Bangalore, and Pune centers. Check out our careers section to learn more.';
  
  const resourcesEl = document.getElementById('lp-resources');
  if (resourcesEl) resourcesEl.value = data.resources || 'Explore opportunities matching your professional criteria and connect with premium recruiters instantly.';
  
  const howEl = document.getElementById('lp-how-we-hire');
  if (howEl) howEl.value = data.howWeHire || 'Our hiring process is designed to be transparent, fair, and fast. We evaluate skills over pedigree.';
  
  const whyEl = document.getElementById('lp-why-join');
  if (whyEl) whyEl.value = data.whyJoin || 'We offer competitive compensation, remote flexibility, and a culture of continuous learning.';
}

function saveLandingPageContent() {
  const data = {
    about: document.getElementById('lp-about').value,
    contact: document.getElementById('lp-contact').value,
    careers: document.getElementById('lp-careers').value,
    resources: document.getElementById('lp-resources').value,
    howWeHire: document.getElementById('lp-how-we-hire').value,
    whyJoin: document.getElementById('lp-why-join').value
  };
  try {
    localStorage.setItem(LP_KEY, JSON.stringify(data));
    showToast('✅ Landing Page content saved & published!', '#15803D');
  } catch(e) {
    showToast('⚠️ Could not save. Try opening page directly.', '#F59E0B');
  }
}

function previewLandingPageContent() {
  const data = {
    about: document.getElementById('lp-about').value,
    contact: document.getElementById('lp-contact').value,
    careers: document.getElementById('lp-careers').value,
    resources: document.getElementById('lp-resources').value,
    howWeHire: document.getElementById('lp-how-we-hire').value,
    whyJoin: document.getElementById('lp-why-join').value
  };
  try {
    localStorage.setItem('preview_landing_page_content', JSON.stringify(data));
    showToast('Opening preview...', '#0A4BD2');
    setTimeout(() => { window.open('../index.html?preview=true', '_blank'); }, 500);
  } catch(e) {
    showToast('⚠️ Could not open preview. Try opening page directly.', '#F59E0B');
  }

}
