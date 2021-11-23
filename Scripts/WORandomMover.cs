using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Weather;

public class WORandomMover : MonoBehaviour
{
  [SerializeField] [Range(0f, 10f)] float speed = 1;
  [SerializeField] [Range(0.1f, 10f)] float posChangeTime;
  [SerializeField] [Range(0f, 5f)] float maxRoamingRangeVertical = 2;
  [SerializeField] [Range(0f, 2.5f)] float maxRoamingRangeHorizontal = 3;

  DaggerfallUnity dfUnity;
  WeatherManager weatherManager;
  GameObject player;
  Camera mainCam;
  Rigidbody rb;
  Transform halo;
  SpriteRenderer m_Renderer;
  SpriteRenderer h_Renderer;
  Material m_Material;
  Material h_Material;

  int my_StartTime;
  int my_EndTime;

  float light_offset;
  float pulseFactor;
  float m_Alpha = 0f;

  public Vector3 startPos;
  Vector3 targetPos;

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
    weatherManager = GameObject.Find("WeatherManager").GetComponent<WeatherManager>();
    mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    rb = GetComponent<Rigidbody>();
    m_Renderer = GetComponent<SpriteRenderer>();
    m_Material = GetComponent<SpriteRenderer>().material;
    h_Material = transform.GetChild(0).GetComponent<SpriteRenderer>().material;
    halo = transform.GetChild(0);
    h_Renderer = halo.GetComponent<SpriteRenderer>();
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
      if ((dfUnity.WorldTime.Now.MinuteOfDay > my_StartTime || dfUnity.WorldTime.Now.MinuteOfDay < my_EndTime) &&
           weatherManager.PlayerWeather.WeatherType != WeatherType.Rain &&
           weatherManager.PlayerWeather.WeatherType != WeatherType.Snow &&
           weatherManager.PlayerWeather.WeatherType != WeatherType.Rain_Normal &&
           weatherManager.PlayerWeather.WeatherType != WeatherType.Snow_Normal &&
           weatherManager.PlayerWeather.WeatherType != WeatherType.Thunder)
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
        m_Renderer.enabled = true;
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
        m_Renderer.enabled = false;
      }

      if (isOn && !init)
      {
        t += Time.fixedDeltaTime;

        m_Alpha = -10 + (Mathf.PingPong(Time.time * pulseFactor, 1f) * 20);
        m_Material.SetColor("_Color", new Color(1f, 1f, 1f, m_Alpha));
        halo.transform.localScale = new Vector3(m_Alpha, m_Alpha, m_Alpha);

        if (m_Alpha > 0) {
          h_Renderer.enabled = true;
        } else {
          h_Renderer.enabled = false;
        }

        if (t > Random.Range(0.3f, 3))
        {
          t = 0f;
          targetPos = new Vector3(
          startPos.x + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal),
          startPos.y + Random.Range(-maxRoamingRangeVertical, maxRoamingRangeVertical),
          startPos.z + Random.Range(-maxRoamingRangeHorizontal, maxRoamingRangeHorizontal));
        }

        rb.AddForce((targetPos - transform.localPosition) * speed);
      }

      transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward, mainCam.transform.rotation * Vector3.up);
    }
  }
}
