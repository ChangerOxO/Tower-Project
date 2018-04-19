using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public GameObject NPC_Player;
    public float OriginSpeed = 5;
    float Speed = 5;
    public float HP = 100;
    public float MaxHP = 100;
    GameObject HP_Bar;
    float HP_Bar_Width;
    bool bHurt = false;
    bool Attack = false;
    public AnimationClip[] Ani = new AnimationClip[3];
    public GameObject NPC_Soul;

    public AudioClip hitVoice;
    public AudioClip hitWall;

    int NPC_Att;
    public int NPC_Max_Att;
    public int NPC_Min_Att;
    GameObject Player;
    public bool IsLoadFlag = false;

	// Use this for initialization
	void Start () {
        if (!IsLoadFlag) {
            HP = MaxHP;
            Speed = OriginSpeed;
        }
        Player = GameObject.FindGameObjectWithTag("Tag_GameManger");   
        
        HP_Bar = transform.Find("Canvas").Find("Image_HP").gameObject;
        HP_Bar_Width = HP_Bar.GetComponent<RectTransform>().sizeDelta.x;    
        
        GetComponent<Animation>().clip = Ani[0];
        GetComponent<Animation>().Play();
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        if (NPC_Player.GetComponent<Animation>()[Ani[1].name.ToString()].normalizedTime >= 0.3) {
            if (!Attack) {
                GetComponent<AudioSource>().clip = hitWall;
                GetComponent<AudioSource>().Play();
                Attack = true;
                NPC_Att = Random.Range(NPC_Min_Att, NPC_Max_Att);
                Player.BroadcastMessage("Player_Hurt", NPC_Att);
                Invoke("Att", NPC_Player.GetComponent<Animation>()[Ani[1].name.ToString()].length);
            }
        }

        HP_Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(HP_Bar_Width * (HP / MaxHP)
            , HP_Bar.GetComponent<RectTransform>().sizeDelta.y);
    }

    void Att() {
        Attack = false;
    }

    void NPC_Hurt(float att) {
        if (!bHurt) {            
            bHurt = true;
            transform.Translate(Vector3.back * Time.deltaTime * 40f);
            HP -= att;
            GetComponent<AudioSource>().clip = hitVoice;
            //GetComponent<AudioSource>().volume = 0.3f;
            GetComponent<AudioSource>().Play();
            if (Speed == 0) {
                Speed = OriginSpeed/2;
                NPC_Player.GetComponent<Animation>().Stop();
                NPC_Player.GetComponent<Animation>().clip = Ani[0];
                NPC_Player.GetComponent<Animation>().Play();
            }
            if (HP <= 0)
            {
                GetComponent<Collider>().enabled = false;
                Speed = 0;
                GetComponent<Animation>().Stop();
                GetComponent<Animation>().clip = Ani[2];
                GetComponent<Animation>().Play();
                Vector3 soul_Pos = new Vector3(transform.localPosition.x, transform.localPosition.y + 5, transform.localPosition.z - 5);
                Instantiate(NPC_Soul, soul_Pos, Quaternion.identity);
                Destroy(gameObject, 1.5f);
                Player.BroadcastMessage("Enemy_Dead");
            }
            else StartCoroutine(NPCHurt());
        }
        bHurt = false;
    }

    void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Collider>().tag == "DefenceWall") {
            Speed = 0;
            NPC_Player.GetComponent<Animation>().Stop();
            NPC_Player.GetComponent<Animation>().clip = Ani[1];
            NPC_Player.GetComponent<Animation>().Play();
        }
        if (other.tag == "Tag_Wall") {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.GetComponent<Collider>().tag == "DefenceWall" 
            && !NPC_Player.GetComponent<Animation>().isPlaying) {
            Speed = 0;
            NPC_Player.GetComponent<Animation>().Stop();
            NPC_Player.GetComponent<Animation>().clip = Ani[1];
            NPC_Player.GetComponent<Animation>().Play();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.GetComponent<Collider>().tag == "DefenceWall") {
            Speed = OriginSpeed;
            NPC_Player.GetComponent<Animation>().Stop();
            NPC_Player.GetComponent<Animation>().clip = Ani[0];
            NPC_Player.GetComponent<Animation>().Play();
        }
    }

    IEnumerator NPCHurt() {
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Collider>().enabled = true;
    }
}
