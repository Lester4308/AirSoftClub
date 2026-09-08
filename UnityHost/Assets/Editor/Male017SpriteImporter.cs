#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Import contract for the male-017 UGUI resource sprites.
public sealed class Male017SpriteImporter : AssetPostprocessor
{
    const string Prefix = "Assets/Resources/Art/Male017/Sprites/";

    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(Prefix, System.StringComparison.Ordinal)) return;
        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100f;
        importer.spritePivot = new Vector2(0f, 1f);
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        importer.sRGBTexture = true;
        importer.isReadable = false;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 2048;
    }

    [MenuItem("Airsoft Club/Reimport Male 017 UI Sprites")]
    static void Reimport()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { Prefix.TrimEnd('/') }))
            AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(guid), ImportAssetOptions.ForceUpdate);
    }
}
#endif
