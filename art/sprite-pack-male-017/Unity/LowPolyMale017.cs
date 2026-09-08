using UnityEngine;

// Standalone visual component. Connect Fire() to your game's shot event.
public sealed class LowPolyMale017 : MonoBehaviour
{
    public Texture2D BodyOlive, BodyCamo, HeadBare, HeadEquipped;
    public Texture2D ArmFrontOlive, ArmFrontCamo, ArmRearOlive, ArmRearCamo, Vest, Rifle;
    public bool Equipped = true, ShowVest = true, ShowWeapon = true, AnimateIdle = true, ReduceMotion;
    public int SortingOrder;
    const float Ppu = 100f;
    SpriteRenderer[] layers;
    Transform visual;
    readonly System.Collections.Generic.Dictionary<Texture2D, Sprite> cache = new System.Collections.Generic.Dictionary<Texture2D, Sprite>();
    float shotTime = -1000f;
    void Awake()
    {
        visual = new GameObject("Visual").transform; visual.SetParent(transform, false);
        layers = new SpriteRenderer[6];
        string[] names = {"Support arm", "Body", "Head", "Vest", "Weapon", "Shooting arm"};
        for (int i=0;i<6;i++) { var go=new GameObject(names[i]);go.transform.SetParent(visual,false);layers[i]=go.AddComponent<SpriteRenderer>(); }
    }
    public void Fire() { if(ShowWeapon) shotTime=Time.time; }
    Sprite Get(Texture2D texture)
    {
        if(texture==null)return null;
        if(!cache.TryGetValue(texture,out var sprite)) { sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0,1),Ppu,0,SpriteMeshType.FullRect);cache.Add(texture,sprite); }
        return sprite;
    }
    void Place(int index, Texture2D texture, Vector2 position, float scale, float rotation=0)
    {
        var r=layers[index];r.sprite=Get(texture);r.sortingOrder=SortingOrder+index;
        r.transform.localPosition=new Vector3((position.x-256)/Ppu,(710-position.y)/Ppu,0);
        r.transform.localRotation=Quaternion.Euler(0,0,rotation);r.transform.localScale=Vector3.one*scale;
    }
    void Arm(int index,Texture2D texture,Vector2 a,Vector2 b,Vector2 c,Vector2 d)
    {
        Vector2 u=b-a,v=d-c;float den=u.sqrMagnitude;
        float co=Vector2.Dot(u,v)/den,si=(u.x*v.y-u.y*v.x)/den;
        Vector2 origin=new Vector2(c.x-co*a.x+si*a.y,c.y-si*a.x-co*a.y);
        Place(index,texture,origin,Mathf.Sqrt(co*co+si*si),-Mathf.Atan2(si,co)*Mathf.Rad2Deg);
    }
    void LateUpdate()
    {
        float age=Time.time-shotTime;
        float kick=!ReduceMotion&&age>=0&&age<.18f?Mathf.Sin(age/.18f*Mathf.PI)*6:0;
        visual.localScale=new Vector3(1,1+(AnimateIdle&&!ReduceMotion?Mathf.Sin(Time.time)*.0018f:0),1);
        Arm(0,Equipped?ArmRearCamo:ArmRearOlive,Equipped?new Vector2(220,180):new Vector2(235,130),Equipped?new Vector2(1100,430):new Vector2(1180,380),new Vector2(340,280),new Vector2(422-kick,320));
        Place(1,Equipped?BodyCamo:BodyOlive,new Vector2(128,183),1.25f);
        Place(2,Equipped?HeadEquipped:HeadBare,Equipped?new Vector2(173,69):new Vector2(155,69),Equipped?.52f:.49f);
        Place(3,ShowVest?Vest:null,new Vector2(169,217),.63f);
        Place(4,ShowWeapon?Rifle:null,new Vector2(175-kick,250),.8f);
        Arm(5,Equipped?ArmFrontCamo:ArmFrontOlive,new Vector2(48,35),new Vector2(252,164),new Vector2(154,249),new Vector2(271-kick,354));
    }
    void OnDestroy() { foreach(var sprite in cache.Values)if(sprite!=null)Destroy(sprite); }
}

