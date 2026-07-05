const fs = require('fs');
const path = require('path');
function walk(dir) {
  let res=[];
  fs.readdirSync(dir).forEach(f=>{
    f=path.join(dir,f);
    if(fs.statSync(f).isDirectory()) res=res.concat(walk(f));
    else if(f.endsWith('.js')) res.push(f);
  });
  return res;
}
walk('D:/Synergech/Project/UIUX/features').forEach(f=>{
  const c = fs.readFileSync(f, 'utf8');
  const m = c.match(/href=["'][^"']*?\.html[^"']*?["']/g);
  if(m) console.log(f, m);
});
