using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;

namespace WildernessOverhaul
{
	public class WildernessOverhaulMod : MonoBehaviour
	{
		static Mod mod;
		static WOTerrainTexturing woTexturing;
		static WOTerrainNature woNature;

		static Mod DREAMMod;
		static bool DREAMModEnabled;

        static Mod InterestingTerrainMod;
        static bool InterestingTerrainModEnabled;
        static Material terrainMaterial;

        static bool BasicRoadsModEnabled = false;

		bool dynamicVegetationClearance;
		bool vegetationInLocations;
		bool fireflies;
		bool shootingStars;
        float fireflyActivationDistance;
		float shootingStarsMin;
		float shootingStarsMax;
		float generalNatureClearance;
		float natureClearance1;
		float natureClearance2;
		float natureClearance3;
		float natureClearance4;
		float natureClearance5;

		[Invoke(StateManager.StateTypes.Start , 0)]
		public static void Init(InitParams initParams)
		{
			mod = initParams.Mod;
			var modGameObject = new GameObject(mod.Title);
			modGameObject.AddComponent<WildernessOverhaulMod>();
			if (ModManager.Instance.GetModFromGUID("5e1af2fc-2c12-4d05-829c-12b37f396e19") != null)
			{
				DREAMMod = ModManager.Instance.GetModFromGUID("5e1af2fc-2c12-4d05-829c-12b37f396e19");
				if (DREAMMod != null && DREAMMod.Enabled)
					DREAMModEnabled = true;
                    Debug.Log("Wilderness Overhaul: DREAM Mod is active");
			}
            if (ModManager.Instance.GetModFromGUID("d08bb628-ff2e-4e2f-ae57-1d4981e61843") != null)
            {
                InterestingTerrainMod = ModManager.Instance.GetModFromGUID("d08bb628-ff2e-4e2f-ae57-1d4981e61843");
                if (InterestingTerrainMod != null && InterestingTerrainMod.Enabled)
                    InterestingTerrainModEnabled = true;
                Debug.Log("Wilderness Overhaul: Interesting Terrain Mod is active");
            }
            Mod basicRoadsMod = ModManager.Instance.GetModFromGUID("566ab21a-22d8-4eea-8ccd-6cb8f7a7ed25");
            if (basicRoadsMod != null && basicRoadsMod.Enabled)
            {
                BasicRoadsModEnabled = true;
                Debug.Log("Wilderness Overhaul: Basic Roads Mod is active");
            }
        }

        void Start()
		{
			Debug.Log("Wilderness Overhaul: Initiating Mod");

			ModSettings settings = mod.GetSettings();
			dynamicVegetationClearance = settings.GetValue<bool>("TerrainNature", "DynamicVegetationClearance");
			vegetationInLocations = settings.GetValue<bool>("TerrainNature", "VegetationInsideJungleLocations");
			fireflies = settings.GetValue<bool>("Nature", "Fireflies");
			shootingStars = settings.GetValue<bool>("Nature", "ShootingStars");
            fireflyActivationDistance = settings.GetValue<float>("Nature", "FireflyActivationDistance");
			shootingStarsMin = settings.GetValue<float>("Nature", "ShootingStarsMinChance");
			shootingStarsMax = settings.GetValue<float>("Nature", "ShootingStarsMaxChance");
			generalNatureClearance = settings.GetValue<float>("TerrainNature", "GeneralNatureClearance");
			natureClearance1 = settings.GetValue<float>("DynamicNatureClearance", "Cities");
			natureClearance2 = settings.GetValue<float>("DynamicNatureClearance", "Hamlets");
			natureClearance3 = settings.GetValue<float>("DynamicNatureClearance", "Villages,Homes(Wealthy),ReligiousCults");
			natureClearance4 = settings.GetValue<float>("DynamicNatureClearance", "Farms,Taverns,Temples,Homes(Poor)");
			natureClearance5 = settings.GetValue<float>("DynamicNatureClearance", "Dungeons(Laybinths,Keeps,Ruins,Graveyards,Covens)");

			woNature = new WOTerrainNature(
				DREAMModEnabled,
                InterestingTerrainModEnabled,
				dynamicVegetationClearance,
				vegetationInLocations,
				fireflies,
				shootingStars,
                fireflyActivationDistance,
				shootingStarsMin,
				shootingStarsMax,
				generalNatureClearance,
				natureClearance1,
				natureClearance2,
				natureClearance3,
				natureClearance4,
				natureClearance5);

			woTexturing = new WOTerrainTexturing(InterestingTerrainModEnabled, BasicRoadsModEnabled);

			DaggerfallUnity.Instance.TerrainNature = woNature;
			DaggerfallUnity.Instance.TerrainTexturing = woTexturing;

			mod.IsReady = true;
			Debug.Log("Wilderness Overhaul: Mod Initiated");
		}
	}
}
