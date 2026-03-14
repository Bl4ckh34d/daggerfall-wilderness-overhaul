using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Utility.ModSupport;

[System.Serializable]
public class WOFireflyProfile
{
  public Color bodyColor = Color.white;
  public Color haloColor = new Color(1f, 1f, 1f, 1f);
  public float haloAlpha = 0.1f;
  public float activationDistance = 800f;
  public float minSpawnHeight = 0.5f;
  public float maxSpawnHeight = 1.5f;
  public float minSpeed = 0.7f;
  public float maxSpeed = 1.2f;
  public float minVerticalRange = 1.5f;
  public float maxVerticalRange = 2.4f;
  public float minHorizontalRange = 2.0f;
  public float maxHorizontalRange = 3.6f;
  public float minTargetChangeTime = 0.4f;
  public float maxTargetChangeTime = 2.6f;
  public float minPulseFactor = 0.15f;
  public float maxPulseFactor = 0.65f;
  public int nightStartMin = 1015;
  public int nightStartMax = 1100;
  public int nightEndMin = 320;
  public int nightEndMax = 375;
  public bool randomizeHuePerInstance = false;
  public float hueVariance = 0.05f;
  public bool cycleHue = false;
  public float hueShiftSpeed = 0.02f;
  public float huePhaseVariance = 0.15f;
}

public class WODistanceChecker : MonoBehaviour
{

  Transform playerTransform;
  float counter;
  bool firefliesActive = false;
  public float distance;

  public List<WORandomMover> allChildren;

  public GameObject firefly;

  void Awake()
  {
    counter = Random.Range(0.0f,2.0f);
    playerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
  }

  public void CreateFirefly(Mod mod, DaggerfallTerrain dfTerrain, int x, int y, float scale, Terrain terrain, float distance, WOFireflyProfile profile) {
    float xVariation = Random.Range(-distance, distance);
    float zVariation = Random.Range(-distance, distance);

    //Vector3 pos = new Vector3(((x + xVariation) * scale), 0, ((y + zVariation) * scale)) + dfTerrain.transform.position;
    Vector3 pos = transform.position + new Vector3(xVariation * scale, 0, zVariation * scale);
    //pos.y = terrain.SampleHeight(new Vector3((x + xVariation) * scale, 0, (y + zVariation) * scale) + dfTerrain.transform.position) + dfTerrain.transform.position.y + Random.Range(1.5f, 3f);
    pos.y = terrain.SampleHeight(pos) + Random.Range(profile.minSpawnHeight * scale, profile.maxSpawnHeight * scale);

    GameObject firefly = mod.GetAsset<GameObject>("Firefly", true);
    firefly.transform.parent = transform;
    WORandomMover mover = firefly.GetComponent<WORandomMover>();
    mover.ApplyProfile(profile);
    mover.startPos = transform.InverseTransformPoint(pos);
    //firefly.transform.position = transform.InverseTransformPoint(pos);
    firefly.transform.position = pos;
  }

  public void AddChildrenToArray() {
    allChildren = GetComponentsInChildren<WORandomMover>().ToList();
  }

  public void DeactivateAllChildren() {
    foreach(WORandomMover firefly in allChildren) {
      firefly.gameObject.SetActive(false);
    }
  }

  public void ActivateAllChildren() {
    foreach(WORandomMover firefly in allChildren) {
      firefly.gameObject.SetActive(true);
    }
  }

  void FixedUpdate()
  {
    if (counter <= 0) {
      if (Vector3.Distance(playerTransform.position, transform.position) <= distance) {
        if (!firefliesActive) {
          ActivateAllChildren();
          foreach(WORandomMover firefly in allChildren) {
            firefly.ToggleActivation(true);
            firefliesActive = true;
          }
        }
      } else {
        if (firefliesActive) {
          DeactivateAllChildren();
          foreach(WORandomMover firefly in allChildren) {
            firefly.ToggleActivation(false);
            firefliesActive = false;
          }
        }
      }
      counter = 2f;
    } else {
      counter -= Time.fixedDeltaTime;
    }
  }
}
