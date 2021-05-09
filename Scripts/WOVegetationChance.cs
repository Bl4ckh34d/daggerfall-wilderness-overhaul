using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using WildernessOverhaul;

namespace WildernessOverhaul
{
	public class WOVegetationChance
	{
		public float chanceOnDirt;
		public float chanceOnGrass;
		public float chanceOnStone;

		public WOVegetationChance(float elevation, DFLocation.ClimateSettings climate) {

			if (climate.WorldClimate == (int)MapsFile.Climates.Woodlands)
			{
				if (elevation > 0.25f)
				{
					chanceOnGrass = Mathf.Clamp(Random.Range(0.225f, 0.300f), 0f, 1f);
					chanceOnDirt = Mathf.Clamp(Random.Range(0.450f, 0.475f), 0f, 1f);
					chanceOnStone = Mathf.Clamp(Random.Range(0.500f, 0.525f), 0f, 1f);
				}
				else if (elevation <= 0.25f && elevation > 0.2f)
				{
					chanceOnGrass = Mathf.Clamp(Random.Range(0.225f, 0.300f), 0f, 1f);
					chanceOnDirt = Mathf.Clamp(Random.Range(0.425f, 0.450f), 0f, 1f);
					chanceOnStone = Mathf.Clamp(Random.Range(0.675f, 0.700f), 0f, 1f);
				}
				else if (elevation <= 0.2f && elevation > 0.15f)
				{
					chanceOnGrass = Mathf.Clamp(Random.Range(0.250f, 0.300f), 0f, 1f);
					chanceOnDirt = Mathf.Clamp(Random.Range(0.300f, 0.325f), 0f, 1f);
					chanceOnStone = Mathf.Clamp(Random.Range(0.650f, 0.675f), 0f, 1f);
				}
				else if (elevation <= 0.15f)
				{
					chanceOnGrass = Mathf.Clamp(Random.Range(0.275f, 0.300f), 0f, 1f);
					chanceOnDirt = Mathf.Clamp(Random.Range(0.150f, 0.175f), 0f, 1f);
					chanceOnStone = Mathf.Clamp(Random.Range(0.625f, 0.650f), 0f, 1f);
				}
			}

			//Adjustment to mountain climate in respect to world height
			else if (climate.WorldClimate == (int)MapsFile.Climates.Mountain)
			{
				if (elevation > WOTerrainTexturing.treeLine)
				{
					chanceOnGrass = Mathf.Clamp(Random.Range(0.250f, 0.275f), 0f, 1f);
					chanceOnDirt = Mathf.Clamp(Random.Range(0.200f, 0.225f), 0f, 1f);
					chanceOnStone = Mathf.Clamp(Random.Range(0.325f, 0.350f), 0f, 1f);
				}
				else if (elevation <= WOTerrainTexturing.treeLine && elevation > 0.6f)
				{
					chanceOnGrass = Mathf.Clamp(Random.Range(0.275f, 0.300f), 0f, 1f);
					chanceOnDirt = Mathf.Clamp(Random.Range(0.375f, 0.400f), 0f, 1f);
					chanceOnStone = Mathf.Clamp(Random.Range(0.275f, 0.300f), 0f, 1f);
				}
				else if (elevation <= 0.6f && elevation > 0.4f)
				{
					chanceOnGrass = Mathf.Clamp(Random.Range(0.300f, 0.325f), 0f, 1f);
					chanceOnDirt = Mathf.Clamp(Random.Range(0.350f, 0.400f), 0f, 1f);
					chanceOnStone = Mathf.Clamp(Random.Range(0.225f, 0.250f), 0f, 1f);
				}
				else if (elevation <= 0.4f && elevation > 0.2f)
				{
					chanceOnGrass = Mathf.Clamp(Random.Range(0.325f, 0.375f), 0f, 1f);
					chanceOnDirt = Mathf.Clamp(Random.Range(0.350f, 0.375f), 0f, 1f);
					chanceOnStone = Mathf.Clamp(Random.Range(0.175f, 0.200f), 0f, 1f);
				}
				else if (elevation <= 0.2f)
				{
					chanceOnGrass = Mathf.Clamp(Random.Range(0.375f, 0.400f), 0f, 1f);
					chanceOnDirt = Mathf.Clamp(Random.Range(0.300f, 0.350f), 0f, 1f);
					chanceOnStone = Mathf.Clamp(Random.Range(0.250f, 0.275f), 0f, 1f);
				}
			}

			//Adjustment to desert climate in respect to world height
			else if (climate.WorldClimate == (int)MapsFile.Climates.Desert)
			{
				chanceOnGrass = Mathf.Clamp(Random.Range(0.100f, 0.150f), 0f, 1f);
				chanceOnDirt = Mathf.Clamp(Random.Range(0.100f, 0.150f), 0f, 1f);
				chanceOnStone = Mathf.Clamp(Random.Range(0.050f, 0.100f), 0f, 1f);
			}

			//Adjustment to desert climate in respect to world height
			else if (climate.WorldClimate == (int)MapsFile.Climates.Desert2)
			{
				chanceOnGrass = Mathf.Clamp(Random.Range(0.05f, 0.25f), 0f, 1f);
				chanceOnDirt = Mathf.Clamp(Random.Range(0.05f, 0.25f), 0f, 1f);
				chanceOnStone = Mathf.Clamp(Random.Range(0.05f, 0.20f), 0f, 1f);
			}

			// Adjustment to temperate woodlands climate in respect to world height
			else if (climate.WorldClimate == (int)MapsFile.Climates.HauntedWoodlands)
			{
				chanceOnGrass = Random.Range(0.05f, 0.09f);
				chanceOnDirt = Random.Range(0.045f, 0.065f);
				chanceOnStone = Random.Range(0.05f, 0.1f);
			}

			//Adjustment to mountain woodland climate in respect to world height
			else if (climate.WorldClimate == (int)MapsFile.Climates.MountainWoods)
			{
				if (elevation > 0.475f)
				{
					chanceOnGrass = Random.Range(0.035f, 0.080f);
					chanceOnDirt = Random.Range(0.120f, 0.150f);
					chanceOnStone = Random.Range(0.120f, 0.150f);
				}
				else if (elevation <= 0.475f && elevation > 0.4f)
				{
					chanceOnGrass = Random.Range(0.040f, 0.100f);
					chanceOnDirt = Random.Range(0.100f, 0.130f);
					chanceOnStone = Random.Range(0.100f, 0.130f);
				}
				else if (elevation <= 0.4f && elevation > 0.325f)
				{
					chanceOnGrass = Random.Range(0.050f, 0.100f);
					chanceOnDirt = Random.Range(0.090f, 0.110f);
					chanceOnStone = Random.Range(0.090f, 0.110f);
				}
				else if (elevation <= 0.325f && elevation > 0.25f)
				{
					chanceOnGrass = Random.Range(0.090f, 0.120f);
					chanceOnDirt = Random.Range(0.070f, 0.090f);
					chanceOnStone = Random.Range(0.060f, 0.090f);
				}
				else if (elevation <= 0.25f)
				{
					chanceOnGrass = Random.Range(0.100f, 0.130f);
					chanceOnDirt = Random.Range(0.060f, 0.090f);
					chanceOnStone = Random.Range(0.050f, 0.080f);
				}
			}

			//Adjustment to swamp climate in respect to world height
			if (climate.WorldClimate == (int)MapsFile.Climates.Swamp)
			{
				chanceOnStone = Mathf.Clamp(Random.Range(0.00f, 0.00f), 0f, 1f);
				chanceOnDirt = Mathf.Clamp(Random.Range(0.00f, 0.00f), 0f, 1f);
				chanceOnGrass = Mathf.Clamp(Random.Range(0.00f, 0.00f), 0f, 1f);
			}

			//Adjustment to rainforest climate in respect to world height
			else if (climate.WorldClimate == (int)MapsFile.Climates.Rainforest)
			{
				chanceOnGrass = /* Random.Range(0.035f,  */0.30f/* ) */;
				chanceOnDirt = /* Random.Range(0.120f,  */0.30f/* ) */;
				chanceOnStone = /* Random.Range(0.120f,  */0.30f/* ) */;
			}

			//Adjustment to subtropical climate in respect to world height
			else if (climate.WorldClimate == (int)MapsFile.Climates.Subtropical)
			{
				chanceOnStone = Mathf.Clamp(Random.Range(0.00f, 0.00f), 0f, 1f);
				chanceOnDirt = Mathf.Clamp(Random.Range(0.00f, 0.00f), 0f, 1f);
				chanceOnGrass = Mathf.Clamp(Random.Range(0.00f, 0.00f), 0f, 1f);
			}
		}
	}
}