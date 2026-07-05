const fs = require('fs');
const html = fs.readFileSync('D:/Synergech/Project/UIUX/features/index.html', 'utf8');
console.log('Has loginPromptModal?', html.includes('loginPromptModal'));
console.log('Apply buttons have toggle?', html.includes('data-bs-target="#loginPromptModal"'));
