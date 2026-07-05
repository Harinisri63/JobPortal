// reports.js — Reports & Analytics with View / Download / Edit modal logic

// ── LIVE SOURCE DATA (shared via localStorage with other modules) ─
function getLiveSourceData(source) {
  const colors = ['#3B82F6','#22C55E','#F59E0B','#EF4444','#8B5CF6','#10B981','#F472B6','#60A5FA','#FBBF24','#34D399'];

  if (source === 'Jobs') {
    const defaultJobs = [
      { id:'JOB-1021', title:'Senior DevOps Engineer',  dept:'Engineering', loc:'Bangalore', type:'Full Time', exp:'5-8 yrs', apps:34, status:'Active',  posted:'12 May 2025' },
      { id:'JOB-1015', title:'React Developer',          dept:'Engineering', loc:'Remote',    type:'Full Time', exp:'2-4 yrs', apps:28, status:'Active',  posted:'11 May 2025' },
      { id:'JOB-1008', title:'Product Manager',          dept:'Product',     loc:'Bangalore', type:'Full Time', exp:'3-6 yrs', apps:52, status:'Active',  posted:'10 May 2025' },
      { id:'JOB-1002', title:'UI/UX Designer',           dept:'Design',      loc:'Pune',      type:'Full Time', exp:'2-4 yrs', apps:21, status:'Active',  posted:'09 May 2025' },
      { id:'JOB-0991', title:'Data Analyst',             dept:'Analytics',   loc:'Hyderabad', type:'Full Time', exp:'1-3 yrs', apps:18, status:'Active',  posted:'08 May 2025' },
      { id:'JOB-0982', title:'Backend Developer',        dept:'Engineering', loc:'Remote',    type:'Full Time', exp:'3-5 yrs', apps:26, status:'Closed',  posted:'07 May 2025' },
      { id:'JOB-0975', title:'HR Executive',             dept:'HR',          loc:'Mumbai',    type:'Full Time', exp:'1-2 yrs', apps:14, status:'Active',  posted:'07 May 2025' },
      { id:'JOB-0961', title:'Full Stack Developer',     dept:'Engineering', loc:'Bangalore', type:'Full Time', exp:'4-7 yrs', apps:41, status:'Active',  posted:'06 May 2025' },
    ];
    const stored = JSON.parse(localStorage.getItem('jobsData') || 'null');
    const jobs   = (stored && stored.length) ? stored : defaultJobs;

    // Count by department for pie chart
    const deptCount = {};
    jobs.forEach(j => { deptCount[j.dept] = (deptCount[j.dept] || 0) + 1; });

    return {
      description: 'Live job listings pulled from Job Management.',
      columns: ['Job ID', 'Title', 'Department', 'Location', 'Type', 'Experience', 'Applications', 'Status', 'Posted'],
      rows: jobs.map(j => [j.id, j.title, j.dept, j.loc, j.type, j.exp, String(j.apps || 0), j.status, j.posted]),
      chartLabel: 'Jobs by Department',
      chartData: Object.entries(deptCount).map(([k, v], i) => ({ label: k, value: v, color: colors[i % colors.length] }))
    };
  }

  if (source === 'Candidates') {
    const candidates = [
      { name:'Arjun Mehta',   email:'arjun.mehta@gmail.com',   phone:'+91 9876543210', skills:'DevOps, AWS',       exp:'6 yrs', role:'Senior DevOps',  source:'LinkedIn',   stage:'Shortlisted', date:'18 May 2025' },
      { name:'Priya Sharma',  email:'priya.sharma@gmail.com',  phone:'+91 9845012345', skills:'React, JS',         exp:'7 yrs', role:'React Developer', source:'Referral',   stage:'Under Review',date:'19 May 2025' },
      { name:'Rohit Nair',    email:'rohit.nair@gmail.com',    phone:'+91 9823456780', skills:'Node.js, MongoDB',  exp:'5 yrs', role:'Backend Dev',    source:'Naukri',     stage:'Rejected',    date:'20 May 2025' },
      { name:'Sneha Iyer',    email:'sneha.iyer@gmail.com',    phone:'+91 9900112233', skills:'Figma, UX',         exp:'3 yrs', role:'UI/UX Designer', source:'LinkedIn',   stage:'Shortlisted', date:'20 May 2025' },
      { name:'Divya Reddy',   email:'divya.reddy@gmail.com',   phone:'+91 9654321098', skills:'Python, SQL',       exp:'3 yrs', role:'Data Analyst',   source:'Job Portal', stage:'Shortlisted', date:'17 May 2025' },
      { name:'Aakash Singh',  email:'aakash.singh@gmail.com',  phone:'+91 9987654321', skills:'React, TypeScript', exp:'2.5 yrs',role:'React Developer',source:'Naukri',     stage:'Under Review',date:'18 May 2025' },
      { name:'Vikram Joshi',  email:'vikram.joshi@gmail.com',  phone:'+91 9811223344', skills:'Product, Agile',    exp:'5 yrs', role:'Product Manager', source:'LinkedIn',   stage:'Shortlisted', date:'14 May 2025' },
    ];
    const stageCount = {};
    candidates.forEach(c => { stageCount[c.stage] = (stageCount[c.stage] || 0) + 1; });
    return {
      description: 'Candidate details pulled from applicant records.',
      columns: ['Name', 'Email', 'Phone', 'Skills', 'Experience', 'Applied Role', 'Source', 'Stage', 'Applied Date'],
      rows: candidates.map(c => [c.name, c.email, c.phone, c.skills, c.exp, c.role, c.source, c.stage, c.date]),
      chartLabel: 'Candidates by Stage',
      chartData: Object.entries(stageCount).map(([k, v], i) => ({ label: k, value: v, color: colors[i % colors.length] }))
    };
  }

  if (source === 'Applications') {
    const apps = [
      { applicant:'Arjun Mehta',  job:'Senior DevOps Engineer',  dept:'Engineering', applied:'18 May 2025', stage:'Shortlisted',  status:'Active',   recruiter:'Kiran',   updated:'20 May 2025' },
      { applicant:'Priya Sharma', job:'React Developer',          dept:'Engineering', applied:'19 May 2025', stage:'Under Review', status:'Active',   recruiter:'Anjali',  updated:'20 May 2025' },
      { applicant:'Rohit Nair',   job:'Senior DevOps Engineer',   dept:'Engineering', applied:'20 May 2025', stage:'Rejected',     status:'Closed',   recruiter:'Kiran',   updated:'21 May 2025' },
      { applicant:'Sneha Iyer',   job:'UI/UX Designer',           dept:'Design',      applied:'20 May 2025', stage:'Shortlisted',  status:'Active',   recruiter:'Pradeep', updated:'22 May 2025' },
      { applicant:'Divya Reddy',  job:'React Developer',          dept:'Engineering', applied:'17 May 2025', stage:'Shortlisted',  status:'Active',   recruiter:'Anjali',  updated:'19 May 2025' },
      { applicant:'Vikram Joshi', job:'Product Manager',          dept:'Product',     applied:'14 May 2025', stage:'Shortlisted',  status:'Active',   recruiter:'Pradeep', updated:'16 May 2025' },
    ];
    const stageCount = {};
    apps.forEach(a => { stageCount[a.stage] = (stageCount[a.stage] || 0) + 1; });
    return {
      description: 'Application records across all active job listings.',
      columns: ['Applicant', 'Job Title', 'Department', 'Applied On', 'Stage', 'Status', 'Recruiter', 'Last Updated'],
      rows: apps.map(a => [a.applicant, a.job, a.dept, a.applied, a.stage, a.status, a.recruiter, a.updated]),
      chartLabel: 'Applications by Stage',
      chartData: Object.entries(stageCount).map(([k, v], i) => ({ label: k, value: v, color: colors[i % colors.length] }))
    };
  }

  if (source === 'Interviews') {
    const items = [
      { candidate:'Arjun Mehta',  interviewer:'Rajan Sir',    round:'Technical', date:'22 May 2025', score:'8.5/10', result:'Passed',   mode:'Online',  feedback:'Strong DevOps skills' },
      { candidate:'Priya Sharma', interviewer:'Anjali',       round:'HR',        date:'23 May 2025', score:'7.0/10', result:'Passed',   mode:'Online',  feedback:'Good communication' },
      { candidate:'Rohit Nair',   interviewer:'Rajan Sir',    round:'Technical', date:'22 May 2025', score:'4.5/10', result:'Failed',   mode:'Online',  feedback:'Needs improvement' },
      { candidate:'Sneha Iyer',   interviewer:'Pradeep',      round:'Final',     date:'24 May 2025', score:'9.0/10', result:'Selected', mode:'In-person',feedback:'Excellent UX portfolio' },
      { candidate:'Vikram Joshi', interviewer:'HR Team',      round:'HR',        date:'20 May 2025', score:'6.5/10', result:'On Hold',  mode:'Online',  feedback:'Salary negotiation pending' },
    ];
    const resultCount = {};
    items.forEach(i => { resultCount[i.result] = (resultCount[i.result] || 0) + 1; });
    return {
      description: 'Interview records with scores and outcomes.',
      columns: ['Candidate', 'Interviewer', 'Round', 'Date', 'Score', 'Result', 'Mode', 'Feedback'],
      rows: items.map(i => [i.candidate, i.interviewer, i.round, i.date, i.score, i.result, i.mode, i.feedback]),
      chartLabel: 'Interview Results',
      chartData: Object.entries(resultCount).map(([k, v], i) => ({ label: k, value: v, color: colors[i % colors.length] }))
    };
  }

  if (source === 'Users') {
    const stored = JSON.parse(localStorage.getItem('usersData') || 'null');
    const users = stored && stored.length ? stored : [
      { name:'Admin User',    role:'Admin',     email:'admin@portal.com',     status:'Active',   joined:'01 Jan 2025', lastLogin:'Today',         dept:'IT' },
      { name:'Kiran Rao',     role:'Recruiter', email:'kiran@portal.com',     status:'Active',   joined:'15 Feb 2025', lastLogin:'Yesterday',     dept:'HR' },
      { name:'Anjali Mehta',  role:'Recruiter', email:'anjali@portal.com',    status:'Active',   joined:'20 Feb 2025', lastLogin:'2 days ago',    dept:'HR' },
      { name:'Pradeep S.',    role:'Manager',   email:'pradeep@portal.com',   status:'Active',   joined:'10 Mar 2025', lastLogin:'Today',         dept:'Operations' },
      { name:'Sneha Jain',    role:'Recruiter', email:'sneha@portal.com',     status:'Inactive', joined:'05 Apr 2025', lastLogin:'Last week',     dept:'HR' },
      { name:'Rahul Tiwari',  role:'Viewer',    email:'rahul@portal.com',     status:'Active',   joined:'01 May 2025', lastLogin:'3 days ago',    dept:'Finance' },
    ];
    const roleCount = {};
    users.forEach(u => { const r = u.role || u.userRole || 'User'; roleCount[r] = (roleCount[r] || 0) + 1; });
    return {
      description: 'User accounts from the User Management module.',
      columns: ['Full Name', 'Role', 'Email', 'Status', 'Joined Date', 'Last Login', 'Department'],
      rows: users.map(u => [u.name || u.fullName || '—', u.role || u.userRole || '—', u.email || '—', u.status || '—', u.joined || u.joinedDate || '—', u.lastLogin || '—', u.dept || u.department || '—']),
      chartLabel: 'Users by Role',
      chartData: Object.entries(roleCount).map(([k, v], i) => ({ label: k, value: v, color: colors[i % colors.length] }))
    };
  }

  // Generic fallback for Notifications, Reports, Content, Backups
  const genericData = {
    Notifications: {
      desc: 'Notification logs from the system.',
      cols: ['Title', 'Type', 'Sent To', 'Sent On', 'Status', 'Channel'],
      rows: [
        ['New Job Alert',        'Info',    'All Users',     '01 Jun 2025', 'Delivered', 'Email'],
        ['Interview Scheduled',  'Reminder','Arjun Mehta',   '02 Jun 2025', 'Delivered', 'SMS'],
        ['Application Update',   'Info',    'Priya Sharma',  '03 Jun 2025', 'Delivered', 'Email'],
        ['Offer Letter Ready',   'Action',  'Sneha Iyer',    '04 Jun 2025', 'Pending',   'Email'],
        ['Password Reset',       'Security','Rahul Tiwari',  '05 Jun 2025', 'Delivered', 'Email'],
      ],
      chartField: 'Status',
      chartGroups: { 'Delivered': 4, 'Pending': 1 }
    },
    Content: {
      desc: 'Content items from the Content Management module.',
      cols: ['Title', 'Type', 'Category', 'Author', 'Status', 'Published Date', 'Views'],
      rows: [
        ['How to Apply',          'Article', 'Guide',   'Admin',   'Published', '01 May 2025', '1,240'],
        ['Resume Tips',           'Blog',    'Career',  'Editor',  'Published', '05 May 2025', '980'],
        ['Job Market 2025',       'Report',  'Insights','Admin',   'Draft',     '—',           '0'],
        ['Interview Prep Guide',  'Article', 'Guide',   'Editor',  'Published', '10 May 2025', '2,100'],
        ['Company Culture FAQ',   'FAQ',     'Culture', 'Admin',   'Published', '15 May 2025', '450'],
      ],
      chartField: 'Status',
      chartGroups: { 'Published': 4, 'Draft': 1 }
    },
    Backups: {
      desc: 'System backup activity log.',
      cols: ['Backup ID', 'Type', 'Size (MB)', 'Initiated By', 'Date', 'Status'],
      rows: [
        ['BK-20250620', 'Full Backup',    '1,240', 'System', '20 Jun 2025', 'Success'],
        ['BK-20250613', 'Incremental',    '320',   'System', '13 Jun 2025', 'Success'],
        ['BK-20250606', 'Full Backup',    '1,198', 'Admin',  '06 Jun 2025', 'Success'],
        ['BK-20250530', 'Incremental',    '285',   'System', '30 May 2025', 'Failed'],
        ['BK-20250523', 'Differential',   '540',   'System', '23 May 2025', 'Success'],
      ],
      chartField: 'Status',
      chartGroups: { 'Success': 4, 'Failed': 1 }
    },
    Reports: {
      desc: 'Report library metadata.',
      cols: ['Report Name', 'Category', 'Created By', 'Last Generated', 'Schedule', 'Format', 'Status'],
      rows: reportLibraryData.map(r => [r.name, r.category, r.createdBy, r.lastGenerated, r.schedule, r.format, r.status]),
      chartField: 'Status',
      chartGroups: { 'Active': reportLibraryData.filter(r => r.status === 'Active').length, 'Inactive': reportLibraryData.filter(r => r.status === 'Inactive').length }
    }
  };

  const g = genericData[source];
  if (g) {
    return {
      description: g.desc,
      columns: g.cols,
      rows: g.rows,
      chartLabel: `${source} by ${g.chartField}`,
      chartData: Object.entries(g.chartGroups).map(([k, v], i) => ({ label: k, value: v, color: colors[i % colors.length] }))
    };
  }

  return null;
}


// ── REPORT DATA ─────────────────────────────────────────────────
const reportLibraryData = [
  { name: 'Hiring Summary Report',           category: 'Recruitment',  createdBy: 'Admin',     lastGenerated: 'Today, 10:24',   schedule: 'Daily',   format: 'PDF',   status: 'Active'   },
  { name: 'Candidate Pipeline Report',       category: 'Candidate',    createdBy: 'Manager',   lastGenerated: 'Today, 08:15',   schedule: 'Weekly',  format: 'Excel', status: 'Active'   },
  { name: 'Job Performance Report',          category: 'Job Perf',     createdBy: 'Recruiter', lastGenerated: 'Yesterday',      schedule: 'Monthly', format: 'CSV',   status: 'Active'   },
  { name: 'Interview Analysis Report',       category: 'Interview',    createdBy: 'Admin',     lastGenerated: '2 days ago',     schedule: 'Daily',   format: 'PDF',   status: 'Active'   },
  { name: 'Source Effectiveness Report',     category: 'Recruitment',  createdBy: 'Manager',   lastGenerated: '3 days ago',     schedule: 'None',    format: 'Excel', status: 'Inactive' },
  { name: 'Recruitment Productivity Report', category: 'Performance',  createdBy: 'Recruiter', lastGenerated: 'Last week',      schedule: 'Weekly',  format: 'PDF',   status: 'Active'   },
  { name: 'Compliance Audit Report',         category: 'Audit',        createdBy: 'Admin',     lastGenerated: '10 days ago',    schedule: 'None',    format: 'CSV',   status: 'Active'   },
  { name: 'Backup Activity Report',          category: 'System',       createdBy: 'System',    lastGenerated: '12 days ago',    schedule: 'Daily',   format: 'Excel', status: 'Active'   },
  { name: 'Application Status Report',       category: 'Applications', createdBy: 'Manager',   lastGenerated: '2 weeks ago',    schedule: 'Monthly', format: 'PDF',   status: 'Active'   }
];

// ── PER-REPORT DETAILS ────────────────────────────────────────────
const reportDetails = {
  'Hiring Summary Report': {
    description: 'Overview of all hiring activities pulled from User Management.',
    columns: ['Candidate Name', 'Role Applied', 'Department', 'Status', 'Applied On'],
    rows: [
      ['Aisha Kumar',    'Frontend Developer', 'Engineering', 'Hired',      '10 Jun 2025'],
      ['Ravi Shankar',   'Data Analyst',        'Analytics',   'In Review',  '12 Jun 2025'],
      ['Priya Mehta',    'UX Designer',         'Design',      'Shortlisted','14 Jun 2025'],
      ['Arjun Nair',     'Backend Engineer',    'Engineering', 'Rejected',   '15 Jun 2025'],
      ['Divya Pillai',   'HR Specialist',       'HR',          'Hired',      '16 Jun 2025'],
    ],
    chartLabel: 'Hiring Status',
    chartData: [
      { label: 'Hired',       value: 2,  color: '#22C55E' },
      { label: 'Shortlisted', value: 1,  color: '#3B82F6' },
      { label: 'In Review',   value: 1,  color: '#F59E0B' },
      { label: 'Rejected',    value: 1,  color: '#EF4444' },
    ]
  },
  'Candidate Pipeline Report': {
    description: 'Pipeline breakdown of all candidates currently in process.',
    columns: ['Candidate', 'Stage', 'Source', 'Recruiter', 'Days in Pipeline'],
    rows: [
      ['Meena Rajan',    'Phone Screen',    'LinkedIn',  'Kiran',   '3'],
      ['Suresh Babu',    'Technical Round', 'Naukri',    'Anjali',  '7'],
      ['Lakshmi V.',     'HR Round',        'Referral',  'Pradeep', '12'],
      ['Vijay Kumar',    'Offer Sent',      'LinkedIn',  'Kiran',   '18'],
      ['Nidhi Shah',     'Document Check',  'Job Portal','Anjali',  '20'],
    ],
    chartLabel: 'Pipeline Stage',
    chartData: [
      { label: 'Phone Screen',    value: 1, color: '#60A5FA' },
      { label: 'Technical Round', value: 1, color: '#818CF8' },
      { label: 'HR Round',        value: 1, color: '#F472B6' },
      { label: 'Offer Sent',      value: 1, color: '#34D399' },
      { label: 'Document Check',  value: 1, color: '#FBBF24' },
    ]
  },
  'Job Performance Report': {
    description: 'Performance metrics for active job listings on the portal.',
    columns: ['Job Title', 'Department', 'Views', 'Applications', 'Conversion %'],
    rows: [
      ['Senior React Developer', 'Engineering', '1,240', '87',  '7.0%'],
      ['Data Scientist',          'Analytics',   '980',  '54',  '5.5%'],
      ['Product Manager',         'Product',     '870',  '102', '11.7%'],
      ['Cloud Architect',         'DevOps',      '620',  '31',  '5.0%'],
      ['Content Strategist',      'Marketing',   '430',  '63',  '14.7%'],
    ],
    chartLabel: 'Applications by Dept',
    chartData: [
      { label: 'Engineering', value: 87,  color: '#3B82F6' },
      { label: 'Analytics',   value: 54,  color: '#8B5CF6' },
      { label: 'Product',     value: 102, color: '#10B981' },
      { label: 'DevOps',      value: 31,  color: '#F59E0B' },
      { label: 'Marketing',   value: 63,  color: '#EF4444' },
    ]
  },
  'Interview Analysis Report': {
    description: 'Detailed analysis of all interviews conducted in the last 30 days.',
    columns: ['Candidate', 'Interviewer', 'Round', 'Score', 'Result'],
    rows: [
      ['Aisha Kumar',  'Rajan Sir',   'Technical', '8.5/10', 'Passed'],
      ['Suresh Babu',  'Priya Maam', 'HR',        '7.0/10', 'Passed'],
      ['Arjun Nair',   'Rajan Sir',   'Technical', '4.5/10', 'Failed'],
      ['Nidhi Shah',   'HR Team',     'Final',     '9.0/10', 'Selected'],
      ['Vijay Kumar',  'Priya Maam', 'HR',        '6.5/10', 'On Hold'],
    ],
    chartLabel: 'Interview Results',
    chartData: [
      { label: 'Passed',   value: 2, color: '#22C55E' },
      { label: 'Selected', value: 1, color: '#3B82F6' },
      { label: 'Failed',   value: 1, color: '#EF4444' },
      { label: 'On Hold',  value: 1, color: '#F59E0B' },
    ]
  },
  'Source Effectiveness Report': {
    description: 'Analysis of which hiring sources are producing the most candidates.',
    columns: ['Source', 'Candidates', 'Hired', 'Cost per Hire (₹)', 'Effectiveness'],
    rows: [
      ['LinkedIn',    '142', '28', '4,200', 'High'],
      ['Naukri',      '98',  '15', '2,800', 'Medium'],
      ['Referral',    '67',  '22', '1,200', 'Very High'],
      ['Job Portal',  '55',  '8',  '3,500', 'Medium'],
      ['Walk-in',     '20',  '3',  '500',   'Low'],
    ],
    chartLabel: 'Hired by Source',
    chartData: [
      { label: 'LinkedIn',   value: 28, color: '#0A66C2' },
      { label: 'Naukri',     value: 15, color: '#E91E1E' },
      { label: 'Referral',   value: 22, color: '#22C55E' },
      { label: 'Job Portal', value: 8,  color: '#F59E0B' },
      { label: 'Walk-in',    value: 3,  color: '#8B5CF6' },
    ]
  },
  'Recruitment Productivity Report': {
    description: 'Recruiter-level productivity metrics for the month.',
    columns: ['Recruiter', 'Openings Handled', 'Candidates Screened', 'Offers Made', 'Closed Positions'],
    rows: [
      ['Kiran Rao',    '12', '84',  '9',  '8'],
      ['Anjali Mehta', '9',  '61',  '6',  '5'],
      ['Pradeep S.',   '15', '102', '11', '10'],
      ['Sneha Jain',   '7',  '48',  '4',  '4'],
      ['Rahul Tiwari', '10', '73',  '8',  '7'],
    ],
    chartLabel: 'Closed Positions by Recruiter',
    chartData: [
      { label: 'Kiran',    value: 8,  color: '#3B82F6' },
      { label: 'Anjali',   value: 5,  color: '#8B5CF6' },
      { label: 'Pradeep',  value: 10, color: '#10B981' },
      { label: 'Sneha',    value: 4,  color: '#F59E0B' },
      { label: 'Rahul',    value: 7,  color: '#EF4444' },
    ]
  },
  'Compliance Audit Report': {
    description: 'Audit log of all compliance-related activities on the portal.',
    columns: ['Action', 'Performed By', 'Module', 'Date', 'Status'],
    rows: [
      ['User Data Export',       'Admin',   'User Mgmt',   '20 Jun 2025', 'Compliant'],
      ['GDPR Data Deletion',     'System',  'Settings',    '18 Jun 2025', 'Compliant'],
      ['Access Log Review',      'Admin',   'Security',    '15 Jun 2025', 'Compliant'],
      ['Policy Update',          'Admin',   'Content Mgmt','10 Jun 2025', 'Pending'],
      ['Backup Integrity Check', 'System',  'Backup',      '05 Jun 2025', 'Compliant'],
    ],
    chartLabel: 'Compliance Status',
    chartData: [
      { label: 'Compliant', value: 4, color: '#22C55E' },
      { label: 'Pending',   value: 1, color: '#F59E0B' },
    ]
  },
  'Backup Activity Report': {
    description: 'System-generated backup activity log for the last 30 days.',
    columns: ['Backup ID', 'Type', 'Size (MB)', 'Initiated By', 'Status'],
    rows: [
      ['BK-20250620', 'Full Backup',      '1,240', 'System', 'Success'],
      ['BK-20250613', 'Incremental',      '320',   'System', 'Success'],
      ['BK-20250606', 'Full Backup',      '1,198', 'Admin',  'Success'],
      ['BK-20250530', 'Incremental',      '285',   'System', 'Failed'],
      ['BK-20250523', 'Differential',     '540',   'System', 'Success'],
    ],
    chartLabel: 'Backup Status',
    chartData: [
      { label: 'Success', value: 4, color: '#22C55E' },
      { label: 'Failed',  value: 1, color: '#EF4444' },
    ]
  },
  'Application Status Report': {
    description: 'Summary of all job applications and their current processing status.',
    columns: ['Applicant', 'Job Title', 'Applied On', 'Current Stage', 'Status'],
    rows: [
      ['Aisha Kumar',  'Frontend Developer', '01 Jun 2025', 'Offer',     'Accepted'],
      ['Suresh Babu',  'Data Analyst',       '05 Jun 2025', 'Interview', 'Pending'],
      ['Nidhi Shah',   'UX Designer',        '08 Jun 2025', 'Screening', 'In Review'],
      ['Arjun Nair',   'Backend Engineer',   '10 Jun 2025', 'Applied',   'Rejected'],
      ['Divya Pillai', 'HR Specialist',      '12 Jun 2025', 'Offer',     'Accepted'],
    ],
    chartLabel: 'Application Status Breakdown',
    chartData: [
      { label: 'Accepted',  value: 2, color: '#22C55E' },
      { label: 'Pending',   value: 1, color: '#F59E0B' },
      { label: 'In Review', value: 1, color: '#3B82F6' },
      { label: 'Rejected',  value: 1, color: '#EF4444' },
    ]
  }
};

// ── STATE ─────────────────────────────────────────────────────────
let selectedCategoryFilter = 'All';
let currentReportName      = '';

// ── DYNAMIC METRICS ───────────────────────────────────────────────
function updateMetrics() {
  const data = reportLibraryData;
  const total      = data.length;
  const active     = data.filter(r => r.status === 'Active').length;
  const inactive   = data.filter(r => r.status === 'Inactive').length;
  const scheduled  = data.filter(r => r.schedule !== 'None').length;
  const daily      = data.filter(r => r.schedule === 'Daily').length;
  const today      = data.filter(r => r.lastGenerated.toLowerCase().includes('today')).length;
  const formats    = new Set(data.map(r => r.format)).size;
  const categories = new Set(data.map(r => r.category)).size;

  function set(id, val) {
    const el = document.getElementById(id);
    if (el) el.textContent = val;
  }

  set('m-total',      total);
  set('m-today',      today);
  set('m-scheduled',  scheduled);
  set('m-active',     active);
  set('m-inactive',   inactive);
  set('m-formats',    formats);
  set('m-categories', categories);
  set('m-daily',      daily);
}

// ── DYNAMIC CATEGORY COUNTS ───────────────────────────────────────
function updateCategoryCounts() {
  const data = reportLibraryData;
  // All
  const allEl = document.getElementById('count-all');
  if (allEl) allEl.textContent = data.length;

  // Per category
  const cats = ['Recruitment','Candidate','Performance','Interview','Job Perf','Audit','System'];
  cats.forEach(cat => {
    const el = document.getElementById('count-' + cat);
    if (el) el.textContent = data.filter(r => r.category === cat).length;
  });

  // Render bottom dynamic lists
  renderBottomDynamicCards();
}

// ── DYNAMIC BOTTOM CARDS ──────────────────────────────────────────
function renderBottomDynamicCards() {
  const data = reportLibraryData;

  // 1. Recent Reports (First 3 in library data)
  const recentList = document.getElementById('recent-reports-list');
  if (recentList) {
    const items = data.slice(0, 3);
    recentList.innerHTML = items.map(r => `
      <li onclick="viewReport('${r.name}')" style="cursor:pointer;">
        <span>${r.name}</span>
        <span class="text-muted">${r.category}</span>
      </li>
    `).join('') || '<li class="text-muted text-center" style="font-size:0.7rem;padding:10px 0;">No reports</li>';
  }

  // 2. Recently Exported (First 3 with PDF/Excel/CSV generated)
  const exportedList = document.getElementById('recently-exported-list');
  if (exportedList) {
    const items = data.filter(r => r.lastGenerated.toLowerCase().includes('today') || r.lastGenerated.toLowerCase().includes('yesterday')).slice(0, 3);
    exportedList.innerHTML = items.map(r => `
      <li onclick="downloadReport('${r.name}')" style="cursor:pointer;">
        <span>${r.name} (${r.format})</span>
        <span class="text-muted">${r.createdBy}</span>
      </li>
    `).join('') || `
      <li onclick="downloadReport('${data[0]?.name || ''}')" style="cursor:pointer;"><span>${data[0]?.name || 'Interview Analysis'} (PDF)</span> <span class="text-muted">Admin</span></li>
      <li onclick="downloadReport('${data[1]?.name || ''}')" style="cursor:pointer;"><span>${data[1]?.name || 'Source Effectiveness'} (Excel)</span> <span class="text-muted">Manager</span></li>
      <li onclick="downloadReport('${data[2]?.name || ''}')" style="cursor:pointer;"><span>${data[2]?.name || 'Compliance Audit'} (CSV)</span> <span class="text-muted">Admin</span></li>
    `;
  }

  // 3. Scheduled Reports (First 3 scheduled ones)
  const scheduledList = document.getElementById('scheduled-reports-list');
  if (scheduledList) {
    const items = data.filter(r => r.schedule !== 'None').slice(0, 3);
    scheduledList.innerHTML = items.map(r => `
      <li onclick="viewReport('${r.name}')" style="cursor:pointer;">
        <span>${r.name}</span>
        <span class="text-muted">${r.schedule === 'Daily' ? 'Daily 00:00' : r.schedule === 'Weekly' ? 'Every Mon' : '1st of Month'}</span>
      </li>
    `).join('') || '<li class="text-muted text-center" style="font-size:0.7rem;padding:10px 0;">No scheduled reports</li>';
  }

  // 4. Recent Shares (Dynamic based on data or mock sharing)
  const sharesList = document.getElementById('recent-shares-list');
  if (sharesList) {
    const sharedReports = data.slice(0, 3);
    const sharesMeta = ['Shared with Manager', 'Shared with Recruiter', 'Shared with Admin'];
    sharesList.innerHTML = sharedReports.map((r, i) => `
      <li onclick="shareReport('${r.name}')" style="cursor:pointer;">
        <span>${r.name}</span>
        <span class="text-muted">${sharesMeta[i % 3]}</span>
      </li>
    `).join('');
  }

  // 5. Recent Audit Activities (Audits log actions)
  const auditsList = document.getElementById('recent-audits-list');
  if (auditsList) {
    // Generate audit trail dynamically from the action types in reports
    const auditLogs = [
      { name: 'System Data Backup Config', action: 'Created' },
      { name: 'Security Key Rotated', action: 'Updated' },
      { name: 'Compliance Logs Exported', action: 'Success' }
    ];
    // Add custom custom-generated audit records dynamically
    const customRec = data.find(r => r.lastGenerated === 'Just now');
    if (customRec) {
      auditLogs.unshift({ name: `Report "${customRec.name}" Created`, action: 'Success' });
      auditLogs.pop();
    }
    auditsList.innerHTML = auditLogs.map(a => `
      <li style="cursor:default;">
        <span>${a.name}</span>
        <span class="text-muted">${a.action}</span>
      </li>
    `).join('');
  }
}

// ── VIEW ALL CATEGORY / LIST FILTERING ────────────────────────────
window.viewAllCategory = function(mode) {
  // Prevent default page reload anchor action
  if (window.event) window.event.preventDefault();

  const searchInput = document.getElementById('lib-search-input');
  if (searchInput) searchInput.value = '';

  if (mode === 'All') {
    selectedCategoryFilter = 'All';
    document.querySelectorAll('.cat-item').forEach(item => {
      if (item.getAttribute('onclick')?.includes('All')) item.classList.add('active');
      else item.classList.remove('active');
    });
    filterLibrary();
    // Open detail modal for the first item
    if (reportLibraryData.length > 0) viewReport(reportLibraryData[0].name);
  } else if (mode === 'Scheduled') {
    selectedCategoryFilter = 'All';
    const filtered = reportLibraryData.filter(r => r.schedule !== 'None');
    renderLibrary(filtered);
    showToast('Showing only Scheduled Reports', '#0A4BD2');
    if (filtered.length > 0) viewReport(filtered[0].name);
  } else if (mode === 'Export') {
    selectedCategoryFilter = 'All';
    const filtered = reportLibraryData.filter(r => r.status === 'Active');
    renderLibrary(filtered);
    showToast('Showing Generated Active Exports', '#0A4BD2');
    if (filtered.length > 0) viewReport(filtered[0].name);
  } else if (mode === 'Shares') {
    selectedCategoryFilter = 'All';
    const filtered = reportLibraryData.slice(0, 3);
    renderLibrary(filtered);
    showToast('Showing Shared Reports', '#0A4BD2');
    if (filtered.length > 0) viewReport(filtered[0].name);
  } else if (mode === 'Audit') {
    selectedCategoryFilter = 'All';
    const filtered = reportLibraryData.filter(r => r.category === 'Audit' || r.category === 'System');
    renderLibrary(filtered);
    showToast('Showing System Audit Reports', '#0A4BD2');
    if (filtered.length > 0) viewReport(filtered[0].name);
  }
};

window.openExplorePage = function(target) {
  if (target === 'reports') {
    // Scroll smoothly to builder card
    document.querySelector('.builder-card')?.scrollIntoView({ behavior: 'smooth' });
    showToast('🔍 Exploring Report Custom Builder', '#0A4BD2');
  } else if (target === 'export') {
    // Open the report list download trigger directly
    if (reportLibraryData.length > 0) {
      // Pick first report to start download prompt
      downloadReport(reportLibraryData[0].name);
      showToast('📥 Opening Export Format Dialog...', '#0A4BD2');
    } else {
      showToast('⚠️ No reports available in Library to export', '#F59E0B');
    }
  } else if (target === 'scheduled') {
    // Jump straight to the Scheduled list
    const filtered = reportLibraryData.filter(r => r.schedule !== 'None');
    renderLibrary(filtered);
    document.getElementById('lib-search-input')?.scrollIntoView({ behavior: 'smooth' });
    showToast('📅 Exploring Scheduled Tasks', '#0A4BD2');
  } else if (target === 'shares') {
    showToast('🔄 Redirecting to Share Configuration...', '#0A4BD2');
  } else if (target === 'audit') {
    openAuditModal();
  }
};

// ── AUDIT LOGS MODAL ──────────────────────────────────────────────
const auditSeedLogs = [
  { name: 'Hiring Summary Report',       action: 'Generated',  user: 'Admin',         time: '2 mins ago'  },
  { name: 'Candidate Pipeline Report',   action: 'Exported',   user: 'HR Manager',    time: '15 mins ago' },
  { name: 'Interview Performance Report',action: 'Shared',     user: 'Admin',         time: '1 hr ago'    },
  { name: 'Offer Acceptance Rate',       action: 'Scheduled',  user: 'System',        time: '3 hrs ago'   },
  { name: 'Recruitment Funnel Report',   action: 'Viewed',     user: 'HR Manager',    time: '5 hrs ago'   },
  { name: 'Application Status Report',   action: 'Generated',  user: 'Recruiter',     time: 'Yesterday'   },
  { name: 'Time-to-Hire Report',         action: 'Exported',   user: 'Admin',         time: 'Yesterday'   },
  { name: 'Source Effectiveness Report', action: 'Deleted',    user: 'Admin',         time: '2 days ago'  },
  { name: 'Diversity Report',            action: 'Generated',  user: 'HR Manager',    time: '3 days ago'  },
  { name: 'Cost Per Hire Report',        action: 'Shared',     user: 'Finance',       time: '4 days ago'  },
];

const actionColors = {
  Generated:  { bg: '#DCFCE7', color: '#15803D' },
  Exported:   { bg: '#DBEAFE', color: '#1D4ED8' },
  Shared:     { bg: '#EDE9FE', color: '#7C3AED' },
  Scheduled:  { bg: '#FEF9C3', color: '#854D0E' },
  Viewed:     { bg: '#F1F5F9', color: '#475569' },
  Deleted:    { bg: '#FEE2E2', color: '#DC2626' },
};

let auditCurrentPage = 1;
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
    time: `${i + 1} hr${i === 0 ? '' : 's'} ago`
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
      return `<tr style="background:${rowBg};border-bottom:1px solid #F1F5F9;">
        <td style="padding:10px 10px;color:var(--text-main,#1E293B);font-weight:600;">
          <i class="fa-solid fa-file-lines" style="color:#0A4BD2;margin-right:6px;"></i>${log.name}
        </td>
        <td style="padding:10px;">
          <span style="background:${ac.bg};color:${ac.color};font-size:0.7rem;font-weight:700;padding:3px 10px;border-radius:20px;">${log.action}</span>
        </td>
        <td style="padding:10px;color:#475569;">${log.user}</td>
        <td style="padding:10px;color:#94A3B8;font-size:0.72rem;">${log.time}</td>
      </tr>`;
    }).join('');
  }
  
  const container = document.getElementById('audit-pagination-container');
  if (container) {
    if (totalPages <= 1) {
      container.innerHTML = '';
    } else {
      let pgHtml = `<a href="javascript:void(0)" onclick="auditGoToPage(${auditCurrentPage - 1})" ${auditCurrentPage === 1 ? 'style="pointer-events:none;opacity:0.5;"' : ''}><i class="fa-solid fa-angle-left"></i> Prev</a>`;
      for(let i=1; i<=totalPages; i++) {
        if(i===1 || i===totalPages || (i >= auditCurrentPage - 1 && i <= auditCurrentPage + 1)) {
          pgHtml += `<a href="javascript:void(0)" class="${i === auditCurrentPage ? 'active' : ''}" onclick="auditGoToPage(${i})">${i}</a>`;
        } else if (i === auditCurrentPage - 2 || i === auditCurrentPage + 2) {
          pgHtml += `<span>...</span>`;
        }
      }
      pgHtml += `<a href="javascript:void(0)" onclick="auditGoToPage(${auditCurrentPage + 1})" ${auditCurrentPage === totalPages ? 'style="pointer-events:none;opacity:0.5;"' : ''}>Next <i class="fa-solid fa-angle-right"></i></a>`;
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
}

function closeAuditModal() {
  const overlay = document.getElementById('audit-logs-overlay');
  if (overlay) overlay.style.display = 'none';
}





// ── PAGINATION STATE ──────────────────────────────────────────────
const PAGE_SIZE   = 5;
let   currentPage = 1;
let   filteredData = [];

// ── RENDER PAGE ───────────────────────────────────────────────────
function renderLibrary(data) {
  filteredData  = data;
  currentPage   = 1;
  renderPage();
}

function renderPage() {
  const tbody = document.getElementById('lib-tbody');
  if (!tbody) return;

  const totalPages = Math.max(1, Math.ceil(filteredData.length / PAGE_SIZE));
  currentPage      = Math.min(Math.max(1, currentPage), totalPages);

  const start = (currentPage - 1) * PAGE_SIZE;
  const slice = filteredData.slice(start, start + PAGE_SIZE);

  if (filteredData.length === 0) {
    tbody.innerHTML = `<tr><td colspan="8" style="text-align:center;padding:24px;color:#94A3B8;font-size:0.8rem;">No reports found.</td></tr>`;
    renderPagination(0, 1);
    return;
  }

  tbody.innerHTML = slice.map(r => `
    <tr>
      <td class="fw-bold">${r.name}</td>
      <td>${r.category}</td>
      <td>${r.createdBy}</td>
      <td>${r.lastGenerated}</td>
      <td>${r.schedule}</td>
      <td><span class="badge bg-light text-dark">${r.format}</span></td>
      <td><span class="badge ${r.status === 'Active' ? 'bg-success' : 'bg-secondary'}">${r.status}</span></td>
      <td class="lib-actions">
        <i class="fa-regular fa-eye"       title="View"     onclick="viewReport('${r.name}')"></i>
        <i class="fa-solid fa-download"    title="Download" onclick="downloadReport('${r.name}')"></i>
        <i class="fa-solid fa-share-nodes" title="Share"    onclick="shareReport('${r.name}')"></i>
        <i class="fa-regular fa-trash-can" title="Delete"   onclick="deleteReport('${r.name}')"></i>
      </td>
    </tr>
  `).join('');

  renderPagination(filteredData.length, totalPages);
}

function renderPagination(total, totalPages) {
  const container = document.getElementById('pagination-container');
  if (!container) return;

  const start = (currentPage - 1) * PAGE_SIZE + 1;
  const end   = Math.min(currentPage * PAGE_SIZE, total);

  // Build page buttons with smart window
  function pageBtn(p, label, active, disabled) {
    return `<button
      onclick="${disabled ? '' : `goToPage(${p})`}"
      style="
        padding:6px 12px; border-radius:7px; border:1px solid ${active ? '#0A4BD2' : '#E2E8F0'};
        background:${active ? '#0A4BD2' : '#fff'}; color:${active ? '#fff' : disabled ? '#CBD5E1' : '#1E293B'};
        font-size:0.78rem; font-weight:${active ? '700' : '500'};
        cursor:${disabled ? 'not-allowed' : 'pointer'};
        min-width:34px; transition:all .15s;
      "
      ${disabled ? 'disabled' : ''}
    >${label}</button>`;
  }

  let pagesHTML = '';

  if (totalPages <= 7) {
    // Show all pages if 7 or fewer
    for (let p = 1; p <= totalPages; p++) {
      pagesHTML += pageBtn(p, p, p === currentPage, false);
    }
  } else {
    // Smart ellipsis pagination
    pagesHTML += pageBtn(1, 1, currentPage === 1, false);
    if (currentPage > 3) {
      pagesHTML += `<span style="align-self:center;color:#94A3B8;font-size:0.8rem;padding:0 4px;">…</span>`;
    }
    const winStart = Math.max(2, currentPage - 1);
    const winEnd   = Math.min(totalPages - 1, currentPage + 1);
    for (let p = winStart; p <= winEnd; p++) {
      pagesHTML += pageBtn(p, p, p === currentPage, false);
    }
    if (currentPage < totalPages - 2) {
      pagesHTML += `<span style="align-self:center;color:#94A3B8;font-size:0.8rem;padding:0 4px;">…</span>`;
    }
    pagesHTML += pageBtn(totalPages, totalPages, currentPage === totalPages, false);
  }

  container.innerHTML = `
    <div style="display:flex;align-items:center;justify-content:space-between;flex-wrap:wrap;gap:10px;padding:12px 4px 4px;">
      <span style="font-size:0.73rem;color:#64748B;">
        Showing <strong>${total > 0 ? start : 0}–${end}</strong> of <strong>${total}</strong> reports
      </span>
      <div style="display:flex;align-items:center;gap:6px;flex-wrap:wrap;">
        ${pageBtn(currentPage - 1, '← Previous', false, currentPage === 1)}
        ${pagesHTML}
        ${pageBtn(currentPage + 1, 'Next →', false, currentPage === totalPages || totalPages === 0)}
      </div>
    </div>
  `;
}

function goToPage(p) {
  currentPage = p;
  renderPage();
}

function selectCategory(element, category) {
  document.querySelectorAll('.cat-item').forEach(item => item.classList.remove('active'));
  element.classList.add('active');
  selectedCategoryFilter = category;
  currentPage = 1;
  filterLibrary();
}

function filterLibrary() {
  const query    = (document.getElementById('lib-search-input')?.value || '').toLowerCase();
  const filtered = reportLibraryData.filter(r => {
    const matchesCategory = selectedCategoryFilter === 'All' || r.category === selectedCategoryFilter;
    const matchesQuery    = r.name.toLowerCase().includes(query) || r.category.toLowerCase().includes(query);
    return matchesCategory && matchesQuery;
  });
  renderLibrary(filtered);
}


// ── VIEW MODAL ────────────────────────────────────────────────────
function viewReport(name) {
  currentReportName = name;
  const r = reportLibraryData.find(x => x.name === name);
  if (!r) return;
  // Get report content details dynamically using the content compiler
  const compiled = buildReportContent(name, (r.format || 'pdf').toLowerCase());
  // The returned builder compiles details dynamically inside buildReportContent
  const det = reportDetails[name] || getLiveSourceData(r.category) || getLiveSourceData('Jobs');
  if (!det) return showToast('Details not available for this report.', '#F59E0B');

  document.getElementById('vr-title').textContent   = name;
  document.getElementById('vr-sub').textContent      = det.description;
  document.getElementById('vr-badge-cat').textContent   = r.category;
  document.getElementById('vr-badge-fmt').textContent   = 'Format: ' + r.format;
  document.getElementById('vr-badge-sched').textContent = 'Schedule: ' + r.schedule;

  const statusEl = document.getElementById('vr-badge-status');
  statusEl.textContent    = r.status;
  statusEl.style.background = r.status === 'Active' ? '#DCFCE7' : '#F1F5F9';
  statusEl.style.color      = r.status === 'Active' ? '#15803D' : '#475569';

  // Table
  const thead = document.getElementById('vr-thead');
  const tbody = document.getElementById('vr-tbody');
  thead.innerHTML = `<tr>${det.columns.map(c => `<th style="padding:10px 12px;text-align:left;font-size:0.72rem;color:#64748B;border-bottom:1px solid #E2E8F0;">${c}</th>`).join('')}</tr>`;
  tbody.innerHTML = det.rows.map((row, i) =>
    `<tr style="background:${i % 2 === 0 ? '#fff' : '#F8FAFC'};">${row.map(cell => `<td style="padding:9px 12px;border-bottom:1px solid #E2E8F0;">${cell}</td>`).join('')}</tr>`
  ).join('');

  // Pie chart
  drawPie(det.chartData);

  const overlay = document.getElementById('view-report-overlay');
  overlay.style.display = 'flex';
  document.body.style.overflow = 'hidden';
}

function drawPie(chartData) {
  const canvas  = document.getElementById('vr-pie-canvas');
  const legend  = document.getElementById('vr-pie-legend');
  const ctx     = canvas.getContext('2d');
  const total   = chartData.reduce((s, d) => s + d.value, 0);
  const cx = 90, cy = 90, r = 80;

  ctx.clearRect(0, 0, 180, 180);
  let startAngle = -Math.PI / 2;

  chartData.forEach(slice => {
    const sweep = (slice.value / total) * 2 * Math.PI;
    ctx.beginPath();
    ctx.moveTo(cx, cy);
    ctx.arc(cx, cy, r, startAngle, startAngle + sweep);
    ctx.closePath();
    ctx.fillStyle = slice.color;
    ctx.fill();
    startAngle += sweep;
  });

  // Centre hole
  ctx.beginPath();
  ctx.arc(cx, cy, 40, 0, 2 * Math.PI);
  ctx.fillStyle = '#fff';
  ctx.fill();
  ctx.font = 'bold 13px Poppins,sans-serif';
  ctx.fillStyle = '#1E293B';
  ctx.textAlign = 'center';
  ctx.fillText(total, cx, cy + 5);

  legend.innerHTML = chartData.map(slice =>
    `<div style="display:flex;align-items:center;gap:8px;">
      <span style="width:12px;height:12px;border-radius:3px;background:${slice.color};flex-shrink:0;"></span>
      <span>${slice.label} — <strong>${slice.value}</strong> (${Math.round((slice.value/total)*100)}%)</span>
    </div>`
  ).join('');
}

function closeViewModal() {
  document.getElementById('view-report-overlay').style.display = 'none';
  document.body.style.overflow = '';
}

function openEditFromView() {
  closeViewModal();
  openEditModal(currentReportName);
}

// ── DOWNLOAD MODAL ────────────────────────────────────────────────
function downloadReport(name) {
  currentReportName = name;
  document.getElementById('dl-report-name').textContent = name;
  const overlay = document.getElementById('download-format-overlay');
  overlay.style.display = 'flex';
  document.body.style.overflow = 'hidden';
}

function closeDownloadModal() {
  document.getElementById('download-format-overlay').style.display = 'none';
  document.body.style.overflow = '';
}

// ── FILE SYSTEM SAVE HANDLE (persists across downloads per session) ─
let _reportDirHandle = null;

async function getReportDir() {
  if (_reportDirHandle) {
    // Verify permission is still granted
    const perm = await _reportDirHandle.queryPermission({ mode: 'readwrite' });
    if (perm === 'granted') return _reportDirHandle;
  }
  // Ask user to select the Report folder
  try {
    _reportDirHandle = await window.showDirectoryPicker({
      id:        'report-save-dir',
      mode:      'readwrite',
      startIn:   'documents'
    });
    return _reportDirHandle;
  } catch (e) {
    return null; // User cancelled
  }
}

async function saveToReportFolder(filename, blob) {
  // Try File System Access API first
  if ('showDirectoryPicker' in window) {
    try {
      const dir = await getReportDir();
      if (dir) {
        const fileHandle = await dir.getFileHandle(filename, { create: true });
        const writable   = await fileHandle.createWritable();
        await writable.write(blob);
        await writable.close();
        return true;
      }
    } catch (e) {
      console.warn('FS API failed, falling back to browser download:', e);
    }
  }
  // Fallback: standard browser download
  const url = URL.createObjectURL(blob);
  const a   = document.createElement('a');
  a.href     = url;
  a.download = filename;
  a.click();
  setTimeout(() => URL.revokeObjectURL(url), 1000);
  return false;
}
function buildReportContent(name, format) {
  let det = reportDetails[name];
  if (!det) {
    const libItem = reportLibraryData.find(r => r.name === name);
    const sourceName = libItem ? libItem.category : builderState.source;
    const baseData = getLiveSourceData(sourceName) || getLiveSourceData('Jobs');

    // If it's a custom report, filter the columns and rows according to custom builder settings
    let selectedFields = builderState.fields;
    if (selectedFields.length === 0) {
      selectedFields = baseData.columns;
    }

    const colIndexes = selectedFields.map(f => baseData.columns.indexOf(f)).filter(idx => idx !== -1);
    
    // Map data rows
    let mappedRows = baseData.rows.map(row => colIndexes.map(idx => row[idx]));

    det = {
      description: baseData.description,
      columns: selectedFields,
      rows: mappedRows,
      chartLabel: baseData.chartLabel,
      chartData: baseData.chartData
    };
  }

  // ── CSV / EXCEL content ───────────────────────────────────────────
  function buildCSV() {
    if (!det) return `"Report","${name}"\n"Generated","${new Date().toLocaleString()}"\n`;
    const rows = [det.columns, ...det.rows];
    let csv = rows.map(row => row.map(c => `"${c}"`).join(',')).join('\n');
    csv += '\n\n"--- Distribution Summary ---"\n';
    const total = det.chartData.reduce((s, d) => s + d.value, 0);
    csv += det.chartData.map(d =>
      `"${d.label}","${d.value}","${Math.round((d.value/total)*100)}%"`
    ).join('\n');
    return csv;
  }

  // ── DOC / WORD content ────────────────────────────────────────────
  function buildDOC() {
    let html = `<html><head><meta charset='utf-8'><title>${name}</title></head><body>`;
    html += `<h2 style="color:#0A4BD2;">${name}</h2>`;
    html += `<p><em>Generated: ${new Date().toLocaleString()}</em></p>`;
    if (det) {
      html += `<p>${det.description}</p>`;
      html += `<table border="1" cellpadding="6" cellspacing="0" style="border-collapse:collapse;width:100%;font-size:12px;"><thead><tr>`;
      html += det.columns.map(c => `<th style="background:#0A4BD2;color:#fff;padding:8px;">${c}</th>`).join('');
      html += `</tr></thead><tbody>`;
      html += det.rows.map((row, i) =>
        `<tr style="background:${i%2===0?'#fff':'#F8FAFC'};">${row.map(c => `<td style="padding:7px;border:1px solid #E2E8F0;">${c}</td>`).join('')}</tr>`
      ).join('');
      html += `</tbody></table><br>`;
      html += `<h3>Distribution Summary</h3><ul>`;
      const total = det.chartData.reduce((s, d) => s + d.value, 0);
      html += det.chartData.map(d =>
        `<li>${d.label}: <strong>${d.value}</strong> (${Math.round((d.value/total)*100)}%)</li>`
      ).join('');
      html += `</ul>`;
    }
    html += `</body></html>`;
    return html;
  }

  // ── PDF print page content ────────────────────────────────────────
  function buildPDFPage() {
    let html = `<!DOCTYPE html><html><head><meta charset='utf-8'><title>${name}</title>
    <style>
      body  { font-family:Arial,sans-serif; margin:30px; color:#1E293B; }
      h1    { font-size:18px; color:#0A4BD2; border-bottom:2px solid #0A4BD2; padding-bottom:6px; }
      .meta { font-size:11px; color:#64748B; margin-bottom:16px; }
      table { width:100%; border-collapse:collapse; margin-top:16px; font-size:12px; }
      th    { background:#0A4BD2; color:#fff; padding:8px 10px; text-align:left; }
      td    { padding:7px 10px; border-bottom:1px solid #E2E8F0; }
      tr:nth-child(even) td { background:#F8FAFC; }
      .chart-section { margin-top:24px; }
      .chart-section h3 { font-size:14px; color:#0A4BD2; margin-bottom:10px; }
      .bar-row { display:flex; align-items:center; gap:10px; margin:6px 0; font-size:11px; }
      .bar     { height:16px; border-radius:4px; min-width:4px; }
      .bar-lbl { min-width:140px; }
    </style></head><body>`;
    html += `<h1>${name}</h1>`;
    html += `<div class="meta">Generated: ${new Date().toLocaleString()}</div>`;
    if (det) {
      html += `<p style="font-size:12px;color:#64748B;">${det.description}</p>`;
      html += `<table><thead><tr>`;
      html += det.columns.map(c => `<th>${c}</th>`).join('');
      html += `</tr></thead><tbody>`;
      html += det.rows.map(row =>
        `<tr>${row.map(c => `<td>${c}</td>`).join('')}</tr>`
      ).join('');
      html += `</tbody></table>`;
      const total = det.chartData.reduce((s, d) => s + d.value, 0);
      html += `<div class="chart-section"><h3>Distribution — ${det.chartLabel}</h3>`;
      html += det.chartData.map(d => {
        const pct = Math.round((d.value / total) * 100);
        return `<div class="bar-row">
          <span class="bar-lbl">${d.label}</span>
          <div class="bar" style="width:${pct*2}px;background:${d.color};"></div>
          <span><strong>${d.value}</strong> (${pct}%)</span>
        </div>`;
      }).join('');
      html += `</div>`;
    }
    html += `</body></html>`;
    return html;
  }

  return { buildCSV, buildDOC, buildPDFPage };
}

async function triggerDownload(format) {
  const name    = currentReportName;
  const content = buildReportContent(name, format);
  const safeName = name.replace(/[^a-z0-9_\-\.]/gi, '_');
  const ts       = new Date().toISOString().slice(0,10);
  closeDownloadModal();

  if (format === 'excel') {
    const csv  = content.buildCSV();
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const saved = await saveToReportFolder(`${safeName}_${ts}.csv`, blob);
    showToast(
      saved ? `✅ Saved to Report folder: ${safeName}_${ts}.csv` : `📥 Downloading ${safeName}.csv`,
      '#15803D'
    );

  } else if (format === 'docx') {
    const doc  = content.buildDOC();
    const blob = new Blob(['\ufeff', doc], { type: 'application/msword' });
    const saved = await saveToReportFolder(`${safeName}_${ts}.doc`, blob);
    showToast(
      saved ? `✅ Saved to Report folder: ${safeName}_${ts}.doc` : `📥 Downloading ${safeName}.doc`,
      '#15803D'
    );

  } else {
    // PDF: open print window. Also save an HTML version to the folder.
    const page  = content.buildPDFPage();
    const blob  = new Blob([page], { type: 'text/html;charset=utf-8;' });
    const saved = await saveToReportFolder(`${safeName}_${ts}.html`, blob);

    const win = window.open('', '_blank');
    if (win) {
      win.document.write(page);
      win.document.close();
      win.focus();
      setTimeout(() => win.print(), 500);
    }
    showToast(
      saved ? `✅ Saved to Report folder & PDF print dialog opened` : `✅ PDF print dialog opened`,
      '#15803D'
    );
  }
}

// ── EDIT MODAL ────────────────────────────────────────────────────
function openEditModal(name) {
  currentReportName = name;
  const r = reportLibraryData.find(x => x.name === name);
  if (!r) return;

  document.getElementById('er-name').value = r.name;
  document.getElementById('er-format').value    = r.format;
  document.getElementById('er-createdby').value = r.createdBy;

  const catSel   = document.getElementById('er-category');
  const schedSel = document.getElementById('er-schedule');
  const statSel  = document.getElementById('er-status');
  [...catSel.options].forEach(o => o.selected = o.value === r.category);
  [...schedSel.options].forEach(o => o.selected = o.value === r.schedule);
  [...statSel.options].forEach(o => o.selected = o.value === r.status);

  const overlay = document.getElementById('edit-report-overlay');
  overlay.style.display = 'flex';
  document.body.style.overflow = 'hidden';
}

function closeEditModal() {
  document.getElementById('edit-report-overlay').style.display = 'none';
  document.body.style.overflow = '';
}

function saveEditReport() {
  const r = reportLibraryData.find(x => x.name === currentReportName);
  if (!r) return;

  const newName = document.getElementById('er-name').value.trim();
  r.name        = newName || r.name;
  r.category    = document.getElementById('er-category').value;
  r.schedule    = document.getElementById('er-schedule').value;
  r.status      = document.getElementById('er-status').value;

  closeEditModal();
  filterLibrary();
  showToast('✅ Report updated successfully!', '#15803D');
}

// ── EDIT icon directly from table ────────────────────────────────
function editReport(name) {
  openEditModal(name);
}

// ── OTHER ACTIONS ─────────────────────────────────────────────────
async function shareReport(name) {
  const libItem = reportLibraryData.find(r => r.name === name);
  const format = libItem ? (libItem.format || 'pdf').toLowerCase() : 'pdf';
  
  // 1. Generate the file first
  const content = buildReportContent(name, format);
  const safeName = name.replace(/[^a-z0-9_\-\.]/gi, '_');
  const ts = new Date().toISOString().slice(0, 10);
  
  let filename = `${safeName}_${ts}.csv`;
  let mimeType = 'text/csv;charset=utf-8;';
  let blobData = '';
  
  if (format === 'excel') {
    blobData = content.buildCSV();
  } else if (format === 'docx') {
    filename = `${safeName}_${ts}.doc`;
    mimeType = 'application/msword';
    blobData = '\ufeff' + content.buildDOC();
  } else {
    filename = `${safeName}_${ts}.html`;
    mimeType = 'text/html;charset=utf-8;';
    blobData = content.buildPDFPage();
  }
  
  const blob = new Blob([blobData], { type: mimeType });
  
  // Save locally to target folder first
  const saved = await saveToReportFolder(filename, blob);
  
  // 2. Generate and present the shareable link
  const mockShareUrl = `${window.location.origin}/share/reports/${encodeURIComponent(safeName)}`;
  
  // Prompt modal or input alert
  const shareText = `Report "${name}" generated & saved to destination!\n\nHere is your shareable link:\n${mockShareUrl}`;
  alert(shareText);
  
  showToast(`🔗 Share link copied to clipboard!`, '#0A4BD2');
}

function deleteReport(name) {
  if (!confirm(`Delete "${name}" from the library?`)) return;
  const idx = reportLibraryData.findIndex(x => x.name === name);
  if (idx !== -1) reportLibraryData.splice(idx, 1);
  filterLibrary();
  updateMetrics();
  updateCategoryCounts();
  showToast(`❌ Deleted "${name}" from library`, '#EF4444');
}

// ── CUSTOM REPORT BUILDER ─────────────────────────────────────────
const builderSourceFields = {
  Jobs:          ['Job Title', 'Department', 'Location', 'Type', 'Status', 'Posted Date', 'Applicants', 'Closing Date'],
  Candidates:    ['Name', 'Email', 'Phone', 'Skills', 'Experience (yrs)', 'Applied Role', 'Source', 'Stage', 'Applied Date'],
  Applications:  ['Applicant', 'Job Title', 'Department', 'Applied On', 'Stage', 'Status', 'Recruiter', 'Last Updated'],
  Interviews:    ['Candidate', 'Interviewer', 'Round', 'Date', 'Score', 'Result', 'Mode', 'Feedback'],
  Users:         ['Full Name', 'Role', 'Email', 'Status', 'Joined Date', 'Last Login', 'Department'],
  Notifications: ['Title', 'Type', 'Sent To', 'Sent On', 'Status', 'Channel'],
  Reports:       ['Report Name', 'Category', 'Created By', 'Last Generated', 'Schedule', 'Format', 'Status'],
  Content:       ['Title', 'Type', 'Category', 'Author', 'Status', 'Published Date', 'Views'],
  Backups:       ['Backup ID', 'Type', 'Size (MB)', 'Initiated By', 'Date', 'Status']
};

const builderState = {
  source:       'Jobs',
  fields:       [],
  filters:      [],
  groupBy:      null,
  sortField:    null,
  sortOrder:    'asc',
  format:       'PDF',
  schedule:     'None',
  reportName:   ''
};

let filterRowCount = 0;

function switchBuilderTab(idx) {
  // Update tab button active state
  document.querySelectorAll('.builder-tab').forEach((btn, i) => {
    btn.classList.toggle('active', i === idx);
  });
  // Show/hide panels
  for (let i = 0; i < 6; i++) {
    const panel = document.getElementById('btab-' + i);
    if (panel) panel.style.display = i === idx ? '' : 'none';
  }
  // Refresh panel content when switching to it
  if (idx === 1) renderFieldCheckboxes();
  if (idx === 3) renderGroupOptions();
  if (idx === 4) renderSortFields();
}

function selectSource(element, source) {
  document.querySelectorAll('.source-item').forEach(el => el.classList.remove('selected'));
  element.classList.add('selected');
  builderState.source  = source;
  builderState.fields  = [];
  builderState.groupBy = null;
  builderState.sortField = null;
  updateLiveSummary();
  // Auto-advance to tab 2
  switchBuilderTab(1);
}

function renderFieldCheckboxes() {
  const fields = builderSourceFields[builderState.source] || [];
  const container = document.getElementById('field-checkboxes');
  if (!container) return;
  container.innerHTML = fields.map(f => {
    const checked = builderState.fields.includes(f);
    return `<label onclick="toggleField(this,'${f}')" style="
      display:flex;align-items:center;gap:8px;padding:9px 14px;
      border:1.5px solid ${checked ? '#0A4BD2' : 'var(--border,#E2E8F0)'};
      border-radius:8px;cursor:pointer;font-size:0.75rem;font-weight:600;
      background:${checked ? '#EFF6FF' : 'var(--card-bg,#fff)'};
      color:${checked ? '#0A4BD2' : 'var(--text-main,#1E293B)'};
      transition:all .15s;user-select:none;" data-field="${f}">
      <span style="width:16px;height:16px;border-radius:4px;border:2px solid ${checked ? '#0A4BD2' : '#CBD5E1'};
        background:${checked ? '#0A4BD2' : 'transparent'};display:inline-flex;align-items:center;justify-content:center;flex-shrink:0;">
        ${checked ? '<i class="fa-solid fa-check" style="color:#fff;font-size:0.55rem;"></i>' : ''}
      </span>
      ${f}
    </label>`;
  }).join('');
}

function toggleField(labelEl, field) {
  const idx = builderState.fields.indexOf(field);
  if (idx === -1) builderState.fields.push(field);
  else            builderState.fields.splice(idx, 1);
  renderFieldCheckboxes();
  updateLiveSummary();
}

function addFilterRow() {
  const fields = builderSourceFields[builderState.source] || [];
  filterRowCount++;
  const id = 'frow-' + filterRowCount;
  const container = document.getElementById('filter-rows');
  const row = document.createElement('div');
  row.id = id;
  row.style.cssText = 'display:flex;gap:8px;align-items:center;flex-wrap:wrap;';
  row.innerHTML = `
    <select onchange="syncFilterSummary()" style="padding:7px 10px;border:1px solid var(--border,#E2E8F0);border-radius:7px;font-size:0.75rem;outline:none;background:var(--card-bg,#fff);color:var(--text-main,#1E293B);">
      ${fields.map(f => `<option>${f}</option>`).join('')}
    </select>
    <select onchange="syncFilterSummary()" style="padding:7px 10px;border:1px solid var(--border,#E2E8F0);border-radius:7px;font-size:0.75rem;outline:none;background:var(--card-bg,#fff);color:var(--text-main,#1E293B);">
      <option>equals</option><option>contains</option><option>starts with</option>
      <option>greater than</option><option>less than</option><option>is empty</option>
    </select>
    <input type="text" placeholder="Value..." onchange="syncFilterSummary()" style="padding:7px 10px;border:1px solid var(--border,#E2E8F0);border-radius:7px;font-size:0.75rem;outline:none;background:var(--card-bg,#fff);color:var(--text-main,#1E293B);min-width:120px;flex:1;" />
    <button onclick="removeFilterRow('${id}')" style="background:#FEE2E2;color:#EF4444;border:none;padding:7px 10px;border-radius:7px;font-size:0.75rem;cursor:pointer;">✕</button>
  `;
  container.appendChild(row);
  syncFilterSummary();
}

function removeFilterRow(id) {
  document.getElementById(id)?.remove();
  syncFilterSummary();
}

function syncFilterSummary() {
  const rows = document.querySelectorAll('#filter-rows > div');
  const count = rows.length;
  const el = document.getElementById('sum-filters');
  if (el) el.textContent = count === 0 ? 'None' : `${count} condition${count > 1 ? 's' : ''}`;
}

function renderGroupOptions() {
  const fields = builderSourceFields[builderState.source] || [];
  const container = document.getElementById('group-options');
  if (!container) return;
  const none = builderState.groupBy === null;
  container.innerHTML = `
    <label onclick="setGroupBy(null)" style="
      padding:9px 16px;border:1.5px solid ${none ? '#0A4BD2' : 'var(--border,#E2E8F0)'};
      border-radius:8px;cursor:pointer;font-size:0.75rem;font-weight:600;
      background:${none ? '#EFF6FF' : 'var(--card-bg,#fff)'};
      color:${none ? '#0A4BD2' : '#64748B'};user-select:none;">
      None
    </label>
    ${fields.map(f => {
      const active = builderState.groupBy === f;
      return `<label onclick="setGroupBy('${f}')" style="
        padding:9px 16px;border:1.5px solid ${active ? '#0A4BD2' : 'var(--border,#E2E8F0)'};
        border-radius:8px;cursor:pointer;font-size:0.75rem;font-weight:600;
        background:${active ? '#EFF6FF' : 'var(--card-bg,#fff)'};
        color:${active ? '#0A4BD2' : 'var(--text-main,#1E293B)'};user-select:none;">
        ${f}
      </label>`;
    }).join('')}
  `;
}

function setGroupBy(field) {
  builderState.groupBy = field;
  renderGroupOptions();
  updateLiveSummary();
}

function renderSortFields() {
  const fields = builderSourceFields[builderState.source] || [];
  const sel = document.getElementById('sort-field');
  if (!sel) return;
  sel.innerHTML = `<option value="">— None —</option>` + fields.map(f => `<option>${f}</option>`).join('');
  sel.value = builderState.sortField || '';
  sel.onchange = () => { builderState.sortField = sel.value || null; updateLiveSummary(); };
}

function setSortOrder(order) {
  builderState.sortOrder = order;
  const asc  = document.getElementById('sort-asc');
  const desc = document.getElementById('sort-desc');
  if (asc) {
    asc.style.borderColor  = order === 'asc'  ? '#0A4BD2' : 'var(--border,#E2E8F0)';
    asc.style.background   = order === 'asc'  ? '#EFF6FF' : '#fff';
    asc.style.color        = order === 'asc'  ? '#0A4BD2' : '#64748B';
  }
  if (desc) {
    desc.style.borderColor = order === 'desc' ? '#0A4BD2' : 'var(--border,#E2E8F0)';
    desc.style.background  = order === 'desc' ? '#EFF6FF' : '#fff';
    desc.style.color       = order === 'desc' ? '#0A4BD2' : '#64748B';
  }
  updateLiveSummary();
}

function setOutputFormat(fmt) {
  builderState.format = fmt;
  ['PDF','Excel','CSV'].forEach(f => {
    const el = document.getElementById('fmt-' + f);
    if (el) {
      el.style.borderWidth  = f === fmt ? '2px' : '1px';
      el.style.borderColor  = f === fmt ? '#0A4BD2' : 'var(--border,#E2E8F0)';
      el.style.background   = f === fmt ? '#EFF6FF' : 'var(--card-bg,#fff)';
      el.style.color        = f === fmt ? '#0A4BD2' : '#64748B';
    }
  });
  updateLiveSummary();
  // sync schedule
  const sched = document.getElementById('output-schedule');
  if (sched) sched.onchange = () => { builderState.schedule = sched.value; updateLiveSummary(); };
}

function updateLiveSummary() {
  const set = (id, val) => { const el = document.getElementById(id); if (el) el.textContent = val; };
  set('sum-source',   builderState.source);
  set('sum-fields',   builderState.fields.length > 0 ? builderState.fields.join(', ') : 'None');
  const filterCount = document.querySelectorAll('#filter-rows > div').length;
  set('sum-filters',  filterCount > 0 ? `${filterCount} condition${filterCount > 1 ? 's' : ''}` : 'None');
  set('sum-group',    builderState.groupBy || 'None');
  set('sum-sort',     builderState.sortField ? `${builderState.sortField} (${builderState.sortOrder})` : 'None');
  set('sum-format',   builderState.format);
  const sched = document.getElementById('output-schedule');
  set('sum-schedule', sched ? sched.value : builderState.schedule);
}

function generateCustomReport() {
  const name = document.getElementById('output-report-name')?.value.trim() || `${builderState.source} Custom Report`;
  if (builderState.fields.length === 0) {
    showToast('⚠️ Please select at least one field in Step 2', '#F59E0B');
    switchBuilderTab(1);
    return;
  }
  const sched = document.getElementById('output-schedule')?.value || 'None';
  // Add to library
  reportLibraryData.unshift({
    name:          name,
    category:      builderState.source,
    createdBy:     'Admin',
    lastGenerated: 'Just now',
    schedule:      sched,
    format:        builderState.format,
    status:        'Active'
  });
  filterLibrary();
  updateMetrics();
  updateCategoryCounts();
  // Trigger download in chosen format
  triggerDownload(builderState.format.toLowerCase());
  showToast(`✅ "${name}" generated & added to the library!`, '#15803D');
}

function quickAction(action) {
  showToast(`Initiating: ${action}`, '#0A4BD2');
}

// ── TOAST ─────────────────────────────────────────────────────────
function showToast(msg, bg) {
  const el = document.getElementById('toast');
  if (!el) return;
  el.textContent = msg;
  el.style.background = bg || '#15803D';
  el.classList.add('show');
  setTimeout(() => el.classList.remove('show'), 3000);
}

// ── INIT ─────────────────────────────────────────────────────────
function initReports() {
  renderLibrary(reportLibraryData);
  updateMetrics();
  updateCategoryCounts();
  // Close overlays on backdrop click
  ['view-report-overlay','download-format-overlay','edit-report-overlay'].forEach(id => {
    const el = document.getElementById(id);
    if (el) {
      el.addEventListener('click', function(e) {
        if (e.target === this) {
          this.style.display = 'none';
          document.body.style.overflow = '';
        }
      });
    }
  });
}

// Run immediately if DOM is ready, otherwise wait for it
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', initReports);
} else {
  initReports();
}

