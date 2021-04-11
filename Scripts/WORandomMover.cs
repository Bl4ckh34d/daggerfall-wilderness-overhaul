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
  int my_StartTime;
  int my_EndTime;

  Vector3 startPos;
  Vector3 targetPos;
  Vector3 dirVect;
  Camera m_Camera;
  float light_offset;
  float pulseFactor;
  Material m_Material;
  Color m_Color;
  float m_Alpha = 0f;
  DaggerfallUnity dfUnity;
  bool isOn = false;
  bool init = true;

  float t = 0f;

  void Awake() {
    dfUnity = GameObject.Find("DaggerfallUnity").GetComponent<DaggerfallUnity>();
  }

  void Start()
  {
    rb = GetComponent<Rigidbody>();
    m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    m_Material = GetComponent<SpriteRenderer>().material;
    m_Color = m_Material.GetColor("_Color");
    my_StartTime = Random.Range(1050, 1110);
    my_EndTime = Random.Range(330, 390);
    light_offset = Random.Range(0f,1f);
    pulseFactor = Random.Range(0.3f,1.0f);
    startPos = transform.position;
    targetPos = new Vector3(
      startPos.x + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal),
      startPos.y + Random.Range(-maxRoamingRangeVertical, maxRoamingRangeVertical),
      startPos.z + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal));
  }

  void Update()
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
      gameObject.GetComponent<SpriteRenderer>().enabled = true;
      if (m_Alpha < 1f) {
        m_Alpha += Time.deltaTime;
        m_Color = new Color(m_Color.r, m_Color.g, m_Color.b, m_Alpha);
        m_Material.SetColor("_Color", m_Color);
      } else {
        init = false;
      }
    }

    if (!isOn && init) {
      if (m_Alpha > 0f) {
        m_Alpha -= Time.deltaTime;
        m_Color = new Color(m_Color.r, m_Color.g, m_Color.b, m_Alpha);
        m_Material.SetColor("_Color", m_Color);
      }
      else {
        init = false;
      }
      GetComponent<SpriteRenderer>().enabled = false;
    }

    if (isOn && !init) {
      t += Time.deltaTime;

      m_Color = new Color(1f,1f,1f, Mathf.PingPong(Time.time * pulseFactor, 1f));
      m_Material.SetColor("_Color", m_Color);

      if (randomizePosChangeTime) {
        if (t > Random.Range(0.1f, 3)) {
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
        }
      }

      dirVect = targetPos - transform.localPosition;
      if (Time.timeScale != 0) {
        if (randomizePosChangeTime) {
            rb.AddForce(dirVect * Random.Range(0.1f, 1.5f));
        } else {
            rb.AddForce(dirVect * speed);
        }
      }

    }
  }

  void LateUpdate() {
    transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
  }
}
