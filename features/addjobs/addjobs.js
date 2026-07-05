// ── Rich text formatting ─────────────────────────────────────────
    function fmt(cmd) {
      document.getElementById('job-description').focus();
      document.execCommand(cmd, false, null);
    }

    // Placeholder behaviour for contenteditable
    const desc = document.getElementById('job-description');
    desc.addEventListener('focus', function() {
      if (this.innerHTML === '') this.style.color = 'var(--text-main)';
    });
    desc.addEventListener('blur', function() {
      if (this.innerHTML.trim() === '' || this.innerHTML === '<br>') {
        this.innerHTML = '';
      }
    });
    desc.style.setProperty('--ph', '"Write detailed job description..."');

    // ── Live Preview ──────────────────────────────────────────────────
    function updatePreview() {
      const title    = document.getElementById('f-title').value.trim();
      const dept     = document.getElementById('f-dept').value;
      const cat      = document.getElementById('f-category').value;
      const loc      = document.getElementById('f-location').value.trim();
      const wmode    = document.getElementById('f-workmode').value;
      const emptype  = document.getElementById('f-emptype').value;
      const explevel = document.getElementById('f-explevel').value;
      const skills   = document.getElementById('f-skills').value.trim();
      const salMin   = document.getElementById('f-salmin').value;
      const salMax   = document.getElementById('f-salmax').value;
      const cur      = document.getElementById('f-currency').value;

      const preview = document.getElementById('preview-body');

      if (!title) {
        preview.innerHTML = `<div class="preview-empty"><i class="fa-regular fa-file-lines"></i><p>Fill the form to see a live preview of your job posting here.</p></div>`;
        return;
      }

      const skillTags = skills ? skills.split(',').map(s => `<span class="preview-tag">${s.trim()}</span>`).join('') : '';
      const salaryStr = (salMin || salMax)
        ? `${cur}${salMin || '?'} – ${cur}${salMax || '?'}`
        : '—';

      preview.innerHTML = `
        <div class="preview-job-title">${title}</div>
        <div style="font-size:0.72rem;color:var(--text-muted);margin-bottom:8px;">Synergech · ${dept || cat || 'Department'}</div>
        <div class="preview-tags">
          ${emptype ? `<span class="preview-tag">${emptype}</span>` : ''}
          ${wmode   ? `<span class="preview-tag">${wmode}</span>` : ''}
          ${explevel? `<span class="preview-tag">${explevel}</span>` : ''}
        </div>
        <hr class="preview-divider">
        <div class="preview-meta">
          ${loc ? `<div><i class="fa-solid fa-location-dot"></i> ${loc}</div>` : ''}
          <div><i class="fa-solid fa-money-bill-wave"></i> ${salaryStr}</div>
          ${skills ? `<div><i class="fa-solid fa-code"></i> ${skills}</div>` : ''}
        </div>
        ${skillTags ? `<hr class="preview-divider"><div style="font-size:0.7rem;font-weight:600;color:var(--text-muted);margin-bottom:6px;">SKILLS</div><div class="preview-tags">${skillTags}</div>` : ''}
        <div style="margin-top:12px;font-size:0.68rem;color:#94a3b8;">Posted by Admin User · ${new Date().toLocaleDateString('en-GB', {day:'2-digit',month:'short',year:'numeric'})}</div>
      `;
    }

    // ── Save to localStorage ─────────────────────────────────────────
    function buildJobObject(status) {
      const title   = document.getElementById('f-title').value.trim();
      const dept    = document.getElementById('f-dept').value || document.getElementById('f-category').value;
      const loc     = document.getElementById('f-location').value.trim();
      const type    = document.getElementById('f-emptype').value;
      const expVal  = document.getElementById('f-minexp').value.trim();
      const salMin  = document.getElementById('f-salmin').value;
      const salMax  = document.getElementById('f-salmax').value;
      const cur     = document.getElementById('f-currency').value;
      const today   = new Date().toLocaleDateString('en-GB', {day:'2-digit', month:'short', year:'numeric'});

      return {
        id: 'JOB-' + Math.floor(1000 + Math.random() * 8999),
        title, dept, loc, type,
        exp: expVal || '0+ years',
        apps: 0,
        status: status,
        posted: today,
        salary: (salMin && salMax) ? `${cur}${salMin} – ${cur}${salMax}` : '—',
        skills: document.getElementById('f-skills').value.trim(),
      };
    }

    function validateForm() {
      const required = [
        { id: 'f-title',    label: 'Job Title' },
        { id: 'f-category', label: 'Job Category' },
        { id: 'f-emptype',  label: 'Employment Type' },
        { id: 'f-location', label: 'Location' },
        { id: 'f-workmode', label: 'Work Mode' },
        { id: 'f-explevel', label: 'Experience Level' },
        { id: 'f-minexp',   label: 'Minimum Experience' },
        { id: 'f-salmin',   label: 'Salary Range (Min)' },
        { id: 'f-salmax',   label: 'Salary Range (Max)' },
        { id: 'f-skills',   label: 'Primary Skills' },
        { id: 'f-qual',     label: 'Minimum Qualification' },
        { id: 'f-deadline', label: 'Application Deadline' },
      ];
      for (const field of required) {
        const el = document.getElementById(field.id);
        if (!el || !el.value.trim()) {
          showToast(`⚠️ Please fill in: ${field.label}`);
          el && el.focus();
          return false;
        }
      }
      const descEl = document.getElementById('job-description');
      if (!descEl.innerText.trim()) {
        showToast('⚠️ Please enter the Job Description.');
        descEl.focus();
        return false;
      }
      return true;
    }

    function saveToStorage(job) {
      let jobs = JSON.parse(localStorage.getItem('jobsData')) || [];
      jobs.unshift(job);
      localStorage.setItem('jobsData', JSON.stringify(jobs));
    }

    function publishJob() {
      if (!validateForm()) return;
      const job = buildJobObject('Active');
      saveToStorage(job);
      showToast('✅ Job published successfully!');
      setTimeout(() => { window.location.href = '../jobmanagement/jobmanagement.html'; }, 1600);
    }

    function saveDraft() {
      const title = document.getElementById('f-title').value.trim();
      if (!title) { showToast('⚠️ Please enter a Job Title before saving draft.'); return; }
      const job = buildJobObject('Draft');
      saveToStorage(job);
      showToast('💾 Draft saved successfully!');
      setTimeout(() => { window.location.href = '../jobmanagement/jobmanagement.html'; }, 1600);
    }

    function saveAndClose() {
      if (!validateForm()) return;
      const job = buildJobObject(document.getElementById('f-status').value);
      saveToStorage(job);
      showToast('✅ Job saved!');
      setTimeout(() => { window.location.href = '../jobmanagement/jobmanagement.html'; }, 1600);
    }

    function cancelJob() {
      if (confirm('Discard changes and go back?')) {
        window.location.href = '../jobmanagement/jobmanagement.html';
      }
    }

    // ── Toast ────────────────────────────────────────────────────────
    let toastTimer;
    function showToast(msg) {
      const t = document.getElementById('toast');
      t.textContent = msg;
      t.style.display = 'block';
      clearTimeout(toastTimer);
      toastTimer = setTimeout(() => { t.style.display = 'none'; }, 3000);
    }