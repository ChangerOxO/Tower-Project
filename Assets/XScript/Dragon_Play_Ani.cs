using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon_Play_Ani : MonoBehaviour {

    public bool Play_Water_sFX = false;
    public float att = 100;

	// Use this for initialization
	void Start () {
        GetComponent<Animation>().Stop();
    }
	
	// Update is called once per frame
	void Update () {

	}

    void PlayDragonAni() {
        GetComponent<Animation>().Play("ani_Dragon");
        GetComponent<AudioSource>().Play();
        Destroy(gameObject.transform.parent.gameObject, 1.7f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tag_Floor")
        {
            gameObject.transform.parent.gameObject.BroadcastMessage("PlayWater_sFX", true);
        }
        if (other.tag == "Tag_Enemy") {
            other.gameObject.BroadcastMessage("NPC_Hurt", att);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Tag_Floor")
        {
            gameObject.transform.parent.gameObject.BroadcastMessage("PlayWater_sFX", false);
        }
    }
}
