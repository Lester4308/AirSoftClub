'use strict';
const canvas=document.querySelector('#view'),ctx=canvas.getContext('2d'),images={};
const names=['body-olive','body-camo','head-bare','head-equipped','arm-front-olive','arm-front-camo','arm-left-olive','arm-left-camo','vest','rifle'];
let fired=-1e4;
function solve(a,b,c,d){let ux=b[0]-a[0],uy=b[1]-a[1],vx=d[0]-c[0],vy=d[1]-c[1],den=ux*ux+uy*uy,co=(ux*vx+uy*vy)/den,si=(ux*vy-uy*vx)/den;return[co,si,-si,co,c[0]-co*a[0]+si*a[1],c[1]-si*a[0]-co*a[1]];}
function draw(name,x,y,s){const im=images[name];ctx.drawImage(im,x,y,im.width*s,im.height*s);}
function arm(name,a,b,c,d){ctx.save();ctx.transform(...solve(a,b,c,d));draw(name,0,0,1);ctx.restore();}
function frame(t){const equipped=document.querySelector('#equipped').checked,c=equipped?'camo':'olive',motion=document.querySelector('#motion').checked&&!matchMedia('(prefers-reduced-motion: reduce)').matches;
ctx.fillStyle='#a8a99e';ctx.fillRect(0,0,512,768);const breathe=motion?Math.sin(t/1000)*1.2:0,age=t-fired,kick=!matchMedia('(prefers-reduced-motion: reduce)').matches&&age>=0&&age<180?Math.sin(age/180*Math.PI)*6:0;
ctx.save();ctx.translate(256,710);ctx.scale(1,1+(motion?Math.sin(t/1000)*.0018:0));ctx.translate(-256,-710);const trigger=[271-kick,354],support=[422-kick,320];
arm('arm-left-'+c,equipped?[220,180]:[235,130],equipped?[1100,430]:[1180,380],[340,280],support);
draw('body-'+c,128,183,1.25);
draw(equipped?'head-equipped':'head-bare',equipped?173:155,69,equipped?.52:.49);
if(document.querySelector('#vest').checked)draw('vest',169,217,.63);
if(document.querySelector('#gun').checked)draw('rifle',175-kick,250,.8);
arm('arm-front-'+c,[48,35],[252,164],[154,249],trigger);
ctx.restore();requestAnimationFrame(frame);}
document.querySelector('#fire').onclick=()=>fired=performance.now();
Promise.all(names.map(name=>new Promise((ok,bad)=>{let im=new Image();im.onload=()=>{images[name]=im;ok();};im.onerror=bad;im.src='sprites/'+name+'.png';}))).then(()=>requestAnimationFrame(frame));



