using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace WildernessOverhaul
{
    public class WOTerrainNature : ITerrainNature
    {
        // static GameObject exterior;
        // static List<GameObject> staticGeometryList;

        // MOD SETTINGS
        private static bool dynamicNatureClearance;
        private static bool vegetationInLocations;
        private static bool firefliesExist;
        private static bool shootingStarsExist;
        private static bool InterestingErodedTerrainEnabled;
        private static float fireflyDistance;
        private static float shootingStarsMinimum;
        private static float shootingStarsMaximum;
        private static float generalNatureClearance;
        private static float natureClearance1;
        private static float natureClearance2;
        private static float natureClearance3;
        private static float natureClearance4;
        private static float natureClearance5;
        private static Mod mod;
        public float height;

        // TERRAIN SETTINGS
        // Max steepness to still place nature billboards
        private const float maxSteepness = 50f; // 50
                                                // Sink flats slightly into ground as slope increases to prevent floaty trees.
        private const float slopeSinkRatio = 70f; // 70
                                                  // Tree border
        private float treeLine = WOTerrainTexturing.treeLine;
        //Flag to signal use of meshes
        public bool NatureMeshUsed { get; protected set; }

        // Important data classes holding values and lists
        public WOVegetationList vegetationList;
        public WOVegetationChance vegetationChance;
        public WOStochasticChances stochastics;

        public WOTerrainNature(
           Mod woMod,
           bool DMEnabled,
           bool ITEnabled,
           bool dNClearance,
           bool vegInLoc,
           bool fireflies,
           bool shootingStars,
           float fireflyActivationDistance,
           float shootingStarsMin,
           float shootingStarsMax,
           float gNClearance,
           float nClearance1,
           float nClearance2,
           float nClearance3,
           float nClearance4,
           float nClearance5)
        {
            mod = woMod;
            // Change a tree texture in desert if DREAM Sprites Mod enabled
            if (DMEnabled)
            {
                List<int> desertTrees = new List<int>(new int[] { 5, 13, 30 });
                Debug.Log("Wilderness Overhaul: DREAM Sprites enabled: " + DMEnabled);
            }
            else
            {
                List<int> desertTrees = new List<int>(new int[] { 5, 13, 13 });
                Debug.Log("Wilderness Overhaul: DREAM Sprites enabled: " + DMEnabled);
            }
            InterestingErodedTerrainEnabled = ITEnabled;
            Debug.Log("Wilderness Overhaul: Interesting Eroded Terrain enabled: " + InterestingErodedTerrainEnabled);
            dynamicNatureClearance = dNClearance;
            Debug.Log("Wilderness Overhaul: Setting Dynamic Nature Clearance: " + dynamicNatureClearance);
            vegetationInLocations = vegInLoc;
            Debug.Log("Wilderness Overhaul: Setting Vegetation in Jungle Location: " + vegetationInLocations);
            generalNatureClearance = gNClearance;
            Debug.Log("Wilderness Overhaul: Setting General Nature Clearance: " + generalNatureClearance);
            firefliesExist = fireflies;
            Debug.Log("Wilderness Overhaul: Generate Fireflies at Night: " + firefliesExist);
            fireflyDistance = fireflyActivationDistance;
            Debug.Log("Wilderness Overhaul: Activation Distance of Fireflies: " + fireflyDistance);
            shootingStarsExist = shootingStars;
            Debug.Log("Wilderness Overhaul: Generate Shooting Stars at Night: " + shootingStarsExist);
            shootingStarsMinimum = shootingStarsMin;
            shootingStarsMaximum = shootingStarsMax;
            Debug.Log("Wilderness Overhaul: Shooting Stars Chance Min: " + shootingStarsMinimum + " and Max: " + shootingStarsMaximum);
            natureClearance1 = nClearance1;
            Debug.Log("Wilderness Overhaul: Setting Nature Clearance 1: " + natureClearance1);
            natureClearance2 = nClearance2;
            Debug.Log("Wilderness Overhaul: Setting Nature Clearance 2: " + natureClearance2);
            natureClearance3 = nClearance3;
            Debug.Log("Wilderness Overhaul: Setting Nature Clearance 3: " + natureClearance3);
            natureClearance4 = nClearance4;
            Debug.Log("Wilderness Overhaul: Setting Nature Clearance 4: " + natureClearance4);
            natureClearance5 = nClearance5;
            Debug.Log("Wilderness Overhaul: Setting Nature Clearance 5: " + natureClearance5);
        }

        public virtual void LayoutNature(
           DaggerfallTerrain dfTerrain,
           DaggerfallBillboardBatch dfBillboardBatch,
           float terrainScale,
           int terrainDist)
        {

            // Preparation of StaticGeometry for collision detection
            /* if(dfTerrain.MapData.hasLocation)
                {
                exterior = GameObject.Find("Exterior");
                staticGeometryList = new List<GameObject>();
                GameObject streamingTarget = GameObject.Find("StreamingTarget");
                AddDescendantsWithTag(streamingTarget.transform, "StaticGeometry", staticGeometryList);
                } */

            // Apply Mod Setting of Dynamic Nature Clearance Distances
            if (dynamicNatureClearance)
            {
                if (dfTerrain.MapData.LocationType == DFRegion.LocationTypes.TownCity)
                    generalNatureClearance = natureClearance1;
                else if (dfTerrain.MapData.LocationType == DFRegion.LocationTypes.TownHamlet)
                    generalNatureClearance = natureClearance2;
                else if (dfTerrain.MapData.LocationType == DFRegion.LocationTypes.TownVillage ||
                   dfTerrain.MapData.LocationType == DFRegion.LocationTypes.HomeWealthy ||
                   dfTerrain.MapData.LocationType == DFRegion.LocationTypes.ReligionCult)
                    generalNatureClearance = natureClearance3;
                else if (dfTerrain.MapData.LocationType == DFRegion.LocationTypes.HomeFarms ||
                   dfTerrain.MapData.LocationType == DFRegion.LocationTypes.ReligionTemple ||
                   dfTerrain.MapData.LocationType == DFRegion.LocationTypes.Tavern ||
                   dfTerrain.MapData.LocationType == DFRegion.LocationTypes.HomePoor)
                    generalNatureClearance = natureClearance4;
                else if (dfTerrain.MapData.LocationType == DFRegion.LocationTypes.DungeonLabyrinth ||
                   dfTerrain.MapData.LocationType == DFRegion.LocationTypes.DungeonKeep ||
                   dfTerrain.MapData.LocationType == DFRegion.LocationTypes.DungeonRuin ||
                   dfTerrain.MapData.LocationType == DFRegion.LocationTypes.Graveyard ||
                   dfTerrain.MapData.LocationType == DFRegion.LocationTypes.Coven)
                    generalNatureClearance = natureClearance5;
                if (dfTerrain.MapData.worldClimate == (int)MapsFile.Climates.Rainforest)
                    generalNatureClearance = 0.5f;
            }

            // Applying General Nature Clearance Distance
            Rect rect = dfTerrain.MapData.locationRect;
            if (rect.x > 0 && rect.y > 0)
            {
                rect.xMin -= generalNatureClearance;
                rect.xMax += generalNatureClearance;
                rect.yMin -= generalNatureClearance;
                rect.yMax += generalNatureClearance;
            }


            // Get terrain
            Terrain terrain = dfTerrain.gameObject.GetComponent<Terrain>();
            if (!terrain)
                return;

            // Get terrain data
            TerrainData terrainData = terrain.terrainData;
            if (!terrainData)
                return;

            // Remove exiting billboards
            dfBillboardBatch.Clear();
            MeshReplacement.ClearNatureGameObjects(terrain);
            foreach (Transform child in dfBillboardBatch.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            // Seed random with terrain key
            Random.InitState(TerrainHelper.MakeTerrainKey(dfTerrain.MapPixelX, dfTerrain.MapPixelY));

            // Just layout some random flats spread evenly across entire map pixel area
            // Flats are aligned with tiles, max 16129 billboards per batch
            Vector2 tilePos = Vector2.zero;
            int tDim = MapsFile.WorldMapTileDim;
            int hDim = DaggerfallUnity.Instance.TerrainSampler.HeightmapDimension;
            float scale = terrainData.heightmapScale.x * (float)hDim / (float)tDim;
            float maxTerrainHeight = DaggerfallUnity.Instance.TerrainSampler.MaxTerrainHeight;
            float beachLine = DaggerfallUnity.Instance.TerrainSampler.BeachElevation;

            // Adapting maxTerrainHeight for Interesting Eroded Terrain Mod
            if (InterestingErodedTerrainEnabled)
            {
                maxTerrainHeight = 4890;
            }

            // Chance scaled by base climate type
            DFLocation.ClimateSettings climate = MapsFile.GetWorldClimateSettings(dfTerrain.MapData.worldClimate);

            // Initialize stochastics & vegetation
            stochastics = new WOStochasticChances();
            vegetationChance = new WOVegetationChance();
            vegetationList = new WOVegetationList();

            // Adds one shooting star Particle System of every MapPixel
            if (shootingStarsExist)
                AddShootingStar(dfTerrain, dfBillboardBatch, 90f, 900f, shootingStarsMinimum, shootingStarsMaximum); // Shooting Stars

            for (int y = 0; y < tDim; y++)
            {
                for (int x = 0; x < tDim; x++)
                {
                    // Get latitude and longitude of this tile
                    int latitude = (int)(dfTerrain.MapPixelX * MapsFile.WorldMapTileDim + x);
                    int longitude = (int)(MapsFile.MaxWorldTileCoordZ - dfTerrain.MapPixelY * MapsFile.WorldMapTileDim + y);

                    // Set texture tile using weighted noise
                    float weight = 0;

                    // Reject based on steepness
                    float steepness = terrainData.GetSteepness((float)x / tDim, (float)y / tDim);
                    if (steepness > maxSteepness)
                        continue;

                    // Reject if inside location rect (expanded slightly to give extra clearance around locations)
                    tilePos.x = x;
                    tilePos.y = y;

                    // Checks Mod Setting if Vegetation should also be spawned inside of Jungle Locations.
                    if (vegetationInLocations)
                    {
                        if (rect.x > 0 && rect.y > 0 && rect.Contains(tilePos) && dfTerrain.MapData.worldClimate != (int)MapsFile.Climates.Rainforest)
                            continue;
                    }
                    else
                    {
                        if (rect.x > 0 && rect.y > 0 && rect.Contains(tilePos))
                            continue;
                    }

                    // Chance also determined by tile type
                    int tile = dfTerrain.MapData.tilemapSamples[x, y] & 0x3F;
                    if (tile == 2)
                    {   // Dirt
                        if (Random.Range(0.0f, 1.0f) > vegetationChance.chanceOnGrass)
                            continue;
                    }
                    else if (tile == 1)
                    {   // Grass
                        if (Random.Range(0.0f, 1.0f) > vegetationChance.chanceOnDirt)
                            continue;
                    }
                    else if (tile == 3)
                    {   // Stone
                        if (Random.Range(0.0f, 1.0f) > vegetationChance.chanceOnStone)
                            continue;
                    }
                    else if (tile == 0)
                    {   // Water
                        continue;
                    }

                    //Defining height for the billboard placement
                    int hx = (int)Mathf.Clamp(hDim * ((float)x / (float)tDim), 0, hDim - 1);
                    int hy = (int)Mathf.Clamp(hDim * ((float)y / (float)tDim), 0, hDim - 1);
                    height = dfTerrain.MapData.heightmapSamples[hy, hx];

                    // Adjust Vegetation Lists to current positions height, climate zone and mapStyle
                    vegetationList.ChangeVegetationLists(height, DaggerfallUnity.Instance.WorldTime.Now.SeasonValue, stochastics.mapStyle);
                    // Adjust Vegetation Chance to current position height and climate zone
                    vegetationChance.ChangeVegetationChances(height, dfTerrain.MapData.worldClimate);

                    if (tile == 0 || height <= 0)
                        continue;

                    BaseDataObject baseData = new BaseDataObject(dfTerrain, dfBillboardBatch, terrain, scale, steepness, x, y, maxTerrainHeight);

                    switch (climate.WorldClimate)
                    {
                        #region Temperate Spawns
                        case (int)MapsFile.Climates.Woodlands:

                            weight = GetNoise(latitude,
                                longitude,
                                stochastics.tempForestFrequency,
                                stochastics.tempForestAmplitude,
                                stochastics.tempForestPersistence,
                                stochastics.tempForestOctaves,
                                100);
                            weight = Mathf.Clamp(weight, 0f, 1f);

                            if (tile == 1) // Dirt
                            {
                                if (height > Random.Range(0.025f, 0.027f))
                                {
                                    if (GetWeightedRecord(weight) == "forest")
                                    {
                                        if (Random.Range(0, 100) < Random.Range(70, 80))
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.15f, 0.30f), true); // Dead Trees

                                            for (int i = 0; i < Random.Range(0, 3); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBeach, Random.Range(1.00f, 2.00f), true); // Beach
                                            }
                                        }
                                        else
                                        {
                                            if (Random.Range(0, 100) < stochastics.mapStyle)
                                            {
                                                for (int i = 0; i < Random.Range(1, 5); i++)
                                                {
                                                    if (Random.Range(0, 100) < 50 && height > Random.Range(0.1f, 0.15f))
                                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.75f, 1.50f), true, 3); // Needle Tree
                                                    else
                                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(0.75f, 1.50f), true); // Tree
                                                }
                                            }
                                            else
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.15f, 0.30f), true); // Dead Trees

                                                for (int i = 0; i < Random.Range(0, 3); i++)
                                                {
                                                    AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBeach, Random.Range(1.00f, 2.00f), true); // Beach
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (tile == 2) // Grass
                            {
                                if (Random.Range(0.0f, 100.0f) <= stochastics.temperateMushroomRingChance) // Mushroom Circle
                                {
                                    AddMushroomRingToBatch(baseData, 23);
                                }
                                else if (GetWeightedRecord(weight, stochastics.tempForestLimit[0], stochastics.tempForestLimit[1]) == "flower")
                                {
                                    if (Random.Range(0, 100) < 5)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandMushroom, 0.00f, true); // Mushroom
                                    }

                                    for (int i = 0; i < Random.Range(4, 7); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandFlowers, Random.Range(0.45f, 0.75f), true); // Flowers
                                    }

                                    float rnd = Random.Range(0, 100);
                                    if (rnd < stochastics.mapStyleChance[1])
                                    {
                                        for (int i = 0; i < Random.Range(2, 4); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandFlowers, Random.Range(0.60f, 1.00f), true); // Flowers
                                        }
                                    }
                                    if (rnd < 5)
                                    {
                                        for (int i = 0; i < Random.Range(0, 3); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(0.85f, 1.15f), true); // Bushes
                                        }
                                    }
                                }
                                else if (GetWeightedRecord(weight, stochastics.tempForestLimit[0], stochastics.tempForestLimit[1]) == "grass")
                                {
                                    float rnd = Random.Range(0, 100);
                                    if (rnd < 4)
                                    {
                                        for (int i = 0; i < Random.Range(9, 14); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(2.25f, 2.75f), true); // Bushes
                                        }

                                        for (int i = 0; i < Random.Range(3, 5); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(0.50f, 0.75f), true); // Bushes
                                        }
                                    }

                                    if (rnd < 3)
                                    {
                                        for (int i = 0; i < Random.Range(3, 8); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(1.00f, 2.25f), true); // Trees
                                        }
                                    }
                                }
                                else if (GetWeightedRecord(weight, stochastics.tempForestLimit[0], stochastics.tempForestLimit[1]) == "forest")
                                {
                                    float rnd = Random.Range(0, 100);
                                    for (int i = 0; i < Random.Range(3, 4); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(1.00f, 2.25f), true); // Trees
                                    }

                                    AddFirefly(baseData, 0.1f, 5, 15, 35); // Fireflies

                                    if (rnd < stochastics.mapStyleChance[3])
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBeach, Random.Range(2.00f, 3.00f), true); // Beach

                                        for (int i = 0; i < Random.Range(0, 2); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(2.00f, 3.00f), true); // Trees
                                        }

                                        for (int i = 0; i < Random.Range(0, 2); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(2.00f, 3.00f), true); // Bushes
                                        }

                                        AddFirefly(baseData, 0.05f, 10, 50, 100); // Fireflies
                                    }

                                    if (rnd < stochastics.mapStyleChance[1])
                                    {
                                        for (int i = 0; i < Random.Range(0, 1); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(2.00f, 3.00f), true); // Bushes
                                        }

                                        AddFirefly(baseData, 0.025f, 15, 100, 250); // Fireflies
                                    }
                                }
                            }
                            else if (tile == 3) // Stone
                            {
                                if (GetWeightedRecord(weight) == "forest")
                                {
                                    for (int i = 0; i < Random.Range(0, 3); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandRocks, Random.Range(0.25f, 1.00f), true); // Stones
                                    }

                                    if (height > 0.15f && Random.Range(0.0f, 100.0f) < 5)
                                    {
                                        if (Random.Range(stochastics.mapStyleChance[3], stochastics.mapStyleChance[stochastics.mapStyleChance.Length - 1]) < stochastics.mapStyle)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.75f, 1.15f), true); // Dead Tree
                                        }
                                        else
                                        {
                                            for (int i = 0; i < Random.Range(0, 3); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.75f, 1.50f), true, 3); // Needle Tree
                                            }
                                        }
                                    }
                                }
                                if (GetWeightedRecord(weight) == "flower")
                                {
                                    for (int i = 0; i < Random.Range(0, 2); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandRocks, Random.Range(0.25f, 0.50f), true); // Stones
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Mountain Spawns

                        case (int)MapsFile.Climates.Mountain:
                            weight += GetNoise(latitude,
                            longitude,
                            stochastics.mountForestFrequency,
                            stochastics.mountForestAmplitude,
                            stochastics.mountForestPersistence,
                            stochastics.mountForestOctaves,
                            100);
                            weight = Mathf.Clamp(weight, 0f, 1f);

                            if (tile == 1) // Dirt
                            {
                                // Beach
                                if (dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight < beachLine)
                                {
                                    if (Random.Range(0, 100) < stochastics.mapStyle)
                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 2)); i++)
                                            AddBillboardToBatch(baseData, vegetationList.mountainsBeach, 0.75f, true); // Beach
                                }
                                else if (dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight >= beachLine
                                    && dfTerrain.MapData.heightmapSamples[hy, hx] < treeLine)
                                {
                                    if (Random.Range(0, 100) < Random.Range(10, 20))
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.mountainsDeadTrees, 0.75f, true); // Dead Trees

                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 1)); i++)
                                            AddBillboardToBatch(baseData, vegetationList.mountainsDeadTrees, 1.50f, true); // Dead Trees
                                    }
                                    else
                                    {
                                        if (Random.Range(0, 100) < stochastics.mapStyle)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 0.00f, true); // Needle Tree
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 1.00f, true); // Needle Tree

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 2.00f, true); // Needle Tree                                        
                                        }
                                        else
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsDeadTrees, 0.50f, true); // Dead Trees
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 1.25f, true); // Needle Trees

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 1)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.75f, true); // Rocks
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 3)); i++)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 1.00f, true); // Rocks

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 3)); i++)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsBeach, 1.00f, true); // Beach                                    
                                }
                            }
                            else if (tile == 2) // Grass
                            {
                                float rndMajor = Random.Range(0.0f, 100.0f);

                                if (Random.Range(0.0f, 100.0f) < stochastics.mountainStoneCircleChance)
                                    AddStoneCircleToBatch(baseData, vegetationList.mountainsRocks, 5, 0);

                                if (height < stochastics.mountForestLimit[0])
                                {
                                    if (rndMajor >= 50)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsTrees, 0.50f, true); // Trees
                                    else if (rndMajor <= 30)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 0.50f, true); // Trees

                                    if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "forest")
                                    {
                                        float rndMinor = Random.Range(0, 100);
                                        if (rndMinor < stochastics.mapStyle)
                                        {
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 8)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsTrees, 1.50f, true); // Trees

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 10)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 1.50f, true); // Grass

                                            if (Random.Range(0, 100) < 40)
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 8)); i++)
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.50f, true); // Flowers

                                            if (Random.Range(0, 100) < 30)
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(4, 12)); i++)
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.75f, true); // Flowers

                                            if (Random.Range(0, 100) < 30)
                                                if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.90f)
                                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 3)); i++)
                                                        AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 1.00f, true); // Rocks

                                            if (Random.Range(0, 100) < 20)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsDeadTrees, 0.25f, true);

                                            if (Random.Range(0, 100) < 15)
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandMushroom, 0.10f, true);
                                        }
                                    }
                                    else if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "flower")
                                    {
                                        if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.90f)
                                            AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.00f, true); // Rock

                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(4, 16)); i++)
                                            AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.50f, true); // Flowers

                                        float rndMinor = Random.Range(0, 100);
                                        if (rndMinor < 40)
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.75f, true); // Flowers
                                    }
                                }
                                else if (height < stochastics.mountForestLimit[1])
                                {
                                    float temp = 80 - (height - stochastics.mountForestLimit[0]) * 750;
                                    if (Random.Range(0, 100) < (temp + (rndMajor / 10)))
                                        AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 0.75f, true);
                                    else if (Random.Range(0, 100) > 95)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 0.50f, true);


                                    if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "forest")
                                    {
                                        float rndMinor = Random.Range(0, 100);
                                        if (rndMinor < stochastics.mapStyle)
                                        {
                                            if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.90f)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.00f, true); // Rock

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(4, 16)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.50f, true); // Flowers

                                            if (rndMinor < 40)
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.75f, true); // Flowers
                                        }
                                    }
                                    else if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "flower")
                                    {
                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 10)); i++)
                                            AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 1.50f, true); // Grass

                                        if (Random.Range(0, 100) < 40)
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(4, 12)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.50f, true); // Flowers

                                        if (Random.Range(0, 100) < 30)
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(8, 16)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 2.00f, true); // Flowers

                                        if (Random.Range(0, 100) < 30)
                                            if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.90f)
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 5)); i++)
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 1.60f, true); // Rocks

                                        if (Random.Range(0, 100) < 20)
                                            AddBillboardToBatch(baseData, vegetationList.mountainsDeadTrees, 0.25f, true);

                                        if (Random.Range(0, 100) < 30)
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandMushroom, 0.10f, true);
                                    }
                                }
                            }

                            else if (tile == 3) // Stone
                            {
                                if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "forest")
                                {
                                    AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.00f, true); // Stones

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(4, 12)); i++)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 2.00f, true); // Stones

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 8)); i++)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.50f, true); // Flowers

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 1)); i++)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 1.0f, true); // Flowers                                  
                                }
                                if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "flower")
                                {
                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 4)); i++)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.75f, true); // Flowers
                                }

                                if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.999f)
                                    AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 1.00f, true); // Rocks
                            }
                            break;

                        #endregion

                        #region Desert1 Spawns

                        case (int)MapsFile.Climates.Desert: //STEPPE
                            break;

                        #endregion

                        #region Desert2 Spawns

                        case (int)MapsFile.Climates.Desert2:
                            break;

                        #endregion

                        #region Haunted Woodlands Spawns

                        case (int)MapsFile.Climates.HauntedWoodlands:
                            break;

                        #endregion

                        #region Woodland Hills Spawns

                        case (int)MapsFile.Climates.MountainWoods:
                            break;
                        #endregion

                        #region Rainforest Spawns

                        case (int)MapsFile.Climates.Rainforest:
                            break;

                        #endregion

                        #region Subtropical Spawns
                        case (int)MapsFile.Climates.Subtropical:

                            if (tile == 1) // Dirt
                            {

                            }
                            else if (tile == 2) // Grass
                            {

                            }
                            else if (tile == 3) // Stone
                            {

                            }
                            break;
                        #endregion

                        #region Swamp Spawns
                        case (int)MapsFile.Climates.Swamp:

                            if (tile == 1) // Dirt
                            {

                            }
                            else if (tile == 2) // Grass
                            {

                            }
                            else if (tile == 3) // Stone
                            {

                            }
                            break;
                            #endregion
                    }
                }
            }

            // Apply new batch
            dfBillboardBatch.Apply();
        }

        public static void AddBillboardToBatch(
           BaseDataObject baseData,
           List<int> billboardCollection,
           float posVariance,
           bool checkOnLand)
        {
            int rnd = (int)Mathf.Round(Random.Range(0, billboardCollection.Count));
            Vector3 pos = new Vector3((baseData.x + Random.Range(-posVariance, posVariance)) * baseData.scale, 0, (baseData.y + Random.Range(-posVariance, posVariance)) * baseData.scale);
            float height = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height - (baseData.steepness / slopeSinkRatio);
            if (checkOnLand &&
                !TileTypeCheck(pos, baseData, true, false, false, true, true) &&
             TileTypeCheck(pos, baseData, false, true, false, false, false) &&
             baseData.steepness < Mathf.Clamp(90f - ((height / baseData.maxTerrainHeight) / 0.85f * 100f), 40f, 90f))
            {
                baseData.dfBillboardBatch.AddItem(billboardCollection[rnd], pos);
            }
            if (!checkOnLand &&
             TileTypeCheck(pos, baseData, true, false, false, false, false) &&
             !TileTypeCheck(pos, baseData, false, false, false, true, true) &&
             baseData.steepness < Mathf.Clamp(90f - ((height / baseData.maxTerrainHeight) / 0.85f * 100f), 40f, 90f))
            {
                baseData.dfBillboardBatch.AddItem(billboardCollection[rnd], pos);
            }
        }

        public static void AddBillboardToBatch(
           BaseDataObject baseData,
           List<int> billboardCollection,
           float posVariance,
           bool checkOnLand,
           int record)
        {
            Vector3 pos = new Vector3((baseData.x + Random.Range(-posVariance, posVariance)) * baseData.scale, 0, (baseData.y + Random.Range(-posVariance, posVariance)) * baseData.scale);
            float height = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height - (baseData.steepness / slopeSinkRatio);

            if (checkOnLand &&
                !TileTypeCheck(pos, baseData, true, false, false, true, true) &&
                TileTypeCheck(pos, baseData, false, true, false, false, false) &&
                baseData.steepness < Mathf.Clamp(100f - ((height / baseData.maxTerrainHeight) * 100f), 40f, 100f))
            {
                baseData.dfBillboardBatch.AddItem(billboardCollection[record], pos);
            }
            if (!checkOnLand &&
                TileTypeCheck(pos, baseData, true, false, false, false, false) &&
                !TileTypeCheck(pos, baseData, false, false, false, true, true) &&
                baseData.steepness < Mathf.Clamp(100f - ((height / baseData.maxTerrainHeight) * 100f), 40f, 100f))
            {
                baseData.dfBillboardBatch.AddItem(billboardCollection[record], pos);
            }
        }

        public static void AddMushroomRingToBatch(
           BaseDataObject baseData,
           int record)
        {
            Vector3 pos = new Vector3(baseData.x * baseData.scale, 0, (baseData.y + 0.5f) * baseData.scale);
            float height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(record, pos);

            pos = new Vector3((baseData.x + 0.272f) * baseData.scale, 0, (baseData.y - 0.404f) * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(record, pos);

            pos = new Vector3((baseData.x - 0.272f) * baseData.scale, 0, (baseData.y - 0.404f) * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(record, pos);

            pos = new Vector3((baseData.x - 0.475f) * baseData.scale, 0, (baseData.y + 0.154f) * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(record, pos);

            pos = new Vector3((baseData.x + 0.475f) * baseData.scale, 0, (baseData.y + 0.154f) * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(record, pos);
        }

        public static void AddStoneCircleToBatch(
           BaseDataObject baseData,
           List<int> billboardCollection,
           int record1,
           int record2)
        {
            Vector3 pos = new Vector3(baseData.x * baseData.scale, 0, baseData.y * baseData.scale);
            float height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(billboardCollection[record1], pos);

            pos = new Vector3((baseData.x + 0.4f) * baseData.scale, 0, baseData.y * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(billboardCollection[record2], pos);

            pos = new Vector3((baseData.x - 0.4f) * baseData.scale, 0, baseData.y * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(billboardCollection[record2], pos);

            pos = new Vector3(baseData.x * baseData.scale, 0, (baseData.y + 0.4f) * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(billboardCollection[record2], pos);

            pos = new Vector3(baseData.x * baseData.scale, 0, (baseData.y - 0.4f) * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(billboardCollection[record2], pos);

            pos = new Vector3((baseData.x + 0.3f) * baseData.scale, 0, (baseData.y + 0.3f) * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(billboardCollection[record2], pos);

            pos = new Vector3((baseData.x - 0.3f) * baseData.scale, 0, (baseData.y + 0.3f) * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(billboardCollection[record2], pos);

            pos = new Vector3((baseData.x + 0.3f) * baseData.scale, 0, (baseData.y - 0.3f) * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(billboardCollection[record2], pos);

            pos = new Vector3((baseData.x - 0.3f) * baseData.scale, 0, (baseData.y - 0.3f) * baseData.scale);
            height2 = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = height2 - (baseData.steepness / slopeSinkRatio);
            baseData.dfBillboardBatch.AddItem(billboardCollection[record2], pos);
        }

        public static void AddFirefly(
           BaseDataObject baseData,
           float rndFirefly,
           float distanceVariation,
           int minNumber,
           int maxNumber)
        {
            if (rndFirefly >= Random.Range(0.0f, 100.0f) && DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter && firefliesExist)
            {
                GameObject fireflyContainer = new GameObject();
                fireflyContainer.name = "fireflyContainer";
                fireflyContainer.transform.parent = baseData.dfBillboardBatch.transform;
                fireflyContainer.transform.position = new Vector3(baseData.dfTerrain.transform.position.x + (baseData.x * baseData.scale), baseData.terrain.SampleHeight(new Vector3(baseData.x * baseData.scale, 0, baseData.y * baseData.scale) + baseData.dfTerrain.transform.position) + baseData.dfTerrain.transform.position.y, baseData.dfTerrain.transform.position.z + (baseData.y * baseData.scale));
                fireflyContainer.AddComponent<WODistanceChecker>();
                fireflyContainer.GetComponent<WODistanceChecker>().distance = fireflyDistance;
                for (int i = 0; i < Random.Range(minNumber, maxNumber); i++)
                {
                    fireflyContainer.GetComponent<WODistanceChecker>().CreateFirefly(mod, baseData.dfTerrain, baseData.x, baseData.y, baseData.scale, baseData.terrain, distanceVariation); // Firefly
                }
                fireflyContainer.GetComponent<WODistanceChecker>().AddChildrenToArray();
                fireflyContainer.GetComponent<WODistanceChecker>().DeactivateAllChildren();
            }
        }

        public static void AddShootingStar(
           DaggerfallTerrain dfTerrain,
           DaggerfallBillboardBatch dfBillboardBatch,
           float rotationAngleX,
           float heightInTheSky,
           float sSMin,
           float sSMax
           )
        {
            Vector3 shootingStarPos = new Vector3(dfTerrain.transform.position.x, dfTerrain.transform.position.z, 0);
            GameObject shootingStarInstance = mod.GetAsset<GameObject>("ShootingStars", true);
            shootingStarInstance.transform.position = new Vector3(shootingStarPos.x, heightInTheSky, shootingStarPos.z);
            shootingStarInstance.transform.parent = dfBillboardBatch.transform;
            shootingStarInstance.transform.rotation = Quaternion.Euler(rotationAngleX, 0, 0);
            shootingStarInstance.AddComponent<WOShootingStarController>();
            shootingStarInstance.GetComponent<WOShootingStarController>().ps = shootingStarInstance.GetComponent<ParticleSystem>();
            var emissionModule = shootingStarInstance.GetComponent<WOShootingStarController>().ps.emission;
            emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(sSMin / 1000, sSMax / 1000);
        }

        static public bool TileTypeCheck(
           Vector3 pos,
           BaseDataObject baseData,
           bool isOnAnyWaterTile,
           bool isOnPureGroundTile,
           bool isOnOrCloseToShallowWaterTile,
           bool isOnOrCloseToStreetTile,
           bool isCollidingWithBuilding)
        {
            bool result = true;

            bool stopCondition = false;

            float offsetA, offsetB;
            int roundedX, roundedY, sampleGround;

            if (isOnAnyWaterTile)
            {
                roundedX = (int)Mathf.Round(pos.x / baseData.scale);
                roundedY = (int)Mathf.Round(pos.z / baseData.scale);
                if (ExtensionMethods.In2DArrayBounds(baseData.dfTerrain.MapData.tilemapSamples, roundedX, roundedY))
                {
                    sampleGround = baseData.dfTerrain.MapData.tilemapSamples[roundedX, roundedY] & 0x3F;

                    if (
                    sampleGround != 0 &&
                    sampleGround != 4 &&
                    sampleGround != 5 &&
                    sampleGround != 6 &&
                    sampleGround != 7 &&
                    sampleGround != 8 &&
                    sampleGround != 19 &&
                    sampleGround != 20 &&
                    sampleGround != 21 &&
                    sampleGround != 22 &&
                    sampleGround != 23 &&
                    sampleGround != 29 &&
                    sampleGround != 30 &&
                    sampleGround != 31 &&
                    sampleGround != 32 &&
                    sampleGround != 33 &&
                    sampleGround != 34 &&
                    sampleGround != 35 &&
                    sampleGround != 36 &&
                    sampleGround != 37 &&
                    sampleGround != 38 &&
                    sampleGround != 40 &&
                    sampleGround != 41 &&
                    sampleGround != 43 &&
                    sampleGround != 44 &&
                    sampleGround != 48 &&
                    sampleGround != 49 &&
                    sampleGround != 50 &&
                    sampleGround != 60 &&
                    sampleGround != 61)
                    {
                        result = false;
                    }
                }
            }
            if (isOnPureGroundTile)
            {
                roundedX = (int)Mathf.Round(pos.x / baseData.scale);
                roundedY = (int)Mathf.Round(pos.z / baseData.scale);
                if (ExtensionMethods.In2DArrayBounds(baseData.dfTerrain.MapData.tilemapSamples, roundedX, roundedY))
                {
                    sampleGround = baseData.dfTerrain.MapData.tilemapSamples[roundedX, roundedY] & 0x3F;

                    if (sampleGround != 1 && sampleGround != 2 && sampleGround != 3)
                    {
                        result = false;
                    }
                }
            }
            if (isOnOrCloseToShallowWaterTile)
            {
                for (int x = 0; x < 2 && stopCondition == false; x++)
                {
                    for (int y = 0; y < 2 && stopCondition == false; y++)
                    {

                        offsetA = 1f;
                        offsetB = 1f;

                        if (x == 1)
                            roundedX = (int)Mathf.Round((pos.x / baseData.scale) + offsetA);
                        else
                            roundedX = (int)Mathf.Round((pos.x / baseData.scale) + offsetB);
                        if (y == 1)
                            roundedY = (int)Mathf.Round((pos.z / baseData.scale) + offsetA);
                        else
                            roundedY = (int)Mathf.Round((pos.z / baseData.scale) + offsetB);

                        if (ExtensionMethods.In2DArrayBounds(baseData.dfTerrain.MapData.tilemapSamples, roundedX - x, roundedY - y))
                        {
                            sampleGround = baseData.dfTerrain.MapData.tilemapSamples[roundedX - x, roundedY - y] & 0x3F;
                            if (
                              sampleGround != 4 &&
                              sampleGround != 5 &&
                              sampleGround != 6 &&
                              sampleGround != 7 &&
                              sampleGround != 8 &&
                              sampleGround != 19 &&
                              sampleGround != 20 &&
                              sampleGround != 21 &&
                              sampleGround != 22 &&
                              sampleGround != 23 &&
                              sampleGround != 29 &&
                              sampleGround != 30 &&
                              sampleGround != 31 &&
                              sampleGround != 32 &&
                              sampleGround != 33 &&
                              sampleGround != 34 &&
                              sampleGround != 35 &&
                              sampleGround != 36 &&
                              sampleGround != 37 &&
                              sampleGround != 38 &&
                              sampleGround != 40 &&
                              sampleGround != 41 &&
                              sampleGround != 43 &&
                              sampleGround != 44 &&
                              sampleGround != 48 &&
                              sampleGround != 49 &&
                              sampleGround != 50 &&
                              sampleGround != 60 &&
                              sampleGround != 61)
                            {
                                stopCondition = result = false;
                            }
                            else
                                stopCondition = true;
                        }
                    }
                }
            }
            if (isOnOrCloseToStreetTile)
            {
                for (int x = 0; x < 2 && stopCondition == false; x++)
                {
                    for (int y = 0; y < 2 && stopCondition == false; y++)
                    {

                        offsetA = 0.7f;
                        offsetB = -0.3f;

                        if (x == 1)
                            roundedX = (int)Mathf.Round((pos.x / baseData.scale) + offsetA);
                        else
                            roundedX = (int)Mathf.Round((pos.x / baseData.scale) + offsetB);
                        if (y == 1)
                            roundedY = (int)Mathf.Round((pos.z / baseData.scale) + offsetA);
                        else
                            roundedY = (int)Mathf.Round((pos.z / baseData.scale) + offsetB);

                        if (ExtensionMethods.In2DArrayBounds(baseData.dfTerrain.MapData.tilemapSamples, roundedX - x, roundedY - y))
                        {
                            sampleGround = baseData.dfTerrain.MapData.tilemapSamples[roundedX - x, roundedY - y] & 0x3F;
                            if (
                             sampleGround != 46 &&
                             sampleGround != 47 &&
                             sampleGround != 55)
                            {
                                stopCondition = result = false;
                            }
                            else
                                stopCondition = true;
                        }
                    }
                }
            }
            if (isCollidingWithBuilding)
            {
                /* if(baseData.dfTerrain.MapData.hasLocation) {
                   foreach(GameObject go in staticGeometryList) {
                    Vector3 newPos = new Vector3(pos.x,pos.y + 100, pos.z);
                    RaycastHit hit;

                    if (Physics.Raycast(baseData.dfTerrain.transform.TransformPoint(newPos), new Vector3(0,-1,0), out hit, Mathf.Infinity))	{
                        Debug.DrawLine(baseData.dfTerrain.transform.TransformPoint(newPos), hit.point, Color.red,30);

                        if(hit.collider.gameObject.tag == "StaticGeometry")	{
                           Debug.DrawLine(
                            baseData.dfTerrain.transform.TransformPoint(newPos),
                            new Vector3(
                               baseData.dfTerrain.transform.TransformPoint(newPos).x,
                               -200,
                               baseData.dfTerrain.transform.TransformPoint(newPos).z),
                            Color.yellow,30);
                           return true;
                        }
                    }
                   }
                } */
            }
            return result;
        }

        /* static private void AddDescendantsWithTag(
           Transform parent,
           string tag,
           List<GameObject> list)
        {
        foreach (Transform child in parent)
        {
            if (child.gameObject.CompareTag(tag))
            {
            list.Add(child.gameObject);
            }
            AddDescendantsWithTag(child, tag, list);
        }
        } */

        // Noise function
        private float GetNoise(
           float x,
           float y,
           float frequency,
           float amplitude,
           float persistance,
           int octaves,
           int seed = 120)
        {
            float finalValue = 0f;
            for (int i = 0; i < octaves; ++i)
            {
                finalValue += Mathf.PerlinNoise(seed + (x * frequency), seed + (y * frequency)) * amplitude;
                frequency *= 7;
                amplitude *= persistance;
            }

            return Mathf.Clamp(finalValue, -1, 1f);
        }

        // Sets texture by range
        private string GetWeightedRecord(
           float weight,
           float limit1 = 0.3f,
           float limit2 = 0.6f)
        {
            if (weight < limit1)
                return "flower";
            else
            {
                if (weight >= limit1 && weight < limit2)
                    return "grass";
                else
                    return "forest";
            }
        }
    }
}

