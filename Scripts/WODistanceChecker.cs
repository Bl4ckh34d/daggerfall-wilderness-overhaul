using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DaggerfallWorkshop;

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

  public void CreateFirefly(DaggerfallTerrain dfTerrain, int x, int y, float scale, Terrain terrain, float distance) {
    float xVariation = Random.Range(-distance, distance);
    float zVariation = Random.Range(-distance, distance);

    Vector3 pos = new Vector3(((x + xVariation) * scale), 0, ((y + zVariation) * scale)) + dfTerrain.transform.position;
    pos.y = terrain.SampleHeight(new Vector3((x + xVariation) * scale, 0, (y + zVariation) * scale) + dfTerrain.transform.position) + dfTerrain.transform.position.y + Random.Range(1.5f, 3f);

    var firefly = GameObject.Instantiate(Resources.Load("Firefly") as GameObject, pos, Quaternion.identity, transform);
    firefly.GetComponent<WORandomMover>().startPos = transform.InverseTransformPoint(pos);
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
