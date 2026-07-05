const fs = require('fs');

const path = 'D:/Synergech/Project/UIUX/features/contentmanagement/contentmanagement.html';
let html = fs.readFileSync(path, 'utf8');

const marker = '<table class="perm-table">';
const end_marker = '<li class="activity-item">';

const start_idx = html.indexOf(marker) + marker.length;
const end_idx = html.indexOf(end_marker, start_idx);

if (start_idx !== -1 + marker.length && end_idx !== -1) {
    const replacement = `
              <thead>
                <tr>
                  <th>Permission</th>
                  <th>Admin</th>
                  <th>Viewer</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>Create Content</td>
                  <td><i class="fa-solid fa-circle-check perm-check"></i></td>
                  <td><i class="fa-solid fa-circle-check perm-check"></i></td>
                </tr>
                <tr>
                  <td>Edit Content</td>
                  <td><i class="fa-solid fa-circle-check perm-check"></i></td>
                  <td><i class="fa-solid fa-circle-check perm-check"></i></td>
                </tr>
                <tr>
                  <td>Delete Content</td>
                  <td><i class="fa-solid fa-circle-check perm-check"></i></td>
                  <td>-</td>
                </tr>
                <tr>
                  <td>Publish/ Unpublish</td>
                  <td><i class="fa-solid fa-circle-check perm-check"></i></td>
                  <td>-</td>
                </tr>
                <tr>
                  <td>Schedule Content</td>
                  <td><i class="fa-solid fa-circle-check perm-check"></i></td>
                  <td>-</td>
                </tr>
                <tr>
                  <td>Manage Categories</td>
                  <td><i class="fa-solid fa-circle-check perm-check"></i></td>
                  <td>-</td>
                </tr>
                <tr>
                  <td>Site setting access</td>
                  <td><i class="fa-solid fa-circle-check perm-check"></i></td>
                  <td>-</td>
                </tr>
                <tr>
                  <td>View Reports</td>
                  <td><i class="fa-solid fa-circle-check perm-check"></i></td>
                  <td>-</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Right Column of Split Row -->
        <div style="display:flex; flex-direction:column; gap:16px;">
          <!-- Content Statistics -->
          <div class="card" style="height:max-content;">
            <div class="card-hdr">
              <div class="card-title">Content Statistics (Last 30 Days)</div>
              <a href="#" style="font-size:0.72rem;font-weight:700;color:var(--primary);text-decoration:none;">View Report</a>
            </div>
            <div style="padding:14px 18px;">
              <div class="stat-list">
                <div class="stat-item">
                  <div class="stat-icon"><i class="fa-regular fa-eye"></i></div>
                  <div class="stat-details">
                    <div class="stat-name">Total Views</div>
                    <div class="stat-val">26,440</div>
                  </div>
                  <div class="stat-change trend-up">+18.6%</div>
                </div>
                <div class="stat-item">
                  <div class="stat-icon"><i class="fa-solid fa-users"></i></div>
                  <div class="stat-details">
                    <div class="stat-name">Total Users Engaged</div>
                    <div class="stat-val">12,980</div>
                  </div>
                  <div class="stat-change trend-up">+14.2%</div>
                </div>
                <div class="stat-item">
                  <div class="stat-icon"><i class="fa-regular fa-clock"></i></div>
                  <div class="stat-details">
                    <div class="stat-name">Avg. Read Time</div>
                    <div class="stat-val">04:36 mins</div>
                  </div>
                  <div class="stat-change trend-up">+6.8%</div>
                </div>
                <div class="stat-item">
                  <div class="stat-icon"><i class="fa-solid fa-arrow-trend-down"></i></div>
                  <div class="stat-details">
                    <div class="stat-name">Bounce Rate</div>
                    <div class="stat-val">28.4%</div>
                  </div>
                  <div class="stat-change trend-down">-4.3%</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Policy Management -->
          <div class="card" style="height:max-content;">
            <div class="card-hdr">
              <div class="card-title">Policy Management</div>
            </div>
            <div style="padding:14px 18px;">
              <div style="display:flex; justify-content:space-between; align-items:center; background:var(--bg); padding:10px 14px; border-radius:6px; border:1px solid var(--border);">
                <span style="font-size:0.75rem; font-weight:600; color:var(--text-main);">Platform Policy</span>
                <div style="display:flex; gap:12px; font-size:0.9rem;">
                  <i class="fa-regular fa-eye" style="cursor:pointer; color:#0A4BD2;" title="View Policy" onclick="openPolicyViewModal()"></i>
                  <i class="fa-solid fa-pencil" style="cursor:pointer; color:#D97706;" title="Edit Policy" onclick="openPolicyEditModal()"></i>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- CHARTS SPLIT CARD PLACEHOLDERS -->
      <div class="two-cols-charts">
        <div class="card">
          <div class="card-hdr"><div class="card-title">Content by Status</div></div>
          <div class="chart-box" id="status-chart-placeholder">
            <div style="width:110px;height:110px;border-radius:50%;background:conic-gradient(var(--primary) 70%, #FEB900 70% 88%, #6366F1 88%);"></div>
          </div>
        </div>
        <div class="card">
          <div class="card-hdr"><div class="card-title">Content by Type</div></div>
          <div class="chart-box" id="type-chart-placeholder">
            <div style="width:110px;height:110px;border-radius:50%;background:conic-gradient(#10B981 40%, #065F46 40% 75%, #0EA5E9 75%);"></div>
          </div>
        </div>
      </div>

      <!-- RECENT CONTENT ACTIVITY -->
      <div class="card">
        <div class="card-hdr">
          <div class="card-title">Recent Content Activity</div>
        </div>
        <div style="padding:16px 18px;">
          <ul class="activity-list" id="recent-activity-list">
            <li class="activity-item">
              <span class="activity-txt">&bull; "How to Prepare for a Technical Interview" updated and republished.</span>
              <span class="activity-time">12 May 2025  10:30 AM</span>
            </li>
            <li class="activity-item">
              <span class="activity-txt">&bull; "Job search Strategies for Freshers" submitted for review.</span>
              <span class="activity-time">07 May 2025  09:15 AM</span>
            </li>
            <li class="activity-item">
              <span class="activity-txt">&bull; "Top 10 in - demand IT industry" draft created.</span>
              <span class="activity-time">10 May 2025  04:45 PM</span>
            </li>
            `;
    
    const new_html = html.substring(0, start_idx) + replacement + html.substring(end_idx);
    fs.writeFileSync(path, new_html, 'utf8');
    console.log('HTML Repaired successfully.');
} else {
    console.log('Could not find markers.');
}
