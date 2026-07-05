const fs = require('fs');
const lines = fs.readFileSync('C:/Users/harin/.gemini/antigravity/brain/705380e7-3f24-4d39-a6bb-8b6bd8731539/.system_generated/logs/transcript.jsonl', 'utf8').split('\n');
lines.forEach((l, i) => {
  if (l.includes('"step_index":6630') || l.includes('"step_index":6615') || l.includes('"step_index":6636') || l.includes('"step_index":6720') || l.includes('"step_index":6604') || l.includes('"step_index":6512')) {
    try {
      const p = JSON.parse(l);
      const args = p.tool_calls?.[0]?.args || (p.tool_calls ? JSON.parse(p.tool_calls[0].function.arguments) : null);
      if (args && args.TargetFile) {
        console.log('Step ' + p.step_index + ' edited ' + args.TargetFile);
        if (args.CodeContent) console.log('Code length: ' + args.CodeContent.length);
        if (args.ReplacementChunks) console.log('Replacements: ' + args.ReplacementChunks.length);
      }
    } catch(e) {}
  }
});
