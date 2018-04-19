using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManger : MonoBehaviour {

    [Header("System")]
    bool IsLoadFlag = false;

    [Header("Player")]
    public GameObject player;
    public AnimationClip ani;

    bool bCanCreateArrow = false;
    bool bCanCreateCombat = false;
    RaycastHit hit;
    Vector3 target_Pos;
    Vector3 look_Pos;

    public GameObject fire_Pos;
    public GameObject Arrow_A;
    public GameObject Arrow_B;
    public GameObject Arrow_C;
    public int Arrow_3_Count = 10;
    
    public int ArrowPenetration = 1;
    float ArrowDamageMult = 1;

    public GameObject Combat_Pos;
    public GameObject Combat;
    public GameObject floor;

    public float PlayerMaxHP = 100;
    float PlayerHP;
    public float PlayerMaxMP = 50;
    float PlayerMP;
    string ArrowType = "A";

    [Header("NPC")]
    public GameObject NPC_Create_Pos;
    public GameObject NPC;
    float NPC_Real_Time = 0;
    float NPC_Born_Time = 0;
    int NPC_Cur_Num = 0;
    public int NPC_Max_Num = 15;
    float EnemyHPMult = 1;

    [Header("Wave")]
    int Wave = 0;
    bool IsNextWave = false;

    [Header("Magic")]
    public GameObject Magic_Obj_Ani;
    GameObject magic_obj;
    //float magic_Cur_Time = 0;
    //float magic_Total_Time = 5;
    bool bCanCreateMagic = true;
    bool bCanDragMagic = false;

    [Header("UI")]
    public Image PlayerHP_Bar;
    float PlayerHP_Bar_Width;
    public Image PlayerMP_Bar;
    float PlayerMP_Bar_Width;
    public Image EnemyNum_Bar;
    float EnemyNum_Bar_Width;
    float val_Enemy;

    public Text Text_SoulNum;
    public int Soul = 50;
    public int OriginSoul = 50; 
    //public Image[] Image_Num;

    public Text Text_ArrowDamageMult;
    public Text Text_AttSpeed;
    public Text Text_ArrowPenetration;

    public GameObject Image_NextWave;
    public GameObject Btn_NextWave;
    
    void Start () {
        if (!IsLoadFlag) {
            PlayerHP = PlayerMaxHP;
            PlayerMP = PlayerMaxMP;
        }
        player.GetComponent<Animation>().clip = ani;
        player.GetComponent<Animation>().Stop();
        PlayerHP_Bar_Width = PlayerHP_Bar.GetComponent<RectTransform>().sizeDelta.x;
        PlayerMP_Bar_Width = PlayerMP_Bar.GetComponent<RectTransform>().sizeDelta.x;
        EnemyNum_Bar_Width = EnemyNum_Bar.GetComponent<RectTransform>().sizeDelta.x;
        Text_ArrowDamageMult.text = ArrowDamageMult.ToString("#0.0");
        Text_AttSpeed.text = player.GetComponent<Animation>()["moon_att"].speed.ToString("#0.0");
        Text_ArrowPenetration.text = ArrowPenetration.ToString();
        Refresh_Bar("All");
    }
	
	void Update () {
        CreateNPC();

        //按下滑鼠左鍵
        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100)) {
                if ((hit.collider.tag == "Tag_Floor" || hit.collider.tag == "Tag_Enemy") && !bCanDragMagic) {
                    if (hit.collider.tag == "Tag_Enemy") target_Pos = new Vector3(hit.point.x, player.transform.position.y, hit.point.z);
                    else target_Pos = new Vector3(hit.point.x + 1.5f, player.transform.position.y, hit.point.z);
                    //使用內插法計算玩家角色注視位置
                    look_Pos = Vector3.Lerp(look_Pos, target_Pos, 1);
                    player.transform.LookAt(look_Pos);                    
                    if (!player.GetComponent<Animation>().isPlaying) { //播動畫
                        player.GetComponent<Animation>().Stop();
                        player.GetComponent<Animation>().Play();
                        bCanCreateArrow = true;
                    }
                }
                if (magic_obj && bCanDragMagic) {
                    if (hit.collider.tag == "Tag_Floor") {
                        magic_obj.transform.position = hit.point;
                    }
                }
            }
        }
        
        //按下滑鼠右鍵
        if (Input.GetMouseButton(1)) {
            if (!player.GetComponent<Animation>().isPlaying) { //播動畫
                player.GetComponent<Animation>().Stop();
                player.GetComponent<Animation>().Play();
                bCanCreateCombat = true;
            }
        }

        //Player做攻擊動畫時射出箭
        if (player.GetComponent<Animation>().isPlaying) {
            if (player.GetComponent<Animation>()["moon_att"].normalizedTime >= 0.5 && bCanCreateArrow) {
                CreateArrow();
                bCanCreateArrow = false;
            }
            else if (player.GetComponent<Animation>()["moon_att"].normalizedTime >= 0.3 && bCanCreateCombat) {
                CreateCombat();
                bCanCreateCombat = false;
            }
        }

        //魔法判定
        if (Button_Manger._press && bCanCreateMagic && PlayerMP >= 10) {
            bCanCreateMagic = false;
            bCanDragMagic = true;
            magic_obj = Instantiate(Magic_Obj_Ani, new Vector3(200, 200, 200), Quaternion.identity);
        }
        else if(!Button_Manger._press && !bCanCreateMagic && magic_obj) {
            bCanCreateMagic = true;
            bCanDragMagic = false;
            magic_obj.BroadcastMessage("PlayDragonAni");
            PlayerMP -= 10;
        }

        if (PlayerHP <= 0) {

        }

        //回魔 & 魔力Bar更新
        PlayerMP = Mathf.Min(1 * Time.deltaTime + PlayerMP, PlayerMaxMP);

        //UI更新
        Refresh_Bar("PlayerMP");
        Text_ArrowDamageMult.text = ArrowDamageMult.ToString("#0.0");
        Text_AttSpeed.text = player.GetComponent<Animation>()["moon_att"].speed.ToString("#0.0");
        Text_ArrowPenetration.text = ArrowPenetration.ToString();
        Text_SoulNum.text = Soul + " / " + OriginSoul;
    }

    void CreateArrow() {
        GameObject perfab_Arrow;
        if (ArrowType == "A") {
            perfab_Arrow = Instantiate(Arrow_A, fire_Pos.transform.position, player.transform.rotation) as GameObject;
            perfab_Arrow.transform.Rotate(Vector3.up * 360);
            perfab_Arrow.GetComponent<Fire_Arrow>().Penetration = ArrowPenetration;
            perfab_Arrow.GetComponent<Fire_Arrow>().att *= ArrowDamageMult;
        }
        else if (ArrowType == "B") {
            perfab_Arrow = Instantiate(Arrow_B, fire_Pos.transform.position, player.transform.rotation) as GameObject;
            perfab_Arrow.transform.Rotate(Vector3.up * 360);
            perfab_Arrow.GetComponent<Fire_Arrow>().Penetration = ArrowPenetration;
            perfab_Arrow.GetComponent<Fire_Arrow>().att *= ArrowDamageMult;
        }
        else if (ArrowType == "C") {
            for (int i = 0; i < Arrow_3_Count; i++)
            {
                float a = i * 5f;
                float n = 360 - (5 * Arrow_3_Count) / 2;
                perfab_Arrow = Instantiate(Arrow_C, fire_Pos.transform.position, player.transform.rotation) as GameObject;
                perfab_Arrow.transform.Rotate(new Vector3(0, n + a, 0));
                perfab_Arrow.transform.Rotate(Vector3.up * 360);
                perfab_Arrow.GetComponent<Fire_Arrow>().Penetration = ArrowPenetration;
                perfab_Arrow.GetComponent<Fire_Arrow>().att *= ArrowDamageMult;
            }
        }
    }

    void CreateCombat() {
        GameObject perfab_Combat;
        perfab_Combat = Instantiate(Combat, Combat_Pos.transform.position, Quaternion.Euler(90, 0, 90));
        perfab_Combat.GetComponent<CombatControllor>().Att *= ArrowDamageMult;
    }

    void CreateNPC() {
        if (!IsNextWave) {
            if (NPC_Cur_Num <= 0) {
                WaveEnd(true);
            }
            else if (NPC_Real_Time >= NPC_Born_Time) {
                Vector3 Max_Start = NPC_Create_Pos.GetComponent<Collider>().bounds.max;
                Vector3 Min_Start = NPC_Create_Pos.GetComponent<Collider>().bounds.min;
                Vector3 Random_Pos = new Vector3(Random.Range(Min_Start.x, Max_Start.x), 0, Max_Start.z);

                GameObject NPC_Perfab = Instantiate(NPC, Random_Pos, Quaternion.Euler(0, 180, 0)) as GameObject;
                NPC_Perfab.GetComponent<EnemyController>().HP *= EnemyHPMult;
                NPC_Perfab.GetComponent<EnemyController>().MaxHP *= EnemyHPMult;

                NPC_Born_Time = Random.Range(1.5f, 3f);
                NPC_Real_Time = 0;

                Refresh_Bar("EnemyNum");
            }
            else {
                NPC_Real_Time += Time.deltaTime;
            }
        }        
    }

    void Player_Hurt(float att) {
        PlayerHP = Mathf.Max(PlayerHP - att, 0);
        Refresh_Bar("PlayerHP");
    }

    void Enemy_Dead() {
        NPC_Cur_Num = Mathf.Max(NPC_Cur_Num - 1, 0);
        Refresh_Bar("EnemyNum");
        Soul++;
        OriginSoul++;
    }

    void WaveEnd(bool WinOrLose) { //True is Win
        if (WinOrLose) {
            Btn_NextWave.SetActive(true);
            IsNextWave = true;
        }
        else {

        }
    }

    void Refresh_Bar(string name) {
        switch (name.ToLower()) {
            case "enemynum":
                EnemyNum_Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    EnemyNum_Bar_Width * ((float)NPC_Cur_Num / (float)NPC_Max_Num),
                    EnemyNum_Bar.GetComponent<RectTransform>().sizeDelta.y);
                break;
            case "enemy":

                break;
            case "playerhp":
                PlayerHP_Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    PlayerHP_Bar_Width * (PlayerHP / PlayerMaxHP), 
                    PlayerMP_Bar.GetComponent<RectTransform>().sizeDelta.y);
                break;
            case "playermp":
                PlayerMP_Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    PlayerMP_Bar_Width * (PlayerMP / PlayerMaxMP), 
                    PlayerMP_Bar.GetComponent<RectTransform>().sizeDelta.y);
                break;
            case "all":
                EnemyNum_Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    EnemyNum_Bar_Width * ((float)NPC_Cur_Num / (float)NPC_Max_Num),
                    EnemyNum_Bar.GetComponent<RectTransform>().sizeDelta.y);
                PlayerHP_Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    PlayerHP_Bar_Width * (PlayerHP / PlayerMaxHP),
                    PlayerMP_Bar.GetComponent<RectTransform>().sizeDelta.y);
                PlayerMP_Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    PlayerMP_Bar_Width * (PlayerMP / PlayerMaxMP),
                    PlayerMP_Bar.GetComponent<RectTransform>().sizeDelta.y);
                break;
        }
    }

    //能力改變BtnFunction
    public void AttSpeedUp() {
        if (Soul > 0) {
            player.GetComponent<Animation>()["moon_att"].speed += 0.2f;
            Soul--;
        }
        else {

        }
    }
    public void ArrowAttDamegeUp() {
        if (Soul > 0) {
            ArrowDamageMult += 0.1f;
            Soul--;
        }        
    }
    public void ArrowPenetrationUp() {
        if (Soul > 0) {
            ArrowPenetration += 1;
            Soul--;
        }        
    }
    public void ChangeArrowType(string Type) {
        ArrowType = Type;
    }

    public void Btn_NextWait() {
        EnemyHPMult = 1 +  ((float)Wave / 10);
        IsNextWave = false;
        StartCoroutine(SetNextWaveImage());
        NPC_Cur_Num = NPC_Max_Num;
        Wave++;
    }

    //存檔&讀取
    public void Save() {
        //填寫PlayerState格式的資料
        PlayerState saveData_Player = new PlayerState();
        saveData_Player.AttSpeed = player.GetComponent<Animation>()["moon_att"].speed;
        saveData_Player.ArrowDamageMult = ArrowDamageMult;
        saveData_Player.ArrowPenetration = ArrowPenetration;
        saveData_Player.ArrowType = ArrowType;
        saveData_Player.PlayerHP = PlayerHP;
        saveData_Player.PlayerMP = PlayerMP;
        //填寫EnemyState格式的資料
        EnemyState saveData_Enemy = new EnemyState();
        saveData_Enemy.EnemyPos = new List<Vector3>();
        saveData_Enemy.EnemyHP = new List<float>();
        foreach (var item in GameObject.FindGameObjectsWithTag("Tag_Enemy"))
        {
            saveData_Enemy.EnemyPos.Add(item.transform.position);
            saveData_Enemy.EnemyHP.Add(item.GetComponent<EnemyController>().HP);
        }

        //轉換成json格式的字串
        string saveJson_Player = JsonUtility.ToJson(saveData_Player);
        string saveJson_Enemy = JsonUtility.ToJson(saveData_Enemy);

        //將字串saveString存到硬碟中
        StreamWriter file = new StreamWriter(Path.Combine(Application.streamingAssetsPath, "SaveData_Player"));
        file.Write(saveJson_Player);
        file.Close();
        file = new StreamWriter(Path.Combine(Application.streamingAssetsPath, "SaveData_Enemy"));
        file.Write(saveJson_Enemy);
        file.Close();
    }

    public void Load() {
        IsLoadFlag = true;
        foreach (var item in GameObject.FindGameObjectsWithTag("Tag_Enemy")) {
            Destroy(item);
        }
        
        //讀取json檔案並轉存成文字格式
        StreamReader file = new StreamReader(Path.Combine(Application.streamingAssetsPath, "SaveData_Player"));
        string loadJson_Player = file.ReadToEnd();
        file.Close();
        file = new StreamReader(Path.Combine(Application.streamingAssetsPath, "SaveData_Enemy"));
        string loadJson_Enemy = file.ReadToEnd();
        file.Close();

        //新增一個物件類型為playerState的變數 loadData
        PlayerState loadData_Player = new PlayerState();
        EnemyState loadData_Enemy = new EnemyState();

        //使用JsonUtillty的FromJson方法將存文字轉成Json
        loadData_Player = JsonUtility.FromJson<PlayerState>(loadJson_Player);
        loadData_Enemy = JsonUtility.FromJson<EnemyState>(loadJson_Enemy);

        //放入數值
        player.GetComponent<Animation>()["moon_att"].speed = loadData_Player.AttSpeed;
        ArrowDamageMult = loadData_Player.ArrowDamageMult;
        ArrowPenetration = loadData_Player.ArrowPenetration;
        ArrowType = loadData_Player.ArrowType;
        PlayerHP = loadData_Player.PlayerHP;
        PlayerMP = loadData_Player.PlayerMP;

        for (int i = 0; i < loadData_Enemy.EnemyPos.Count; i++)
        {
            GameObject loadNPC = Instantiate(NPC, loadData_Enemy.EnemyPos[i], Quaternion.Euler(0, 180, 0));
            loadNPC.GetComponent<EnemyController>().HP = loadData_Enemy.EnemyHP[i];
            loadNPC.GetComponent<EnemyController>().IsLoadFlag = IsLoadFlag;
        }
        IsLoadFlag = false;
    }

    IEnumerator SetNextWaveImage() {
        Image_NextWave.SetActive(true);
        yield return new WaitForSeconds(2f);
        Image_NextWave.SetActive(false);
    }

    public class PlayerState {
        public float AttSpeed;
        public float ArrowDamageMult;
        public int ArrowPenetration;
        public string ArrowType;
        public float PlayerHP;
        public float PlayerMP;
    }

    public class EnemyState {
        public List<Vector3> EnemyPos;
        public List<float> EnemyHP;
    }

    public class WaveState {
        public int Wave;
    }
}
