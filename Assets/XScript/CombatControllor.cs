using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControllor : MonoBehaviour {

    public float SpriteCount = 10;
    public float DeltaTime = 0.05f;
    public bool isActiveMove = true;

    //float xScale = 1;//X軸位置
    //float yScale = 1;//Y軸位置

    //float yPos = 0;//Y軸偏移量
    float NextTime = 0;//播放下一格時間

    float CurKeyFrame = 0;//目前影格
    float SpriteIndex = 0;//目前動畫索引值

    public float Att = 50f;

    // Use this for initialization
    void Start () {
        //yScale = 1 / SpriteCount;//設定貼圖X值比例大小
        NextTime = DeltaTime;//設定下一格間隔時間
    }
	
	// Update is called once per frame
	void Update () {
        if (isActiveAndEnabled) {
            if (Time.time > NextTime && SpriteIndex <= 1) {
                CurKeyFrame++;
                NextTime = Time.time + DeltaTime;
                SpriteIndex += 1 / SpriteCount;
            }
            GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, SpriteIndex);
        }
        if (SpriteIndex >= 1) {
            Destroy(gameObject, 0.05f);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Collider>().tag == "Tag_Enemy") {
            //Instantiate(Arrow_FX, transform.position, Quaternion.identity);
            other.gameObject.BroadcastMessage("NPC_Hurt", Att);
        }
    }
}