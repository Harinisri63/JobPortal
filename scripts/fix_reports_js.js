const fs = require('fs');
const p = 'D:/Synergech/Project/UIUX/features/reports/reports.js';
let h = fs.readFileSync(p, 'utf8');

const target = `function openAuditModal() {
  // Build log rows: merge seed + live library recent actions
  const liveEntries = reportLibraryData.slice(0, 5).map((r, i) => ({
    name: r.name,
    action: ['Generated','Exported','Viewed','Scheduled','Shared'][i % 5],
    user: r.createdBy || 'System',
    time: \`\${i + 1} hr\${i === 0 ? '' : 's'} ago\`
  }));

  const allLogs = [...liveEntries, ...auditSeedLogs];

  const tbody = document.getElementById('audit-table-tbody');
  if (tbody) {
    tbody.innerHTML = allLogs.map((log, idx) => {
      const ac = actionColors[log.action] || { bg: '#F1F5F9', color: '#475569' };
      const rowBg = idx % 2 === 0 ? 'var(--card-bg,#fff)' : '#F8FAFC';
      return \`<tr style="background:\${rowBg};border-bottom:1px solid #F1F5F9;">
        <td style="padding:10px 10px;color:var(--text-main,#1E293B);font-weight:600;">
          <i class="fa-solid fa-file-lines" style="color:#0A4BD2;margin-right:6px;"></i>\${log.name}
        </td>
        <td style="padding:10px;">
          <span style="background:\${ac.bg};color:\${ac.color};font-size:0.7rem;font-weight:700;padding:3px 10px;border-radius:20px;">\${log.action}</span>
        </td>
        <td style="padding:10px;color:#475569;">\${log.user}</td>
        <td style="padding:10px;color:#94A3B8;font-size:0.72rem;">\${log.time}</td>
      </tr>\`;
    }).join('');
  }

  const overlay = document.getElementById('audit-logs-overlay');
  if (overlay) {
    overlay.style.display = 'flex';
    overlay.style.alignItems = 'center';
    overlay.style.justifyContent = 'center';
  }
}`;

const replacement = `let auditCurrentPage = 1;
const auditItemsPerPage = 5;

window.auditGoToPage = function(page) {
  auditCurrentPage = page;
  renderAuditTable();
};

function renderAuditTable() {
  const liveEntries = reportLibraryData.slice(0, 5).map((r, i) => ({
    name: r.name,
    action: ['Generated','Exported','Viewed','Scheduled','Shared'][i % 5],
    user: r.createdBy || 'System',
    time: \`\${i + 1} hr\${i === 0 ? '' : 's'} ago\`
  }));
  const allLogs = [...liveEntries, ...auditSeedLogs];
  
  const totalPages = Math.ceil(allLogs.length / auditItemsPerPage);
  if(auditCurrentPage > totalPages && totalPages > 0) auditCurrentPage = totalPages;

  const startIdx = (auditCurrentPage - 1) * auditItemsPerPage;
  const pageLogs = allLogs.slice(startIdx, startIdx + auditItemsPerPage);

  const tbody = document.getElementById('audit-table-tbody');
  if (tbody) {
    tbody.innerHTML = pageLogs.map((log, idx) => {
      const ac = actionColors[log.action] || { bg: '#F1F5F9', color: '#475569' };
      const rowBg = idx % 2 === 0 ? 'var(--card-bg,#fff)' : '#F8FAFC';
      return \`<tr style="background:\${rowBg};border-bottom:1px solid #F1F5F9;">
        <td style="padding:10px 10px;color:var(--text-main,#1E293B);font-weight:600;">
          <i class="fa-solid fa-file-lines" style="color:#0A4BD2;margin-right:6px;"></i>\${log.name}
        </td>
        <td style="padding:10px;">
          <span style="background:\${ac.bg};color:\${ac.color};font-size:0.7rem;font-weight:700;padding:3px 10px;border-radius:20px;">\${log.action}</span>
        </td>
        <td style="padding:10px;color:#475569;">\${log.user}</td>
        <td style="padding:10px;color:#94A3B8;font-size:0.72rem;">\${log.time}</td>
      </tr>\`;
    }).join('');
  }
  
  const container = document.getElementById('audit-pagination-container');
  if (container) {
    if (totalPages <= 1) {
      container.innerHTML = '';
    } else {
      let pgHtml = \`<a href="javascript:void(0)" onclick="auditGoToPage(\${auditCurrentPage - 1})" \${auditCurrentPage === 1 ? 'style="pointer-events:none;opacity:0.5;"' : ''}><i class="fa-solid fa-angle-left"></i> Prev</a>\`;
      for(let i=1; i<=totalPages; i++) {
        if(i===1 || i===totalPages || (i >= auditCurrentPage - 1 && i <= auditCurrentPage + 1)) {
          pgHtml += \`<a href="javascript:void(0)" class="\${i === auditCurrentPage ? 'active' : ''}" onclick="auditGoToPage(\${i})">\${i}</a>\`;
        } else if (i === auditCurrentPage - 2 || i === auditCurrentPage + 2) {
          pgHtml += \`<span>...</span>\`;
        }
      }
      pgHtml += \`<a href="javascript:void(0)" onclick="auditGoToPage(\${auditCurrentPage + 1})" \${auditCurrentPage === totalPages ? 'style="pointer-events:none;opacity:0.5;"' : ''}>Next <i class="fa-solid fa-angle-right"></i></a>\`;
      container.innerHTML = pgHtml;
    }
  }
}

function openAuditModal() {
  auditCurrentPage = 1;
  renderAuditTable();

  const overlay = document.getElementById('audit-logs-overlay');
  if (overlay) {
    overlay.style.display = 'flex';
    overlay.style.alignItems = 'center';
    overlay.style.justifyContent = 'center';
  }
}`;

h = h.replace(target, replacement);
fs.writeFileSync(p, h, 'utf8');
