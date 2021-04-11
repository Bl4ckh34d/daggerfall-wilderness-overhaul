using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop;

public class WORandomMover : MonoBehaviour
{
  [SerializeField] [Range(0f, 10f)] float speed;
  [SerializeField] [Range(0.1f, 10f)] float posChangeTime;
  [SerializeField] [Range(0f, 5f)] float maxRoamingRangeVertical;
  [SerializeField] [Range(0f, 2.5f)] float maxRoamingRangeHorizontal;
  [SerializeField] bool randomizePosChangeTime = false;

  Rigidbody rb;
  DaggerfallUnity dfUnity;
  SpriteRenderer sRenderer;
  Camera m_Camera;
  Material m_Material;

  int my_StartTime;
  int my_EndTime;

  float light_offset;
  float pulseFactor;
  float m_Alpha = 0f;

  Vector3 startPos;
  Vector3 targetPos;
  Vector3 dirVect;

  bool isOn = false;
  bool init = true;

  float t = 0f;

  void Awake() {
    dfUnity = GameObject.Find("DaggerfallUnity").GetComponent<DaggerfallUnity>();
    rb = GetComponent<Rigidbody>();
    sRenderer = GetComponent<SpriteRenderer>();
    m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    m_Material = GetComponent<SpriteRenderer>().material;
  }

  void Start()
  {
    startPos = transform.position;
    my_StartTime = Random.Range(1030, 1100);
    my_EndTime = Random.Range(340, 370);
    light_offset = Random.Range(0f,1f);
    pulseFactor = Random.Range(0.3f,1.0f);
    targetPos = new Vector3(
      startPos.x + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal),
      startPos.y + Random.Range(-maxRoamingRangeVertical, maxRoamingRangeVertical),
      startPos.z + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal));
  }

  void FixedUpdate()
  {
    if (dfUnity.WorldTime.Now.MinuteOfDay > my_StartTime || dfUnity.WorldTime.Now.MinuteOfDay < my_EndTime) {
      if (!isOn) {
        init = true;
        m_Alpha = 0f;
        transform.position = startPos;
      }
      isOn = true;
    } else {
      if (isOn) {
        init = true;
      }
      isOn = false;
    }

    if (isOn && init) {
      sRenderer.enabled = true;
      if (m_Alpha < 1f) {
        m_Alpha += Time.fixedDeltaTime;
        m_Material.SetColor("_Color", new Color(1f, 1f, 1f, m_Alpha));
      } else {
        init = false;
      }
    }

    if (!isOn && init) {
      if (m_Alpha > 0f) {
        m_Alpha -= Time.fixedDeltaTime;
        m_Material.SetColor("_Color", new Color(1f, 1f, 1f, m_Alpha));
      }
      else {
        init = false;
      }
      sRenderer.enabled = false;
    }

    if (isOn && !init) {
      t += Time.fixedDeltaTime;

      m_Material.SetColor("_Color", new Color(1f, 1f, 1f, Mathf.PingPong(Time.time * pulseFactor, 1f)));

      if (randomizePosChangeTime) {
        if (t > Random.Range(0.3f, 3)) {
        t = 0f;
        targetPos = new Vector3(
        startPos.x + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal),
        startPos.y + Random.Range(-maxRoamingRangeVertical, maxRoamingRangeVertical),
        startPos.z + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal));
        }
      } else {
        if (t > posChangeTime) {
          t = 0f;
          targetPos = new Vector3(
          startPos.x + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal),
          startPos.y + Random.Range(-maxRoamingRangeVertical, maxRoamingRangeVertical),
          startPos.z + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal));
          if (randomizePosChangeTime) {
            speed = Random.Range(0.1f, 1.5f);
          }
        }
      }

      dirVect = targetPos - transform.localPosition;
      if (Time.timeScale == 1f) {
        rb.AddForce(dirVect * speed);
      } else {
        transform.position = startPos;
      }
    }

    transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
  }
}
