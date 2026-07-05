// admin_helpcenter.js - Interaction scripts for Help Center Dashboard

const viewedArticlesData = [
  { title: 'How to Generate Candidate Report', category: 'Reports', views: '2,450', date: '12 May 2025' },
  { title: 'Managing Portal Content', category: 'Content Management', views: '1,980', date: '10 May 2025' },
  { title: 'Backup and Restore Procedures', category: 'Backup & Recovery', views: '1,560', date: '09 May 2025' },
  { title: 'Bulk Upload Jobs', category: 'Job Management', views: '1,430', date: '08 May 2025' }
];

function renderArticles(data) {
  const tbody = document.getElementById('articles-tbody');
  if (!tbody) return;

  if (data.length === 0) {
    tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted py-4">No articles found.</td></tr>`;
    return;
  }

  tbody.innerHTML = data.map(a => `
    <tr>
      <td class="fw-bold"><a href="#" onclick="openArticle('${a.title}')" class="text-decoration-none text-dark">${a.title}</a></td>
      <td>${a.category}</td>
      <td>${a.views}</td>
      <td>${a.date}</td>
    </tr>
  `).join('');
}

function filterHelpArticles() {
  const query = document.getElementById('help-search-input').value.toLowerCase();
  
  // 1. Filter the table data
  const filtered = viewedArticlesData.filter(a => 
    a.title.toLowerCase().includes(query) || a.category.toLowerCase().includes(query)
  );
  renderArticles(filtered);

  // 2. Filter Help Modules (.help-card)
  const helpCards = document.querySelectorAll('.help-card');
  helpCards.forEach(card => {
    if (card.textContent.toLowerCase().includes(query)) {
      card.style.display = 'block';
    } else {
      card.style.display = 'none';
    }
  });

  // 3. Filter Quick Tutorials
  // The structure is roughly a div containing .tut-title and .tut-desc. We can just target the direct wrapper or just hide the flex item if there's a specific class.
  // In the HTML, tutorials are just direct children of .help-grid. We can target .help-grid > div that are NOT .help-card (or just loop through them).
  const tutGrids = document.querySelectorAll('.help-grid');
  // The second grid is the tutorial grid
  if (tutGrids.length > 1) {
    const tutItems = tutGrids[1].children;
    Array.from(tutItems).forEach(item => {
      if (item.textContent.toLowerCase().includes(query)) {
        item.style.display = 'block';
      } else {
        item.style.display = 'none';
      }
    });
  }

  // 4. Filter FAQs
  const faqs = document.querySelectorAll('.faq-item-btn');
  faqs.forEach(faq => {
    if (faq.textContent.toLowerCase().includes(query)) {
      faq.style.display = 'flex';
    } else {
      faq.style.display = 'none';
    }
  });
}

function quickSearch(tag) {
  document.getElementById('help-search-input').value = tag;
  filterHelpArticles();
  showToast(`🔍 Showing results for: ${tag}`, '#0A4BD2');
}
const guidedTutorialsData = {
  'How to create a Job': [
    '1. Go to Job Management and click "+ Create Job".',
    '2. Enter the Job Title, Department, and Employment Type.',
    '3. Write the Job Description and list the key Requirements.',
    '4. Set the Compensation range and target location.',
    '5. Click "Publish" to immediately post the job online.'
  ],
  'How to edit job': [
    '1. Locate the specific job in the Job Management table.',
    '2. Click the "Edit" (pencil) icon under the Actions column.',
    '3. Update the necessary fields (e.g., salary, description).',
    '4. Review your changes in the preview panel.',
    '5. Click "Save Changes" to update the live posting.'
  ],
  'How to schedule interview': [
    '1. Navigate to Interview Management.',
    '2. Select a shortlisted candidate from the pipeline list.',
    '3. Click the "Schedule" button on their profile.',
    '4. Select the interviewer, date, and preferred time slot.',
    '5. Click "Send Invite" to automatically email the candidate.'
  ],
  'How to generate Reports': [
    '1. Go to the Reports & Analytics dashboard.',
    '2. Choose a report type (e.g., Time-to-Hire, Active Jobs).',
    '3. Apply specific filters like Date Range or Department.',
    '4. Click "Generate Report" to build the visual data.',
    '5. Click "Export as PDF/CSV" to download the final report.'
  ],
  'How to publish content': [
    '1. Open Content Management.',
    '2. Select the portal page or section you want to modify.',
    '3. Make your text or image changes in the visual editor.',
    '4. Click "Preview" to see how it looks to public users.',
    '5. Click "Publish" to deploy the changes to the live site.'
  ],
  'How to restore Backups': [
    '1. Access the Backup & Recovery center.',
    '2. Scroll down to the Recent Backups table.',
    '3. Identify the successful backup snapshot you need to restore.',
    '4. Click the circular "Restore" icon on the right side.',
    '5. Type "CONFIRM" in the security prompt to begin the restore.'
  ]
};

function showGuide(guideName) {
  const modal = document.getElementById('steps-modal');
  const title = document.getElementById('steps-modal-title');
  const content = document.getElementById('steps-modal-content');
  
  if (modal && title && content) {
    title.textContent = guideName;
    const steps = guidedTutorialsData[guideName] || ['Detailed steps for this guide are currently being written.'];
    
    content.innerHTML = steps.map(step => `
      <div style="background:#fff; padding:12px 16px; border:1px solid #E2E8F0; border-radius:8px; font-size:0.8rem; font-weight:600; color:#1E293B;">
        ${step}
      </div>
    `).join('');
    
    // Support dark mode manually
    if (document.body.classList.contains('dark-mode')) {
      const stepDivs = content.querySelectorAll('div');
      stepDivs.forEach(div => {
        div.style.background = '#1E1E1E';
        div.style.borderColor = '#333';
        div.style.color = '#E2E8F0';
      });
    }

    modal.style.display = 'flex';
  }
}

const faqData = {
  'How do I create a new job?': 'Navigate to Job Management, click "+ Create Job", and follow the wizard to publish a new job to the portal.',
  'How do I generate a report?': 'Navigate to Reports & Analytics, select the desired report template, apply your date filters, and click "Generate Report".',
  'How do I restore a backup?': 'Go to Backup & Recovery, find a valid backup log in the table, click the restore icon, and confirm the action.',
  'How do I publish content changes?': 'In Content Management, make your text or image edits using the visual editor, preview them, and then click "Publish Changes".',
  'How do I manage notification templates?': 'In Notifications, select "Manage Templates", create or edit an SMS/Email template, and save it for future campaigns.'
};

const contentMagData = [
  { action: 'Create Content', adminHasAccess: true },
  { action: 'Edit Content', adminHasAccess: false },
  { action: 'Delete Content', adminHasAccess: true }
];

function initDynamicSections() {
  // Render Most Viewed Articles
  renderArticles(viewedArticlesData);

  // Render FAQs
  const faqContainer = document.getElementById('faq-container');
  if (faqContainer) {
    faqContainer.innerHTML = Object.keys(faqData).map(question => 
      `<button class="faq-item-btn" onclick="openFAQ('${question}')">${question}</button>`
    ).join('');
  }

  // Render Content Management Help
  const contentMagTbody = document.getElementById('content-mag-tbody');
  if (contentMagTbody) {
    contentMagTbody.innerHTML = contentMagData.map(item => `
      <tr>
        <td>${item.action}</td>
        <td>${item.adminHasAccess ? '<i class="fa-solid fa-check text-success"></i>' : '&mdash;'}</td>
      </tr>
    `).join('');
  }
}

// Call on load
document.addEventListener('DOMContentLoaded', () => {
  initDynamicSections();
});

function renderPremiumSteps(titleText, subtitleText, infoText, stepsArray) {
  const modal = document.getElementById('steps-modal');
  const title = document.getElementById('steps-modal-title');
  const subtitle = document.getElementById('steps-modal-subtitle');
  const content = document.getElementById('steps-modal-content');
  const infoTextEl = document.getElementById('steps-modal-info-text');
  const infoBox = document.getElementById('steps-modal-info');
  
  if (modal && title && content) {
    title.textContent = titleText;
    subtitle.textContent = subtitleText;
    
    if (infoText) {
      infoTextEl.textContent = infoText;
      infoBox.style.display = 'flex';
    } else {
      infoBox.style.display = 'none';
    }
    
    content.innerHTML = stepsArray.map(step => {
      // Split the step at the first colon if it exists to bold the prefix (e.g. "Add Profile Photo (15%): Click ...")
      let htmlStep = step;
      const colonIdx = step.indexOf(':');
      if (colonIdx !== -1 && colonIdx < 50) { // arbitrary limit so we don't bold a whole sentence
        const boldPart = step.substring(0, colonIdx + 1);
        const restPart = step.substring(colonIdx + 1);
        // also remove the starting number if it exists (e.g., "1. ") since we use <ol>
        const cleanBold = boldPart.replace(/^\d+\.\s*/, '');
        htmlStep = `<strong>${cleanBold}</strong>${restPart}`;
      } else {
        htmlStep = step.replace(/^\d+\.\s*/, '');
      }
      return `<li style="margin-bottom:8px;">${htmlStep}</li>`;
    }).join('');
    
    // Support dark mode manually
    if (document.body.classList.contains('dark-mode')) {
      document.querySelector('.pwd-modal-box').style.background = '#1E1E1E';
      title.style.color = '#E2E8F0';
      subtitle.style.color = '#94A3B8';
      content.style.color = '#cbd5e1';
      document.querySelector('.pwd-modal-footer').style.background = '#1E1E1E';
      document.querySelector('.pwd-modal-footer').style.borderColor = '#333';
      if (infoBox) {
        infoBox.style.background = '#164e63';
        infoBox.style.borderColor = '#0891b2';
        infoTextEl.style.color = '#cffafe';
      }
    } else {
      document.querySelector('.pwd-modal-box').style.background = '#fff';
      title.style.color = '#1E293B';
      subtitle.style.color = '#1E293B';
      content.style.color = '#475569';
      document.querySelector('.pwd-modal-footer').style.background = '#fff';
      document.querySelector('.pwd-modal-footer').style.borderColor = '#E2E8F0';
      if (infoBox) {
        infoBox.style.background = '#E0F8FB';
        infoBox.style.borderColor = '#BCE5EA';
        infoTextEl.style.color = '#0E7490';
      }
    }

    modal.style.display = 'flex';
  }
}

function openFAQ(question) {
  const modal = document.getElementById('steps-modal');
  const title = document.getElementById('steps-modal-title');
  const content = document.getElementById('steps-modal-content');
  
  if (modal && title && content) {
    title.textContent = 'FAQ Answer';
    const answer = faqData[question] || 'No answer available.';
    
    content.innerHTML = `
      <div style="background:#fff; padding:16px; border:1px solid #E2E8F0; border-radius:8px;">
        <h6 style="color:#0A4BD2; font-weight:700; margin-bottom:10px;">${question}</h6>
        <p style="font-size:0.8rem; color:#1E293B; line-height:1.5; margin:0;">${answer}</p>
      </div>
    `;
    
    if (document.body.classList.contains('dark-mode')) {
      const div = content.querySelector('div');
      div.style.background = '#1E1E1E';
      div.style.borderColor = '#333';
      div.querySelector('p').style.color = '#E2E8F0';
    }

    modal.style.display = 'flex';
  }
}

function backupAction(action) {
  showToast(`💾 Backup Help Action: ${action}`, '#0A4BD2');
}

function quickAction(action) {
  showToast(`⚡ Quick Action Triggered: ${action}`, '#D97706');
}

// ──────────────────────────────────────────────────────────────
//  MODULE STEPS MODAL
// ──────────────────────────────────────────────────────────────
const moduleStepsData = {
  'Job Management': [
    '1. Navigate to the Job Management dashboard from the sidebar.',
    '2. Click on "+ Create Job" to open the job wizard.',
    '3. Fill in the required Job Details, Requirements, and Compensation.',
    '4. Preview the Job post to ensure all formatting is correct.',
    '5. Click "Publish" to push the job to the portal live.'
  ],
  'Candidate Management': [
    '1. Open the Candidate Management dashboard.',
    '2. Use the search bar to filter candidates by name, email, or role.',
    '3. Click on a candidate\'s profile to view their detailed application.',
    '4. Move the candidate through stages (e.g., Shortlisted, Interview).',
    '5. Send an automated email using the action dropdown.'
  ],
  'Interview Management': [
    '1. Go to the Interviews section.',
    '2. Select an active candidate from a job pipeline.',
    '3. Click "Schedule Interview" and choose a date and time slot.',
    '4. Assign an interviewer and generate a meeting link.',
    '5. Save to send automatic calendar invites to the candidate.'
  ],
  'Content Management': [
    '1. Navigate to Content Management.',
    '2. Select the page you wish to edit (e.g., Home Page, About Us).',
    '3. Use the visual editor to modify text, images, or layout.',
    '4. Click "Save Draft" to preview the changes.',
    '5. Click "Publish Changes" to push them to the live portal.'
  ],
  'Backup & Recovery': [
    '1. Open the Backup & Recovery dashboard.',
    '2. To create a backup, click "Create Backup" and choose Full or Incremental.',
    '3. To restore, find a successful log in the table.',
    '4. Click the "Restore" action icon on that row.',
    '5. Confirm the warning prompt to initiate system restoration.'
  ],
  'Notifications': [
    '1. Go to Notification Management.',
    '2. Select "Create Campaign" or choose a predefined template.',
    '3. Select the target audience (e.g., All Candidates, Specific Roles).',
    '4. Write your notification content (SMS or Email).',
    '5. Schedule the delivery time or click "Send Now".'
  ],
  'Report Management': [
    '1. Navigate to the Reports & Analytics dashboard.',
    '2. Select a report category (e.g., Hiring Velocity, Platform Usage).',
    '3. Adjust the date filters to your desired range.',
    '4. Click "Generate Report" to view the visual charts.',
    '5. Click "Export" to download the data as PDF or CSV.'
  ]
};

function openModuleSteps(moduleName) {
  const modal = document.getElementById('steps-modal');
  const title = document.getElementById('steps-modal-title');
  const content = document.getElementById('steps-modal-content');
  
  if (modal && title && content) {
    title.textContent = moduleName + ' Steps';
    const steps = moduleStepsData[moduleName] || ['No steps available for this module.'];
    
    content.innerHTML = steps.map(step => `
      <div style="background:#fff; padding:12px 16px; border:1px solid #E2E8F0; border-radius:8px; font-size:0.8rem; font-weight:600; color:#1E293B;">
        ${step}
      </div>
    `).join('');
    
    // Support dark mode manually if body has dark-mode (since we injected styles inline, we need to adapt)
    if (document.body.classList.contains('dark-mode')) {
      const stepDivs = content.querySelectorAll('div');
      stepDivs.forEach(div => {
        div.style.background = '#1E1E1E';
        div.style.borderColor = '#333';
        div.style.color = '#E2E8F0';
      });
    }

    modal.style.display = 'flex';
  }
}

function closeModuleSteps() {
  const modal = document.getElementById('steps-modal');
  if (modal) modal.style.display = 'none';
}

// ──────────────────────────────────────────────────────────────
// CONTACT SUPPORT LOGIC
// ──────────────────────────────────────────────────────────────
const contactSupportSteps = {
  'Email': ['Open Email Client: Click the button to open your default app.', 'Draft Message: Describe your technical issue clearly.', 'Attach Logs: Include any screenshot or error code.', 'Send to Support: Email reaches our L2 engineering team.', 'Await Reply: Standard response time is 2-4 hours.'],
  'Chat': ['Open Chat Widget: A bubble will appear on the bottom right.', 'Enter Details: Type your name and Admin ID.', 'Connect to Agent: You will be routed to the next available specialist.', 'Explain Issue: Chat in real-time.', 'Save Transcript: You can download the log after the session.'],
  'SMS': ['Verify Phone: Ensure your mobile number is on file.', 'Text Keyword: Send "HELP" to 555-0192.', 'Automated Triage: A bot will classify your issue.', 'Agent Callback: An agent will call you within 15 minutes.', 'Resolve: The issue is resolved over the phone.'],
  'All Options': ['Review SLAs: Check response times for each tier.', 'Select Channel: Choose Email (non-urgent) or Chat (urgent).', 'Prepare Data: Have your Admin ID ready.', 'Contact Account Manager: For billing issues, use direct line.', 'Submit Ticket: Track status in the portal.']
};

function contactSupport(channel) {
  const steps = contactSupportSteps[channel] || ['1. Follow standard procedures.'];
  renderPremiumSteps(
    channel + ' Support Guide',
    `Follow these steps to connect with ${channel} support:`,
    'Having your Admin ID ready speeds up resolution by 50%.',
    steps
  );
}

function openArticle(title) {
  showToast(`Opening article details: ${title}`, '#0A4BD2');
}

function showToast(msg, bg) {
  const el = document.getElementById('toast');
  if (!el) return;
  el.textContent = msg;
  el.style.background = bg || '#15803D';
  el.classList.add('show');
  setTimeout(() => el.classList.remove('show'), 3000);
}

// ──────────────────────────────────────────────────────────────
// PREMIUM MODAL RENDER LOGIC
// ──────────────────────────────────────────────────────────────
function renderPremiumSteps(titleText, subtitleText, infoText, stepsArray) {
  const modal = document.getElementById('steps-modal');
  const title = document.getElementById('steps-modal-title');
  const subtitle = document.getElementById('steps-modal-subtitle');
  const content = document.getElementById('steps-modal-content');
  const infoTextEl = document.getElementById('steps-modal-info-text');
  const infoBox = document.getElementById('steps-modal-info');
  
  if (modal && title && content) {
    title.textContent = titleText;
    subtitle.textContent = subtitleText;
    
    if (infoText) {
      infoTextEl.textContent = infoText;
      infoBox.style.display = 'flex';
    } else {
      infoBox.style.display = 'none';
    }
    
    content.innerHTML = stepsArray.map(step => {
      let htmlStep = step;
      const colonIdx = step.indexOf(':');
      if (colonIdx !== -1 && colonIdx < 50) {
        const boldPart = step.substring(0, colonIdx + 1);
        const restPart = step.substring(colonIdx + 1);
        const cleanBold = boldPart.replace(/^\d+\.\s*/, '');
        htmlStep = `<strong>${cleanBold}</strong>${restPart}`;
      } else {
        htmlStep = step.replace(/^\d+\.\s*/, '');
      }
      return `<li style="margin-bottom:8px;">${htmlStep}</li>`;
    }).join('');
    
    // Support dark mode manually
    if (document.body.classList.contains('dark-mode')) {
      document.querySelector('.pwd-modal-box').style.background = '#1E1E1E';
      title.style.color = '#E2E8F0';
      subtitle.style.color = '#94A3B8';
      content.style.color = '#cbd5e1';
      document.querySelector('.pwd-modal-footer').style.background = '#1E1E1E';
      document.querySelector('.pwd-modal-footer').style.borderColor = '#333';
      if (infoBox) {
        infoBox.style.background = '#164e63';
        infoBox.style.borderColor = '#0891b2';
        infoTextEl.style.color = '#cffafe';
      }
    } else {
      document.querySelector('.pwd-modal-box').style.background = '#fff';
      title.style.color = '#1E293B';
      subtitle.style.color = '#1E293B';
      content.style.color = '#475569';
      document.querySelector('.pwd-modal-footer').style.background = '#fff';
      document.querySelector('.pwd-modal-footer').style.borderColor = '#E2E8F0';
      if (infoBox) {
        infoBox.style.background = '#E0F8FB';
        infoBox.style.borderColor = '#BCE5EA';
        infoTextEl.style.color = '#0E7490';
      }
    }

    modal.style.display = 'flex';
  }
}

// Rewire openModuleSteps to use the new premium render
function openModuleSteps(moduleName) {
  const steps = moduleStepsData[moduleName] || ['No steps available for this module.'];
  renderPremiumSteps(
    moduleName + ' Guide',
    'Follow these clear guide steps to manage your module:',
    'Completing these setups ensures your portal operates securely.',
    steps
  );
}

// ──────────────────────────────────────────────────────────────
// REPORTS HELP CENTER LOGIC
// ──────────────────────────────────────────────────────────────
const reportActionSteps = {
  'Create Report': ['Choose Report Type: Select from Hiring, Application, or Custom.', 'Set Date Filters: Pick your timeline.', 'Select Columns: Choose the data points you want.', 'Generate: Click generate to process data.', 'Save: Save to your dashboard.'],
  'Export PDF': ['Open Report: Navigate to any generated report.', 'Format Options: Select A4 or Letter.', 'Include Charts: Check the box to embed visual graphs.', 'Click Export: Wait for the download to finish.', 'Print or Share: The file is ready.'],
  'Export Excel': ['Open Report: Go to your data table.', 'Data Formatting: Ensure date ranges are valid.', 'Click Export Excel: Download the raw .xlsx file.', 'Enable Macros: If applicable for custom calculations.', 'Analyze: Open in Excel.'],
  'Export CSV': ['Open Data Table: Navigate to candidate or job lists.', 'Filter Data: Ensure you have the exact rows needed.', 'Click CSV: It will instantly download.', 'Import: Use this file for external CRMs.', 'Check Encoding: Uses UTF-8 by default.'],
  'Preview Report': ['Generate Data: Wait for the system to process.', 'Click Preview: A modal will appear.', 'Check Columns: Ensure all data fits.', 'Toggle Charts: Switch between visual and tabular view.', 'Close Preview: Click the X to exit.'],
  'Schedule Report': ['Open Report Settings: Find the clock icon.', 'Select Frequency: Daily, Weekly, or Monthly.', 'Add Emails: Type the recipients.', 'Set Format: PDF or CSV.', 'Save Schedule: The report runs automatically.'],
  'Email Reports': ['Select Report: Check the box next to a report.', 'Click Email: A popup appears.', 'Add Addresses: Comma-separated list.', 'Add Message: Optional context.', 'Send: Dispatched immediately.'],
  'Report Template': ['Create Blank Report: Add your standard columns.', 'Click Save as Template: Name it.', 'Access Later: Found in the Templates tab.', 'Share: Allow other admins to use it.', 'Update: Edit the template at any time.'],
  'Analytical Report': ['Go to Analytics Hub: High-level metrics.', 'Choose Dimension: Jobs vs Candidates.', 'Set Timeframe: Year over Year.', 'View Graphs: Bar charts and line charts.', 'Export: Download the visual suite.'],
  'Hiring Report': ['Select Hiring Hub: Go to reports.', 'Filter by Department: Engineering, HR, etc.', 'Check Time-to-hire: Key metric.', 'View Pipeline: See stages.', 'Export: Send to executives.'],
  'Application Report': ['Select Applications: In reports tab.', 'Filter by Job: Choose a specific position.', 'Check Sources: See where they applied from.', 'Check Drop-offs: View incomplete apps.', 'Export CSV: For deep dive.'],
  'Custom Reports': ['Click Custom Builder: Start from scratch.', 'Drag Columns: Build your view.', 'Apply Math: Sum or average columns.', 'Name Report: Save for later.', 'Run: Execute the query.']
};

function reportAction(action) {
  const steps = reportActionSteps[action] || ['1. Follow standard procedures.'];
  renderPremiumSteps(
    action + ' Guide',
    `Follow these steps to safely execute ${action}:`,
    'Proper reporting gives executives 100% visibility into hiring.',
    steps
  );
}

// ──────────────────────────────────────────────────────────────
// BACKUP & RECOVERY HELP CENTER LOGIC
// ──────────────────────────────────────────────────────────────
const backupActionSteps = {
  'Immediate Backup': ['Navigate to Backup Center: Open the dashboard.', 'Select Full Backup: Ensure all data is captured.', 'Click Run Now: Start the process immediately.', 'Monitor Progress: Wait for the bar to reach 100%.', 'Verify: Check the logs for success.'],
  'Scheduled Backup': ['Open Backup Settings: Click the gear icon.', 'Set Cron Schedule: Pick daily at midnight.', 'Select Scope: Database or Filesystem.', 'Save Settings: The system will run it automatically.', 'Test: Trigger a manual run to ensure the schedule works.'],
  'Restore Backup': ['Find Snapshot: Locate a green successful backup.', 'Click Restore: Hit the icon on the right.', 'Confirm Warning: Type "CONFIRM" to proceed.', 'Wait: The system will go into maintenance mode.', 'Validate: Check the frontend portal to ensure data is back.'],
  'Backup History': ['Go to Logs: Click the History tab.', 'Filter by Status: Show only failed or successful.', 'View Details: Click any row to see metadata.', 'Download Logs: Export the text file.', 'Analyze: Give to engineering if there are errors.'],
  'Backup Status': ['Check Dashboard: Look at the main pie chart.', 'View Storage: Ensure you have enough disk space.', 'Check Last Run: Verify it ran within 24 hours.', 'Check Integrity: Run a checksum validation.', 'Clear Old: Delete backups older than 90 days.'],
  'Retention Policy': ['Open Settings: Go to Data Management.', 'Set Days: Enter "30" or "90" days.', 'Select Archival: Choose Cold Storage if needed.', 'Apply: Save the policy.', 'Verify: Old backups should automatically purge.'],
  'Export Backup': ['Locate Backup: Find it in the table.', 'Click Export: Download the .zip file.', 'Decrypt: Enter the admin password.', 'Store Locally: Keep it in a secure drive.', 'Do Not Share: Keep data confidential.'],
  'Recovery Logs': ['Open Audits: Navigate to the security tab.', 'Search "Restore": Filter by action.', 'View Admin: See who triggered it.', 'Check Timestamp: Note the exact time.', 'Export PDF: Save for compliance.'],
  'Disaster Recovery': ['Assess Damage: Identify the failure point.', 'Communicate: Alert the team.', 'Select Last Good State: Find a stable backup.', 'Trigger Full Restore: This will overwrite data.', 'Run Health Checks: Ensure system is stable.']
};

function backupAction(action) {
  const steps = backupActionSteps[action] || ['1. Follow standard procedures.'];
  renderPremiumSteps(
    action + ' Guide',
    `Follow these steps to safely execute ${action}:`,
    'Proper backup protocols prevent critical data loss.',
    steps
  );
}

document.addEventListener('DOMContentLoaded', () => {
  if (typeof initDynamicSections === 'function') {
    initDynamicSections();
  }
});

// ──────────────────────────────────────────────────────────────
// QUICK ACTIONS LOGIC
// ──────────────────────────────────────────────────────────────
const quickActionSteps = {
  'Create Articles': ['Open Content Management: Go to the Articles tab.', 'Click New Article: Starts the authoring wizard.', 'Write Content: Use the rich text editor.', 'Add Tags: Categorize the article for search.', 'Publish: Make it visible on the portal.'],
  'Create FAQs': ['Go to FAQ Section: Inside Content Management.', 'Click Add FAQ: Enter the question.', 'Write Answer: Provide a concise response.', 'Assign Category: e.g. Billing or Onboarding.', 'Save: Pushes it to the live Help Center.'],
  'Create Guide': ['Open Tutorials Module: Start a new guide.', 'Name Guide: Keep it action-oriented.', 'Add Steps: Use the step-by-step builder.', 'Attach Images: Visuals help users understand.', 'Publish Guide: It will appear on the dashboard.'],
  'Generate Report': ['Open Reporting Engine: Found in the sidebar.', 'Select Dataset: Choose users or activity.', 'Set Date Filter: e.g. Last 30 Days.', 'Click Generate: Wait for data compilation.', 'Export: Download as PDF or Excel.'],
  'Backup Now': ['Navigate to Backups: Open the Backup Center.', 'Click Run Immediate Backup: Start the manual process.', 'Wait for Completion: Do not close the window.', 'Verify Snapshot: Check the history table.', 'Store Securely: Optionally download the archive.']
};

function quickAction(action) {
  const steps = quickActionSteps[action] || ['1. Follow standard procedures.'];
  renderPremiumSteps(
    action + ' Walkthrough',
    `Follow these steps to complete the ${action} action:`,
    'Quick actions save you time by navigating directly to key features.',
    steps
  );
}
