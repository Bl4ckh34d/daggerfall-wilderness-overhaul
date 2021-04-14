using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WODistanceChecker : MonoBehaviour
{

  Transform playerTransform;
  float counter;
  bool firefliesActive = false;

  public List<WORandomMover> allChildren;

  public GameObject firefly;

  void Awake()
  {
    counter = Random.Range(0.0f,5.0f);
    //firefly = Resources.Load("Firefly") as GameObject;
    playerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
  }

  public void CreateFirefly(Vector3 position, int x, int y, float scale, Terrain terrain, float distance) {
    Vector3 pos = new Vector3(position.x + Random.Range(-distance, distance), 0, position.z + Random.Range(-distance, distance));
    pos.y = terrain.SampleHeight(pos) + Random.Range(1.5f, 4);
    var firefly = GameObject.Instantiate(Resources.Load("Firefly") as GameObject, pos, Quaternion.identity, transform);
    firefly.GetComponent<WORandomMover>().startPos = transform.InverseTransformPoint(pos);
  }

  public void AddChildrenToArray() {
    allChildren = GetComponentsInChildren<WORandomMover>().ToList();
  }

  public void DeactivateAllChildren() {
    foreach(WORandomMover rm in allChildren) {
      rm.gameObject.SetActive(false);
    }
  }

  public void ActivateAllChildren() {
    foreach(WORandomMover rm in allChildren) {
      rm.gameObject.SetActive(true);
    }
  }

  void FixedUpdate()
  {
    if (counter <= 0) {
      if (Vector3.Distance(playerTransform.position, transform.position) <= 200f) {
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
