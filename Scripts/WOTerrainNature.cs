using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Utility;

namespace WildernessOverhaul
{
    public class WOTerrainNature : ITerrainNature
    {
        // static GameObject exterior;
        // static List<GameObject> staticGeometryList;

        // MOD SETTINGS
        private readonly bool dynamicNatureClearance;
        private readonly bool vegetationInLocations;
        private readonly bool firefliesExist;
        private readonly bool shootingStarsExist;
        private readonly bool InterestingTerrainEnabled;
        private static float fireflyDistance;
        private static float shootingStarsMinimum;
        private static float shootingStarsMaximum;
        private static float generalNatureClearance;
        private static float natureClearance1;
        private static float natureClearance2;
        private static float natureClearance3;
        private static float natureClearance4;
        private static float natureClearance5;

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
            InterestingTerrainEnabled = ITEnabled;
            Debug.Log("Wilderness Overhaul: Interesting Terrain enabled: " + InterestingTerrainEnabled);
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

            float terrainElevation = Mathf.Clamp((dfTerrain.MapData.worldHeight / 128f), 0.0f, 1.0f);

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

            // Adapting maxTerrainHeight for Interesting Terrain Mod
            if (InterestingTerrainEnabled) {
                maxTerrainHeight = 4890;
            }

            // Chance scaled by base climate type
            DFLocation.ClimateSettings climate = MapsFile.GetWorldClimateSettings(dfTerrain.MapData.worldClimate);

            // Initialize and set up Vegetation Lists and Vegetation Spawn Chances by climate, elevation and season
            vegetationList = new WOVegetationList(terrainElevation, DaggerfallUnity.Instance.WorldTime.Now.SeasonValue);
            vegetationChance = new WOVegetationChance(terrainElevation, climate);
            stochastics = new WOStochasticChances();

            // Adds one shooting star Particle System of every MapPixel
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
                    if (tile == 2) {   // Dirt
                        if (Random.Range(0.0f, 1.0f) > vegetationChance.chanceOnGrass)
                            continue;
                    }
                    else if (tile == 1) {   // Grass
                        if (Random.Range(0.0f, 1.0f) > vegetationChance.chanceOnDirt)
                            continue;
                    }
                    else if (tile == 3) {   // Stone
                        if (Random.Range(0.0f, 1.0f) > vegetationChance.chanceOnStone)
                            continue;
                    }
                    else if (tile == 0) {   // Water
                        continue;
                    }
                    else if (tile == 4 || tile == 5 || tile == 6 || tile == 21 || tile == 31 || tile == 32) {   // Shore (Water)
                    }
                    else {   // Anything else
                        continue;
                    }

                    //Defining height for the billboard placement
                    int hx = (int)Mathf.Clamp(hDim * ((float)x / (float)tDim), 0, hDim - 1);
                    int hy = (int)Mathf.Clamp(hDim * ((float)y / (float)tDim), 0, hDim - 1);
                    float height = dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight;

                    if (tile == 0 || height <= 0)
                        continue;

                    BaseDataObject baseData = new BaseDataObject(dfTerrain, dfBillboardBatch, terrain, scale, steepness, x, y);

                    switch (climate.WorldClimate)
                    {
                        #region Temperate Spawns
                        case (int)MapsFile.Climates.Woodlands:

                            weight += GetNoise(latitude, longitude, stochastics.tempForestFrequency, stochastics.tempForestAmplitude, stochastics.tempForestPersistence, stochastics.tempForestOctaves, 100);
                            weight = Mathf.Clamp(weight,0f,1f);
                            if (tile == 1) // Dirt
                            {
                                if (dfTerrain.MapData.worldHeight / 128f > Random.Range(0.15f, 0.18f))
                                {
                                    if (GetWeightedRecord(weight) == "forest")
                                    {
                                        if (Random.Range(0, 100) < Random.Range(70, 80))
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees,Random.Range(0.15f,0.30f), true); // Dead Trees
                                        }
                                        else
                                        {
                                            if (Random.Range(0, 100) < stochastics.mapStyle)
                                            {
                                                for (int i = 0; i < Random.Range(1,5); i++)
                                                {
                                                    AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.75f,1.10f), true, 3); // Needle Tree
                                                }
                                            }
                                            else
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.15f,0.30f), true); // Dead Trees
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

                                    for (int i = 0; i < Random.Range(3,7); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandFlowers, Random.Range(0.45f,0.75f), true); // Flowers
                                    }

                                    float rnd = Random.Range(0, 100);
                                    if (rnd < stochastics.mapStyleChance[1])
                                    {
                                        for (int i = 0; i < Random.Range(2,5); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandFlowers, Random.Range(0.60f,1.00f), true); // Flowers
                                        }
                                    }
                                    if (rnd < 5)
                                    {
                                        for (int i = 0; i < Random.Range(0,3); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(0.85f,1.15f), true); // Bushes
                                        }
                                    }
                                }
                                else if (GetWeightedRecord(weight, stochastics.tempForestLimit[0], stochastics.tempForestLimit[1]) == "grass")
                                {
                                    float rnd = Random.Range(0, 100);
                                    if (rnd < 3)
                                    {
                                        for (int i = 0; i < Random.Range(15,25); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(2.25f,3.00f), true); // Bushes
                                        }
                                    }

                                    if(rnd < 1) {
                                        for (int i = 0; i < Random.Range(1,5); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(1.25f,1.65f), true); // Trees
                                        }
                                    }
                                }
                                else if (GetWeightedRecord(weight, stochastics.tempForestLimit[0], stochastics.tempForestLimit[1]) == "forest")
                                {
                                    float rnd = Random.Range(0, 100);
                                    if (rnd < stochastics.mapStyle)
                                    {

                                        for (int i = 0; i < Random.Range(3,5); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(1.00f,1.35f), true); // Trees
                                        }

                                        for (int i = 0; i < Random.Range(2,4); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(1.00f,1.35f), true); // Bushes
                                        }

                                        AddFirefly(baseData, 0.1f, 5, 15, 35, firefliesExist); // Fireflies

                                        if (rnd < stochastics.mapStyleChance[3])
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBeach, Random.Range(0.10f,0.30f), true); // Beach

                                            for (int i = 0; i < Random.Range(1,2); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(1.00f,1.45f), true); // Trees
                                            }

                                            for (int i = 0; i < Random.Range(1,2); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(1.00f,1.45f), true); // Bushes
                                            }

                                            AddFirefly(baseData, 0.1f, 10, 50, 100, firefliesExist); // Fireflies
                                        }

                                        if (rnd < stochastics.mapStyleChance[1])
                                        {
                                            for (int i = 0; i < Random.Range(0,1); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(1.25f,1.75f), true); // Trees
                                            }

                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.10f,0.30f), true); // Dead Trees

                                            for (int i = 0; i < Random.Range(0,1); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(1.25f,1.75f), true); // Bushes
                                            }

                                            AddFirefly(baseData, 0.1f, 15, 100, 250, firefliesExist); // Fireflies
                                        }
                                    }
                                }
                            }
                            else if (tile == 3) // Stone
                            {
                                if (GetWeightedRecord(weight) == "forest")
                                {
                                    for (int i = 0; i < Random.Range(1,4); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandRocks, Random.Range(0.75f,1.15f), true); // Stones
                                    }
                                }
                                if (GetWeightedRecord(weight) == "flower")
                                {
                                    AddBillboardToBatch(baseData, vegetationList.temperateWoodlandRocks, Random.Range(0.75f,1.15f), true); // Stones
                                }
                            }
                            break;
                        #endregion

                        #region Mountain Spawns
                        case (int)MapsFile.Climates.Mountain:

                            weight += GetNoise(latitude, longitude, stochastics.mountForestFrequency, stochastics.mountForestAmplitude, stochastics.mountForestPersistence, stochastics.mountForestOctaves, 100);

                            if (tile == 1) // Dirt
                            {
                                // Beach
                                if (dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight < beachLine)
                                {
                                    if (Random.Range(0, 100) < stochastics.mapStyle)
                                    {
                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 2)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsBeach, 0.75f, true); // Beach
                                        }
                                    }
                                }
                                else if (dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight >= beachLine && dfTerrain.MapData.heightmapSamples[hy, hx] < treeLine)
                                {
                                    if (Random.Range(0, 100) < Random.Range(10, 20))
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.mountainsDeadTrees, 0.75f, true); // Dead Trees

                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 1)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsDeadTrees, 1.50f, true); // Dead Trees
                                        }
                                    }
                                    else
                                    {
                                        if (Random.Range(0, 100) < stochastics.mapStyle)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 0.00f, true); // Needle Tree

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 1.00f, true); // Needle Tree
                                            }

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 1.50f, true); // Needle Tree
                                            }
                                        }
                                        else
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsDeadTrees, 0.50f, true); // Dead Trees

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 1.25f, true); // Needle Trees
                                            }

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 1)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.75f, true); // Rocks
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 3)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 1.00f, true); // Rocks
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 3)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.mountainsBeach, 1.00f, true); // Beach
                                    }
                                }
                            }
                            else if (tile == 2) // Grass
                            {
                                float rndMajor = Random.Range(0.0f, 100.0f);
                                if (Random.Range(0.0f, 100.0f) < stochastics.mountainStoneCircleChance)
                                {
                                    AddStoneCircleToBatch(baseData, vegetationList.mountainsRocks, 5, 0);
                                }
                                if (dfTerrain.MapData.heightmapSamples[hy, hx] > treeLine)
                                {
                                    float rndMinor = Random.Range(0, 100);
                                    if (rndMinor < stochastics.mapStyle)
                                    {
                                        if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.97f)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.00f, true); // Rock
                                        }

                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 4)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.5f, true); // Flowers
                                        }

                                        if (rndMinor < 40)
                                        {
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 4)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.00f, true); // Flowers
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 4)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.5f, true); // Rocks
                                        }

                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 4)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 0.75f, true); // Grass
                                        }

                                        if (rndMinor < 40)
                                        {
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 4)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 1.25f, true); // Grass
                                            }
                                        }
                                    }
                                }
                                else if (dfTerrain.MapData.heightmapSamples[hy, hx] < treeLine && dfTerrain.MapData.heightmapSamples[hy, hx] >= Random.Range(0.70f, 0.72f))
                                {
                                    if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "flower")
                                    {
                                        float rndMinor = Random.Range(0, 100);
                                        if (rndMinor < stochastics.mapStyle)
                                        {
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsTrees, 0.50f, true); // Trees
                                            }

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 4)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsTrees, 1.00f, true); // Trees
                                            }

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 4)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 0.50f, true); // Grass
                                            }

                                            if (rndMinor < 40)
                                            {
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 4)); i++)
                                                {
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsTrees, 1.50f, true); // Trees
                                                }

                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 4)); i++)
                                                {
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.75f, true); // Trees
                                                }

                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                                {
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 1.00f, true); // Grass
                                                }
                                            }

                                            if (rndMinor < 30)
                                            {
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 2)); i++)
                                                {
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 1.75f, true); // Trees
                                                }
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                                {
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.75f, true); // Trees
                                                }

                                                if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.98f)
                                                {
                                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 2)); i++)
                                                    {
                                                        AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 1.00f, true); // Rocks
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "forest")
                                    {
                                        if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.98f)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.00f, true); // Rock
                                        }

                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 2)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsTrees, 0.50f, true); // Trees
                                        }

                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 2)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsTrees, 1.00f, true); // Trees
                                        }

                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.20f, true); // Flowers
                                        }

                                        float rndMinor = Random.Range(0, 100);
                                        if (rndMinor < 40)
                                        {
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.50f, true); // Flowers
                                            }
                                        }
                                    }
                                }
                                else if (dfTerrain.MapData.heightmapSamples[hy, hx] < Random.Range(0.70f, 0.72f) && dfTerrain.MapData.heightmapSamples[hy, hx] > Random.Range(0.45f, 0.40f))
                                {
                                    if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "forest")
                                    {
                                        float rndMinor = Random.Range(0, 100);
                                        if (rndMinor < stochastics.mapStyle)
                                        {
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsTrees, 0.50f, true); // Trees
                                            }

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 0.50f, true); // Grass
                                            }

                                            if (rndMinor < 40)
                                            {
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                                {
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.00f, true); // Flowers
                                                }
                                            }

                                            if (rndMinor < 30)
                                            {
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                                {
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.75f, true); // Flowers
                                                }

                                                if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.95f)
                                                {
                                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 2)); i++)
                                                    {
                                                        AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 1.00f, true); // Rocks
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "flower")
                                    {
                                        if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.95f)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.00f, true); // Rock
                                        }

                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.50f, true); // Flowers
                                        }

                                        float rndMinor = Random.Range(0, 100);
                                        if (rndMinor < 40)
                                        {
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                            {
                                                AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.75f, true); // Flowers
                                            }
                                        }
                                    }
                                }
                                else if (dfTerrain.MapData.heightmapSamples[hy, hx] < Random.Range(0.40f, 0.45f))
                                {
                                    float rndMinor = Random.Range(0, 100);
                                    if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "forest")
                                    {
                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsTrees, 0.50f, true); // Trees
                                        }

                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 0.50f, true); // Grass
                                        }

                                        if (rndMinor < 30)
                                        {
                                            if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.95f)
                                            {
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 2)); i++)
                                                {
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 1.00f, true); // Rocks
                                                }
                                            }
                                        }
                                    }
                                    else if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "flower")
                                    {
                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                        {
                                            AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.50f, true); // Trees
                                        }

                                        if (rndMinor < 30)
                                        {
                                            if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.95f)
                                            {
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 1)); i++)
                                                {
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 1.00f, true); // Rocks
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (tile == 3) // Stone
                            {
                                if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "forest")
                                {
                                    AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.00f, true); // Stones

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 2)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.25f, true); // Stones
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 3)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.50f, true); // Flowers
                                    }
                                }
                                if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "flower")
                                {
                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 4)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.75f, true); // Flowers
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Desert1 Spawns
                        case (int)MapsFile.Climates.Desert: //STEPPE

                            /* if (tile == 1) // Dirt
              {
                float elevationRnd = Random.Range(0.01f,0.03f);
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd && Random.Range(0.0f,100.0f) < Random.Range(0.0f,20.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/2 && Random.Range(0.0f,100.0f) < Random.Range(10.0f,40.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/3 && Random.Range(0.0f,100.0f) < Random.Range(30.0f,70.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(Random.Range(0,100) < desert1DirtChance)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Trees

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 3.5f); // Dead Trees
                  }
                }
                else
                {
                  if(Random.Range(0,100) < Random.Range(20,55))
                  {
                    if(Random.Range(0,100) < mapStyleChance)
                    {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, 1f); // Cactus

                if(Random.Range(0,100) < Random.Range(2,6))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,6)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 1.5f); // Flowers
                  }

                  if(Random.Range(0,100) < Random.Range(0,15))
                  {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                    }
                  }
                }
                    }
                  }
                }
              }
              else if (tile == 2) // Grass
              {
                float elevationRnd = Random.Range(0.01f,0.03f);
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd && Random.Range(0.0f,100.0f) < Random.Range(0.0f,15.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/2 && Random.Range(0.0f,100.0f) < Random.Range(5.0f,30.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/3 && Random.Range(0.0f,100.0f) < Random.Range(20.0f,50.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(Random.Range(0,100) < Random.Range(5,15))
                {
                  float rndMajor = Random.Range(0,100);
                  if(rndMajor < mapStyleChance)
                  {
                    if(Random.Range(0,100) < Random.Range(30,70))
                    {
                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,6)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 4f); // Flowers
                }
                    }

                    float rndGrass = Random.Range(0.0f,100.0f);
                    if (rndGrass <= desert1GrassChance1)
                    {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, 0.25f); // Cactus

                if(Random.Range(0,100) < Random.Range(10,15))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,8)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 0.5f); // Flowers
                  }

                  if(Random.Range(0,100) < Random.Range(10,15))
                  {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                    }
                  }
                }
                    }
                    else if(rndGrass > desert1GrassChance1 && rndGrass <= desert1GrassChance2)
                    {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, 0.25f); // Plant

                if(Random.Range(0,100) < Random.Range(10,15))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,8)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 0.5f); // Flowers
                  }

                  if(Random.Range(0,100) < Random.Range(10,15))
                  {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                    }
                  }
                }
                    }

                    else if(Random.Range(0,100) < Random.Range(3,13))
                    {
                for(int i = 0; i < (int)Mathf.Round(Random.Range(5,25)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Cactus
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(50,100)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Flowers
                }
                    }

                    else if(Random.Range(0,100) < Random.Range(10,20))
                    {
                for(int i = 0; i < (int)Mathf.Round(Random.Range(50,100)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(2.5f,8f)); // Flowers
                }
                    }
                  }
                  else
                  {
                    if(Random.Range(0,100) < Random.Range(1,6))
                    {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Tree
                if (rndMajor > mapStyleChance4)
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,5)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 0.5f); // Flowers
                  }
                }

                if (rndMajor > mapStyleChance3)
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,3)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 3.5f); // Dead Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,20)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 3f); // Flowers
                  }
                }
                    }
                    else if(Random.Range(0.0f,100.0f) < Random.Range(10.0f,30.0f))
                    {
                for(int i = 0; i < (int)Mathf.Round(Random.Range(4,15)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(5,30)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(15,40)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,10)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(60,135)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                }
                    }
                  }
                }
              }
              else if (tile == 3) // Stone
              {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 1f); // Stones

                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,5)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 2f); // Stones
                }
              }*/
                            if (
                              tile == 0 || tile == 4 || tile == 5 || tile == 6 || tile == 7 || tile == 8 || tile == 19 || tile == 20 || tile == 21 || tile == 22 ||
                              tile == 23 || tile == 29 || tile == 30 || tile == 31 || tile == 32 || tile == 33 || tile == 34 || tile == 35 || tile == 36 || tile == 37 ||
                              tile == 38 || tile == 40 || tile == 41 || tile == 43 || tile == 44 || tile == 48 || tile == 49 || tile == 50)
                            {
                                int rndMajor = Random.Range(0, 3);
                                if (rndMajor < 1)
                                {
                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(5, 10)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(2f, 2.5f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(10, 25)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(3.5f, 6f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(20, 40)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterPlants, Random.Range(4f, 7f), true); // Plants
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(40, 80)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(5f, 9f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(20, 40)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(1.2f, 3f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(10, 20)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterFlowers, Random.Range(0.2f, 1.5f), false); // Flowers
                                    }
                                }
                                else if (rndMajor >= 1 && rndMajor < 2)
                                {
                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(3, 8)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(1f, 3.5f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(6, 18)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(2f, 6f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(15, 30)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterPlants, Random.Range(2.5f, 7f), true); // Plants
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(30, 60)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(2.5f, 9f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(10, 20)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(1.2f, 3f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(5, 10)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterFlowers, Random.Range(0.2f, 0.5f), false); // Flowers
                                    }
                                }
                                else if (rndMajor >= 2)
                                {
                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 5)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(1f, 3.5f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(4, 12)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(1.5f, 6f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(10, 20)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterPlants, Random.Range(1.5f, 7f), true); // Plants
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(15, 25)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(1.5f, 9f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(6, 12)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(1.2f, 3f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 5)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterFlowers, Random.Range(0.2f, 0.5f), false); // Flowers
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Desert2 Spawns
                        case (int)MapsFile.Climates.Desert2: //REAL Desert

                            /*if (tile == 1) // Dirt
              {
                float elevationRnd = Random.Range(0.01f,0.03f);
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd && Random.Range(0.0f,100.0f) < Random.Range(0.0f,25.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/2 && Random.Range(0.0f,100.0f) < Random.Range(25.0f,50.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/3 && Random.Range(0.0f,100.0f) < Random.Range(50.0f,75.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(Random.Range(0,100) < desert2DirtChance)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Trees

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,5)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 3.5f); // Dead Trees
                  }
                }
                if(Random.Range(0,100) < Random.Range(10,15))
                {
                  if(Random.Range(0,100) < mapStyleChance)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, 1f); // Cactus

                    if(Random.Range(0,100) < Random.Range(2,6))
                    {
                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,6)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 1.5f); // Flowers
                }

                if(Random.Range(0,100) < Random.Range(0,15))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                  }
                }
                    }
                  }
                }
              }
              else if (tile == 2) // Grass
              {
                float elevationRnd = Random.Range(0.01f,0.03f);
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd && Random.Range(0.0f,100.0f) < Random.Range(0.0f,25.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/2 && Random.Range(0.0f,100.0f) < Random.Range(25.0f,50.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(dfTerrain.MapData.heightmapSamples[hy, hx] < elevationRnd/3 && Random.Range(0.0f,100.0f) < Random.Range(50.0f,75.0f))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                  }
                }
                if(Random.Range(0,100) < Random.Range(0,3))
                {
                  float rndMajor = Random.Range(0,100);
                  if(rndMajor < mapStyleChance)
                  {
                    float rndGrass = Random.Range(0.0f,100.0f);
                    if (rndGrass <= desert2GrassChance1)
                    {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, 0.25f); // Cactus

                if(Random.Range(0,100) < Random.Range(10,15))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,8)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 1.5f); // Flowers
                  }

                  if(Random.Range(0,100) < Random.Range(10,15))
                  {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 3f); // Flowers
                    }
                  }
                }
                    }
                    else if(rndGrass > desert2GrassChance1 && rndGrass <= desert2GrassChance2)
                    {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, 0.25f); // Plant

                if(Random.Range(0,100) < Random.Range(10,15))
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,8)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 1.5f); // Flowers
                  }

                  if(Random.Range(0,100) < Random.Range(10,15))
                  {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,15)); i++)
                    {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2.75f); // Flowers
                    }
                  }
                }
                    }
                    else if(Random.Range(0.0f,100.0f) < Random.Range(0.0f,10.0f))
                    {
                for(int i = 0; i < (int)Mathf.Round(Random.Range(4,10)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(2f,3f)); // Trees
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertTrees, scale, steepness, terrain, x, y, Random.Range(3f,5f)); // Trees
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(20,50)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertPlants, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Plants
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(20,40)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(3.5f,9f)); // Cactus
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(20,45)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3f,6f)); // Flowers
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(80,165)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,12f)); // Flowers
                }
                    }

                    else if(Random.Range(0,100) < Random.Range(0,30))
                    {
                for(int i = 0; i < (int)Mathf.Round(Random.Range(10,40)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertCactus, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Cactus
                }

                for(int i = 0; i < (int)Mathf.Round(Random.Range(50,125)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(3.5f,7f)); // Flowers
                }
                    }

                    else if(Random.Range(0,100) < Random.Range(0,50))
                    {
                for(int i = 0; i < (int)Mathf.Round(Random.Range(50,125)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, Random.Range(2.5f,6.5f)); // Flowers
                }
                    }
                  }
                  else
                  {
                    if(Random.Range(0,100) < Random.Range(5,10))
                    {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Tree


                if (rndMajor > mapStyleChance4)
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,5)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                  }
                }

                if (rndMajor > mapStyleChance3)
                {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertDeadTrees, scale, steepness, terrain, x, y, 3.5f); // Dead Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,10)); i++)
                  {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 3f); // Flowers
                  }
                }
                    }
                  }
                }
              }
              else if (tile == 3) // Stone
              {
                AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 1f); // Stones

                for(int i = 0; i < (int)Mathf.Round(Random.Range(0,7)); i++)
                {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, desertStones, scale, steepness, terrain, x, y, 2.5f); // Stones
                }
              }*/
                            if (
                              tile == 0 || tile == 4 || tile == 5 || tile == 6 || tile == 7 || tile == 8 || tile == 19 || tile == 20 || tile == 21 || tile == 22 ||
                              tile == 23 || tile == 29 || tile == 30 || tile == 31 || tile == 32 || tile == 33 || tile == 34 || tile == 35 || tile == 36 || tile == 37 ||
                              tile == 38 || tile == 40 || tile == 41 || tile == 43 || tile == 44 || tile == 48 || tile == 49 || tile == 50)
                            {
                                int rndMajor = Random.Range(0, 3);
                                if (rndMajor < 1)
                                {
                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(5, 20)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(2f, 2.5f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(10, 35)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(3.5f, 6f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(30, 50)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterPlants, Random.Range(4f, 7f), true); // Plants
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(60, 120)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(5f, 9f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(20, 40)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(1.2f, 3f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(25, 40)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterFlowers, Random.Range(0.2f, 1.5f), false); // Flowers
                                    }
                                }
                                else if (rndMajor >= 1 && rndMajor < 2)
                                {
                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(3, 8)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(1f, 3.5f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(6, 18)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(2f, 6f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(17, 35)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterPlants, Random.Range(2.5f, 7f), true); // Plants
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(40, 75)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(2.5f, 9f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(10, 20)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(1.2f, 3f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(10, 25)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterFlowers, Random.Range(0.2f, 1.5f), false); // Flowers
                                    }
                                }
                                else if (rndMajor >= 2)
                                {
                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 5)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(1f, 3.5f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(4, 12)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(1.5f, 6f), true); // Trees
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(10, 25)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterPlants, Random.Range(1.5f, 7f), true); // Plants
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(15, 35)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(1.5f, 9f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(6, 12)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertFlowers, Random.Range(1.2f, 3f), true); // Flowers
                                    }

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(5, 15)); i++)
                                    {
                                        AddBillboardToBatch(baseData, vegetationList.desertWaterFlowers, Random.Range(0.2f, 1.5f), false); // Flowers
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Haunted Woodlands Spawns
                        case (int)MapsFile.Climates.HauntedWoodlands:

                            /* if (tile == 1) // Dirt
              {
                  if(Random.Range(0,100) > mapStyleChance2)
                  {
                if(Random.Range(0,100) < Random.Range(95,100))
                {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandPlants, scale, steepness, terrain, x, y, 0.25f); // Dead Trees

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 1.5f); // Dirt Trees
                    }
                }
                else
                {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 1.5f); // Bones

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandPlants, scale, steepness, terrain, x, y, 1.5f); // Plants
                    }
                }
                  }
                  else
                  {
                if(Random.Range(0,100) < Random.Range(50,60))
                {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandPlants, scale, steepness, terrain, x, y, 0.25f); // Dead Trees

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 1.5f); // Dirt Trees
                    }
                }
                else
                {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandPlants, scale, steepness, terrain, x, y, 1.5f); // Plants
                    }
                }
                  }
              }
              else if (tile == 2) // Grass
              {
                  if (Random.Range(0.0f,100.0f) <= 0.075f) // Mushroom Circle
                  {
                Vector3 pos = new Vector3(x * scale, 0, (y + 0.5f) * scale);
                float height2 = terrain.SampleHeight(pos + terrain.transform.position);
                pos.y = height2 - (steepness / slopeSinkRatio);
                dfBillboardBatch.AddItem(Random.Range(22,23), pos);

                pos = new Vector3((x + 0.272f) * scale, 0, (y - 0.404f) * scale);
                height2 = terrain.SampleHeight(pos + terrain.transform.position);
                pos.y = height2 - (steepness / slopeSinkRatio);
                dfBillboardBatch.AddItem(Random.Range(22,23), pos);

                pos = new Vector3((x - 0.272f) * scale, 0, (y - 0.404f) * scale);
                height2 = terrain.SampleHeight(pos + terrain.transform.position);
                pos.y = height2 - (steepness / slopeSinkRatio);
                dfBillboardBatch.AddItem(Random.Range(22,23), pos);

                pos = new Vector3((x - 0.475f) * scale, 0, (y + 0.154f) * scale);
                height2 = terrain.SampleHeight(pos + terrain.transform.position);
                pos.y = height2 - (steepness / slopeSinkRatio);
                dfBillboardBatch.AddItem(Random.Range(22,23), pos);

                pos = new Vector3((x + 0.475f) * scale, 0, (y + 0.154f) * scale);
                height2 = terrain.SampleHeight(pos + terrain.transform.position);
                pos.y = height2 - (steepness / slopeSinkRatio);
                dfBillboardBatch.AddItem(Random.Range(22,23), pos);
                  }

                  float rndMajor = Random.Range(0,100);
                  if(rndMajor < mapStyleChance)
                  {
                if(Random.Range(0,100) < Random.Range(0,30))
                {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(6,8)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandTrees, scale, steepness, terrain, x, y, 2.5f); // Trees
                    }

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 2.5f); // Dead Trees
                    }

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 2.5f); // Dirt Trees
                    }

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(5,10)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 2f); // Bushes
                    }

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(10,20)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 2.75f); // Bushes
                    }

                    if(rndMajor < mapStyleChance2)
                    {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,6)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandTrees, scale, steepness, terrain, x, y, 2f); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 2f); // Dead Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 2f); // Dirt Trees
                  }
                    }

                    if(rndMajor < mapStyleChance1)
                    {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 2.5f); // Stones
                  }
                    }
                }
                else
                {
                    if(Random.Range(0,100) < Random.Range(0,30))
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandFlowers, scale, steepness, terrain, x, y, 0.35f); // Flowers

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandFlowers, scale, steepness, terrain, x, y, 0.55f); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 1f); // Dead Trees
                  }

                  if (rndMajor > mapStyleChance2)
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandMushroom, scale, steepness, terrain, x, y, 0.75f); // Mushrooms
                      }

                      if(Random.Range(0, 100) < Random.Range(0,5))
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 0.5f); // Bones

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 1.5f); // Dirt Trees
                    }
                      }
                  }

                  if (rndMajor > mapStyleChance1)
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,2)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandMushroom, scale, steepness, terrain, x, y, 0.5f); // Mushrooms
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 0.75f); // Bushes
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 1.5f); // Dead Trees
                      }

                      if(Random.Range(0, 100) < Random.Range(0,5))
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 0.5f); // Bones
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 1.5f); // Stones
                      }
                  }
                    }
                }
                  }
                  else
                  {
                if(Random.Range(0,100) < Random.Range(0,70))
                {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(6,8)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandTrees, scale, steepness, terrain, x, y, 2.5f); // Trees
                    }

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 2.5f); // Dead Trees
                    }

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 2.5f); // Dirt Trees
                    }

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(5,10)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 2f); // Bushes
                    }

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(10,20)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 2.75f); // Bushes
                    }

                    if(rndMajor < mapStyleChance3)
                    {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(4,6)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandTrees, scale, steepness, terrain, x, y, 2f); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 2f); // Dead Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 2f); // Dirt Trees
                  }
                    }

                    if(rndMajor < mapStyleChance4)
                    {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 2.5f); // Stones
                  }
                    }
                }
                else
                {
                    if(Random.Range(0,100) < Random.Range(0,30))
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandFlowers, scale, steepness, terrain, x, y, 0.35f); // Flowers

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandFlowers, scale, steepness, terrain, x, y, 0.55f); // Flowers
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 1f); // Dead Trees
                  }

                  if (rndMajor > mapStyleChance3)
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandMushroom, scale, steepness, terrain, x, y, 0.75f); // Mushrooms
                      }

                      if(Random.Range(0, 100) < Random.Range(0,5))
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 0.5f); // Bones

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDirtTrees, scale, steepness, terrain, x, y, 1.5f); // Dirt Trees
                    }
                      }
                  }

                  if (rndMajor > mapStyleChance4)
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,2)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandMushroom, scale, steepness, terrain, x, y, 0.5f); // Mushrooms
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBushes, scale, steepness, terrain, x, y, 0.75f); // Bushes
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 1.5f); // Dead Trees
                      }

                      if(Random.Range(0, 100) < Random.Range(0,5))
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 0.5f); // Bones
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 1.5f); // Stones
                      }
                  }
                    }
                }
                  }
              }
              else if (tile == 3) // Stone
              {
                  if(Random.Range(0,100) > mapStyleChance)
                  {
                if(Random.Range(0,100) < Random.Range(10,20))
                {
                    if(Random.Range(0,100) < Random.Range(95,100))
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Trees
                    }
                    else
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 1.5f); // Bones
                    }
                }
                else
                {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 1.5f); // Stones
                    }
                }
                  }
                  else
                  {
                if(Random.Range(0,100) < Random.Range(10,20))
                {
                    if(Random.Range(0,100) < Random.Range(95,100))
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandDeadTrees, scale, steepness, terrain, x, y, 0.25f); // Dead Trees
                    }
                    else
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandBones, scale, steepness, terrain, x, y, 1.5f); // Bones
                    }
                }
                else
                {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, hauntedWoodlandRocks, scale, steepness, terrain, x, y, 1.5f); // Stones
                    }
                }
                  }
              } */
                            break;
                        #endregion

                        #region Woodland Hills Spawns
                        case (int)MapsFile.Climates.MountainWoods:

                            /* if (tile == 1) // Dirt
                  {
                if(dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight < beachLine)
                {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBeach, scale, steepness, terrain, x, y, 0.25f); // Beach

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBeach, scale, steepness, terrain, x, y, 1.5f); // Beach
                    }
                }
                else if(Random.Range(0,100) > mapStyleChance2)
                {
                    if(Random.Range(0,100) < Random.Range(95,100))
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDirtPlants, scale, steepness, terrain, x, y, 0.75f); // Dirt Plants

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 1.5f); // Needle Trees
                  }
                    }
                    else
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDeadTrees, scale, steepness, terrain, x, y, 2f); // Dead Tree

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes
                  }
                    }
                }
                else
                {
                    if(Random.Range(0,100) < Random.Range(50,60))
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDirtPlants, scale, steepness, terrain, x, y, 0.25f); // Dirt Plants

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, 1.5f); // Needle Trees
                  }
                    }
                    else
                    {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDirtPlants, scale, steepness, terrain, x, y, 1.5f); // Dirt Plants
                  }
                    }
                }
                  }
                  else if (tile == 2) // Grass
                  {
                float rndMajor = Random.Range(0,100);
                if(rndMajor < mapStyleChance)
                {
                    if(Random.Range(0,100) < Random.Range(0,30))
                    {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(5,8)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDeadTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Dead Trees
                  }

                  if(terrainElevation > 0.3f)
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, Random.Range(3f,4f)); // Needle Trees
                      }
                  }
                  else
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(3f,4f)); // Trees
                      }
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(5,10)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, Random.Range(2f,2.5f)); // Bushes
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(10,20)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Bushes
                  }

                  if(rndMajor < mapStyleChance2)
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(3,5)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3f)); // Trees
                      }

                      if(terrainElevation > 0.28f)
                      {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(1,2)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsDeadTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3f)); // Dead Trees
                    }
                      }
                      else
                      {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(1,2)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3f)); // Trees
                    }
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3f)); // Needle Trees
                      }
                  }

                  if(rndMajor < mapStyleChance1)
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsRocks, scale, steepness, terrain, x, y, Random.Range(2.5f,3.5f)); // Stones
                      }
                  }
                    }
                    else
                    {
                  if(rndMajor < mapStyleChance)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 0f);  // Trees

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,3)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.5f,3f)); // Trees
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, Random.Range(2.25f,2.75f)); // Bushes
                      }
                      if(rndMajor < mapStyleChance1)
                      {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(1,2)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.75f,3.25f)); // Trees
                    }
                      }

                      if(rndMajor < mapStyleChance0)
                      {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,3)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.75f,3.25f)); // Trees
                    }
                      }
                  }
                  else
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, 0.35f); // Flowers
                      if (rndMajor > mapStyleChance4)
                      {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(3,8)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(0.35f,0.55f)); // Flowers
                    }
                      }

                      if (rndMajor > mapStyleChance3)
                      {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(3,8)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(0.9f,1.3f)); // Flowers
                    }
                      }
                  }
                    }
                }
                else
                {
                    if(Random.Range(0,100) < Random.Range(0,70))
                    {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(3,5)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.25f,2.75f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.25f,2.75f)); // Trees
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(3,5)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, Random.Range(1.75f,2.25f)); // Bushes
                  }

                  for(int i = 0; i < (int)Mathf.Round(Random.Range(6,8)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsBushes, scale, steepness, terrain, x, y, Random.Range(2.25f,2.75f)); // Bushes
                  }

                  if(rndMajor < mapStyleChance3)
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(4,6)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.25f,3.75f)); // Trees
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, Random.Range(2.75f,3.25f)); // Trees
                      }

                  }

                  if(rndMajor < mapStyleChance4)
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(2,5)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsRocks, scale, steepness, terrain, x, y, Random.Range(1.75f,2.25f)); // Stones
                      }
                  }
                    }
                    else
                    {
                  if(Random.Range(0,100) < Random.Range(0,50))
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, 0.35f); // Flowers

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,5)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(0.55f, 0.75f)); // Flowers
                      }

                      if (rndMajor > mapStyleChance4)
                      {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(3,8)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(1.25f, 1.75f)); // Flowers
                    }
                      }

                      if (rndMajor > mapStyleChance3)
                      {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(3,8)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(1.25f, 1.8f)); // Flowers
                    }
                      }

                      for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 1f); // Trees
                      }

                      if (rndMajor > mapStyleChance3)
                      {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(2,4)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(1f,1.5f)); // Flowers
                    }

                    if(Random.Range(0, 100) < Random.Range(0,5))
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(1.35f, 1.65f)); // Flowers

                        for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                        {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 1f); // Trees
                        }
                    }
                      }

                      if (rndMajor > mapStyleChance4)
                      {
                    for(int i = 0; i < (int)Mathf.Round(Random.Range(1,5)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(0.75f, 1.25f)); // Flowers
                    }

                    for(int i = 0; i < (int)Mathf.Round(Random.Range(0,1)); i++)
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 0f); // Trees
                    }

                    if(Random.Range(0, 100) < Random.Range(0,5))
                    {
                        AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsFlowers, scale, steepness, terrain, x, y, Random.Range(1.25f, 1.75f)); // Flowers
                    }
                      }
                  }
                    }
                }
                  }
                  else if (tile == 3) // Stone
                  {
                if(Random.Range(0,100) > mapStyleChance)
                {
                    if(Random.Range(0,100) < Random.Range(10,20))
                    {
                  if(Random.Range(0,100) < Random.Range(85,100))
                  {
                      if(terrainElevation > 0.28f)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, 1.5f); // Needle Trees
                      }
                      else
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 1.5f); // Trees
                      }
                  }
                  else
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsRocks, scale, steepness, terrain, x, y, 1.5f); // Rocks
                  }
                    }
                    else
                    {
                  for(int i = 0; i < (int)Mathf.Round(Random.Range(0,2)); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsRocks, scale, steepness, terrain, x, y, 1.5f); // Rocks
                  }
                    }
                }
                else
                {
                    if(Random.Range(0,100) < Random.Range(10,20))
                    {
                  if(Random.Range(0,100) < Random.Range(85,100))
                  {
                      if(terrainElevation > 0.28f)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, 1.5f); // Needle Trees
                      }
                      else
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsTrees, scale, steepness, terrain, x, y, 1.5f); // Trees
                      }
                  }
                  else
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsRocks, scale, steepness, terrain, x, y, 1.5f); // Rocks
                  }
                    }
                    else
                    {
                  if(terrainElevation > 0.28f)
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, Random.Range(1.5f, 1.75f)); // Needle Trees
                      }
                  }
                  else
                  {
                      for(int i = 0; i < (int)Mathf.Round(Random.Range(1,3)); i++)
                      {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, woodlandHillsNeedleTrees, scale, steepness, terrain, x, y, Random.Range(1.5f, 1.75f)); // Needle Trees
                      }
                  }
                    }
                }
                  } */
                            break;
                        #endregion

                        #region Rainforest Spawns
                        case (int)MapsFile.Climates.Rainforest:

                            /* if (tile == 1) // Dirt
              {
                  if(Random.Range(0,100) < Random.Range(25,75))
                  {
                if(Random.Range(0,100) < mapStyleChance)
                {
                    for(int i = 0; i < Random.Range(1,3); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1f); // Trees
                    }

                    if(Random.Range(0,100) < mapStyleChance2)
                    {
                  for(int i = 0; i < Random.Range(0,6); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance1)
                    {
                  for(int i = 0; i < Random.Range(0,5); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance0)
                    {
                  for(int i = 0; i < Random.Range(0,3); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestEggs, scale, steepness, terrain, x, y, 0.25f); // Eggs
                  }
                    }
                }
                else
                {
                    for(int i = 0; i < Random.Range(1,3); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1f); // Trees
                    }

                    if(Random.Range(0,100) < mapStyleChance0)
                    {
                  for(int i = 0; i < Random.Range(0,6); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance2)
                    {
                  for(int i = 0; i < Random.Range(0,5); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance1)
                    {
                  for(int i = 0; i < Random.Range(0,3); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestEggs, scale, steepness, terrain, x, y, 0.25f); // Eggs
                  }
                    }
                }
                  }
              }
              else if (tile == 2) // Grass
              {
                  if(Random.Range(0,100) < Random.Range(25,75))
                  {
                if(Random.Range(0,100) < mapStyleChance)
                {
                    for(int i = 0; i < Random.Range(4,6); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1.5f); // Trees
                    }

                    if(Random.Range(0,100) < mapStyleChance2)
                    {
                  for(int i = 0; i < Random.Range(0,6); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestPlants, scale, steepness, terrain, x, y, 1.5f); // Plants
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance1)
                    {
                  for(int i = 0; i < Random.Range(0,6); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance0)
                    {
                  for(int i = 0; i < Random.Range(0,5); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance0)
                    {
                  for(int i = 0; i < Random.Range(0,4); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestRocks, scale, steepness, terrain, x, y, 2f); // Rocks
                  }
                    }
                }
                else
                {
                    for(int i = 0; i < Random.Range(4,6); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1.5f); // Trees
                    }

                    if(Random.Range(0,100) < mapStyleChance4)
                    {
                  for(int i = 0; i < Random.Range(0,6); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestPlants, scale, steepness, terrain, x, y, 1.5f); // Plants
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance3)
                    {
                  for(int i = 0; i < Random.Range(0,6); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance2)
                    {
                  for(int i = 0; i < Random.Range(0,5); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance1)
                    {
                  for(int i = 0; i < Random.Range(0,4); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestRocks, scale, steepness, terrain, x, y, 2f); // Rocks
                  }
                    }
                }
                  }
              }
              else if (tile == 3) // Stone
              {
                  if(Random.Range(0,100) < Random.Range(25,75))
                  {
                if(Random.Range(0,100) < mapStyleChance)
                {
                    for(int i = 0; i < Random.Range(1,3); i++)
                    {
                  AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1f); // Trees
                    }

                    if(Random.Range(0,100) < mapStyleChance0)
                    {
                  for(int i = 0; i < Random.Range(0,6); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestPlants, scale, steepness, terrain, x, y, 1.5f); // Plants
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance0)
                    {
                  for(int i = 0; i < Random.Range(0,6); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance1)
                    {
                  for(int i = 0; i < Random.Range(0,5); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance2)
                    {
                  for(int i = 0; i < Random.Range(0,4); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestRocks, scale, steepness, terrain, x, y, 2f); // Rocks
                  }
                    }
                }
                else
                {
                    AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestTrees, scale, steepness, terrain, x, y, 1f); // Trees

                    if(Random.Range(0,100) < mapStyleChance2)
                    {
                  for(int i = 0; i < Random.Range(0,6); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestPlants, scale, steepness, terrain, x, y, 1.5f); // Plants
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance3)
                    {
                  for(int i = 0; i < Random.Range(0,6); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestBushes, scale, steepness, terrain, x, y, 1.5f); // Bushes
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance4)
                    {
                  for(int i = 0; i < Random.Range(0,5); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestFlowers, scale, steepness, terrain, x, y, 2f); // Flowers
                  }
                    }

                    if(Random.Range(0,100) < mapStyleChance5)
                    {
                  for(int i = 0; i < Random.Range(0,4); i++)
                  {
                      AddBillboardToBatch(dfTerrain, dfBillboardBatch, rainforestRocks, scale, steepness, terrain, x, y, 2f); // Rocks
                  }
                    }
                }
                  }
              } */
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
              !IsOnAnyWaterTile(baseData.dfTerrain, pos, baseData.scale) &&
              !IsCollidingWithBuilding(baseData.dfTerrain, pos, baseData.scale) &&
              !IsOnOrCloseToStreetTile(baseData.dfTerrain, pos, baseData.scale) &&
              baseData.steepness < Random.Range(40f,50f))
            {
                baseData.dfBillboardBatch.AddItem(billboardCollection[rnd], pos);
            }
            if (!checkOnLand &&
              IsOnOrCloseToShallowWaterTile(baseData.dfTerrain, pos, baseData.scale) &&
              !IsCollidingWithBuilding(baseData.dfTerrain, pos, baseData.scale) &&
              !IsOnOrCloseToStreetTile(baseData.dfTerrain, pos, baseData.scale) &&
              baseData.steepness < Random.Range(30f,45f))
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
              !IsOnAnyWaterTile(baseData.dfTerrain, pos, baseData.scale) &&
              !IsCollidingWithBuilding(baseData.dfTerrain, pos, baseData.scale) &&
              !IsOnOrCloseToStreetTile(baseData.dfTerrain, pos, baseData.scale) &&
              baseData.steepness < Random.Range(30f,45f))
            {
                baseData.dfBillboardBatch.AddItem(billboardCollection[record], pos);
            }
            if (!checkOnLand &&
              IsOnOrCloseToShallowWaterTile(baseData.dfTerrain, pos, baseData.scale) &&
              !IsCollidingWithBuilding(baseData.dfTerrain, pos, baseData.scale) &&
              !IsOnOrCloseToStreetTile(baseData.dfTerrain, pos, baseData.scale) &&
              baseData.steepness < Random.Range(30f,45f))
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
          int record2) {
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
          int maxNumber,
          bool firefliesExist)
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
                    fireflyContainer.GetComponent<WODistanceChecker>().CreateFirefly(baseData.dfTerrain, baseData.x, baseData.y, baseData.scale, baseData.terrain, distanceVariation); // Firefly
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
            var shootingStarInstance = GameObject.Instantiate(Resources.Load("ShootingStars") as GameObject, new Vector3(shootingStarPos.x, heightInTheSky, shootingStarPos.y), Quaternion.identity, dfBillboardBatch.transform);
            shootingStarInstance.transform.rotation = Quaternion.Euler(rotationAngleX, 0, 0);
            var sSps = shootingStarInstance.GetComponent<ParticleSystem>();
            var emissionModule = sSps.emission;
            emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(sSMin / 1000, sSMax / 1000);
        }

        static public bool IsOnAnyWaterTile(
          DaggerfallTerrain dfTerrain,
          Vector3 pos,
          float scale)
        {
            bool result = true;
            int roundedX = (int)Mathf.Round(pos.x / scale);
            int roundedY = (int)Mathf.Round(pos.z / scale);

            if (ExtensionMethods.In2DArrayBounds(dfTerrain.MapData.tilemapSamples, roundedX, roundedY))
            {
                int sampleGround = dfTerrain.MapData.tilemapSamples[roundedX, roundedY] & 0x3F;
                if (
                  sampleGround != 0 &&
                  sampleGround != 4 &&
                  sampleGround != 5 &&
                  sampleGround != 6 &&
                  sampleGround != 21 &&
                  sampleGround != 31 &&
                  sampleGround != 32)
                {
                    result = false;
                }
            }
            return result;
        }

        static public bool IsOnOrCloseToShallowWaterTile(
          DaggerfallTerrain dfTerrain,
          Vector3 pos,
          float scale)
        {
            bool result = true;
            bool stopCondition = false;

            float offsetA = 1f;
            float offsetB = 1f;

            int roundedX;
            int roundedY;

            for (int x = 0; x < 2 && stopCondition == false; x++)
            {
                for (int y = 0; y < 2 && stopCondition == false; y++)
                {

                    if (x == 1)
                        roundedX = (int)Mathf.Round((pos.x / scale) + offsetA);
                    else
                        roundedX = (int)Mathf.Round((pos.x / scale) + offsetB);
                    if (y == 1)
                        roundedY = (int)Mathf.Round((pos.z / scale) + offsetA);
                    else
                        roundedY = (int)Mathf.Round((pos.z / scale) + offsetB);

                    if (ExtensionMethods.In2DArrayBounds(dfTerrain.MapData.tilemapSamples, roundedX - x, roundedY - y))
                    {
                        int sampleGround = dfTerrain.MapData.tilemapSamples[roundedX - x, roundedY - y] & 0x3F;
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
                          sampleGround != 50)
                        {
                            stopCondition = result = false;
                        }
                        else
                            stopCondition = result = true;
                    }
                }
            }
            return result;
        }

        static public bool IsOnOrCloseToStreetTile(
          DaggerfallTerrain dfTerrain,
          Vector3 pos,
          float scale)
        {
            bool result = true;
            bool stopCondition = false;

            float offsetA = 0.7f;
            float offsetB = -0.3f;

            int roundedX;
            int roundedY;

            for (int x = 0; x < 2 && stopCondition == false; x++)
            {
                for (int y = 0; y < 2 && stopCondition == false; y++)
                {
                    if (x == 1)
                        roundedX = (int)Mathf.Round((pos.x / scale) + offsetA);
                    else
                        roundedX = (int)Mathf.Round((pos.x / scale) + offsetB);
                    if (y == 1)
                        roundedY = (int)Mathf.Round((pos.z / scale) + offsetA);
                    else
                        roundedY = (int)Mathf.Round((pos.z / scale) + offsetB);

                    if (ExtensionMethods.In2DArrayBounds(dfTerrain.MapData.tilemapSamples, roundedX - x, roundedY - y))
                    {
                        int sampleGround = dfTerrain.MapData.tilemapSamples[roundedX - x, roundedY - y] & 0x3F;
                        if (
                          sampleGround != 46 &&
                          sampleGround != 47 &&
                          sampleGround != 55)
                        {
                            stopCondition = result = false;
                        }
                        else
                            stopCondition = result = true;
                    }
                }
            }
            return result;
        }

        static public bool IsCollidingWithBuilding(
          DaggerfallTerrain dfTerrain,
          Vector3 pos,
          float scale)
        {
            /* if(dfTerrain.MapData.hasLocation)
            {
                foreach(GameObject go in staticGeometryList)
                {
                Vector3 newPos = new Vector3(pos.x,pos.y + 100, pos.z);
                RaycastHit hit;

                if (Physics.Raycast(dfTerrain.transform.TransformPoint(newPos), new Vector3(0,-1,0), out hit, Mathf.Infinity))
                {
                    Debug.DrawLine(dfTerrain.transform.TransformPoint(newPos), hit.point, Color.red,30);

                    if(hit.collider.gameObject.tag == "StaticGeometry")
                    {
                Debug.DrawLine(dfTerrain.transform.TransformPoint(newPos), new Vector3(dfTerrain.transform.TransformPoint(newPos).x,-200,dfTerrain.transform.TransformPoint(newPos).z), Color.yellow,30);
                return true;
                    }
                }
                }
            } */
            return false;
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

            return Mathf.Clamp(finalValue, -1, 1);
        }

        // Sets texture by range
        private string GetWeightedRecord(
          float weight,
          float limit1 = 0.3f,
          float limit2 = 0.6f)
        {
            if (weight < limit1)
                return "flower";
            else if (weight >= limit1 && weight < limit2)
                return "grass";
            else
                return "forest";
        }
    }
}


