using UnityEditor;
using UnityEngine;

public sealed class Volumetric017SpriteImporter : AssetPostprocessor
{
    const string Prefix = "Assets/Resources/Art/Volumetric017/Sprites/";

    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(Prefix, System.StringComparison.Ordinal)) return;
        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.maxTextureSize = 2048;
        importer.spritePixelsPerUnit = 100f;
    }
}
