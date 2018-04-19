using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_sFX : MonoBehaviour {
    
    void PlayWater_sFX(bool step) {
        if (step)
        {
            GetComponent<ParticleSystem>().Play();
            GetComponent<AudioSource>().Play();
        }
        else
        {
            GetComponent<ParticleSystem>().Stop();
        }
    }
}
