let currentPage = 1;
const itemsPerPage = 8;

// ─── DATA ────────────────────────────────────────────────────────
    let defaultJobs = [
      { id:'JOB-1021', title:'Senior DevOps Engineer',  dept:'Engineering', loc:'Bangalore', type:'Full Time', exp:'5-8 years',  apps:34, status:'Active',  posted:'12 May 2025' },
      { id:'JOB-1015', title:'React Developer',          dept:'Engineering', loc:'Remote',    type:'Full Time', exp:'2-4 years',  apps:28, status:'Active',  posted:'11 May 2025' },
      { id:'JOB-1008', title:'Product Manager',          dept:'Product',     loc:'Bangalore', type:'Full Time', exp:'3-6 years',  apps:52, status:'Active',  posted:'10 May 2025' },
      { id:'JOB-1002', title:'UI/UX Designer',           dept:'Design',      loc:'Pune',      type:'Full Time', exp:'2-4 years',  apps:21, status:'Active',  posted:'09 May 2025' },
      { id:'JOB-0991', title:'Data Analyst',             dept:'Analytics',   loc:'Hyderabad', type:'Full Time', exp:'1-3 years',  apps:18, status:'Active',  posted:'08 May 2025' },
      { id:'JOB-0982', title:'Backend Developer',        dept:'Engineering', loc:'Remote',    type:'Full Time', exp:'3-5 years',  apps:26, status:'Closed',  posted:'07 May 2025' },
      { id:'JOB-0975', title:'HR Executive',             dept:'HR',          loc:'Mumbai',    type:'Full Time', exp:'1-2 years',  apps:14, status:'Active',  posted:'07 May 2025' },
      { id:'JOB-0961', title:'Full Stack Developer',     dept:'Engineering', loc:'Bangalore', type:'Full Time', exp:'4-7 years',  apps:41, status:'Active',  posted:'06 May 2025' },
    ];

    // Load from localStorage or fall back to defaultJobs
    let jobsData = JSON.parse(localStorage.getItem('jobsData'));
    if (!jobsData || !Array.isArray(jobsData) || jobsData.length === 0) {
      jobsData = defaultJobs;
      localStorage.setItem('jobsData', JSON.stringify(jobsData));
    }

    // Next job counter for new additions
    let nextId = 910;

    // ─── APPLICANTS DATA (keyed by jobId) ─────────────────────────────
    const applicantsData = {
      'JOB-1021': [
        { name:'Arjun Mehta',     email:'arjun.mehta@gmail.com',   phone:'+91 9876543210', location:'Bangalore', exp:'6 years',    status:'Under Review', applied:'18 May 2025', avatar:'AM' },
        { name:'Priya Sharma',    email:'priya.sharma@gmail.com',   phone:'+91 9845012345', location:'Remote',    exp:'7 years',    status:'Shortlisted',  applied:'19 May 2025', avatar:'PS' },
        { name:'Rohit Nair',      email:'rohit.nair@gmail.com',     phone:'+91 9823456780', location:'Pune',      exp:'5 years',    status:'Rejected',     applied:'20 May 2025', avatar:'RN' },
        { name:'Sneha Iyer',      email:'sneha.iyer@gmail.com',     phone:'+91 9900112233', location:'Hyderabad', exp:'8 years',    status:'Shortlisted',  applied:'20 May 2025', avatar:'SI' },
        { name:'Karan Verma',     email:'karan.verma@gmail.com',    phone:'+91 9711223344', location:'Bangalore', exp:'6 years',    status:'Under Review', applied:'21 May 2025', avatar:'KV' },
      ],
      'JOB-1015': [
        { name:'Divya Reddy',     email:'divya.reddy@gmail.com',    phone:'+91 9654321098', location:'Remote',    exp:'3 years',    status:'Shortlisted',  applied:'17 May 2025', avatar:'DR' },
        { name:'Aakash Singh',    email:'aakash.singh@gmail.com',   phone:'+91 9987654321', location:'Mumbai',    exp:'2.5 years',  status:'Under Review', applied:'18 May 2025', avatar:'AS' },
        { name:'Meena Pillai',    email:'meena.pillai@gmail.com',   phone:'+91 9765432109', location:'Chennai',   exp:'4 years',    status:'Shortlisted',  applied:'18 May 2025', avatar:'MP' },
      ],
      'JOB-1008': [
        { name:'Vikram Joshi',    email:'vikram.joshi@gmail.com',   phone:'+91 9811223344', location:'Bangalore', exp:'5 years',    status:'Shortlisted',  applied:'14 May 2025', avatar:'VJ' },
        { name:'Ananya Das',      email:'ananya.das@gmail.com',     phone:'+91 9922334455', location:'Delhi',     exp:'4 years',    status:'Under Review', applied:'15 May 2025', avatar:'AD' },
        { name:'Suresh Kumar',    email:'suresh.kumar@gmail.com',   phone:'+91 9633445566', location:'Pune',      exp:'6 years',    status:'Rejected',     applied:'15 May 2025', avatar:'SK' },
      ],
      'JOB-1002': [
        { name:'Harini Sri',      email:'harini.sri@gmail.com',     phone:'+91 9876543210', location:'Chennai',   exp:'3 years',    status:'Shortlisted',  applied:'12 May 2025', avatar:'HS' },
        { name:'Rajan Pillai',    email:'rajan.pillai@gmail.com',   phone:'+91 9765001122', location:'Pune',      exp:'2 years',    status:'Under Review', applied:'13 May 2025', avatar:'RP' },
      ],
      'JOB-0991': [
        { name:'Chitra Nair',     email:'chitra.nair@gmail.com',    phone:'+91 9633221100', location:'Hyderabad', exp:'2 years',    status:'Under Review', applied:'11 May 2025', avatar:'CN' },
        { name:'Hari Prasad',     email:'hari.prasad@gmail.com',    phone:'+91 9988776655', location:'Remote',    exp:'1.5 years',  status:'Shortlisted',  applied:'12 May 2025', avatar:'HP' },
      ],
      'JOB-0982': [
        { name:'Nikhil Tiwari',   email:'nikhil.tiwari@gmail.com',  phone:'+91 9871234560', location:'Remote',    exp:'4 years',    status:'Shortlisted',  applied:'09 May 2025', avatar:'NT' },
        { name:'Pooja Saxena',    email:'pooja.saxena@gmail.com',   phone:'+91 9855443322', location:'Mumbai',    exp:'3 years',    status:'Rejected',     applied:'10 May 2025', avatar:'PX' },
      ],
      'JOB-0975': [
        { name:'Lakshmi Rao',     email:'lakshmi.rao@gmail.com',    phone:'+91 9700123456', location:'Mumbai',    exp:'1 year',     status:'Under Review', applied:'09 May 2025', avatar:'LR' },
      ],
      'JOB-0961': [
        { name:'Siddharth Gupta', email:'siddharth.g@gmail.com',    phone:'+91 9876001234', location:'Bangalore', exp:'5 years',    status:'Shortlisted',  applied:'08 May 2025', avatar:'SG' },
        { name:'Nandini Kulkarni',email:'nandini.k@gmail.com',      phone:'+91 9645678901', location:'Pune',      exp:'4 years',    status:'Under Review', applied:'09 May 2025', avatar:'NK' },
        { name:'Abhinav Sharma',  email:'abhinav.s@gmail.com',      phone:'+91 9811009988', location:'Remote',    exp:'6 years',    status:'Shortlisted',  applied:'09 May 2025', avatar:'AB' },
      ],
    };

    // ─── RENDER TABLES ────────────────────────────────────────────────
    function badgeHtml(status) {
      const cls = status === 'Active' ? 'badge-active' : status === 'Closed' ? 'badge-closed' : 'badge-draft';
      return `<span class="badge-status ${cls}">${status}</span>`;
    }

    function renderDash(data) {
      const totalPages = Math.ceil(data.length / itemsPerPage) || 1;
      const start = (currentPage - 1) * itemsPerPage;
      const pageData = data.slice(start, start + itemsPerPage);

      const tbody = document.getElementById('dash-tbody');
      tbody.innerHTML = pageData.map(j => `
        <tr>
          <td class="job-id">${j.id}</td>
          <td class="job-title">${j.title}</td>
          <td>${j.dept}</td>
          <td>${j.loc}</td>
          <td>${j.type}</td>
          <td>${j.exp}</td>
          <td>${j.apps}</td>
          <td>${badgeHtml(j.status)}</td>
          <td>${j.posted}</td>
          <td>
            <div class="action-icons">
              <a href="#" title="View" onclick="viewJob('${j.id}');return false;"><i class="fa-regular fa-eye"></i></a>
              <button title="Edit" onclick="openEditJobModal('${j.id}')"><i class="fa-regular fa-pen-to-square"></i></button>
              <button title="More"><i class="fa-solid fa-ellipsis-vertical"></i></button>
            </div>
          </td>
        </tr>`).join('');
        
      updateMetrics(data);
      renderPagination(totalPages, 'dash');
    }

    function updateMetrics(data) {
      const total = data.length;
      let active = 0, closed = 0, draft = 0;
      data.forEach(j => {
        if(j.status === 'Active') active++;
        else if(j.status === 'Closed') closed++;
        else if(j.status === 'Draft') draft++;
      });
      
      const elTotal = document.getElementById('metric-total');
      const elActive = document.getElementById('metric-active');
      const elClosed = document.getElementById('metric-closed');
      const elDraft = document.getElementById('metric-draft');
      
      if(elTotal) elTotal.textContent = total;
      if(elActive) elActive.textContent = active;
      if(elClosed) elClosed.textContent = closed;
      if(elDraft) elDraft.textContent = draft;
    }

    function renderOverview(data) {
      const totalPages = Math.ceil(data.length / itemsPerPage) || 1;
      const start = (currentPage - 1) * itemsPerPage;
      const pageData = data.slice(start, start + itemsPerPage);

      const tbody = document.getElementById('ov-tbody');
      tbody.innerHTML = pageData.map(j => `
        <tr>
          <td class="job-id">${j.id}</td>
          <td class="job-title">${j.title}</td>
          <td>${j.dept}</td>
          <td>${j.loc}</td>
          <td>${j.type}</td>
          <td>${j.exp}</td>
          <td>${badgeHtml(j.status)}</td>
          <td>${j.posted}</td>
          <td>
            <div class="action-icons">
              <a href="#" title="View" onclick="viewJob('${j.id}');return false;"><i class="fa-regular fa-eye"></i></a>
              <button title="Edit" onclick="openEditJobModal('${j.id}')"><i class="fa-regular fa-pen-to-square"></i></button>
              <button title="More"><i class="fa-solid fa-ellipsis-vertical"></i></button>
            </div>
          </td>
        </tr>`).join('');
        
      renderPagination(totalPages, 'ov');
    }

    function renderPagination(totalPages, view) {
      const containerId = view === 'dash' ? 'pagination-container-dash' : 'pagination-container-ov';
      const container = document.getElementById(containerId);
      if (!container) return;

      let html = '';
      
      if (currentPage > 1) {
        html += `<button onclick="goToPage(${currentPage - 1}, '${view}')">← Previous</button>`;
      } else {
        html += `<button disabled>← Previous</button>`;
      }
      
      for (let i = 1; i <= totalPages; i++) {
        if (i === currentPage) {
          html += `<button class="pg-active">${i}</button>`;
        } else {
          html += `<button onclick="goToPage(${i}, '${view}')">${i}</button>`;
        }
      }
      
      if (currentPage < totalPages) {
        html += `<button onclick="goToPage(${currentPage + 1}, '${view}')">Next →</button>`;
      } else {
        html += `<button disabled>Next →</button>`;
      }
      
      container.innerHTML = html;
    }

    window.goToPage = function(page, view) {
      currentPage = page;
      if (!view) {
         view = document.getElementById('view-dashboard').classList.contains('active') ? 'dash' : 'ov';
      }
      filterTable(view, false);
    };

    // ─── FILTER ───────────────────────────────────────────────────────
    function filterTable(view, resetPage = true) {
      if (resetPage) currentPage = 1;
      let data = [...jobsData];
      if (view === 'dash') {
        const q      = document.getElementById('dash-search').value.toLowerCase();
        const status = document.getElementById('dash-status').value;
        const dept   = document.getElementById('dash-dept').value;
        const type   = document.getElementById('dash-type').value;
        data = data.filter(j =>
          (!q      || j.title.toLowerCase().includes(q) || j.id.toLowerCase().includes(q) || j.loc.toLowerCase().includes(q) || j.dept.toLowerCase().includes(q)) &&
          (!status || j.status === status) &&
          (!dept   || j.dept   === dept)  &&
          (!type   || j.type   === type)
        );
        renderDash(data);
      } else {
        const q = document.getElementById('ov-search').value.toLowerCase();
        data = data.filter(j =>
          !q || j.title.toLowerCase().includes(q) || j.id.toLowerCase().includes(q) || j.dept.toLowerCase().includes(q) || j.loc.toLowerCase().includes(q)
        );
        renderOverview(data);
      }
    }

    function resetFilter(view) {
      document.getElementById('dash-search').value = '';
      document.getElementById('dash-status').value = '';
      document.getElementById('dash-dept').value   = '';
      document.getElementById('dash-type').value   = '';
      currentPage = 1;
      filterTable(view, false);
    }

    // ─── VIEW SWITCHER ────────────────────────────────────────────────
    const titles = {
      dashboard: 'Job Management Dashboard',
      overview:  'Job Management',
      detail:    'Job Details'
    };

    function showView(name) {
      document.querySelectorAll('.view').forEach(v => v.classList.remove('active'));
      document.getElementById('view-' + name).classList.add('active');
      document.getElementById('header-title').textContent = titles[name];
    }

    // ─── VIEW JOB DETAIL ─────────────────────────────────────────────
    function viewJob(id) {
      const j = jobsData.find(x => x.id === id);
      if (!j) return;
      document.getElementById('det-id').textContent      = j.id;
      document.getElementById('det-posted').textContent  = '. Posted on ' + j.posted;
      document.getElementById('det-badge').textContent   = j.status;
      document.getElementById('det-badge').className     = 'badge-status ' + (j.status === 'Active' ? 'badge-active' : j.status === 'Closed' ? 'badge-closed' : 'badge-draft');
      document.getElementById('det-title').textContent   = j.title;
      document.getElementById('det-dept').textContent    = j.dept;
      document.getElementById('det-type').textContent    = j.type;
      document.getElementById('det-exp').textContent     = j.exp;
      document.getElementById('det-footer-id').textContent = j.id;

      // Populate qualification grid dynamically
      document.getElementById('det-qual-exp').textContent    = j.exp;
      document.getElementById('det-qual-type').textContent   = j.type;
      document.getElementById('det-qual-dept').textContent   = j.dept;
      document.getElementById('det-qual-posted').textContent = j.posted;

      // Wire Edit button
      const editBtn = document.querySelector('.btn-edit-job');
      editBtn.setAttribute('onclick', `openEditJobModal('${j.id}')`);

      // Wire View Applications button
      const appsBtn = document.querySelector('.btn-view-apps');
      appsBtn.setAttribute('onclick', `openAppsModal('${j.id}')`);

      showView('detail');
    }

    // ─── VIEW APPLICATIONS MODAL ─────────────────────────────────────
    function openAppsModal(jobId) {
      const job = jobsData.find(x => x.id === jobId);
      const applicants = applicantsData[jobId] || [];

      document.getElementById('apps-modal-title').innerHTML =
        `Applicants &nbsp;<span style="font-size:0.75rem;font-weight:500;color:#64748b;">— ${job ? job.title : jobId} &nbsp;(${applicants.length} applied)</span>`;

      const container = document.getElementById('apps-list-container');

      if (applicants.length === 0) {
        container.innerHTML = `<div style="text-align:center;padding:40px 0;color:#94a3b8;font-size:0.85rem;"><i class="fa-regular fa-folder-open" style="font-size:2rem;margin-bottom:10px;display:block;"></i>No applications received yet.</div>`;
      } else {
        container.innerHTML = applicants.map((a, i) => {
          const statusColors = {
            'Shortlisted':  { bg: '#DCFCE7', color: '#15803D' },
            'Under Review': { bg: '#FEF3C7', color: '#D97706' },
            'Rejected':     { bg: '#FEE2E2', color: '#B91C1C' },
          };
          const sc = statusColors[a.status] || { bg: '#EEF2FF', color: '#4F46E5' };
          const avatarColors = ['#6366F1','#8B5CF6','#0EA5E9','#10B981','#F59E0B','#EF4444','#EC4899','#14B8A6'];
          const avBg = avatarColors[i % avatarColors.length];

          return `
          <div style="background:#F8FAFC;border:1px solid #E2E8F0;border-radius:10px;padding:14px 16px;display:flex;gap:14px;align-items:flex-start;">
            <!-- Avatar -->
            <div style="width:42px;height:42px;border-radius:50%;background:${avBg};color:#fff;display:flex;align-items:center;justify-content:center;font-size:0.75rem;font-weight:700;flex-shrink:0;">${a.avatar}</div>
            <!-- Info -->
            <div style="flex:1;min-width:0;">
              <div style="display:flex;justify-content:space-between;align-items:center;flex-wrap:wrap;gap:6px;">
                <span style="font-size:0.88rem;font-weight:700;color:#1E293B;">${a.name}</span>
                <span style="background:${sc.bg};color:${sc.color};font-size:0.65rem;font-weight:700;padding:3px 10px;border-radius:20px;">${a.status}</span>
              </div>
              <div style="display:grid;grid-template-columns:1fr 1fr;gap:4px 18px;margin-top:8px;">
                <span style="font-size:0.73rem;color:#475569;"><i class="fa-regular fa-envelope" style="width:14px;"></i> ${a.email}</span>
                <span style="font-size:0.73rem;color:#475569;"><i class="fa-solid fa-phone" style="width:14px;"></i> ${a.phone}</span>
                <span style="font-size:0.73rem;color:#475569;"><i class="fa-solid fa-location-dot" style="width:14px;"></i> ${a.location}</span>
                <span style="font-size:0.73rem;color:#475569;"><i class="fa-solid fa-briefcase" style="width:14px;"></i> ${a.exp} experience</span>
              </div>
              <div style="margin-top:8px;font-size:0.68rem;color:#94a3b8;">Applied on ${a.applied}</div>
            </div>
          </div>`;
        }).join('');
      }

      document.getElementById('viewAppsModal').classList.add('open');
    }

    function closeAppsModal() {
      document.getElementById('viewAppsModal').classList.remove('open');
    }

    // ─── ADD JOB MODAL ────────────────────────────────────────────────
    function openAddJobModal() {
      document.getElementById('addJobModal').classList.add('open');
    }
    function closeAddJobModal() {
      document.getElementById('addJobModal').classList.remove('open');
    }
    function addNewJob() {
      const title  = document.getElementById('new-title').value.trim();
      const dept   = document.getElementById('new-dept').value;
      const loc    = document.getElementById('new-loc').value.trim();
      const type   = document.getElementById('new-type').value;
      const exp    = document.getElementById('new-exp').value.trim() || '0-1 years';
      const status = document.getElementById('new-status').value;
      const salary = document.getElementById('new-salary') ? document.getElementById('new-salary').value.trim() : '';
      const skills = document.getElementById('f-skills') ? document.getElementById('f-skills').value.trim() : '';
      const desc   = document.getElementById('new-desc') ? document.getElementById('new-desc').value.trim() : '';

      if (!title || !dept || !loc || !type) {
        showToast('Please fill in all required fields.'); return;
      }
      const today = new Date();
      const posted = today.toLocaleDateString('en-GB', { day:'2-digit', month:'short', year:'numeric' }).replace(/ /g,' ');
      const newJob = {
        id: 'JOB-0' + (--nextId), title, dept, loc, type, exp, apps: 0, status, posted,
        salary: salary || '₹ 12-18 LPA',
        skills: skills,
        description: desc
      };
      jobsData.unshift(newJob);
      localStorage.setItem('jobsData', JSON.stringify(jobsData));
      renderDash(jobsData);
      renderOverview(jobsData);
      closeAddJobModal();
      showToast('Job "' + title + '" posted successfully!');
      // Reset form
      ['new-title','new-dept','new-loc','new-type','new-exp','new-salary','new-desc','new-deadline','f-category','f-workmode','f-skills'].forEach(id => {
        const el = document.getElementById(id);
        if (el) el.value = '';
      });
      updatePreviewJobManagement();
    }

    // ─── LIVE PREVIEW FOR JOB MANAGEMENT MODAL ────────────────────────
    function updatePreviewJobManagement() {
      const title    = document.getElementById('new-title') ? document.getElementById('new-title').value.trim() : '';
      const location = document.getElementById('new-loc')   ? document.getElementById('new-loc').value.trim() : '';
      const empty    = document.getElementById('preview-empty-jm');
      const content  = document.getElementById('preview-content-jm');

      if (!empty || !content) return;

      if (!title || !location) {
        empty.style.display = 'block';
        content.style.display = 'none';
        return;
      }

      empty.style.display = 'none';
      content.style.display = 'block';

      document.getElementById('p-title-jm').textContent    = title;
      document.getElementById('p-location-jm').textContent = location;

      const exp     = document.getElementById('new-exp')    ? document.getElementById('new-exp').value.trim()    : 'Not specified';
      const salary  = document.getElementById('new-salary') ? document.getElementById('new-salary').value.trim() : '';
      const empType = document.getElementById('new-type')   ? document.getElementById('new-type').value : 'Full Time';
      const skills  = document.getElementById('f-skills')   ? document.getElementById('f-skills').value.trim()   : '';
      const desc    = document.getElementById('new-desc')   ? document.getElementById('new-desc').value.trim()   : '';

      document.getElementById('p-exp-jm').textContent     = exp || 'Not specified';
      document.getElementById('p-salary-jm').textContent  = salary || 'Not specified';
      document.getElementById('p-emptype-jm').textContent = empType;

      // Tags
      const tagsContainer = document.getElementById('p-tags-jm');
      tagsContainer.innerHTML = '';
      if (skills) {
        skills.split(',').forEach(s => {
          if (s.trim()) {
            const tag = document.createElement('span');
            tag.className = 'addjob-preview-tag';
            tag.textContent = s.trim();
            tagsContainer.appendChild(tag);
          }
        });
      }

      // Description
      document.getElementById('p-desc-jm').textContent = desc || '';
    }

    // ─── RICH TEXT FORMAT HELPER ──────────────────────────────────────
    function fmt(cmd) {
      document.execCommand(cmd, false, null);
    }

    // ─── EDIT JOB MODAL ───────────────────────────────────────────────
    function openEditJobModal(id) {
      const j = jobsData.find(x => x.id === id);
      if (!j) return;
      document.getElementById('edit-job-id').value = j.id;
      document.getElementById('edit-title').value = j.title;
      document.getElementById('edit-dept').value = j.dept;
      document.getElementById('edit-loc').value = j.loc;
      document.getElementById('edit-type').value = j.type;
      document.getElementById('edit-exp').value = j.exp;
      document.getElementById('edit-status').value = j.status;
      
      document.getElementById('editJobModal').classList.add('open');
    }

    function closeEditJobModal() {
      document.getElementById('editJobModal').classList.remove('open');
    }

    function saveJobChanges() {
      const id = document.getElementById('edit-job-id').value;
      const title = document.getElementById('edit-title').value.trim();
      const dept = document.getElementById('edit-dept').value;
      const loc = document.getElementById('edit-loc').value.trim();
      const type = document.getElementById('edit-type').value;
      const exp = document.getElementById('edit-exp').value.trim();
      const status = document.getElementById('edit-status').value;

      if (!title || !loc) {
        showToast('Please fill in all required fields.');
        return;
      }

      const index = jobsData.findIndex(x => x.id === id);
      if (index !== -1) {
        jobsData[index].title = title;
        jobsData[index].dept = dept;
        jobsData[index].loc = loc;
        jobsData[index].type = type;
        jobsData[index].exp = exp;
        jobsData[index].status = status;

        localStorage.setItem('jobsData', JSON.stringify(jobsData));
        renderDash(jobsData);
        renderOverview(jobsData);
        closeEditJobModal();
        showToast('Job details updated successfully!');
        
        // If we are currently looking at this job's detail view, refresh it
        const currentView = document.querySelector('.view.active').id;
        if (currentView === 'view-detail' && document.getElementById('det-id').textContent === id) {
          viewJob(id);
        }
      }
    }

    // ─── TOAST ───────────────────────────────────────────────────────
    let toastTimer;
    function showToast(msg) {
      const t = document.getElementById('toast');
      t.textContent = msg;
      t.style.display = 'block';
      clearTimeout(toastTimer);
      toastTimer = setTimeout(() => { t.style.display = 'none'; }, 3000);
    }

    // ─── TAB SWITCHER IN SIDEBAR (Dashboard ↔ Overview) ─────────────
    // Clicking "Job Management" link cycles between dashboard & overview
    document.querySelector('.sidebar-nav a.active').addEventListener('click', function(e) {
      e.preventDefault();
      const current = document.querySelector('.view.active').id;
      showView(current === 'view-dashboard' ? 'overview' : 'dashboard');
    });

    // ─── INIT ─────────────────────────────────────────────────────────
    renderDash(jobsData);
    renderOverview(jobsData);
    showView('dashboard');

// Export functionality
function toggleExportMenu() {
  const menu = document.getElementById('export-menu');
  menu.style.display = menu.style.display === 'none' ? 'flex' : 'none';
}

// Close export menu when clicking outside
document.addEventListener('click', function(e) {
  const menu = document.getElementById('export-menu');
  if (menu && menu.style.display === 'flex' && !e.target.closest('.btn-export') && !e.target.closest('#export-menu')) {
    menu.style.display = 'none';
  }
});

function exportJobs(type) {
  toggleExportMenu();
  const jobsToExport = JSON.parse(localStorage.getItem('jobsData')) || defaultJobs;
  
  if (type === 'pdf') {
    if (!window.jspdf) {
      alert("jsPDF library not loaded");
      return;
    }
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF();
    doc.text("Job Listings", 14, 15);
    const tableData = jobsToExport.map(j => [j.id, j.title, j.dept, j.loc, j.type, j.status, j.posted]);
    doc.autoTable({
      head: [['ID', 'Title', 'Department', 'Location', 'Type', 'Status', 'Posted Date']],
      body: tableData,
      startY: 20,
      styles: { fontSize: 8 }
    });
    doc.save('Jobs_Export.pdf');
  } 
  else if (type === 'excel') {
    let csvContent = "data:text/csv;charset=utf-8,";
    csvContent += "ID,Title,Department,Location,Type,Status,Posted Date\n";
    jobsToExport.forEach(j => {
      let row = `"${j.id}","${j.title}","${j.dept}","${j.loc}","${j.type}","${j.status}","${j.posted}"`;
      csvContent += row + "\r\n";
    });
    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "Jobs_Export.csv");
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
  else if (type === 'doc') {
    let htmlContent = `<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'>
      <head><meta charset='utf-8'><title>Job Listings</title></head><body>
      <h2>Job Listings</h2>
      <table border="1" cellpadding="5" cellspacing="0" style="border-collapse: collapse; width: 100%;">
        <tr style="background-color: #f1f5f9; text-align: left;">
          <th>ID</th><th>Title</th><th>Department</th><th>Location</th><th>Type</th><th>Status</th><th>Posted Date</th>
        </tr>`;
    jobsToExport.forEach(j => {
      htmlContent += `<tr><td>${j.id}</td><td>${j.title}</td><td>${j.dept}</td><td>${j.loc}</td><td>${j.type}</td><td>${j.status}</td><td>${j.posted}</td></tr>`;
    });
    htmlContent += `</table></body></html>`;
    
    const blob = new Blob(['\ufeff', htmlContent], { type: 'application/msword' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "Jobs_Export.doc";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}
