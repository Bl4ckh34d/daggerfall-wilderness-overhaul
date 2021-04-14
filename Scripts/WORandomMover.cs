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
  Material h_Material;
  GameObject player;
  Transform glowLight;

  int my_StartTime;
  int my_EndTime;

  float light_offset;
  float pulseFactor;
  float m_Alpha = 0f;

  public Vector3 startPos;
  Vector3 targetPos;
  Vector3 dirVect;

  bool isOn = false;
  bool init = true;
  public bool isPerforming = false;

  float t = 0f;

  public void ToggleActivation(bool state)
  {
    if (state) {
      if (!isPerforming) {
        isPerforming = true;
      }
    } else {
      if (isPerforming) {
        isPerforming = false;
      }
    }
  }

  void Awake()
  {
    dfUnity = GameObject.Find("DaggerfallUnity").GetComponent<DaggerfallUnity>();
    m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    rb = GetComponent<Rigidbody>();
    sRenderer = GetComponent<SpriteRenderer>();
    m_Material = GetComponent<SpriteRenderer>().material;
    h_Material = transform.GetChild(0).GetComponent<SpriteRenderer>().material;
    glowLight = transform.GetChild(0);
  }

  void Start()
  {
    my_StartTime = Random.Range(1015, 1100);
    my_EndTime = Random.Range(320, 375);
    light_offset = Random.Range(0f, 1f);
    pulseFactor = Random.Range(0.1f, 0.75f);
    h_Material.SetColor("_Color", new Color(1f, 1f, 1f, 0.1f));
    targetPos = new Vector3(
     startPos.x + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal),
     startPos.y + Random.Range(-maxRoamingRangeVertical, maxRoamingRangeVertical),
     startPos.z + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal));
  }

  void FixedUpdate()
  {
    if (isPerforming) {
      if (dfUnity.WorldTime.Now.MinuteOfDay > my_StartTime || dfUnity.WorldTime.Now.MinuteOfDay < my_EndTime)
      {
        if (!isOn)
        {
          init = true;
          m_Alpha = -10f;
        }
        isOn = true;
      }
      else
      {
        if (isOn)
        {
          init = true;
        }
        isOn = false;
      }

      if (isOn && init)
      {
        sRenderer.enabled = true;
        if (m_Alpha < 10f)
        {
          m_Alpha += Time.fixedDeltaTime * 5;
          m_Material.SetColor("_Color", new Color(1f, 1f, 1f, m_Alpha));
          h_Material.SetColor("_Color", new Color(1f, 1f, 1f, Mathf.Clamp(m_Alpha,0,0.1f)));
        }
        else
        {
          init = false;
        }
      }

      if (!isOn && init)
      {
        if (m_Alpha > -10f)
        {
          m_Alpha -= Time.fixedDeltaTime * 5;
          m_Material.SetColor("_Color", new Color(1f, 1f, 1f, m_Alpha));
          h_Material.SetColor("_Color", new Color(1f, 1f, 1f, Mathf.Clamp(m_Alpha,0,0.1f)));
        }
        else
        {
          init = false;
        }
        sRenderer.enabled = false;
      }

      if (isOn && !init)
      {
        t += Time.fixedDeltaTime;

        m_Alpha = -10 + (Mathf.PingPong(Time.time * pulseFactor, 1f) * 20);
        m_Material.SetColor("_Color", new Color(1f, 1f, 1f, m_Alpha));
        glowLight.transform.localScale = new Vector3(m_Alpha, m_Alpha, m_Alpha);

        if (m_Alpha > 0) {
          glowLight.GetComponent<SpriteRenderer>().enabled = true;
        } else {
          glowLight.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (randomizePosChangeTime)
        {
          if (t > Random.Range(0.3f, 3))
          {
            t = 0f;
            targetPos = new Vector3(
            startPos.x + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal),
            startPos.y + Random.Range(-maxRoamingRangeVertical, maxRoamingRangeVertical),
            startPos.z + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal));
          }
        }
        else
        {
          if (t > posChangeTime)
          {
            t = 0f;
            targetPos = new Vector3(
            startPos.x + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal),
            startPos.y + Random.Range(-maxRoamingRangeVertical, maxRoamingRangeVertical),
            startPos.z + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal));
            if (randomizePosChangeTime)
            {
              speed = Random.Range(0.1f, 1.5f);
            }
          }
        }

        dirVect = targetPos - transform.localPosition;
        if (Time.timeScale == 1f)
        {
          rb.AddForce(dirVect * speed);
        }
      }

      transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
    }
  }
}
