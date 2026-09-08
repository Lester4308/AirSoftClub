'use strict';
// Source-space anchors; all operations are display-time, original PNGs unchanged.
(function(scope){
const roster={armor:{x:262,y:258,width:408,height:584},face:{x:294,y:154,width:395,height:195},helmet:{x:307,y:14,width:325,height:225},camo:{x:155,y:298,width:707,height:1029}};
const weapons={rifle:{key:'rifle',x:250,y:225,scale:.34,trigger:[660,470],support:[1350,300],muzzle:[2010,265]},smg:{key:'smg',x:250,y:204,scale:.34,trigger:[660,560],support:[1230,405],muzzle:[1510,342]}};
const arms={front:{key:'front',shoulder:[190,170],elbow:[370,750],grip:[1250,400],attach:[220,330],scale:.24},rear:{key:'rear',shoulder:[200,210],elbow:[620,770],grip:[1320,240],attach:[486,328],scale:.19}};
function solve(a,b,c,d){const ux=b[0]-a[0],uy=b[1]-a[1],vx=d[0]-c[0],vy=d[1]-c[1],den=ux*ux+uy*uy;if(den===0)throw new Error('Coincident arm anchors');const co=(ux*vx+uy*vy)/den,si=(ux*vy-uy*vx)/den;return [co,si,-si,co,c[0]-co*a[0]+si*a[1],c[1]-si*a[0]-co*a[1]];}
function point(m,p){return [m[0]*p[0]+m[2]*p[1]+m[4],m[1]*p[0]+m[3]*p[1]+m[5]];}
function articulated(arm,target){
  const s=arm.attach,dx=target[0]-s[0],dy=target[1]-s[1],distance=Math.hypot(dx,dy);
  const l1=Math.hypot(arm.elbow[0]-arm.shoulder[0],arm.elbow[1]-arm.shoulder[1])*arm.scale;
  const l2=Math.hypot(arm.grip[0]-arm.elbow[0],arm.grip[1]-arm.elbow[1])*arm.scale;
  const d=Math.max(Math.abs(l1-l2)+1e-6,Math.min(l1+l2-1e-6,distance)),ux=distance>1e-9?dx/distance:1,uy=distance>1e-9?dy/distance:0;
  const projected=(l1*l1-l2*l2+d*d)/(2*d),bend=Math.sqrt(Math.max(0,l1*l1-projected*projected));
  const elbow=[s[0]+ux*projected-uy*bend,s[1]+uy*projected+ux*bend],grip=[s[0]+ux*d,s[1]+uy*d];
  return {upper:solve(arm.shoulder,arm.elbow,s,elbow),lower:solve(arm.elbow,arm.grip,elbow,grip),elbow,grip,lengths:[l1,l2],reachable:Math.abs(d-distance)<1e-5};
}
function skin(arm,bones,p){
  const dx=arm.grip[0]-arm.shoulder[0],dy=arm.grip[1]-arm.shoulder[1],length=Math.hypot(dx,dy);
  const dot=((p[0]-arm.elbow[0])*dx+(p[1]-arm.elbow[1])*dy)/length;
  const z=Math.max(0,Math.min(1,(dot+125)/250)),w=z*z*(3-2*z),a=point(bones.upper,p),b=point(bones.lower,p);
  return [a[0]*(1-w)+b[0]*w,a[1]*(1-w)+b[1]*w];
}
function pose(name,time,age,motion,aim=0){
  const w=weapons[name];if(!w)throw new Error('Unknown weapon');
  const angle=Math.max(-8,Math.min(8,aim))*Math.PI/180,co=Math.cos(angle),si=Math.sin(angle);
  const kick=motion&&age>=0&&age<180?Math.sin(Math.PI*age/180)*12:0,breath=motion?Math.sin(time/950)*1.6:0;
  // Rotate around the buttstock contact rather than swinging it away from the shoulder.
  const pivot=[290,350],x=pivot[0]+co*(w.x-pivot[0])-si*(w.y-pivot[1])-kick*co;
  const y=pivot[1]+si*(w.x-pivot[0])+co*(w.y-pivot[1])-kick*si+breath;
  const matrix=[co*w.scale,si*w.scale,-si*w.scale,co*w.scale,x,y];
  const world=p=>point(matrix,p),trigger=world(w.trigger),support=world(w.support),muzzle=world(w.muzzle);
  return {w,x,y,matrix,direction:[co,si],trigger,support,muzzle,front:solve(arms.front.shoulder,arms.front.grip,arms.front.attach,trigger),rear:solve(arms.rear.shoulder,arms.rear.grip,arms.rear.attach,support)};
}
scope.ArtRig={roster,weapons,arms,solve,point,pose,articulated,skin};
})(typeof module!=='undefined'?module.exports:window);
