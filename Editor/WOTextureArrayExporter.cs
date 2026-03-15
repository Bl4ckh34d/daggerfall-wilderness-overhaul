using System.IO;
using UnityEditor;
using UnityEngine;

namespace WildernessOverhaul.Editor
{
    public static class WOTextureArrayExporter
    {
        private const string ExtractShaderName = "Hidden/WildernessOverhaul/ExtractTextureArraySlice";
        private static readonly int[] VanillaTerrainArchives = { 2, 3, 4, 102, 103, 104, 302, 303, 304, 402, 403, 404 };

        [MenuItem("Tools/Wilderness Overhaul/Export Selected Texture Array")]
        private static void ExportSelectedTextureArray()
        {
            Texture2DArray textureArray = Selection.activeObject as Texture2DArray;
            if (textureArray == null)
            {
                EditorUtility.DisplayDialog("Wilderness Overhaul", "Select a Texture2DArray asset first.", "OK");
                return;
            }

            string defaultFolder = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(textureArray));
            string targetFolder = EditorUtility.SaveFolderPanel("Export Texture Array Slices", Application.dataPath, defaultFolder);
            if (string.IsNullOrEmpty(targetFolder))
                return;

            ExportTextureArray(textureArray, targetFolder);
        }

        [MenuItem("Tools/Wilderness Overhaul/Export Selected Texture Array", true)]
        private static bool ValidateExportSelectedTextureArray()
        {
            return Selection.activeObject is Texture2DArray;
        }

        [MenuItem("Tools/Wilderness Overhaul/Export All Mod Texture Arrays")]
        private static void ExportAllModTextureArrays()
        {
            string rootFolder = EditorUtility.SaveFolderPanel("Export Mod Texture Arrays", Application.dataPath, "WildernessOverhaul-TextureArrays");
            if (string.IsNullOrEmpty(rootFolder))
                return;

            string[] guids = AssetDatabase.FindAssets("t:Texture2DArray", new[] { "Assets/Game/Mods/WildernessOverhaul/Textures" });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Texture2DArray textureArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(assetPath);
                if (textureArray == null)
                    continue;

                string folderName = Path.GetFileNameWithoutExtension(assetPath);
                string exportFolder = Path.Combine(rootFolder, folderName);
                ExportTextureArray(textureArray, exportFolder);
            }

            EditorUtility.DisplayDialog("Wilderness Overhaul", "Finished exporting texture arrays.", "OK");
        }

        [MenuItem("Tools/Wilderness Overhaul/Export All DFU Vanilla Terrain Arrays")]
        private static void ExportAllVanillaTerrainArrays()
        {
            if (DaggerfallWorkshop.DaggerfallUnity.Instance == null || DaggerfallWorkshop.DaggerfallUnity.Instance.MaterialReader == null)
            {
                EditorUtility.DisplayDialog("Wilderness Overhaul", "Enter play mode with DFU initialized first so MaterialReader can build the vanilla terrain arrays.", "OK");
                return;
            }

            string rootFolder = EditorUtility.SaveFolderPanel("Export DFU Vanilla Terrain Arrays", Application.dataPath, "DFU-Vanilla-TerrainArrays");
            if (string.IsNullOrEmpty(rootFolder))
                return;

            foreach (int archive in VanillaTerrainArchives)
            {
                Material material = DaggerfallWorkshop.DaggerfallUnity.Instance.MaterialReader.GetTerrainTextureArrayMaterial(archive);
                if (material == null)
                    continue;

                Texture2DArray textureArray = material.GetTexture(DaggerfallWorkshop.TileTexArrUniforms.TileTexArr) as Texture2DArray;
                if (textureArray == null)
                    continue;

                string exportFolder = Path.Combine(rootFolder, $"{archive:D3}-Vanilla");
                ExportTextureArray(textureArray, exportFolder);
            }

            EditorUtility.DisplayDialog("Wilderness Overhaul", "Finished exporting DFU vanilla terrain arrays.", "OK");
        }

        private static void ExportTextureArray(Texture2DArray textureArray, string targetFolder)
        {
            Shader shader = Shader.Find(ExtractShaderName);
            if (shader == null)
            {
                EditorUtility.DisplayDialog("Wilderness Overhaul", $"Missing shader: {ExtractShaderName}", "OK");
                return;
            }

            Directory.CreateDirectory(targetFolder);

            Material material = new Material(shader);
            material.hideFlags = HideFlags.HideAndDontSave;
            material.SetTexture("_TexArray", textureArray);

            RenderTexture renderTexture = RenderTexture.GetTemporary(textureArray.width, textureArray.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            Texture2D output = new Texture2D(textureArray.width, textureArray.height, TextureFormat.RGBA32, false);

            try
            {
                for (int slice = 0; slice < textureArray.depth; slice++)
                {
                    material.SetFloat("_Slice", slice);
                    Graphics.Blit(null, renderTexture, material);

                    RenderTexture previous = RenderTexture.active;
                    RenderTexture.active = renderTexture;
                    output.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                    output.Apply(false, false);
                    RenderTexture.active = previous;

                    string filePath = Path.Combine(targetFolder, $"{textureArray.name}-{slice:D3}.png");
                    File.WriteAllBytes(filePath, output.EncodeToPNG());
                }

                string indexPath = Path.Combine(targetFolder, "index.txt");
                File.WriteAllText(indexPath,
                    $"source={AssetDatabase.GetAssetPath(textureArray)}\n" +
                    $"width={textureArray.width}\n" +
                    $"height={textureArray.height}\n" +
                    $"depth={textureArray.depth}\n");
            }
            finally
            {
                RenderTexture.ReleaseTemporary(renderTexture);
                Object.DestroyImmediate(output);
                Object.DestroyImmediate(material);
            }
        }
    }
}
