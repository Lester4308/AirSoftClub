from PIL import Image, ImageFilter, ImageOps, ImageDraw, ImageChops
from pathlib import Path
import random

root = Path('art/volumetric-017/source/weapons')
out = Path('UnityHost/Assets/Resources/Art/Volumetric017/Weapons')
review_dir = Path('art/volumetric-017/review')
files = ['pistol','smg','assault-rifle','pump-shotgun','dmr','sniper-rifle']

def enhance_volume(im):
    """Realistic-stylized volumetric 2D enhancement via PIL."""
    rgba = im.convert('RGBA')
    alpha = rgba.getchannel('A')
    if alpha.getbbox() is None:
        return rgba
    # --- Ambient occlusion: darker near inner edges ---
    # compute distance transform of alpha (approximated by blurring alpha)
    a_blur = alpha.filter(ImageFilter.GaussianBlur(6))
    # AO mask: dark regions near opaque boundary
    # use inverted dilated-minus-normal approach
    edges = alpha.filter(ImageFilter.FIND_EDGES).filter(ImageFilter.GaussianBlur(5))
    ao = ImageOps.invert(alpha.filter(ImageFilter.GaussianBlur(2)))
    ao = ao.filter(ImageFilter.GaussianBlur(8))
    ao = ao.point(lambda v: int(v * 0.55))  # up to ~55% darkening at edges

    r,g,b,a_out = rgba.split()
    # Apply AO: multiply RGB by (1 - ao*strength)
    ao_strength = ao.point(lambda v: 255 - int(v * 0.30))  # scale to (225-255)
    r = ImageChops.multiply(r, ao_strength)
    g = ImageChops.multiply(g, ao_strength)
    b = ImageChops.multiply(b, ao_strength)
    # normalize back — multiply by factor>1 to restore brightness
    # after multiply, values scaled by (0.88..1.0) so brighten slightly
    r = r.point(lambda v: min(255, int(v * 1.06)))
    g = g.point(lambda v: min(255, int(v * 1.06)))
    b = b.point(lambda v: min(255, int(v * 1.06)))

    # --- Rim light: warm highlight on top-left edges ---
    # build rim mask from alpha erosion
    erosion_amount = 3
    a_dilated = alpha.filter(ImageFilter.MaxFilter(5))  # grow
    rim_raw = ImageChops.subtract(a_dilated, alpha)     # thin ring
    rim = rim_raw.filter(ImageFilter.GaussianBlur(2))
    # only apply rim on upper-left portion (directional), skip bottom-right strongly
    w,h = rim.size
    rim_grad = rim.copy()
    px = rim_grad.load()
    for y in range(h):
        for x in range(w):
            # warm light from upper-left: weight by distance from bottom-right
            dist = (x/w + (1-y/h)) / 2.0
            if rim.getpixel((x,y)) > 0:
                v = int(rim.getpixel((x,y)) * (0.5 + dist*0.6))
                px[x,y] = v
            else:
                px[x,y] = 0
    rim_light = rim_grad.point(lambda v: int(v * 0.9))
    # warm tint
    warm_r = rim_light.point(lambda v: min(255,int(v*1.25)))
    warm_g = rim_light.point(lambda v: min(255,int(v*0.98)))
    warm_b = rim_light.point(lambda v: min(255,int(v*0.85)))
    r = ImageChops.add(r, warm_r)
    g = ImageChops.add(g, warm_g)
    b = ImageChops.add(b, warm_b)

    # --- Material grain: subtle noise only inside alpha ---
    width,height = alpha.size
    noise = Image.effect_noise((width,height), 24).convert('L')  # uniform noise
    # keep subtle
    grain = noise.point(lambda v: int((v-128)*0.18)+128)
    grain_a = grain.filter(ImageFilter.GaussianBlur(0.6))
    # composite grain via screen-ish only where alpha > 200
    grain_merged = ImageChops.screen(Image.merge('RGB',[r,g,b]), grain_a.convert('RGB'))
    r,g,b = grain_merged.split()

    result = Image.merge('RGBA',(r,g,b,alpha))
    return result

# process each v2 source -> enhanced master
for slug in files:
    src = root/f'{slug}-v2.png'
    if not src.exists():
        print(f'MISSING {src}')
        continue
    im = Image.open(src).convert('RGBA')
    enh = enhance_volume(im)
    # save enhanced master in review + source
    damage_dir = review_dir/'enhanced'
    damage_dir.mkdir(parents=True, exist_ok=True)
    enh.save(review_dir/f'{slug}-enhanced.png')
    # canvas the background around content for card/icon (same as before)
    box = enh.getchannel('A').getbbox()
    crop = enh.crop(box)
    pad = max(crop.width//18, 16)
    canvas = Image.new('RGBA',(crop.width+2*pad, crop.height+2*pad))
    canvas.alpha_composite(crop,(pad,pad))
    for tag,w in [('card',512),('icon',256)]:
        scale=w/canvas.width
        canvas.resize((w,max(1,round(canvas.height*scale))),Image.Resampling.LANCZOS).save(out/f'{slug}-{tag}.png',optimize=True)
    # silhouette
    scale=min(116/canvas.width,52/canvas.height)
    sz=(max(1,round(canvas.width*scale)),max(1,round(canvas.height*scale)))
    a=canvas.getchannel('A').resize(sz,Image.Resampling.LANCZOS)
    blk=Image.new('RGBA',sz,(0,0,0,255)); blk.putalpha(a)
    sil=Image.new('RGBA',(128,64)); sil.alpha_composite(blk,((128-sz[0])//2,(64-sz[1])//2))
    sil.save(out/f'{slug}-silhouette.png')
    print(f'ENHANCED {slug} -> {box}')

# build final review contact sheet
review=Image.new('RGBA',(1500,1020),(24,27,25,255));d=ImageDraw.Draw(review)
for i,slug in enumerate(files):
    crop=Image.open(review_dir/f'{slug}-enhanced.png').convert('RGBA')
    box=crop.getchannel('A').getbbox(); crop=crop.crop(box)
    crop.thumbnail((680,260));x=35+(i%2)*740+(680-crop.width)//2;y=55+(i//2)*320+(260-crop.height)//2
    review.alpha_composite(crop,(x,y));d.text((35+(i%2)*740,20+(i//2)*320),slug.upper(),fill='white')
rp=review_dir/'weapons-six-v3-enhanced.png'
review.save(rp); print(rp)
