using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Utility;

namespace WildernessOverhaul
{
    public class WOVegetationList
    {
        public List<int> temperateWoodlandFlowers, temperateWoodlandMushroom, temperateWoodlandBushes, temperateWoodlandRocks, temperateWoodlandTrees, temperateWoodlandDeadTrees, temperateWoodlandBeach;
        public List<int> woodlandHillsFlowers, woodlandHillsBushes, woodlandHillsRocks, woodlandHillsTrees, woodlandHillsNeedleTrees, woodlandHillsDeadTrees, woodlandHillsDirtPlants, woodlandHillsBeach;
        public List<int> mountainsFlowers, mountainsGrass, mountainsRocks, mountainsTrees, mountainsNeedleTrees, mountainsDeadTrees, mountainsBeach;
        public List<int> desertFlowers, desertWaterFlowers, desertPlants, desertWaterPlants, desertStones, desertTrees, desertCactus, desertDeadTrees;
        public List<int> hauntedWoodlandFlowers, hauntedWoodlandMushroom, hauntedWoodlandBones, hauntedWoodlandPlants, hauntedWoodlandBushes, hauntedWoodlandRocks, hauntedWoodlandTrees, hauntedWoodlandDirtTrees, hauntedWoodlandDeadTrees, hauntedWoodlandBeach;
        public List<int> rainforestFlowers, rainforestEggs, rainforestPlants, rainforestBushes, rainforestRocks, rainforestTrees, rainforestBeach;
        public List<int> subtropicalFlowers, subtropicalPlants, subtropicalBushes, subtropicalTrees, subtropicalSucculents, subtropicalRocks, subtropicalDeadTrees;
        public List<int> swampFlowers, swampWaterFlowers, swampPlants, swampWaterPlants, swampBushes, swampTrees, swampDeadTrees, swampRocks, swampBones;

        public List<int> collectionTemperateWoodlandFlowers = new List<int>(new int[] {2, 21, 22});
        public List<int> collectionTemperateWoodlandMushroom = new List<int>(new int[] { 7, 9, 23 });
        public List<int> collectionTemperateWoodlandBushes = new List<int>(new int[] { 1, 27, 28 });
        public List<int> collectionTemperateWoodlandRocks = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });
        public List<int> collectionTemperateWoodlandTrees = new List<int>(new int[] { 11, 12, 13, 14, 15, 16, 17, 18 });
        public List<int> collectionTemperateWoodlandDeadTrees = new List<int>(new int[] { 19, 20, 24, 25, 29, 30, 31 });
        public List<int> collectionTemperateWoodlandBeach = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });

        public List<int> collectionWoodlandHillsFlowers = new List<int>(new int[] { 2, 7, 21, 22 });
        public List<int> collectionWoodlandHillsBushes = new List<int>(new int[] { 9, 27, 31 });
        public List<int> collectionWoodlandHillsRocks = new List<int>(new int[] { 1, 3, 4, 6, 8, 10, 17, 18, 28 });
        public List<int> collectionWoodlandHillsTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 13, 13, 13, 14, 14, 14, 15, 15, 15, 16, 16, 16, 25, 30 });
        public List<int> collectionWoodlandHillsNeedleTrees = new List<int>(new int[] { 5, 11, 12, 25, 30 });
        public List<int> collectionWoodlandHillsDeadTrees = new List<int>(new int[] { 19, 20, 24 });
        public List<int> collectionWoodlandHillsDirtPlants = new List<int>(new int[] { 23, 26, 29 });
        public List<int> collectionWoodlandHillsBeach = new List<int>(new int[] { 26, 29, 31 });

        public List<int> collectionMountainsFlowers = new List<int>(new int[] { 22 });
        public List<int> collectionMountainsGrass = new List<int>(new int[] { 2, 7, 9, 23 });
        public List<int> collectionMountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 8, 10, 14, 17, 18, 27, 28, 31 });
        public List<int> collectionMountainsTrees = new List<int>(new int[] { 5, 11, 12, 13, 15, 21, 25, 30 });
        public List<int> collectionMountainsNeedleTrees = new List<int>(new int[] { 5, 11, 12, 25, 30, 12, 25, 30 });
        public List<int> collectionMountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 24, 26, 29 });
        public List<int> collectionMountainsBeach = new List<int>(new int[] { 8, 26, 29, 2, 7, 7, 7, 23, 23 });

        public List<int> collectionDesertFlowers = new List<int>(new int[] { 9, 9, 9, 17, 24, 24, 24, 26, 26, 31, 31, 31 });
        public List<int> collectionDesertWaterFlowers = new List<int>(new int[] { 7, 17, 24, 24, 29, 29 });
        public List<int> collectionDesertPlants = new List<int>(new int[] { 7, 25, 27 });
        public List<int> collectionDesertWaterPlants = new List<int>(new int[] { 7, 7, 7, 7, 7, 25, 27, 27, 27, 27, 27 });
        public List<int> collectionDesertStones = new List<int>(new int[] { 2, 3, 4, 6, 8, 18, 19, 20, 21, 22, });
        public List<int> collectionDesertTrees = new List<int>(new int[] { 5, 13, 13 });
        public List<int> collectionDesertCactus = new List<int>(new int[] { 1, 14, 15, 16 });
        public List<int> collectionDesertDeadTrees = new List<int>(new int[] { 10, 11, 12, 23, 28 });

        public List<int> collectionHauntedWoodlandFlowers = new List<int>(new int[] { 21 });
        public List<int> collectionHauntedWoodlandMushroom = new List<int>(new int[] { 22, 23 });
        public List<int> collectionHauntedWoodlandBones = new List<int>(new int[] { 11 });
        public List<int> collectionHauntedWoodlandPlants = new List<int>(new int[] { 7, 14, 17, 29, 31 });
        public List<int> collectionHauntedWoodlandBushes = new List<int>(new int[] { 2, 26, 27, 28 });
        public List<int> collectionHauntedWoodlandRocks = new List<int>(new int[] { 1, 3, 4, 5, 6, 8, 10, 12 });
        public List<int> collectionHauntedWoodlandTrees = new List<int>(new int[] { 13, 13, 13, 15 });
        public List<int> collectionHauntedWoodlandDirtTrees = new List<int>(new int[] { 18, 19, 20, 31 });
        public List<int> collectionHauntedWoodlandDeadTrees = new List<int>(new int[] { 16, 18, 24, 25, 30, 31 });
        public List<int> collectionHauntedWoodlandBeach = new List<int>(new int[] { 31, 8, 9, 14, 29, 31 });

        public List<int> collectionRainforestFlowers = new List<int>(new int[] { 6, 20, 21, 22, 26, 27 });
        public List<int> collectionRainforestEggs = new List<int>(new int[] { 28, 29, 31 });
        public List<int> collectionRainforestPlants = new List<int>(new int[] { 2, 5, 10, 11, 23, 24 });
        public List<int> collectionRainforestBushes = new List<int>(new int[] { 3, 9, 16, 18 });
        public List<int> collectionRainforestRocks = new List<int>(new int[] { 1, 4, 17, 19, 25 });
        public List<int> collectionRainforestTrees = new List<int>(new int[] { 12, 13, 14, 15, 30 });
        public List<int> collectionRainforestBeach = new List<int>(new int[] { });

        public List<float> elevationLevels = new List<float>(new float[] {
                Random.Range(0.66f,0.64f), Random.Range(0.61f,0.59f), Random.Range(0.56f,0.54f),
                Random.Range(0.51f,0.49f), Random.Range(0.46f,0.44f), Random.Range(0.41f,0.39f),
                Random.Range(0.36f, 0.34f), Random.Range(0.31f,0.29f), Random.Range(0.26f,0.24f),
                Random.Range(0.21f,0.19f), Random.Range(0.11f,0.09f), Random.Range(0.096f,0.84f),
                Random.Range(0.071f,0.069f), Random.Range(0.041f,0.039f)});

        //TEMPERATE WOODLAND
        //ELEVATION 0
        public List<int> temperateWoodlandFlowersAtElevation0 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation0 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation0 = new List<int>(new int[] { 1, 1, 1, 27, 28 });
        public List<int> temperateWoodlandRocksAtElevation0 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });
        public List<int> temperateWoodlandTreesAtElevation0 = new List<int>(new int[] { 13, 14, 15, 17, 18, 25, 25, 25 });
        public List<int> temperateWoodlandBeachAtElevation0 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandDeadTreesAtElevation0 = new List<int>(new int[] { 19, 20, 24, 25, 25, 25, 29, 30, 31 });

        //ELEVATION 1
        public List<int> temperateWoodlandTreesAtElevation1 = new List<int>(new int[] { 13, 14, 15, 17, 18, 25, 25, 25 });
        public List<int> temperateWoodlandDeadTreesAtElevation1 = new List<int>(new int[] { 19, 20, 24, 25, 25, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation1 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation1 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation1 = new List<int>(new int[] { 1, 1, 1, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation1 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation1 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 2
        public List<int> temperateWoodlandTreesAtElevation2 = new List<int>(new int[] { 13, 14, 15, 17, 18, 25, 25, 25 });
        public List<int> temperateWoodlandDeadTreesAtElevation2 = new List<int>(new int[] { 19, 20, 24, 25, 25, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation2 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation2 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation2 = new List<int>(new int[] { 1, 1, 1, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation2 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation2 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 3
        public List<int> temperateWoodlandTreesAtElevation3 = new List<int>(new int[] { 13, 14, 15, 17, 18, 25, 25, 25 });
        public List<int> temperateWoodlandDeadTreesAtElevation3 = new List<int>(new int[] { 19, 20, 24, 25, 25, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation3 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation3 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation3 = new List<int>(new int[] { 1, 1, 1, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation3 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation3 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 4
        public List<int> temperateWoodlandTreesAtElevation4 = new List<int>(new int[] { 13, 14, 15, 17, 18, 25, 25, 25 });
        public List<int> temperateWoodlandDeadTreesAtElevation4 = new List<int>(new int[] { 19, 20, 24, 25, 25, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation4 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation4 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation4 = new List<int>(new int[] { 1, 1, 1, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation4 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation4 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 5
        public List<int> temperateWoodlandTreesAtElevation5 = new List<int>(new int[] { 13, 14, 15, 17, 18, 25, 25, 25 });
        public List<int> temperateWoodlandDeadTreesAtElevation5 = new List<int>(new int[] { 19, 20, 24, 25, 25, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation5 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation5 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation5 = new List<int>(new int[] { 1, 1, 1, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation5 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation5 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 6
        public List<int> temperateWoodlandTreesAtElevation6 = new List<int>(new int[] { 13, 14, 15, 17, 18, 25, 25, 25 });
        public List<int> temperateWoodlandDeadTreesAtElevation6 = new List<int>(new int[] { 19, 20, 24, 25, 25, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation6 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation6 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation6 = new List<int>(new int[] { 1, 1, 1, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation6 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation6 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 7
        public List<int> temperateWoodlandTreesAtElevation7 = new List<int>(new int[] { 13, 14, 15, 17, 18, 25, 25, 25 });
        public List<int> temperateWoodlandDeadTreesAtElevation7 = new List<int>(new int[] { 19, 20, 24, 25, 25, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation7 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation7 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation7 = new List<int>(new int[] { 1, 1, 1, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation7 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation7 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 8
        public List<int> temperateWoodlandTreesAtElevation8 = new List<int>(new int[] { 13, 14, 15, 17, 18, 25, 25, 25 });
        public List<int> temperateWoodlandDeadTreesAtElevation8 = new List<int>(new int[] { 19, 20, 24, 25, 25, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation8 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation8 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation8 = new List<int>(new int[] { 1, 1, 1, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation8 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation8 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 9
        public List<int> temperateWoodlandTreesAtElevation9A = new List<int>(new int[] { 25, 25, 25, 25, 25, 20, 19, 26, 26, 30, 1, 1, 1}); // Conifere Forest 1
        public List<int> temperateWoodlandTreesAtElevation9B = new List<int>(new int[] { 11, 13, 13, 14, 15, 16, 17, 1}); // Mixed Forest 1
        public List<int> temperateWoodlandTreesAtElevation9C = new List<int>(new int[] { 11, 13, 14, 15, 16, 17, 17, 18, 18, 18, 1}); // Oak forest
        public List<int> temperateWoodlandTreesAtElevation9D = new List<int>(new int[] { 13, 13, 25, 25, 25, 1, 1, 1}); // Conifere forest 2
        public List<int> temperateWoodlandTreesAtElevation9E = new List<int>(new int[] { 11, 14, 14, 14, 15, 15, 24, 1}); // Birch Forest
        public List<int> temperateWoodlandTreesAtElevation9F = new List<int>(new int[] { 11, 13, 13, 13, 14, 15, 25, 25, 1}); // Conifere forest 2
        public List<int> temperateWoodlandDeadTreesAtElevation9 = new List<int>(new int[] { 19, 20, 24, 25, 25, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation9 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation9 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation9 = new List<int>(new int[] { 1, 1, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation9 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation9 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 10
        public List<int> temperateWoodlandTreesAtElevation10A = new List<int>(new int[] { 25, 25, 25, 25, 25, 20, 19, 26, 26, 30, 1, 1, 1}); // Conifere Forest 1
        public List<int> temperateWoodlandTreesAtElevation10B = new List<int>(new int[] { 11, 13, 13, 14, 15, 16, 17, 1}); // Mixed Forest 1
        public List<int> temperateWoodlandTreesAtElevation10C = new List<int>(new int[] { 11, 13, 14, 15, 16, 17, 17, 18, 18, 18, 1}); // Oak forest
        public List<int> temperateWoodlandTreesAtElevation10D = new List<int>(new int[] { 11, 13, 14, 15, 16, 16, 17, 17, 18, 18, 1}); // Deciduous Forest
        public List<int> temperateWoodlandTreesAtElevation10E = new List<int>(new int[] { 11, 14, 14, 14, 15, 15, 24, 1}); // Birch Forest
        public List<int> temperateWoodlandTreesAtElevation10F = new List<int>(new int[] { 11, 13, 13, 13, 14, 15, 25, 25, 1}); // Conifere forest 2
        public List<int> temperateWoodlandDeadTreesAtElevation10 = new List<int>(new int[] { 19, 20, 24, 25, 25, 25, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation10 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation10 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation10 = new List<int>(new int[] { 1, 1, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation10 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation10 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 11
        public List<int> temperateWoodlandTreesAtElevation11A = new List<int>(new int[] { 11, 13, 14, 15, 25, 25, 25, 25, 25, 20, 19, 26, 26, 1}); // Conifere forest
        public List<int> temperateWoodlandTreesAtElevation11B = new List<int>(new int[] { 11, 12, 13, 13, 14, 15, 16, 17, 18, 26, 26, 1}); // Mixed Forest 1
        public List<int> temperateWoodlandTreesAtElevation11C = new List<int>(new int[] { 11, 13, 14, 15, 16, 16, 17, 17, 18, 18, 26, 26, 1}); // Deciduous Forest
        public List<int> temperateWoodlandTreesAtElevation11D = new List<int>(new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 18, 26, 26, 1}); // Mixed Forest 2
        public List<int> temperateWoodlandTreesAtElevation11E = new List<int>(new int[] { 11, 13, 14, 14, 14, 15, 15, 15, 16, 17, 18, 24, 26, 26, 1}); // Birch Forest
        public List<int> temperateWoodlandTreesAtElevation11F = new List<int>(new int[] { 11, 13, 13, 14, 15, 16, 17, 17, 18, 18, 18, 18, 26, 26, 1}); // Oak forest
        public List<int> temperateWoodlandDeadTreesAtElevation11 = new List<int>(new int[] { 19, 20, 24, 25, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation11 = new List<int>(new int[] { 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation11 = new List<int>(new int[] { 7, 23 });
        public List<int> temperateWoodlandBushesAtElevation11 = new List<int>(new int[] { 1, 26, 27, 27, 28, 28 });
        public List<int> temperateWoodlandBeachAtElevation11 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation11 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 12
        public List<int> temperateWoodlandTreesAtElevation12A = new List<int>(new int[] { 11, 12, 13, 14, 15, 16, 17, 18}); // Mixed Forest
        public List<int> temperateWoodlandTreesAtElevation12B = new List<int>(new int[] { 11, 12, 12, 12, 13, 16, 16, 16, 17, 17, 17, 18}); // Beach forest 1
        public List<int> temperateWoodlandTreesAtElevation12C = new List<int>(new int[] { 11, 12, 12, 12, 13, 16, 16, 16, 17, 18, 18, 18}); // Beach forest 2
        public List<int> temperateWoodlandTreesAtElevation12D = new List<int>(new int[] { 11, 13, 14, 14, 14, 14, 15, 15, 15, 16, 17, 18, 24}); // Birch Forest 1
        public List<int> temperateWoodlandTreesAtElevation12E = new List<int>(new int[] { 11, 13, 14, 14, 14, 15, 15, 15, 15, 16, 17, 18, 24}); // Birch Forest 2
        public List<int> temperateWoodlandTreesAtElevation12F = new List<int>(new int[] { 11, 12, 13, 14, 15, 16, 16, 17, 17, 18, 18}); // Deciduous Forest
        public List<int> temperateWoodlandDeadTreesAtElevation12 = new List<int>(new int[] { 19, 20, 24, 25, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation12 = new List<int>(new int[] { 2, 21, 21, 21, 22, 22 });
        public List<int> temperateWoodlandMushroomAtElevation12 = new List<int>(new int[] { 7, 7, 7, 9, 23, 23 });
        public List<int> temperateWoodlandBushesAtElevation12 = new List<int>(new int[] { 1, 26, 27, 27, 28, 28 });
        public List<int> temperateWoodlandBeachAtElevation12 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation12 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });


        //ELEVATION 13
        public List<int> temperateWoodlandTreesAtElevation13A = new List<int>(new int[] { 11, 11, 14, 14, 15, 15, 15, 24}); // Birch Forest
        public List<int> temperateWoodlandTreesAtElevation13B = new List<int>(new int[] { 13, 13, 18, 18, 18, 27, 28}); // Oak forest
        public List<int> temperateWoodlandTreesAtElevation13C = new List<int>(new int[] { 11, 12, 13, 14, 15, 16, 18}); // Deciduous Forest
        public List<int> temperateWoodlandTreesAtElevation13D = new List<int>(new int[] { 12, 12, 12, 16, 17, 27, 27, 28, 28}); // Beach forest 1
        public List<int> temperateWoodlandTreesAtElevation13E = new List<int>(new int[] { 12, 12, 16, 16, 18, 27, 27, 28, 28}); // Beach forest 2
        public List<int> temperateWoodlandTreesAtElevation13F = new List<int>(new int[] { 12, 12, 17, 17, 18, 27, 27, 28, 28}); // Beach forest 3
        public List<int> temperateWoodlandDeadTreesAtElevation13 = new List<int>(new int[] { 19, 20, 24, 29, 30, 31 });
        public List<int> temperateWoodlandFlowersAtElevation13 = new List<int>(new int[] { 2, 21, 22 });
        public List<int> temperateWoodlandMushroomAtElevation13 = new List<int>(new int[] { 7, 9, 23 });
        public List<int> temperateWoodlandBushesAtElevation13 = new List<int>(new int[] { 26, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation13 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation13 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        //ELEVATION 14
        public List<int> temperateWoodlandTreesAtElevation14A = new List<int>(new int[] { 12, 16, 17, 18, 27, 27, 28, 28}); // Deciduous Forest
        public List<int> temperateWoodlandTreesAtElevation14B = new List<int>(new int[] { 12, 12, 12, 16, 17, 27, 27, 27, 28, 28, 28}); // Beach forest 1
        public List<int> temperateWoodlandTreesAtElevation14C = new List<int>(new int[] { 12, 12, 16, 16, 18, 27, 27, 27, 28, 28, 28}); // Beach forest 2
        public List<int> temperateWoodlandTreesAtElevation14D = new List<int>(new int[] { 12, 12, 17, 17, 18, 27, 27, 27, 28, 28, 28}); // Beach forest 3
        public List<int> temperateWoodlandDeadTreesAtElevation14 = new List<int>(new int[] { 19, 19, 20, 20, 24, 24, 29, 29, 30, 31, 31 });
        public List<int> temperateWoodlandFlowersAtElevation14 = new List<int>(new int[] { 2, 2, 2, 21, 22, 22 });
        public List<int> temperateWoodlandMushroomAtElevation14 = new List<int>(new int[] { 7, 7, 9, 9, 9, 23 });
        public List<int> temperateWoodlandBushesAtElevation14 = new List<int>(new int[] { 26, 26, 26, 27, 28 });
        public List<int> temperateWoodlandBeachAtElevation14 = new List<int>(new int[] { 31, 3, 3, 4, 4, 5, 5, 6, 29 });
        public List<int> temperateWoodlandRocksAtElevation14 = new List<int>(new int[] { 3, 4, 5, 6, 8, 10, 26 });

        public WOVegetationList(int rndSeed) {
            Random.seed = rndSeed;

            woodlandHillsRocks = collectionWoodlandHillsRocks;
            woodlandHillsDeadTrees = collectionWoodlandHillsDeadTrees;
            woodlandHillsBeach = collectionWoodlandHillsBeach;

            desertFlowers = collectionDesertFlowers;
            desertWaterFlowers = collectionDesertWaterFlowers;
            desertPlants = collectionDesertPlants;
            desertWaterPlants = collectionDesertWaterPlants;
            desertStones = collectionDesertStones;
            desertTrees = collectionDesertTrees;
            desertCactus = collectionDesertCactus;
            desertDeadTrees = collectionDesertDeadTrees;

            hauntedWoodlandFlowers = collectionHauntedWoodlandFlowers;
            hauntedWoodlandMushroom = collectionHauntedWoodlandMushroom;
            hauntedWoodlandBones = collectionHauntedWoodlandBones;
            hauntedWoodlandPlants = collectionHauntedWoodlandPlants;
            hauntedWoodlandBushes = collectionHauntedWoodlandBushes;
            hauntedWoodlandRocks = collectionHauntedWoodlandRocks;
            hauntedWoodlandTrees = collectionHauntedWoodlandTrees;
            hauntedWoodlandDirtTrees = collectionHauntedWoodlandDirtTrees;
            hauntedWoodlandDeadTrees = collectionHauntedWoodlandDeadTrees;
            hauntedWoodlandBeach = collectionHauntedWoodlandBeach;

            rainforestFlowers = collectionRainforestFlowers;
            rainforestEggs = collectionRainforestEggs;
            rainforestPlants = collectionRainforestPlants;
            rainforestBushes = collectionRainforestBushes;
            rainforestRocks = collectionRainforestRocks;
            rainforestTrees = collectionRainforestTrees;
            rainforestBeach = collectionRainforestBeach;

            subtropicalFlowers = Weighted(2, 7, 21, 22, 24, 26);
            subtropicalPlants = Weighted(7, 23, 25, 27, 29, 31);
            subtropicalBushes = Weighted(9, 26, 27, 28);
            subtropicalTrees = Weighted(5, 11, 13, 14, 15, 24, 30);
            subtropicalSucculents = Weighted(1, 14, 15, 16, 25);
            subtropicalRocks = Weighted(1, 3, 4, 6, 8, 10, 17, 18, 28);
            subtropicalDeadTrees = Weighted(10, 11, 12, 19, 20, 23, 28);

            swampFlowers = Weighted(7, 21, 22, 24, 29);
            swampWaterFlowers = Weighted(7, 17, 24, 24, 29, 29);
            swampPlants = Weighted(2, 5, 7, 10, 11, 23, 24, 27);
            swampWaterPlants = Weighted(7, 7, 25, 27, 27, 29);
            swampBushes = Weighted(3, 9, 16, 18, 26, 27, 28);
            swampTrees = Weighted(12, 13, 14, 15, 18, 30);
            swampDeadTrees = Weighted(16, 18, 19, 20, 24, 25, 29, 30, 31);
            swampRocks = Weighted(1, 4, 17, 19, 25, 31);
            swampBones = Weighted(11, 31);
        }

        private List<int> Weighted(params int[] entries)
        {
            return new List<int>(entries);
        }

        private int GetElevationBand(float elevation)
        {
            for (int i = 0; i < elevationLevels.Count; i++)
            {
                if (elevation > elevationLevels[i])
                    return i;
            }

            return elevationLevels.Count;
        }

        private int GetElevationTier(int elevationBand)
        {
            if (elevationBand <= 2)
                return 0;
            if (elevationBand <= 5)
                return 1;
            if (elevationBand <= 8)
                return 2;
            if (elevationBand <= 11)
                return 3;

            return 4;
        }

        private void ConfigureAdditionalBiomeLists(float elevation, DaggerfallDateTime.Seasons season, float mapStyle)
        {
            int elevationBand = GetElevationBand(elevation);
            int tier = GetElevationTier(elevationBand);
            bool spring = season == DaggerfallDateTime.Seasons.Spring;
            bool summer = season == DaggerfallDateTime.Seasons.Summer;
            bool winter = season == DaggerfallDateTime.Seasons.Winter;
            bool lushStyle = mapStyle >= 50f;
            bool sparseStyle = mapStyle <= 35f;

            ConfigureWoodlandHillsSupportLists(tier, winter);
            ConfigureDesertLists(tier, spring, summer, winter, lushStyle, sparseStyle);
            ConfigureHauntedWoodlandLists(tier, spring, summer, winter, lushStyle, sparseStyle);
            ConfigureRainforestLists(tier, spring, summer, winter, lushStyle, sparseStyle);
            ConfigureSubtropicalLists(tier, spring, summer, winter, lushStyle, sparseStyle);
            ConfigureSwampLists(tier, spring, summer, winter, lushStyle, sparseStyle);
        }

        private void ConfigureWoodlandHillsSupportLists(int tier, bool winter)
        {
            switch (tier)
            {
                case 0:
                    woodlandHillsRocks = Weighted(1, 3, 4, 6, 8, 10, 17, 18, 18, 28, 28);
                    woodlandHillsDeadTrees = Weighted(19, 20, 24, 24, 29, 30);
                    woodlandHillsBeach = Weighted(26, 26, 29, 31);
                    break;
                case 1:
                    woodlandHillsRocks = Weighted(1, 3, 4, 6, 8, 10, 17, 18, 28);
                    woodlandHillsDeadTrees = Weighted(19, 20, 24, 24, 29, 30);
                    woodlandHillsBeach = Weighted(26, 29, 31);
                    break;
                case 2:
                    woodlandHillsRocks = Weighted(1, 3, 4, 6, 8, 10, 17, 18, 28);
                    woodlandHillsDeadTrees = Weighted(19, 20, 24, 29, 30);
                    woodlandHillsBeach = Weighted(23, 26, 29, 31);
                    break;
                case 3:
                    woodlandHillsRocks = Weighted(1, 3, 4, 6, 8, 10, 17, 18);
                    woodlandHillsDeadTrees = Weighted(19, 20, 24, 29, 30);
                    woodlandHillsBeach = Weighted(23, 26, 29, 31, 31);
                    break;
                default:
                    woodlandHillsRocks = Weighted(1, 3, 4, 6, 8, 10, 17);
                    woodlandHillsDeadTrees = Weighted(19, 20, 24, 29, 30);
                    woodlandHillsBeach = Weighted(23, 26, 29, 31, 31);
                    break;
            }
        }

        private void ConfigureDesertLists(int tier, bool spring, bool summer, bool winter, bool lushStyle, bool sparseStyle)
        {
            switch (tier)
            {
                case 0:
                    desertTrees = lushStyle ? Weighted(5, 13, 13, 30) : Weighted(5, 13, 13);
                    desertCactus = Weighted(1, 14, 15, 16, 16, 16);
                    desertDeadTrees = Weighted(10, 11, 12, 23, 23, 28, 28);
                    desertPlants = Weighted(7, 25, 25, 27);
                    desertWaterPlants = Weighted(7, 7, 25, 27, 27, 27);
                    desertStones = Weighted(2, 3, 4, 6, 8, 18, 19, 20, 21, 22, 22);
                    desertFlowers = Weighted(9, 17, 24, 26, 31);
                    desertWaterFlowers = Weighted(7, 17, 24, 24, 29, 29);
                    break;
                case 1:
                    desertTrees = lushStyle ? Weighted(5, 13, 13, 30, 30) : Weighted(5, 13, 13, 30);
                    desertCactus = Weighted(1, 14, 15, 16, 16);
                    desertDeadTrees = Weighted(10, 11, 12, 23, 28, 28);
                    desertPlants = Weighted(7, 25, 27, 27);
                    desertWaterPlants = Weighted(7, 7, 25, 27, 27, 27, 27);
                    desertStones = Weighted(2, 3, 4, 6, 8, 18, 19, 20, 21, 22);
                    desertFlowers = Weighted(9, 17, 24, 24, 26, 31);
                    desertWaterFlowers = Weighted(7, 17, 24, 24, 24, 29, 29);
                    break;
                case 2:
                    desertTrees = lushStyle ? Weighted(5, 13, 13, 30, 30) : Weighted(5, 13, 13, 30);
                    desertCactus = sparseStyle ? Weighted(1, 14, 15, 16, 16, 16) : Weighted(1, 14, 15, 16, 16);
                    desertDeadTrees = Weighted(10, 11, 12, 23, 28);
                    desertPlants = Weighted(7, 25, 27, 27, 27);
                    desertWaterPlants = Weighted(7, 7, 7, 25, 27, 27, 27, 27);
                    desertStones = Weighted(2, 3, 4, 6, 8, 18, 19, 20, 21);
                    desertFlowers = Weighted(9, 17, 24, 24, 26, 26, 31);
                    desertWaterFlowers = Weighted(7, 17, 24, 24, 24, 29, 29, 29);
                    break;
                case 3:
                    desertTrees = lushStyle ? Weighted(5, 13, 13, 30, 30, 30) : Weighted(5, 13, 13, 30, 30);
                    desertCactus = sparseStyle ? Weighted(1, 14, 15, 16, 16) : Weighted(1, 14, 15, 16);
                    desertDeadTrees = Weighted(10, 11, 12, 23, 28);
                    desertPlants = Weighted(7, 25, 27, 27, 27, 27);
                    desertWaterPlants = Weighted(7, 7, 7, 7, 25, 27, 27, 27, 27);
                    desertStones = Weighted(2, 3, 4, 6, 8, 18, 19, 20);
                    desertFlowers = Weighted(9, 17, 24, 24, 26, 26, 31, 31);
                    desertWaterFlowers = Weighted(7, 17, 24, 24, 24, 29, 29, 29);
                    break;
                default:
                    desertTrees = lushStyle ? Weighted(5, 13, 13, 30, 30, 30, 30) : Weighted(5, 13, 13, 30, 30, 30);
                    desertCactus = Weighted(1, 14, 15, 16);
                    desertDeadTrees = Weighted(10, 11, 12, 23, 28);
                    desertPlants = Weighted(7, 25, 27, 27, 27, 27, 27);
                    desertWaterPlants = Weighted(7, 7, 7, 7, 7, 25, 27, 27, 27, 27, 27);
                    desertStones = Weighted(2, 3, 4, 6, 8, 18, 19);
                    desertFlowers = Weighted(9, 17, 24, 24, 26, 26, 31, 31, 31);
                    desertWaterFlowers = Weighted(7, 17, 24, 24, 24, 29, 29, 29);
                    break;
            }
        }

        private void ConfigureHauntedWoodlandLists(int tier, bool spring, bool summer, bool winter, bool lushStyle, bool sparseStyle)
        {
            switch (tier)
            {
                case 0:
                    hauntedWoodlandTrees = sparseStyle ? Weighted(13, 15, 31) : Weighted(13, 13, 15, 18, 31);
                    hauntedWoodlandDirtTrees = Weighted(18, 19, 20, 31, 31);
                    hauntedWoodlandDeadTrees = Weighted(16, 18, 24, 25, 30, 31, 31, 31);
                    hauntedWoodlandBushes = Weighted(2, 26, 27);
                    hauntedWoodlandPlants = Weighted(7, 14, 17, 29, 31);
                    hauntedWoodlandRocks = Weighted(1, 3, 4, 5, 6, 8, 10, 12, 12);
                    hauntedWoodlandBones = Weighted(11, 11, 11, 31);
                    hauntedWoodlandFlowers = Weighted(21, 21);
                    hauntedWoodlandMushroom = Weighted(22, 22, 23);
                    hauntedWoodlandBeach = Weighted(31, 8, 9, 14, 29, 31, 31);
                    break;
                case 1:
                    hauntedWoodlandTrees = sparseStyle ? Weighted(13, 15, 31) : Weighted(13, 13, 15, 15, 18, 31);
                    hauntedWoodlandDirtTrees = Weighted(18, 19, 20, 31);
                    hauntedWoodlandDeadTrees = Weighted(16, 18, 24, 25, 30, 31, 31);
                    hauntedWoodlandBushes = Weighted(2, 26, 27, 28);
                    hauntedWoodlandPlants = Weighted(7, 14, 17, 29, 31, 31);
                    hauntedWoodlandRocks = Weighted(1, 3, 4, 5, 6, 8, 10, 12);
                    hauntedWoodlandBones = Weighted(11, 11, 31);
                    hauntedWoodlandFlowers = Weighted(21, 21);
                    hauntedWoodlandMushroom = Weighted(22, 22, 23);
                    hauntedWoodlandBeach = Weighted(31, 8, 9, 14, 29, 31);
                    break;
                case 2:
                    hauntedWoodlandTrees = lushStyle ? Weighted(13, 13, 15, 15, 18, 18, 31) : Weighted(13, 13, 15, 18, 31);
                    hauntedWoodlandDirtTrees = Weighted(18, 19, 20, 31);
                    hauntedWoodlandDeadTrees = Weighted(16, 18, 24, 25, 30, 31, 31);
                    hauntedWoodlandBushes = Weighted(2, 26, 27, 28, 28);
                    hauntedWoodlandPlants = Weighted(7, 14, 17, 17, 29, 31);
                    hauntedWoodlandRocks = Weighted(1, 3, 4, 5, 6, 8, 10, 12);
                    hauntedWoodlandBones = Weighted(11, 31);
                    hauntedWoodlandFlowers = Weighted(21, 21, 21);
                    hauntedWoodlandMushroom = Weighted(22, 22, 23, 23);
                    hauntedWoodlandBeach = Weighted(31, 8, 9, 14, 29, 31);
                    break;
                case 3:
                    hauntedWoodlandTrees = lushStyle ? Weighted(13, 13, 13, 15, 15, 18, 18, 31) : Weighted(13, 13, 15, 18, 18, 31);
                    hauntedWoodlandDirtTrees = Weighted(18, 19, 20, 31, 31);
                    hauntedWoodlandDeadTrees = Weighted(16, 18, 24, 25, 30, 31, 31);
                    hauntedWoodlandBushes = Weighted(2, 26, 27, 27, 28, 28);
                    hauntedWoodlandPlants = Weighted(7, 14, 17, 17, 29, 31, 31);
                    hauntedWoodlandRocks = Weighted(1, 3, 4, 5, 6, 8, 10);
                    hauntedWoodlandBones = Weighted(11, 31);
                    hauntedWoodlandFlowers = Weighted(21, 21, 21);
                    hauntedWoodlandMushroom = Weighted(22, 22, 23, 23);
                    hauntedWoodlandBeach = Weighted(31, 8, 9, 14, 29, 31);
                    break;
                default:
                    hauntedWoodlandTrees = lushStyle ? Weighted(13, 13, 13, 15, 15, 18, 18, 18, 31) : Weighted(13, 13, 15, 18, 18, 31);
                    hauntedWoodlandDirtTrees = Weighted(18, 19, 20, 31);
                    hauntedWoodlandDeadTrees = Weighted(16, 18, 24, 25, 30, 31, 31);
                    hauntedWoodlandBushes = Weighted(2, 26, 27, 27, 28, 28);
                    hauntedWoodlandPlants = Weighted(7, 14, 17, 17, 29, 31, 31);
                    hauntedWoodlandRocks = Weighted(1, 3, 4, 5, 6, 8, 10);
                    hauntedWoodlandBones = Weighted(11, 31);
                    hauntedWoodlandFlowers = Weighted(21, 21, 21);
                    hauntedWoodlandMushroom = Weighted(22, 22, 23, 23);
                    hauntedWoodlandBeach = Weighted(31, 8, 9, 14, 29, 31);
                    break;
            }
        }

        private void ConfigureRainforestLists(int tier, bool spring, bool summer, bool winter, bool lushStyle, bool sparseStyle)
        {
            switch (tier)
            {
                case 0:
                    rainforestTrees = lushStyle ? Weighted(12, 12, 13, 13, 14, 15, 30, 30, 30) : Weighted(12, 12, 13, 13, 14, 15, 30, 30);
                    rainforestBushes = Weighted(3, 9, 16, 18);
                    rainforestPlants = Weighted(2, 5, 10, 11, 23, 24);
                    rainforestRocks = Weighted(1, 4, 17, 19, 25, 25);
                    rainforestFlowers = Weighted(6, 20, 21, 22, 26, 27);
                    rainforestEggs = Weighted(28, 29, 31);
                    rainforestBeach = Weighted();
                    break;
                case 1:
                    rainforestTrees = lushStyle ? Weighted(12, 13, 13, 14, 15, 15, 30, 30, 30) : Weighted(12, 13, 13, 14, 15, 30, 30);
                    rainforestBushes = Weighted(3, 9, 16, 18, 18);
                    rainforestPlants = Weighted(2, 5, 10, 11, 23, 24, 24);
                    rainforestRocks = Weighted(1, 4, 17, 19, 25);
                    rainforestFlowers = Weighted(6, 20, 21, 22, 26, 27);
                    rainforestEggs = Weighted(28, 29, 29, 31);
                    rainforestBeach = Weighted();
                    break;
                case 2:
                    rainforestTrees = lushStyle ? Weighted(12, 13, 13, 14, 14, 15, 15, 30, 30) : Weighted(12, 13, 14, 14, 15, 15, 30);
                    rainforestBushes = Weighted(3, 9, 16, 18, 18);
                    rainforestPlants = Weighted(2, 5, 10, 11, 23, 24, 24, 24);
                    rainforestRocks = Weighted(1, 4, 17, 19, 25);
                    rainforestFlowers = Weighted(6, 20, 21, 22, 26, 27, 27);
                    rainforestEggs = Weighted(28, 29, 29, 31, 31);
                    rainforestBeach = Weighted();
                    break;
                case 3:
                    rainforestTrees = lushStyle ? Weighted(12, 13, 14, 14, 15, 15, 30, 30) : Weighted(12, 13, 14, 15, 15, 30);
                    rainforestBushes = Weighted(3, 9, 16, 18, 18, 18);
                    rainforestPlants = Weighted(2, 5, 10, 11, 23, 24, 24, 24);
                    rainforestRocks = Weighted(1, 4, 17, 19);
                    rainforestFlowers = Weighted(6, 20, 21, 22, 26, 27, 27);
                    rainforestEggs = Weighted(28, 29, 29, 31, 31);
                    rainforestBeach = Weighted();
                    break;
                default:
                    rainforestTrees = sparseStyle ? Weighted(12, 13, 14, 15, 30, 30) : Weighted(12, 13, 14, 14, 15, 15, 30, 30);
                    rainforestBushes = Weighted(3, 9, 16, 18, 18, 18);
                    rainforestPlants = Weighted(2, 5, 10, 11, 23, 24, 24, 24, 24);
                    rainforestRocks = Weighted(1, 4, 17, 19);
                    rainforestFlowers = Weighted(6, 20, 21, 22, 26, 27, 27, 27);
                    rainforestEggs = Weighted(28, 29, 29, 31, 31, 31);
                    rainforestBeach = Weighted();
                    break;
            }
        }

        private void ConfigureSubtropicalLists(int tier, bool spring, bool summer, bool winter, bool lushStyle, bool sparseStyle)
        {
            switch (tier)
            {
                case 0:
                    subtropicalTrees = Weighted(5, 11, 12, 13, 25, 30);
                    subtropicalBushes = Weighted(9, 26, 27);
                    subtropicalPlants = Weighted(7, 23, 25, 27, 29);
                    subtropicalFlowers = Weighted(2, 7, 21, 22, 24, 26);
                    subtropicalSucculents = Weighted(1, 14, 15, 16, 16, 25);
                    subtropicalRocks = Weighted(1, 3, 4, 6, 8, 10, 17, 18, 28, 28);
                    subtropicalDeadTrees = Weighted(10, 11, 12, 19, 20, 23, 28, 28);
                    break;
                case 1:
                    subtropicalTrees = Weighted(5, 11, 13, 14, 15, 24, 30);
                    subtropicalBushes = Weighted(9, 26, 27, 28);
                    subtropicalPlants = Weighted(7, 23, 25, 27, 29, 31);
                    subtropicalFlowers = Weighted(2, 7, 21, 22, 24, 26);
                    subtropicalSucculents = Weighted(1, 14, 15, 16, 25);
                    subtropicalRocks = Weighted(1, 3, 4, 6, 8, 10, 17, 18, 28);
                    subtropicalDeadTrees = Weighted(10, 11, 12, 19, 20, 23, 28);
                    break;
                case 2:
                    subtropicalTrees = lushStyle ? Weighted(11, 13, 14, 15, 15, 24, 30, 30) : Weighted(11, 13, 14, 15, 24, 30);
                    subtropicalBushes = Weighted(9, 26, 27, 27, 28);
                    subtropicalPlants = Weighted(7, 23, 25, 27, 29, 31);
                    subtropicalFlowers = Weighted(2, 7, 21, 22, 24, 26, 26);
                    subtropicalSucculents = sparseStyle ? Weighted(1, 14, 15, 16, 25, 25) : Weighted(1, 14, 15, 16, 25);
                    subtropicalRocks = Weighted(1, 3, 4, 6, 8, 10, 17, 18);
                    subtropicalDeadTrees = Weighted(10, 11, 12, 19, 20, 23, 28);
                    break;
                case 3:
                    subtropicalTrees = lushStyle ? Weighted(11, 12, 13, 14, 15, 15, 24, 30, 30) : Weighted(11, 12, 13, 14, 15, 24, 30);
                    subtropicalBushes = Weighted(9, 26, 27, 27, 28, 28);
                    subtropicalPlants = Weighted(7, 23, 25, 27, 29, 31, 31);
                    subtropicalFlowers = Weighted(2, 7, 21, 22, 24, 26, 26);
                    subtropicalSucculents = sparseStyle ? Weighted(1, 14, 15, 16, 25) : Weighted(1, 14, 15, 16);
                    subtropicalRocks = Weighted(1, 3, 4, 6, 8, 10, 17);
                    subtropicalDeadTrees = Weighted(10, 11, 12, 19, 20, 23, 28);
                    break;
                default:
                    subtropicalTrees = lushStyle ? Weighted(12, 13, 14, 14, 15, 15, 24, 30, 30) : Weighted(12, 13, 14, 15, 24, 30);
                    subtropicalBushes = Weighted(9, 26, 27, 27, 28, 28);
                    subtropicalPlants = Weighted(7, 23, 25, 27, 29, 31, 31);
                    subtropicalFlowers = Weighted(2, 7, 21, 22, 24, 26, 26, 26);
                    subtropicalSucculents = Weighted(1, 14, 15, 16);
                    subtropicalRocks = Weighted(1, 3, 4, 6, 8, 10, 17);
                    subtropicalDeadTrees = Weighted(10, 11, 12, 19, 20, 23, 28);
                    break;
            }
        }

        private void ConfigureSwampLists(int tier, bool spring, bool summer, bool winter, bool lushStyle, bool sparseStyle)
        {
            switch (tier)
            {
                case 0:
                    swampTrees = Weighted(12, 13, 14, 15, 30);
                    swampDeadTrees = Weighted(16, 18, 19, 20, 24, 25, 29, 30, 31, 31);
                    swampBushes = Weighted(3, 9, 16, 18, 26, 27);
                    swampPlants = Weighted(2, 5, 7, 10, 11, 23, 24, 27);
                    swampWaterPlants = Weighted(7, 7, 25, 27, 29);
                    swampFlowers = Weighted(7, 21, 22, 24, 29, 29);
                    swampWaterFlowers = Weighted(7, 17, 24, 24, 29, 29, 29);
                    swampRocks = Weighted(1, 4, 17, 19, 25, 31, 31);
                    swampBones = Weighted(11, 31);
                    break;
                case 1:
                    swampTrees = Weighted(12, 13, 14, 15, 18, 30);
                    swampDeadTrees = Weighted(16, 18, 19, 20, 24, 25, 29, 30, 31, 31);
                    swampBushes = Weighted(3, 9, 16, 18, 26, 27, 28);
                    swampPlants = Weighted(2, 5, 7, 10, 11, 23, 24, 27, 27);
                    swampWaterPlants = Weighted(7, 7, 25, 27, 27, 29);
                    swampFlowers = Weighted(7, 21, 22, 24, 29, 29);
                    swampWaterFlowers = Weighted(7, 17, 24, 24, 29, 29, 29);
                    swampRocks = Weighted(1, 4, 17, 19, 25, 31);
                    swampBones = Weighted(11, 31);
                    break;
                case 2:
                    swampTrees = lushStyle ? Weighted(12, 13, 14, 15, 18, 30, 30) : Weighted(12, 13, 14, 15, 18, 30);
                    swampDeadTrees = Weighted(16, 18, 19, 20, 24, 25, 29, 30, 31);
                    swampBushes = Weighted(3, 9, 16, 18, 18, 26, 27, 28);
                    swampPlants = Weighted(2, 5, 7, 10, 11, 23, 24, 24, 27);
                    swampWaterPlants = Weighted(7, 7, 25, 27, 27, 29, 29);
                    swampFlowers = Weighted(7, 21, 22, 24, 24, 29, 29);
                    swampWaterFlowers = Weighted(7, 17, 24, 24, 24, 29, 29, 29);
                    swampRocks = Weighted(1, 4, 17, 19, 25, 31);
                    swampBones = Weighted(11, 31);
                    break;
                case 3:
                    swampTrees = lushStyle ? Weighted(12, 13, 14, 15, 15, 18, 30, 30) : Weighted(12, 13, 14, 15, 18, 30);
                    swampDeadTrees = Weighted(16, 18, 19, 20, 24, 25, 29, 30, 31);
                    swampBushes = Weighted(3, 9, 16, 18, 18, 26, 27, 28);
                    swampPlants = Weighted(2, 5, 7, 10, 11, 23, 24, 24, 27, 27);
                    swampWaterPlants = Weighted(7, 7, 25, 27, 27, 29, 29, 29);
                    swampFlowers = Weighted(7, 21, 22, 24, 24, 29, 29);
                    swampWaterFlowers = Weighted(7, 17, 24, 24, 24, 29, 29, 29);
                    swampRocks = Weighted(1, 4, 17, 19, 25);
                    swampBones = Weighted(11, 31);
                    break;
                default:
                    swampTrees = lushStyle ? Weighted(12, 13, 14, 15, 15, 18, 30, 30, 30) : Weighted(12, 13, 14, 15, 18, 30, 30);
                    swampDeadTrees = Weighted(16, 18, 19, 20, 24, 25, 29, 30, 31);
                    swampBushes = Weighted(3, 9, 16, 18, 18, 26, 27, 28, 28);
                    swampPlants = Weighted(2, 5, 7, 10, 11, 23, 24, 24, 27, 27);
                    swampWaterPlants = Weighted(7, 7, 7, 25, 27, 27, 29, 29, 29);
                    swampFlowers = Weighted(7, 21, 22, 24, 24, 29, 29, 29);
                    swampWaterFlowers = Weighted(7, 17, 24, 24, 24, 29, 29, 29, 29);
                    swampRocks = Weighted(1, 4, 17, 19, 25);
                    swampBones = Weighted(11, 31);
                    break;
            }
        }

        public void ChangeVegetationLists(float elevation, DaggerfallDateTime.Seasons season, float mapStyle) {

            float rnd;

            elevationLevels = new List<float>(new float[] {
                Random.Range(0.66f,0.64f), Random.Range(0.61f,0.59f), Random.Range(0.56f,0.54f),
                Random.Range(0.51f,0.49f), Random.Range(0.46f,0.44f), Random.Range(0.41f,0.39f),
                Random.Range(0.36f, 0.34f), Random.Range(0.31f,0.29f), Random.Range(0.26f,0.24f),
                Random.Range(0.21f,0.19f), Random.Range(0.11f,0.09f), Random.Range(0.096f,0.84f),
                Random.Range(0.071f,0.069f), Random.Range(0.041f,0.039f)});

            ConfigureAdditionalBiomeLists(elevation, season, mapStyle);

            if (season == DaggerfallDateTime.Seasons.Winter) {
                if (elevation > elevationLevels[0]) {
                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation0;
                    temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation0;
                    temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation0;
                    temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation0;
                    temperateWoodlandBushes = temperateWoodlandBushesAtElevation0;
                    temperateWoodlandBeach = temperateWoodlandBeachAtElevation0;
                    temperateWoodlandRocks = temperateWoodlandRocksAtElevation0;

                    mountainsTrees = new List<int>(new int[] { 29 });
                    mountainsNeedleTrees = new List<int>(new int[] { 11, 25 });
                    mountainsDeadTrees = new List<int>(new int[] { 19, 20, 24, 30, 29 });
                    mountainsFlowers = new List<int>(new int[] { 7 });
                    mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                    mountainsGrass = new List<int>(new int[] { 7, 7, 7, 9, 26 });

                    woodlandHillsFlowers = new List<int>(new int[] { 7, 22, 22, 22, 22 });
                    woodlandHillsBushes = new List<int>(new int[] { 23, 27 });
                    woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 14, 25, 12 });
                    woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 12 });
                    woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 31 });
                } else {
                    if (elevation > elevationLevels[1]) {
                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation1;
                        temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation1;
                        temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation1;
                        temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation1;
                        temperateWoodlandBushes = temperateWoodlandBushesAtElevation1;
                        temperateWoodlandBeach = temperateWoodlandBeachAtElevation1;
                        temperateWoodlandRocks = temperateWoodlandRocksAtElevation1;

                        mountainsTrees = new List<int>(new int[] { 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 11, 25, 25 });
                        mountainsNeedleTrees = new List<int>(new int[] { 11, 25, 25 });
                        mountainsDeadTrees = new List<int>(new int[] { 19, 20, 24, 30, 29 });
                        mountainsFlowers = new List<int>(new int[] { 7 });
                        mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                        mountainsGrass = new List<int>(new int[] { 7, 7, 7, 9, 26 });

                        woodlandHillsFlowers = new List<int>(new int[] { 7, 22, 22, 22, 22 });
                        woodlandHillsBushes = new List<int>(new int[] { 23, 27 });
                        woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 14, 25, 12 });
                        woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 12 });
                        woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 31 });
                    } else {
                        if (elevation > elevationLevels[2]) {
                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation2;
                            temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation2;
                            temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation2;
                            temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation2;
                            temperateWoodlandBushes = temperateWoodlandBushesAtElevation2;
                            temperateWoodlandBeach = temperateWoodlandBeachAtElevation2;
                            temperateWoodlandRocks = temperateWoodlandRocksAtElevation2;

                            mountainsTrees = new List<int>(new int[] { 29, 29, 29, 29, 29, 29, 29, 11, 25, 25, 25 });
                            mountainsNeedleTrees = new List<int>(new int[] { 11, 25, 25 });
                            mountainsDeadTrees = new List<int>(new int[] { 19, 20, 24, 30, 29 });
                            mountainsFlowers = new List<int>(new int[] { 7 });
                            mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                            mountainsGrass = new List<int>(new int[] { 7, 7, 7, 9, 23, 26 });

                            woodlandHillsFlowers = new List<int>(new int[] { 7, 22, 22, 22, 22 });
                            woodlandHillsBushes = new List<int>(new int[] { 23, 27 });
                            woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 14, 25, 12 });
                            woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 12 });
                            woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 31 });
                        } else {
                            if (elevation > elevationLevels[3]) {
                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation3;
                                temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation3;
                                temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation3;
                                temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation3;
                                temperateWoodlandBushes = temperateWoodlandBushesAtElevation3;
                                temperateWoodlandBeach = temperateWoodlandBeachAtElevation3;
                                temperateWoodlandRocks = temperateWoodlandRocksAtElevation3;

                                mountainsTrees = new List<int>(new int[] { 29, 29, 29, 29, 29, 29, 29, 11, 25, 25 });
                                mountainsNeedleTrees = new List<int>(new int[] { 11, 25, 25 });
                                mountainsDeadTrees = new List<int>(new int[] { 19, 20, 24, 30, 29 });
                                mountainsFlowers = new List<int>(new int[] { 7 });
                                mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                mountainsGrass = new List<int>(new int[] { 7, 7, 7, 9, 23, 26 });

                                woodlandHillsFlowers = new List<int>(new int[] { 7, 22, 22, 22, 22 });
                                woodlandHillsBushes = new List<int>(new int[] { 23, 27 });
                                woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 14, 25, 12 });
                                woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 12 });
                                woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 31 });
                            } else {
                                if (elevation > elevationLevels[4]) {
                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation4;
                                    temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation4;
                                    temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation4;
                                    temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation4;
                                    temperateWoodlandBushes = temperateWoodlandBushesAtElevation4;
                                    temperateWoodlandBeach = temperateWoodlandBeachAtElevation4;
                                    temperateWoodlandRocks = temperateWoodlandRocksAtElevation4;

                                    mountainsTrees = new List<int>(new int[] { 29, 29, 29, 29, 29, 5, 11, 25, 25 });
                                    mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 11, 25, 25, 25 });
                                    mountainsDeadTrees = new List<int>(new int[] { 19, 20, 24, 30, 29 });
                                    mountainsFlowers = new List<int>(new int[] { 7 });
                                    mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                    mountainsGrass = new List<int>(new int[] { 7, 7, 7, 9, 23, 26 });

                                    woodlandHillsFlowers = new List<int>(new int[] { 7, 22, 22, 22, 22 });
                                    woodlandHillsBushes = new List<int>(new int[] { 23, 27 });
                                    woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 14, 25, 12 });
                                    woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 12 });
                                    woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 31 });
                                } else {
                                    if (elevation > elevationLevels[5]) {
                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation5;
                                        temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation5;
                                        temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation5;
                                        temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation5;
                                        temperateWoodlandBushes = temperateWoodlandBushesAtElevation5;
                                        temperateWoodlandBeach = temperateWoodlandBeachAtElevation5;
                                        temperateWoodlandRocks = temperateWoodlandRocksAtElevation5;

                                        mountainsTrees = new List<int>(new int[] { 29, 29, 29, 29, 29, 5, 11, 13, 21, 25, 25, 25 });
                                        mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 11, 25, 25, 25 });
                                        mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 24, 30, 29 });
                                        mountainsFlowers = new List<int>(new int[] { 7 });
                                        mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                        mountainsGrass = new List<int>(new int[] { 7, 7, 9, 23, 23, 23, 26 });

                                        woodlandHillsFlowers = new List<int>(new int[] { 7, 22, 22, 22, 22 });
                                        woodlandHillsBushes = new List<int>(new int[] { 23, 27 });
                                        woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 14, 25, 12 });
                                        woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 12 });
                                        woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 31 });
                                    } else {
                                        if (elevation > elevationLevels[6]) {
                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation6;
                                            temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation6;
                                            temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation6;
                                            temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation6;
                                            temperateWoodlandBushes = temperateWoodlandBushesAtElevation6;
                                            temperateWoodlandBeach = temperateWoodlandBeachAtElevation6;
                                            temperateWoodlandRocks = temperateWoodlandRocksAtElevation6;

                                            mountainsTrees = new List<int>(new int[] { 29, 29, 29, 5, 11, 11, 13, 21, 25, 25, 25 });
                                            mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 11, 25, 25, 25 });
                                            mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 24, 30, 29 });
                                            mountainsFlowers = new List<int>(new int[] { 7 });
                                            mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                            mountainsGrass = new List<int>(new int[] { 7, 7, 9, 23, 23, 23, 26 });

                                            woodlandHillsFlowers = new List<int>(new int[] { 7, 22, 22, 22, 22 });
                                            woodlandHillsBushes = new List<int>(new int[] { 23, 27 });
                                            woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 14, 25, 12 });
                                            woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 12 });
                                            woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 31 });
                                        } else {
                                            if (elevation > elevationLevels[7]) {
                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation7;
                                                temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation7;
                                                temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation7;
                                                temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation7;
                                                temperateWoodlandBushes = temperateWoodlandBushesAtElevation7;
                                                temperateWoodlandBeach = temperateWoodlandBeachAtElevation7;
                                                temperateWoodlandRocks = temperateWoodlandRocksAtElevation7;

                                                mountainsTrees = new List<int>(new int[] { 29, 29, 29, 5, 11, 11, 13, 21, 25, 25, 25 });
                                                mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 11, 25, 25, 25 });
                                                mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 24, 30, 29 });
                                                mountainsFlowers = new List<int>(new int[] { 7 });
                                                mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                mountainsGrass = new List<int>(new int[] { 7, 7, 9, 23, 23, 23, 26 });

                                                woodlandHillsFlowers = new List<int>(new int[] { 7, 22, 22, 22, 22 });
                                                woodlandHillsBushes = new List<int>(new int[] { 23, 23, 27 });
                                                woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 14, 25, 12 });
                                                woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 12 });
                                                woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 31 });
                                            } else {
                                                if (elevation > elevationLevels[8]) {
                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation8;
                                                    temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation8;
                                                    temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation8;
                                                    temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation8;
                                                    temperateWoodlandBushes = temperateWoodlandBushesAtElevation8;
                                                    temperateWoodlandBeach = temperateWoodlandBeachAtElevation8;
                                                    temperateWoodlandRocks = temperateWoodlandRocksAtElevation8;

                                                    mountainsTrees = new List<int>(new int[] { 29, 29, 5, 11, 11, 13, 21, 25, 25 });
                                                    mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 11, 25, 25 });
                                                    mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 24, 30, 29 });
                                                    mountainsFlowers = new List<int>(new int[] { 7 });
                                                    mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                    mountainsGrass = new List<int>(new int[] { 7, 7, 9, 23, 23, 26 });

                                                    woodlandHillsFlowers = new List<int>(new int[] { 7, 22, 22, 22 });
                                                    woodlandHillsBushes = new List<int>(new int[] { 26, 27 });
                                                    woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 14, 25, 12 });
                                                    woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 12 });
                                                    woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 9, 31 });
                                                } else {
                                                    if (elevation > elevationLevels[9]) {
                                                        rnd = mapStyle + Random.Range(-10f,10f);
                                                        if (rnd <= 25)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9F;
                                                        if (rnd > 25)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9A;
                                                        if (rnd > 30)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9B;
                                                        if (rnd > 35)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9C;
                                                        if (rnd > 41)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9D;
                                                        if (rnd > 47)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9E;
                                                        if (rnd > 55)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9F;
                                                        if (rnd > 60)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation10A;
                                                        temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation9;
                                                        temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation9;
                                                        temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation9;
                                                        temperateWoodlandBushes = temperateWoodlandBushesAtElevation9;
                                                        temperateWoodlandBeach = temperateWoodlandBeachAtElevation9;
                                                        temperateWoodlandRocks = temperateWoodlandRocksAtElevation9;

                                                        mountainsTrees = new List<int>(new int[] { 29, 5, 11, 11, 13, 24, 21, 25, 25 });
                                                        mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 11, 25, 25, 25 });
                                                        mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 24, 30, 29 });
                                                        mountainsFlowers = new List<int>(new int[] { 7 });
                                                        mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                        mountainsGrass = new List<int>(new int[] { 7, 7, 9, 23, 23, 26 });

                                                        woodlandHillsFlowers = new List<int>(new int[] { 7, 21, 22, 22 });
                                                        woodlandHillsBushes = new List<int>(new int[] { 27 });
                                                        woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 13, 14, 14, 16, 25, 12 });
                                                        woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 11, 12, 25, 12 });
                                                        woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 9, 31 });
                                                    } else {
                                                        if (elevation > elevationLevels[10]) {
                                                            rnd = mapStyle + Random.Range(-10f,10f);
                                                            if (rnd <= 25)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10F;
                                                            if (rnd > 25)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10A;
                                                            if (rnd > 30)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10B;
                                                            if (rnd > 35)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10C;
                                                            if (rnd > 41)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10D;
                                                            if (rnd > 47)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10E;
                                                            if (rnd > 55)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10F;
                                                            if (rnd > 60)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10A;
                                                            temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation10;
                                                            temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation10;
                                                            temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation10;
                                                            temperateWoodlandBushes = temperateWoodlandBushesAtElevation10;
                                                            temperateWoodlandBeach = temperateWoodlandBeachAtElevation10;
                                                            temperateWoodlandRocks = temperateWoodlandRocksAtElevation10;

                                                            mountainsTrees = new List<int>(new int[] { 5, 5, 11, 11, 11, 13, 13, 24, 24, 24, 21, 25 });
                                                            mountainsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 11, 25 });
                                                            mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 30, 30, 29, 29 });
                                                            mountainsFlowers = new List<int>(new int[] { 7 });
                                                            mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                            mountainsGrass = new List<int>(new int[] { 7, 9, 9, 23, 26, 26 });

                                                            woodlandHillsFlowers = new List<int>(new int[] { 7, 21, 21, 21, 22, 22 });
                                                            woodlandHillsBushes = new List<int>(new int[] { 23, 27, 27 });
                                                            woodlandHillsTrees = new List<int>(new int[] { 12, 12, 13, 14, 14, 16 });
                                                            woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 25, 12 });
                                                            woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 9, 31 });
                                                        } else {
                                                            if (elevation > elevationLevels[11]) {
                                                                rnd = mapStyle + Random.Range(-10f,10f);
                                                                if (rnd <= 25)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11F;
                                                                if (rnd > 25)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11A;
                                                                if (rnd > 30)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11B;
                                                                if (rnd > 35)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11C;
                                                                if (rnd > 41)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11D;
                                                                if (rnd > 47)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11E;
                                                                if (rnd > 55)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11F;
                                                                if (rnd > 60)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11A;
                                                                temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation11;
                                                                temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation11;
                                                                temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation11;
                                                                temperateWoodlandBushes = temperateWoodlandBushesAtElevation11;
                                                                temperateWoodlandBeach = temperateWoodlandBeachAtElevation11;
                                                                temperateWoodlandRocks = temperateWoodlandRocksAtElevation11;

                                                                mountainsTrees = new List<int>(new int[] { 5, 5, 11, 11, 11, 13, 13, 24, 24, 24, 21, 25 });
                                                                mountainsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 11, 11, 25 });
                                                                mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 30, 30, 29, 29 });
                                                                mountainsFlowers = new List<int>(new int[] { 7 });
                                                                mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                                mountainsGrass = new List<int>(new int[] { 7, 9, 9, 23, 26, 26 });

                                                                woodlandHillsFlowers = new List<int>(new int[] { 7, 21, 22 });
                                                                woodlandHillsBushes = new List<int>(new int[] { 27, 31 });
                                                                woodlandHillsTrees = new List<int>(new int[] { 12, 12, 13, 13, 14, 14, 14, 16, 16 });
                                                                woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 11, 12, 25, 12 });
                                                                woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 9 });
                                                            } else {
                                                                if (elevation > elevationLevels[12]) {
                                                                    rnd = mapStyle + Random.Range(-10f,10f);
                                                                    if (rnd <= 25)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12F;
                                                                    if (rnd > 25)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12A;
                                                                    if (rnd > 30)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12B;
                                                                    if (rnd > 35)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12C;
                                                                    if (rnd > 41)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12D;
                                                                    if (rnd > 47)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12E;
                                                                    if (rnd > 55)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12F;
                                                                    if (rnd > 60)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12A;
                                                                    temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation12;
                                                                    temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation12;
                                                                    temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation12;
                                                                    temperateWoodlandBushes = temperateWoodlandBushesAtElevation12;
                                                                    temperateWoodlandBeach = temperateWoodlandBeachAtElevation12;
                                                                    temperateWoodlandRocks = temperateWoodlandRocksAtElevation12;

                                                                    mountainsTrees = new List<int>(new int[] { 5, 5, 11, 11, 11, 13, 13, 24, 24, 24, 21, 25 });
                                                                    mountainsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 11, 25 });
                                                                    mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 30, 30, 29, 29 });
                                                                    mountainsFlowers = new List<int>(new int[] { 7 });
                                                                    mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                                    mountainsGrass = new List<int>(new int[] { 7, 9, 9, 23, 26, 26 });

                                                                    woodlandHillsFlowers = new List<int>(new int[] { 7, 21, 21, 21 });
                                                                    woodlandHillsBushes = new List<int>(new int[] { 27, 27 });
                                                                    woodlandHillsTrees = new List<int>(new int[] { 12, 13, 13, 13, 14, 14, 14, 16, 16 });
                                                                    woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 12 });
                                                                    woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 9, 31 });
                                                                } else {
                                                                    if (elevation > elevationLevels[13]) {
                                                                        rnd = mapStyle + Random.Range(-10f,10f);
                                                                        if (rnd <= 25)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13F;
                                                                        if (rnd > 25)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13A;
                                                                        if (rnd > 30)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13B;
                                                                        if (rnd > 35)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13C;
                                                                        if (rnd > 41)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13D;
                                                                        if (rnd > 47)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13E;
                                                                        if (rnd > 55)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13F;
                                                                        if (rnd > 60)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13A;
                                                                        temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation13;
                                                                        temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation13;
                                                                        temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation13;
                                                                        temperateWoodlandBushes = temperateWoodlandBushesAtElevation13;
                                                                        temperateWoodlandBeach = temperateWoodlandBeachAtElevation13;
                                                                        temperateWoodlandRocks = temperateWoodlandRocksAtElevation13;

                                                                        mountainsTrees = new List<int>(new int[] { 5, 5, 11, 11, 11, 13, 13, 24, 24, 24, 21, 25 });
                                                                        mountainsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 11, 11, 25 });
                                                                        mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 30, 30, 29, 29 });
                                                                        mountainsFlowers = new List<int>(new int[] { 7 });
                                                                        mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                                        mountainsGrass = new List<int>(new int[] { 7, 9, 9, 9, 23, 26, 26, 26 });

                                                                        woodlandHillsFlowers = new List<int>(new int[] { 7, 21, 21 });
                                                                        woodlandHillsBushes = new List<int>(new int[] { 27, 27, 27, 31 });
                                                                        woodlandHillsTrees = new List<int>(new int[] { 13, 14, 14, 14, 16 });
                                                                        woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 11, 11 });
                                                                        woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 9 });
                                                                    } else {
                                                                        rnd = mapStyle + Random.Range(-15f,15f);
                                                                        if (rnd <= 18)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14D;
                                                                        if (rnd > 18)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14A;
                                                                        if (rnd > 30)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14B;
                                                                        if (rnd > 42)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14C;
                                                                        if (rnd > 55)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14D;
                                                                        if (rnd > 67)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14A;
                                                                        temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation14;
                                                                        temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation14;
                                                                        temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation14;
                                                                        temperateWoodlandBushes = temperateWoodlandBushesAtElevation14;
                                                                        temperateWoodlandBeach = temperateWoodlandBeachAtElevation14;
                                                                        temperateWoodlandRocks = temperateWoodlandRocksAtElevation14;

                                                                        mountainsTrees = new List<int>(new int[] { 5, 5, 23, 23, 11, 11, 11, 13, 13, 24, 24, 24, 21, 25 });
                                                                        mountainsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 11, 25 });
                                                                        mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 30, 30, 29, 29 });
                                                                        mountainsFlowers = new List<int>(new int[] { 7 });
                                                                        mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 9, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                                        mountainsBeach = new List<int>(new int[] { 8, 26, 29, 7, 7, 7, 23, 23 });
                                                                        mountainsGrass = new List<int>(new int[] { 7, 9, 9, 9, 23, 26, 26, 26 });

                                                                        woodlandHillsFlowers = new List<int>(new int[] { 7, 7, 9, 9, 21, 26, 26 });
                                                                        woodlandHillsBushes = new List<int>(new int[] { 27, 27, 31 });
                                                                        woodlandHillsTrees = new List<int>(new int[] { 13, 13, 14, 16, 16 });
                                                                        woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 11, 11 });
                                                                        woodlandHillsDirtPlants = new List<int>(new int[] { 22, 29, 26, 9 });
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } else {
                if (elevation > elevationLevels[0]) {
                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation0;
                    temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation0;
                    temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation0;
                    temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation0;
                    temperateWoodlandBushes = temperateWoodlandBushesAtElevation0;
                    temperateWoodlandBeach = temperateWoodlandBeachAtElevation0;
                    temperateWoodlandRocks = temperateWoodlandRocksAtElevation0;

                    mountainsTrees = new List<int>(new int[] { 22 });
                    mountainsNeedleTrees = new List<int>(new int[] { 12, 25, 30, 12, 25, 30 });
                    mountainsDeadTrees = new List<int>(new int[] { 19, 20, 24, 26, 29 });
                    mountainsFlowers = new List<int>(new int[] { 22 });
                    mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                    mountainsGrass = new List<int>(new int[] { 2, 2, 2, 7, 23 });

                    woodlandHillsFlowers = new List<int>(new int[] { 2, 22, 22, 22, 22 });
                    woodlandHillsBushes = new List<int>(new int[] { 9, 27 });
                    woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 15, 25, 30 });
                    woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 30 });
                    woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 31 });
                } else {
                    if (elevation > elevationLevels[1]) {
                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation1;
                        temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation1;
                        temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation1;
                        temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation1;
                        temperateWoodlandBushes = temperateWoodlandBushesAtElevation1;
                        temperateWoodlandBeach = temperateWoodlandBeachAtElevation1;
                        temperateWoodlandRocks = temperateWoodlandRocksAtElevation1;

                        mountainsTrees = new List<int>(new int[] { 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 12, 25, 30 });
                        mountainsNeedleTrees = new List<int>(new int[] { 12, 25, 30, 12, 25, 30 });
                        mountainsDeadTrees = new List<int>(new int[] { 19, 20, 24, 26, 29 });
                        mountainsFlowers = new List<int>(new int[] { 22 });
                        mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                        mountainsGrass = new List<int>(new int[] { 2, 2, 2, 7, 23 });

                        woodlandHillsFlowers = new List<int>(new int[] { 2, 22, 22, 22, 22 });
                        woodlandHillsBushes = new List<int>(new int[] { 9, 27 });
                        woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 15, 25, 30 });
                        woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 30 });
                        woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 31 });
                    } else {
                        if (elevation > elevationLevels[2]) {
                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation2;
                            temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation2;
                            temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation2;
                            temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation2;
                            temperateWoodlandBushes = temperateWoodlandBushesAtElevation2;
                            temperateWoodlandBeach = temperateWoodlandBeachAtElevation2;
                            temperateWoodlandRocks = temperateWoodlandRocksAtElevation2;

                            mountainsTrees = new List<int>(new int[] { 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 12, 12, 25, 25, 30, 30 });
                            mountainsNeedleTrees = new List<int>(new int[] { 12, 25, 30, 12, 25, 30 });
                            mountainsDeadTrees = new List<int>(new int[] { 19, 20, 24, 26, 29 });
                            mountainsFlowers = new List<int>(new int[] { 22 });
                            mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                            mountainsGrass = new List<int>(new int[] { 2, 2, 2, 7, 9, 23 });

                            woodlandHillsFlowers = new List<int>(new int[] { 2, 22, 22, 22, 22 });
                            woodlandHillsBushes = new List<int>(new int[] { 9, 27 });
                            woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 15, 25, 30 });
                            woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 30 });
                            woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 31 });
                        } else {
                            if (elevation > elevationLevels[3]) {
                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation3;
                                temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation3;
                                temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation3;
                                temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation3;
                                temperateWoodlandBushes = temperateWoodlandBushesAtElevation3;
                                temperateWoodlandBeach = temperateWoodlandBeachAtElevation3;
                                temperateWoodlandRocks = temperateWoodlandRocksAtElevation3;

                                mountainsTrees = new List<int>(new int[] { 22, 22, 22, 22, 22, 22, 22, 22, 12, 12, 25, 25, 30, 30 });
                                mountainsNeedleTrees = new List<int>(new int[] { 12, 25, 30, 12, 25, 30 });
                                mountainsDeadTrees = new List<int>(new int[] { 19, 20, 24, 26, 29 });
                                mountainsFlowers = new List<int>(new int[] { 22 });
                                mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                mountainsGrass = new List<int>(new int[] { 2, 2, 2, 7, 9, 23 });

                                woodlandHillsFlowers = new List<int>(new int[] { 2, 22, 22, 22, 22 });
                                woodlandHillsBushes = new List<int>(new int[] { 9, 27 });
                                woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 15, 25, 30 });
                                woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 30 });
                                woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 31 });
                            } else {
                                if (elevation > elevationLevels[4]) {
                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation4;
                                    temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation4;
                                    temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation4;
                                    temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation4;
                                    temperateWoodlandBushes = temperateWoodlandBushesAtElevation4;
                                    temperateWoodlandBeach = temperateWoodlandBeachAtElevation4;
                                    temperateWoodlandRocks = temperateWoodlandRocksAtElevation4;

                                    mountainsTrees = new List<int>(new int[] { 22, 22, 22, 22, 22, 22, 22, 5, 11, 12, 12, 25, 25, 30, 30 });
                                    mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 12, 25, 30, 12, 25, 30 });
                                    mountainsDeadTrees = new List<int>(new int[] { 19, 20, 24, 26, 29 });
                                    mountainsFlowers = new List<int>(new int[] { 22 });
                                    mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                    mountainsGrass = new List<int>(new int[] { 2, 2, 2, 7, 9, 23 });

                                    woodlandHillsFlowers = new List<int>(new int[] { 2, 22, 22, 22, 22 });
                                    woodlandHillsBushes = new List<int>(new int[] { 9, 27 });
                                    woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 15, 25, 30 });
                                    woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 30 });
                                    woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 31 });
                                } else {
                                    if (elevation > elevationLevels[5]) {
                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation5;
                                        temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation5;
                                        temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation5;
                                        temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation5;
                                        temperateWoodlandBushes = temperateWoodlandBushesAtElevation5;
                                        temperateWoodlandBeach = temperateWoodlandBeachAtElevation5;
                                        temperateWoodlandRocks = temperateWoodlandRocksAtElevation5;

                                        mountainsTrees = new List<int>(new int[] { 22, 22, 22, 22, 22, 5, 11, 12, 13, 21, 25, 25, 30, 30 });
                                        mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 12, 12, 25, 25, 30, 30 });
                                        mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 24, 26, 29 });
                                        mountainsFlowers = new List<int>(new int[] { 22 });
                                        mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                        mountainsGrass = new List<int>(new int[] { 2, 2, 7, 9, 9, 9, 23 });

                                        woodlandHillsFlowers = new List<int>(new int[] { 2, 22, 22, 22, 22 });
                                        woodlandHillsBushes = new List<int>(new int[] { 9, 27 });
                                        woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 15, 25, 30 });
                                        woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 30 });
                                        woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 31 });
                                    } else {
                                        if (elevation > elevationLevels[6]) {
                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation6;
                                            temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation6;
                                            temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation6;
                                            temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation6;
                                            temperateWoodlandBushes = temperateWoodlandBushesAtElevation6;
                                            temperateWoodlandBeach = temperateWoodlandBeachAtElevation6;
                                            temperateWoodlandRocks = temperateWoodlandRocksAtElevation6;

                                            mountainsTrees = new List<int>(new int[] { 22, 22, 22, 22, 5, 11, 12, 13, 21, 25, 25, 30, 30 });
                                            mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 12, 12, 25, 25, 30, 30 });
                                            mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 24, 26, 29 });
                                            mountainsFlowers = new List<int>(new int[] { 22 });
                                            mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                            mountainsGrass = new List<int>(new int[] { 2, 2, 7, 9, 9, 9, 23 });

                                            woodlandHillsFlowers = new List<int>(new int[] { 2, 22, 22, 22, 22 });
                                            woodlandHillsBushes = new List<int>(new int[] { 9, 27 });
                                            woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 15, 25, 30 });
                                            woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 30 });
                                            woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 31 });
                                        } else {
                                            if (elevation > elevationLevels[7]) {
                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation7;
                                                temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation7;
                                                temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation7;
                                                temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation7;
                                                temperateWoodlandBushes = temperateWoodlandBushesAtElevation7;
                                                temperateWoodlandBeach = temperateWoodlandBeachAtElevation7;
                                                temperateWoodlandRocks = temperateWoodlandRocksAtElevation7;

                                                mountainsTrees = new List<int>(new int[] { 22, 22, 22, 5, 11, 12, 13, 21, 25, 25, 30, 30 });
                                                mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 12, 12, 25, 25, 30, 30 });
                                                mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 24, 26, 29 });
                                                mountainsFlowers = new List<int>(new int[] { 22 });
                                                mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                mountainsGrass = new List<int>(new int[] { 2, 2, 7, 9, 9, 9, 23 });

                                                woodlandHillsFlowers = new List<int>(new int[] { 2, 22, 22, 22, 22 });
                                                woodlandHillsBushes = new List<int>(new int[] { 9, 9, 27 });
                                                woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 15, 25, 30 });
                                                woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 30 });
                                                woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 31 });
                                            } else {
                                                if (elevation > elevationLevels[8]) {
                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation8;
                                                    temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation8;
                                                    temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation8;
                                                    temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation8;
                                                    temperateWoodlandBushes = temperateWoodlandBushesAtElevation8;
                                                    temperateWoodlandBeach = temperateWoodlandBeachAtElevation8;
                                                    temperateWoodlandRocks = temperateWoodlandRocksAtElevation8;

                                                    mountainsTrees = new List<int>(new int[] { 22, 22, 5, 11, 12, 13, 21, 25, 30 });
                                                    mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 12, 25, 30 });
                                                    mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 24, 26, 29 });
                                                    mountainsFlowers = new List<int>(new int[] { 22 });
                                                    mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                    mountainsGrass = new List<int>(new int[] { 2, 2, 7, 9, 9, 23 });

                                                    woodlandHillsFlowers = new List<int>(new int[] { 2, 22, 22, 22 });
                                                    woodlandHillsBushes = new List<int>(new int[] { 9, 27 });
                                                    woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 14, 15, 25, 30 });
                                                    woodlandHillsNeedleTrees = new List<int>(new int[] { 12, 25, 30 });
                                                    woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 7, 31 });
                                                } else {
                                                    if (elevation > elevationLevels[9]) {
                                                        rnd = mapStyle + Random.Range(-10f,10f);
                                                        if (rnd <= 25)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9F;
                                                        if (rnd > 25)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9A;
                                                        if (rnd > 30)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9B;
                                                        if (rnd > 35)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9C;
                                                        if (rnd > 41)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9D;
                                                        if (rnd > 47)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9E;
                                                        if (rnd > 55)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation9F;
                                                        if (rnd > 60)
                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation10A;
                                                        temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation9;
                                                        temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation9;
                                                        temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation9;
                                                        temperateWoodlandBushes = temperateWoodlandBushesAtElevation9;
                                                        temperateWoodlandBeach = temperateWoodlandBeachAtElevation9;
                                                        temperateWoodlandRocks = temperateWoodlandRocksAtElevation9;

                                                        mountainsTrees = new List<int>(new int[] { 22, 5, 11, 12, 13, 15, 21, 25, 30 });
                                                        mountainsNeedleTrees = new List<int>(new int[] { 5, 11, 12, 25, 30, 12, 25, 30 });
                                                        mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 24, 26, 29 });
                                                        mountainsFlowers = new List<int>(new int[] { 22 });
                                                        mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                        mountainsGrass = new List<int>(new int[] { 2, 2, 7, 9, 9, 23 });

                                                        woodlandHillsFlowers = new List<int>(new int[] { 2, 21, 22, 22 });
                                                        woodlandHillsBushes = new List<int>(new int[] { 27 });
                                                        woodlandHillsTrees = new List<int>(new int[] { 5, 11, 12, 12, 13, 13, 14, 14, 16, 25, 12 });
                                                        woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 11, 12, 25, 12 });
                                                        woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 7, 31 });
                                                    } else {
                                                        if (elevation > elevationLevels[10]) {
                                                            rnd = mapStyle + Random.Range(-10f,10f);
                                                            if (rnd <= 25)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10F;
                                                            if (rnd > 25)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10A;
                                                            if (rnd > 30)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10B;
                                                            if (rnd > 35)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10C;
                                                            if (rnd > 41)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10D;
                                                            if (rnd > 47)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10E;
                                                            if (rnd > 55)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10F;
                                                            if (rnd > 60)
                                                                temperateWoodlandTrees = temperateWoodlandTreesAtElevation10A;
                                                            temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation10;
                                                            temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation10;
                                                            temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation10;
                                                            temperateWoodlandBushes = temperateWoodlandBushesAtElevation10;
                                                            temperateWoodlandBeach = temperateWoodlandBeachAtElevation10;
                                                            temperateWoodlandRocks = temperateWoodlandRocksAtElevation10;

                                                            mountainsTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 13, 13, 15, 15, 15, 21, 30 });
                                                            mountainsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 12, 30 });
                                                            mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 26, 26, 29, 29 });
                                                            mountainsFlowers = new List<int>(new int[] { 22 });
                                                            mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                            mountainsGrass = new List<int>(new int[] { 2, 7, 7, 9, 23, 23 });

                                                            woodlandHillsFlowers = new List<int>(new int[] { 2, 21, 21, 21, 22, 22 });
                                                            woodlandHillsBushes = new List<int>(new int[] { 9, 27, 27 });
                                                            woodlandHillsTrees = new List<int>(new int[] { 12, 12, 13, 14, 15, 16 });
                                                            woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 25, 30 });
                                                            woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 7, 31 });
                                                        } else {
                                                            if (elevation > elevationLevels[11]) {
                                                                rnd = mapStyle + Random.Range(-10f,10f);
                                                                if (rnd <= 25)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11F;
                                                                if (rnd > 25)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11A;
                                                                if (rnd > 30)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11B;
                                                                if (rnd > 35)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11C;
                                                                if (rnd > 41)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11D;
                                                                if (rnd > 47)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11E;
                                                                if (rnd > 55)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11F;
                                                                if (rnd > 60)
                                                                    temperateWoodlandTrees = temperateWoodlandTreesAtElevation11A;
                                                                temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation11;
                                                                temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation11;
                                                                temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation11;
                                                                temperateWoodlandBushes = temperateWoodlandBushesAtElevation11;
                                                                temperateWoodlandBeach = temperateWoodlandBeachAtElevation11;
                                                                temperateWoodlandRocks = temperateWoodlandRocksAtElevation11;

                                                                mountainsTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 13, 13, 15, 15, 15, 21, 30 });
                                                                mountainsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 12, 30 });
                                                                mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 26, 26, 29, 29 });
                                                                mountainsFlowers = new List<int>(new int[] { 22 });
                                                                mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                                mountainsGrass = new List<int>(new int[] { 2, 7, 7, 9, 23, 23 });

                                                                woodlandHillsFlowers = new List<int>(new int[] { 2, 21, 22 });
                                                                woodlandHillsBushes = new List<int>(new int[] { 27 });
                                                                woodlandHillsTrees = new List<int>(new int[] { 12, 12, 13, 13, 14, 14, 15, 15, 16, 16 });
                                                                woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 11, 12, 25, 30 });
                                                                woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 7, 31 });
                                                            } else {
                                                                if (elevation > elevationLevels[12]) {
                                                                    rnd = mapStyle + Random.Range(-10f,10f);
                                                                    if (rnd <= 25)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12F;
                                                                    if (rnd > 25)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12A;
                                                                    if (rnd > 30)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12B;
                                                                    if (rnd > 35)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12C;
                                                                    if (rnd > 41)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12D;
                                                                    if (rnd > 47)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12E;
                                                                    if (rnd > 55)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12F;
                                                                    if (rnd > 60)
                                                                        temperateWoodlandTrees = temperateWoodlandTreesAtElevation12A;
                                                                    temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation12;
                                                                    temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation12;
                                                                    temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation12;
                                                                    temperateWoodlandBushes = temperateWoodlandBushesAtElevation12;
                                                                    temperateWoodlandBeach = temperateWoodlandBeachAtElevation12;
                                                                    temperateWoodlandRocks = temperateWoodlandRocksAtElevation12;

                                                                    mountainsTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 13, 13, 15, 15, 15, 21, 30 });
                                                                    mountainsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 12, 30 });
                                                                    mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 26, 26, 29, 29 });
                                                                    mountainsFlowers = new List<int>(new int[] { 22 });
                                                                    mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                                    mountainsGrass = new List<int>(new int[] { 2, 7, 7, 9, 23, 23 });

                                                                    woodlandHillsFlowers = new List<int>(new int[] { 2, 21, 21, 21 });
                                                                    woodlandHillsBushes = new List<int>(new int[] { 27, 27, 31 });
                                                                    woodlandHillsTrees = new List<int>(new int[] { 12, 13, 13, 13, 14, 14, 15, 15, 16, 16 });
                                                                    woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 12 });
                                                                    woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 7 });
                                                                } else {
                                                                    if (elevation > elevationLevels[13]) {
                                                                        rnd = mapStyle + Random.Range(-10f,10f);
                                                                        if (rnd <= 25)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13F;
                                                                        if (rnd > 25)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13A;
                                                                        if (rnd > 30)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13B;
                                                                        if (rnd > 35)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13C;
                                                                        if (rnd > 41)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13D;
                                                                        if (rnd > 47)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13E;
                                                                        if (rnd > 55)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13F;
                                                                        if (rnd > 60)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation13A;
                                                                        temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation13;
                                                                        temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation13;
                                                                        temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation13;
                                                                        temperateWoodlandBushes = temperateWoodlandBushesAtElevation13;
                                                                        temperateWoodlandBeach = temperateWoodlandBeachAtElevation13;
                                                                        temperateWoodlandRocks = temperateWoodlandRocksAtElevation13;

                                                                        mountainsTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 13, 13, 15, 15, 15, 21, 30 });
                                                                        mountainsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 12, 30 });
                                                                        mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 26, 26, 29, 29 });
                                                                        mountainsFlowers = new List<int>(new int[] { 22 });
                                                                        mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                                        mountainsGrass = new List<int>(new int[] { 2, 7, 7, 7, 9, 23, 23, 23 });

                                                                        woodlandHillsFlowers = new List<int>(new int[] { 2, 21, 21 });
                                                                        woodlandHillsBushes = new List<int>(new int[] { 27, 27, 27, 31 });
                                                                        woodlandHillsTrees = new List<int>(new int[] { 13, 14, 14, 15, 15, 16 });
                                                                        woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 11, 11 });
                                                                        woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 7 });
                                                                    } else {
                                                                        rnd = mapStyle + Random.Range(-15f,15f);
                                                                        if (rnd <= 18)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14D;
                                                                        if (rnd > 18)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14A;
                                                                        if (rnd > 30)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14B;
                                                                        if (rnd > 42)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14C;
                                                                        if (rnd > 55)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14D;
                                                                        if (rnd > 67)
                                                                            temperateWoodlandTrees = temperateWoodlandTreesAtElevation14A;
                                                                        temperateWoodlandDeadTrees = temperateWoodlandDeadTreesAtElevation14;
                                                                        temperateWoodlandFlowers = temperateWoodlandFlowersAtElevation14;
                                                                        temperateWoodlandMushroom = temperateWoodlandMushroomAtElevation14;
                                                                        temperateWoodlandBushes = temperateWoodlandBushesAtElevation14;
                                                                        temperateWoodlandBeach = temperateWoodlandBeachAtElevation14;
                                                                        temperateWoodlandRocks = temperateWoodlandRocksAtElevation14;

                                                                        mountainsTrees = new List<int>(new int[] { 5, 5, 9, 9, 11, 11, 12, 13, 13, 15, 15, 15, 21, 30 });
                                                                        mountainsNeedleTrees = new List<int>(new int[] { 5, 5, 11, 11, 12, 12, 30 });
                                                                        mountainsDeadTrees = new List<int>(new int[] { 16, 19, 20, 26, 26, 29, 29 });
                                                                        mountainsFlowers = new List<int>(new int[] { 22 });
                                                                        mountainsRocks = new List<int>(new int[] { 1, 3, 4, 6, 7, 8, 10, 14, 17, 18, 27, 28, 31 });
                                                                        mountainsGrass = new List<int>(new int[] { 2, 7, 7, 7, 9, 23, 23, 23 });

                                                                        woodlandHillsFlowers = new List<int>(new int[] { 2, 2, 7, 7, 21, 23, 23 });
                                                                        woodlandHillsBushes = new List<int>(new int[] { 27, 27, 31 });
                                                                        woodlandHillsTrees = new List<int>(new int[] { 13, 14, 15, 16, 16 });
                                                                        woodlandHillsNeedleTrees = new List<int>(new int[] { 5, 11, 11 });
                                                                        woodlandHillsDirtPlants = new List<int>(new int[] { 26, 29, 23, 7 });
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
