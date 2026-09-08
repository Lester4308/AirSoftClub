'use strict';
// Runtime textured mesh animation. Original PNG/alpha is sampled, never rewritten.
window.ArmSkin=class {
  constructor(image,arm){
    this.image=image;this.arm=arm;this.triangles=[];this.cache=null;
    this.canvas=document.createElement('canvas');this.canvas.width=1024;this.canvas.height=1536;this.ctx=this.canvas.getContext('2d');
    const sample=document.createElement('canvas');sample.width=image.width;sample.height=image.height;
    const sc=sample.getContext('2d',{willReadFrequently:true});sc.drawImage(image,0,0);const rgba=sc.getImageData(0,0,image.width,image.height).data;
    const step=64;
    for(let y=0;y<image.height;y+=step)for(let x=0;x<image.width;x+=step){
      const right=Math.min(x+step,image.width),bottom=Math.min(y+step,image.height);let visible=false;
      for(let py=y;py<bottom&&!visible;py++)for(let px=x;px<right;px++)if(rgba[(py*image.width+px)*4+3]>0){visible=true;break;}
      if(visible){this.triangles.push([[x,y],[right,y],[right,bottom]],[[x,y],[right,bottom],[x,bottom]]);}
    }
  }
  draw(ctx,bones){
    const signature=bones.upper.concat(bones.lower).join(',');
    if(signature!==this.cache){
      this.ctx.clearRect(0,0,1024,1536);
      for(const source of this.triangles)this.triangle(source,source.map(p=>ArtRig.skin(this.arm,bones,p)));
      this.cache=signature;
    }
    ctx.drawImage(this.canvas,0,0);
  }
  triangle(s,d){
    const [p,q,r]=s,[a,b,c]=d,ux=q[0]-p[0],uy=q[1]-p[1],vx=r[0]-p[0],vy=r[1]-p[1],det=ux*vy-uy*vx;
    const ax=b[0]-a[0],ay=b[1]-a[1],bx=c[0]-a[0],by=c[1]-a[1];
    const m=[(ax*vy-bx*uy)/det,(ay*vy-by*uy)/det,(-ax*vx+bx*ux)/det,(-ay*vx+by*ux)/det];
    const center=[(a[0]+b[0]+c[0])/3,(a[1]+b[1]+c[1])/3];
    const padded=d.map(p=>{const dx=p[0]-center[0],dy=p[1]-center[1],length=Math.hypot(dx,dy)||1;return[p[0]+dx/length*.35,p[1]+dy/length*.35];});
    const ctx=this.ctx;ctx.save();ctx.beginPath();ctx.moveTo(...padded[0]);ctx.lineTo(...padded[1]);ctx.lineTo(...padded[2]);ctx.closePath();ctx.clip();
    ctx.transform(...m,a[0]-m[0]*p[0]-m[2]*p[1],a[1]-m[1]*p[0]-m[3]*p[1]);ctx.drawImage(this.image,0,0);ctx.restore();
  }
};
