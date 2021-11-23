using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Weather;

public class WOShootingStarController : MonoBehaviour
{
    int my_StartTime;
    int my_EndTime;

    public ParticleSystem ps;

    DaggerfallUnity dfUnity;
    WeatherManager weatherManager;

    // Start is called before the first frame update
    void Start()
    {
        dfUnity = GameObject.Find("DaggerfallUnity").GetComponent<DaggerfallUnity>();
        weatherManager = GameObject.Find("WeatherManager").GetComponent<WeatherManager>();
        my_StartTime = Random.Range(1045, 1100);
        my_EndTime = Random.Range(330, 360);
    }

    // Update is called once per frame
    void Update()
    {
        if ((dfUnity.WorldTime.Now.MinuteOfDay > my_StartTime || dfUnity.WorldTime.Now.MinuteOfDay < my_EndTime) &&
             weatherManager.PlayerWeather.WeatherType != WeatherType.Rain &&
             weatherManager.PlayerWeather.WeatherType != WeatherType.Rain_Normal &&
             weatherManager.PlayerWeather.WeatherType != WeatherType.Snow &&
             weatherManager.PlayerWeather.WeatherType != WeatherType.Snow_Normal &&
             weatherManager.PlayerWeather.WeatherType != WeatherType.Thunder &&
             weatherManager.PlayerWeather.WeatherType != WeatherType.Overcast &&
             weatherManager.PlayerWeather.WeatherType != WeatherType.Cloudy &&
             weatherManager.PlayerWeather.WeatherType != WeatherType.Fog) {
            if (!ps.isPlaying) {
                ps.Play();
            }
        } else if (dfUnity.WorldTime.Now.MinuteOfDay <= my_StartTime || dfUnity.WorldTime.Now.MinuteOfDay >= my_EndTime) {
            if (ps.isPlaying) {
                ps.Stop();
            }
        }
    }
}
