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

        bool dynamicVegetationClearance;
        bool vegetationInLocations;
        bool fireflies;
        float generalNatureClearance;
        float natureClearance1;
        float natureClearance2;
        float natureClearance3;
        float natureClearance4;
        float natureClearance5;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<WildernessOverhaulMod>();
            if (ModManager.Instance.GetModFromGUID("5e1af2fc-2c12-4d05-829c-12b37f396e19") != null) {
                DREAMMod = ModManager.Instance.GetModFromGUID("5e1af2fc-2c12-4d05-829c-12b37f396e19");
                if (DREAMMod != null && DREAMMod.Enabled)
                    DREAMModEnabled = true;
            }
        }

        void Awake()
        {
            Debug.Log("Wilderness Overhaul: initiating mod");

            ModSettings settings = mod.GetSettings();
            dynamicVegetationClearance = settings.GetValue<bool>("TerrainNature", "DynamicVegetationClearance");
            vegetationInLocations = settings.GetValue<bool>("TerrainNature", "VegetationInsideJungleLocations");
            fireflies = settings.GetValue<bool>("Wildlife", "Fireflies");
            generalNatureClearance = settings.GetValue<float>("TerrainNature", "GeneralNatureClearance");
            natureClearance1 = settings.GetValue<float>("DynamicNatureClearance", "Cities");
            natureClearance2 = settings.GetValue<float>("DynamicNatureClearance", "Hamlets");
            natureClearance3 = settings.GetValue<float>("DynamicNatureClearance", "Villages,Homes(Wealthy),ReligiousCults");
            natureClearance4 = settings.GetValue<float>("DynamicNatureClearance", "Farms,Taverns,Temples,Homes(Poor)");
            natureClearance5 = settings.GetValue<float>("DynamicNatureClearance", "Dungeons(Laybinths,Keeps,Ruins,Graveyards,Covens)");

            woNature = new WOTerrainNature(
                DREAMModEnabled,
                dynamicVegetationClearance,
                vegetationInLocations,
                fireflies,
                generalNatureClearance,
                natureClearance1,
                natureClearance2,
                natureClearance3,
                natureClearance4,
                natureClearance5);
            woTexturing = new WOTerrainTexturing();
            DaggerfallUnity.Instance.TerrainNature = woNature;
            DaggerfallUnity.Instance.TerrainTexturing = woTexturing;

            mod.IsReady = true;
            Debug.Log("Wilderness Overhaul: mod initiated");
        }
    }
}
