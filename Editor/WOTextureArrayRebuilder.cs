using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace WildernessOverhaul.Editor
{
    public static class WOTextureArrayRebuilder
    {
        private const string SourceRoot = "Assets/Game/Mods/daggerfall-wilderness-overhaul/Textures/TextureFiles";
        private const string TargetRoot = "Assets/Game/Mods/daggerfall-wilderness-overhaul/Textures";
        private const string VanillaRoot = "Assets/Game/Mods/daggerfall-wilderness-overhaul/Textures/VanillaTextureFiles";
        private const int TerrainSliceCount = 56;

        [MenuItem("Tools/Wilderness Overhaul/Rebuild All Mod Texture Arrays From TextureFiles")]
        private static void RebuildAllTextureArrays()
        {
            AssetDatabase.Refresh();

            string[] folderPaths = AssetDatabase.GetSubFolders(SourceRoot);
            if (folderPaths == null || folderPaths.Length == 0)
            {
                EditorUtility.DisplayDialog("Wilderness Overhaul", $"No source folders found in {SourceRoot}.", "OK");
                return;
            }

            foreach (string folderPath in folderPaths.OrderBy(x => x))
                RebuildTextureArrayFromFolder(folderPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Wilderness Overhaul", "Finished rebuilding mod texture arrays from TextureFiles.", "OK");
        }

        private static void RebuildTextureArrayFromFolder(string folderPath)
        {
            string folderName = Path.GetFileName(folderPath);
            Texture2D[] textures = BuildTextureList(folderPath, folderName);

            if (textures.Length == 0)
            {
                Debug.LogWarning($"Wilderness Overhaul: No textures found in {folderPath}");
                return;
            }

            Texture2D first = textures[0];
            Texture2DArray generated = new Texture2DArray(first.width, first.height, textures.Length, first.format, first.mipmapCount > 1);

            bool canCopy = (SystemInfo.copyTextureSupport & CopyTextureSupport.DifferentTypes) == CopyTextureSupport.DifferentTypes;
            if (canCopy)
            {
                generated.Apply(false, true);
                for (int i = 0; i < textures.Length; i++)
                    Graphics.CopyTexture(textures[i], 0, generated, i);
            }
            else
            {
                for (int i = 0; i < textures.Length; i++)
                    generated.SetPixels32(textures[i].GetPixels32(), i);

                generated.Apply(true, true);
            }

            Texture2DArray existing = AssetDatabase.LoadAssetAtPath<Texture2DArray>($"{TargetRoot}/{folderName}.asset");
            if (existing != null)
            {
                generated.wrapMode = existing.wrapMode;
                generated.filterMode = existing.filterMode;
                generated.anisoLevel = existing.anisoLevel;
                EditorUtility.CopySerialized(generated, existing);
                EditorUtility.SetDirty(existing);
                UnityEngine.Object.DestroyImmediate(generated);
                Debug.Log($"Wilderness Overhaul: Rebuilt existing texture array {folderName}.asset from {folderPath}");
            }
            else
            {
                generated.wrapMode = first.wrapMode;
                generated.filterMode = first.filterMode;
                generated.anisoLevel = first.anisoLevel;
                AssetDatabase.CreateAsset(generated, $"{TargetRoot}/{folderName}.asset");
                Debug.Log($"Wilderness Overhaul: Created texture array {folderName}.asset from {folderPath}");
            }
        }

        private static Texture2D[] BuildTextureList(string folderPath, string folderName)
        {
            string archiveId = folderName.Substring(0, 3);
            string vanillaFolder = $"{VanillaRoot}/{archiveId}-Vanilla";

            // Terrain arrays in current DFU expect 56 slices.
            if (AssetDatabase.IsValidFolder(vanillaFolder))
            {
                Texture2D[] textures = new Texture2D[TerrainSliceCount];
                for (int i = 0; i < TerrainSliceCount; i++)
                {
                    string fileName = $"-{i:D3}.png";
                    string modPath = $"{folderPath}/{fileName}";
                    string vanillaPath = $"{vanillaFolder}/{fileName}";

                    Texture2D modTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(modPath);
                    string chosenPath = modTexture != null ? modPath : vanillaPath;
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(chosenPath);
                    if (texture == null)
                        throw new InvalidOperationException($"Missing slice {fileName} for {folderName}. Checked {modPath} and {vanillaPath}.");

                    textures[i] = texture;
                }

                return textures;
            }

            // Fallback for non-standard folders.
            string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
            return textureGuids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Select(AssetDatabase.LoadAssetAtPath<Texture2D>)
                .Where(x => x != null)
                .ToArray();
        }
    }
}
