// ════════════════════════════════════════════════════════════════════
  //  DATA
  // ════════════════════════════════════════════════════════════════════

  // Load jobs from localStorage (shared with jobmanagement.html) or use defaults
  const defaultJobs = [
    { id:'JOB-1021', title:'Senior DevOps Engineer',  dept:'Engineering', loc:'Bangalore', type:'Full Time', exp:'5-8 years',  status:'Active',  posted:'12 May 2025' },
    { id:'JOB-1015', title:'React Developer',          dept:'Engineering', loc:'Remote',    type:'Full Time', exp:'2-4 years',  status:'Active',  posted:'11 May 2025' },
    { id:'JOB-1008', title:'Product Manager',          dept:'Product',     loc:'Bangalore', type:'Full Time', exp:'3-6 years',  status:'Active',  posted:'10 May 2025' },
    { id:'JOB-1002', title:'UI/UX Designer',           dept:'Design',      loc:'Pune',      type:'Full Time', exp:'2-4 years',  status:'Active',  posted:'09 May 2025' },
    { id:'JOB-0991', title:'Data Analyst',             dept:'Analytics',   loc:'Hyderabad', type:'Full Time', exp:'1-3 years',  status:'Active',  posted:'08 May 2025' },
    { id:'JOB-0982', title:'Backend Developer',        dept:'Engineering', loc:'Remote',    type:'Full Time', exp:'3-5 years',  status:'Closed',  posted:'07 May 2025' },
    { id:'JOB-0975', title:'HR Executive',             dept:'HR',          loc:'Mumbai',    type:'Full Time', exp:'1-2 years',  status:'Active',  posted:'07 May 2025' },
    { id:'JOB-0961', title:'Full Stack Developer',     dept:'Engineering', loc:'Bangalore', type:'Full Time', exp:'4-7 years',  status:'Active',  posted:'06 May 2025' },
  ];
  const jobsData = JSON.parse(localStorage.getItem('jobsData')) || defaultJobs;

  // Applicants per job
  const defaultApplicantsData = {
    'JOB-1021': [
      { id:'CND-10234', name:'Arjun Mehta',      email:'arjun.mehta@gmail.com',    phone:'+91 9876543210', loc:'Bangalore', exp:'6 years',    status:'Shortlisted', applied:'18 May 2025', resumeScore:85, atsScore:92, skills:['AWS','Docker','Kubernetes'], skills2:['Jenkins','CI/CD'],       currentJob:'DevOps Eng at Infosys\nJan 2019 – Present (5.4 yrs)', avatarBg:'#6366F1', avatar:'AM' },
      { id:'CND-10235', name:'Priya Sharma',      email:'priya.sharma@gmail.com',   phone:'+91 9845012345', loc:'Remote',    exp:'7 years',    status:'Interview',   applied:'19 May 2025', resumeScore:90, atsScore:95, skills:['Terraform','Ansible','GCP'],  skills2:['Python','Linux'],        currentJob:'SRE at Wipro\nAug 2017 – Present (6.9 yrs)',         avatarBg:'#8B5CF6', avatar:'PS' },
      { id:'CND-10236', name:'Rohit Nair',        email:'rohit.nair@gmail.com',     phone:'+91 9823456780', loc:'Pune',      exp:'5 years',    status:'Rejected',    applied:'20 May 2025', resumeScore:62, atsScore:68, skills:['AWS','Linux'],               skills2:['Bash'],                  currentJob:'Sys Admin at HCL\nMar 2020 – Present (4.2 yrs)',     avatarBg:'#EF4444', avatar:'RN' },
      { id:'CND-10237', name:'Sneha Iyer',        email:'sneha.iyer@gmail.com',     phone:'+91 9900112233', loc:'Hyderabad', exp:'8 years',    status:'Hired',       applied:'20 May 2025', resumeScore:94, atsScore:97, skills:['Kubernetes','Docker','AWS'],  skills2:['Grafana','Prometheus'],  currentJob:'Lead DevOps at TCS\nJun 2016 – Present (8 yrs)',     avatarBg:'#10B981', avatar:'SI' },
      { id:'CND-10238', name:'Karan Verma',       email:'karan.verma@gmail.com',    phone:'+91 9711223344', loc:'Bangalore', exp:'6 years',    status:'Applied',     applied:'21 May 2025', resumeScore:78, atsScore:82, skills:['CI/CD','Jenkins','Git'],      skills2:['AWS','Docker'],          currentJob:'DevOps at Mphasis\nFeb 2019 – Present (5.3 yrs)',    avatarBg:'#F59E0B', avatar:'KV' },
    ],
    'JOB-1015': [
      { id:'CND-10240', name:'Divya Reddy',       email:'divya.reddy@gmail.com',    phone:'+91 9654321098', loc:'Remote',    exp:'3 years',    status:'Shortlisted', applied:'17 May 2025', resumeScore:80, atsScore:86, skills:['React','TypeScript','Redux'], skills2:['GraphQL','Jest'],        currentJob:'Frontend Dev at Accenture\nMay 2021 – Present',      avatarBg:'#EC4899', avatar:'DR' },
      { id:'CND-10241', name:'Aakash Singh',      email:'aakash.singh@gmail.com',   phone:'+91 9987654321', loc:'Mumbai',    exp:'2.5 years',  status:'Applied',     applied:'18 May 2025', resumeScore:70, atsScore:75, skills:['React','CSS','HTML'],         skills2:['Bootstrap','jQuery'],    currentJob:'UI Dev at Cognizant\nNov 2022 – Present',            avatarBg:'#0EA5E9', avatar:'AS' },
      { id:'CND-10242', name:'Meena Pillai',      email:'meena.pillai@gmail.com',   phone:'+91 9765432109', loc:'Chennai',   exp:'4 years',    status:'Interview',   applied:'18 May 2025', resumeScore:88, atsScore:91, skills:['React','Next.js','Tailwind'],  skills2:['Node.js','MongoDB'],    currentJob:'React Dev at Tech M\nJan 2021 – Present',            avatarBg:'#14B8A6', avatar:'MP' },
    ],
    'JOB-1008': [
      { id:'CND-10244', name:'Vikram Joshi',      email:'vikram.joshi@gmail.com',   phone:'+91 9811223344', loc:'Bangalore', exp:'5 years',    status:'Shortlisted', applied:'14 May 2025', resumeScore:82, atsScore:88, skills:['Roadmapping','Agile','Jira'],  skills2:['SQL','Figma'],          currentJob:'PM at Flipkart\nMar 2020 – Present',                avatarBg:'#6366F1', avatar:'VJ' },
      { id:'CND-10245', name:'Ananya Das',        email:'ananya.das@gmail.com',     phone:'+91 9922334455', loc:'Delhi',     exp:'4 years',    status:'Applied',     applied:'15 May 2025', resumeScore:74, atsScore:79, skills:['Product Strategy','OKRs'],     skills2:['Analytics','Excel'],    currentJob:'APM at Amazon\nJul 2021 – Present',                 avatarBg:'#8B5CF6', avatar:'AD' },
      { id:'CND-10246', name:'Suresh Kumar',      email:'suresh.kumar@gmail.com',   phone:'+91 9633445566', loc:'Pune',      exp:'6 years',    status:'Interview',   applied:'15 May 2025', resumeScore:86, atsScore:90, skills:['Go-to-Market','SCRUM','PRD'],  skills2:['SQL','Tableau'],        currentJob:'Senior PM at Razorpay\nJan 2019 – Present',          avatarBg:'#F59E0B', avatar:'SK' },
    ],
    'JOB-1002': [
      { id:'CND-10248', name:'Harini Sri',        email:'harini.sri@gmail.com',     phone:'+91 9876543210', loc:'Chennai',   exp:'3 years',    status:'Shortlisted', applied:'12 May 2025', resumeScore:88, atsScore:93, skills:['Figma','Adobe XD','Sketch'],   skills2:['Prototyping','Zeplin'], currentJob:'UI Designer at Zoho\nApr 2022 – Present',            avatarBg:'#EC4899', avatar:'HS' },
      { id:'CND-10249', name:'Rajan Pillai',      email:'rajan.pillai@gmail.com',   phone:'+91 9765001122', loc:'Pune',      exp:'2 years',    status:'Applied',     applied:'13 May 2025', resumeScore:72, atsScore:76, skills:['Figma','CSS','Illustration'],  skills2:['Canva'],               currentJob:'Graphic Designer at BYJU\'S\nJun 2023 – Present',   avatarBg:'#0EA5E9', avatar:'RP' },
    ],
    'JOB-0991': [
      { id:'CND-10251', name:'Chitra Nair',       email:'chitra.nair@gmail.com',    phone:'+91 9633221100', loc:'Hyderabad', exp:'2 years',    status:'Applied',     applied:'11 May 2025', resumeScore:68, atsScore:72, skills:['Python','SQL','Excel'],         skills2:['Power BI'],            currentJob:'Data Analyst at Deloitte\nFeb 2023 – Present',       avatarBg:'#10B981', avatar:'CN' },
      { id:'CND-10252', name:'Hari Prasad',       email:'hari.prasad@gmail.com',    phone:'+91 9988776655', loc:'Remote',    exp:'1.5 years',  status:'Shortlisted', applied:'12 May 2025', resumeScore:75, atsScore:80, skills:['Tableau','SQL','R'],           skills2:['Matplotlib'],          currentJob:'Junior Analyst at Accenture\nSep 2023 – Present',    avatarBg:'#6366F1', avatar:'HP' },
    ],
    'JOB-0982': [
      { id:'CND-10254', name:'Nikhil Tiwari',     email:'nikhil.tiwari@gmail.com',  phone:'+91 9871234560', loc:'Remote',    exp:'4 years',    status:'Shortlisted', applied:'09 May 2025', resumeScore:82, atsScore:87, skills:['Java','Spring Boot','MySQL'],   skills2:['Redis','Kafka'],        currentJob:'Backend Dev at Infosys\nMar 2021 – Present',          avatarBg:'#8B5CF6', avatar:'NT' },
      { id:'CND-10255', name:'Pooja Saxena',      email:'pooja.saxena@gmail.com',   phone:'+91 9855443322', loc:'Mumbai',    exp:'3 years',    status:'Rejected',    applied:'10 May 2025', resumeScore:58, atsScore:62, skills:['Node.js','MongoDB','REST'],     skills2:['Express'],             currentJob:'Dev at Wipro\nJan 2022 – Present',                   avatarBg:'#EF4444', avatar:'PX' },
    ],
    'JOB-0975': [
      { id:'CND-10257', name:'Lakshmi Rao',       email:'lakshmi.rao@gmail.com',    phone:'+91 9700123456', loc:'Mumbai',    exp:'1 year',     status:'Applied',     applied:'09 May 2025', resumeScore:65, atsScore:70, skills:['Recruitment','HRMS'],          skills2:['Onboarding'],          currentJob:'HR Trainee at TCS\nAug 2023 – Present',              avatarBg:'#F59E0B', avatar:'LR' },
    ],
    'JOB-0961': [
      { id:'CND-10259', name:'Siddharth Gupta',   email:'siddharth.g@gmail.com',    phone:'+91 9876001234', loc:'Bangalore', exp:'5 years',    status:'Shortlisted', applied:'08 May 2025', resumeScore:83, atsScore:88, skills:['React','Node.js','MongoDB'],   skills2:['AWS','Docker'],         currentJob:'MERN Dev at Cognizant\nJul 2020 – Present',           avatarBg:'#6366F1', avatar:'SG' },
      { id:'CND-10260', name:'Nandini Kulkarni',  email:'nandini.k@gmail.com',      phone:'+91 9645678901', loc:'Pune',      exp:'4 years',    status:'Interview',   applied:'09 May 2025', resumeScore:79, atsScore:84, skills:['Angular','TypeScript','Java'], skills2:['Spring','SQL'],         currentJob:'Full Stack at HCL\nMar 2021 – Present',               avatarBg:'#EC4899', avatar:'NK' },
      { id:'CND-10261', name:'Abhinav Sharma',    email:'abhinav.s@gmail.com',      phone:'+91 9811009988', loc:'Remote',    exp:'6 years',    status:'Applied',     applied:'09 May 2025', resumeScore:86, atsScore:89, skills:['React','Django','PostgreSQL'], skills2:['Redis','GraphQL'],      currentJob:'Tech Lead at Mphasis\nJan 2019 – Present',            avatarBg:'#14B8A6', avatar:'AB' },
    ],
  };

  const storedApplicants = localStorage.getItem('adminApplicantsData');
  let applicantsData = storedApplicants ? JSON.parse(storedApplicants) : defaultApplicantsData;
  if (!storedApplicants) {
    localStorage.setItem('adminApplicantsData', JSON.stringify(applicantsData));
  }

  // Current job context for Frame 2
  let currentJobId = null;
  let currentCandidates = [];
  let currentPage = 1;
  const itemsPerPage = 8;

  // ════════════════════════════════════════════════════════════════════
  //  FRAME 1 — RENDER JOB CARDS
  // ════════════════════════════════════════════════════════════════════
  function pillClass(s) {
    return s === 'Shortlisted' ? 'pill-shortlisted' : s === 'Interview' ? 'pill-interview' : s === 'Hired' ? 'pill-hired' : s === 'Rejected' ? 'pill-rejected' : 'pill-applied';
  }

  function renderJobCards(data) {
    const grid = document.getElementById('jobs-grid');
    if (!data.length) {
      grid.innerHTML = `<div class="empty-state" style="grid-column:1/-1;"><i class="fa-regular fa-folder-open"></i><p>No jobs found matching your search.</p></div>`;
      return;
    }
    grid.innerHTML = data.map(j => {
      const appsArr = applicantsData[j.id] || [];
      const total   = appsArr.length;
      const applied      = appsArr.filter(a => a.status === 'Applied').length;
      const shortlisted  = appsArr.filter(a => a.status === 'Shortlisted').length;
      const interview    = appsArr.filter(a => a.status === 'Interview').length;
      const hired        = appsArr.filter(a => a.status === 'Hired').length;
      const statusCls = j.status === 'Active' ? 'active' : j.status === 'Closed' ? 'closed' : 'draft';

      return `
        <div class="job-app-card">
          <div class="jac-top">
            <div>
              <div class="jac-title">${j.title}</div>
              <div class="jac-dept">${j.dept} &middot; ${j.loc}</div>
            </div>
            <span class="jac-badge ${statusCls}">${j.status}</span>
          </div>
          <div class="jac-meta">
            <span><i class="fa-solid fa-briefcase"></i> ${j.type}</span>
            <span><i class="fa-solid fa-clock"></i> ${j.exp}</span>
            <span><i class="fa-regular fa-calendar"></i> ${j.posted}</span>
          </div>
          <div class="jac-applicant-count">
            <div class="jac-count-num">${total}</div>
            <div>
              <div class="jac-count-lbl"><b>Total Applicants</b>applied for this role</div>
            </div>
          </div>
          <div class="jac-breakdown">
            <div class="jac-bk applied">
              <div class="bk-num">${applied}</div>
              <div class="bk-lbl">Applied</div>
            </div>
            <div class="jac-bk shortlisted">
              <div class="bk-num">${shortlisted}</div>
              <div class="bk-lbl">Shortlisted</div>
            </div>
            <div class="jac-bk interview">
              <div class="bk-num">${interview}</div>
              <div class="bk-lbl">Interview</div>
            </div>
            <div class="jac-bk hired">
              <div class="bk-num">${hired}</div>
              <div class="bk-lbl">Hired</div>
            </div>
          </div>
          <div class="jac-footer">
            <span class="jac-posted">Posted: ${j.posted} &nbsp;|&nbsp; ID: ${j.id}</span>
            <button class="btn-view-all" onclick="viewCandidates('${j.id}')">
              <i class="fa-solid fa-users"></i> View All Candidates
            </button>
          </div>
        </div>`;
    }).join('');
  }

  function filterJobCards() {
    const q   = document.getElementById('job-search').value.toLowerCase();
    const st  = document.getElementById('job-status-filter').value;
    const data = jobsData.filter(j =>
      (!q  || j.title.toLowerCase().includes(q) || j.dept.toLowerCase().includes(q) || j.loc.toLowerCase().includes(q)) &&
      (!st || j.status === st)
    );
    renderJobCards(data);
  }

  // ════════════════════════════════════════════════════════════════════
  //  FRAME 2 — CANDIDATES TABLE
  // ════════════════════════════════════════════════════════════════════
  function viewCandidates(jobId) {
    currentJobId = jobId;
    const job = jobsData.find(j => j.id === jobId);
    currentCandidates = applicantsData[jobId] || [];

    // Update banner
    document.getElementById('current-job-title').textContent = job ? job.title : jobId;
    document.getElementById('banner-title').textContent      = job ? job.title : jobId;
    document.getElementById('banner-count').textContent      = currentCandidates.length;
    document.getElementById('page-title').textContent        = 'Applications';
    if (job) {
      document.getElementById('banner-meta').innerHTML = `
        <span><i class="fa-solid fa-building"></i> ${job.dept}</span>
        <span><i class="fa-solid fa-location-dot"></i> ${job.loc}</span>
        <span><i class="fa-solid fa-briefcase"></i> ${job.type}</span>
        <span><i class="fa-solid fa-clock"></i> ${job.exp}</span>
        <span><i class="fa-regular fa-calendar"></i> Posted: ${job.posted}</span>
      `;
    }

    // Reset filters
    document.getElementById('cand-search').value   = '';
    document.getElementById('cf-status').value     = '';
    document.getElementById('cf-exp').value        = '';
    currentPage = 1;

    renderCandidates(currentCandidates);

    // Switch view
    document.getElementById('view-jobs').classList.remove('active');
    document.getElementById('view-candidates').classList.add('active');
  }

  function showJobs() {
    document.getElementById('view-candidates').classList.remove('active');
    document.getElementById('view-jobs').classList.add('active');
    document.getElementById('page-title').textContent = 'Applications';
  }

  function renderCandidates(data) {
    const tbody = document.getElementById('cand-tbody');
    const empty = document.getElementById('cand-empty');
    document.getElementById('cand-count-lbl').textContent = data.length;

    if (!data.length) {
      tbody.innerHTML = '';
      empty.style.display = 'block';
      const paginationContainer = document.getElementById('pagination-container');
      if (paginationContainer) paginationContainer.innerHTML = '';
      return;
    }
    empty.style.display = 'none';

    const totalPages = Math.ceil(data.length / itemsPerPage) || 1;
    if (currentPage > totalPages) currentPage = totalPages;
    const startIndex = (currentPage - 1) * itemsPerPage;
    const pageData = data.slice(startIndex, startIndex + itemsPerPage);

    const scoreBar = (s) => `
      <div style="display:flex;align-items:center;gap:6px;">
        <span style="font-size:0.7rem;font-weight:700;color:var(--text-main);width:28px;">${s}</span>
        <div style="flex:1;height:5px;background:#E2E8F0;border-radius:3px;overflow:hidden;min-width:60px;">
          <div style="width:${s}%;height:100%;background:linear-gradient(90deg,var(--primary),#60A5FA);border-radius:3px;"></div>
        </div>
      </div>`;

    tbody.innerHTML = pageData.map(c => `
      <tr onclick="openModal('${c.id}')">
        <td><input type="checkbox" onclick="event.stopPropagation()"/></td>
        <td>
          <div style="display:flex;align-items:center;gap:10px;">
            <div style="width:32px;height:32px;border-radius:50%;background:${c.avatarBg};color:#fff;display:flex;align-items:center;justify-content:center;font-size:0.62rem;font-weight:800;flex-shrink:0;">${c.avatar}</div>
            <div>
              <div class="cand-name">${c.name}</div>
              <div style="font-size:0.62rem;color:var(--text-muted);">${c.email}</div>
            </div>
          </div>
        </td>
        <td>${c.exp}</td>
        <td>${c.loc}</td>
        <td><span class="status-pill ${pillClass(c.status)}">${c.status}</span></td>
        <td style="min-width:100px;">${scoreBar(c.resumeScore)}</td>
        <td style="white-space:nowrap;font-size:0.7rem;">${c.applied}</td>
        <td>
          <div style="display:flex;gap:8px;align-items:center;">
            <button title="View Profile" style="background:none;border:none;cursor:pointer;color:var(--primary);font-size:0.82rem;padding:0;" onclick="event.stopPropagation(); openModal('${c.id}')"><i class="fa-regular fa-eye"></i></button>
            <button title="More" style="background:none;border:none;cursor:pointer;color:var(--text-muted);font-size:0.82rem;" onclick="event.stopPropagation()"><i class="fa-solid fa-ellipsis-vertical"></i></button>
          </div>
        </td>
      </tr>`).join('');
      
    renderPagination(totalPages);
  }

  window.renderPagination = function(totalPages) {
    const container = document.getElementById('pagination-container');
    if (!container) return;
    if (totalPages <= 1) {
      container.innerHTML = '';
      return;
    }

    let html = `<button ${currentPage === 1 ? 'disabled style="opacity:0.5;cursor:not-allowed;"' : `onclick="goToPage(${currentPage - 1})"`}>&larr; Prev</button>`;
    for (let i = 1; i <= totalPages; i++) {
      if (i === currentPage) {
        html += `<button class="pg-active">${i}</button>`;
      } else {
        html += `<button onclick="goToPage(${i})">${i}</button>`;
      }
    }
    html += `<button ${currentPage === totalPages ? 'disabled style="opacity:0.5;cursor:not-allowed;"' : `onclick="goToPage(${currentPage + 1})"`}>Next &rarr;</button>`;
    
    container.innerHTML = html;
  };

  window.goToPage = function(page) {
    currentPage = page;
    filterCandidates(false);
  };

  function filterCandidates(resetPage = true) {
    if (resetPage) currentPage = 1;
    const q   = document.getElementById('cand-search').value.toLowerCase();
    const st  = document.getElementById('cf-status').value;
    const exp = document.getElementById('cf-exp').value;
    const data = currentCandidates.filter(c =>
      (!q  || c.name.toLowerCase().includes(q) || c.email.toLowerCase().includes(q) || c.loc.toLowerCase().includes(q) || c.skills.some(s => s.toLowerCase().includes(q))) &&
      (!st  || c.status === st)
    );
    renderCandidates(data);
  }

  function resetCandFilter() {
    document.getElementById('cand-search').value = '';
    document.getElementById('cf-status').value   = '';
    document.getElementById('cf-exp').value      = '';
    currentPage = 1;
    renderCandidates(currentCandidates);
  }

  function toggleAll(el) {
    document.querySelectorAll('#cand-tbody input[type=checkbox]').forEach(c => c.checked = el.checked);
  }

  // ════════════════════════════════════════════════════════════════════
  //  EXPORT FORMAT MODAL
  // ════════════════════════════════════════════════════════════════════
  let selectedFormat = 'pdf';

  function openExportModal() {
    if (!currentCandidates || currentCandidates.length === 0) {
      alert('No candidates to export. Please open a job first.');
      return;
    }
    selectedFormat = 'pdf';
    selectFormat('pdf');
    document.getElementById('export-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  }

  function closeExportModal() {
    document.getElementById('export-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  }

  function handleExpBackdrop(e) {
    if (e.target === document.getElementById('export-backdrop')) closeExportModal();
  }

  function selectFormat(fmt) {
    selectedFormat = fmt;
    ['pdf','docx','xlsx'].forEach(f => {
      const el = document.getElementById('opt-' + f);
      el.classList.toggle('selected', f === fmt);
    });
  }

  function doDownload() {
    const job = jobsData.find(j => j.id === currentJobId);
    const jobTitle = job ? job.title.replace(/[^a-zA-Z0-9]/g, '_') : 'Candidates';
    const timestamp = new Date().toISOString().slice(0,10);
    const filename = `${jobTitle}_Candidates_${timestamp}`;

    if (selectedFormat === 'pdf') {
      exportAsPDF(job, filename);
    } else if (selectedFormat === 'docx') {
      exportAsWord(job, filename);
    } else if (selectedFormat === 'xlsx') {
      exportAsCSV(job, filename);
    }
    closeExportModal();
  }

  /* ── PDF: generate and download real PDF ─────────────────────────── */
  function exportAsPDF(job, filename) {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF({ orientation: 'landscape', unit: 'mm', format: 'a4' });

    // Title
    doc.setFontSize(16);
    doc.setTextColor(10, 75, 210);
    doc.text((job ? job.title : 'Candidates') + ' — Applicants Report', 14, 18);

    // Subtitle
    doc.setFontSize(9);
    doc.setTextColor(100, 116, 139);
    doc.text('Department: ' + (job ? job.dept : '—') + '   |   Location: ' + (job ? job.loc : '—') + '   |   Generated: ' + new Date().toLocaleString(), 14, 25);

    // Table
    const head = [['Name','Email','Phone','Location','Exp','Status','Resume','ATS','Skills','Applied On']];
    const body = currentCandidates.map(c => [
      c.name, c.email, c.phone, c.loc, c.exp, c.status,
      c.resumeScore + '/100', String(c.atsScore),
      [...c.skills, ...c.skills2].join(', '),
      c.applied
    ]);

    doc.autoTable({
      head: head,
      body: body,
      startY: 30,
      styles: { fontSize: 7.5, cellPadding: 2.5, overflow: 'linebreak', font: 'helvetica' },
      headStyles: { fillColor: [10, 75, 210], textColor: 255, fontStyle: 'bold', fontSize: 7.5 },
      alternateRowStyles: { fillColor: [248, 250, 252] },
      columnStyles: {
        0: { cellWidth: 28 },
        1: { cellWidth: 40 },
        2: { cellWidth: 28 },
        8: { cellWidth: 45 },
      },
      margin: { left: 14, right: 14 },
    });

    // Footer
    const pageCount = doc.internal.getNumberOfPages();
    for (let i = 1; i <= pageCount; i++) {
      doc.setPage(i);
      doc.setFontSize(7);
      doc.setTextColor(150);
      doc.text('ProPath Admin — Page ' + i + ' of ' + pageCount, 14, doc.internal.pageSize.height - 8);
    }

    doc.save(filename + '.pdf');
  }

  /* ── Word (.doc via HTML) ─────────────────────────────────────────── */
  function exportAsWord(job, filename) {
    const rows = currentCandidates.map(c => `
      <tr>
        <td>${c.name}</td><td>${c.email}</td><td>${c.phone}</td>
        <td>${c.loc}</td><td>${c.exp}</td><td>${c.status}</td>
        <td>${c.resumeScore}/100</td><td>${c.atsScore}</td>
        <td>${[...c.skills,...c.skills2].join(', ')}</td><td>${c.applied}</td>
      </tr>`).join('');

    const html = `<html xmlns:o="urn:schemas-microsoft-com:office:office"
      xmlns:w="urn:schemas-microsoft-com:office:word"
      xmlns="http://www.w3.org/TR/REC-html40"><head>
      <meta charset="utf-8"/>
      <title>${filename}</title>
      <link href="applications.css" rel="stylesheet"/></head><body>
      <h2>${job ? job.title : 'Candidates'} — Applicants Report</h2>
      <p><b>Department:</b> ${job ? job.dept : '—'} &nbsp; <b>Location:</b> ${job ? job.loc : '—'} &nbsp; <b>Generated:</b> ${new Date().toLocaleString()}</p>
      <table>
        <thead><tr>
          <th>Name</th><th>Email</th><th>Phone</th><th>Location</th>
          <th>Experience</th><th>Status</th><th>Resume</th><th>ATS</th>
          <th>Skills</th><th>Applied On</th>
        </tr></thead>
        <tbody>${rows}</tbody>
      </table></body></html>`;

    triggerDownload(new Blob([html], { type: 'application/msword' }), filename + '.doc');
  }

  /* ── Excel (CSV) ─────────────────────────────────────────────────── */
  function exportAsCSV(job, filename) {
    const header = ['Name','Email','Phone','Location','Experience','Status','Resume Score','ATS Score','Skills','Current Job','Applied On'];
    const rows = currentCandidates.map(c => [
      c.name, c.email, c.phone, c.loc, c.exp, c.status,
      c.resumeScore + '/100', c.atsScore,
      [...c.skills, ...c.skills2].join(' | '),
      c.currentJob.replace('\n',' '),
      c.applied
    ].map(v => `"${String(v).replace(/"/g,'""')}"`).join(','));

    const csv = [header.join(','), ...rows].join('\r\n');
    const bom = '\uFEFF'; // UTF-8 BOM for Excel
    triggerDownload(new Blob([bom + csv], { type: 'text/csv;charset=utf-8;' }), filename + '.csv');
  }

  /* ── Helper: trigger browser download ────────────────────────────── */
  function triggerDownload(blob, fname) {
    const url = URL.createObjectURL(blob);
    const a   = document.createElement('a');
    a.href     = url;
    a.download = fname;
    document.body.appendChild(a);
    a.click();
    setTimeout(() => { URL.revokeObjectURL(url); a.remove(); }, 1000);
  }

  // ════════════════════════════════════════════════════════════════════
  //  MODAL
  // ════════════════════════════════════════════════════════════════════
  function openModal(candId) {
    // Search across all jobs
    let c = null;
    for (const arr of Object.values(applicantsData)) {
      c = arr.find(x => x.id === candId);
      if (c) break;
    }
    if (!c) return;

    document.getElementById('cm-hdr-name').textContent = c.name;

    const bar = p => `<div class="cm-bar"><div class="cm-bar-fill" style="width:${p}%"></div></div>`;
    const allSkills = [...c.skills, ...c.skills2].map(s => `<span class="cm-skill">${s}</span>`).join('');

    document.getElementById('cm-body').innerHTML = `
      <div style="display:flex;align-items:center;gap:12px;margin-bottom:12px;">
        <div class="cm-avatar" style="background:${c.avatarBg};">${c.avatar}</div>
        <div>
          <div class="cm-name">${c.name}</div>
          <div class="cm-role">${c.exp} experience &nbsp;·&nbsp; ${c.loc}</div>
        </div>
      </div>
      <div class="cm-info">
        <span><i class="fa-regular fa-envelope"></i>${c.email}</span>
        <span><i class="fa-solid fa-phone"></i>${c.phone}</span>
        <span><i class="fa-solid fa-location-dot"></i>${c.loc}, India</span>
      </div>
      <div class="cm-status-row">
        <span class="status-pill ${pillClass(c.status)}">${c.status}</span>
        <span class="cm-applied">Applied on <b>${c.applied}</b></span>
      </div>
      <div class="cm-actions">
        <button class="cm-btn-prim" onclick="openProfileModal('${c.id}')">View Profile</button>
        <button class="cm-btn-sec"><i class="fa-solid fa-download me-1"></i>Download Resume</button>
      </div>
      <hr class="cm-divider"/>
      <div class="cm-label">Resume Score</div>
      <div class="cm-bar-wrap">
        <span class="cm-score-num">${c.resumeScore}/100</span>${bar(c.resumeScore)}
        <span style="font-size:0.65rem;color:var(--text-muted);">+</span>
      </div>
      <div class="cm-label">ATS Score</div>
      <div class="cm-bar-wrap">
        <span class="cm-score-num">${c.atsScore}</span>${bar(c.atsScore)}
        <span style="font-size:0.65rem;color:var(--text-muted);">+</span>
      </div>
      <hr class="cm-divider"/>
      <div class="cm-label">Skills Match</div>
      <div class="cm-skills">${allSkills}</div>
      <div class="cm-label">Current Job</div>
      <div class="cm-jobs-val">${c.currentJob.replace('\n','<br/>')}</div>
      <hr class="cm-divider"/>
      <div class="cm-label">Notes</div>
      <div class="cm-notes"><textarea placeholder="Add your notes here..."></textarea></div>
    `;

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

  window.openProfileModal = function(candId) {
    let c = null;
    for (const arr of Object.values(applicantsData)) {
      c = arr.find(x => x.id === candId);
      if (c) break;
    }
    if (!c) return;

    closeModal();

    const profileBody = document.getElementById('profile-modal-body');
    profileBody.innerHTML = `
      <div class="card mb-4" style="border-radius: 12px; border: 1px solid #E2E8F0; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05);">
        <div class="card-header bg-white border-bottom-0 pt-4 pb-0 px-4 d-flex justify-content-between align-items-center" style="border-radius: 12px 12px 0 0;">
          <div>
            <h4 class="fw-bold mb-1" style="color: #1E293B;">Basic info</h4>
            <p class="text-muted mb-0" style="font-size: 0.9rem;">Some info may be visible to other people using ProPath services.</p>
          </div>
        </div>
        <div class="card-body p-0 mt-3">
          <ul class="list-group list-group-flush" style="border-radius: 0 0 12px 12px;">
            <li class="list-group-item px-4 py-3 d-flex justify-content-between align-items-center" style="border-color: #E2E8F0;">
              <span class="text-muted fw-bold" style="font-size: 0.75rem; letter-spacing: 0.5px; width: 120px;">PHOTO</span>
              <div class="d-flex align-items-center justify-content-between w-100">
                <div style="width: 45px; height: 45px; border-radius: 50%; background: ${c.avatarBg}; color: white; display: flex; align-items: center; justify-content: center; font-weight: bold; font-size: 1.2rem;">${c.avatar}</div>
                <i class="fa-solid fa-chevron-right text-muted"></i>
              </div>
            </li>
            <li class="list-group-item px-4 py-3 d-flex justify-content-between align-items-center" style="border-color: #E2E8F0;">
              <span class="text-muted fw-bold" style="font-size: 0.75rem; letter-spacing: 0.5px; width: 120px;">NAME</span>
              <div class="d-flex align-items-center justify-content-between w-100">
                <span class="fw-medium text-dark">${c.name}</span>
                <i class="fa-solid fa-chevron-right text-muted"></i>
              </div>
            </li>
            <li class="list-group-item px-4 py-3 d-flex justify-content-between align-items-center" style="border-color: #E2E8F0;">
              <span class="text-muted fw-bold" style="font-size: 0.75rem; letter-spacing: 0.5px; width: 120px;">BIRTHDAY</span>
              <div class="d-flex align-items-center justify-content-between w-100">
                <span class="fw-medium text-dark">15 May 1995</span>
                <i class="fa-solid fa-chevron-right text-muted"></i>
              </div>
            </li>
            <li class="list-group-item px-4 py-3 d-flex justify-content-between align-items-center border-bottom-0">
              <span class="text-muted fw-bold" style="font-size: 0.75rem; letter-spacing: 0.5px; width: 120px;">GENDER</span>
              <div class="d-flex align-items-center justify-content-between w-100">
                <span class="fw-medium text-dark">${c.name.includes('Harini') || c.name.includes('Divya') || c.name.includes('Priya') || c.name.includes('Sneha') || c.name.includes('Meena') || c.name.includes('Ananya') || c.name.includes('Chitra') || c.name.includes('Pooja') || c.name.includes('Lakshmi') || c.name.includes('Nandini') ? 'Female' : 'Male'}</span>
                <i class="fa-solid fa-chevron-right text-muted"></i>
              </div>
            </li>
          </ul>
        </div>
      </div>

      <div class="card" style="border-radius: 12px; border: 1px solid #E2E8F0; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05);">
        <div class="card-header bg-white border-bottom-0 pt-4 pb-0 px-4" style="border-radius: 12px 12px 0 0;">
          <h4 class="fw-bold mb-1" style="color: #1E293B;">Contact info</h4>
          <p class="text-muted mb-0" style="font-size: 0.9rem;">Contact addresses connected to your Seeker account.</p>
        </div>
        <div class="card-body p-0 mt-3">
          <ul class="list-group list-group-flush" style="border-radius: 0 0 12px 12px;">
            <li class="list-group-item px-4 py-3 d-flex justify-content-between align-items-center" style="border-color: #E2E8F0;">
              <span class="text-muted fw-bold" style="font-size: 0.75rem; letter-spacing: 0.5px; width: 120px;">EMAIL</span>
              <div class="d-flex align-items-center justify-content-between w-100">
                <span class="fw-medium text-dark">${c.email}</span>
                <i class="fa-solid fa-chevron-right text-muted"></i>
              </div>
            </li>
            <li class="list-group-item px-4 py-3 d-flex justify-content-between align-items-center border-bottom-0">
              <span class="text-muted fw-bold" style="font-size: 0.75rem; letter-spacing: 0.5px; width: 120px;">PHONE</span>
              <div class="d-flex align-items-center justify-content-between w-100">
                <span class="fw-medium text-dark">${c.phone}</span>
                <i class="fa-solid fa-chevron-right text-muted"></i>
              </div>
            </li>
          </ul>
        </div>
      </div>
    `;
    
    document.getElementById('profile-modal-backdrop').classList.add('open');
    document.body.style.overflow = 'hidden';
  };

  window.closeProfileModal = function() {
    document.getElementById('profile-modal-backdrop').classList.remove('open');
    document.body.style.overflow = '';
  };

  window.handleProfileBackdropClick = function(e) {
    if (e.target === document.getElementById('profile-modal-backdrop')) {
      closeProfileModal();
    }
  };

  document.addEventListener('keydown', e => { 
    if (e.key === 'Escape') {
      closeModal(); 
      closeProfileModal();
    }
  });

  function updateStatsMetrics() {
    let totalApps = 0;
    let shortlisted = 0;
    let interview = 0;
    let hired = 0;

    Object.values(applicantsData).forEach(arr => {
      totalApps += arr.length;
      arr.forEach(c => {
        const s = (c.status || '').toLowerCase();
        if (s === 'shortlisted') shortlisted++;
        else if (s === 'interview') interview++;
        else if (s === 'hired') hired++;
      });
    });

    const el = id => document.getElementById(id);
    if (el('app-metric-total')) el('app-metric-total').textContent = totalApps.toLocaleString();
    if (el('app-metric-shortlisted')) el('app-metric-shortlisted').textContent = shortlisted.toLocaleString();
    if (el('app-metric-interview')) el('app-metric-interview').textContent = interview.toLocaleString();
    if (el('app-metric-hired')) el('app-metric-hired').textContent = hired.toLocaleString();
  }

  // ════════════════════════════════════════════════════════════════════
  //  INIT
  // ════════════════════════════════════════════════════════════════════
  renderJobCards(jobsData);
  updateStatsMetrics();