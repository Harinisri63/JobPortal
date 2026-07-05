const fs = require('fs');
const lines = fs.readFileSync('D:/Synergech/Project/UIUX/scripts/history.log', 'utf8').split('\n');
lines.forEach((l, i) => {
  if(l.includes('write_to_file')) {
    try {
      const p = JSON.parse(l);
      const args = p.tool_calls?.[0]?.args || (p.tool_calls ? JSON.parse(p.tool_calls[0].function.arguments) : null);
      if(args && args.TargetFile && args.TargetFile.includes('index.html')) {
        const html = args.CodeContent;
        if(html.includes('About Us') && html.includes('<section')) {
           const match = html.match(/<section[^>]*>[\s\S]*?About Us[\s\S]*?<\/section>/);
           if(match) console.log("FOUND SECTION: ", match[0].substring(0, 300));
        }
      }
    } catch(e){}
  }
});
