const fs = require('fs');
const path = 'D:/Synergech/Project/UIUX/features/Admin/admin.html';
let content = fs.readFileSync(path, 'utf8');

// Replace date picker html
content = content.replace(
  '<div class="date-range-picker-container">\n            <span>May 06 2025 - Jun 06 2025</span>',
  '<div class="date-range-picker-container" onclick="simulateDateChange()" style="cursor:pointer;" title="Click to change date range">\n            <span id="dashboard-date-range">May 06 2025 - Jun 06 2025</span>'
);

// Add Javascript function before the closing script tag around line 1515
const jsToAdd = `
    // Simulate date range change
    window.simulateDateChange = function() {
      // Toggle to the requested date range
      const dateSpan = document.getElementById('dashboard-date-range');
      if (dateSpan.innerText === '03 Jun 2026 - 03 Jul 2026') {
        dateSpan.innerText = 'May 06 2025 - Jun 06 2025';
      } else {
        dateSpan.innerText = '03 Jun 2026 - 03 Jul 2026';
      }
      
      // Update charts to simulate new data
      updateUserRegChart();
      updateHiringChart();
      
      // Simulate slight numeric tweaks to metrics
      const usersEl = document.getElementById('stat-total-users');
      const jobsEl = document.getElementById('stat-active-jobs');
      const appsEl = document.getElementById('stat-total-apps');
      const newUsersEl = document.getElementById('stat-new-users');
      const complaintsEl = document.getElementById('stat-complaints');
      
      if(usersEl) usersEl.innerText = Math.floor(Math.random() * 50) + 5;
      if(jobsEl) jobsEl.innerText = Math.floor(Math.random() * 20) + 2;
      if(appsEl) appsEl.innerText = Math.floor(Math.random() * 100) + 10;
      if(newUsersEl) newUsersEl.innerText = Math.floor(Math.random() * 10) + 1;
      if(complaintsEl) complaintsEl.innerText = Math.floor(Math.random() * 15) + 1;
    };
`;

content = content.replace('  </script>\n</body>', jsToAdd + '  </script>\n</body>');

fs.writeFileSync(path, content, 'utf8');
console.log("Updated admin.html");
