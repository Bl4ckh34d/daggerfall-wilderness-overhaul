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
            string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
            Texture2D[] textures = textureGuids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Select(AssetDatabase.LoadAssetAtPath<Texture2D>)
                .Where(x => x != null)
                .ToArray();

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
    }
}
