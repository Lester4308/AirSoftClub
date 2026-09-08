const assert=require('node:assert/strict');
const {ArtRig:r}=require('./rig.js');
let poses=0,maxError=0;
for(const weapon of ['rifle','smg'])for(const aim of [-8,0,8])for(const time of [0,500,1800])for(let age=-20;age<=400;age+=5){
  const p=r.pose(weapon,time,age,true,aim);
  for(const [name,target] of [['front',p.trigger],['rear',p.support]]){
    const arm=r.arms[name],bones=r.articulated(arm,target);assert(bones.reachable,'configured grip must be reachable');
    for(const [actual,expected] of [[r.skin(arm,bones,arm.shoulder),arm.attach],[r.skin(arm,bones,arm.elbow),bones.elbow],[r.skin(arm,bones,arm.grip),target]])assert(Math.hypot(actual[0]-expected[0],actual[1]-expected[1])<1e-8,'skinned joint drift');
    const upper=Math.hypot(bones.elbow[0]-arm.attach[0],bones.elbow[1]-arm.attach[1]);
    const lower=Math.hypot(bones.grip[0]-bones.elbow[0],bones.grip[1]-bones.elbow[1]);
    assert(Math.abs(upper-bones.lengths[0])<1e-8&&Math.abs(lower-bones.lengths[1])<1e-8,'limb length changed');
    for(const matrix of [bones.upper,bones.lower])assert(Math.abs(Math.hypot(matrix[0],matrix[1])-arm.scale)<1e-8,'segment scale changed');
  }
  for(const [name,target] of [['front',p.trigger],['rear',p.support]]){
    const arm=r.arms[name],matrix=p[name];assert(matrix.every(Number.isFinite));
    for(const [actual,expected] of [[r.point(matrix,arm.shoulder),arm.attach],[r.point(matrix,arm.grip),target]]){
      const error=Math.hypot(actual[0]-expected[0],actual[1]-expected[1]);maxError=Math.max(maxError,error);assert(error<1e-8,'shoulder or grip drift');
    }
  }
  assert(p.muzzle[0]>p.support[0],'muzzle must stay ahead of supporting hand');poses++;
}
for(const weapon of ['rifle','smg']){
  const a=r.pose(weapon,0,-1,false),b=r.pose(weapon,2000,90,false);
  assert.deepEqual(a.trigger,b.trigger,'reduced motion must stay still');
  const start=r.pose(weapon,0,0,true),end=r.pose(weapon,0,180,true);
  assert.deepEqual(start.trigger,end.trigger,'recoil must settle');
}
for(const arm of Object.values(r.arms))for(const target of [arm.attach,[10000,10000]]){
  const bones=r.articulated(arm,target);assert(!bones.reachable);assert(bones.upper.concat(bones.lower).every(Number.isFinite));
}
console.log(JSON.stringify({status:'PASS',poses,maxAnchorError:maxError,checks:['shoulder fixed','skinned grip and elbow attached','constant bone lengths and scale','unreachable targets clamped','finite transforms','muzzle ahead','reduced motion','recoil settles']},null,2));

