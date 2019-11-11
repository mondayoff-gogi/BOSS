using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Slider_class
{
    public Slider[] sliders;
}

public class GameManage : MonoBehaviour
{
    static public GameManage instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            //DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }  //--------------인스턴스화를 위함 ----


    public GameObject boss;
    private Camera _main;
    private Animator _animator;
    public GameObject selected_player;
    public Vector2 JoysitckDir;
    public GameObject[] player;

    public int num_char = 2;  //케릭터 죽는 경우 이 스크립트에서 num_char 줄어들게 해야함

    // 캐릭터 직업의 인덱스를 저장. ex) 0번에는 1번 캐릭터의 직업 인덱스가 저장됨
    public int[] character_index;

    // 보스의 인덱스를 저장.
    private int boss_index;

    // 솔로용
    [HideInInspector]
    public float[] Damage_toBoss;
    [HideInInspector]
    public float[] Damage_fromBoss;
    [HideInInspector]
    public float[] Heal;

    // 멀티용
    public float[,] Multi_Damage_toBoss;
    public float[,] Multi_Damage_fromBoss;
    public float[,] Multi_Heal;
    //--------------결과창을 위한 변수들 ----
    public Camera main_camera;
    // fade효과를 위한 판넬
    public GameObject fade_InOut;
    // fade효과를 캔버스
    public GameObject fade_canvas;
    // 결과창 캔버스
    public GameObject result_canvas;
    // 결과창 판넬
    public GameObject Single_result_panel;
    public GameObject Multi_panel;
    public GameObject Multi_result_panel;
    public GameObject Multi_statstics_panel;
    public Sprite defeat;
    public Sprite victory;
    public bool IsGameEnd;
    public Sprite[] character_images;

    public Image[] icon;
    public Image[] Multi_icon;

    public Slider_class[] Single_player_scroll;
    public Slider_class[] Multi_player_scroll;

    public Text[] Single_Player_DamageToBoss_Text;
    public Text[] Single_Player_DamageFromBoss_Text;
    public Text[] Single_Player_Heal_Text;

    public Text[] Multi_Player_DamageToBoss_Text;
    public Text[] Multi_player_DamageFromBoss_Text;
    public Text[] Multi_player_Heal_Text;

    public GameObject[] Single_result_character_panel;
    public GameObject[] Multi_result_character_panel;

    public GameObject Multi_Item_Button;
    public GameObject Multi_Statstics_Button;

    private float big_damage = 1;
    private float big_defence = 1;
    private float big_heal = 1;

    private List<Item> drop_item;
    private List<Item> get_item_list = new List<Item>();

    public GameObject grid;
    public GameObject[] item_display;
    private InventorySlot[] display_item;

    public GameObject Multi_grid;
    public GameObject[] Multi_item_display;
    private InventorySlot[] Multi_display_item;

    public Image item_icon;
    public Text item_name;
    public Text item_description;
    public Text item_count;

    public Image Multi_item_icon;
    public Text Multi_item_name;
    public Text Multi_item_description;
    public Text Multi_item_count;

    public GameObject Single_win_title;
    public GameObject Single_lose_title;

    public GameObject Multi_win_title;
    public GameObject Multi_lose_title;

    public GameObject getItem_button;
    public GameObject Multi_GetItem_Button;

    public Text[] Multi_Player_ID;

    public GameObject win_effect;
    private GameObject win_effect_temp;

    public GameObject lose_effect;
    private GameObject lose_effect_temp;

    private GameObject target_player;
    private Vector2 dir;

    // 난이도에 따른 아이템 확률 [난이도,장비타입]
    private float[,] drop_chance = new float[4, 3];

    public GameObject end_effect;

    [HideInInspector]
    public GameObject[] Character_dummy;
    [HideInInspector]
    public GameObject[] Character_other;
    [HideInInspector]
    public GameObject[] all_character_array;

    public int My_Character_num;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");

        all_character_array = new GameObject[4];

        if (NetworkManager.instance.is_multi)
        {
            if (NetworkManager.instance.My_character == null)
                NetworkManager.instance.My_character = player[0];

            Character_other = new GameObject[Character_dummy.Length];
            // take a look in PlayerManager.cs script
            //newPlayer = GameObject.Instantiate( local player avatar or model, spawn position, spawn rotation)
            for (int i = 0; i < NetworkManager.instance.Player_num; i++)
            {

                if (i == NetworkManager.instance.my_index)
                {
                    player[0].transform.position = NetworkManager.instance.spawnPoints[i].position;
                    all_character_array[i] = player[0];
                    continue;
                }
                Character_other[i] = Instantiate(Character_dummy[i], NetworkManager.instance.spawnPoints[i].position,Quaternion.identity);
                all_character_array[i] = Character_other[i];
                Character_other[i].GetComponent<CharacterStat>().char_num = NetworkManager.instance.char_index[i, 0];
                Character_other[i].GetComponent<CharacterStat>().My_Index = i;
                Character_other[i].GetComponent<CharacterStat>().ChangeSprite();
            }
            //케릭터 instantiate
            NetworkManager.instance.gameIsRunning = true;
            BossStatus.instance.player = (GameObject[])all_character_array.Clone();
            BossStatus.instance.target_player = all_character_array[0];
            Multi_Damage_toBoss = new float[all_character_array.Length,2];
            Multi_Damage_fromBoss = new float[all_character_array.Length,2];
            Multi_Heal = new float[all_character_array.Length,2];
            //BossStatus.instance.Aggro = new float[all_character_array.Length];
            //BossStatus.instance._distance = new float[all_character_array.Length];
            //BossStatus.instance.taunt = new float[all_character_array.Length];
            for (int i = 0; i < NetworkManager.instance.Player_num * 2; i++)
            {
                Multi_result_character_panel[i].SetActive(true);
            }
            big_damage = Multi_Damage_toBoss[0, 0];
            big_defence = Multi_Damage_fromBoss[0, 0];
            big_heal = Multi_Heal[0, 0];
            BossStatus.instance.StartCalAggro();
        }
        else
        {
            Character_other = new GameObject[4];    // 다른 더미의 스테이터스 바를 위해 존재
            BossStatus.instance.player = (GameObject[])player.Clone();
            BossStatus.instance.target_player = player[0];
            Damage_fromBoss = new float[2];
            Damage_toBoss = new float[2];
            Heal = new float[2];
            for (int i = 0; i < 2; i++)
            {
                Single_result_character_panel[i].SetActive(true);
            }
            big_damage = Damage_toBoss[0];
            big_defence = Damage_fromBoss[0];
            big_heal = Heal[0];
        }
        IsGameEnd = false;
        //player = GameObject.FindGameObjectsWithTag("Player");
        boss_index = UpLoadData.boss_index;
        character_index = UpLoadData.character_index;
        StartCoroutine(StopObjects());
        _animator = boss.GetComponent<Animator>();
        _animator.SetFloat("DirX", 0f);
        _animator.SetFloat("DirY", -1f);
        result_canvas.SetActive(false);


        drop_item = DatabaseManager.instance.boss_drop_item[UpLoadData.boss_index * 8 + UpLoadData.boss_level];
        display_item = grid.GetComponentsInChildren<InventorySlot>();
        Multi_display_item = Multi_grid.GetComponentsInChildren<InventorySlot>();
        _main = Camera.main;


        // 드록 확률
        drop_chance[0, 0] = 20f; drop_chance[0, 1] = 40f; drop_chance[0, 2] = 0;

        DeActiveSlot();
        InitializeDescription();
        My_Character_num = player[0].GetComponent<CharacterStat>().char_num;
        NetworkManager.instance.My_character = player[0];
    }

    IEnumerator StopObjects()
    {
        yield return new WaitForSeconds(6f);

        GameUI.instance.UI_ON();

        for (int i = 0; i < player.Length; i++)
        {
            player[i].GetComponent<Character_Control>().enabled = true;
            player[i].GetComponent<CharacterStat>().StopAllCoroutines();
            player[i].GetComponent<CharacterStat>().enabled = true;
        }

        yield return new WaitForSeconds(1f);

        switch (UpLoadData.boss_index)
        {
            case 0:
                boss.GetComponent<DesertBossMove_koki>().enabled = true;
                break;
            case 1:
                boss.GetComponent<Boss2Move>().enabled = true;
                break;
            case 2:
                boss.GetComponent<Boss3Move>().enabled = true;
                break;
            case 3:
                boss.GetComponent<Boss4Move>().enabled = true;
                break;
            case 4:
                boss.GetComponent<Boss5Move>().enabled = true;
                break;
            case 5:
                boss.GetComponent<Boss6Move>().enabled = true;
                break;
            case 6:
                boss.GetComponent<Boss7Move>().enabled = true;
                break;
            case 7:
                boss.GetComponent<Boss8Move>().enabled = true;
                break;
        }


        yield return 0;
    }

    public void CharacterWinGame()
    {
        IsGameEnd = true;
        GameUI.instance.UI_OFF();
        int temp = UpLoadData.boss_index * 8 + UpLoadData.boss_level;
        _main.GetComponent<Camera_move>().VivrateForTime(0.5f);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.gameIsRunning = false;
            UpLoadData.boss_is_cleared[temp + 4] = true;
            StartCoroutine(Multi_VictoryResult());
        }
        else
        {
            UpLoadData.boss_is_cleared[temp] = true;
            StartCoroutine(VictoryResult());
        }
    }

    public void BossWinGame()
    {
        IsGameEnd = true;
        GameUI.instance.UI_OFF();
        _main.GetComponent<Camera_move>().VivrateForTime(0.5f);
        StartCoroutine(DefeatResult());
    }

    public void Multi_MyCharacter_Die()
    {
        GameUI.instance.UI_OFF();
        _main.GetComponent<Camera_move>().VivrateForTime(0.5f);
        StartCoroutine(Multi_MyCharacter_Dead());
    }

    public void Multi_Boss_Win_Game()
    {
        IsGameEnd = true;
        GameUI.instance.UI_OFF();
        _main.GetComponent<Camera_move>().VivrateForTime(0.5f);
        NetworkManager.instance.gameIsRunning = false;
        StopAllCoroutines();
        StartCoroutine(Multi_Boss_Win_Game_Cor());
    }

    IEnumerator DefeatResult()
    {
        WaitForSecondsRealtime waittime = new WaitForSecondsRealtime(0.005f);
        WaitForSecondsRealtime wait_second = new WaitForSecondsRealtime(1f);


        result_canvas.SetActive(true);
        end_effect.SetActive(true);
        Time.timeScale = 0f;

        _main.GetComponent<Camera_move>().enabled = false;
        dir = (target_player.transform.position - _main.transform.position) / 3;
        _main.transform.localRotation = Quaternion.Euler(0, 0, -60f);
        int t = -1;
        for (int i = 0; i < 3; i++)
        {
            _main.transform.position += (Vector3)dir;
            _main.transform.localRotation = Quaternion.Euler(0, 0, _main.transform.localRotation.z * (t) - (20f * t));
            _main.orthographicSize -= i * 2;
            t *= -1;
            yield return wait_second;
        }
        _main.transform.localRotation = Quaternion.Euler(0, 0, 0f);

        Time.timeScale = 1f;

        while (_main.orthographicSize <= 8)
        {
            _main.orthographicSize += Time.deltaTime * 2;
            yield return waittime;
        }

        yield return new WaitForSeconds(0.5f);
        end_effect.SetActive(false);

        BGMManager.instance.FadeOutMusic();

        Vector3 temp = main_camera.transform.position;

        switch (UpLoadData.boss_index)
        {
            case 0:
                boss.GetComponent<DesertBossMove_koki>().enabled = false;
                boss.GetComponent<DesertBossMove_koki>().StopAllCoroutines();
                break;
            case 1:
                boss.GetComponent<Boss2Move>().enabled = false;
                boss.GetComponent<Boss2Move>().StopAllCoroutines();
                break;
            case 2:
                boss.GetComponent<Boss3Move>().enabled = false;
                boss.GetComponent<Boss3Move>().StopAllCoroutines();
                break;
            case 3:
                boss.GetComponent<Boss4Move>().enabled = false;
                boss.GetComponent<Boss4Move>().StopAllCoroutines();
                break;
            case 4:
                boss.GetComponent<Boss5Move>().enabled = false;
                boss.GetComponent<Boss5Move>().StopAllCoroutines();
                break;
            case 5:
                boss.GetComponent<Boss6Move>().enabled = false;
                boss.GetComponent<Boss6Move>().StopAllCoroutines();
                break;
            case 6:
                boss.GetComponent<Boss7Move>().enabled = false;
                boss.GetComponent<Boss7Move>().StopAllCoroutines();
                break;
            case 7:
                boss.GetComponent<Boss8Move>().enabled = false;
                boss.GetComponent<Boss8Move>().StopAllCoroutines();
                break;
        }
        for (int i = 0; i < player.Length; i++)
        {
            player[i].GetComponent<Character_Control>().StopAllCoroutines();
            player[i].GetComponent<Character_Control>().RunningStop();
            player[i].GetComponent<Character_Control>().enabled = false;
            player[i].GetComponent<CharacterStat>().StopAllCoroutines();
            player[i].GetComponent<CharacterStat>().enabled = false;
        }

        yield return new WaitForSeconds(1f);

        GameUI.instance.gameObject.SetActive(false);
        Destroy(boss);
        Single_result_panel.SetActive(true);
        Single_lose_title.SetActive(true);
        SoundManager.instance.Play(19);
        BGMManager.instance.FadeInMusic();

        temp.x += 3;
        temp.y += 6;
        lose_effect_temp = Instantiate(lose_effect, temp, Quaternion.Euler(0, 90, 0));
        Destroy(win_effect_temp, 2f);
        temp.x -= 6;
        lose_effect_temp = Instantiate(lose_effect, temp, Quaternion.Euler(0, -90, 0));
        Destroy(win_effect_temp, 2f);
        BossWinDropItem();
        StartCoroutine(HalfFadeOut());
        StartCoroutine(HalfFadeIn());
        for (int i = 0; i < 2; i++)
        {
            icon[i].sprite = character_images[UpLoadData.character_index[i]];
        }
        GetBig();
        for (int i = 0; i < 2; i++)
        {
            if ((float)Damage_toBoss[i] == 0)
                Single_Player_DamageToBoss_Text[i].text = "0";
            else
                Single_Player_DamageToBoss_Text[i].text = ((float)Damage_toBoss[i]).ToString();

            if ((float)Damage_fromBoss[i] == 0)
                Single_Player_DamageFromBoss_Text[i].text = "0";
            else
                Single_Player_DamageFromBoss_Text[i].text = ((float)Damage_fromBoss[i]).ToString();

            if ((float)Heal[i] == 0)
                Single_Player_Heal_Text[i].text = "0";
            else
                Single_Player_Heal_Text[i].text = ((float)Heal[i]).ToString();

            Single_player_scroll[i].sliders[0].value = (float)Damage_toBoss[i] / ((float)big_damage + 1);
            Debug.Log(big_damage);
            Single_player_scroll[i].sliders[1].value = (float)Damage_fromBoss[i] / ((float)big_defence + 1);
            Debug.Log(big_defence);
            Single_player_scroll[i].sliders[2].value = (float)Heal[i] / ((float)big_heal + 1);
            Debug.Log(big_heal);

        }
    }

    IEnumerator Multi_MyCharacter_Dead()
    {
        WaitForSecondsRealtime waittime = new WaitForSecondsRealtime(0.005f);
        WaitForSecondsRealtime wait_second = new WaitForSecondsRealtime(1f);

        
        end_effect.SetActive(true);
        Time.timeScale = 0f;

        _main.GetComponent<Camera_move>().enabled = false;
        dir = (BossStatus.instance.target_player.transform.position - _main.transform.position) / 3;
        _main.transform.localRotation = Quaternion.Euler(0, 0, -60f);
        int t = -1;
        for (int i = 0; i < 3; i++)
        {
            _main.transform.position += (Vector3)dir;
            _main.transform.localRotation = Quaternion.Euler(0, 0, _main.transform.localRotation.z * (t) - (20f * t));
            _main.orthographicSize -= i * 2;
            t *= -1;
            yield return wait_second;
        }
        _main.transform.localRotation = Quaternion.Euler(0, 0, 0f);

        Time.timeScale = 1f;

        while (_main.orthographicSize <= 8)
        {
            _main.orthographicSize += Time.deltaTime * 2;
            yield return waittime;
        }

        yield return new WaitForSeconds(0.5f);
        end_effect.SetActive(false);

        BGMManager.instance.FadeOutMusic();
        Camera.main.GetComponent<Camera_move>().my_character_dead = true;
    }

    IEnumerator Multi_Boss_Win_Game_Cor()
    {
        WaitForSecondsRealtime waittime = new WaitForSecondsRealtime(0.005f);
        WaitForSecondsRealtime wait_second = new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;

        yield return wait_second;

        result_canvas.SetActive(true);
        end_effect.SetActive(true);
        Multi_panel.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        end_effect.SetActive(false);

        BGMManager.instance.FadeOutMusic();

        Vector3 temp = main_camera.transform.position;

        switch (UpLoadData.boss_index)
        {
            case 0:
                boss.GetComponent<DesertBossMove_koki>().enabled = false;
                boss.GetComponent<DesertBossMove_koki>().StopAllCoroutines();
                break;
            case 1:
                boss.GetComponent<Boss2Move>().enabled = false;
                boss.GetComponent<Boss2Move>().StopAllCoroutines();
                break;
            case 2:
                boss.GetComponent<Boss3Move>().enabled = false;
                boss.GetComponent<Boss3Move>().StopAllCoroutines();
                break;
            case 3:
                boss.GetComponent<Boss4Move>().enabled = false;
                boss.GetComponent<Boss4Move>().StopAllCoroutines();
                break;
            case 4:
                boss.GetComponent<Boss5Move>().enabled = false;
                boss.GetComponent<Boss5Move>().StopAllCoroutines();
                break;
            case 5:
                boss.GetComponent<Boss6Move>().enabled = false;
                boss.GetComponent<Boss6Move>().StopAllCoroutines();
                break;
            case 6:
                boss.GetComponent<Boss7Move>().enabled = false;
                boss.GetComponent<Boss7Move>().StopAllCoroutines();
                break;
            case 7:
                boss.GetComponent<Boss8Move>().enabled = false;
                boss.GetComponent<Boss8Move>().StopAllCoroutines();
                break;
        }

        player[0].GetComponent<Character_Control>().StopAllCoroutines();
        player[0].GetComponent<Character_Control>().RunningStop();
        player[0].GetComponent<Character_Control>().enabled = false;
        player[0].GetComponent<CharacterStat>().StopAllCoroutines();
        player[0].GetComponent<CharacterStat>().enabled = false;


        yield return new WaitForSeconds(1f);

        GameUI.instance.gameObject.SetActive(false);
        Destroy(boss);
        Multi_result_panel.SetActive(true);
        Multi_Item_Button.SetActive(true);
        Multi_Statstics_Button.SetActive(true);
        Multi_lose_title.SetActive(true);
        SoundManager.instance.Play(19);
        BGMManager.instance.FadeInMusic();

        temp.x += 3;
        temp.y += 6;
        lose_effect_temp = Instantiate(lose_effect, temp, Quaternion.Euler(0, 90, 0));
        Destroy(win_effect_temp, 2f);
        temp.x -= 6;
        lose_effect_temp = Instantiate(lose_effect, temp, Quaternion.Euler(0, -90, 0));
        Destroy(win_effect_temp, 2f);
        BossWinDropItem();
        StartCoroutine(HalfFadeOut());
        StartCoroutine(HalfFadeIn());
        for (int i = 0; i < NetworkManager.instance.Player_num * 2; i++)
        {
            int t = i / 2;
            int j = i % 2;
            Multi_icon[i].sprite = character_images[NetworkManager.instance.char_index[t, j]];
        }
        Multi_GetBig();
        for (int i = 0; i < NetworkManager.instance.Player_num * 2; i++)
        {
            int t = i / 2;
            int j = i % 2;

            Multi_Player_DamageToBoss_Text[i].gameObject.SetActive(true);
            Multi_player_DamageFromBoss_Text[i].gameObject.SetActive(true);
            Multi_player_Heal_Text[i].gameObject.SetActive(true);

            if ((float)Multi_Damage_toBoss[t, j] == 0)
                Multi_Player_DamageToBoss_Text[i].text = "0";
            else
                Multi_Player_DamageToBoss_Text[i].text = ((float)Multi_Damage_toBoss[t, j]).ToString();

            if ((float)Multi_Damage_fromBoss[t, j] == 0)
                Multi_player_DamageFromBoss_Text[i].text = "0";
            else
                Multi_player_DamageFromBoss_Text[i].text = ((float)Multi_Damage_fromBoss[t, j]).ToString();

            if ((float)Multi_Heal[t, j] == 0)
                Multi_player_Heal_Text[i].text = "0";
            else
                Multi_player_Heal_Text[i].text = ((float)Multi_Heal[t, j]).ToString();

            Multi_player_scroll[i].sliders[0].value = (float)Multi_Damage_toBoss[t,j] / ((float)big_damage + 1);
            Multi_player_scroll[i].sliders[1].value = (float)Multi_Damage_fromBoss[t,j] / ((float)big_defence + 1);
            Multi_player_scroll[i].sliders[2].value = (float)Multi_Heal[t,j] / ((float)big_heal + 1);
        }
    }

    IEnumerator VictoryResult()
    {
        WaitForSecondsRealtime waittime = new WaitForSecondsRealtime(0.005f);
        WaitForSecondsRealtime wait_second = new WaitForSecondsRealtime(1f);


        result_canvas.SetActive(true);
        end_effect.SetActive(true);
        Time.timeScale = 0f;

        _main.GetComponent<Camera_move>().enabled = false;
        dir = (BossStatus.instance.gameObject.transform.position - _main.transform.position) / 3;

        for (int i = 0; i < 3; i++)
        {
            _main.transform.position += (Vector3)dir;
            _main.orthographicSize -= i * 2;
            yield return wait_second;
        }
        Time.timeScale = 1f;
        for (int i = 0; i < player.Length; i++)
        {
            player[i].tag = "Abnormal";
        }
        while (_main.orthographicSize <= 8)
        {
            _main.orthographicSize += Time.deltaTime * 2;
            yield return waittime;
        }
        yield return new WaitForSeconds(0.5f);
        end_effect.SetActive(false);

        BGMManager.instance.FadeOutMusic();

        Vector3 temp = main_camera.transform.position;
        switch (UpLoadData.boss_index)
        {
            case 0:
                boss.GetComponent<DesertBossMove_koki>().enabled = false;
                boss.GetComponent<DesertBossMove_koki>().StopAllCoroutines();
                break;
            case 1:
                boss.GetComponent<Boss2Move>().enabled = false;
                boss.GetComponent<Boss2Move>().StopAllCoroutines();
                break;
            case 2:
                boss.GetComponent<Boss3Move>().enabled = false;
                boss.GetComponent<Boss3Move>().StopAllCoroutines();
                break;
            case 3:
                boss.GetComponent<Boss4Move>().enabled = false;
                boss.GetComponent<Boss4Move>().StopAllCoroutines();
                break;
            case 4:
                boss.GetComponent<Boss5Move>().enabled = false;
                boss.GetComponent<Boss5Move>().StopAllCoroutines();
                break;
            case 5:
                boss.GetComponent<Boss6Move>().enabled = false;
                boss.GetComponent<Boss6Move>().StopAllCoroutines();
                break;
            case 6:
                boss.GetComponent<Boss7Move>().enabled = false;
                boss.GetComponent<Boss7Move>().StopAllCoroutines();
                break;
            case 7:
                boss.GetComponent<Boss8Move>().enabled = false;
                boss.GetComponent<Boss8Move>().StopAllCoroutines();
                break;
        }
        for (int i = 0; i < player.Length; i++)
        {
            player[i].GetComponent<Character_Control>().StopAllCoroutines();
            player[i].GetComponent<Character_Control>().RunningStop();
            player[i].GetComponent<Character_Control>().enabled = false;
            player[i].GetComponent<CharacterStat>().StopAllCoroutines();
            player[i].GetComponent<CharacterStat>().enabled = false;
        }
        yield return new WaitForSeconds(1f);

        GameUI.instance.gameObject.SetActive(false);
        Destroy(boss);
        fade_canvas.SetActive(true);
        fade_canvas.GetComponent<Canvas>().worldCamera = main_camera;
        fade_canvas.GetComponent<Canvas>().sortingLayerName = "Skill";
        Single_result_panel.SetActive(true);
        Single_win_title.SetActive(true);
        SoundManager.instance.Play(17);
        BGMManager.instance.FadeInMusic();
        temp.x += 3;
        temp.y += 6;
        win_effect_temp = Instantiate(win_effect, temp, Quaternion.identity);
        Destroy(win_effect_temp, 2f);
        temp.x -= 6;
        win_effect_temp = Instantiate(win_effect, temp, Quaternion.identity);
        Destroy(win_effect_temp, 2f);

        CharacterWinDropItem(UpLoadData.boss_level);
        Debug.Log(UpLoadData.boss_level);
        StartCoroutine(HalfFadeOut());
        StartCoroutine(HalfFadeIn());
        for (int i = 0; i < 2; i++)
        {
            icon[i].sprite = character_images[UpLoadData.character_index[i]];
        }
        GetBig();

        for (int i = 0; i < 2; i++)
        {
            if ((float)Damage_toBoss[i] == 0)
                Single_Player_DamageToBoss_Text[i].text = "0";
            else
                Single_Player_DamageToBoss_Text[i].text = ((float)Damage_toBoss[i]).ToString();

            if ((float)Damage_fromBoss[i] == 0)
                Single_Player_DamageFromBoss_Text[i].text = "0";
            else
                Single_Player_DamageFromBoss_Text[i].text = ((float)Damage_fromBoss[i]).ToString();

            if ((float)Heal[i] == 0)
                Single_Player_Heal_Text[i].text = "0";
            else
                Single_Player_Heal_Text[i].text = ((float)Heal[i]).ToString();            

            Single_player_scroll[i].sliders[0].value = (float)Damage_toBoss[i] / ((float)big_damage + 1);

            Single_player_scroll[i].sliders[1].value = (float)Damage_fromBoss[i] / ((float)big_defence + 1);

            Single_player_scroll[i].sliders[2].value = (float)Heal[i] / ((float)big_heal + 1);

        }
    }

    IEnumerator Multi_VictoryResult()
    {
        WaitForSecondsRealtime waittime = new WaitForSecondsRealtime(0.005f);
        WaitForSecondsRealtime wait_second = new WaitForSecondsRealtime(1f);


        result_canvas.SetActive(true);
        end_effect.SetActive(true);
        Multi_panel.SetActive(true);
        Time.timeScale = 0f;

        _main.GetComponent<Camera_move>().enabled = false;
        dir = (BossStatus.instance.gameObject.transform.position - _main.transform.position) / 3;

        for (int i = 0; i < 3; i++)
        {
            _main.transform.position += (Vector3)dir;
            _main.orthographicSize -= i * 2;
            yield return wait_second;
        }
        Time.timeScale = 1f;
        for (int i = 0; i < player.Length; i++)
        {
            player[i].tag = "Abnormal";
        }
        while (_main.orthographicSize <= 8)
        {
            _main.orthographicSize += Time.deltaTime * 2;
            yield return waittime;
        }
        yield return new WaitForSeconds(0.5f);
        end_effect.SetActive(false);

        BGMManager.instance.FadeOutMusic();

        Vector3 temp = main_camera.transform.position;
        switch (UpLoadData.boss_index)
        {
            case 0:
                boss.GetComponent<DesertBossMove_koki>().enabled = false;
                boss.GetComponent<DesertBossMove_koki>().StopAllCoroutines();
                break;
            case 1:
                boss.GetComponent<Boss2Move>().enabled = false;
                boss.GetComponent<Boss2Move>().StopAllCoroutines();
                break;
            case 2:
                boss.GetComponent<Boss3Move>().enabled = false;
                boss.GetComponent<Boss3Move>().StopAllCoroutines();
                break;
            case 3:
                boss.GetComponent<Boss4Move>().enabled = false;
                boss.GetComponent<Boss4Move>().StopAllCoroutines();
                break;
            case 4:
                boss.GetComponent<Boss5Move>().enabled = false;
                boss.GetComponent<Boss5Move>().StopAllCoroutines();
                break;
            case 5:
                boss.GetComponent<Boss6Move>().enabled = false;
                boss.GetComponent<Boss6Move>().StopAllCoroutines();
                break;
            case 6:
                boss.GetComponent<Boss7Move>().enabled = false;
                boss.GetComponent<Boss7Move>().StopAllCoroutines();
                break;
            case 7:
                boss.GetComponent<Boss8Move>().enabled = false;
                boss.GetComponent<Boss8Move>().StopAllCoroutines();
                break;
        }

        player[0].GetComponent<Character_Control>().StopAllCoroutines();
        player[0].GetComponent<Character_Control>().RunningStop();
        player[0].GetComponent<Character_Control>().enabled = false;
        player[0].GetComponent<CharacterStat>().StopAllCoroutines();
        player[0].GetComponent<CharacterStat>().enabled = false;

        yield return new WaitForSeconds(1f);

        GameUI.instance.gameObject.SetActive(false);
        Destroy(boss);
        fade_canvas.SetActive(true);
        fade_canvas.GetComponent<Canvas>().worldCamera = main_camera;
        fade_canvas.GetComponent<Canvas>().sortingLayerName = "Skill";
        Multi_result_panel.SetActive(true);
        Multi_win_title.SetActive(true);
        Multi_Item_Button.SetActive(true);
        Multi_Statstics_Button.SetActive(true);
        SoundManager.instance.Play(17);
        BGMManager.instance.FadeInMusic();
        temp.x += 3;
        temp.y += 6;
        win_effect_temp = Instantiate(win_effect, temp, Quaternion.identity);
        Destroy(win_effect_temp, 2f);
        temp.x -= 6;
        win_effect_temp = Instantiate(win_effect, temp, Quaternion.identity);
        Destroy(win_effect_temp, 2f);

        CharacterWinDropItem(UpLoadData.boss_level);
        Debug.Log(UpLoadData.boss_level);

        StartCoroutine(HalfFadeOut());
        StartCoroutine(HalfFadeIn());
        for (int i = 0; i < NetworkManager.instance.Player_num * 2; i++)
        {
            int t = i / 2;
            int j = i % 2;
            Multi_icon[i].sprite = character_images[NetworkManager.instance.char_index[t, j]];
        }
        Multi_GetBig();

        for (int i = 0; i < NetworkManager.instance.Player_num * 2; i++)
        {
            int t = i / 2;
            int j = i % 2;

            Multi_Player_DamageToBoss_Text[i].gameObject.SetActive(true);
            Multi_player_DamageFromBoss_Text[i].gameObject.SetActive(true);
            Multi_player_Heal_Text[i].gameObject.SetActive(true);

            if ((float)Multi_Damage_toBoss[t, j] == 0)
                Multi_Player_DamageToBoss_Text[i].text = "0";
            else
                Multi_Player_DamageToBoss_Text[i].text = ((float)Multi_Damage_toBoss[t, j]).ToString();

            if ((float)Multi_Damage_fromBoss[t, j] == 0)
                Multi_player_DamageFromBoss_Text[i].text = "0";
            else
                Multi_player_DamageFromBoss_Text[i].text = ((float)Multi_Damage_fromBoss[t, j]).ToString();

            if ((float)Multi_Heal[t, j] == 0)
                Multi_player_Heal_Text[i].text = "0";
            else
                Multi_player_Heal_Text[i].text = ((float)Multi_Heal[t, j]).ToString();

            Multi_player_scroll[i].sliders[0].value = (float)Multi_Damage_toBoss[t, j] / ((float)big_damage + 1);
            Multi_player_scroll[i].sliders[1].value = (float)Multi_Damage_fromBoss[t, j] / ((float)big_defence + 1);
            Multi_player_scroll[i].sliders[2].value = (float)Multi_Heal[t, j] / ((float)big_heal + 1);
        }
    }

    IEnumerator HalfFadeOut()
    {
        Color color = fade_InOut.GetComponent<Image>().color;
        while (color.a < 0.5f)
        {
            color.a += 3f * Time.deltaTime;
            fade_InOut.GetComponent<Image>().color = color;
            yield return new WaitForSeconds(0.01f);
        }
        yield return 0;
    }

    IEnumerator HalfFadeIn()
    {
        Color color = Single_result_panel.GetComponent<Image>().color;
        while (color.a < 0.5f)
        {
            color.a += 3f * Time.deltaTime;
            Single_result_panel.GetComponent<Image>().color = color;
            yield return new WaitForSeconds(0.01f);
        }
        //result_title_image.enabled = true;
        yield return 0;
    }

    private void GetBig()
    {
        for (int i = 0; i < 2; i++)
        {
            if (Damage_toBoss[i] > big_damage)
            {
                big_damage = Damage_toBoss[i];
            }

            if (Damage_fromBoss[i] > big_defence)
            {
                big_defence = Damage_fromBoss[i];
            }

            if (Heal[i] > big_heal)
            {
                big_heal = Heal[i];
            }
        }
    }

    private void Multi_GetBig()
    {
        for (int i = 0; i < NetworkManager.instance.Player_num; i++)
        {
            for(int j = 0; j < 2; j++)
            {
                if (Multi_Damage_toBoss[i,j] > big_damage)
                {
                    big_damage = Multi_Damage_toBoss[i,j];
                }

                if (Multi_Damage_fromBoss[i,j] > big_defence)
                {
                    big_defence = Multi_Damage_fromBoss[i, j];
                }

                if (Multi_Heal[i,j] > big_heal)
                {
                    big_heal = Multi_Heal[i,j];
                }
            }
        }
    }

    public void GoToTitle()
    {
        InitializeDescription();

        DatabaseManager.instance.SaveData();

        Destroy(GameUI.instance.gameObject);

        if(NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.GameOver();

            NetworkManager.instance.is_multi = false;
        }

        Destroy(GameManage.instance.gameObject);
        SceneManager.LoadScene("TItle");
    }

    private void CharacterWinDropItem(int index)
    {

        for (int i = 0; i < drop_item.Count; i++)
        {
            if (drop_item[i].itemType == Item.ItemType.Use && get_item_list.Count < 8)
            {
                float use = Random.Range(1.0f, 100.0f);
                if (use <= drop_item[i]._drop_chance)
                {
                    int count = Random.Range(1, 4);
                    Item temp = get_item_list.Find(item => item.itemID == drop_item[i].itemID);
                    if (temp != null)
                    {
                        temp.itemCount += count;
                        continue;
                    }
                    else
                    {
                        drop_item[i].itemCount = count;
                        get_item_list.Add(drop_item[i]);
                        continue;
                    }
                }
                else
                    continue;
            }
            else if (drop_item[i].itemType == Item.ItemType.ETC && get_item_list.Count < 8)
            {
                float use = Random.Range(1.0f, 100.0f);
                if (use <= drop_item[i]._drop_chance)
                {
                    int count = Random.Range(1, 4);
                    Item temp = get_item_list.Find(item => item.itemID == drop_item[i].itemID);
                    if (temp != null)
                    {
                        temp.itemCount += count;
                        continue;
                    }
                    else
                    {
                        drop_item[i].itemCount = count;
                        get_item_list.Add(drop_item[i]);
                        continue;
                    }
                }
                else
                    continue;
            }
            else if(drop_item[i].itemType == Item.ItemType.Equip && get_item_list.Count < 8)
            {
                float use = Random.Range(1.0f, 100.0f);
                if (use <= drop_item[i]._drop_chance)
                {                   
                    get_item_list.Add(drop_item[i]);
                    continue;                   
                }
                else
                    continue;
            }

        }

        if (NetworkManager.instance.is_multi)
        {
            for (int i = 0; i < get_item_list.Count; i++)
            {
                Multi_item_display[i].SetActive(true);
                Debug.Log(1);
                Multi_display_item[i].AddItem(get_item_list[i]);
            }

            if (get_item_list.Count <= 0)
            {
                Multi_item_description.text = "획득할 아이템이 없습니다.";
                Multi_GetItem_Button.SetActive(false);
                Debug.Log(2);

            }
            else
            {
                Multi_GetItem_Button.SetActive(true);
                Debug.Log(3);
            }
        }
        else
        {
            for (int i = 0; i < get_item_list.Count; i++)
            {
                item_display[i].SetActive(true);

                display_item[i].AddItem(get_item_list[i]);
            }

            if (get_item_list.Count <= 0)
            {
                item_description.text = "획득할 아이템이 없습니다.";
                getItem_button.SetActive(false);
            }
            else
            {
                getItem_button.SetActive(true);
            }
        }
    }

    private void BossWinDropItem()
    {
        for (int i = 0; i < drop_item.Count; i++)
        {
            float use = Random.Range(1.0f, 100.0f);
            if (drop_item[i].itemType == Item.ItemType.Use && get_item_list.Count < 8)
            {
                if (use <= drop_item[i]._drop_chance)
                {
                    int count = Random.Range(1, 4);
                    Item temp = get_item_list.Find(item => item.itemID == drop_item[i].itemID);
                    if (temp != null)
                    {
                        temp.itemCount += count;
                        continue;
                    }
                    else
                    {
                        drop_item[i].itemCount = count;
                        get_item_list.Add(drop_item[i]);
                        continue;
                    }
                }
                else
                    continue;
            }
        }


        if (NetworkManager.instance.is_multi)
        {
            for (int i = 0; i < get_item_list.Count; i++)
            {
                Multi_item_display[i].SetActive(true);

                Multi_display_item[i].AddItem(get_item_list[i]);
            }

            if (get_item_list.Count <= 0)
            {
                Multi_item_description.text = "획득할 아이템이 없습니다.";
                Multi_GetItem_Button.SetActive(false);
            }
            else
            {
                Multi_GetItem_Button.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < get_item_list.Count; i++)
            {
                item_display[i].SetActive(true);

                display_item[i].AddItem(get_item_list[i]);
            }

            if (get_item_list.Count <= 0)
            {
                item_description.text = "획득할 아이템이 없습니다.";
                getItem_button.SetActive(false);
            }
            else
            {
                getItem_button.SetActive(true);
            }
        }
    }


    private void DeActiveSlot()
    {
        int count = DatabaseManager.instance.max_item_count - DatabaseManager.instance.itemList.Count;
        if (count > 8)
        {
            count = 8;
        }
        if (NetworkManager.instance.is_multi)
        {
            for (int i = 0; i < count; i++)
            {
                Multi_item_display[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                item_display[i].SetActive(false);
            }
        }

    }


    // 수정 요망
    public void GetItem()
    {
        for (int i = 0; i < get_item_list.Count; i++)
        {
            if (DatabaseManager.instance.itemList.Count == DatabaseManager.instance.max_item_count)
            {
                break;
            }


            if (get_item_list[i].itemType == Item.ItemType.Use)
            {

                if (DatabaseManager.instance.itemList.Find(item => item.itemID == get_item_list[i].itemID) == null)
                {
                    UpLoadData.item_is_gained[get_item_list[i].itemID - 19901] = true;
                    Item temp = (Item)get_item_list[i].Clone();
                    DatabaseManager.instance.itemList.Add(temp);
                    UpLoadData.new_item_count++;
                }
                else
                {
                    DatabaseManager.instance.itemList.Find(item => item.itemID == get_item_list[i].itemID).itemCount += get_item_list[i].itemCount;
                }
            }
            else if (get_item_list[i].itemType == Item.ItemType.Equip)
            {
                UpLoadData.item_is_gained[get_item_list[i].itemID - 10001] = true;
                Item temp = (Item)get_item_list[i].Clone();
                DatabaseManager.instance.itemList.Add(get_item_list[i]);
                UpLoadData.new_item_count++;
            }
            else if (get_item_list[i].itemType == Item.ItemType.ETC)
            {
                if(DatabaseManager.instance.itemList.Find(item => item.itemID == get_item_list[i].itemID) == null)
                {
                    UpLoadData.item_is_gained[get_item_list[i].itemID - 29801] = true;
                    Item temp = (Item)get_item_list[i].Clone();
                    DatabaseManager.instance.itemList.Add(get_item_list[i]);
                    UpLoadData.new_item_count++;
                }
                else
                {
                    DatabaseManager.instance.itemList.Find(item => item.itemID == get_item_list[i].itemID).itemCount += get_item_list[i].itemCount;
                }
            }

        }
        DeActiveSlot();

        GetDescription();
    }


    public void DisplayItemOnDescription(int index)
    {
        if (NetworkManager.instance.is_multi)
        {
            Color a = Multi_item_icon.color;
            a.a = 1;
            Multi_item_icon.color = a;
            Multi_item_icon.sprite = get_item_list[index].itemIcon;
            Multi_item_name.text = get_item_list[index].itemName;
            Multi_item_description.text = get_item_list[index].itemDescription;
            Multi_item_count.text = "x" + get_item_list[index].itemCount.ToString();
        }
        else
        {
            Color a = item_icon.color;
            a.a = 1;
            item_icon.color = a;
            item_icon.sprite = get_item_list[index].itemIcon;
            item_name.text = get_item_list[index].itemName;
            item_description.text = get_item_list[index].itemDescription;
            item_count.text = "x" + get_item_list[index].itemCount.ToString();
        }
    }

    private void InitializeDescription()
    {
        if (NetworkManager.instance.is_multi)
        {
            Color a = Multi_item_icon.color;
            a.a = 0;
            Multi_item_icon.color = a;
            Multi_item_icon.sprite = null;
            Multi_item_name.text = "";
            Multi_item_description.text = "";
            Multi_item_count.text = "";
        }
        else
        {
            Color a = item_icon.color;
            a.a = 0;
            item_icon.color = a;
            item_icon.sprite = null;
            item_name.text = "";
            item_description.text = "";
            item_count.text = "";
        }
    }


    private void GetDescription()
    {
        if (NetworkManager.instance.is_multi)
        {
            Color a = item_icon.color;
            a.a = 0;
            Multi_item_icon.color = a;
            Multi_item_icon.sprite = null;
            Multi_item_name.text = "";
            if (DatabaseManager.instance.itemList.Count == DatabaseManager.instance.max_item_count)
            {
                Multi_item_description.text = "더 이상 아이템을 획득할 수 없습니다.";
            }
            else
            {
                Multi_item_description.text = "아이템을 모두\n획득하셨습니다.";
            }
            Multi_item_count.text = "";
        }
        else
        {
            Color a = item_icon.color;
            a.a = 0;
            item_icon.color = a;
            item_icon.sprite = null;
            item_name.text = "";
            if (DatabaseManager.instance.itemList.Count == DatabaseManager.instance.max_item_count)
            {
                item_description.text = "더 이상 아이템을 획득할 수 없습니다.";
            }
            else
            {
                item_description.text = "아이템을 모두\n획득하셨습니다.";
            }
            item_count.text = "";
        }
    }

    public void GetTargetPlayer(GameObject player)
    {
        target_player = player;
    }

    public void MultiResultButton()
    {
        Multi_result_panel.SetActive(true);
        Multi_statstics_panel.SetActive(false);
    }

    public void MultiStatsticsButton()
    {
        Multi_result_panel.SetActive(false);
        Multi_statstics_panel.SetActive(true);
        for (int i = 0; i < NetworkManager.instance.Player_num; i++)
        {
            Multi_Player_ID[i].text = NetworkManager.instance.players_name_string[i];
        }
    }
}


