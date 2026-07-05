const fs = require('fs');
const p = 'D:/Synergech/Project/UIUX/features/reports/reports.html';
let h = fs.readFileSync(p, 'utf8');

// Remove wrongly inserted CSS and container
h = h.replace(/<style>[\s\S]*?<\/style>\s*<div class="audit-pagination" id="audit-pagination-container"><\/div>/, '');

// Insert it properly after audit-table-tbody
const target = '<tbody id="audit-table-tbody">\n                <!-- Dynamic -->\n              </tbody>\n            </table>';
const replacement = target + `
            <style>
              .audit-pagination { display: flex; align-items: center; justify-content: center; gap: 8px; font-size: 0.85rem; font-weight: 500; margin-top: 15px; padding-bottom: 20px;}
              .audit-pagination a { display: inline-flex; align-items: center; justify-content: center; min-width: 32px; height: 32px; border-radius: 6px; color: #475569; text-decoration: none; transition: 0.2s; padding: 0 10px; }
              .audit-pagination a:hover { background: #F1F5F9; color: #0F172A; }
              .audit-pagination a.active { background: #0A4BD2; color: #fff; }
              .audit-pagination span { color: #94A3B8; padding: 0 4px; }
            </style>
            <div class="audit-pagination" id="audit-pagination-container"></div>`;

h = h.replace(target, replacement);
fs.writeFileSync(p, h, 'utf8');
