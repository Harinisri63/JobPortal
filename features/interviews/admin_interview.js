// admin_interview.js – Handles job list, candidate view, and interview scheduling

const jobsData = [
  { id:'JOB-1021', title:'Senior DevOps Engineer',  dept:'Engineering', loc:'Bangalore', type:'Full Time', exp:'5-8 years',  status:'Active',  posted:'12 May 2025' },
  { id:'JOB-1015', title:'React Developer',          dept:'Engineering', loc:'Remote',    type:'Full Time', exp:'2-4 years',  status:'Active',  posted:'11 May 2025' },
  { id:'JOB-1008', title:'Product Manager',          dept:'Product',     loc:'Bangalore', type:'Full Time', exp:'3-6 years',  status:'Active',  posted:'10 May 2025' },
  { id:'JOB-1002', title:'UI/UX Designer',           dept:'Design',      loc:'Pune',      type:'Full Time', exp:'2-4 years',  status:'Active',  posted:'09 May 2025' },
  { id:'JOB-0991', title:'Data Analyst',             dept:'Analytics',   loc:'Hyderabad', type:'Full Time', exp:'1-3 years',  status:'Active',  posted:'08 May 2025' },
  { id:'JOB-0982', title:'Backend Developer',        dept:'Engineering', loc:'Remote',    type:'Full Time', exp:'3-5 years',  status:'Closed',  posted:'07 May 2025' },
  { id:'JOB-0975', title:'HR Executive',             dept:'HR',          loc:'Mumbai',    type:'Full Time', exp:'1-2 years',  status:'Active',  posted:'07 May 2025' },
  { id:'JOB-0961', title:'Full Stack Developer',     dept:'Engineering', loc:'Bangalore', type:'Full Time', exp:'4-7 years',  status:'Active',  posted:'06 May 2025' },
];

const applicantsData = {
  'JOB-1021': [
    { id:'CND-10234', name:'Arjun Mehta',     email:'arjun.mehta@gmail.com',   phone:'+91 9876543210', loc:'Bangalore', exp:'6 years',   intStatus:'Scheduled',  applied:'18 May 2025', resumeScore:85, avatar:'AM', avatarBg:'#6366F1', skills:['AWS','Docker','Kubernetes'] },
    { id:'CND-10235', name:'Priya Sharma',    email:'priya.sharma@gmail.com',  phone:'+91 9845012345', loc:'Remote',    exp:'7 years',   intStatus:'Eligible',   applied:'19 May 2025', resumeScore:90, avatar:'PS', avatarBg:'#8B5CF6', skills:['Terraform','GCP','Python'] },
    { id:'CND-10236', name:'Rohit Nair',      email:'rohit.nair@gmail.com',    phone:'+91 9823456780', loc:'Pune',      exp:'5 years',   intStatus:'Rejected',   applied:'20 May 2025', resumeScore:62, avatar:'RN', avatarBg:'#EF4444', skills:['AWS','Linux'] },
    { id:'CND-10237', name:'Sneha Iyer',      email:'sneha.iyer@gmail.com',    phone:'+91 9900112233', loc:'Hyderabad', exp:'8 years',   intStatus:'Completed',  applied:'20 May 2025', resumeScore:94, avatar:'SI', avatarBg:'#10B981', skills:['Kubernetes','Docker'] },
    { id:'CND-10238', name:'Karan Verma',     email:'karan.verma@gmail.com',   phone:'+91 9711223344', loc:'Bangalore', exp:'6 years',   intStatus:'Eligible',   applied:'21 May 2025', resumeScore:78, avatar:'KV', avatarBg:'#F59E0B', skills:['CI/CD','Jenkins'] },
  ],
  'JOB-1015': [
    { id:'CND-10240', name:'Divya Reddy',     email:'divya.reddy@gmail.com',   phone:'+91 9654321098', loc:'Remote',    exp:'3 years',   intStatus:'Eligible',   applied:'17 May 2025', resumeScore:80, avatar:'DR', avatarBg:'#EC4899', skills:['React','TypeScript'] },
    { id:'CND-10241', name:'Aakash Singh',    email:'aakash.singh@gmail.com',  phone:'+91 9987654321', loc:'Mumbai',    exp:'2.5 years', intStatus:'Scheduled',  applied:'18 May 2025', resumeScore:70, avatar:'AS', avatarBg:'#0EA5E9', skills:['React','CSS'] },
    { id:'CND-10242', name:'Meena Pillai',    email:'meena.pillai@gmail.com',  phone:'+91 9765432109', loc:'Chennai',   exp:'4 years',   intStatus:'Completed',  applied:'18 May 2025', resumeScore:88, avatar:'MP', avatarBg:'#14B8A6', skills:['Next.js','Node.js'] },
  ],
  'JOB-1008': [
    { id:'CND-10244', name:'Vikram Joshi',    email:'vikram.joshi@gmail.com',  phone:'+91 9811223344', loc:'Bangalore', exp:'5 years',   intStatus:'Eligible',   applied:'14 May 2025', resumeScore:82, avatar:'VJ', avatarBg:'#6366F1', skills:['Agile','Jira'] },
    { id:'CND-10245', name:'Ananya Das',      email:'ananya.das@gmail.com',    phone:'+91 9922334455', loc:'Delhi',     exp:'4 years',   intStatus:'Scheduled',  applied:'15 May 2025', resumeScore:74, avatar:'AD', avatarBg:'#8B5CF6', skills:['OKRs','Analytics'] },
    { id:'CND-10246', name:'Suresh Kumar',    email:'suresh.kumar@gmail.com',  phone:'+91 9633445566', loc:'Pune',      exp:'6 years',   intStatus:'Eligible',   applied:'15 May 2025', resumeScore:86, avatar:'SK', avatarBg:'#F59E0B', skills:['SCRUM','PRD'] },
  ],
  'JOB-1002': [
    { id:'CND-10248', name:'Harini Sri',      email:'harini.sri@gmail.com',    phone:'+91 9876543210', loc:'Chennai',   exp:'3 years',   intStatus:'Eligible',   applied:'12 May 2025', resumeScore:88, avatar:'HS', avatarBg:'#EC4899', skills:['Figma','Adobe XD'] },
    { id:'CND-10249', name:'Rajan Pillai',    email:'rajan.pillai@gmail.com',  phone:'+91 9765001122', loc:'Pune',      exp:'2 years',   intStatus:'Eligible',   applied:'13 May 2025', resumeScore:72, avatar:'RP', avatarBg:'#0EA5E9', skills:['Figma','CSS'] },
  ],
  'JOB-0991': [
    { id:'CND-10251', name:'Chitra Nair',     email:'chitra.nair@gmail.com',   phone:'+91 9633221100', loc:'Hyderabad', exp:'2 years',   intStatus:'Eligible',   applied:'11 May 2025', resumeScore:68, avatar:'CN', avatarBg:'#10B981', skills:['Python','SQL'] },
    { id:'CND-10252', name:'Hari Prasad',     email:'hari.prasad@gmail.com',   phone:'+91 9988776655', loc:'Remote',    exp:'1.5 years', intStatus:'Eligible',   applied:'12 May 2025', resumeScore:75, avatar:'HP', avatarBg:'#6366F1', skills:['Tableau','R'] },
  ],
  'JOB-0982': [
    { id:'CND-10254', name:'Nikhil Tiwari',   email:'nikhil.tiwari@gmail.com', phone:'+91 9871234560', loc:'Remote',    exp:'4 years',   intStatus:'Completed',  applied:'09 May 2025', resumeScore:82, avatar:'NT', avatarBg:'#8B5CF6', skills:['Java','Spring Boot'] },
    { id:'CND-10255', name:'Pooja Saxena',    email:'pooja.saxena@gmail.com',  phone:'+91 9855443322', loc:'Mumbai',    exp:'3 years',   intStatus:'Rejected',   applied:'10 May 2025', resumeScore:58, avatar:'PX', avatarBg:'#EF4444', skills:['Node.js','MongoDB'] },
  ],
  'JOB-0975': [
    { id:'CND-10257', name:'Lakshmi Rao',     email:'lakshmi.rao@gmail.com',   phone:'+91 9700123456', loc:'Mumbai',    exp:'1 year',    intStatus:'Eligible',   applied:'09 May 2025', resumeScore:65, avatar:'LR', avatarBg:'#F59E0B', skills:['Recruitment','HRMS'] },
  ],
  'JOB-0961': [
    { id:'CND-10259', name:'Siddharth Gupta', email:'siddharth.g@gmail.com',   phone:'+91 9876001234', loc:'Bangalore', exp:'5 years',   intStatus:'Scheduled',  applied:'08 May 2025', resumeScore:83, avatar:'SG', avatarBg:'#6366F1', skills:['React','Node.js'] },
    { id:'CND-10260', name:'Nandini Kulkarni',email:'nandini.k@gmail.com',     phone:'+91 9645678901', loc:'Pune',      exp:'4 years',   intStatus:'Eligible',   applied:'09 May 2025', resumeScore:79, avatar:'NK', avatarBg:'#EC4899', skills:['Angular','Java'] },
    { id:'CND-10261', name:'Abhinav Sharma',  email:'abhinav.s@gmail.com',     phone:'+91 9811009988', loc:'Remote',    exp:'6 years',   intStatus:'Eligible',   applied:'09 May 2025', resumeScore:86, avatar:'AB', avatarBg:'#14B8A6', skills:['Django','PostgreSQL'] },
  ],
};

let currentJobId = null;
let currentCandidates = [];
let currentCandidate = null;
let selectedInterviewType = 'Technical';
let selectedInterviewMode = 'Online';

let currentPage = 1;
const itemsPerPage = 8;

// ════════════════════════════════════════════════════════════
//  FRAME 1 — RENDER JOB CARDS
// ════════════════════════════════════════════════════════════
function getIntStatusCounts(appsArr) {
  return {
    eligible:  appsArr.filter(a => a.intStatus === 'Eligible').length,
    scheduled: appsArr.filter(a => a.intStatus === 'Scheduled').length,
    completed: appsArr.filter(a => a.intStatus === 'Completed').length,
    rejected:  appsArr.filter(a => a.intStatus === 'Rejected').length,
  };
}

function renderJobCards(data) {
  const grid = document.getElementById('jobs-grid');
  if (!grid) return;
  if (!data.length) {
    grid.innerHTML = `<div style="grid-column:1/-1;text-align:center;padding:60px 20px;color:#CBD5E1;font-size:.8rem;">No jobs found.</div>`;
    return;
  }
  grid.innerHTML = data.map(j => {
    const arr = applicantsData[j.id] || [];
    const total = arr.length;
    const c = getIntStatusCounts(arr);
    const statusCls = j.status === 'Active' ? 'badge-active' : 'badge-closed';
    return `
      <div class="job-card">
        <div class="jc-top">
          <div>
            <div class="jc-title">${j.title}</div>
            <div class="jc-dept">${j.dept} &middot; ${j.loc}</div>
          </div>
          <span class="jc-badge ${statusCls}">${j.status}</span>
        </div>
        <div class="jc-meta">
          <span><i class="fa-solid fa-briefcase"></i>${j.type}</span>
          <span><i class="fa-solid fa-clock"></i>${j.exp}</span>
          <span><i class="fa-regular fa-calendar"></i>${j.posted}</span>
        </div>
        <div class="jc-count-box">
          <div class="jc-count-num">${total}</div>
          <div>
            <div class="jc-count-sub"><b>Total Applicants</b>eligible for interview</div>
          </div>
        </div>
        <div class="jc-breakdown">
          <div class="bk bk-eligible"><div class="bn">${c.eligible}</div><div class="bl">Eligible</div></div>
          <div class="bk bk-scheduled"><div class="bn">${c.scheduled}</div><div class="bl">Scheduled</div></div>
          <div class="bk bk-done"><div class="bn">${c.completed}</div><div class="bl">Done</div></div>
          <div class="bk bk-rejected"><div class="bn">${c.rejected}</div><div class="bl">Rejected</div></div>
        </div>
        <div class="jc-footer">
          <span class="jc-posted">ID: ${j.id} &nbsp;|&nbsp; ${j.posted}</span>
          <button class="btn-view-cands" onclick="viewCandidates('${j.id}')">
            <i class="fa-solid fa-users"></i> View Candidates
          </button>
        </div>
      </div>`;
  }).join('');
}

function filterJobCards() {
  const q  = document.getElementById('job-search').value.toLowerCase();
  const st = document.getElementById('job-status-filter').value;
  renderJobCards(jobsData.filter(j =>
    (!q  || j.title.toLowerCase().includes(q) || j.dept.toLowerCase().includes(q)) &&
    (!st || j.status === st)
  ));
}

// ════════════════════════════════════════════════════════════
//  FRAME 2 — CANDIDATES TABLE
// ════════════════════════════════════════════════════════════
function viewCandidates(jobId) {
  currentJobId = jobId;
  const job = jobsData.find(j => j.id === jobId);
  currentCandidates = applicantsData[jobId] || [];

  document.getElementById('current-job-title').textContent = job ? job.title : jobId;
  document.getElementById('banner-title').textContent = job ? job.title : jobId;
  document.getElementById('banner-count').textContent = currentCandidates.length;
  if (job) {
    document.getElementById('banner-meta').innerHTML = `
      <span><i class="fa-solid fa-building"></i>${job.dept}</span>
      <span><i class="fa-solid fa-location-dot"></i>${job.loc}</span>
      <span><i class="fa-solid fa-briefcase"></i>${job.type}</span>
      <span><i class="fa-solid fa-clock"></i>${job.exp}</span>`;
  }
  document.getElementById('cand-search').value = '';
  document.getElementById('cf-status').value = '';
  currentPage = 1;
  renderCandidates(currentCandidates);

  document.getElementById('view-jobs').classList.remove('active');
  document.getElementById('view-candidates').classList.add('active');
  document.getElementById('page-title').textContent = 'Interviews';
}

function showJobs() {
  document.getElementById('view-candidates').classList.remove('active');
  document.getElementById('view-jobs').classList.add('active');
}

function pillClass(s) {
  if (s === 'Scheduled') return 'pill-scheduled';
  if (s === 'Completed') return 'pill-completed';
  if (s === 'Rejected')  return 'pill-rejected';
  return 'pill-eligible';
}

function renderCandidates(data) {
  const tbody = document.getElementById('cand-tbody');
  const empty = document.getElementById('cand-empty');
  if (!tbody) return;

  const totalPages = Math.ceil(data.length / itemsPerPage);
  if (currentPage > totalPages && totalPages > 0) currentPage = totalPages;

  const startIndex = (currentPage - 1) * itemsPerPage;
  const pageData = data.slice(startIndex, startIndex + itemsPerPage);

  if (!pageData.length) {
    tbody.innerHTML = '';
    empty.style.display = 'block';
    renderPagination(0);
    return;
  }
  empty.style.display = 'none';

  tbody.innerHTML = pageData.map(c => {
    let statusHTML = `<span class="status-pill ${pillClass(c.intStatus)}">${c.intStatus}</span>`;
    if (c.intStatus === 'Scheduled' && c.interviewDate && c.interviewTime) {
      statusHTML += `<div style="font-size:.65rem;font-weight:600;color:var(--text-muted);margin-top:4px;">
        <i class="fa-regular fa-calendar-days" style="color:var(--primary);margin-right:2px;"></i> ${formatDate(c.interviewDate)}<br/>
        <i class="fa-regular fa-clock" style="color:var(--primary);margin-right:2px;"></i> ${formatTime(c.interviewTime)}
      </div>`;
    }
    return `
      <tr onclick="openScheduleModal('${c.id}')">
        <td onclick="event.stopPropagation()"><input type="checkbox"/></td>
        <td>
          <div style="display:flex;align-items:center;gap:10px;">
            <div style="width:32px;height:32px;border-radius:50%;background:${c.avatarBg};color:#fff;display:flex;align-items:center;justify-content:center;font-size:.6rem;font-weight:800;flex-shrink:0;">${c.avatar}</div>
            <div>
              <div class="cand-name">${c.name}</div>
              <div style="font-size:.62rem;color:var(--text-muted);">${c.email}</div>
            </div>
          </div>
        </td>
        <td>${c.exp}</td>
        <td>${c.loc}</td>
        <td>${statusHTML}</td>
        <td>
          <div class="score-bar-wrap">
            <span style="font-size:.7rem;font-weight:700;width:28px;">${c.resumeScore}</span>
            <div class="score-bar"><div class="score-bar-fill" style="width:${c.resumeScore}%"></div></div>
          </div>
        </td>
        <td style="white-space:nowrap;font-size:.7rem;">${c.applied}</td>
        <td onclick="event.stopPropagation()">
          <div style="display: flex; gap: 6px; align-items: center; white-space: nowrap;">
            <button onclick="openScheduleModal('${c.id}')" style="background:var(--primary);border:none;border-radius:6px;padding:6px 12px;font-size:.68rem;font-weight:700;color:#fff;cursor:pointer;font-family:'Poppins',sans-serif;white-space:nowrap;display:inline-flex;align-items:center;gap:4px;">
              <i class="fa-regular fa-calendar-plus"></i> ${c.intStatus === 'Scheduled' ? 'Reschedule' : 'Schedule'}
            </button>
          </div>
        </td>
      </tr>`;
  }).join('');
  renderPagination(totalPages);
}

function filterCands(resetPage = true) {
  if (resetPage) currentPage = 1;
  const q  = document.getElementById('cand-search').value.toLowerCase();
  const st = document.getElementById('cf-status').value;
  renderCandidates(currentCandidates.filter(c =>
    (!q  || c.name.toLowerCase().includes(q) || c.email.toLowerCase().includes(q) || c.loc.toLowerCase().includes(q)) &&
    (!st || c.intStatus === st)
  ));
}

function resetCandFilters() {
  document.getElementById('cand-search').value = '';
  document.getElementById('cf-status').value = '';
  currentPage = 1;
  renderCandidates(currentCandidates);
}

function renderPagination(totalPages) {
  const container = document.getElementById('pagination-container');
  if (!container) return;
  if (totalPages <= 1) {
    container.innerHTML = '';
    return;
  }
  
  let html = '';
  html += `<button ${currentPage === 1 ? 'disabled' : ''} onclick="goToPage(${currentPage - 1})">← Prev</button>`;
  
  for (let i = 1; i <= totalPages; i++) {
    if (i === currentPage) {
      html += `<button class="pg-active">${i}</button>`;
    } else {
      html += `<button onclick="goToPage(${i})">${i}</button>`;
    }
  }
  
  html += `<button ${currentPage === totalPages ? 'disabled' : ''} onclick="goToPage(${currentPage + 1})">Next →</button>`;
  container.innerHTML = html;
}

window.goToPage = function(page) {
  currentPage = page;
  filterCands(false);
};

function toggleAll(el) {
  document.querySelectorAll('#cand-tbody input[type=checkbox]').forEach(c => c.checked = el.checked);
}

// ════════════════════════════════════════════════════════════
//  MODAL — SCHEDULE INTERVIEW
// ════════════════════════════════════════════════════════════
function openScheduleModal(candId) {
  let c = null;
  for (const arr of Object.values(applicantsData)) {
    c = arr.find(x => x.id === candId);
    if (c) break;
  }
  if (!c) return;
  currentCandidate = c;

  document.getElementById('sm-avatar').textContent = c.avatar;
  document.getElementById('sm-avatar').style.background = c.avatarBg;
  document.getElementById('sm-cand-name').textContent = c.name;
  const job = jobsData.find(j => j.id === currentJobId);
  document.getElementById('sm-cand-role').textContent = (job ? job.title : '') + ' — ' + c.exp + ' Experience';
  document.getElementById('sm-email').textContent = c.email;
  document.getElementById('sm-phone').textContent = c.phone;
  document.getElementById('sm-loc').textContent = c.loc + ', India';

  document.getElementById('sm-date').value = '';
  document.getElementById('sm-time').value = '';
  document.getElementById('sm-interviewer').value = '';
  document.getElementById('sm-panel').value = '';
  document.getElementById('sm-link').value = '';
  document.getElementById('sm-venue').value = '';
  document.getElementById('sm-notes').value = '';
  document.getElementById('sm-duration').selectedIndex = 1;
  document.getElementById('sm-round').selectedIndex = 0;

  selectedInterviewType = 'Technical';
  selectedInterviewMode = 'Online';
  document.querySelectorAll('.type-pill').forEach(p => p.classList.toggle('selected', p.dataset.val === 'Technical'));
  document.querySelectorAll('.mode-btn').forEach(b => b.classList.toggle('selected', b.dataset.val === 'Online'));
  document.getElementById('link-field').style.display = '';
  document.getElementById('venue-field').style.display = 'none';

  document.getElementById('sch-backdrop').classList.add('open');
  document.body.style.overflow = 'hidden';
}

function closeModal() {
  document.getElementById('sch-backdrop').classList.remove('open');
  document.body.style.overflow = '';
}

function handleBackdropClick(e) {
  if (e.target === document.getElementById('sch-backdrop')) closeModal();
}

document.addEventListener('keydown', e => { if (e.key === 'Escape') closeModal(); });

function selectType(el) {
  document.querySelectorAll('.type-pill').forEach(p => p.classList.remove('selected'));
  el.classList.add('selected');
  selectedInterviewType = el.dataset.val;
}

function selectMode(el) {
  document.querySelectorAll('.mode-btn').forEach(b => b.classList.remove('selected'));
  el.classList.add('selected');
  selectedInterviewMode = el.dataset.val;
  const isOnline = selectedInterviewMode === 'Online';
  document.getElementById('link-field').style.display  = isOnline ? '' : 'none';
  document.getElementById('venue-field').style.display = isOnline ? 'none' : '';
}

function updateInterviewStats() {
  let eligibleCount = 0;
  let scheduledCount = 0;
  let completedCount = 0;
  let rejectedCount = 0;

  Object.values(applicantsData).forEach(arr => {
    arr.forEach(c => {
      const s = (c.intStatus || '').toLowerCase();
      if (s === 'eligible') eligibleCount++;
      else if (s === 'scheduled') scheduledCount++;
      else if (s === 'completed') completedCount++;
      else if (s === 'rejected') rejectedCount++;
    });
  });

  const el = id => document.getElementById(id);
  if (el('int-metric-eligible')) el('int-metric-eligible').textContent = eligibleCount.toLocaleString();
  if (el('int-metric-scheduled')) el('int-metric-scheduled').textContent = scheduledCount.toLocaleString();
  if (el('int-metric-completed')) el('int-metric-completed').textContent = completedCount.toLocaleString();
  if (el('int-metric-rejected')) el('int-metric-rejected').textContent = rejectedCount.toLocaleString();
}

function scheduleInterview() {
  const date       = document.getElementById('sm-date').value;
  const time       = document.getElementById('sm-time').value;
  const interviewer = document.getElementById('sm-interviewer').value.trim();

  if (!date || !time || !interviewer) {
    showToast('Please fill Interview Date, Time and Interviewer Name.', '#EF4444');
    return;
  }

  for (const arr of Object.values(applicantsData)) {
    const c = arr.find(x => x.id === currentCandidate.id);
    if (c) { 
      c.intStatus = 'Scheduled'; 
      c.interviewDate = date;
      c.interviewTime = time;
      break; 
    }
  }

  closeModal();
  renderCandidates(currentCandidates);
  renderJobCards(jobsData);
  updateInterviewStats();
  showToast(`✅ Interview scheduled for ${currentCandidate.name} on ${formatDate(date)} at ${formatTime(time)}`, '#15803D');
}

function formatDate(d) {
  if (!d) return '';
  const dt = new Date(d);
  return dt.toLocaleDateString('en-GB', { day:'2-digit', month:'short', year:'numeric' });
}

function formatTime(t) {
  if (!t) return '';
  const [h, m] = t.split(':');
  const hr = parseInt(h);
  return `${hr % 12 || 12}:${m} ${hr >= 12 ? 'PM' : 'AM'}`;
}

function showToast(msg, bg) {
  const el = document.getElementById('toast');
  if (!el) return;
  el.textContent = msg;
  el.style.background = bg || '#15803D';
  el.classList.add('show');
  setTimeout(() => el.classList.remove('show'), 4000);
}

// ════════════════════════════════════════════════════════════
//  INIT
// ════════════════════════════════════════════════════════════
document.addEventListener('DOMContentLoaded', () => {
  renderJobCards(jobsData);
  updateInterviewStats();
});
