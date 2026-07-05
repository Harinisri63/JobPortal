// ── CANDIDATE DATA MAP ────────────────────────────────────────────
  const candidates = {
    'CND-10234': { name:'Arav Kumar',   role:'Senior DevOps Engineer', status:'Shortlisted', stage:'Interview', loc:'Bangalore, India', email:'arav.kumar@gmail.com',   phone:'+91 9876548321', applied:'12 May 2025', avatar:'AK', avatarBg:'#6366F1' },
    'CND-10235': { name:'Priya Sharma', role:'React Developer',         status:'Interview',   stage:'Interview', loc:'Pune, India',       email:'priya.sharma@gmail.com', phone:'+91 9845012345', applied:'11 May 2025', avatar:'PS', avatarBg:'#8B5CF6' },
    'CND-10236': { name:'Rohan Joshi',  role:'Product Manager',         status:'Applied',     stage:'Applied',   loc:'Hyderabad, India',  email:'rohan.joshi@gmail.com',  phone:'+91 9823456780', applied:'10 May 2025', avatar:'RJ', avatarBg:'#0EA5E9' },
    'CND-10237': { name:'Neha Singh',   role:'UI/UX Designer',          status:'Shortlisted', stage:'Interview', loc:'Delhi, India',      email:'neha.singh@gmail.com',   phone:'+91 9900112233', applied:'09 May 2025', avatar:'NS', avatarBg:'#EC4899' },
    'CND-10238': { name:'Vikram Mehta', role:'Data Analyst',            status:'Applied',     stage:'Applied',   loc:'Mumbai, India',     email:'vikram.mehta@gmail.com', phone:'+91 9711223344', applied:'08 May 2025', avatar:'VM', avatarBg:'#10B981' },
    'CND-10239': { name:'Kavya',        role:'Backend Developer',       status:'Interview',   stage:'Interview', loc:'Bangalore, India',  email:'kavya.m@gmail.com',      phone:'+91 9655443322', applied:'07 May 2025', avatar:'KM', avatarBg:'#F59E0B' },
    'CND-10240': { name:'Arjun',        role:'Full Stack Developer',    status:'Applied',     stage:'Applied',   loc:'Noida, India',      email:'arjun.r@gmail.com',      phone:'+91 9876001234', applied:'06 May 2025', avatar:'AR', avatarBg:'#EF4444' },
    'CND-10241': { name:'Simran',       role:'HR Executive',            status:'Shortlisted', stage:'Shortlisted', loc:'Chandigarh, India',email:'simran.k@gmail.com',    phone:'+91 9988776655', applied:'05 May 2025', avatar:'SK', avatarBg:'#14B8A6' },
    'CND-10242': { name:'Deepak',       role:'QA Engineer',             status:'Applied',     stage:'Applied',   loc:'Pune, India',       email:'deepak.v@gmail.com',     phone:'+91 9700123456', applied:'04 May 2025', avatar:'DV', avatarBg:'#6366F1' },
  };

  function pillClass(s) {
    return s === 'Shortlisted' ? 'pill-shortlisted' : s === 'Interview' ? 'pill-interview' : s === 'Hired' ? 'pill-hired' : s === 'Rejected' ? 'pill-rejected' : 'pill-applied';
  }

  // ── LOAD FROM URL PARAM ───────────────────────────────────────────
  window.addEventListener('DOMContentLoaded', function() {
    const params = new URLSearchParams(window.location.search);
    const id = params.get('id');
    const c  = candidates[id];
    if (!c) return; // keep defaults (Rohan Sharma shown by default)

    document.getElementById('ph-avatar').textContent  = c.avatar;
    document.getElementById('ph-avatar').style.background = c.avatarBg;
    document.getElementById('ph-name').textContent    = c.name;
    document.getElementById('ph-role').textContent    = c.role;
    document.getElementById('ph-id').textContent      = id;
    document.getElementById('ph-applied').textContent = c.applied;
    document.getElementById('ph-stage').textContent   = c.stage;

    const statusEl = document.getElementById('ph-status');
    statusEl.textContent  = c.status;
    statusEl.className    = 'status-pill ' + pillClass(c.status);

    document.getElementById('ph-contact').innerHTML = `
      <span><i class="fa-solid fa-location-dot"></i> ${c.loc}</span>
      <span><i class="fa-regular fa-envelope"></i> ${c.email}</span>
      <span><i class="fa-solid fa-phone"></i> ${c.phone}</span>
    `;
    document.title = c.name + ' – Candidate Profile';
  });

  function saveNote(btn) {
    const area = btn.previousElementSibling;
    if (!area.value.trim()) return;
    btn.textContent = '✅ Saved!';
    btn.style.color = '#15803D';
    btn.style.borderColor = '#15803D';
    setTimeout(() => { btn.textContent = 'Save Note'; btn.style.color = ''; btn.style.borderColor = ''; }, 2000);
  }