#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
public static class LowPolyMale017Setup
{
    [MenuItem("Assets/Low Poly Male 017/Create character from selected pack folder")]
    static void Create()
    {
        string root=AssetDatabase.GetAssetPath(Selection.activeObject);
        if(!AssetDatabase.IsValidFolder(root)||!AssetDatabase.IsValidFolder(root+"/sprites")) { Debug.LogError("Select the imported sprite-pack-male-017 folder containing sprites.");return; }
        string[] files={"body-olive","body-camo","head-bare","head-equipped","arm-front-olive","arm-front-camo","arm-left-olive","arm-left-camo","vest","rifle"};
        var textures=new Texture2D[files.Length];
        for(int i=0;i<files.Length;i++){
            string path=root+"/sprites/"+files[i]+".png";
            var importer=AssetImporter.GetAtPath(path) as TextureImporter;
            if(importer==null){Debug.LogError("Missing "+path);return;}
            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
            importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;importer.maxTextureSize=2048;importer.SaveAndReimport();
            textures[i]=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        var go=new GameObject("Low Poly Male 017");Undo.RegisterCreatedObjectUndo(go,"Create male character");
        var c=go.AddComponent<LowPolyMale017>();
        c.BodyOlive=textures[0];c.BodyCamo=textures[1];c.HeadBare=textures[2];c.HeadEquipped=textures[3];
        c.ArmFrontOlive=textures[4];c.ArmFrontCamo=textures[5];c.ArmRearOlive=textures[6];c.ArmRearCamo=textures[7];c.Vest=textures[8];c.Rifle=textures[9];
        Selection.activeGameObject=go;
        Debug.Log("Character created. Enter Play mode to render; call LowPolyMale017.Fire() for recoil.");
    }
}
#endif

