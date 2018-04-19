using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire_Arrow : MonoBehaviour {

    public float att = 30.0f; //攻擊力
    public float Speed = 30.0f;
    public int Penetration = 1;
    public GameObject Arrow_FX;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
	}

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Tag_Wall") {
            Destroy(gameObject);
        }
        if (other.GetComponent<Collider>().tag == "Tag_Enemy") {
            Instantiate(Arrow_FX, transform.position, Quaternion.identity);
            Penetration -= 1;
            if (Penetration == 0) {
                Destroy(gameObject);
            }
            other.gameObject.BroadcastMessage("NPC_Hurt", att);
        }
    }
}
