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
        private static int randomSeed;
        private static bool dynamicNatureClearance;
        private static bool vegetationInLocations;
        private static bool firefliesExist;
        private static bool shootingStarsExist;
        private static bool InterestingErodedTerrainEnabled;
        private static float fireflyActivationDistanceScale = 1f;
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
        private const float maxSteepness = 50f; // 50 - Max steepness to still place nature billboards
        private const float slopeSinkRatio = 70f; // 70 - Sink flats slightly into ground as slope increases to prevent floaty trees.
        private float treeLine = WOTerrainTexturing.treeLine; // Tree border
        public bool NatureMeshUsed { get; protected set; } // Flag to signal use of meshes

        // Important data classes holding values and lists
        public WOVegetationList vegetationList;
        public WOVegetationChance vegetationChance;
        public WOStochasticChances stochastics;

        public WOTerrainNature(
           Mod woMod,
           bool DMEnabled,
           bool ITEnabled,
           int rngSeed,
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

            if (DMEnabled) { // Change a tree texture in desert if DREAM Sprites Mod enabled
                List<int> desertTrees = new List<int>(new int[] { 5, 13, 30 });
            } else {
                List<int> desertTrees = new List<int>(new int[] { 5, 13, 13 });
            }
            Debug.Log("Wilderness Overhaul: DREAM Sprites enabled: " + DMEnabled);
            InterestingErodedTerrainEnabled = ITEnabled;
            Debug.Log("Wilderness Overhaul: Interesting Eroded Terrain enabled: " + InterestingErodedTerrainEnabled);
            randomSeed = rngSeed;
            Random.seed = randomSeed;
            Debug.Log("Wilderness Overhaul: Random Seed: " + rngSeed);
            dynamicNatureClearance = dNClearance;
            Debug.Log("Wilderness Overhaul: Setting Dynamic Nature Clearance: " + dynamicNatureClearance);
            vegetationInLocations = vegInLoc;
            Debug.Log("Wilderness Overhaul: Setting Vegetation in Jungle Location: " + vegetationInLocations);
            generalNatureClearance = gNClearance;
            Debug.Log("Wilderness Overhaul: Setting General Nature Clearance: " + generalNatureClearance);
            firefliesExist = fireflies;
            Debug.Log("Wilderness Overhaul: Generate Fireflies at Night: " + firefliesExist);
            fireflyActivationDistanceScale = Mathf.Max(0.1f, fireflyActivationDistance / 800f);
            Debug.Log("Wilderness Overhaul: Firefly Activation Distance Scale: " + fireflyActivationDistanceScale);
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

            // Preparation of StaticGeometry for collision detection - Dysfunctional and therefore currently not implemented
            /* if(dfTerrain.MapData.hasLocation)
            {
                exterior = GameObject.Find("Exterior");
                staticGeometryList = new List<GameObject>();
                GameObject streamingTarget = GameObject.Find("StreamingTarget");
                AddDescendantsWithTag(streamingTarget.transform, "StaticGeometry", staticGeometryList);
            } */

            // Apply Mod Setting of Dynamic Nature Clearance Distances
            if (dynamicNatureClearance) {
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
            foreach (Transform child in dfBillboardBatch.transform) {
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
            if (InterestingErodedTerrainEnabled) {
                maxTerrainHeight = 4890;
            }

            // Chance scaled by base climate type
            DFLocation.ClimateSettings climate = MapsFile.GetWorldClimateSettings(dfTerrain.MapData.worldClimate);

            // Initialize stochastics & vegetation
            stochastics = new WOStochasticChances(randomSeed);
            vegetationChance = new WOVegetationChance(randomSeed);
            vegetationList = new WOVegetationList(randomSeed);

            // Adds one shooting star Particle System of every MapPixel
            if (shootingStarsExist)
                AddShootingStar(dfTerrain, dfBillboardBatch, 90f, 1200f, shootingStarsMinimum, shootingStarsMaximum); // Shooting Stars

            for (int y = 0; y < tDim; y++) {
                for (int x = 0; x < tDim; x++) {
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
                    if (vegetationInLocations) {
                        if (rect.x > 0 && rect.y > 0 && rect.Contains(tilePos) && dfTerrain.MapData.worldClimate != (int)MapsFile.Climates.Rainforest)
                            continue;
                    } else {
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

                    ContainerObject baseData = new ContainerObject(dfTerrain, dfBillboardBatch, terrain, scale, steepness, x, y, maxTerrainHeight);

                    switch (climate.WorldClimate) {
                        #region Temperate Spawns
                        case (int)MapsFile.Climates.Woodlands:

                            weight = GetNoise(latitude,
                                longitude,
                                stochastics.tempForestFrequency,
                                stochastics.tempForestAmplitude,
                                stochastics.tempForestPersistence,
                                stochastics.tempForestOctaves,
                                100);
                            weight = Mathf.Clamp(weight, 0f, 1f);// Stone

                            if (TryAddWoodlandMicroEncounter(baseData, tile, height, weight))
                                break;

                            if (tile == 1) { // Dirt
                                if (height > Random.Range(0.025f, 0.027f)) {
                                    if (GetWeightedRecord(weight) == "forest") {
                                        if (Random.Range(0, 100) < Random.Range(70, 80)) {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.15f, 0.30f), true); // Dead Trees

                                            for (int i = 0; i < Random.Range(0, 3); i++) {
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBeach, Random.Range(1.00f, 2.00f), true); // Beach
                                            }
                                        } else {
                                            if (Random.Range(0, 100) < stochastics.mapStyle) {
                                                for (int i = 0; i < Random.Range(1, 5); i++) {
                                                    if (Random.Range(0, 100) < 50 && height > Random.Range(0.1f, 0.15f))
                                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.75f, 1.50f), true, 3); // Needle Tree
                                                    else
                                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(0.75f, 1.50f), true); // Tree
                                                }
                                            } else {
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.15f, 0.30f), true); // Dead Trees

                                                for (int i = 0; i < Random.Range(0, 3); i++) {
                                                    AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBeach, Random.Range(1.00f, 2.00f), true); // Beach
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (tile == 2) { // Grass
                                if (Random.Range(0.0f, 100.0f) <= stochastics.temperateMushroomRingChance) // Mushroom Circle
                                    AddMushroomRingToBatch(baseData, 23);
                                else if (GetWeightedRecord(weight, stochastics.tempForestLimit[0], stochastics.tempForestLimit[1]) == "flower") {
                                    if (Random.Range(0, 100) < 5) {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandMushroom, 0.00f, true); // Mushroom
                                    }

                                    for (int i = 0; i < Random.Range(4, 7); i++) {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandFlowers, Random.Range(0.45f, 0.75f), true); // Flowers
                                    }

                                    float rnd = Random.Range(0, 100);
                                    if (rnd < stochastics.mapStyleChance[1]) {
                                        for (int i = 0; i < Random.Range(2, 4); i++) {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandFlowers, Random.Range(0.60f, 1.00f), true); // Flowers
                                        }
                                    }
                                    if (rnd < 5) {
                                        for (int i = 0; i < Random.Range(0, 3); i++) {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(0.85f, 1.15f), true); // Bushes
                                        }
                                    }
                                }
                                else if (GetWeightedRecord(weight, stochastics.tempForestLimit[0], stochastics.tempForestLimit[1]) == "grass") {
                                    float rnd = Random.Range(0, 100);
                                    if (rnd < 4) {
                                        for (int i = 0; i < Random.Range(9, 14); i++) {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(2.25f, 2.75f), true); // Bushes
                                        }

                                        for (int i = 0; i < Random.Range(3, 5); i++) {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(0.50f, 0.75f), true); // Bushes
                                        }
                                    }

                                    if (rnd < 3) {
                                        for (int i = 0; i < Random.Range(3, 8); i++) {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(1.00f, 2.25f), true); // Trees
                                        }
                                    }
                                }

                                else if (weight >= stochastics.tempForestLimit[1] - 0.01f && weight < stochastics.tempForestLimit[1]) {
                                    for (int i = 0; i < Random.Range(5, 15); i++) {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(0.25f, 1.50f), true); // Bushes
                                    }
                                }
								else if (weight <= stochastics.tempForestLimit[1] + 0.01f && weight >= stochastics.tempForestLimit[1]) {
                                    for (int i = 0; i < Random.Range(10, 20); i++) {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(0.5f, 2.00f), true); // Bushes
                                    }
                                }
								else if (weight <= stochastics.tempForestLimit[1] + 0.02f && weight > stochastics.tempForestLimit[1] + 0.01f) {
                                    for (int i = 0; i < Random.Range(5, 15); i++) {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(0.25f, 1.50f), true); // Bushes
                                    }
                                }

                                else if (GetWeightedRecord(weight, stochastics.tempForestLimit[0], stochastics.tempForestLimit[1]) == "forest") {
                                    float rnd = Random.Range(0, 100);
                                    for (int i = 0; i < Random.Range(3, 4); i++) {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(1.00f, 2.25f), true); // Trees
                                    }

                                    AddFirefly(baseData, 0.1f, 5, 15, 35, CreateWoodlandFireflyProfile()); // Fireflies

                                    if (rnd < stochastics.mapStyleChance[3]) {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBeach, Random.Range(2.00f, 3.00f), true); // Beach

                                        for (int i = 0; i < Random.Range(0, 2); i++) {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandTrees, Random.Range(2.00f, 3.00f), true); // Trees
                                        }

                                        for (int i = 0; i < Random.Range(0, 2); i++) {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(2.00f, 3.00f), true); // Bushes
                                        }

                                        AddFirefly(baseData, 0.05f, 10, 50, 100, CreateWoodlandFireflyProfile(true)); // Fireflies
                                    }

                                    if (rnd < stochastics.mapStyleChance[1]) {
                                        for (int i = 0; i < Random.Range(0, 1); i++) {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBushes, Random.Range(2.00f, 3.00f), true); // Bushes
                                        }

                                        AddFirefly(baseData, 0.025f, 15, 100, 250, CreateWoodlandFireflyProfile(true)); // Fireflies
                                    }
                                }
                            }
                            else if (tile == 3) { // Stone
                                if (GetWeightedRecord(weight) == "forest") {
                                    for (int i = 0; i < Random.Range(0, 3); i++) {
                                        AddBillboardToBatch(baseData, vegetationList.temperateWoodlandRocks, Random.Range(0.25f, 1.00f), true); // Stones
                                    }

                                    if (height > 0.15f && Random.Range(0.0f, 100.0f) < 5) {
                                        if (Random.Range(stochastics.mapStyleChance[3], stochastics.mapStyleChance[stochastics.mapStyleChance.Length - 1]) < stochastics.mapStyle) {
                                            AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.75f, 1.15f), true); // Dead Tree
                                        }
                                        else {
                                            for (int i = 0; i < Random.Range(0, 3); i++) {
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, Random.Range(0.75f, 1.50f), true, 3); // Needle Tree
                                            }
                                        }
                                    }
                                }
                                if (GetWeightedRecord(weight) == "flower") {
                                    for (int i = 0; i < Random.Range(0, 2); i++) {
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

                            if (TryAddMountainMicroEncounter(baseData, tile, height, weight))
                                break;

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
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBeach, Random.Range(1.00f, 2.00f), true); // Beach
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
                                                    AddBillboardToBatch(baseData, vegetationList.temperateWoodlandBeach, Random.Range(1.00f, 2.00f), true); // Beach
                                            }
                                        }
                                    }
                                }

                                if (height < stochastics.mountForestLimit[0])
                                {
                                    if (Random.Range(0, 100) < 20)
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
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 4)); i++)
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

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 1)); i++)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsDeadTrees, 1.00f, true); // Beach                                    
                                }
                            }
                            else if (tile == 2) // Grass
                            {
                                float rndMajor = Random.Range(0.0f, 100.0f);

                                if (Random.Range(0.0f, 100.0f) < stochastics.mountainStoneCircleChance)
                                    AddStoneCircleToBatch(baseData, vegetationList.mountainsRocks, 5, 0);

                                if (height < stochastics.mountForestLimit[0])
                                {
                                    if (rndMajor >= 60)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsTrees, 0.50f, true); // Trees
                                    else if (rndMajor <= 25)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 0.50f, true); // Trees

                                    if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "forest")
                                    {
                                        float rndMinor = Random.Range(0, 100);
                                        if (rndMinor < stochastics.mapStyle)
                                        {
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 3)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsTrees, 0.50f, true); // Trees

                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 4)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 1.50f, true); // Grass

                                            if (Random.Range(0, 100) < 40)
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 4)); i++)
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.50f, true); // Flowers

                                            if (Random.Range(0, 100) < 25)
                                                for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 6)); i++)
                                                    AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.50f, true); // Flowers

                                            if (Random.Range(0, 100) < 25)
                                                if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.90f)
                                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 3)); i++)
                                                        AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 1.00f, true); // Rocks

                                            if (Random.Range(0, 100) < 20)
                                                AddBillboardToBatch(baseData, vegetationList.mountainsDeadTrees, 0.50f, true);

                                            if (Random.Range(0, 100) < 10)
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandMushroom, 0.25f, true);

                                            AddFirefly(baseData, 0.02f, 6, 6, 14, CreateMountainFireflyProfile());
                                        }
                                    }
                                    else if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "flower")
                                    {
                                        if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.90f)
                                            AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.00f, true); // Rock

                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 6)); i++)
                                            AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.50f, true); // Flowers

                                        float rndMinor = Random.Range(0, 100);
                                        if (rndMinor < 40)
                                            for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 3)); i++)
                                                AddBillboardToBatch(baseData, vegetationList.temperateWoodlandMushroom, 0.50f, true); // Flowers
                                    }
                                }
                                else if (height < stochastics.mountForestLimit[1])
                                {
                                    float temp = 75 - (height - stochastics.mountForestLimit[0]) * 750;
                                    if (Random.Range(0, 100) < (temp + (rndMajor / 10)))
                                        AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 0.75f, true);
                                    else if (Random.Range(0, 100) > 95)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 0.50f, true);

                                    if (Random.Range(0, 100) < 20)
                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 4)); i++)
                                            AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 1.50f, true); // Grass

                                    if (Random.Range(0, 100) < 15)
                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 6)); i++)
                                            AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 1.510f, true); // Flowers

                                    if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.975f)
                                        for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 5)); i++)
                                            AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 1.60f, true); // Rocks

                                    if (Random.Range(0, 100) < 20)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsDeadTrees, 0.5f, true);
                                }
                            }

                            else if (tile == 3) // Stone
                            {
                                if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "forest")
                                {
                                    AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 0.10f, true); // Stones

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(4, 6)); i++)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsRocks, 2.00f, true); // Stones

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(1, 4)); i++)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.50f, true); // Flowers

                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(0, 3)); i++)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsGrass, 1.0f, true); // Flowers                                  
                                }
                                if (GetWeightedRecord(weight, stochastics.mountForestLimit[0], stochastics.mountForestLimit[1]) == "flower")
                                {
                                    for (int i = 0; i < (int)Mathf.Round(Random.Range(2, 4)); i++)
                                        AddBillboardToBatch(baseData, vegetationList.mountainsFlowers, 0.75f, true); // Flowers
                                }

                                if ((int)Mathf.Round(Random.Range(0.00f, 1.00f)) > 0.9975f)
                                    AddBillboardToBatch(baseData, vegetationList.mountainsNeedleTrees, 1.00f, true); // Rocks
                            }
                            break;

                        #endregion

                        #region Desert1 Spawns

                        case (int)MapsFile.Climates.Desert: //STEPPE
                            LayoutDesertNature(baseData, latitude, longitude, tile, height, false);
                            break;

                        #endregion

                        #region Desert2 Spawns

                        case (int)MapsFile.Climates.Desert2:
                            LayoutDesertNature(baseData, latitude, longitude, tile, height, true);
                            break;

                        #endregion

                        #region Haunted Woodlands Spawns

                        case (int)MapsFile.Climates.HauntedWoodlands:
                            LayoutHauntedWoodlandNature(baseData, latitude, longitude, tile);
                            break;

                        #endregion

                        #region Woodland Hills Spawns

                        case (int)MapsFile.Climates.MountainWoods:
                            LayoutMountainWoodsNature(baseData, latitude, longitude, tile, height);
                            break;
                        #endregion

                        #region Rainforest Spawns

                        case (int)MapsFile.Climates.Rainforest:
                            LayoutRainforestNature(baseData, latitude, longitude, tile, height);
                            break;

                        #endregion

                        #region Subtropical Spawns
                        case (int)MapsFile.Climates.Subtropical:
                            LayoutSubtropicalNature(baseData, latitude, longitude, tile, height);
                            break;
                        #endregion

                        #region Swamp Spawns
                        case (int)MapsFile.Climates.Swamp:
                            LayoutSwampNature(baseData, latitude, longitude, tile, height);
                            break;
                            #endregion
                    }
                }
            }

            // Apply new batch
            dfBillboardBatch.Apply();
        }

        private bool Roll(float chance)
        {
            return Random.Range(0f, 100f) < chance;
        }

        private void AddCluster(
         ContainerObject baseData,
         List<int> billboardCollection,
         int minCount,
         int maxCount,
         float minVariance,
         float maxVariance,
         bool checkOnLand)
        {
            if (billboardCollection == null || billboardCollection.Count == 0 || maxCount <= minCount)
                return;

            int count = Random.Range(minCount, maxCount);
            for (int i = 0; i < count; i++)
                AddBillboardToBatch(baseData, billboardCollection, Random.Range(minVariance, maxVariance), checkOnLand);
        }

        private void LayoutDesertNature(
         ContainerObject baseData,
         int latitude,
         int longitude,
         int tile,
         float height,
         bool arid)
        {
            float weight = GetNoise(
                latitude,
                longitude,
                arid ? 0.022f : 0.016f,
                arid ? 0.70f : 0.55f,
                arid ? 0.45f : 0.50f,
                3,
                arid ? 240 : 220);
            weight = Mathf.Clamp(weight, 0f, 1f);
            string patch = GetWeightedRecord(weight, arid ? 0.20f : 0.30f, arid ? 0.58f : 0.54f);
            int oasisDistance = GetNearestTileDistance(baseData, 0, arid ? 5 : 6);
            bool oasisCore = oasisDistance >= 0 && oasisDistance <= 2;
            bool oasisOuter = oasisDistance > 2;

            if (TryDesertMicroEncounter(baseData, tile, arid, oasisCore, oasisOuter, patch))
                return;

            if (tile == 1)
            {
                if (oasisCore)
                {
                    AddCluster(baseData, vegetationList.desertWaterPlants, arid ? 2 : 3, arid ? 5 : 6, 0.35f, 1.25f, true);
                    AddCluster(baseData, vegetationList.desertWaterFlowers, 2, 5, 0.25f, 0.90f, true);
                    if (Roll(arid ? 12f : 28f))
                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(0.35f, 0.90f), true);
                }
                else if (oasisOuter)
                {
                    AddCluster(baseData, vegetationList.desertWaterPlants, 1, 4, 0.30f, 1.00f, true);
                    AddCluster(baseData, vegetationList.desertPlants, 1, 3, 0.25f, 0.80f, true);
                    if (Roll(arid ? 8f : 18f))
                        AddCluster(baseData, vegetationList.desertWaterFlowers, 1, 3, 0.25f, 0.75f, true);
                    if (!arid && Roll(10f))
                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(0.35f, 0.80f), true);
                }
                else if (patch == "forest")
                {
                    AddCluster(baseData, vegetationList.desertCactus, arid ? 2 : 1, arid ? 5 : 4, 0.45f, 1.40f, true);
                    if (Roll(arid ? 35f : 20f))
                        AddCluster(baseData, vegetationList.desertDeadTrees, 1, 3, 0.45f, 1.10f, true);
                    if (!arid && Roll(30f))
                        AddCluster(baseData, vegetationList.desertPlants, 1, 3, 0.25f, 0.80f, true);
                }
                else if (patch == "grass")
                {
                    AddCluster(baseData, vegetationList.desertPlants, 2, 5, 0.30f, 1.00f, true);
                    if (Roll(30f))
                        AddCluster(baseData, vegetationList.desertFlowers, 1, 4, 0.25f, 0.85f, true);
                }
                else
                {
                    AddCluster(baseData, vegetationList.desertStones, 1, 4, 0.25f, 1.00f, true);
                }
            }
            else if (tile == 2)
            {
                if (oasisCore)
                {
                    AddCluster(baseData, vegetationList.desertWaterPlants, arid ? 4 : 5, arid ? 7 : 9, 0.35f, 1.50f, true);
                    AddCluster(baseData, vegetationList.desertWaterFlowers, arid ? 2 : 3, arid ? 5 : 7, 0.30f, 1.00f, true);
                    if (Roll(arid ? 18f : 40f))
                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(0.50f, 1.25f), true);
                    if (Roll(arid ? 20f : 30f))
                        AddCluster(baseData, vegetationList.desertCactus, 1, 3, 0.45f, 1.00f, true);
                    AddFirefly(baseData, arid ? 0.03f : 0.08f, 6, arid ? 6 : 10, arid ? 14 : 24, CreateDesertOasisFireflyProfile());
                }
                else if (oasisOuter)
                {
                    AddCluster(baseData, vegetationList.desertWaterPlants, 2, 5, 0.35f, 1.20f, true);
                    AddCluster(baseData, vegetationList.desertPlants, 2, 5, 0.35f, 1.05f, true);
                    if (Roll(arid ? 15f : 22f))
                        AddCluster(baseData, vegetationList.desertWaterFlowers, 1, 4, 0.30f, 0.90f, true);
                    if (!arid && Roll(18f))
                        AddBillboardToBatch(baseData, vegetationList.desertTrees, Random.Range(0.45f, 1.05f), true);
                    if (!arid)
                        AddFirefly(baseData, 0.03f, 7, 6, 16, CreateDesertOasisFireflyProfile());
                }
                else if (patch == "flower")
                {
                    AddCluster(baseData, vegetationList.desertFlowers, 4, 8, 0.35f, 1.00f, true);
                    AddCluster(baseData, vegetationList.desertPlants, 1, 4, 0.35f, 1.00f, true);
                }
                else if (patch == "grass")
                {
                    AddCluster(baseData, vegetationList.desertPlants, 3, 7, 0.45f, 1.35f, true);
                    if (Roll(arid ? 35f : 20f))
                        AddCluster(baseData, vegetationList.desertCactus, 1, 3, 0.45f, 1.25f, true);
                }
                else
                {
                    AddCluster(baseData, arid ? vegetationList.desertCactus : vegetationList.desertTrees, 2, 5, 0.75f, 1.90f, true);
                    if (Roll(arid ? 40f : 25f))
                        AddCluster(baseData, vegetationList.desertDeadTrees, 1, 3, 0.50f, 1.30f, true);
                }
            }
            else if (tile == 3)
            {
                AddCluster(baseData, vegetationList.desertStones, 1, 4, 0.20f, 1.10f, true);

                if (oasisCore && Roll(arid ? 10f : 18f))
                    AddCluster(baseData, vegetationList.desertWaterPlants, 1, 3, 0.25f, 0.80f, true);

                if (oasisOuter && Roll(arid ? 8f : 15f))
                    AddCluster(baseData, vegetationList.desertPlants, 1, 3, 0.25f, 0.80f, true);

                if (!oasisCore && !oasisOuter && patch == "forest" && Roll(arid ? 25f : 15f))
                    AddBillboardToBatch(baseData, arid ? vegetationList.desertCactus : vegetationList.desertDeadTrees, Random.Range(0.45f, 1.00f), true);
            }
        }

        private void LayoutHauntedWoodlandNature(
         ContainerObject baseData,
         int latitude,
         int longitude,
         int tile)
        {
            float weight = GetNoise(latitude, longitude, 0.018f, 0.85f, 0.45f, 3, 320);
            weight = Mathf.Clamp(weight, 0f, 1f);
            string patch = GetWeightedRecord(weight, 0.35f, 0.62f);

            if (TryHauntedMicroEncounter(baseData, tile))
                return;

            if (tile == 1)
            {
                if (patch == "forest")
                {
                    AddCluster(baseData, vegetationList.hauntedWoodlandDirtTrees, 1, 3, 0.35f, 1.00f, true);
                    AddCluster(baseData, vegetationList.hauntedWoodlandDeadTrees, 1, 3, 0.50f, 1.30f, true);
                    if (Roll(25f))
                        AddCluster(baseData, vegetationList.hauntedWoodlandBeach, 1, 3, 0.40f, 1.15f, true);
                }
                else if (patch == "grass")
                {
                    AddCluster(baseData, vegetationList.hauntedWoodlandPlants, 2, 5, 0.30f, 0.90f, true);
                    if (Roll(25f))
                        AddCluster(baseData, vegetationList.hauntedWoodlandBushes, 1, 3, 0.45f, 1.10f, true);
                }
                else
                {
                    AddCluster(baseData, vegetationList.hauntedWoodlandBones, 1, 3, 0.15f, 0.65f, true);
                    AddCluster(baseData, vegetationList.hauntedWoodlandRocks, 1, 3, 0.30f, 0.90f, true);
                }
            }
            else if (tile == 2)
            {
                if (patch == "flower")
                {
                    if (Roll(2f))
                        AddMushroomRingToBatch(baseData, 23);

                    AddCluster(baseData, vegetationList.hauntedWoodlandFlowers, 3, 6, 0.30f, 0.85f, true);
                    AddCluster(baseData, vegetationList.hauntedWoodlandMushroom, 1, 3, 0.20f, 0.65f, true);
                    if (Roll(20f))
                        AddCluster(baseData, vegetationList.hauntedWoodlandBones, 1, 2, 0.15f, 0.60f, true);
                    AddFirefly(baseData, 0.03f, 5, 6, 14, CreateHauntedFireflyProfile());
                }
                else if (patch == "grass")
                {
                    AddCluster(baseData, vegetationList.hauntedWoodlandBushes, 2, 5, 0.45f, 1.25f, true);
                    AddCluster(baseData, vegetationList.hauntedWoodlandPlants, 2, 5, 0.30f, 1.00f, true);
                    if (Roll(15f))
                        AddCluster(baseData, vegetationList.hauntedWoodlandBones, 1, 3, 0.15f, 0.75f, true);
                }
                else
                {
                    AddCluster(baseData, vegetationList.hauntedWoodlandTrees, 2, 4, 0.75f, 1.70f, true);
                    AddCluster(baseData, vegetationList.hauntedWoodlandDeadTrees, 1, 3, 0.50f, 1.20f, true);
                    if (Roll(20f))
                        AddCluster(baseData, vegetationList.hauntedWoodlandBushes, 1, 3, 0.50f, 1.20f, true);
                    if (Roll(10f))
                        AddCluster(baseData, vegetationList.hauntedWoodlandMushroom, 1, 3, 0.25f, 0.80f, true);
                }
            }
            else if (tile == 3)
            {
                AddCluster(baseData, vegetationList.hauntedWoodlandRocks, 1, 4, 0.20f, 1.00f, true);
                if (patch != "flower" && Roll(25f))
                    AddCluster(baseData, vegetationList.hauntedWoodlandDeadTrees, 1, 2, 0.40f, 0.90f, true);
                if (Roll(15f))
                    AddCluster(baseData, vegetationList.hauntedWoodlandBones, 1, 3, 0.15f, 0.70f, true);
            }
        }

        private void LayoutMountainWoodsNature(
         ContainerObject baseData,
         int latitude,
         int longitude,
         int tile,
         float height)
        {
            float weight = GetNoise(latitude, longitude, 0.016f, 0.85f, 0.45f, 3, 360);
            weight = Mathf.Clamp(weight, 0f, 1f);
            string patch = GetWeightedRecord(weight, 0.38f, 0.68f);

            if (TryMountainWoodsMicroEncounter(baseData, tile))
                return;

            if (tile == 1)
            {
                if (patch == "forest")
                {
                    if (Roll(height > 0.30f ? 60f : 40f))
                        AddCluster(baseData, vegetationList.woodlandHillsNeedleTrees, 2, 5, 0.50f, 1.50f, true);
                    else
                        AddCluster(baseData, vegetationList.woodlandHillsTrees, 1, 4, 0.50f, 1.40f, true);

                    if (Roll(20f))
                        AddCluster(baseData, vegetationList.woodlandHillsDeadTrees, 1, 3, 0.40f, 1.00f, true);
                }
                else if (patch == "grass")
                {
                    AddCluster(baseData, vegetationList.woodlandHillsDirtPlants, 2, 5, 0.30f, 0.95f, true);
                    if (Roll(20f))
                        AddCluster(baseData, vegetationList.woodlandHillsBeach, 1, 3, 0.30f, 0.90f, true);
                }
                else
                {
                    AddCluster(baseData, vegetationList.woodlandHillsDeadTrees, 1, 2, 0.35f, 0.90f, true);
                    AddCluster(baseData, vegetationList.woodlandHillsBeach, 1, 3, 0.25f, 0.80f, true);
                }
            }
            else if (tile == 2)
            {
                if (patch == "flower")
                {
                    AddCluster(baseData, vegetationList.woodlandHillsFlowers, 4, 7, 0.35f, 0.85f, true);
                    if (Roll(20f))
                        AddCluster(baseData, vegetationList.woodlandHillsBushes, 1, 3, 0.40f, 1.00f, true);
                }
                else if (patch == "grass")
                {
                    AddCluster(baseData, vegetationList.woodlandHillsBushes, 3, 6, 0.50f, 1.25f, true);
                    AddCluster(baseData, vegetationList.woodlandHillsDirtPlants, 1, 3, 0.30f, 0.90f, true);
                    if (Roll(10f))
                        AddBillboardToBatch(baseData, vegetationList.woodlandHillsTrees, Random.Range(0.65f, 1.20f), true);
                }
                else
                {
                    AddCluster(baseData, vegetationList.woodlandHillsTrees, 2, 5, 0.75f, 1.75f, true);
                    if (Roll(height > 0.28f ? 45f : 25f))
                        AddCluster(baseData, vegetationList.woodlandHillsNeedleTrees, 1, 3, 0.55f, 1.30f, true);
                    if (Roll(15f))
                        AddFirefly(baseData, 0.05f, 8, 20, 45, CreateMountainWoodsFireflyProfile());
                }
            }
            else if (tile == 3)
            {
                AddCluster(baseData, vegetationList.woodlandHillsRocks, 1, 4, 0.20f, 1.00f, true);
                if (patch == "forest" && Roll(20f))
                    AddCluster(baseData, vegetationList.woodlandHillsNeedleTrees, 1, 3, 0.40f, 1.00f, true);
                if (Roll(15f))
                    AddCluster(baseData, vegetationList.woodlandHillsDeadTrees, 1, 2, 0.35f, 0.85f, true);
            }
        }

        private void LayoutRainforestNature(
         ContainerObject baseData,
         int latitude,
         int longitude,
         int tile,
         float height)
        {
            float weight = GetNoise(latitude, longitude, 0.022f, 1.0f, 0.55f, 3, 400);
            weight = Mathf.Clamp(weight, 0f, 1f);
            string patch = GetWeightedRecord(weight, 0.22f, 0.50f);
            bool lowland = height < 0.08f;

            if (TryRainforestMicroEncounter(baseData, tile, lowland))
                return;

            if (tile == 1)
            {
                if (patch == "forest")
                {
                    AddCluster(baseData, vegetationList.rainforestTrees, 2, 5, 0.60f, 1.45f, true);
                    AddCluster(baseData, vegetationList.rainforestPlants, 2, 5, 0.35f, 1.10f, true);
                    AddCluster(baseData, vegetationList.rainforestBushes, 1, 3, 0.45f, 1.05f, true);
                }
                else if (patch == "grass")
                {
                    AddCluster(baseData, vegetationList.rainforestPlants, 3, 7, 0.35f, 1.15f, true);
                    if (Roll(20f))
                        AddCluster(baseData, vegetationList.rainforestFlowers, 1, 4, 0.30f, 0.80f, true);
                }
                else
                {
                    AddCluster(baseData, vegetationList.rainforestFlowers, 2, 5, 0.25f, 0.75f, true);
                    if (lowland)
                        AddCluster(baseData, vegetationList.rainforestEggs, 1, 3, 0.20f, 0.60f, true);
                }

                if (lowland && Roll(20f))
                    AddCluster(baseData, vegetationList.rainforestBushes, 1, 3, 0.40f, 0.95f, true);
            }
            else if (tile == 2)
            {
                if (patch == "flower")
                {
                    AddCluster(baseData, vegetationList.rainforestFlowers, 4, 8, 0.30f, 0.90f, true);
                    AddCluster(baseData, vegetationList.rainforestPlants, 2, 5, 0.35f, 1.00f, true);
                    if (Roll(20f))
                        AddCluster(baseData, vegetationList.rainforestEggs, 1, 3, 0.25f, 0.70f, true);
                }
                else if (patch == "grass")
                {
                    AddCluster(baseData, vegetationList.rainforestBushes, 3, 7, 0.45f, 1.20f, true);
                    AddCluster(baseData, vegetationList.rainforestPlants, 3, 7, 0.35f, 1.10f, true);
                    if (Roll(25f))
                        AddCluster(baseData, vegetationList.rainforestTrees, 1, 3, 0.60f, 1.20f, true);
                }
                else
                {
                    AddCluster(baseData, vegetationList.rainforestTrees, 3, 6, 0.75f, 1.80f, true);
                    AddCluster(baseData, vegetationList.rainforestPlants, 3, 7, 0.40f, 1.20f, true);
                    AddCluster(baseData, vegetationList.rainforestBushes, 2, 5, 0.45f, 1.20f, true);
                    if (lowland || Roll(20f))
                        AddFirefly(baseData, 0.08f, 8, 25, 60, CreateRainforestFireflyProfile());
                    if (Roll(20f))
                        AddCluster(baseData, vegetationList.rainforestEggs, 1, 3, 0.25f, 0.70f, true);
                }
            }
            else if (tile == 3)
            {
                AddCluster(baseData, vegetationList.rainforestRocks, 1, 3, 0.20f, 0.90f, true);
                if (patch != "flower")
                    AddCluster(baseData, vegetationList.rainforestPlants, 1, 4, 0.25f, 0.90f, true);
                if (patch == "forest" && Roll(15f))
                    AddCluster(baseData, vegetationList.rainforestTrees, 1, 2, 0.45f, 1.00f, true);
            }
        }

        private void LayoutSubtropicalNature(
         ContainerObject baseData,
         int latitude,
         int longitude,
         int tile,
         float height)
        {
            float canopyWeight = GetNoise(latitude, longitude, 0.018f, 0.80f, 0.48f, 3, 450);
            float moistureWeight = GetNoise(latitude, longitude, 0.010f, 0.95f, 0.55f, 3, 470);
            canopyWeight = Mathf.Clamp(canopyWeight, 0f, 1f);
            moistureWeight = Mathf.Clamp(moistureWeight, 0f, 1f);

            bool dryHighland = height > 0.18f;
            bool upperSlope = height > 0.24f;
            bool lushLowland = height < 0.08f;
            bool riparianPocket = moistureWeight > (lushLowland ? 0.48f : 0.68f);
            bool denseStyle = stochastics.mapStyle >= stochastics.mapStyleChance[3];

            string patch = GetWeightedRecord(
                canopyWeight,
                dryHighland ? 0.40f : 0.30f,
                dryHighland ? 0.70f : 0.58f);

            if (TrySubtropicalMicroEncounter(baseData, tile, riparianPocket, dryHighland))
                return;

            if (tile == 1)
            {
                if (riparianPocket)
                {
                    AddCluster(baseData, vegetationList.subtropicalPlants, 2, 5, 0.30f, 0.95f, true);
                    AddCluster(baseData, vegetationList.subtropicalBushes, 1, 3, 0.25f, 0.85f, true);
                    if (!upperSlope && Roll(denseStyle ? 35f : 20f))
                        AddCluster(baseData, vegetationList.subtropicalTrees, 1, 3, 0.45f, 1.00f, true);
                    if (Roll(15f))
                        AddCluster(baseData, vegetationList.subtropicalFlowers, 1, 3, 0.25f, 0.75f, true);
                }
                else if (patch == "forest")
                {
                    AddCluster(baseData, vegetationList.subtropicalTrees, 1, dryHighland ? 3 : 4, 0.50f, 1.30f, true);
                    if (dryHighland)
                    {
                        AddCluster(baseData, vegetationList.subtropicalSucculents, 1, 3, 0.35f, 0.95f, true);
                        if (Roll(18f))
                            AddCluster(baseData, vegetationList.subtropicalDeadTrees, 1, 2, 0.30f, 0.85f, true);
                    }
                    else
                    {
                        AddCluster(baseData, vegetationList.subtropicalBushes, 1, 3, 0.30f, 0.85f, true);
                        if (Roll(20f))
                            AddCluster(baseData, vegetationList.subtropicalPlants, 1, 3, 0.25f, 0.80f, true);
                    }
                }
                else if (patch == "grass")
                {
                    if (dryHighland)
                    {
                        AddCluster(baseData, vegetationList.subtropicalSucculents, 1, 3, 0.30f, 0.90f, true);
                        AddCluster(baseData, vegetationList.subtropicalRocks, 1, 3, 0.25f, 0.85f, true);
                    }
                    else
                    {
                        AddCluster(baseData, vegetationList.subtropicalPlants, 2, 5, 0.30f, 0.95f, true);
                        AddCluster(baseData, vegetationList.subtropicalBushes, 1, 3, 0.25f, 0.85f, true);
                    }
                }
                else
                {
                    AddCluster(baseData, vegetationList.subtropicalRocks, 1, 3, 0.25f, 0.85f, true);
                    AddCluster(baseData, vegetationList.subtropicalSucculents, 1, dryHighland ? 3 : 2, 0.30f, 0.80f, true);
                    if (!dryHighland && Roll(12f))
                        AddCluster(baseData, vegetationList.subtropicalBushes, 1, 2, 0.25f, 0.70f, true);
                }
            }
            else if (tile == 2)
            {
                if (riparianPocket)
                {
                    AddCluster(baseData, vegetationList.subtropicalTrees, denseStyle ? 2 : 1, denseStyle ? 5 : 4, 0.65f, 1.45f, true);
                    AddCluster(baseData, vegetationList.subtropicalBushes, 2, 5, 0.40f, 1.05f, true);
                    AddCluster(baseData, vegetationList.subtropicalPlants, 2, 5, 0.30f, 0.95f, true);
                    if (Roll(30f))
                        AddCluster(baseData, vegetationList.subtropicalFlowers, 2, 5, 0.25f, 0.75f, true);
                    if (lushLowland && Roll(10f))
                        AddFirefly(baseData, 0.05f, 8, 12, 25, CreateSubtropicalFireflyProfile());
                }
                else if (patch == "flower")
                {
                    AddCluster(baseData, vegetationList.subtropicalFlowers, 3, 7, 0.30f, 0.85f, true);
                    AddCluster(baseData, vegetationList.subtropicalPlants, 1, 4, 0.25f, 0.75f, true);
                    if (!dryHighland && Roll(20f))
                        AddCluster(baseData, vegetationList.subtropicalBushes, 1, 3, 0.40f, 0.95f, true);
                }
                else if (patch == "grass")
                {
                    if (dryHighland)
                    {
                        AddCluster(baseData, vegetationList.subtropicalSucculents, 2, 4, 0.40f, 1.00f, true);
                        AddCluster(baseData, vegetationList.subtropicalBushes, 1, 3, 0.35f, 0.95f, true);
                        if (Roll(12f))
                            AddCluster(baseData, vegetationList.subtropicalRocks, 1, 3, 0.25f, 0.80f, true);
                    }
                    else
                    {
                        AddCluster(baseData, vegetationList.subtropicalBushes, 2, denseStyle ? 6 : 5, 0.45f, 1.10f, true);
                        AddCluster(baseData, vegetationList.subtropicalPlants, 2, 5, 0.35f, 1.00f, true);
                    }
                    if (Roll(dryHighland ? 25f : 12f))
                        AddCluster(baseData, vegetationList.subtropicalSucculents, 1, 3, 0.45f, 1.10f, true);
                }
                else
                {
                    if (dryHighland)
                    {
                        AddCluster(baseData, vegetationList.subtropicalTrees, 1, 3, 0.65f, 1.40f, true);
                        AddCluster(baseData, vegetationList.subtropicalSucculents, 1, 3, 0.45f, 1.05f, true);
                    }
                    else
                    {
                        AddCluster(baseData, vegetationList.subtropicalTrees, denseStyle ? 3 : 2, denseStyle ? 5 : 4, 0.70f, 1.65f, true);
                        AddCluster(baseData, vegetationList.subtropicalBushes, 1, 3, 0.40f, 1.00f, true);
                        if (Roll(18f))
                            AddCluster(baseData, vegetationList.subtropicalFlowers, 1, 3, 0.25f, 0.75f, true);
                    }
                    if (Roll(12f))
                        AddCluster(baseData, vegetationList.subtropicalBushes, 1, 3, 0.40f, 1.00f, true);
                }
            }
            else if (tile == 3)
            {
                AddCluster(baseData, vegetationList.subtropicalRocks, dryHighland ? 3 : 2, dryHighland ? 6 : 5, 0.20f, 1.00f, true);

                if (dryHighland)
                {
                    AddCluster(baseData, vegetationList.subtropicalSucculents, 1, 4, 0.25f, 0.85f, true);
                    if (Roll(18f))
                        AddCluster(baseData, vegetationList.subtropicalDeadTrees, 1, 2, 0.30f, 0.80f, true);
                }
                else if (riparianPocket)
                {
                    AddCluster(baseData, vegetationList.subtropicalPlants, 1, 3, 0.25f, 0.80f, true);
                    if (Roll(15f))
                        AddCluster(baseData, vegetationList.subtropicalBushes, 1, 2, 0.25f, 0.75f, true);
                }
                else
                {
                    if (patch == "forest" && Roll(20f))
                        AddCluster(baseData, vegetationList.subtropicalTrees, 1, 2, 0.35f, 0.90f, true);
                    if (patch == "flower" && Roll(18f))
                        AddCluster(baseData, vegetationList.subtropicalFlowers, 1, 3, 0.20f, 0.60f, true);
                    if (Roll(15f))
                        AddCluster(baseData, vegetationList.subtropicalDeadTrees, 1, 2, 0.30f, 0.80f, true);
                }
            }
        }

        private void LayoutSwampNature(
         ContainerObject baseData,
         int latitude,
         int longitude,
         int tile,
         float height)
        {
            float weight = GetNoise(latitude, longitude, 0.024f, 0.90f, 0.55f, 3, 500);
            weight = Mathf.Clamp(weight, 0f, 1f);
            string patch = GetWeightedRecord(weight, 0.30f, 0.58f);
            bool wetLowland = height < 0.08f;

            if (TrySwampMicroEncounter(baseData, tile, wetLowland))
                return;

            if (tile == 1)
            {
                if (wetLowland)
                {
                    AddCluster(baseData, vegetationList.swampWaterPlants, 3, 6, 0.30f, 1.00f, true);
                    AddCluster(baseData, vegetationList.swampWaterFlowers, 1, 4, 0.20f, 0.75f, true);
                    if (Roll(25f))
                        AddCluster(baseData, vegetationList.swampDeadTrees, 1, 3, 0.35f, 1.00f, true);
                }
                else if (patch == "forest")
                {
                    AddCluster(baseData, vegetationList.swampTrees, 1, 4, 0.50f, 1.30f, true);
                    AddCluster(baseData, vegetationList.swampPlants, 3, 6, 0.35f, 1.10f, true);
                    if (Roll(20f))
                        AddCluster(baseData, vegetationList.swampDeadTrees, 1, 3, 0.35f, 1.00f, true);
                }
                else
                {
                    AddCluster(baseData, vegetationList.swampPlants, 2, 5, 0.30f, 0.95f, true);
                    AddCluster(baseData, vegetationList.swampBushes, 1, 4, 0.40f, 1.00f, true);
                    if (Roll(15f))
                        AddCluster(baseData, vegetationList.swampBones, 1, 2, 0.15f, 0.50f, true);
                }
            }
            else if (tile == 2)
            {
                if (wetLowland || patch == "flower")
                {
                    AddCluster(baseData, vegetationList.swampWaterPlants, 3, 7, 0.35f, 1.20f, true);
                    AddCluster(baseData, vegetationList.swampWaterFlowers, 2, 5, 0.25f, 0.85f, true);
                    AddCluster(baseData, vegetationList.swampFlowers, 2, 5, 0.25f, 0.90f, true);
                    if (Roll(25f))
                        AddFirefly(baseData, 0.10f, 8, 20, 50, CreateSwampFireflyProfile());
                }
                else if (patch == "grass")
                {
                    AddCluster(baseData, vegetationList.swampBushes, 3, 7, 0.45f, 1.20f, true);
                    AddCluster(baseData, vegetationList.swampPlants, 3, 6, 0.35f, 1.05f, true);
                    if (Roll(20f))
                        AddCluster(baseData, vegetationList.swampDeadTrees, 1, 2, 0.35f, 0.90f, true);
                }
                else
                {
                    AddCluster(baseData, vegetationList.swampTrees, 2, 5, 0.65f, 1.60f, true);
                    AddCluster(baseData, vegetationList.swampPlants, 2, 6, 0.40f, 1.10f, true);
                    AddCluster(baseData, vegetationList.swampDeadTrees, 1, 3, 0.35f, 1.00f, true);
                    AddFirefly(baseData, 0.08f, 10, 25, 60, CreateSwampFireflyProfile());
                }
            }
            else if (tile == 3)
            {
                AddCluster(baseData, vegetationList.swampRocks, 1, 3, 0.20f, 0.85f, true);
                if (Roll(20f))
                    AddCluster(baseData, vegetationList.swampBones, 1, 3, 0.15f, 0.60f, true);
                if (patch != "flower" && Roll(20f))
                    AddCluster(baseData, vegetationList.swampDeadTrees, 1, 2, 0.35f, 0.85f, true);
            }
        }

        private int GetNearestTileDistance(
         ContainerObject baseData,
         int tileType,
         int maxRadius)
        {
            int nearestDistance = -1;

            for (int radius = 1; radius <= maxRadius; radius++)
            {
                for (int offsetY = -radius; offsetY <= radius; offsetY++)
                {
                    for (int offsetX = -radius; offsetX <= radius; offsetX++)
                    {
                        if (Mathf.Max(Mathf.Abs(offsetX), Mathf.Abs(offsetY)) != radius)
                            continue;

                        int sampleX = baseData.x + offsetX;
                        int sampleY = baseData.y + offsetY;
                        if (!ExtensionMethods.In2DArrayBounds(baseData.dfTerrain.MapData.tilemapSamples, sampleX, sampleY))
                            continue;

                        int sampleTile = baseData.dfTerrain.MapData.tilemapSamples[sampleX, sampleY] & 0x3F;
                        if (sampleTile == tileType)
                        {
                            nearestDistance = radius;
                            return nearestDistance;
                        }
                    }
                }
            }

            return nearestDistance;
        }

        private WOFireflyProfile CreateFireflyProfile(
         Color bodyColor,
         Color haloColor,
         float activationDistance,
         float minSpawnHeight,
         float maxSpawnHeight,
         float minSpeed,
         float maxSpeed,
         float minVerticalRange,
         float maxVerticalRange,
         float minHorizontalRange,
         float maxHorizontalRange,
         float minTargetChangeTime,
         float maxTargetChangeTime,
         float minPulseFactor,
         float maxPulseFactor,
         int nightStartMin,
         int nightStartMax,
         int nightEndMin,
         int nightEndMax,
         bool randomizeHuePerInstance = false,
         float hueVariance = 0.05f,
         bool cycleHue = false,
         float hueShiftSpeed = 0.02f,
         float huePhaseVariance = 0.15f,
         float haloAlpha = 0.1f)
        {
            return new WOFireflyProfile()
            {
                bodyColor = bodyColor,
                haloColor = haloColor,
                activationDistance = activationDistance * fireflyActivationDistanceScale,
                minSpawnHeight = minSpawnHeight,
                maxSpawnHeight = maxSpawnHeight,
                minSpeed = minSpeed,
                maxSpeed = maxSpeed,
                minVerticalRange = minVerticalRange,
                maxVerticalRange = maxVerticalRange,
                minHorizontalRange = minHorizontalRange,
                maxHorizontalRange = maxHorizontalRange,
                minTargetChangeTime = minTargetChangeTime,
                maxTargetChangeTime = maxTargetChangeTime,
                minPulseFactor = minPulseFactor,
                maxPulseFactor = maxPulseFactor,
                nightStartMin = nightStartMin,
                nightStartMax = nightStartMax,
                nightEndMin = nightEndMin,
                nightEndMax = nightEndMax,
                randomizeHuePerInstance = randomizeHuePerInstance,
                hueVariance = hueVariance,
                cycleHue = cycleHue,
                hueShiftSpeed = hueShiftSpeed,
                huePhaseVariance = huePhaseVariance,
                haloAlpha = haloAlpha,
            };
        }

        private WOFireflyProfile CreateWoodlandFireflyProfile(bool dense = false, bool magical = false)
        {
            WOFireflyProfile profile = CreateFireflyProfile(
                new Color(0.62f, 1.00f, 0.55f),
                new Color(0.72f, 1.00f, 0.65f),
                dense ? 820f : 700f,
                0.55f,
                1.55f,
                0.75f,
                1.15f,
                1.5f,
                2.3f,
                2.0f,
                3.5f,
                0.45f,
                2.3f,
                0.12f,
                0.60f,
                1015,
                1085,
                325,
                375,
                false,
                0.04f,
                false,
                0.02f,
                0.10f,
                0.10f);

            if (magical)
            {
                profile.randomizeHuePerInstance = true;
                profile.hueVariance = 0.22f;
                profile.cycleHue = true;
                profile.hueShiftSpeed = 0.010f;
                profile.huePhaseVariance = 0.45f;
                profile.haloAlpha = 0.14f;
            }

            return profile;
        }

        private WOFireflyProfile CreateMountainFireflyProfile(bool magical = false)
        {
            WOFireflyProfile profile = CreateFireflyProfile(
                new Color(0.55f, 0.72f, 1.00f),
                new Color(0.65f, 0.82f, 1.00f),
                680f,
                0.50f,
                1.35f,
                0.65f,
                0.95f,
                1.2f,
                1.9f,
                1.6f,
                2.8f,
                0.55f,
                2.6f,
                0.10f,
                0.42f,
                1040,
                1095,
                330,
                370,
                false,
                0.03f,
                magical,
                magical ? 0.012f : 0.02f,
                magical ? 0.35f : 0.08f,
                magical ? 0.14f : 0.10f);

            if (magical)
            {
                profile.randomizeHuePerInstance = true;
                profile.hueVariance = 0.18f;
            }

            return profile;
        }

        private WOFireflyProfile CreateMountainWoodsFireflyProfile(bool magical = false)
        {
            WOFireflyProfile profile = CreateFireflyProfile(
                new Color(0.50f, 0.95f, 0.85f),
                new Color(0.62f, 1.00f, 0.92f),
                760f,
                0.55f,
                1.45f,
                0.75f,
                1.05f,
                1.4f,
                2.1f,
                1.9f,
                3.2f,
                0.45f,
                2.4f,
                0.12f,
                0.55f,
                1025,
                1090,
                325,
                375,
                magical,
                magical ? 0.18f : 0.04f,
                magical,
                magical ? 0.012f : 0.02f,
                magical ? 0.35f : 0.10f,
                magical ? 0.14f : 0.10f);

            return profile;
        }

        private WOFireflyProfile CreateRainforestFireflyProfile(bool magical = false)
        {
            WOFireflyProfile profile = CreateFireflyProfile(
                new Color(0.45f, 1.00f, 0.78f),
                new Color(0.55f, 1.00f, 0.88f),
                860f,
                0.70f,
                1.90f,
                0.85f,
                1.25f,
                1.8f,
                2.9f,
                2.2f,
                3.8f,
                0.35f,
                2.1f,
                0.16f,
                0.72f,
                1005,
                1080,
                325,
                385,
                magical,
                magical ? 0.20f : 0.06f,
                magical,
                magical ? 0.014f : 0.02f,
                magical ? 0.40f : 0.12f,
                magical ? 0.15f : 0.11f);

            return profile;
        }

        private WOFireflyProfile CreateSwampFireflyProfile(bool magical = false)
        {
            WOFireflyProfile profile = CreateFireflyProfile(
                new Color(0.76f, 1.00f, 0.36f),
                new Color(0.88f, 1.00f, 0.52f),
                900f,
                0.45f,
                1.45f,
                0.55f,
                0.95f,
                1.0f,
                1.9f,
                2.6f,
                4.2f,
                0.55f,
                2.9f,
                0.14f,
                0.62f,
                1000,
                1085,
                325,
                390,
                magical,
                magical ? 0.24f : 0.08f,
                magical,
                magical ? 0.012f : 0.018f,
                magical ? 0.45f : 0.15f,
                magical ? 0.16f : 0.12f);

            return profile;
        }

        private WOFireflyProfile CreateDesertOasisFireflyProfile(bool magical = false)
        {
            WOFireflyProfile profile = CreateFireflyProfile(
                new Color(0.55f, 0.98f, 0.82f),
                new Color(0.80f, 1.00f, 0.72f),
                650f,
                0.35f,
                1.05f,
                0.65f,
                0.95f,
                0.8f,
                1.6f,
                1.4f,
                2.4f,
                0.60f,
                2.8f,
                0.12f,
                0.45f,
                1035,
                1095,
                330,
                365,
                magical,
                magical ? 0.20f : 0.08f,
                magical,
                magical ? 0.010f : 0.018f,
                magical ? 0.35f : 0.10f,
                magical ? 0.14f : 0.09f);

            return profile;
        }

        private WOFireflyProfile CreateSubtropicalFireflyProfile(bool magical = false)
        {
            WOFireflyProfile profile = CreateFireflyProfile(
                new Color(1.00f, 0.82f, 0.48f),
                new Color(1.00f, 0.90f, 0.62f),
                740f,
                0.55f,
                1.45f,
                0.70f,
                1.00f,
                1.3f,
                2.2f,
                2.0f,
                3.2f,
                0.45f,
                2.4f,
                0.12f,
                0.52f,
                1015,
                1085,
                325,
                380,
                magical,
                magical ? 0.20f : 0.06f,
                magical,
                magical ? 0.012f : 0.02f,
                magical ? 0.35f : 0.10f,
                magical ? 0.14f : 0.10f);

            return profile;
        }

        private WOFireflyProfile CreateHauntedFireflyProfile(bool magical = false)
        {
            WOFireflyProfile profile = CreateFireflyProfile(
                new Color(0.78f, 0.48f, 1.00f),
                new Color(0.90f, 0.60f, 1.00f),
                720f,
                0.50f,
                1.30f,
                0.60f,
                0.90f,
                1.2f,
                2.0f,
                1.8f,
                3.0f,
                0.55f,
                2.7f,
                0.12f,
                0.48f,
                1030,
                1095,
                330,
                372,
                magical,
                magical ? 0.24f : 0.10f,
                magical,
                magical ? 0.012f : 0.008f,
                magical ? 0.45f : 0.18f,
                magical ? 0.15f : 0.11f);

            return profile;
        }

        private static void AddPatternRing(
         ContainerObject baseData,
         List<int> billboardCollection,
         float radiusX,
         float radiusY,
         int count,
         bool checkOnLand,
         float rotationDegrees,
         float jitter = 0.08f)
        {
            if (billboardCollection == null || billboardCollection.Count == 0 || count <= 0)
                return;

            for (int i = 0; i < count; i++)
            {
                float angle = rotationDegrees + (360f / count * i) + Random.Range(-8f, 8f);
                Vector2 offset = RotateOffset(new Vector2(radiusX, 0f), angle);
                offset.y *= radiusY / Mathf.Max(0.01f, radiusX);
                offset.x += Random.Range(-jitter, jitter);
                offset.y += Random.Range(-jitter, jitter);
                AddBillboardAtOffsetToBatch(baseData, billboardCollection, offset.x, offset.y, checkOnLand);
            }
        }

        private static void AddPatternArc(
         ContainerObject baseData,
         List<int> billboardCollection,
         float radiusX,
         float radiusY,
         float startDegrees,
         float endDegrees,
         int count,
         bool checkOnLand,
         float rotationDegrees,
         float jitter = 0.08f)
        {
            if (billboardCollection == null || billboardCollection.Count == 0 || count <= 0)
                return;

            if (count == 1)
            {
                float angle = rotationDegrees + ((startDegrees + endDegrees) * 0.5f);
                Vector2 singleOffset = RotateOffset(new Vector2(radiusX, 0f), angle);
                singleOffset.y *= radiusY / Mathf.Max(0.01f, radiusX);
                AddBillboardAtOffsetToBatch(baseData, billboardCollection, singleOffset.x, singleOffset.y, checkOnLand);
                return;
            }

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / (float)(count - 1);
                float angle = rotationDegrees + Mathf.Lerp(startDegrees, endDegrees, t) + Random.Range(-8f, 8f);
                Vector2 offset = RotateOffset(new Vector2(radiusX, 0f), angle);
                offset.y *= radiusY / Mathf.Max(0.01f, radiusX);
                offset.x += Random.Range(-jitter, jitter);
                offset.y += Random.Range(-jitter, jitter);
                AddBillboardAtOffsetToBatch(baseData, billboardCollection, offset.x, offset.y, checkOnLand);
            }
        }

        private static void AddPatternLine(
         ContainerObject baseData,
         List<int> billboardCollection,
         float startX,
         float startY,
         float endX,
         float endY,
         int count,
         bool checkOnLand,
         float jitter = 0.10f)
        {
            if (billboardCollection == null || billboardCollection.Count == 0 || count <= 0)
                return;

            if (count == 1)
            {
                AddBillboardAtOffsetToBatch(baseData, billboardCollection, (startX + endX) * 0.5f, (startY + endY) * 0.5f, checkOnLand);
                return;
            }

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / (float)(count - 1);
                float offsetX = Mathf.Lerp(startX, endX, t) + Random.Range(-jitter, jitter);
                float offsetY = Mathf.Lerp(startY, endY, t) + Random.Range(-jitter, jitter);
                AddBillboardAtOffsetToBatch(baseData, billboardCollection, offsetX, offsetY, checkOnLand);
            }
        }

        private static void AddPatternScatter(
         ContainerObject baseData,
         List<int> billboardCollection,
         float centerX,
         float centerY,
         int count,
         float minRadius,
         float maxRadius,
         bool checkOnLand)
        {
            if (billboardCollection == null || billboardCollection.Count == 0 || count <= 0)
                return;

            for (int i = 0; i < count; i++)
            {
                float angle = Random.Range(0f, 360f);
                float radius = Random.Range(minRadius, maxRadius);
                Vector2 offset = RotateOffset(new Vector2(radius, 0f), angle);
                AddBillboardAtOffsetToBatch(baseData, billboardCollection, centerX + offset.x, centerY + offset.y, checkOnLand);
            }
        }

        private static Vector2 RotateOffset(Vector2 offset, float rotationDegrees)
        {
            float radians = rotationDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            return new Vector2(offset.x * cos - offset.y * sin, offset.x * sin + offset.y * cos);
        }

        private static void AddBillboardAtOffsetToBatch(
         ContainerObject baseData,
         List<int> billboardCollection,
         float offsetX,
         float offsetY,
         bool checkOnLand)
        {
            if (billboardCollection == null || billboardCollection.Count == 0)
                return;

            int record = Random.Range(0, billboardCollection.Count);
            AddBillboardAtOffsetToBatch(baseData, billboardCollection, offsetX, offsetY, checkOnLand, record);
        }

        private static void AddBillboardAtOffsetToBatch(
         ContainerObject baseData,
         List<int> billboardCollection,
         float offsetX,
         float offsetY,
         bool checkOnLand,
         int record)
        {
            if (billboardCollection == null || billboardCollection.Count == 0 || record < 0 || record >= billboardCollection.Count)
                return;

            Vector3 pos = new Vector3((baseData.x + offsetX) * baseData.scale, 0, (baseData.y + offsetY) * baseData.scale);
            float sampledHeight = baseData.terrain.SampleHeight(pos + baseData.terrain.transform.position);
            pos.y = sampledHeight - (baseData.steepness / slopeSinkRatio);

            if (checkOnLand &&
                !TileTypeCheck(pos, baseData, true, false, false, true, true) &&
                TileTypeCheck(pos, baseData, false, true, false, false, false) &&
                baseData.steepness < Mathf.Clamp(90f - ((sampledHeight / baseData.maxTerrainHeight) / 0.85f * 100f), 40f, 90f))
            {
                baseData.dfBillboardBatch.AddItem(billboardCollection[record], pos);
            }
            else if (!checkOnLand &&
                TileTypeCheck(pos, baseData, true, false, false, false, false) &&
                !TileTypeCheck(pos, baseData, false, false, false, true, true) &&
                baseData.steepness < Mathf.Clamp(90f - ((sampledHeight / baseData.maxTerrainHeight) / 0.85f * 100f), 40f, 90f))
            {
                baseData.dfBillboardBatch.AddItem(billboardCollection[record], pos);
            }
        }

        private bool TryAddWoodlandMicroEncounter(ContainerObject baseData, int tile, float height, float weight)
        {
            if (!Roll(0.03f))
                return false;

            int encounter = Random.Range(0, 20);
            if (encounter >= 10)
                return TryAddWoodlandMicroEncounterExtra(baseData, tile, height, weight);

            float rotation = Random.Range(0f, 360f);
            switch (encounter)
            {
                case 0:
                    if (tile != 2)
                        return false;
                    AddMushroomRingToBatch(baseData, 23);
                    AddPatternRing(baseData, vegetationList.temperateWoodlandFlowers, 0.95f, 0.80f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandBushes, 0f, 0f, 2, 0.45f, 0.95f, true);
                    return true;
                case 1:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandBushes, 0f, 0f, 4, 0.20f, 0.75f, true);
                    AddPatternRing(baseData, vegetationList.temperateWoodlandFlowers, 1.15f, 0.95f, 7, true, rotation);
                    return true;
                case 2:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.temperateWoodlandDeadTrees, 1.10f, 0.75f, 205f, 338f, 3, true, rotation);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandMushroom, 0f, 0.15f, 3, 0.15f, 0.55f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandBushes, 0f, -0.10f, 2, 0.30f, 0.80f, true);
                    return true;
                case 3:
                    if (tile != 2)
                        return false;
                    AddPatternLine(baseData, vegetationList.temperateWoodlandFlowers, -1.20f, -0.45f, 1.10f, 0.55f, 7, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandFlowers, 0.10f, 0.05f, 3, 0.20f, 0.65f, true);
                    return true;
                case 4:
                    if (tile != 2)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.temperateWoodlandTrees, -0.72f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.temperateWoodlandTrees, 0.72f, 0f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandBushes, 0f, 0f, 3, 0.20f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandFlowers, 0f, 0f, 3, 0.30f, 0.85f, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.temperateWoodlandRocks, 1.05f, 0.90f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandFlowers, 0f, 0f, 4, 0.25f, 0.75f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandBushes, 0f, 0f, 2, 0.25f, 0.65f, true);
                    return true;
                case 6:
                    if (tile != 2)
                        return false;
                    AddPatternArc(baseData, vegetationList.temperateWoodlandBushes, 1.35f, 0.85f, 160f, 330f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandFlowers, 0.25f, 0f, 4, 0.25f, 0.90f, true);
                    return true;
                case 7:
                    if (tile != 2)
                        return false;
                    AddPatternRing(baseData, vegetationList.temperateWoodlandTrees, 1.25f, 1.05f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandBushes, 0f, 0f, 4, 0.25f, 0.85f, true);
                    return true;
                case 8:
                    if (tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, 0f, 0.10f, true);
                    AddPatternRing(baseData, vegetationList.temperateWoodlandMushroom, 0.78f, 0.62f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandFlowers, 0f, 0f, 2, 0.35f, 0.75f, true);
                    return true;
                default:
                    if (tile != 2 || height < 0.05f || weight < stochastics.tempForestLimit[1])
                        return false;
                    AddPatternRing(baseData, vegetationList.temperateWoodlandFlowers, 1.10f, 0.90f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandBushes, 0f, 0f, 3, 0.25f, 0.85f, true);
                    AddFirefly(baseData, 0.08f, 6, 10, 20, CreateWoodlandFireflyProfile(false, true));
                    return true;
            }
        }

        private bool TryAddMountainMicroEncounter(ContainerObject baseData, int tile, float height, float weight)
        {
            if (!Roll(0.03f))
                return false;

            int encounter = Random.Range(0, 20);
            if (encounter >= 10)
                return TryAddMountainMicroEncounterExtra(baseData, tile, height, weight);

            float rotation = Random.Range(0f, 360f);
            switch (encounter)
            {
                case 0:
                    if (tile != 2)
                        return false;
                    AddStoneCircleToBatch(baseData, vegetationList.mountainsRocks, 5, 0);
                    AddPatternScatter(baseData, vegetationList.mountainsFlowers, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.mountainsRocks, -1.15f, -0.15f, 1.20f, 0.25f, 6, true);
                    AddPatternScatter(baseData, vegetationList.mountainsFlowers, 0f, 0f, 3, 0.30f, 0.85f, true);
                    return true;
                case 2:
                    if (tile != 2)
                        return false;
                    AddPatternArc(baseData, vegetationList.mountainsNeedleTrees, 1.25f, 0.85f, 195f, 345f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.mountainsGrass, 0f, 0.15f, 3, 0.25f, 0.80f, true);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.mountainsRocks, 1.35f, 0.90f, 170f, 335f, 6, true, rotation);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.mountainsDeadTrees, 0f, -0.20f, true);
                    return true;
                case 4:
                    if (tile != 2)
                        return false;
                    AddPatternRing(baseData, vegetationList.mountainsFlowers, 0.90f, 0.70f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.mountainsGrass, 0f, 0f, 4, 0.20f, 0.70f, true);
                    AddPatternRing(baseData, vegetationList.mountainsRocks, 1.35f, 1.05f, 4, true, rotation + 18f);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.mountainsDeadTrees, 1.10f, 0.92f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.mountainsRocks, 0f, 0f, 3, 0.25f, 0.75f, true);
                    return true;
                case 6:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.mountainsNeedleTrees, -1.05f, -0.25f, 1.15f, 0.30f, 4, true);
                    AddPatternScatter(baseData, vegetationList.mountainsDeadTrees, 0f, 0f, 2, 0.35f, 0.90f, true);
                    return true;
                case 7:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.mountainsTrees, 0f, 0f, 3, 0.25f, 0.90f, true);
                    AddPatternRing(baseData, vegetationList.mountainsFlowers, 1.10f, 0.85f, 5, true, rotation);
                    return true;
                case 8:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.mountainsRocks, 1.30f, 0.70f, 190f, 345f, 5, true, rotation);
                    AddPatternArc(baseData, vegetationList.mountainsNeedleTrees, 0.80f, 0.45f, 205f, 335f, 2, true, rotation);
                    return true;
                default:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.mountainsTrees, 0f, 0f, 2, 0.25f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.mountainsNeedleTrees, 0f, 0f, 2, 0.35f, 0.90f, true);
                    AddPatternScatter(baseData, vegetationList.mountainsFlowers, 0f, 0f, 3, 0.25f, 0.90f, true);
                    return true;
            }
        }

        private bool TryDesertMicroEncounter(ContainerObject baseData, int tile, bool arid, bool oasisCore, bool oasisOuter, string patch)
        {
            if (!Roll(arid ? 0.022f : 0.03f))
                return false;

            int encounter = Random.Range(0, 20);
            if (encounter >= 10)
                return TryDesertMicroEncounterExtra(baseData, tile, arid, oasisCore, oasisOuter, patch);

            float rotation = Random.Range(0f, 360f);
            switch (encounter)
            {
                case 0:
                    if (!oasisCore || tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.desertTrees, 1.15f, 0.75f, 185f, 340f, arid ? 2 : 3, true, rotation);
                    AddPatternScatter(baseData, vegetationList.desertWaterPlants, 0f, 0f, arid ? 4 : 5, 0.20f, 0.85f, true);
                    AddPatternScatter(baseData, vegetationList.desertWaterFlowers, 0f, 0f, 3, 0.20f, 0.75f, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.desertCactus, 1.05f, 0.90f, arid ? 6 : 5, true, rotation);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.desertCactus, 0f, 0f, true);
                    return true;
                case 2:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.desertDeadTrees, 1.00f, 0.70f, 200f, 340f, 3, true, rotation);
                    AddPatternScatter(baseData, vegetationList.desertCactus, 0f, 0f, 2, 0.25f, 0.80f, true);
                    AddPatternScatter(baseData, vegetationList.desertStones, 0f, 0f, 2, 0.25f, 0.70f, true);
                    return true;
                case 3:
                    if (tile != 2)
                        return false;
                    AddPatternLine(baseData, vegetationList.desertFlowers, -1.10f, -0.20f, 1.15f, 0.25f, 6, true);
                    AddPatternScatter(baseData, vegetationList.desertPlants, 0f, 0f, 3, 0.20f, 0.75f, true);
                    return true;
                case 4:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.desertStones, 1.20f, 0.75f, 175f, 325f, 5, true, rotation);
                    AddPatternScatter(baseData, arid ? vegetationList.desertCactus : vegetationList.desertPlants, 0f, 0f, 2, 0.25f, 0.70f, true);
                    return true;
                case 5:
                    if (!(oasisCore || oasisOuter) || tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.desertTrees, -0.70f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.desertTrees, 0.70f, 0f, true);
                    AddPatternScatter(baseData, vegetationList.desertWaterPlants, 0f, 0f, 4, 0.20f, 0.75f, true);
                    return true;
                case 6:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.desertCactus, -1.05f, -0.30f, 1.05f, 0.30f, arid ? 5 : 4, true);
                    AddPatternScatter(baseData, vegetationList.desertStones, 0f, 0f, 2, 0.25f, 0.70f, true);
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.desertStones, 1.05f, 0.80f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.desertPlants, 0f, 0f, 3, 0.20f, 0.60f, true);
                    if (!arid)
                        AddPatternScatter(baseData, vegetationList.desertFlowers, 0f, 0f, 2, 0.20f, 0.55f, true);
                    return true;
                case 8:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.desertCactus, 1.25f, 0.70f, 165f, 330f, arid ? 5 : 4, true, rotation);
                    AddPatternArc(baseData, vegetationList.desertDeadTrees, 0.80f, 0.45f, 205f, 320f, 2, true, rotation);
                    if (!arid)
                        AddPatternScatter(baseData, vegetationList.desertPlants, 0f, 0f, 2, 0.20f, 0.55f, true);
                    return true;
                default:
                    if (!(oasisCore || oasisOuter) || tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.desertWaterPlants, 0.95f, 0.75f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.desertWaterFlowers, 0f, 0f, 3, 0.20f, 0.70f, true);
                    if (!arid)
                        AddBillboardAtOffsetToBatch(baseData, vegetationList.desertTrees, 0f, -0.10f, true);
                    return true;
            }
        }

        private bool TryHauntedMicroEncounter(ContainerObject baseData, int tile)
        {
            if (!Roll(0.028f))
                return false;

            int encounter = Random.Range(0, 20);
            if (encounter >= 10)
                return TryHauntedMicroEncounterExtra(baseData, tile);

            float rotation = Random.Range(0f, 360f);
            switch (encounter)
            {
                case 0:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.hauntedWoodlandBones, 1.00f, 0.82f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandMushroom, 0f, 0f, 2, 0.15f, 0.45f, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.hauntedWoodlandDeadTrees, 1.10f, 0.80f, 180f, 340f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandPlants, 0f, 0f, 3, 0.25f, 0.70f, true);
                    return true;
                case 2:
                    if (tile == 0)
                        return false;
                    AddMushroomRingToBatch(baseData, 23);
                    AddPatternRing(baseData, vegetationList.hauntedWoodlandBones, 1.15f, 0.95f, 5, true, rotation);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.hauntedWoodlandBushes, 0.95f, 0.75f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandBones, 0f, 0f, 2, 0.20f, 0.55f, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandDeadTrees, 0f, 0f, 2, 0.35f, 0.85f, true);
                    return true;
                case 4:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandFlowers, 0f, 0f, 4, 0.25f, 0.80f, true);
                    AddPatternArc(baseData, vegetationList.hauntedWoodlandDeadTrees, 1.05f, 0.68f, 195f, 335f, 3, true, rotation);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandBones, 0f, 0f, 1, 0.20f, 0.40f, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.hauntedWoodlandRocks, 1.20f, 0.75f, 165f, 330f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandBones, 0f, 0f, 3, 0.25f, 0.75f, true);
                    return true;
                case 6:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandTrees, 0f, 0f, 3, 0.30f, 0.95f, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandDeadTrees, 0f, 0f, 2, 0.25f, 0.75f, true);
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.hauntedWoodlandDirtTrees, -1.00f, -0.20f, 1.00f, 0.20f, 4, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandDeadTrees, 0f, 0f, 2, 0.25f, 0.75f, true);
                    return true;
                case 8:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.hauntedWoodlandMushroom, 1.05f, 0.70f, 175f, 340f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandBushes, 0f, 0f, 3, 0.25f, 0.70f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.hauntedWoodlandRocks, 1.15f, 0.65f, 170f, 335f, 5, true, rotation);
                    AddPatternArc(baseData, vegetationList.hauntedWoodlandDeadTrees, 0.90f, 0.45f, 195f, 325f, 2, true, rotation);
                    return true;
            }
        }

        private bool TryMountainWoodsMicroEncounter(ContainerObject baseData, int tile)
        {
            if (!Roll(0.03f))
                return false;

            int encounter = Random.Range(0, 20);
            if (encounter >= 10)
                return TryMountainWoodsMicroEncounterExtra(baseData, tile);

            float rotation = Random.Range(0f, 360f);
            switch (encounter)
            {
                case 0:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.woodlandHillsNeedleTrees, 1.15f, 0.80f, 185f, 340f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsFlowers, 0f, 0f, 2, 0.25f, 0.70f, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.woodlandHillsRocks, 1.05f, 0.85f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsTrees, 0f, 0f, 2, 0.25f, 0.70f, true);
                    return true;
                case 2:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.woodlandHillsFlowers, 0f, 0f, 4, 0.25f, 0.80f, true);
                    AddPatternArc(baseData, vegetationList.woodlandHillsBushes, 1.10f, 0.70f, 180f, 335f, 4, true, rotation);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.woodlandHillsDeadTrees, 1.00f, 0.70f, 190f, 335f, 3, true, rotation);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsNeedleTrees, 0f, 0f, 2, 0.30f, 0.85f, true);
                    return true;
                case 4:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandMushroom, 0f, 0f, 4, 0.20f, 0.70f, true);
                    AddPatternArc(baseData, vegetationList.woodlandHillsBushes, 1.10f, 0.65f, 175f, 330f, 4, true, rotation);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.woodlandHillsTrees, -0.85f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.woodlandHillsTrees, 0.85f, 0f, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsBushes, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
                case 6:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.woodlandHillsNeedleTrees, 1.15f, 0.65f, 170f, 330f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsFlowers, 0f, 0f, 3, 0.25f, 0.80f, true);
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.woodlandHillsRocks, 1.10f, 0.88f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsDirtPlants, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 8:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.woodlandHillsTrees, 0f, 0f, 2, 0.30f, 0.80f, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsNeedleTrees, 0f, 0f, 2, 0.35f, 0.90f, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsBushes, 0f, 0f, 2, 0.25f, 0.75f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.woodlandHillsTrees, -1.00f, -0.25f, 1.10f, 0.25f, 4, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsBushes, 0f, 0f, 2, 0.25f, 0.70f, true);
                    return true;
            }
        }

        private bool TryRainforestMicroEncounter(ContainerObject baseData, int tile, bool lowland)
        {
            if (!Roll(0.03f))
                return false;

            int encounter = Random.Range(0, 20);
            if (encounter >= 10)
                return TryRainforestMicroEncounterExtra(baseData, tile, lowland);

            float rotation = Random.Range(0f, 360f);
            switch (encounter)
            {
                case 0:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.rainforestEggs, 0.85f, 0.68f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.rainforestPlants, 0f, 0f, 4, 0.20f, 0.65f, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.rainforestPlants, 1.00f, 0.80f, 7, true, rotation);
                    AddPatternScatter(baseData, vegetationList.rainforestFlowers, 0f, 0f, 3, 0.20f, 0.60f, true);
                    return true;
                case 2:
                    if (tile != 2)
                        return false;
                    AddPatternRing(baseData, vegetationList.rainforestFlowers, 1.20f, 0.95f, 7, true, rotation);
                    AddPatternScatter(baseData, vegetationList.rainforestBushes, 0f, 0f, 3, 0.25f, 0.70f, true);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.rainforestTrees, 0f, 0f, 4, 0.35f, 1.05f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestPlants, 0f, 0f, 4, 0.25f, 0.85f, true);
                    return true;
                case 4:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.rainforestBushes, 1.25f, 0.85f, 175f, 340f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.rainforestFlowers, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.rainforestRocks, 1.10f, 0.70f, 170f, 335f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.rainforestPlants, 0f, 0f, 3, 0.20f, 0.75f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.rainforestTrees, 0f, 0.05f, true);
                    return true;
                case 6:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.rainforestFlowers, 0f, 0f, 4, 0.20f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestPlants, 0f, 0f, 4, 0.20f, 0.75f, true);
                    if (lowland)
                        AddFirefly(baseData, 0.06f, 6, 15, 30, CreateRainforestFireflyProfile(true));
                    return true;
                case 7:
                    if (tile != 2)
                        return false;
                    AddPatternLine(baseData, vegetationList.rainforestFlowers, -1.15f, -0.30f, 1.15f, 0.25f, 7, true);
                    AddPatternScatter(baseData, vegetationList.rainforestPlants, 0f, 0f, 3, 0.25f, 0.80f, true);
                    return true;
                case 8:
                    if (tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.rainforestTrees, -0.85f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.rainforestTrees, 0.85f, 0f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestBushes, 0f, 0f, 3, 0.20f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestPlants, 0f, 0f, 3, 0.25f, 0.75f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.rainforestTrees, 1.15f, 0.95f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.rainforestBushes, 0f, 0f, 4, 0.20f, 0.70f, true);
                    return true;
            }
        }

        private bool TrySubtropicalMicroEncounter(ContainerObject baseData, int tile, bool riparianPocket, bool dryHighland)
        {
            if (!Roll(0.028f))
                return false;

            int encounter = Random.Range(0, 20);
            if (encounter >= 10)
                return TrySubtropicalMicroEncounterExtra(baseData, tile, riparianPocket, dryHighland);

            float rotation = Random.Range(0f, 360f);
            switch (encounter)
            {
                case 0:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.subtropicalSucculents, 1.25f, 0.80f, 170f, 335f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.subtropicalRocks, 0f, 0f, 2, 0.25f, 0.70f, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.subtropicalTrees, 1.15f, 0.90f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.subtropicalFlowers, 0f, 0f, 4, 0.20f, 0.65f, true);
                    return true;
                case 2:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.subtropicalBushes, 1.20f, 0.82f, 180f, 340f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.subtropicalPlants, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.subtropicalTrees, -1.05f, -0.25f, 1.10f, 0.22f, 4, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalBushes, 0f, 0f, 2, 0.20f, 0.65f, true);
                    return true;
                case 4:
                    if (tile != 2)
                        return false;
                    AddPatternLine(baseData, vegetationList.subtropicalFlowers, -1.15f, -0.18f, 1.20f, 0.20f, 7, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalPlants, 0f, 0f, 2, 0.20f, 0.55f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalSucculents, 0f, 0f, 1, 0.35f, 0.80f, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.subtropicalDeadTrees, 1.00f, 0.78f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.subtropicalSucculents, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 6:
                    if (!riparianPocket || tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.subtropicalTrees, 0f, 0f, 3, 0.30f, 0.90f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalPlants, 0f, 0f, 4, 0.20f, 0.75f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalFlowers, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.subtropicalTrees, -0.82f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.subtropicalTrees, 0.82f, 0f, true);
                    AddPatternRing(baseData, vegetationList.subtropicalFlowers, 1.10f, 0.85f, 5, true, rotation);
                    return true;
                case 8:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.subtropicalBushes, 1.05f, 0.82f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.subtropicalTrees, 0f, 0f, 2, 0.35f, 0.95f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.subtropicalTrees, 0f, 0f, dryHighland ? 2 : 3, 0.35f, 0.95f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalDeadTrees, 0f, 0f, 1, 0.30f, 0.75f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalRocks, 0f, 0f, 2, 0.25f, 0.70f, true);
                    return true;
            }
        }

        private bool TrySwampMicroEncounter(ContainerObject baseData, int tile, bool wetLowland)
        {
            if (!Roll(0.028f))
                return false;

            int encounter = Random.Range(0, 20);
            if (encounter >= 10)
                return TrySwampMicroEncounterExtra(baseData, tile, wetLowland);

            float rotation = Random.Range(0f, 360f);
            switch (encounter)
            {
                case 0:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.swampWaterPlants, 1.00f, 0.80f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.swampWaterFlowers, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternRing(baseData, vegetationList.swampDeadTrees, 1.05f, 0.84f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.swampWaterPlants, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 2:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.swampBones, 0f, 0f, 4, 0.20f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.swampPlants, 0f, 0f, 3, 0.20f, 0.75f, true);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.swampWaterPlants, 1.20f, 0.75f, 180f, 340f, 5, true, rotation);
                    AddPatternArc(baseData, vegetationList.swampDeadTrees, 0.90f, 0.50f, 205f, 330f, 2, true, rotation);
                    return true;
                case 4:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.swampTrees, 0f, 0f, 3, 0.35f, 0.95f, true);
                    AddPatternScatter(baseData, vegetationList.swampBushes, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.swampRocks, 1.10f, 0.70f, 170f, 335f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.swampDeadTrees, 0f, 0f, 2, 0.30f, 0.75f, true);
                    AddPatternScatter(baseData, vegetationList.swampPlants, 0f, 0f, 2, 0.20f, 0.65f, true);
                    return true;
                case 6:
                    if (!wetLowland || tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.swampWaterFlowers, 0f, 0f, 4, 0.20f, 0.65f, true);
                    AddPatternScatter(baseData, vegetationList.swampWaterPlants, 0f, 0f, 4, 0.20f, 0.70f, true);
                    AddFirefly(baseData, 0.06f, 6, 12, 28, CreateSwampFireflyProfile(true));
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.swampTrees, -0.80f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.swampTrees, 0.80f, 0f, true);
                    AddPatternScatter(baseData, vegetationList.swampWaterPlants, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 8:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.swampFlowers, 0f, 0f, 4, 0.20f, 0.75f, true);
                    AddPatternScatter(baseData, vegetationList.swampWaterFlowers, 0f, 0f, 3, 0.20f, 0.65f, true);
                    AddPatternScatter(baseData, vegetationList.swampPlants, 0f, 0f, 2, 0.20f, 0.65f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.swampBushes, 1.15f, 0.80f, 180f, 340f, 6, true, rotation);
                    AddPatternScatter(baseData, vegetationList.swampDeadTrees, 0f, 0f, 2, 0.30f, 0.80f, true);
                    AddPatternScatter(baseData, vegetationList.swampWaterPlants, 0f, 0f, 2, 0.25f, 0.70f, true);
                    return true;
            }
        }

        private bool TryAddWoodlandMicroEncounterExtra(ContainerObject baseData, int tile, float height, float weight)
        {
            float rotation = Random.Range(0f, 360f);
            switch (Random.Range(0, 10))
            {
                case 0:
                    if (tile != 2)
                        return false;
                    AddPatternLine(baseData, vegetationList.temperateWoodlandFlowers, -1.10f, -0.42f, 1.10f, 0.42f, 7, true);
                    AddPatternLine(baseData, vegetationList.temperateWoodlandBushes, -1.00f, 0.50f, 1.00f, -0.48f, 5, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.temperateWoodlandTrees, 1.25f, 0.78f, 170f, 328f, 4, true, rotation);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.temperateWoodlandDeadTrees, 0f, -0.12f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandBushes, 0f, 0f, 2, 0.25f, 0.65f, true);
                    return true;
                case 2:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandMushroom, 0f, 0f, 5, 0.15f, 0.65f, true);
                    AddPatternArc(baseData, vegetationList.temperateWoodlandFlowers, 1.05f, 0.68f, 180f, 340f, 5, true, rotation);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.temperateWoodlandRocks, -1.00f, 0f, 1.05f, 0f, 5, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandFlowers, 0f, 0f, 4, 0.20f, 0.75f, true);
                    return true;
                case 4:
                    if (tile != 2)
                        return false;
                    AddPatternLine(baseData, vegetationList.temperateWoodlandTrees, -1.05f, -0.58f, 1.05f, -0.55f, 3, true);
                    AddPatternLine(baseData, vegetationList.temperateWoodlandTrees, -1.00f, 0.58f, 1.00f, 0.55f, 3, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandFlowers, 0f, 0f, 4, 0.25f, 0.80f, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.temperateWoodlandDeadTrees, -0.95f, -0.18f, 0.95f, 0.12f, 3, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandMushroom, 0f, 0f, 4, 0.15f, 0.60f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandBushes, 0f, 0f, 2, 0.25f, 0.65f, true);
                    return true;
                case 6:
                    if (tile != 2)
                        return false;
                    AddPatternArc(baseData, vegetationList.temperateWoodlandBushes, 1.25f, 0.70f, 175f, 338f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandTrees, 0f, 0f, 2, 0.30f, 0.80f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandFlowers, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
                case 7:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandTrees, 0f, 0f, 4, 0.30f, 0.95f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandBushes, 0f, 0f, 4, 0.20f, 0.75f, true);
                    return true;
                case 8:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandFlowers, 0f, 0f, 5, 0.20f, 0.80f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandBushes, 0f, 0f, 2, 0.20f, 0.60f, true);
                    AddFirefly(baseData, 0.07f, 7, 8, 18, CreateWoodlandFireflyProfile(false, true));
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandRocks, 0f, 0f, 4, 0.25f, 0.85f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandDeadTrees, 0f, 0f, 2, 0.35f, 0.90f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandFlowers, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
            }
        }

        private bool TryAddMountainMicroEncounterExtra(ContainerObject baseData, int tile, float height, float weight)
        {
            float rotation = Random.Range(0f, 360f);
            switch (Random.Range(0, 10))
            {
                case 0:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.mountainsRocks, -1.20f, -0.18f, 1.20f, 0.18f, 6, true);
                    AddPatternLine(baseData, vegetationList.mountainsFlowers, -1.00f, 0.42f, 1.00f, -0.42f, 5, true);
                    return true;
                case 1:
                    if (tile != 2)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.mountainsNeedleTrees, -0.80f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.mountainsNeedleTrees, 0.80f, 0f, true);
                    AddPatternScatter(baseData, vegetationList.mountainsRocks, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
                case 2:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.mountainsDeadTrees, 1.10f, 0.72f, 180f, 336f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.mountainsFlowers, 0f, 0f, 3, 0.20f, 0.75f, true);
                    return true;
                case 3:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.mountainsTrees, 0f, 0f, 3, 0.30f, 0.85f, true);
                    AddPatternScatter(baseData, vegetationList.mountainsGrass, 0f, 0f, 4, 0.20f, 0.70f, true);
                    return true;
                case 4:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.mountainsNeedleTrees, -1.10f, -0.55f, 1.10f, -0.55f, 3, true);
                    AddPatternLine(baseData, vegetationList.mountainsDeadTrees, -1.00f, 0.52f, 1.00f, 0.52f, 3, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.mountainsRocks, -1.00f, -0.55f, 1.00f, 0.55f, 5, true);
                    AddPatternLine(baseData, vegetationList.mountainsRocks, -1.00f, 0.55f, 1.00f, -0.55f, 5, true);
                    return true;
                case 6:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.mountainsTrees, 0f, 0f, 2, 0.25f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.mountainsNeedleTrees, 0f, 0f, 2, 0.25f, 0.75f, true);
                    AddPatternScatter(baseData, vegetationList.mountainsGrass, 0f, 0f, 4, 0.20f, 0.75f, true);
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.mountainsRocks, 1.25f, 0.70f, 170f, 330f, 5, true, rotation);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.mountainsDeadTrees, 0f, 0.10f, true);
                    AddPatternScatter(baseData, vegetationList.mountainsGrass, 0f, 0f, 2, 0.20f, 0.65f, true);
                    return true;
                case 8:
                    if (tile != 2)
                        return false;
                    AddPatternLine(baseData, vegetationList.mountainsFlowers, -1.15f, -0.15f, 1.15f, 0.20f, 7, true);
                    AddPatternScatter(baseData, vegetationList.mountainsGrass, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.mountainsRocks, -1.00f, 0f, 1.00f, 0f, 5, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.mountainsDeadTrees, -0.85f, 0.40f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.mountainsDeadTrees, 0.85f, -0.40f, true);
                    return true;
            }
        }

        private bool TryDesertMicroEncounterExtra(ContainerObject baseData, int tile, bool arid, bool oasisCore, bool oasisOuter, string patch)
        {
            float rotation = Random.Range(0f, 360f);
            switch (Random.Range(0, 10))
            {
                case 0:
                    if (!(oasisCore || oasisOuter) || tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.desertTrees, -0.85f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.desertTrees, 0.85f, 0f, true);
                    AddPatternScatter(baseData, vegetationList.desertWaterPlants, 0f, 0f, 5, 0.20f, 0.80f, true);
                    AddPatternScatter(baseData, vegetationList.desertWaterFlowers, 0f, 0f, 3, 0.20f, 0.60f, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.desertStones, -1.20f, -0.10f, 1.20f, 0.10f, 6, true);
                    AddPatternScatter(baseData, vegetationList.desertPlants, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 2:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.desertCactus, 0f, 0f, arid ? 5 : 4, 0.25f, 0.85f, true);
                    AddPatternScatter(baseData, vegetationList.desertStones, 0f, 0f, 2, 0.25f, 0.70f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.desertDeadTrees, 0f, -0.12f, true);
                    return true;
                case 3:
                    if (!(oasisCore || oasisOuter) || tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.desertTrees, 1.20f, 0.72f, 180f, 330f, arid ? 2 : 3, true, rotation);
                    AddPatternArc(baseData, vegetationList.desertWaterPlants, 0.85f, 0.45f, 200f, 320f, 4, true, rotation);
                    return true;
                case 4:
                    if (tile != 2)
                        return false;
                    AddPatternLine(baseData, oasisCore ? vegetationList.desertWaterFlowers : vegetationList.desertFlowers, -1.10f, -0.22f, 1.10f, 0.18f, 6, true);
                    AddPatternScatter(baseData, vegetationList.desertPlants, 0f, 0f, 2, 0.20f, 0.55f, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.desertStones, -1.05f, -0.48f, 1.05f, 0.48f, 5, true);
                    AddPatternLine(baseData, vegetationList.desertStones, -1.05f, 0.48f, 1.05f, -0.48f, 5, true);
                    AddPatternScatter(baseData, vegetationList.desertCactus, 0f, 0f, 2, 0.20f, 0.65f, true);
                    return true;
                case 6:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.desertDeadTrees, 0f, 0f, 3, 0.30f, 0.90f, true);
                    AddPatternScatter(baseData, vegetationList.desertCactus, 0f, 0f, 2, 0.25f, 0.75f, true);
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.desertCactus, 1.15f, 0.75f, 175f, 335f, arid ? 5 : 4, true, rotation);
                    AddPatternArc(baseData, vegetationList.desertPlants, 0.85f, 0.45f, 200f, 325f, 3, true, rotation);
                    return true;
                case 8:
                    if (!(oasisCore || oasisOuter) || tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.desertTrees, 0f, 0f, 3, 0.30f, 0.85f, true);
                    AddPatternScatter(baseData, vegetationList.desertWaterPlants, 0f, 0f, 4, 0.20f, 0.75f, true);
                    AddPatternScatter(baseData, vegetationList.desertFlowers, 0f, 0f, 2, 0.20f, 0.55f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.desertCactus, 0f, 0f, arid ? 4 : 3, 0.25f, 0.75f, true);
                    AddPatternScatter(baseData, vegetationList.desertStones, 0f, 0f, 3, 0.20f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.desertDeadTrees, 0f, 0f, 1, 0.30f, 0.80f, true);
                    return true;
            }
        }

        private bool TryHauntedMicroEncounterExtra(ContainerObject baseData, int tile)
        {
            float rotation = Random.Range(0f, 360f);
            switch (Random.Range(0, 10))
            {
                case 0:
                    if (tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.hauntedWoodlandDeadTrees, 0f, -0.08f, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandFlowers, 0f, 0f, 4, 0.20f, 0.65f, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandBones, 0f, 0f, 2, 0.15f, 0.45f, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.hauntedWoodlandBones, -1.05f, -0.22f, 1.05f, 0.22f, 5, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandRocks, 0f, 0f, 2, 0.20f, 0.60f, true);
                    return true;
                case 2:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.hauntedWoodlandDeadTrees, 1.10f, 0.72f, 180f, 338f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandMushroom, 0f, 0f, 4, 0.15f, 0.55f, true);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.hauntedWoodlandBushes, 1.15f, 0.75f, 175f, 338f, 5, true, rotation);
                    AddPatternLine(baseData, vegetationList.hauntedWoodlandRocks, -0.70f, 0f, 0.70f, 0f, 3, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandBones, 0f, 0f, 1, 0.15f, 0.35f, true);
                    return true;
                case 4:
                    if (tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.hauntedWoodlandDeadTrees, -0.75f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.hauntedWoodlandDeadTrees, 0.75f, 0f, true);
                    AddPatternLine(baseData, vegetationList.hauntedWoodlandFlowers, -0.80f, -0.32f, 0.80f, 0.32f, 5, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.hauntedWoodlandDirtTrees, -1.05f, -0.55f, 1.05f, -0.55f, 3, true);
                    AddPatternLine(baseData, vegetationList.hauntedWoodlandDeadTrees, -1.00f, 0.55f, 1.00f, 0.55f, 3, true);
                    return true;
                case 6:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.hauntedWoodlandBushes, 1.05f, 0.65f, 170f, 330f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandMushroom, 0f, 0f, 4, 0.15f, 0.50f, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandBones, 0f, 0f, 2, 0.15f, 0.45f, true);
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandTrees, 0f, 0f, 2, 0.30f, 0.85f, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandDeadTrees, 0f, 0f, 3, 0.25f, 0.80f, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandRocks, 0f, 0f, 2, 0.20f, 0.65f, true);
                    return true;
                case 8:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.hauntedWoodlandRocks, -1.00f, 0f, 1.00f, 0f, 5, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.hauntedWoodlandDeadTrees, -0.72f, 0.35f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.hauntedWoodlandDeadTrees, 0.72f, -0.35f, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandBones, 0f, 0f, 2, 0.15f, 0.45f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandBushes, 0f, 0f, 4, 0.20f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandMushroom, 0f, 0f, 3, 0.15f, 0.50f, true);
                    AddPatternScatter(baseData, vegetationList.hauntedWoodlandDeadTrees, 0f, 0f, 1, 0.30f, 0.75f, true);
                    return true;
            }
        }

        private bool TryMountainWoodsMicroEncounterExtra(ContainerObject baseData, int tile)
        {
            float rotation = Random.Range(0f, 360f);
            switch (Random.Range(0, 10))
            {
                case 0:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.woodlandHillsBushes, -1.00f, -0.48f, 1.00f, -0.48f, 4, true);
                    AddPatternLine(baseData, vegetationList.woodlandHillsBushes, -1.00f, 0.48f, 1.00f, 0.48f, 4, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.woodlandHillsNeedleTrees, 0f, 0f, true);
                    return true;
                case 1:
                    if (tile != 2)
                        return false;
                    AddPatternArc(baseData, vegetationList.woodlandHillsTrees, 1.15f, 0.72f, 175f, 338f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsFlowers, 0f, 0f, 4, 0.20f, 0.75f, true);
                    return true;
                case 2:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandMushroom, 0f, 0f, 4, 0.15f, 0.55f, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsNeedleTrees, 0f, 0f, 2, 0.35f, 0.90f, true);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.woodlandHillsDeadTrees, -1.00f, -0.20f, 1.00f, 0.20f, 3, true);
                    AddPatternArc(baseData, vegetationList.woodlandHillsNeedleTrees, 1.05f, 0.60f, 185f, 332f, 3, true, rotation);
                    return true;
                case 4:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.woodlandHillsBushes, 0f, 0f, 5, 0.20f, 0.80f, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsFlowers, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.woodlandHillsTrees, 0f, 0f, 3, 0.30f, 0.85f, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsFlowers, 0f, 0f, 4, 0.20f, 0.75f, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsBushes, 0f, 0f, 2, 0.20f, 0.65f, true);
                    return true;
                case 6:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.woodlandHillsRocks, -1.10f, 0f, 1.10f, 0f, 5, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsDirtPlants, 0f, 0f, 3, 0.20f, 0.65f, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsTrees, 0f, 0f, 1, 0.30f, 0.70f, true);
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.woodlandHillsNeedleTrees, -0.75f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.woodlandHillsNeedleTrees, 0.75f, 0f, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsBushes, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 8:
                    if (tile != 2)
                        return false;
                    AddPatternScatter(baseData, vegetationList.woodlandHillsTrees, 0f, 0f, 2, 0.25f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.woodlandHillsNeedleTrees, 0f, 0f, 2, 0.25f, 0.75f, true);
                    AddPatternScatter(baseData, vegetationList.temperateWoodlandMushroom, 0f, 0f, 3, 0.15f, 0.50f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.woodlandHillsTrees, 1.20f, 0.75f, 178f, 335f, 4, true, rotation);
                    AddPatternLine(baseData, vegetationList.woodlandHillsBushes, -0.65f, 0f, 0.65f, 0f, 3, true);
                    return true;
            }
        }

        private bool TryRainforestMicroEncounterExtra(ContainerObject baseData, int tile, bool lowland)
        {
            float rotation = Random.Range(0f, 360f);
            switch (Random.Range(0, 10))
            {
                case 0:
                    if (tile != 2)
                        return false;
                    AddPatternLine(baseData, vegetationList.rainforestFlowers, -1.00f, -0.42f, 1.00f, 0.42f, 7, true);
                    AddPatternLine(baseData, vegetationList.rainforestPlants, -1.00f, 0.42f, 1.00f, -0.42f, 5, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.rainforestPlants, 1.20f, 0.78f, 176f, 336f, 6, true, rotation);
                    AddPatternArc(baseData, vegetationList.rainforestBushes, 0.85f, 0.50f, 195f, 325f, 4, true, rotation);
                    return true;
                case 2:
                    if (tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.rainforestTrees, -0.80f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.rainforestTrees, 0.80f, 0f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestEggs, 0f, 0f, 3, 0.15f, 0.50f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestPlants, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.rainforestTrees, 0f, 0f, 3, 0.35f, 0.95f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestBushes, 0f, 0f, 4, 0.20f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestFlowers, 0f, 0f, 3, 0.20f, 0.70f, true);
                    return true;
                case 4:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.rainforestRocks, -1.05f, -0.15f, 1.05f, 0.18f, 5, true);
                    AddPatternLine(baseData, vegetationList.rainforestPlants, -0.95f, 0.42f, 0.95f, -0.38f, 5, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.rainforestTrees, 1.10f, 0.75f, 175f, 335f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.rainforestBushes, 0f, 0f, 3, 0.20f, 0.65f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestPlants, 0f, 0f, 2, 0.20f, 0.55f, true);
                    return true;
                case 6:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.rainforestFlowers, 0f, 0f, 5, 0.20f, 0.80f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestPlants, 0f, 0f, 4, 0.20f, 0.70f, true);
                    if (lowland)
                        AddFirefly(baseData, 0.06f, 7, 10, 20, CreateRainforestFireflyProfile(true));
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.rainforestBushes, 1.10f, 0.72f, 180f, 338f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.rainforestTrees, 0f, 0f, 2, 0.35f, 0.90f, true);
                    return true;
                case 8:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.rainforestRocks, 1.10f, 0.65f, 170f, 330f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.rainforestEggs, 0f, 0f, 3, 0.15f, 0.45f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestPlants, 0f, 0f, 2, 0.20f, 0.55f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.rainforestTrees, -0.95f, -0.20f, 0.95f, 0.20f, 3, true);
                    AddPatternScatter(baseData, vegetationList.rainforestBushes, 0f, 0f, 4, 0.20f, 0.65f, true);
                    AddPatternScatter(baseData, vegetationList.rainforestFlowers, 0f, 0f, 2, 0.20f, 0.60f, true);
                    return true;
            }
        }

        private bool TrySubtropicalMicroEncounterExtra(ContainerObject baseData, int tile, bool riparianPocket, bool dryHighland)
        {
            float rotation = Random.Range(0f, 360f);
            switch (Random.Range(0, 10))
            {
                case 0:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.subtropicalTrees, -1.00f, -0.20f, 1.00f, 0.20f, 3, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalSucculents, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.subtropicalBushes, 0f, 0f, 5, 0.20f, 0.75f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalFlowers, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 2:
                    if (tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.subtropicalTrees, -0.85f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.subtropicalTrees, 0.85f, 0f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalRocks, 0f, 0f, 3, 0.20f, 0.65f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalFlowers, 0f, 0f, 2, 0.20f, 0.60f, true);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.subtropicalSucculents, 0f, 0f, 4, 0.25f, 0.80f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalDeadTrees, 0f, 0f, 2, 0.25f, 0.70f, true);
                    return true;
                case 4:
                    if (tile != 2)
                        return false;
                    AddPatternLine(baseData, vegetationList.subtropicalFlowers, -1.05f, -0.30f, 1.05f, 0.30f, 6, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalBushes, 0f, 0f, 2, 0.20f, 0.60f, true);
                    return true;
                case 5:
                    if (!riparianPocket || tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.subtropicalTrees, 0f, 0f, 3, 0.30f, 0.85f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalPlants, 0f, 0f, 4, 0.20f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalFlowers, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 6:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.subtropicalBushes, -1.00f, -0.48f, 1.00f, -0.48f, 4, true);
                    AddPatternLine(baseData, vegetationList.subtropicalBushes, -1.00f, 0.48f, 1.00f, 0.48f, 4, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.subtropicalTrees, 0f, 0f, true);
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.subtropicalRocks, 0f, 0f, 4, 0.20f, 0.75f, true);
                    AddPatternLine(baseData, vegetationList.subtropicalSucculents, -1.00f, -0.15f, 1.00f, 0.15f, 4, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalFlowers, 0f, 0f, 2, 0.20f, 0.60f, true);
                    return true;
                case 8:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.subtropicalTrees, 1.15f, 0.72f, 178f, 336f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.subtropicalDeadTrees, 0f, 0f, 1, 0.25f, 0.65f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalBushes, 0f, 0f, 2, 0.20f, 0.60f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.subtropicalBushes, 0f, 0f, 4, 0.20f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalSucculents, 0f, 0f, dryHighland ? 3 : 2, 0.20f, 0.65f, true);
                    AddPatternScatter(baseData, vegetationList.subtropicalTrees, 0f, 0f, 1, 0.30f, 0.75f, true);
                    return true;
            }
        }

        private bool TrySwampMicroEncounterExtra(ContainerObject baseData, int tile, bool wetLowland)
        {
            float rotation = Random.Range(0f, 360f);
            switch (Random.Range(0, 10))
            {
                case 0:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.swampDeadTrees, -1.00f, -0.50f, 1.00f, -0.50f, 3, true);
                    AddPatternLine(baseData, vegetationList.swampDeadTrees, -1.00f, 0.50f, 1.00f, 0.50f, 3, true);
                    return true;
                case 1:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.swampWaterPlants, 1.15f, 0.72f, 178f, 336f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.swampWaterFlowers, 0f, 0f, 4, 0.20f, 0.65f, true);
                    return true;
                case 2:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.swampFlowers, 0f, 0f, 4, 0.20f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.swampPlants, 0f, 0f, 4, 0.20f, 0.70f, true);
                    AddPatternScatter(baseData, vegetationList.swampBones, 0f, 0f, 2, 0.15f, 0.45f, true);
                    return true;
                case 3:
                    if (tile == 0)
                        return false;
                    AddPatternScatter(baseData, vegetationList.swampTrees, 0f, 0f, 3, 0.30f, 0.85f, true);
                    AddPatternScatter(baseData, vegetationList.swampDeadTrees, 0f, 0f, 2, 0.25f, 0.70f, true);
                    if (wetLowland)
                        AddFirefly(baseData, 0.06f, 8, 12, 24, CreateSwampFireflyProfile(true));
                    return true;
                case 4:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.swampRocks, -1.00f, -0.15f, 1.00f, 0.15f, 5, true);
                    AddPatternLine(baseData, vegetationList.swampWaterPlants, -1.05f, 0.40f, 1.05f, -0.40f, 5, true);
                    return true;
                case 5:
                    if (tile == 0)
                        return false;
                    AddPatternLine(baseData, vegetationList.swampBones, -0.95f, -0.20f, 0.95f, 0.20f, 5, true);
                    AddPatternScatter(baseData, vegetationList.swampWaterPlants, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 6:
                    if (tile == 0)
                        return false;
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.swampDeadTrees, -0.78f, 0f, true);
                    AddBillboardAtOffsetToBatch(baseData, vegetationList.swampDeadTrees, 0.78f, 0f, true);
                    AddPatternScatter(baseData, vegetationList.swampWaterPlants, 0f, 0f, 4, 0.20f, 0.65f, true);
                    return true;
                case 7:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.swampBushes, 1.10f, 0.75f, 180f, 338f, 5, true, rotation);
                    AddPatternScatter(baseData, vegetationList.swampPlants, 0f, 0f, 3, 0.20f, 0.65f, true);
                    return true;
                case 8:
                    if (tile != 2)
                        return false;
                    AddPatternLine(baseData, vegetationList.swampFlowers, -1.00f, -0.35f, 1.00f, 0.35f, 6, true);
                    AddPatternScatter(baseData, vegetationList.swampWaterFlowers, 0f, 0f, 3, 0.20f, 0.60f, true);
                    return true;
                default:
                    if (tile == 0)
                        return false;
                    AddPatternArc(baseData, vegetationList.swampDeadTrees, 1.10f, 0.72f, 175f, 335f, 4, true, rotation);
                    AddPatternScatter(baseData, vegetationList.swampPlants, 0f, 0f, 3, 0.20f, 0.65f, true);
                    AddPatternScatter(baseData, vegetationList.swampBones, 0f, 0f, 2, 0.15f, 0.45f, true);
                    return true;
            }
        }

        public static void AddBillboardToBatch(
         ContainerObject baseData,
         List<int> billboardCollection,
         float posVariance,
         bool checkOnLand) {
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
         ContainerObject baseData,
         List<int> billboardCollection,
         float posVariance,
         bool checkOnLand,
         int record) {
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
         ContainerObject baseData,
         int record) {
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
         ContainerObject baseData,
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
         ContainerObject baseData,
         float rndFirefly,
         float distanceVariation,
         int minNumber,
         int maxNumber,
         WOFireflyProfile profile) {
            if (profile == null)
                return;

            if (rndFirefly >= Random.Range(0.0f, 100.0f) && DaggerfallUnity.Instance.WorldTime.Now.SeasonValue != DaggerfallDateTime.Seasons.Winter && firefliesExist) {
                GameObject fireflyContainer = new GameObject();
                fireflyContainer.name = "fireflyContainer";
                fireflyContainer.transform.parent = baseData.dfBillboardBatch.transform;
                fireflyContainer.transform.position = new Vector3(baseData.dfTerrain.transform.position.x + (baseData.x * baseData.scale), baseData.terrain.SampleHeight(new Vector3(baseData.x * baseData.scale, 0, baseData.y * baseData.scale) + baseData.dfTerrain.transform.position) + baseData.dfTerrain.transform.position.y, baseData.dfTerrain.transform.position.z + (baseData.y * baseData.scale));
                fireflyContainer.AddComponent<WODistanceChecker>();
                fireflyContainer.GetComponent<WODistanceChecker>().distance = profile.activationDistance;
                for (int i = 0; i < Random.Range(minNumber, maxNumber); i++) {
                    fireflyContainer.GetComponent<WODistanceChecker>().CreateFirefly(mod, baseData.dfTerrain, baseData.x, baseData.y, baseData.scale, baseData.terrain, distanceVariation, profile); // Firefly
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
         float sSMax) {
            Vector3 shootingStarPos = new Vector3(dfTerrain.transform.position.x, dfTerrain.transform.position.y, dfTerrain.transform.position.z);
            GameObject shootingStarInstance = mod.GetAsset<GameObject>("ShootingStars", true);
            shootingStarInstance.transform.position = new Vector3(shootingStarPos.x, shootingStarPos.y + heightInTheSky, shootingStarPos.z);
            shootingStarInstance.transform.parent = dfBillboardBatch.transform;
            shootingStarInstance.transform.rotation = Quaternion.Euler(rotationAngleX, 0, 0);
            shootingStarInstance.AddComponent<WOShootingStarController>();
            shootingStarInstance.GetComponent<WOShootingStarController>().ps = shootingStarInstance.GetComponent<ParticleSystem>();
            var emissionModule = shootingStarInstance.GetComponent<WOShootingStarController>().ps.emission;
            emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(sSMin / 1000, sSMax / 1000);
        }

        static public bool TileTypeCheck(
         Vector3 pos,
         ContainerObject baseData,
         bool isOnAnyWaterTile,
         bool isOnPureGroundTile,
         bool isOnOrCloseToShallowWaterTile,
         bool isOnOrCloseToStreetTile,
         bool isCollidingWithBuilding) {
            bool result = true;

            bool stopCondition = false;

            float offsetA, offsetB;
            int roundedX, roundedY, sampleGround;

            if (isOnAnyWaterTile) {
                roundedX = (int)Mathf.Round(pos.x / baseData.scale);
                roundedY = (int)Mathf.Round(pos.z / baseData.scale);
                if (ExtensionMethods.In2DArrayBounds(baseData.dfTerrain.MapData.tilemapSamples, roundedX, roundedY)) {
                    sampleGround = baseData.dfTerrain.MapData.tilemapSamples[roundedX, roundedY] & 0x3F;

                    if (sampleGround != 0 &&
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
                     sampleGround != 61) {
                        result = false;
                    }
                }
            }
            if (isOnPureGroundTile) {
                roundedX = (int)Mathf.Round(pos.x / baseData.scale);
                roundedY = (int)Mathf.Round(pos.z / baseData.scale);
                if (ExtensionMethods.In2DArrayBounds(baseData.dfTerrain.MapData.tilemapSamples, roundedX, roundedY)) {
                    sampleGround = baseData.dfTerrain.MapData.tilemapSamples[roundedX, roundedY] & 0x3F;

                    if (sampleGround != 1 && sampleGround != 2 && sampleGround != 3) {
                        result = false;
                    }
                }
            }
            if (isOnOrCloseToShallowWaterTile) {
                for (int x = 0; x < 2 && stopCondition == false; x++) {
                    for (int y = 0; y < 2 && stopCondition == false; y++) {

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

                        if (ExtensionMethods.In2DArrayBounds(baseData.dfTerrain.MapData.tilemapSamples, roundedX - x, roundedY - y)) {
                            sampleGround = baseData.dfTerrain.MapData.tilemapSamples[roundedX - x, roundedY - y] & 0x3F;
                            if (sampleGround != 4 &&
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
                             sampleGround != 61) {
                                stopCondition = result = false;
                            }
                            else
                                stopCondition = true;
                        }
                    }
                }
            }
            if (isOnOrCloseToStreetTile) {
                for (int x = 0; x < 2 && stopCondition == false; x++) {
                    for (int y = 0; y < 2 && stopCondition == false; y++) {

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

                        if (ExtensionMethods.In2DArrayBounds(baseData.dfTerrain.MapData.tilemapSamples, roundedX - x, roundedY - y)) {
                            sampleGround = baseData.dfTerrain.MapData.tilemapSamples[roundedX - x, roundedY - y] & 0x3F;
                            if (sampleGround != 46 &&
                             sampleGround != 47 &&
                             sampleGround != 55) {
                                stopCondition = result = false;
                            }
                            else
                                stopCondition = true;
                        }
                    }
                }
            }
            if (isCollidingWithBuilding) {
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
            foreach (Transform child in parent) {
                if (child.gameObject.CompareTag(tag)) {
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
         int seed = 120) {
            float finalValue = 0f;
            for (int i = 0; i < octaves; ++i) {
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
         float limit2 = 0.6f) {
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

