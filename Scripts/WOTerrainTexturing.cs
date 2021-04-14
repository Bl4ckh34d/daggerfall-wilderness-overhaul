﻿// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut, Daniel87
//
// Notes:
//

using UnityEngine;
using System;
using DaggerfallConnect.Arena2;
using Unity.Jobs;
using Unity.Collections;

namespace DaggerfallWorkshop
{
	/// <summary>
	/// Generates texture tiles for terrains and uses marching squares for tile transitions.
	/// These features are very much in early stages of development.
	/// </summary>
	public class WOTerrainTexturing : ITerrainTexturing
	{
		// Use same seed to ensure continuous tiles
		const int seed = 417028;
		const byte water = 0;
		const byte dirt = 1;
		const byte grass = 2;
		const byte stone = 3;

		const float desert1Frequency = 0.02f;
		const float desert1Amplitude = 0.3f;
		const float desert1Persistance = 0.5f;
		const int desert1Octaves = 5;
		const float desert1UpperWaterSpread = -1.0f;
		const float desert1LowerGrassSpread = 0.4f;
		const float desert1UpperGrassSpread = 0.5f;

		const float desert2Frequency = 0.02f;
		const float desert2Amplitude = 0.3f;
		const float desert2Persistance = 0.55f;
		const int desert2Octaves = 5;
		const float desert2UpperWaterSpread = -1.0f;
		const float desert2LowerGrassSpread = 0.35f;
		const float desert2UpperGrassSpread = 0.5f;

		const float mountainFrequency = 0.025f;
		const float mountainAmplitude = 0.3f;
		const float mountainPersistance = 0.95f;
		const int mountainOctaves = 5;
		const float mountainUpperWaterSpread = 0.0f;
		const float mountainLowerGrassSpread = 0.35f;
		const float mountainUpperGrassSpread = 0.95f;

		const float rainforestFrequency = 0.035f;
		const float rainforestAmplitude = 0.4f;
		const float rainforestPersistance = 0.8f;
		const int rainforestOctaves = 5;
		const float rainforestUpperWaterSpread = 0.0f;
		const float rainforestLowerGrassSpread = 0.35f;
		const float rainforestUpperGrassSpread = 0.95f;

		const float swampFrequency = 0.02f;
		const float swampAmplitude = 0.3f;
		const float swampPersistance = 0.5f;
		const int swampOctaves = 5;
		const float swampUpperWaterSpread = -1.0f;
		const float swampLowerGrassSpread = 0.4f;
		const float swampUpperGrassSpread = 0.5f;

		const float subtropicalFrequency = 0.02f;
		const float subtropicalAmplitude = 0.3f;
		const float subtropicalPersistance = 0.5f;
		const int subtropicalOctaves = 5;
		const float subtropicalUpperWaterSpread = -1.0f;
		const float subtropicalLowerGrassSpread = 0.4f;
		const float subtropicalUpperGrassSpread = 0.5f;

		const float mountainWoodsFrequency = 0.035f;
		const float mountainWoodsAmplitude = 0.4f;
		const float mountainWoodsPersistance = 0.8f;
		const int mountainWoodsOctaves = 5;
		const float mountainWoodsUpperWaterSpread = 0.0f;
		const float mountainWoodsLowerGrassSpread = 0.35f;
		const float mountainWoodsUpperGrassSpread = 0.95f;

		const float woodlandsFrequency = 0.035f;
		const float woodlandsAmplitude = 0.4f;
		const float woodlandsPersistance = 0.8f;
		const int woodlandsOctaves = 5;
		const float woodlandsUpperWaterSpread = 0.0f;
		const float woodlandsLowerGrassSpread = 0.35f;
		const float woodlandsUpperGrassSpread = 0.95f;

		const float hauntedWoodsFrequency = 0.035f;
		const float hauntedWoodsAmplitude = 0.4f;
		const float hauntedWoodsPersistance = 0.8f;
		const int hauntedWoodsOctaves = 5;
		const float hauntedWoodsUpperWaterSpread = 0.0f;
		const float hauntedWoodsLowerGrassSpread = 0.35f;
		const float hauntedWoodsUpperGrassSpread = 0.95f;

		const float oceanFrequency = 0.1f;
		const float oceanAmplitude = 0.95f;
		const float oceanPersistance = 0.3f;
		const int oceanOctaves = 5;
		const float oceanUpperWaterSpread = 0.0f;
		const float oceanLowerGrassSpread = 0.35f;
		const float oceanUpperGrassSpread = 0.95f;

		public static float treeLine = UnityEngine.Random.Range(0.815f, 0.835f);

		protected static readonly int tileDataDim = MapsFile.WorldMapTileDim + 1;
		protected static readonly int assignTilesDim = MapsFile.WorldMapTileDim;

		protected byte[] lookupTable;

		public WOTerrainTexturing()
		{
			CreateLookupTable();
		}

		public virtual JobHandle ScheduleAssignTilesJob(ITerrainSampler terrainSampler, ref MapPixelData mapData, JobHandle dependencies, bool march = true)
		{
			// Cache tile data to minimise noise sampling during march.
			NativeArray<byte> tileData = new NativeArray<byte>(tileDataDim * tileDataDim, Allocator.TempJob);

			GenerateTileDataJob tileDataJob = new GenerateTileDataJob
			{
				heightmapData = mapData.heightmapData,
				tileData = tileData,
				tdDim = tileDataDim,
				hDim = terrainSampler.HeightmapDimension,
				maxTerrainHeight = terrainSampler.MaxTerrainHeight,
				oceanElevation = terrainSampler.OceanElevation,
				beachElevation = terrainSampler.BeachElevation,
				mapPixelX = mapData.mapPixelX,
				mapPixelY = mapData.mapPixelY,
				worldClimate = mapData.worldClimate,
			};
			JobHandle tileDataHandle = tileDataJob.Schedule(tileDataDim * tileDataDim, 64, dependencies);

			// Assign tile data to terrain
			NativeArray<byte> lookupData = new NativeArray<byte>(lookupTable, Allocator.TempJob);
			AssignTilesJob assignTilesJob = new AssignTilesJob
			{
				lookupTable = lookupData,
				tileData = tileData,
				tilemapData = mapData.tilemapData,
				tdDim = tileDataDim,
				tDim = assignTilesDim,
				march = march,
				locationRect = mapData.locationRect,
			};
			JobHandle assignTilesHandle = assignTilesJob.Schedule(assignTilesDim * assignTilesDim, 64, tileDataHandle);

			// Add both working native arrays to disposal list.
			mapData.nativeArrayList.Add(tileData);
			mapData.nativeArrayList.Add(lookupData);

			return assignTilesHandle;
		}

		#region Marching Squares - WIP

		// Very basic marching squares for water > dirt > grass > stone transitions.
		// Cannot handle water > grass or water > stone, etc.
		// Will improve this at later date to use a wider range of transitions.
		protected struct AssignTilesJob : IJobParallelFor
		{
			[ReadOnly]
			public NativeArray<byte> tileData;
			[ReadOnly]
			public NativeArray<byte> lookupTable;

			public NativeArray<byte> tilemapData;

			public int tdDim;
			public int tDim;
			public bool march;
			public Rect locationRect;

			public void Execute(int index)
			{
				int x = JobA.Row(index, tDim);
				int y = JobA.Col(index, tDim);

				// Do nothing if in location rect as texture already set, to 0xFF if zero
				if (tilemapData[index] != 0)
					return;

				// Assign tile texture
				if (march)
				{
					// Get sample points
					int tdIdx = JobA.Idx(x, y, tdDim);
					int b0 = tileData[tdIdx];               // tileData[x, y]
					int b1 = tileData[tdIdx + 1];           // tileData[x + 1, y]
					int b2 = tileData[tdIdx + tdDim];       // tileData[x, y + 1]
					int b3 = tileData[tdIdx + tdDim + 1];   // tileData[x + 1, y + 1]

					int shape = (b0 & 1) | (b1 & 1) << 1 | (b2 & 1) << 2 | (b3 & 1) << 3;
					int ring = (b0 + b1 + b2 + b3) >> 2;
					int tileID = shape | ring << 4;

					tilemapData[index] = lookupTable[tileID];
				}
				else
				{
					tilemapData[index] = tileData[JobA.Idx(x, y, tdDim)];
				}
			}
		}

		protected struct GenerateTileDataJob : IJobParallelFor
		{
			[ReadOnly]
			public NativeArray<float> heightmapData;

			public NativeArray<byte> tileData;

			public int hDim;
			public int tdDim;
			public float maxTerrainHeight;
			public float oceanElevation;
			public float beachElevation;
			public int mapPixelX;
			public int mapPixelY;
			public int worldClimate;

			// Gets noise value
			private float NoiseWeight(float worldX, float worldY, float height)
			{
				float woodlandsPersistanceRnd = woodlandsPersistance + ((height / maxTerrainHeight) * 1.5f) - 0.35f;
				float mountainWoodsPersistanceRnd = mountainWoodsPersistance + ((height / maxTerrainHeight) * 1.5f) - 0.35f;
				float hauntedWoodsPersistanceRnd = hauntedWoodsPersistance + ((height / maxTerrainHeight) * 1.5f) - 0.35f;
				float mountainsPersistanceRnd;
				if ((height / maxTerrainHeight) + JobRand.Next(-5, 5) / 1000f > treeLine)
				{
					mountainsPersistanceRnd = mountainPersistance - treeLine + ((height / maxTerrainHeight) * 1.2f);
				}
				else
				{
					mountainsPersistanceRnd = mountainPersistance - (1 - (height / maxTerrainHeight)) * 0.4f;
				}
				float desert1PersistanceRnd = desert1Persistance + ((height / maxTerrainHeight) * 2) - 0.25f;
				float desert2PersistanceRnd = desert2Persistance + ((height / maxTerrainHeight) * 2) - 0.25f;
				float subtropicalPersistanceRnd = subtropicalPersistance + ((height / maxTerrainHeight) * 2) - 0.25f;
				float swampPersistanceRnd = swampPersistance + ((height / maxTerrainHeight) * 3) - 0.25f;
				float rainforestPersistanceRnd = rainforestPersistance + ((height / maxTerrainHeight) * 1.5f) - 0.35f;

				switch (worldClimate)
				{
					case (int)MapsFile.Climates.Desert:
						return GetNoise(worldX, worldY, desert1Frequency, desert1Amplitude, desert1PersistanceRnd, desert1Octaves, seed);
					case (int)MapsFile.Climates.Desert2:
						return GetNoise(worldX, worldY, desert2Frequency, desert2Amplitude, desert2PersistanceRnd, desert2Octaves, seed);
					case (int)MapsFile.Climates.Mountain:
						return GetNoise(worldX, worldY, mountainFrequency, mountainAmplitude, mountainsPersistanceRnd, mountainOctaves, seed);
					case (int)MapsFile.Climates.Rainforest:
						return GetNoise(worldX, worldY, rainforestFrequency, rainforestAmplitude, rainforestPersistance, rainforestOctaves, seed);
					case (int)MapsFile.Climates.Swamp:
						return GetNoise(worldX, worldY, swampFrequency, swampAmplitude, swampPersistance, swampOctaves, seed);
					case (int)MapsFile.Climates.Subtropical:
						return GetNoise(worldX, worldY, subtropicalFrequency, subtropicalAmplitude, subtropicalPersistanceRnd, subtropicalOctaves, seed);
					case (int)MapsFile.Climates.MountainWoods:
						return GetNoise(worldX, worldY, mountainWoodsFrequency, mountainWoodsAmplitude, mountainWoodsPersistanceRnd, mountainWoodsOctaves, seed);
					case (int)MapsFile.Climates.Woodlands:
						return GetNoise(worldX, worldY, woodlandsFrequency, woodlandsAmplitude, woodlandsPersistanceRnd, woodlandsOctaves, seed);
					case (int)MapsFile.Climates.HauntedWoodlands:
						return GetNoise(worldX, worldY, hauntedWoodsFrequency, hauntedWoodsAmplitude, hauntedWoodsPersistanceRnd, hauntedWoodsOctaves, seed);
					case (int)MapsFile.Climates.Ocean:
						return GetNoise(worldX, worldY, oceanFrequency, oceanAmplitude, oceanPersistance, oceanOctaves, seed);
				}
				return GetNoise(worldX, worldY, 0.1f, 0.95f, 0.3f, 5, seed); //worldX, worldY, 0.05f, 0.9f, 0.4f, 3, seed
			}

			// Sets texture by range
			private byte GetWeightedRecord(float weight, float upperWaterSpread = 0.0f, float lowerGrassSpread = 0.35f, float upperGrassSpread = 0.95f)
			{
				if (weight < upperWaterSpread)
					return water;
				else if (weight >= upperWaterSpread && weight < lowerGrassSpread)
					return dirt;
				else if (weight >= lowerGrassSpread && weight < upperGrassSpread)
					return grass;
				else
					return stone;
			}

			// Noise function
			private float GetNoise(
			  float x,
			  float y,
			  float frequency,
			  float amplitude,
			  float persistance,
			  int octaves,
			  int seed = 0)
			{
				float finalValue = 0f;
				for (int i = 0; i < octaves; ++i)
				{
					finalValue += Mathf.PerlinNoise(seed + (x * frequency), seed + (y * frequency)) * amplitude;
					frequency *= 2.0f;
					amplitude *= persistance;
				}
				return Mathf.Clamp(finalValue, -1, 1);
			}

			public void Execute(int index)
			{
				int x = JobA.Row(index, tdDim);
				int y = JobA.Col(index, tdDim);
				int uB = heightmapData.Length;

				// Height sample for ocean and beach tiles
				int hx = (int)Mathf.Clamp(hDim * ((float)x / (float)tdDim), 0, hDim - 1);
				int hy = (int)Mathf.Clamp(hDim * ((float)y / (float)tdDim), 0, hDim - 1);
				float height = heightmapData[JobA.Idx(hy, hx, hDim)] * maxTerrainHeight;  // x & y swapped in heightmap for TerrainData.SetHeights()

				// Ocean texture
				if (height <= oceanElevation)
				{
					tileData[index] = water;
					return;
				}
				// Beach texture
				// Adds a little +/- randomness to threshold so beach line isn't too regular
				if (height <= beachElevation + (JobRand.Next(-15000000, 15000000) / 10000000f))
				{
					tileData[index] = dirt;
					return;
				}

				// Get latitude and longitude of this tile
				int latitude = (int)(mapPixelX * MapsFile.WorldMapTileDim + x);
				int longitude = (int)(MapsFile.MaxWorldTileCoordZ - mapPixelY * MapsFile.WorldMapTileDim + y);

				// Set texture tile using weighted noise
				float weight = 0;

				weight += NoiseWeight(latitude, longitude, height);

				switch (worldClimate)
				{
					case (int)MapsFile.Climates.Desert:
						tileData[index] = GetWeightedRecord(weight, desert1UpperWaterSpread, desert1LowerGrassSpread, desert1UpperGrassSpread);
						break;
					case (int)MapsFile.Climates.Desert2:
						tileData[index] = GetWeightedRecord(weight, desert2UpperWaterSpread, desert2LowerGrassSpread, desert2UpperGrassSpread);
						break;
					case (int)MapsFile.Climates.Mountain:
						tileData[index] = GetWeightedRecord(weight, mountainUpperWaterSpread, mountainLowerGrassSpread, mountainUpperGrassSpread);
						break;
					case (int)MapsFile.Climates.Rainforest:
						tileData[index] = GetWeightedRecord(weight, rainforestUpperWaterSpread, rainforestLowerGrassSpread, rainforestUpperGrassSpread);
						break;
					case (int)MapsFile.Climates.Swamp:
						tileData[index] = GetWeightedRecord(weight, swampUpperWaterSpread, swampLowerGrassSpread, swampUpperGrassSpread);
						break;
					case (int)MapsFile.Climates.Subtropical:
						tileData[index] = GetWeightedRecord(weight, subtropicalUpperWaterSpread, subtropicalLowerGrassSpread, subtropicalUpperGrassSpread);
						break;
					case (int)MapsFile.Climates.MountainWoods:
						tileData[index] = GetWeightedRecord(weight, mountainWoodsUpperWaterSpread, mountainWoodsLowerGrassSpread, mountainWoodsUpperGrassSpread);
						break;
					case (int)MapsFile.Climates.Woodlands:
						tileData[index] = GetWeightedRecord(weight, woodlandsUpperWaterSpread, woodlandsLowerGrassSpread, woodlandsUpperGrassSpread);
						break;
					case (int)MapsFile.Climates.HauntedWoodlands:
						tileData[index] = GetWeightedRecord(weight, hauntedWoodsUpperWaterSpread, hauntedWoodsLowerGrassSpread, hauntedWoodsUpperGrassSpread);
						break;
					case (int)MapsFile.Climates.Ocean:
						tileData[index] = GetWeightedRecord(weight, oceanUpperWaterSpread, oceanLowerGrassSpread, oceanUpperGrassSpread);
						break;
				}
				// Check for lowest local point in desert to place oasis
				if (worldClimate == (int)MapsFile.Climates.Desert2 &&
					LowestPointFound(30, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData))
				{
					tileData[index] = water;
				}
				if (worldClimate == (int)MapsFile.Climates.Desert &&
					LowestPointFound(80, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData))
				{
					tileData[index] = water;
				}
			}
		}

		// Creates lookup table
		void CreateLookupTable()
		{
			lookupTable = new byte[64];
			AddLookupRange(0, 1, 5, 48, false, 0);
			AddLookupRange(2, 1, 10, 51, true, 16);
			AddLookupRange(2, 3, 15, 53, false, 32);
			AddLookupRange(3, 3, 15, 53, true, 48);
		}

		// Adds range of 16 values to lookup table
		void AddLookupRange(int baseStart, int baseEnd, int shapeStart, int saddleIndex, bool reverse, int offset)
		{
			if (reverse)
			{
				// high > low
				lookupTable[offset] = MakeLookup(baseStart, false, false);
				lookupTable[offset + 1] = MakeLookup(shapeStart + 2, true, true);
				lookupTable[offset + 2] = MakeLookup(shapeStart + 2, false, false);
				lookupTable[offset + 3] = MakeLookup(shapeStart + 1, true, true);
				lookupTable[offset + 4] = MakeLookup(shapeStart + 2, false, true);
				lookupTable[offset + 5] = MakeLookup(shapeStart + 1, false, true);
				lookupTable[offset + 6] = MakeLookup(saddleIndex, true, false); //d
				lookupTable[offset + 7] = MakeLookup(shapeStart, true, true);
				lookupTable[offset + 8] = MakeLookup(shapeStart + 2, true, false);
				lookupTable[offset + 9] = MakeLookup(saddleIndex, false, false); //d
				lookupTable[offset + 10] = MakeLookup(shapeStart + 1, false, false);
				lookupTable[offset + 11] = MakeLookup(shapeStart, false, false);
				lookupTable[offset + 12] = MakeLookup(shapeStart + 1, true, false);
				lookupTable[offset + 13] = MakeLookup(shapeStart, false, true);
				lookupTable[offset + 14] = MakeLookup(shapeStart, true, false);
				lookupTable[offset + 15] = MakeLookup(baseEnd, false, false);
			}
			else
			{
				// low > high
				lookupTable[offset] = MakeLookup(baseStart, false, false);
				lookupTable[offset + 1] = MakeLookup(shapeStart, true, false);
				lookupTable[offset + 2] = MakeLookup(shapeStart, false, true);
				lookupTable[offset + 3] = MakeLookup(shapeStart + 1, true, false);
				lookupTable[offset + 4] = MakeLookup(shapeStart, false, false);
				lookupTable[offset + 5] = MakeLookup(shapeStart + 1, false, false);
				lookupTable[offset + 6] = MakeLookup(saddleIndex, false, false); //d
				lookupTable[offset + 7] = MakeLookup(shapeStart + 2, true, false);
				lookupTable[offset + 8] = MakeLookup(shapeStart, true, true);
				lookupTable[offset + 9] = MakeLookup(saddleIndex, true, false); //d
				lookupTable[offset + 10] = MakeLookup(shapeStart + 1, false, true);
				lookupTable[offset + 11] = MakeLookup(shapeStart + 2, false, true);
				lookupTable[offset + 12] = MakeLookup(shapeStart + 1, true, true);
				lookupTable[offset + 13] = MakeLookup(shapeStart + 2, false, false);
				lookupTable[offset + 14] = MakeLookup(shapeStart + 2, true, true);
				lookupTable[offset + 15] = MakeLookup(baseEnd, false, false);
			}
		}

		// Encodes a byte with Daggerfall tile lookup
		byte MakeLookup(int index, bool rotate, bool flip)
		{
			if (index > 55)
				throw new IndexOutOfRangeException("Index out of range. Valid range 0-55");
			if (rotate) index += 64;
			if (flip) index += 128;

			return (byte)index;
		}

		static bool LowestPointFound(int chance, NativeArray<float> heightmapData, float maxTerrainHeight, int hx, int hy, int hDim, int upperBound, int index, int tdDim, NativeArray<byte> tileData)
		{
			int newChance = (int)(chance - (chance * heightmapData[JobA.Idx(hy, hx, hDim)] * 2));

			if (tileData[index] != dirt ||
			  JobA.Row(index, tdDim) - 5 <= 0 ||
			  JobA.Col(index, tdDim) + 5 >= tdDim ||
			  JobA.Row(index, tdDim) + 5 >= tdDim ||
			  JobA.Col(index, tdDim) - 5 <= 0)
			{
				return false;
			}
			else
			{
				float thisHeight = heightmapData[JobA.Idx(hy, hx, hDim)] * maxTerrainHeight;
				if (JobA.Idx(hy - 5, hx - 5, hDim) < 0 ||
					JobA.Idx(hy - 5, hx - 5, hDim) > upperBound ||
					JobA.Idx(hy - 5, hx + 5, hDim) < 0 ||
					JobA.Idx(hy - 5, hx + 5, hDim) > upperBound ||
					JobA.Idx(hy + 5, hx - 5, hDim) < 0 ||
					JobA.Idx(hy + 5, hx - 5, hDim) > upperBound ||
					JobA.Idx(hy + 5, hx + 5, hDim) < 0 ||
					JobA.Idx(hy + 5, hx + 5, hDim) > upperBound
					)
				{
					return false;
				}
				else
				{
					for (int a = -4; a < 5; a++)
					{
						for (int b = -4; b < 5; b++)
						{
							if ((a != 0 && b != 0) && heightmapData[JobA.Idx(hy + a, hx + b, hDim)] * maxTerrainHeight < thisHeight + (0.0000035f * Mathf.Abs(a)) + (0.0000035f * Mathf.Abs(b)))
							{
								return false;
							}
						}
					}
					if (JobRand.Next(0, 100) <= newChance)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
		}
		#endregion
	}
}