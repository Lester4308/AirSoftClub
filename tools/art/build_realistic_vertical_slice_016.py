import bpy, math
from pathlib import Path
from mathutils import Vector
ROOT=Path(r'C:/Users/Ihor/Documents/ChatGPT/Airsoft_Club_Game')
SRC=ROOT/'art/realistic-3d-vertical-slice/source/mpfb_base_created.blend'
OUT=ROOT/'art/realistic-3d-vertical-slice'
bpy.ops.wm.open_mainfile(filepath=str(SRC))

def clear(): bpy.ops.object.select_all(action='SELECT'); bpy.ops.object.delete(use_global=False)
def mat(name,color,metal=0.0,rough=.5):
 m=bpy.data.materials.new(name); m.diffuse_color=(*color,1); m.use_nodes=True; p=m.node_tree.nodes.get('Principled BSDF'); p.inputs['Base Color'].default_value=(*color,1); p.inputs['Metallic'].default_value=metal; p.inputs['Roughness'].default_value=rough; return m
def cube(name,loc,scale,ma,bev=.03):
 bpy.ops.mesh.primitive_cube_add(location=loc); o=bpy.context.object;o.name=name;o.scale=scale; bpy.ops.object.transform_apply(location=False,rotation=False,scale=True); b=o.modifiers.new('EdgeBevel','BEVEL');b.width=bev;b.segments=2;o.data.materials.append(ma);return o
def cyl(name,loc,r,depth,ma,rot=(0,0,0),vertices=20):
 bpy.ops.mesh.primitive_cylinder_add(vertices=vertices,radius=r,depth=depth,location=loc,rotation=rot);o=bpy.context.object;o.name=name;o.data.materials.append(ma);b=o.modifiers.new('EdgeBevel','BEVEL');b.width=.015;b.segments=2;return o
def uv(name,loc,scale,ma):
 bpy.ops.mesh.primitive_uv_sphere_add(segments=32,ring_count=20,location=loc);o=bpy.context.object;o.name=name;o.scale=scale;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(ma);return o
fabric=mat('MAT_Fabric_Olive',(0.10,.14,.10),0,.82); cord=mat('MAT_Cordura',(0.055,.07,.055),0,.72); polymer=mat('MAT_Polymer',(0.025,.032,.03),0,.42); metal=mat('MAT_PaintedMetal',(.045,.055,.052),.75,.28); rubber=mat('MAT_Rubber',(.018,.02,.019),0,.92); skinM=mat('MAT_Skin_Male',(.34,.19,.13),0,.5); skinF=mat('MAT_Skin_Female',(.43,.26,.19),0,.48); glass=mat('MAT_OpticGlass',(.025,.12,.13),.15,.12); accent=mat('MAT_TeamTape',(.08,.55,.68),.05,.35)

def set_gender(o,gender):
 keys=o.data.shape_keys.key_blocks
 for k in keys:
  if k.name!='Basis': k.value=0
 ethnicity=['$md-$as-','$md-$ca-','$md-$af-']
 suffix='$fe-' if gender=='female' else '$ma-'
 for pre in ethnicity:
  for k in keys:
   if pre in k.name and suffix in k.name: k.value=.333
 for k in keys:
  if 'universal-' in k.name and suffix in k.name: k.value=1.0
 if gender=='female':
  for k in keys:
   if '$md-$fe-' in k.name: k.value=.16

def duplicate_body(gender,x):
 src=bpy.data.objects['Human']; o=src.copy();o.data=src.data.copy();bpy.context.collection.objects.link(o);o.name=f'CHR_{gender.title()}_Body';o.location.x=x;set_gender(o,gender);o.data.materials.clear();o.data.materials.append(skinF if gender=='female' else skinM);return o

def gear(gender,x):
 # Uniform uses sculpted body as visible anatomical foundation; blockout gear proves volume hierarchy.
 torso=cube(f'{gender}_UniformTorso',(x,0,1.16),(.24,.15,.34),fabric,.06)
 pelvis=cube(f'{gender}_UniformPelvis',(x,0,.86),(.20,.14,.16),fabric,.05)
 carrier=cube(f'{gender}_Carrier',(x,-.165,1.17),(.225,.055,.245),cord,.035)
 plate=cube(f'{gender}_FrontPlate',(x,-.227,1.18),(.16,.023,.17),polymer,.025)
 for i in (-.11,0,.11): cube(f'{gender}_Pouch_{i}',(x+i,-.27,1.02),(.045,.04,.075),cord,.012)
 # belt + knee protection visual volume
 cube(f'{gender}_Belt',(x,0,.80),(.205,.155,.028),cord,.012)
 for dx in (-.115,.115): cube(f'{gender}_Knee_{dx}',(x+dx,-.11,.47),(.07,.035,.09),polymer,.022)
 # helmet shell / ear protection / visor
 uv(f'{gender}_Helmet',(x,-.01,1.61),(.145,.13,.105),polymer)
 cube(f'{gender}_EyePro',(x,-.132,1.57),(.115,.018,.025),glass,.015)
 for dx in (-.155,.155): cyl(f'{gender}_EarPro_{dx}',(x+dx,-.005,1.57),.038,.035,polymer,(0,math.pi/2,0),20)
 # team tape deliberately local, not full body tint
 cube(f'{gender}_TeamTape',(x,-.172,.79),(.105,.012,.018),accent,.005)

def rifle(x=0,y=-.52,z=1.12):
 cube('WPN_AR_Receiver',(x,y,z),(.30,.045,.07),metal,.018)
 cube('WPN_AR_Handguard',(x+.36,y,z+.005),(.17,.037,.052),polymer,.015)
 cyl('WPN_AR_Barrel',(x+.64,y,z+.005),.018,.29,metal,(0,math.pi/2,0),24)
 cyl('WPN_AR_Muzzle',(x+.80,y,z+.005),.028,.09,metal,(0,math.pi/2,0),24)
 cube('WPN_AR_Stock',(x-.43,y,z+.01),(.16,.052,.075),polymer,.025)
 cube('WPN_AR_Grip',(x-.14,y,z-.12),(.04,.042,.12),rubber,.018); bpy.context.object.rotation_euler.y=-.22
 mag=cube('WPN_AR_Magazine',(x+.03,y,z-.16),(.055,.037,.13),polymer,.025);mag.rotation_euler.y=-.10
 cube('WPN_AR_TopRail',(x+.08,y,z+.084),(.33,.027,.012),metal,.005)
 cube('WPN_AR_OpticBase',(x+.03,y,z+.115),(.07,.04,.026),metal,.01)
 cyl('WPN_AR_Optic',(x+.03,y,z+.17),.045,.14,metal,(0,math.pi/2,0),24)
 cyl('WPN_AR_Lens',(x-.045,y,z+.17),.036,.008,glass,(0,math.pi/2,0),24)
 # documented sockets as empties
 for n,loc in {'GripPrimary':(x-.14,y-.05,z-.12),'GripSupport':(x+.38,y-.05,z-.07),'StockShoulder':(x-.58,y,z+.01),'Muzzle':(x+.85,y,z+.005),'Magazine':(x+.03,y,z-.16),'OpticRail':(x+.05,y,z+.10)}.items():
  e=bpy.data.objects.new(n,None);bpy.context.collection.objects.link(e);e.location=loc;e.empty_display_type='PLAIN_AXES';e.empty_display_size=.06

def silhouette_weapon(name,x,z,length,body_h,stock,mag=True,barrel=.22):
 c=bpy.data.collections.get('WeaponSilhouettes');
 if not c: c=bpy.data.collections.new('WeaponSilhouettes');bpy.context.scene.collection.children.link(c)
 objs=[]
 objs.append(cube(name+'_Body',(x,1.0,z),(length*.30,.035,body_h),polymer,.012));objs.append(cube(name+'_Stock',(x-length*.42,1.0,z),(length*.14,.04,stock),polymer,.015));objs.append(cyl(name+'_Barrel',(x+length*.43,1,z),.012,barrel,metal,(0,math.pi/2,0),16))
 if mag: objs.append(cube(name+'_Magazine',(x,1,z-.09),(.035,.03,.09),polymer,.012))
 for o in objs:
  for cc in list(o.users_collection): cc.objects.unlink(o)
  c.objects.link(o)

src=bpy.data.objects['Human'];src.hide_render=True;src.hide_viewport=True
male=duplicate_body('male',-.65);female=duplicate_body('female',.65);gear('male',-.65);gear('female',.65);rifle(-.65,-.42,1.18)
for i,(n,l,h,s,mg,b) in enumerate([('Pistol',.45,.045,.04,True,.09),('SMG',.70,.055,.06,True,.16),('AssaultRifle',.90,.065,.075,True,.25),('Shotgun',1.02,.05,.065,False,.40),('DMR',1.10,.055,.07,True,.38),('SniperRifle',1.25,.05,.075,True,.55)]): silhouette_weapon(n,-2.5+i,2.15,l,h,s,mg,b)
# floor/background
floor=mat('MAT_Floor',(.018,.022,.02),0,.78); cube('Floor',(0,0,-.03),(4,2.5,.03),floor,.01)
# Camera looking slightly downward, characters and weapons visible
bpy.ops.object.camera_add(location=(3.9,-7.6,3.1));cam=bpy.context.object;cam.name='CAM_Catalog';bpy.context.scene.camera=cam
def track(o,p): o.rotation_euler=(Vector(p)-o.location).to_track_quat('-Z','Y').to_euler()
track(cam,(0,0,1.05));cam.data.lens=58
# lights
bpy.ops.object.light_add(type='AREA',location=(-3,-4,5));bpy.context.object.data.energy=1050;bpy.context.object.data.shape='DISK';bpy.context.object.data.size=4;track(bpy.context.object,(0,0,1))
bpy.ops.object.light_add(type='AREA',location=(4,-1,2.5));bpy.context.object.data.energy=700;bpy.context.object.data.color=(.35,.58,1);bpy.context.object.data.size=3;track(bpy.context.object,(0,0,1))
bpy.ops.object.light_add(type='AREA',location=(0,2.5,4));bpy.context.object.data.energy=950;bpy.context.object.data.color=(1,.55,.25);bpy.context.object.data.size=2;track(bpy.context.object,(0,0,1.2))
scene=bpy.context.scene;scene.render.engine='BLENDER_EEVEE';scene.render.resolution_x=1600;scene.render.resolution_y=900;scene.render.resolution_percentage=100;scene.render.image_settings.file_format='PNG';scene.render.filepath=str(OUT/'renders/vertical-slice-blockout-016.png');scene.world.color=(.006,.008,.007);scene.view_settings.look='AgX - Medium High Contrast'
(OUT/'renders').mkdir(parents=True,exist_ok=True);(OUT/'source').mkdir(parents=True,exist_ok=True);(OUT/'exports').mkdir(parents=True,exist_ok=True)
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'source/airsoft_vertical_slice_016.blend'))
bpy.ops.render.render(write_still=True)
# Export visible blockout objects as FBX for Unity. Keep source body hidden.
for o in bpy.context.selected_objects:o.select_set(False)
for o in bpy.context.scene.objects:
 if not o.hide_render and o.type in {'MESH','EMPTY'}: o.select_set(True)
bpy.ops.wm.obj_export(filepath=str(OUT/'exports/airsoft_vertical_slice_016.obj'),export_selected_objects=True,export_materials=True)
print('BLOCKOUT_DONE',scene.render.filepath)
