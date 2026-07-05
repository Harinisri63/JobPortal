  const defaultCandidates = [
    { id:'CND-10234', name:'Arav Kumar',   title:'Senior DevOps Engineer', exp:'5-8 years', loc:'Bangalore',  status:'Shortlisted', applied:'12 May 2025', email:'arav.kumar@gmail.com',   phone:'+91 9876548321', resumeScore:85, atsScore:92, skills:['AWS','Docker','Kubernetes'], skills2:['Jenkins','CI/CD','Linux'], currentJob:'DevOps Engineer at Tech Solutions\nJan 2022 – Present (2.5 years)', accountStatus: 'Active' },
    { id:'CND-10235', name:'Priya Sharma', title:'React Developer',         exp:'3-5 years', loc:'Pune',       status:'Interview',   applied:'11 May 2025', email:'priya.sharma@gmail.com', phone:'+91 9845012345', resumeScore:78, atsScore:85, skills:['React','TypeScript','Redux'], skills2:['Node.js','GraphQL'], currentJob:'Frontend Developer at Infosys\nMar 2021 – Present (3.1 years)', accountStatus: 'Active' },
    { id:'CND-10236', name:'Rohan Joshi',  title:'Product Manager',         exp:'4-7 years', loc:'Hyderabad',  status:'Applied',     applied:'10 May 2025', email:'rohan.joshi@gmail.com',  phone:'+91 9823456780', resumeScore:72, atsScore:80, skills:['Roadmapping','Agile','Jira'], skills2:['SQL','Analytics'], currentJob:'Associate PM at Wipro\nJun 2020 – Present (4 years)', accountStatus: 'Active' },
    { id:'CND-10237', name:'Neha Singh',   title:'UI/UX Designer',          exp:'2-4 years', loc:'Delhi',      status:'Shortlisted', applied:'09 May 2025', email:'neha.singh@gmail.com',   phone:'+91 9900112233', resumeScore:88, atsScore:90, skills:['Figma','Adobe XD','Sketch'], skills2:['Prototyping','User Research'], currentJob:'UI Designer at HCL\nApr 2022 – Present (2.1 years)', accountStatus: 'Active' },
    { id:'CND-10238', name:'Vikram Mehta', title:'Data Analyst',            exp:'2-3 years', loc:'Mumbai',     status:'Applied',     applied:'08 May 2025', email:'vikram.mehta@gmail.com', phone:'+91 9711223344', resumeScore:75, atsScore:82, skills:['Python','SQL','Power BI'], skills2:['Tableau','Excel'], currentJob:'Analyst at Accenture\nJan 2023 – Present (1.4 years)', accountStatus: 'Active' },
    { id:'CND-10239', name:'Kavya',        title:'Backend Developer',       exp:'3-6 years', loc:'Bangalore',  status:'Interview',   applied:'07 May 2025', email:'kavya.m@gmail.com',      phone:'+91 9655443322', resumeScore:82, atsScore:88, skills:['Java','Spring Boot','MySQL'], skills2:['Redis','Kafka'], currentJob:'Backend Eng at TCS\nSep 2021 – Present (2.7 years)', accountStatus: 'Active' },
    { id:'CND-10240', name:'Arjun',        title:'Full Stack Developer',    exp:'4-6 years', loc:'Noida',      status:'Applied',     applied:'06 May 2025', email:'arjun.r@gmail.com',      phone:'+91 9876001234', resumeScore:80, atsScore:84, skills:['React','Node.js','MongoDB'], skills2:['AWS','Docker'], currentJob:'MERN Dev at Cognizant\nJul 2020 – Present (3.9 years)', accountStatus: 'Active' },
    { id:'CND-10241', name:'Simran',       title:'HR Executive',            exp:'1-2 years', loc:'Chandigarh', status:'Shortlisted', applied:'05 May 2025', email:'simran.k@gmail.com',     phone:'+91 9988776655', resumeScore:68, atsScore:72, skills:['Recruitment','HRMS','Onboarding'], skills2:['Payroll'], currentJob:'HR Trainee at Syntel\nAug 2023 – Present (0.8 years)', accountStatus: 'Active' },
    { id:'CND-10242', name:'Deepak',       title:'QA Engineer',             exp:'2-4 years', loc:'Pune',       status:'Applied',     applied:'04 May 2025', email:'deepak.v@gmail.com',     phone:'+91 9700123456', resumeScore:70, atsScore:76, skills:['Selenium','Manual Testing','JIRA'], skills2:['API Testing','Postman'], currentJob:'QA Analyst at Mphasis\nFeb 2022 – Present (2.3 years)', accountStatus: 'Active' }
  ];

  let candidates = JSON.parse(localStorage.getItem('candidatesData')) || defaultCandidates;
  if (!localStorage.getItem('candidatesData')) {
    localStorage.setItem('candidatesData', JSON.stringify(defaultCandidates));
  }

  function saveCandidates() {
    localStorage.setItem('candidatesData', JSON.stringify(candidates));
  }

  let selected = null;
  let currentPage = 1;
  const itemsPerPage = 8;

  // ── STATUS PILL ────────────────────────────────────────────────────
  function pillClass(s) {
    return s === 'Shortlisted' ? 'pill-shortlisted' : s === 'Interview' ? 'pill-interview' : s === 'Hired' ? 'pill-hired' : s === 'Rejected' ? 'pill-rejected' : 'pill-applied';
  }

  // ── RENDER TABLE ──────────────────────────────────────────────────
  function renderTable(data) {
    const totalPages = Math.ceil(data.length / itemsPerPage) || 1;
    const startIndex = (currentPage - 1) * itemsPerPage;
    const pageData = data.slice(startIndex, startIndex + itemsPerPage);

    const tbody = document.getElementById('cand-tbody');
    tbody.innerHTML = pageData.map(c => `
      <tr onclick="selectCandidate('${c.id}')" id="row-${c.id}" class="${selected === c.id ? 'selected' : ''}">
        <td><input type="checkbox" class="cand-chk" data-id="${c.id}" onclick="event.stopPropagation()"/></td>
        <td class="cand-name">${c.name}</td>
        <td style="font-size:0.7rem;">${c.title}</td>
        <td>${c.exp}</td>
        <td>${c.loc}</td>
        <td><span class="status-pill ${pillClass(c.status)}">${c.status}</span></td>
        <td style="white-space:nowrap;font-size:0.7rem;">${c.applied}</td>
        <td>
          <div style="display:flex;gap:6px;align-items:center;">
            <button title="View Details" style="background:none;border:none;cursor:pointer;color:var(--primary);font-size:0.82rem;" onclick="event.stopPropagation(); selectCandidate('${c.id}')"><i class="fa-regular fa-eye"></i></button>
            <button title="More" style="background:none;border:none;cursor:pointer;color:var(--text-muted);font-size:0.82rem;" onclick="event.stopPropagation()"><i class="fa-solid fa-ellipsis-vertical"></i></button>
          </div>
        </td>
      </tr>`).join('');

    renderPagination(totalPages);
  }

  function renderPagination(totalPages) {
    const container = document.getElementById('pagination-container');
    if (!container) return;
    let html = '';

    html += `<button onclick="goToPage(${currentPage - 1})" ${currentPage === 1 ? 'disabled style="opacity:0.5;cursor:not-allowed;"' : ''}>← Previous</button>`;

    for (let i = 1; i <= totalPages; i++) {
      html += `<button class="${i === currentPage ? 'pg-active' : ''}" onclick="goToPage(${i})">${i}</button>`;
    }

    html += `<button onclick="goToPage(${currentPage + 1})" ${currentPage === totalPages ? 'disabled style="opacity:0.5;cursor:not-allowed;"' : ''}>Next →</button>`;

    container.innerHTML = html;
  }

  window.goToPage = function(page) {
    currentPage = page;
    filterCandidates(false);
  };

  // ── FILTER ────────────────────────────────────────────────────────
  function filterCandidates(resetPage = true) {
    if (resetPage) currentPage = 1;
    const q      = document.getElementById('cand-search').value.toLowerCase();
    const status = document.getElementById('fil-status').value;
    const loc    = document.getElementById('fil-loc').value;
    const exp    = document.getElementById('fil-exp').value;
    const title  = document.getElementById('fil-title').value;
    const data = candidates.filter(c =>
      (!q      || c.name.toLowerCase().includes(q) || c.email.toLowerCase().includes(q) || c.title.toLowerCase().includes(q) || c.skills.some(s => s.toLowerCase().includes(q))) &&
      (!status || c.status === status) &&
      (!loc    || c.loc === loc) &&
      (!exp    || c.exp === exp) &&
      (!title  || c.title === title)
    );
    renderTable(data);
  }

  function resetFilter() {
    ['fil-title','fil-loc','fil-exp','fil-status'].forEach(id => document.getElementById(id).value = '');
    document.getElementById('cand-search').value = '';
    renderTable(candidates);
  }

  function toggleAll(el) {
    document.querySelectorAll('#cand-tbody input[type=checkbox]').forEach(c => c.checked = el.checked);
  }

  // ── OPEN / CLOSE MODAL ────────────────────────────────────────────
  function selectCandidate(id) {
    const c = candidates.find(x => x.id === id);
    if (!c) return;

    // Update modal header
    document.getElementById('cm-hdr-name').textContent = c.name;

    // Build modal body
    const bar = pct => `<div class="cm-bar"><div class="cm-bar-fill" style="width:${pct}%"></div></div>`;
    const skillTag = s => `<span class="cm-skill">${s}</span>`;
    const allSkills = [...c.skills, ...c.skills2].map(skillTag).join('');

    document.getElementById('cm-body').innerHTML = `
      <div class="cm-name">${c.name}</div>
      <div class="cm-role">${c.title}</div>
      <div class="cm-info">
        <span><i class="fa-solid fa-location-dot"></i>${c.loc}, India</span>
        <span><i class="fa-regular fa-envelope"></i>${c.email}</span>
        <span><i class="fa-solid fa-phone"></i>${c.phone}</span>
      </div>
      <div class="cm-status-row">
        <span class="status-pill ${pillClass(c.status)}">${c.status}</span>
        <span class="cm-applied">Applied on <b>${c.applied}</b></span>
      </div>
      <div class="cm-actions">
        <a href="../candidateprofile/profile.html" class="cm-btn-prim">View Profile</a>
        <button class="cm-btn-sec"><i class="fa-solid fa-download me-1"></i>Download Resume</button>
      </div>
      <hr class="cm-divider"/>
      <div class="cm-score-label">Resume Score</div>
      <div class="cm-bar-wrap">
        <span class="cm-score-num">${c.resumeScore}/100</span>
        ${bar(c.resumeScore)}
        <span style="font-size:0.65rem;color:var(--text-muted);">+</span>
      </div>
      <div class="cm-score-label">ATS Score</div>
      <div class="cm-bar-wrap">
        <span class="cm-score-num">${c.atsScore}</span>
        ${bar(c.atsScore)}
        <span style="font-size:0.65rem;color:var(--text-muted);">+</span>
      </div>
      <hr class="cm-divider"/>
      <div class="cm-skills-label">Skills Match</div>
      <div class="cm-skills">${allSkills}</div>
      <div class="cm-jobs-label">Current Jobs</div>
      <div class="cm-jobs-val">${c.currentJob.replace('\n', '<br/>')}</div>
      <hr class="cm-divider"/>
      <div class="cm-notes-label">Notes</div>
      <div class="cm-notes"><textarea placeholder="Add your notes here..."></textarea></div>
      <div class="cm-approve-reject-row">
        <button class="cm-btn-approve" onclick="approveCandidate('${c.id}')">
          <i class="fa-solid fa-check"></i> Approve
        </button>
        <button class="cm-btn-reject" onclick="rejectCandidate('${c.id}')">
          <i class="fa-solid fa-xmark"></i> Reject
        </button>
      </div>
    `;

    // Open modal
    document.getElementById('cand-modal-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  }

  function closeModal() {
    document.getElementById('cand-modal-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }

  function handleBackdropClick(e) {
    if (e.target === document.getElementById('cand-modal-backdrop')) closeModal();
  }

  // Close on Escape key
  document.addEventListener('keydown', e => { if (e.key === 'Escape') closeModal(); });

  // ── APPROVE / REJECT (Account Activation/Deactivation for duplicate handling) ──
  function approveCandidate(id) {
    const c = candidates.find(x => x.id === id);
    if (!c) return;
    
    if (!confirm('Are you sure you want to approve and activate the user account for "' + c.name + '"?')) {
      return;
    }
    
    c.accountStatus = 'Active';
    saveCandidates();
    updateMetrics();
    showCandToast('✅ User account for "' + c.name + '" activated successfully!', '#15803D');
    
    // Do not change c.status or status pill as requested
    setTimeout(closeModal, 1800);
  }

  function rejectCandidate(id) {
    const c = candidates.find(x => x.id === id);
    if (!c) return;
    
    if (!confirm('Are you sure you want to reject and deactivate candidate "' + c.name + '" as a duplicate?')) {
      return;
    }
    
    c.accountStatus = 'Deactivated';
    saveCandidates();
    updateMetrics();
    showCandToast('❌ Duplicate candidate "' + c.name + '" rejected & deactivated.', '#B91C1C');
    
    // Do not change c.status or status pill as requested
    setTimeout(closeModal, 1800);
  }

  // ── BULK APPROVE / REJECT ──────────────────────────────────────────
  function getSelectedCandidateIds() {
    const checkboxes = document.querySelectorAll('#cand-tbody input.cand-chk:checked');
    return Array.from(checkboxes).map(cb => cb.getAttribute('data-id'));
  }

  function bulkApprove() {
    const ids = getSelectedCandidateIds();
    if (ids.length === 0) {
      showCandToast('⚠️ Please select at least one candidate checkbox.', '#FEF3C7');
      return;
    }
    if (!confirm('Are you sure you want to approve and activate ' + ids.length + ' selected candidate account(s)?')) {
      return;
    }
    ids.forEach(id => {
      const c = candidates.find(x => x.id === id);
      if (c) c.accountStatus = 'Active';
    });
    saveCandidates();
    updateMetrics();
    showCandToast('✅ Approved and activated ' + ids.length + ' candidate account(s)!', '#15803D');
    
    // Clear selection
    document.getElementById('chk-all').checked = false;
    renderTable(candidates);
  }

  function bulkReject() {
    const ids = getSelectedCandidateIds();
    if (ids.length === 0) {
      showCandToast('⚠️ Please select at least one candidate checkbox.', '#FEF3C7');
      return;
    }
    if (!confirm('Are you sure you want to reject and deactivate ' + ids.length + ' selected candidate account(s)?')) {
      return;
    }
    ids.forEach(id => {
      const c = candidates.find(x => x.id === id);
      if (c) c.accountStatus = 'Deactivated';
    });
    saveCandidates();
    updateMetrics();
    showCandToast('❌ Rejected and deactivated ' + ids.length + ' duplicate candidate(s).', '#B91C1C');
    
    document.getElementById('chk-all').checked = false;
    renderTable(candidates);
  }

  function showCandToast(msg, bg) {
    let t = document.getElementById('cand-toast');
    if (!t) {
      t = document.createElement('div');
      t.id = 'cand-toast';
      t.style.cssText = 'position:fixed;bottom:28px;right:28px;padding:13px 24px;border-radius:10px;font-size:0.8rem;font-weight:600;color:#fff;z-index:99999;display:none;font-family:Poppins,sans-serif;box-shadow:0 8px 24px rgba(0,0,0,0.2);animation:fadeUp 0.25s ease;';
      document.body.appendChild(t);
    }
    t.textContent = msg;
    t.style.background = bg;
    t.style.display = 'block';
    clearTimeout(t._timer);
    t._timer = setTimeout(() => { t.style.display = 'none'; }, 2800);
  }

  // ── EXPORT CANDIDATE FUNCTIONS ──────────────────────────────────────
  function openExportModal() {
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
    let selectedFmt = 'pdf';
    if (document.getElementById('fmt-docx').checked) selectedFmt = 'docx';
    if (document.getElementById('fmt-excel').checked) selectedFmt = 'excel';

    const ext = selectedFmt === 'excel' ? 'xlsx' : selectedFmt;
    const filename = 'candidates_export.' + ext;
    const filepath = '../../Report/' + filename;

    // Trigger download
    const link = document.createElement('a');
    link.href = filepath;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    showCandToast('✅ Exported file "' + filename + '" downloaded successfully!', '#15803D');
    closeExportModal();
  }

  function updateMetrics() {
    const totalCount = candidates.length;
    const newCount = candidates.filter(c => c.status === 'Applied').length;
    const shortlistedCount = candidates.filter(c => c.status === 'Shortlisted').length;
    const hiredCount = candidates.filter(c => c.status === 'Hired').length;

    const SNAP_KEY = 'user_mgmt_stat_snapshot';
    const prevSnap = JSON.parse(localStorage.getItem(SNAP_KEY) || 'null');

    function trendHTML(current, prevVal) {
      if (prevVal === null || prevVal === undefined) return '<span style="color:var(--text-muted);font-size:0.65rem;">No prior data</span>';
      const diff = current - prevVal;
      if (diff === 0) return '<span style="color:var(--text-muted);font-size:0.65rem;">No change</span>';
      const pct = prevVal === 0 ? 100 : Math.abs(((diff / prevVal) * 100)).toFixed(1);
      const sign = diff > 0 ? '+' : '-';
      const color = diff > 0 ? '#22C55E' : '#EF4444';
      return `<span style="color:${color};font-weight:700;">${sign}${pct}% vs last session</span>`;
    }

    function setStat(valId, trendId, current, prevVal) {
      const vEl = document.getElementById(valId);
      const tEl = document.getElementById(trendId);
      if (vEl) vEl.textContent = current.toLocaleString();
      if (tEl) tEl.innerHTML = trendHTML(current, prevVal);
    }

    const p = prevSnap || {};
    setStat('metric-total-candidates',       'metric-total-candidates-sub',       totalCount,       p.total);
    setStat('metric-new-candidates',         'metric-new-candidates-sub',         newCount,         p.new);
    setStat('metric-shortlisted-candidates', 'metric-shortlisted-candidates-sub', shortlistedCount, p.shortlisted);
    setStat('metric-hired-candidates',       'metric-hired-candidates-sub',       hiredCount,       p.hired);

    // Save snapshot for next comparison
    localStorage.setItem(SNAP_KEY, JSON.stringify({
      total: totalCount,
      new: newCount,
      shortlisted: shortlistedCount,
      hired: hiredCount
    }));
  }

  // ── INIT ─────────────────────────────────────────────────────────
  renderTable(candidates);
  updateMetrics();