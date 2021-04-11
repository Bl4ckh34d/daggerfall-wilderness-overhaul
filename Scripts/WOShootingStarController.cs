using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop;

public class WOShootingStarController : MonoBehaviour
{
    int my_StartTime;
    int my_EndTime;

    public ParticleSystem ps;

    DaggerfallUnity dfUnity;

    // Start is called before the first frame update
    void Start()
    {
        dfUnity = GameObject.Find("DaggerfallUnity").GetComponent<DaggerfallUnity>();
        my_StartTime = Random.Range(1045, 1100);
        my_EndTime = Random.Range(330, 360);
    }

    // Update is called once per frame
    void Update()
    {
        if (dfUnity.WorldTime.Now.MinuteOfDay > my_StartTime || dfUnity.WorldTime.Now.MinuteOfDay < my_EndTime) {
            if (!ps.isPlaying) {
                Debug.Log("Playing particle system");
                ps.Play();
            }
        } else if (dfUnity.WorldTime.Now.MinuteOfDay <= my_StartTime || dfUnity.WorldTime.Now.MinuteOfDay >= my_EndTime) {
            if (ps.isPlaying) {
                ps.Stop();
                Debug.Log("Playing particle system");
            }
        }
    }
}
