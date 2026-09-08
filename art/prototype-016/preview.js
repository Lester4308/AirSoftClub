'use strict';
const images={},skins={},el=id=>document.getElementById(id);
const ac=el('assembly').getContext('2d'),rc=el('rig').getContext('2d'),bc=el('battle').getContext('2d');
const reduced=matchMedia('(prefers-reduced-motion: reduce)');
let idle=false,shotAt=-10000,ready=false,raf=null,last=0,previewAge=null;
window.artTest={loaded:false,drawCount:0,version:7};
function footShadow(ctx,x,y,rx,ry){
  ctx.save();ctx.translate(x,y);ctx.scale(rx,ry);
  const shade=ctx.createRadialGradient(0,0,0,0,0,1);
  shade.addColorStop(0,'rgba(0,0,0,.25)');shade.addColorStop(1,'rgba(0,0,0,0)');
  ctx.fillStyle=shade;ctx.beginPath();ctx.arc(0,0,1,0,Math.PI*2);ctx.fill();ctx.restore();
}
function layer(ctx,key,p){ctx.drawImage(images[key],p.x,p.y,p.width,p.height);}
function clipped(ctx,polygons,paint){
  ctx.save();ctx.beginPath();
  for(const polygon of polygons){ctx.moveTo(...polygon[0]);for(const p of polygon.slice(1))ctx.lineTo(...p);ctx.closePath();}
  ctx.clip();paint();ctx.restore();
}
const rect=(x,y,w,h)=>[[x,y],[x+w,y],[x+w,y+h],[x,y+h]];
function roster(ctx,x,y,h,preset,t){
  ctx.save();ctx.translate(x,y);ctx.scale(h/1536,h/1536);
  const breathe=idle&&!reduced.matches?Math.sin(t/850)*.0025:0;
  ctx.translate(512,1484);ctx.scale(1,1+breathe);ctx.translate(-512,-1484);
  const camo=preset>0&&el('camo').checked,helmet=preset===2&&el('helmet').checked,armor=preset===2&&el('armor').checked;
  // Clothing is a replacement silhouette. Render only exposed body regions beneath it.
  // The helmet hides the hair instead of stacking on an unmodified spiky silhouette.
  const head=helmet?rect(335,158,280,195):rect(0,0,1024,350);
  if(camo){
    clipped(ctx,[head,rect(250,794,126,150),rect(640,794,90,150),rect(0,1260,1024,276)],()=>ctx.drawImage(images.base,0,0,1024,1536));
    layer(ctx,'camo',ArtRig.roster.camo);
  }else if(helmet){
    clipped(ctx,[head,rect(0,340,1024,1196)],()=>ctx.drawImage(images.base,0,0,1024,1536));
  }else ctx.drawImage(images.base,0,0,1024,1536);
  if(armor){
    layer(ctx,'armor',ArtRig.roster.armor);
    // Sleeves pass in front of the carrier's side straps/pouches.
    const sleeves=[[[210,415],[307,387],[361,425],[355,565],[347,650],[345,800],[210,825]],[[628,425],[714,440],[745,810],[659,825],[632,610]]];
    clipped(ctx,sleeves,()=>camo?layer(ctx,'camo',ArtRig.roster.camo):ctx.drawImage(images.base,0,0,1024,1536));
  }
  if(helmet)layer(ctx,'helmet',ArtRig.roster.helmet);
  if(preset>0&&el('face').checked)layer(ctx,'face',ArtRig.roster.face);
  ctx.restore();
}
function combat(ctx,x,y,h,t,age,inspect=false){
  const p=ArtRig.pose(el('weapon').value,t,age,!reduced.matches&&(idle||age>=0&&age<180),Number(el('aim').value));
  const front=ArtRig.articulated(ArtRig.arms.front,p.trigger),rear=ArtRig.articulated(ArtRig.arms.rear,p.support);
  ctx.save();ctx.translate(x,y);ctx.scale(h/1536,h/1536);
  footShadow(ctx,176,1480,115,24);footShadow(ctx,614,1450,148,25);
  if(el('rear').checked)skins.rear.draw(ctx,rear);
  ctx.drawImage(images.combat,-32,0,1024,1536);
  if(el('gun').checked){ctx.save();ctx.transform(...p.matrix);ctx.drawImage(images[p.w.key],0,0);ctx.restore();}
  // The support fingers wrap in front of the handguard; the sleeve remains behind it.
  if(el('rear').checked&&el('gun').checked){
    ctx.save();ctx.transform(...rear.lower);
    clipped(ctx,[[[1184,110],[1390,40],[1525,90],[1530,280],[1370,367],[1220,359]]],()=>ctx.drawImage(images.rear,0,0));
    ctx.restore();
  }
  if(el('front').checked)skins.front.draw(ctx,front);
  if(inspect&&el('anchors').checked){
    const points=[['Плече',ArtRig.arms.front.attach],['Лікоть',front.elbow],['Хват',front.grip],['Лікоть',rear.elbow],['Цівка',rear.grip],['Дуло',p.muzzle]];
    const anchorColor=el('background').value==='#a3a49a'?'#214c41':'#a4e4d3';
    ctx.lineWidth=3;ctx.strokeStyle=anchorColor;ctx.fillStyle=anchorColor;ctx.font='22px system-ui';
    for(const [label,xy] of points){ctx.beginPath();ctx.arc(...xy,7,0,Math.PI*2);ctx.stroke();ctx.fillText(label,xy[0]+10,xy[1]-10);}
  }
  if(el('gun').checked&&age>=0&&age<320){
    if(age<60){ctx.fillStyle='#f4d79c';ctx.beginPath();ctx.arc(...p.muzzle,7,0,Math.PI*2);ctx.fill();}
    ctx.fillStyle=el('bb').value;ctx.beginPath();ctx.arc(p.muzzle[0]+age*2*p.direction[0],p.muzzle[1]+age*2*p.direction[1],5,0,Math.PI*2);ctx.fill();
  }
  ctx.restore();return p;
}
function clear(ctx,w,h){ctx.fillStyle=el('background').value;ctx.fillRect(0,0,w,h);}
function draw(t){
  if(!ready)return;const age=previewAge===null?t-shotAt:previewAge;
  if(previewAge!==null)t=0;
  clear(ac,1152,630);for(let i=0;i<3;i++)roster(ac,i*384-7,14,605,i,t);
  clear(rc,1152,660);combat(rc,40,20,620,t,age,true);
  rc.fillStyle=el('background').value==='#a3a49a'?'#18211c':'#c4ccc3';rc.font='18px system-ui';rc.fillText('Окремі PNG: тіло + 2 руки + зброя',540,72);
  rc.fillText('Плече й передпліччя мають сталу довжину',540,106);
  rc.fillText('Ноги залишаються нерухомими',540,140);
  rc.fillText(el('weapon').value==='rifle'?'Assault Rifle':'SMG · компактний хват',540,190);
  rc.fillText('Хват зблизька',540,244);
  clipped(rc,[rect(530,260,602,360)],()=>combat(rc,380,105,1152,t,age,true));
  const n=Number(el('count').value),rows=n===16?4:n===4?2:1,cols=rows,h=n===16?128:n===4?245:460;
  clear(bc,1152,620);bc.strokeStyle='#58635b';
  for(let r=0;r<rows;r++){
    const y=rows===1?110:rows===2?65+r*265:55+r*139;
    bc.beginPath();bc.moveTo(20,y+h);bc.lineTo(1132,y+h);bc.stroke();
    for(let c=0;c<cols;c++)for(let side=0;side<2;side++){
      const x=rows===1?90:rows===2?48+c*230:22+c*119;
      bc.save();if(side){bc.translate(1152,0);bc.scale(-1,1);}
      combat(bc,x,y,h,t,age);
      bc.fillStyle=side?'#b88155':'#7aa994';bc.fillRect(x+h*.1,y-8,h*.27,4);bc.restore();
    }
  }
  bc.fillStyle=el('background').value==='#a3a49a'?'#18211c':'#e3e5db';bc.font='16px system-ui';bc.fillText(`${n}v${n} · модульна поза · ${h}px`,22,26);
  window.artTest.drawCount++;
}
function tick(t){raf=null;if(t-last>30){draw(t);last=t;}if(idle||t-shotAt<500)raf=requestAnimationFrame(tick);}
function wake(){if(raf===null)raf=requestAnimationFrame(tick);}
el('idle').onclick=()=>{idle=!idle;el('idle').setAttribute('aria-pressed',String(idle));el('idle').textContent=idle?'Зупинити idle':'Увімкнути idle';wake();};
function fire(){if(!el('gun').checked)return;previewAge=null;el('phaseValue').textContent='Відтворення';shotAt=performance.now();wake();}
el('fire').onclick=fire;el('rigFire').onclick=fire;
el('phase').oninput=()=>{previewAge=Number(el('phase').value);shotAt=-10000;el('phaseValue').textContent=previewAge+' мс · пауза';draw(performance.now());};
for(const id of ['armor','face','helmet','camo','count','bb','background','weapon','front','rear','gun','anchors','aim'])el(id).onchange=()=>draw(performance.now());
const assets={base:'male-base',armor:'armor',face:'face-protection',helmet:'helmet-shell-v2',camo:'camouflage-v2',combat:'combat-body-v3',front:'arm-front-v2',rear:'arm-rear-v3',rifle:'rifle-v2',smg:'smg-v2'};
Promise.all(Object.entries(assets).map(([key,name])=>new Promise((resolve,reject)=>{const img=new Image();img.onload=()=>{images[key]=img;resolve();};img.onerror=()=>reject(new Error(`Не завантажено ${name}.png`));img.src=`assets/${name}.png`;}))).then(()=>{for(const key of ['front','rear'])skins[key]=new ArmSkin(images[key],ArtRig.arms[key]);ready=true;window.artTest.loaded=true;el('status').textContent='Ревізія 7 · три положення зброї · пальці охоплюють цівку · ліва рука без видимої кишені';draw(performance.now());}).catch(err=>{el('error').textContent=err.message;});





