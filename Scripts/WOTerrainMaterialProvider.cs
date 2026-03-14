// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net), Nystul, TheLacus
// Contributors:
//
// Notes:
//

using System;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop
{
    public class WOTerrainMaterialProvider : TerrainMaterialProvider
    {
        Mod mod;
        TextureReader textureReader;

        public WOTerrainMaterialProvider(Mod injectedMod) {
            mod = injectedMod;
            textureReader = new TextureReader(DaggerfallUnity.Instance.Arena2Path);
        }


        private readonly Shader shader = Shader.Find(MaterialReader._DaggerfallTilemapTextureArrayShaderName);

        internal static bool IsSupported
        {
            get { return SystemInfo.supports2DArrayTextures && DaggerfallUnity.Settings.EnableTextureArrays; }
        }

        public sealed override Material CreateMaterial()
        {
            return new Material(shader);
        }

        public override void PromoteMaterial(DaggerfallTerrain daggerfallTerrain, TerrainMaterialData terrainMaterialData)
        {
            int archive = GetGroundArchive(terrainMaterialData.WorldClimate);
            Texture2DArray modAlbedoArray = GetModTerrainTextureArray(archive);
            Texture2DArray normalArray = textureReader.GetTerrainTextureArray(archive, TextureMap.Normal);
            Texture2DArray parallaxArray = textureReader.GetTerrainTextureArray(archive, TextureMap.Height);
            Texture2DArray metallicGlossArray = textureReader.GetTerrainTextureArray(archive, TextureMap.MetallicGloss);

            // Keep the mod's custom albedo, but reuse DFU's current terrain lighting stack.
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TileTexArr, modAlbedoArray);
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TileNormalMapTexArr, normalArray);
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TileParallaxMapTexArr, parallaxArray);
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TileMetallicGlossMapTexArr, metallicGlossArray);
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TilemapTex, terrainMaterialData.TileMapTexture);

            if (normalArray != null)
                terrainMaterialData.Material.EnableKeyword(KeyWords.NormalMap);
            else
                terrainMaterialData.Material.DisableKeyword(KeyWords.NormalMap);

            if (parallaxArray != null)
                terrainMaterialData.Material.EnableKeyword(KeyWords.HeightMap);
            else
                terrainMaterialData.Material.DisableKeyword(KeyWords.HeightMap);

            if (metallicGlossArray != null)
            {
                terrainMaterialData.Material.EnableKeyword(KeyWords.MetallicGlossMap);
                terrainMaterialData.Material.SetFloat(Uniforms.Smoothness, 0.35f);
            }
            else
            {
                terrainMaterialData.Material.DisableKeyword(KeyWords.MetallicGlossMap);
            }
        }

        private Texture2DArray GetModTerrainTextureArray(int archive)
        {
            Texture2DArray textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("302-TexArray.asset");
            if (archive == 002)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("002-TexArray.asset");
            if (archive == 003)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("003-TexArray.asset");
            if (archive == 004)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("004-TexArray.asset");
            if (archive == 102)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("102-TexArray.asset");
            if (archive == 103)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("103-TexArray.asset");
            if (archive == 104)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("104-TexArray.asset");
            if (archive == 302) {
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("302-TexArray.asset");
            }
            if (archive == 303)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("303-TexArray.asset");
            if (archive == 304)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("304-TexArray.asset");
            if (archive == 402)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("402-TexArray.asset");
            if (archive == 403)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("403-TexArray.asset");
            if (archive == 404)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("404-TexArray.asset");
            textureArrayTerrainTiles.filterMode = DaggerfallUnity.Instance.MaterialReader.MainFilterMode;
            return textureArrayTerrainTiles;
        }

        private TextureFormat ParseTextureFormat(SupportedAlphaTextureFormats format)
        {
            switch (format)
            {
                default:
                case SupportedAlphaTextureFormats.RGBA32:
                    return TextureFormat.RGBA32;
                case SupportedAlphaTextureFormats.ARGB32:
                    return TextureFormat.ARGB32;
                case SupportedAlphaTextureFormats.ARGB444:
                    return TextureFormat.ARGB4444;
                case SupportedAlphaTextureFormats.RGBA444:
                    return TextureFormat.RGBA4444;
            }
        }
    }
}
