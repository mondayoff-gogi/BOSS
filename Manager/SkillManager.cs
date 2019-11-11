using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    static public SkillManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }  //--------------인스턴스화를 위함 ----

    // 화면 밖을 나가지 않게 하기 위한 제한
    public float left_boundary = -11f;
    public float right_boundary = 11f;
    public float up_boundary = 7f;
    public float down_boundary = -7f;
    public int HammerDDD;
    public GameObject player1;
    //public GameObject player2;

    public GameObject[] NormalAttack;
    public GameObject[] NormalAttack_toMonster;

    public GameObject[] skill_prefab;
    //분류
    //0~9   0 : 독화살 기모으기      1 : 쏘는 이펙트   2: 화살   3 : 뒤로물어나기 발  4: 펑  5: 화살  6:강공 준비 7:강공이펙트 8.강공 화살   9.사일런트 준비
    //10~19  10 : 슬래시   11: 대쉬  12:한방준비 13:파워어택!    14: Cancel 공격!
    //20~29 20: 토네이도 기모으기   21: 토네이도    22: Vanish기모으기  23:Vanish  24 : 전기어디떨어뜨릴지   25:전기   26: 마나번쓸때   27:마나번
    //30~39 30: Hammerdown 준비   31: HammerDown! 32 : ReduceDamage 이펙트 33:쉴드이펙트  34 : 버프시작   35: 버프이펙트 36: DDD
    //40~49
    //50~59 : 0 : warning 1 : 캐스팅 이펙트  2: Mage_Skill1_Effect2 3: Mage_Skill1_Effect1 4: Mage_Skill2_Casting 5: Mage_Skill2_Effect 6: arrow 7: Mage_Skill3_Addition 8: Mage_Skill3_Effect_Sum
    //60~69 : 쉴드
    //70~79 : 2: 비었음
    //80~89  80 : 사일런트 화살
    //150~159 : 스피어 스킬
    public GameObject BlueCircleRange;
    public GameObject GreenCircleRange;
    public GameObject BlueStraightRange;
    public GameObject GreenStraightRange;
    public GameObject RedCircleWarning;
    public GameObject BlueFanWarning;
    private GameObject prefab_redcircle;
    private GameObject prefab_bluecircle;
    private GameObject prefab_greencircle;
    private GameObject prefab_bluestraight;
    private GameObject prefab_greenstraight;
    [HideInInspector]
    public Vector2 mouse_pos; //넘겨주는 용도의  mouse_pos;
    [HideInInspector]
    public float timer_temp; //넘겨주는 용도의 timer;
    public float[] skill_cooltime;
    public float[] skill_mana;
    public float[] buff_timer;
    public Camera UI_camera;

    private Character_Control _character1;
    //private Character_Control _character2;

    private Transform _char1_pos;
    //private Transform _char2_pos;

    private Vector3 vector_temp;
    private Vector3 scale_temp;

    private GameObject[] skill_prefab_temp;
    private GameObject gameobject_temp;
    public int player_1_num;
    //private int player_2_num;

    public bool[] skill_on_1;
    //public bool[] skill_on_2;

    // 스킬 범위를 저장하는 배열
    public float[] skill_range;

    public Vector2 temp;

    private Animator _animator;

    // 쉴드용
    private SpriteRenderer player_sprite;
    public Material material_temp;
    private GameObject[] player_transtorms;


    private GameObject casting_skill0=null;
    private GameObject casting_skill1=null;
    private GameObject casting_skill2=null;

    private bool[] Is_CastingZone;

    public GameObject[] SkillButton;

    private RaycastHit2D hitInfo;
    // Start is called before the first frame update
    void Start()
    {
        Is_CastingZone = new bool[4];
        Is_CastingZone[0] = false; Is_CastingZone[1] = false; Is_CastingZone[2] = false; Is_CastingZone[3] = false;
        _character1 = player1.GetComponent<Character_Control>();
        //_character2 = player2.GetComponent<Character_Control>();

        player_1_num = player1.GetComponent<CharacterStat>().char_num;
        //player_2_num = player2.GetComponent<CharacterStat>().char_num;
        skill_on_1 = new bool[4];
        //skill_on_2 = new bool[4];
        _char1_pos = _character1.transform;
        //_char2_pos = _character2.transform;
        skill_range = new float[32]; //스킬 범위 주기위한 배열, 배열 키우면 같이 키울것!
        skill_range[0] = 5; //druid 3시 스킬
        skill_range[24] = 3;    // 메이지 1번 스킬
        skill_range[29] = 1;    // 스피어 1번스킬
        skill_range[30] = 2;    // 스피어 2번스킬
        skill_prefab_temp = new GameObject[skill_prefab.Length];
        player_transtorms = GameObject.FindGameObjectsWithTag("Player");
        _animator = player1.GetComponent<Animator>();

    }  


    //스킬 범위 표시, 스킬 방향또는 위치 표시, 
    void Update()
    {
        if (skill_on_1[0]) //12시 방향 스킬 누르는경우
        {
            switch (player_1_num)
            {
                case 0: //ass 베기 방감 + 연계
                    if(!Is_CastingZone[0])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[0] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 6;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0)&& !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[12] = Instantiate(skill_prefab[12], _character1.transform);
                        casting_skill1 = skill_prefab_temp[12];
                        casting_skill0 = prefab_bluestraight;
                        prefab_bluestraight.transform.localScale = new Vector3(4,1,2);
                    }
                    else if(Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();

                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    if(prefab_bluestraight)
                    {
                        //누르고 돌리는동안 위치 바뀜
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if(mouse_pos.x<0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    if (Input.GetMouseButtonUp(0)&& prefab_bluestraight) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 0];
                        if(player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();

                        skill_prefab_temp[10] = Instantiate(skill_prefab[10], _character1.transform); //주변 데미지 이펙트
                        

                        vector_temp.x = _character1.GetComponent<Character_Control>()._animator.GetFloat("DirX");
                        vector_temp.y = _character1.GetComponent<Character_Control>()._animator.GetFloat("DirY");
                        vector_temp.z = 0;
                        vector_temp.Normalize();
                        TurnSkillDir(skill_prefab_temp[10], Vector3.zero, vector_temp);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(10, skill_prefab_temp[10].transform);
                        }
                        Destroy(prefab_bluestraight);
                        casting_skill0 = null;
                        Destroy(skill_prefab_temp[12]);
                        casting_skill1 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;                        
                    }
                    break;
                case 1: //Arrow
                    if (!Is_CastingZone[0])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[0] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 18;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[0] = Instantiate(skill_prefab[0], _character1.transform); //기모으는 이펙트
                        casting_skill0 = prefab_bluestraight;
                        casting_skill1 = skill_prefab_temp[0];
                        prefab_bluestraight.transform.localScale = new Vector3(18, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();
                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;

                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 0];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();

                        if(OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir+(Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        skill_prefab_temp[1] = Instantiate(skill_prefab[1], _character1.transform); //쏘는 이펙트
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(1, skill_prefab_temp[1].transform, 2);
                        }
                        Destroy(skill_prefab_temp[1], 2.0f); //
                        //if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                        //    skill_prefab_temp[1].transform.rotation = Quaternion.Euler(Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char1_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x)), -90, 0);
                        //else
                        //    skill_prefab_temp[1].transform.rotation = Quaternion.Euler(180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char1_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x)), -90, 0);
                        
                        skill_prefab_temp[1].transform.rotation = Quaternion.Euler(prefab_bluestraight.transform.rotation.x, -90, 0);
                        skill_prefab_temp[2] = Instantiate(skill_prefab[2], _character1.transform); //화살
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(2, skill_prefab_temp[2].transform);
                        }
                        Destroy(prefab_bluestraight);
                        Destroy(skill_prefab_temp[0]);
                        casting_skill0 = null;
                        casting_skill1=null; 

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    break;
                case 2: //Fan Tornado
                    if (!Is_CastingZone[0])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[0] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 14;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[22] = Instantiate(skill_prefab[22], _character1.transform); //기모으는 이펙트
                        casting_skill0 = prefab_bluestraight;
                        casting_skill1 = skill_prefab_temp[22];
                        prefab_bluestraight.transform.localScale = new Vector3(10, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();
                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 0];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        skill_prefab_temp[21] = Instantiate(skill_prefab[21], _character1.transform); //쏘는 이펙트
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(21, skill_prefab_temp[21].transform);
                        }
                        Destroy(prefab_bluestraight);
                        Destroy(skill_prefab_temp[22]);
                        casting_skill0 = null;
                        casting_skill1 = null;

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    break;
                case 3: //Hammer HammerDown!
                    if (!Is_CastingZone[0])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[0] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill2 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 7;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        prefab_bluecircle = Instantiate(BlueCircleRange, _character1.transform);
                        skill_prefab_temp[30] = Instantiate(skill_prefab[30], _character1.transform); //기모으는 이펙트
                        casting_skill0 = prefab_bluecircle;
                        casting_skill1 = skill_prefab_temp[30];
                        prefab_bluecircle.transform.localScale = Vector3.one * 3;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();
                        casting_skill2 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    if (prefab_bluecircle)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            vector_temp = mouse_pos.normalized * 1.4f;
                            vector_temp.z = 0;
                            prefab_bluecircle.transform.localPosition = vector_temp;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            vector_temp = (mouse_pos - (Vector2)_character1.transform.position).normalized * 1.4f;
                            vector_temp.z = 0;
                            prefab_bluecircle.transform.localPosition = vector_temp;
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                        //_character1.GetComponent<Character_Control>().transform.Translate(vector_temp * _character1.GetComponent<Character_Control>().move_speed * 0.5f * Time.deltaTime);
                    }
                    if (Input.GetMouseButtonUp(0) && prefab_bluecircle) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 0];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();

                        
                        skill_prefab_temp[31] = Instantiate(skill_prefab[31], _character1.transform); //쾅내려찍기
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;
                            if (mouse_pos.x > 0)
                                skill_prefab_temp[31].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if(mouse_pos.x<0)
                            {
                                skill_prefab_temp[31].GetComponent<ParticleSystemRenderer>().flip = new Vector3(0, 1, 0);
                                skill_prefab_temp[31].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            }
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                skill_prefab_temp[31].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char1_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x)));
                            else
                            {
                                skill_prefab_temp[31].GetComponent<ParticleSystemRenderer>().flip = new Vector3(0, 1, 0);
                                skill_prefab_temp[31].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char1_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x)));
                            }
                        }

                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(31, skill_prefab_temp[31].transform);
                        }

                        Destroy(prefab_redcircle);
                        Destroy(prefab_bluecircle);
                        Destroy(skill_prefab_temp[30]);
                        casting_skill0 = null;
                        casting_skill1 = null;
                        casting_skill2 = null;

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    break;
				case 4: // 힐러 힐링 오브 생성
                    if (!Is_CastingZone[0])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[0] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill2 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 10;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        prefab_bluecircle = Instantiate(BlueCircleRange, _character1.transform);
                        skill_prefab_temp[41] = Instantiate(skill_prefab[41], _character1.transform); //기모으는 이펙트
                        casting_skill0 = prefab_bluecircle;
                        casting_skill1 = skill_prefab_temp[41];
                        prefab_bluecircle.transform.localScale = Vector3.one * 3;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();
                        casting_skill2 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    if (prefab_bluecircle)
                    {
                        //누르고 돌리는동안 위치 바뀜
                        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);


                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;
                            
                            prefab_bluecircle.transform.position = (Vector2)_character1.transform.position+mouse_pos*60;
                        }
                        else
                        {
                            if (Vector2.Distance(mouse_pos, _character1.transform.position) > 5)
                                prefab_bluecircle.transform.localPosition = (mouse_pos - (Vector2)_character1.transform.position).normalized * 2;
                            else
                                prefab_bluecircle.transform.position = mouse_pos;
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluecircle) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 0];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();


                        StartCoroutine(HealerSkill1(player1, prefab_bluecircle.transform.position));

                        Destroy(prefab_redcircle);
                        Destroy(prefab_bluecircle);
                        Destroy(skill_prefab_temp[41]);
                        casting_skill0 = null;
                        casting_skill1 = null;
                        casting_skill2 = null;

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    break;
                case 5: // 법사 장판
                    if (!Is_CastingZone[0])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[0] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill2 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 15;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        prefab_bluecircle = Instantiate(BlueCircleRange, _character1.transform);
                        skill_prefab_temp[51] = Instantiate(skill_prefab[51], _character1.transform); //기모으는 이펙트
                        casting_skill0 = prefab_bluecircle;
                        casting_skill1 = skill_prefab_temp[51];
                        prefab_bluecircle.transform.localScale = Vector3.one * 3;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();
                        casting_skill2 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    if (prefab_bluecircle)
                    {
                        //누르고 돌리는동안 위치 바뀜
                        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);


                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            prefab_bluecircle.transform.position = (Vector2)_character1.transform.position + mouse_pos * 90;
                        }
                        else
                        {
                            if (Vector2.Distance(mouse_pos, _character1.transform.position) > 7.5)
                                prefab_bluecircle.transform.localPosition = (mouse_pos - (Vector2)_character1.transform.position).normalized * 3;
                            else
                                prefab_bluecircle.transform.position = mouse_pos;
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluecircle) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 0];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();


                        mouse_pos = prefab_bluecircle.transform.position;
                        StartCoroutine(MageSkill1(player1, mouse_pos, vector_temp, 5f));

                        Destroy(prefab_redcircle);
                        Destroy(prefab_bluecircle);
                        Destroy(skill_prefab_temp[51]);
                        casting_skill0 = null;
                        casting_skill1 = null;
                        casting_skill2 = null;

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    break;
                case 6:     // 쉴드 방패치기
                    if (!Is_CastingZone[0])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[0] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 6;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[62] = Instantiate(skill_prefab[62], _character1.transform); //기모으는 이펙트
                        casting_skill0 = prefab_bluestraight;
                        casting_skill1 = skill_prefab_temp[62];
                        prefab_bluestraight.transform.localScale = new Vector3(4, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();
                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }

                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 0];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();


                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        //mouse_pos = (mouse_pos - (Vector2)_character1.transform.position).normalized * 3;
                        //mouse_pos = (Vector2)_character1.transform.position + mouse_pos;
                        StartCoroutine(ShieldSkill1(_character1.gameObject, mouse_pos));

                        Destroy(prefab_bluestraight);
                        Destroy(skill_prefab_temp[62]);
                        casting_skill0 = null;
                        casting_skill1 = null;

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    break;
                case 7: // 스피어
                    if (!Is_CastingZone[0])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[0] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 14;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[71] = Instantiate(skill_prefab[71], _character1.transform); //기모으는 이펙트
                        casting_skill0 = prefab_bluestraight;
                        casting_skill1 = skill_prefab_temp[71];
                        prefab_bluestraight.transform.localScale = new Vector3(6, 0.5f, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();
                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 0];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[0].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[0].GetComponent<ButtonScript>().SkillOff();


                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }

                        vector_temp = mouse_pos - (Vector2)_char1_pos.position;
                        vector_temp.z = 0;
                        StartCoroutine(SpearSkill1(_char1_pos.position, vector_temp, _char1_pos));

                        Destroy(prefab_bluestraight);
                        Destroy(skill_prefab_temp[71]);
                        casting_skill0 = null;
                        casting_skill1 = null;

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[0] = false;
                        skill_on_1[0] = false;
                    }
                    break;
            }

        }
        if (skill_on_1[1]) //3시 스킬
        {
            switch (player_1_num)
            {
                case 0: //Ass Dash
                    if (!Is_CastingZone[1])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[1] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;

                        prefab_redcircle.transform.localScale = Vector3.one * 12;
                        _character1.GetComponent<Character_Control>().Runable = false;

                    }
                    else if(Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[12] = Instantiate(skill_prefab[12], _character1.transform);
                        casting_skill1 = skill_prefab_temp[12];
                        casting_skill0 = prefab_bluestraight;
                        prefab_bluestraight.transform.localScale = new Vector3(6, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        skill_on_1[1] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 1];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)  
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();


                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }

                        skill_prefab_temp[4] = Instantiate(skill_prefab[4], _character1.transform); //펑
                        skill_prefab_temp[4].transform.SetParent(skill_prefab_temp[4].transform.parent.parent);
                        Destroy(skill_prefab_temp[4], 1.0f);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(4, _character1.transform,1,false,true);
                        }
                        skill_prefab_temp[11] = Instantiate(skill_prefab[11], _character1.transform); //대쉬
                        TurnSkillDir(skill_prefab_temp[11], _character1.transform.position, mouse_pos); //돌리고

                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(11, skill_prefab_temp[11].transform,1);
                        }
                        Destroy(prefab_bluestraight);
                        casting_skill0 = null;
                        Destroy(skill_prefab_temp[12]);
                        casting_skill1 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        skill_on_1[1] = false;
                    }
                    break;
                case 1: //Arrow 후퇴사격
                    if (!Is_CastingZone[1])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[1] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;

                        prefab_redcircle.transform.localScale = Vector3.one * 12;
                        _character1.GetComponent<Character_Control>().Runable = false;

                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[0] = Instantiate(skill_prefab[0], _character1.transform); //기모으는 이펙트
                        casting_skill1 = skill_prefab_temp[0];
                        casting_skill0 = prefab_bluestraight;
                        prefab_bluestraight.transform.localScale = new Vector3(4, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();   
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        skill_on_1[1] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 1];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        skill_prefab_temp[3] = Instantiate(skill_prefab[3], _character1.transform); //뒤로 물러나는 발자국 이펙트
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(3, skill_prefab_temp[3].transform,100,true);
                        }
                        skill_prefab_temp[4] = Instantiate(skill_prefab[4], _character1.transform); //뒤로 물러나는 펑 이펙트
                        skill_prefab_temp[4].transform.SetParent(skill_prefab_temp[4].transform.parent.parent);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(4, skill_prefab_temp[4].transform,2,false,true);
                        }
                        skill_prefab_temp[5] = Instantiate(skill_prefab[5], _character1.transform); //화살 날리는 이펙트
                        
                        vector_temp.x = _character1.GetComponent<Character_Control>()._animator.GetFloat("DirX");
                        vector_temp.y = _character1.GetComponent<Character_Control>()._animator.GetFloat("DirY");
                        vector_temp.z = 0;
                        vector_temp.Normalize();
                        TurnSkillDir(skill_prefab_temp[5], _character1.transform.position, mouse_pos);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(5, skill_prefab_temp[5].transform, 2);
                        }
                        Destroy(prefab_bluestraight);
                        Destroy(skill_prefab_temp[0]);
                        casting_skill1 = null;
                        Destroy(skill_prefab_temp[4], 2f);
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        casting_skill0 = null;
                        skill_on_1[1] = false;
                    }
                    break;
                case 2: //Fan Vanish   koki
                    if (!Is_CastingZone[1])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[1] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill2 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 15;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        prefab_bluecircle = Instantiate(BlueCircleRange, _character1.transform);
                        skill_prefab_temp[22] = Instantiate(skill_prefab[22], _character1.transform); //기모으는 이펙트
                        casting_skill1 = skill_prefab_temp[22];
                        casting_skill0 = prefab_bluecircle;
                        prefab_bluecircle.transform.localScale = Vector3.one * 4f;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        skill_on_1[1] = false;
                    }
                    if (prefab_bluecircle)
                    {
                        //누르고 돌리는동안 위치 바뀜
                        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);


                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            prefab_bluecircle.transform.position = (Vector2)_character1.transform.position + mouse_pos * 90;
                        }
                        else
                        {
                            if (Vector2.Distance(mouse_pos, _character1.transform.position) > 7.5f)
                                prefab_bluecircle.transform.localPosition = (mouse_pos - (Vector2)_character1.transform.position).normalized * 3f;
                            else
                                prefab_bluecircle.transform.position = mouse_pos;
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;

                    }
                    if (Input.GetMouseButtonUp(0) && prefab_bluecircle) //떼면 이펙트
                    {
                        _character1.GetComponent<Character_Control>()._animator.SetBool("IsMagic", true);
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 1];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();
                       
                        skill_prefab_temp[15] = Instantiate(skill_prefab[15], prefab_bluecircle.transform.position, Quaternion.identity); //뒤로 물러나는 펑 이펙트
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(15, skill_prefab_temp[15].transform,1f,false,true);
                        }
                        vector_temp.x = _character1.GetComponent<Character_Control>()._animator.GetFloat("DirX");
                        vector_temp.y = _character1.GetComponent<Character_Control>()._animator.GetFloat("DirY");
                        vector_temp.z = 0;
                        vector_temp.Normalize();
                        Destroy(prefab_redcircle);
                        Destroy(prefab_bluecircle);
                        Destroy(skill_prefab_temp[22]);
                        casting_skill1 = null;
                        Destroy(skill_prefab_temp[15], 1f);
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);

                        skill_on_1[1] = false;
                    }
                    break;
                case 3:
                    if (!Is_CastingZone[1])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[1] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;

                        prefab_redcircle.transform.localScale = Vector3.one * 3;
                        _character1.GetComponent<Character_Control>().Runable = false;

                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);

                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 1];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();

                        skill_prefab_temp[32] = Instantiate(skill_prefab[32], _character1.transform); //쓸때 이펙트
                        Destroy(skill_prefab_temp[32], 2f);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(32, skill_prefab_temp[32].transform,2f);
                        }
                        skill_prefab_temp[33] = Instantiate(skill_prefab[33], _character1.transform);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(33, skill_prefab_temp[33].transform,100,true);
                        }
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        casting_skill0 = null;
                        skill_on_1[1] = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        skill_on_1[1] = false;
                    }
                    break;
                case 4: // 힐러 범위 힐
                    if (!Is_CastingZone[1])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[1] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 18;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[41] = Instantiate(skill_prefab[41], _character1.transform); //기모으는 이펙트
                        casting_skill1 = skill_prefab_temp[41];
                        casting_skill0 = prefab_bluestraight;
                        prefab_bluestraight.transform.localScale = new Vector3(10, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        skill_on_1[1] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 1];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        StartCoroutine(HealerSkill2(player1, mouse_pos));

                        Destroy(prefab_bluestraight);
                        Destroy(skill_prefab_temp[41]);
                        casting_skill1 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        casting_skill0 = null;
                        skill_on_1[1] = false;
                    }
                    break;
                case 5: // 법사 화염 방어막 //koki
                    if (!Is_CastingZone[1])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[1] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 10;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        prefab_bluecircle = Instantiate(BlueCircleRange, _character1.transform);
                        skill_prefab_temp[51] = Instantiate(skill_prefab[51], _character1.transform);

                        casting_skill1 = skill_prefab_temp[51];
                        casting_skill0 = prefab_bluecircle;
                        prefab_bluecircle.transform.localScale = Vector3.one * 3;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        skill_on_1[1] = false;
                    }
                    if (prefab_bluecircle)
                    {
                        //누르고 돌리는동안 위치 바뀜
                        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);


                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            prefab_bluecircle.transform.position = (Vector2)_character1.transform.position + mouse_pos * 60;
                        }
                        else
                        {
                            if (Vector2.Distance(mouse_pos, _character1.transform.position) > 5f)
                                prefab_bluecircle.transform.localPosition = (mouse_pos - (Vector2)_character1.transform.position).normalized * 2f;
                            else
                                prefab_bluecircle.transform.position = mouse_pos;
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    if (Input.GetMouseButtonUp(0) && prefab_bluecircle) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 1];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();
                        
                        skill_prefab_temp[55] = Instantiate(skill_prefab[55], prefab_bluecircle.transform.position, Quaternion.identity);
                        skill_prefab_temp[55].GetComponent<Mage_Skill2_New>().GetPlayer(_character1.gameObject);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(55, skill_prefab_temp[55].transform,100,false,true);
                        }
                        // 여기에 범위로 캐릭터 화염 보호막 생성하는 코드 들어가면 됨

                        Destroy(prefab_bluecircle);
                        Destroy(prefab_redcircle);
                        Destroy(skill_prefab_temp[51]);
                        casting_skill1 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        casting_skill0 = null;
                        skill_on_1[1] = false;
                    }
                    break;
                case 6:
                    if (!Is_CastingZone[1])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[1] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;

                        prefab_redcircle.transform.localScale = Vector3.one * 3;
                        _character1.GetComponent<Character_Control>().Runable = false;

                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);

                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 1];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();

                        vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char1_pos.position;
                        skill_prefab_temp[67] = Instantiate(skill_prefab[67], _char1_pos);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(67, skill_prefab_temp[67].transform, 100,true);
                        }
                        StartCoroutine(ShieldSkill2(player1, vector_temp));

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        casting_skill0 = null;
                        skill_on_1[1] = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        skill_on_1[1] = false;
                    }
                    break;
                case 7:     // 스피어 대쉬후 찌르기
                    if (!Is_CastingZone[1])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[1] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 18;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[71] = Instantiate(skill_prefab[71], _character1.transform); //기모으는 이펙트
                        casting_skill1 = skill_prefab_temp[71];
                        casting_skill0 = prefab_bluestraight;
                        prefab_bluestraight.transform.localScale = new Vector3(6, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        skill_on_1[1] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 1];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[1].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[1].GetComponent<ButtonScript>().SkillOff();



                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        skill_prefab_temp[152] = Instantiate(skill_prefab[152], _character1.transform);
                        Destroy(skill_prefab_temp[152], 0.5f);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(152, skill_prefab_temp[152].transform,0.5f);
                        }
                        StartCoroutine(SpearSkill2(vector_temp, mouse_pos, _char1_pos));



                        Destroy(prefab_bluestraight);
                        Destroy(skill_prefab_temp[71]);
                        casting_skill1 = null;
                        //_character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[1] = false;
                        casting_skill0 = null;
                        skill_on_1[1] = false;
                    }
                    break;
            }

        }
        if (skill_on_1[2]) //6시 스킬
        {
            switch (player_1_num)
            {
                case 0: //ass power
                    if (!Is_CastingZone[2])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[2] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 4;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[12] = Instantiate(skill_prefab[12], _character1.transform);
                        casting_skill1 = skill_prefab_temp[12];
                        casting_skill0 = prefab_bluestraight;
                        prefab_bluestraight.transform.localScale = new Vector3(3, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        Destroy(prefab_bluestraight);
                        casting_skill0 = null;
                        Destroy(skill_prefab_temp[12]);
                        casting_skill1 = null;
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 2];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)  
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[1] = 0;

                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        skill_prefab_temp[13] = Instantiate(skill_prefab[13], _character1.transform); //쏘는 이펙트
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(13, skill_prefab_temp[13].transform);
                        }
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    break;
                case 1: //Arrow 강공격 
                    if (!Is_CastingZone[2])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[2] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 18;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[0] = Instantiate(skill_prefab[0], _character1.transform); //기모으는 이펙트
                        casting_skill1 = skill_prefab_temp[0];
                        casting_skill0 = prefab_bluestraight;
                        prefab_bluestraight.transform.localScale = new Vector3(18, 1, 2);
                        vector_temp = prefab_bluestraight.transform.localScale;
                        timer_temp = 0;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (timer_temp < 5)
                            timer_temp += Time.deltaTime;
                        if (skill_prefab_temp[0].transform.localScale.x < 0.65)
                            skill_prefab_temp[0].transform.localScale *= 1 + (Time.deltaTime / 5);
                        if (timer_temp < 5)
                            vector_temp.y += Time.deltaTime * 0.2f;
                        prefab_bluestraight.transform.localScale = vector_temp;

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        Destroy(prefab_bluestraight);
                        Destroy(skill_prefab_temp[0]);
                        casting_skill1 = null;
                        casting_skill0 = null;

                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 2];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[1] = 0;

                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        skill_prefab_temp[7] = Instantiate(skill_prefab[7], _character1.transform); //팡!
                        skill_prefab_temp[7].transform.localScale *= 1 + (timer_temp / 5);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(7, skill_prefab_temp[7].transform, 1.5f);
                        }
                        Destroy(skill_prefab_temp[7], 1.5f);

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        skill_prefab_temp[8] = Instantiate(skill_prefab[8], _character1.transform); //화려한화살
                        skill_prefab_temp[8].transform.localScale *= (1 + timer_temp / 5);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(8, skill_prefab_temp[8].transform);
                        }
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    break;
                case 2:
                    if (!Is_CastingZone[2])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[2] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill2 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 18;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        prefab_bluecircle = Instantiate(BlueCircleRange, _character1.transform);
                        skill_prefab_temp[22] = Instantiate(skill_prefab[22], _character1.transform);
                        casting_skill1 = skill_prefab_temp[22];
                        casting_skill0 = prefab_bluecircle;
                        prefab_bluecircle.transform.localScale = Vector3.one * 3;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        casting_skill2 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    if (prefab_bluecircle)
                    {
                        //누르고 돌리는동안 위치 바뀜
                        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);


                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            prefab_bluecircle.transform.position = (Vector2)_character1.transform.position + mouse_pos * 108;
                        }
                        else
                        {
                            if (Vector2.Distance(mouse_pos, _character1.transform.position) > 9f)
                                prefab_bluecircle.transform.localPosition = (mouse_pos - (Vector2)_character1.transform.position).normalized * 3.6f;
                            else
                                prefab_bluecircle.transform.position = mouse_pos;
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    if (Input.GetMouseButtonUp(0) && prefab_bluecircle) //떼면 이펙트
                    {
                        
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 2];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[1] = 0;

                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        // 시간차로 만들기
                        skill_prefab_temp[25] = Instantiate(skill_prefab[25], _character1.transform); //팡!
                        skill_prefab_temp[25].transform.position = prefab_bluecircle.transform.position;
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(25, skill_prefab_temp[25].transform);
                        }
                        Destroy(prefab_redcircle);
                        Destroy(prefab_bluecircle);
                        Destroy(skill_prefab_temp[22]);
                        casting_skill0 = null;
                        casting_skill1 = null;
                        casting_skill2 = null;

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    break;
                case 3: //Hammer Buff 
                    if (!Is_CastingZone[2])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[2] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 3;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);

                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 2];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        skill_prefab_temp[34] = Instantiate(skill_prefab[34], _character1.transform); //준비
                        Destroy(skill_prefab_temp[34], 2f);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(34, skill_prefab_temp[34].transform, 2f);
                        }                        
                        //버프걸어주고
                        skill_prefab_temp[35] = Instantiate(skill_prefab[35], _character1.transform);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerAllBuff(35);
                        }
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        casting_skill0 = null;
                        skill_on_1[2] = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    break;
                case 4: // 힐러 범위 공격
                    if (!Is_CastingZone[2])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[2] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 18;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[41] = Instantiate(skill_prefab[41], _character1.transform); //기모으는 이펙트
                        casting_skill1 = skill_prefab_temp[41];
                        casting_skill0 = prefab_bluestraight;
                        prefab_bluestraight.transform.localScale = new Vector3(10, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 2];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        StartCoroutine(HealerSkill3(player1, mouse_pos));

                        Destroy(prefab_bluestraight);
                        Destroy(skill_prefab_temp[41]);
                        casting_skill1 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        casting_skill0 = null;
                        skill_on_1[2] = false;
                    }
                    break;
                case 5: // 법사 범위 공격
                    if (!Is_CastingZone[2])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[2] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 16;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueFanWarning, _character1.transform);
                        skill_prefab_temp[51] = Instantiate(skill_prefab[51], _character1.transform);
                        casting_skill1 = skill_prefab_temp[51];
                        casting_skill0 = prefab_bluestraight;
                        prefab_bluestraight.transform.localScale = Vector3.one*5f;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        Destroy(prefab_bluestraight);
                        casting_skill0 = null;
                        Destroy(skill_prefab_temp[51]);
                        casting_skill1 = null;
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 2];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[1] = 0;

                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        temp = _char1_pos.position;
                        StartCoroutine(MageSkill3(player1, mouse_pos, vector_temp));

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    break;
                case 6: // 쉴드 도발
                    if (!Is_CastingZone[2])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[2] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 3;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);

                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 2];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        StartCoroutine(ShieldSkill3(player1, vector_temp));

                        Is_CastingZone[2] = false;
                        casting_skill0 = null;
                        skill_on_1[2] = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    break;
                case 7:     // 공속버프, 피흡
                    if (!Is_CastingZone[2])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[2] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 3;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);

                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 2];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[2].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        StartCoroutine(SpearSkill3(player1));

                        Is_CastingZone[2] = false;
                        casting_skill0 = null;
                        skill_on_1[2] = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[2].GetComponent<ButtonScript>().SkillOff();

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[2] = false;
                        skill_on_1[2] = false;
                    }
                    break;
            }

        }
        if (skill_on_1[3]) //9시 스킬
        {
            switch (player_1_num)
            {
                case 0: //ass cancel
                    if (!Is_CastingZone[3])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[3] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 4;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[12] = Instantiate(skill_prefab[12], _character1.transform);
                        casting_skill1 = skill_prefab_temp[12];
                        casting_skill0 = prefab_bluestraight;
                        prefab_bluestraight.transform.localScale = new Vector3(3, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        Destroy(prefab_bluestraight);
                        casting_skill0 = null;
                        Destroy(skill_prefab_temp[12]);
                        casting_skill1 = null;
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 3];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)  
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        skill_prefab_temp[14] = Instantiate(skill_prefab[14], _character1.transform); //누르자마자
                        vector_temp.x = _character1.GetComponent<Character_Control>()._animator.GetFloat("DirX");
                        vector_temp.y = _character1.GetComponent<Character_Control>()._animator.GetFloat("DirY");
                        vector_temp.z = 0;
                        vector_temp.Normalize();
                        TurnSkillDir(skill_prefab_temp[14], Vector2.zero, vector_temp);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(14, skill_prefab_temp[14].transform);
                        }

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    break;
                case 1: //Arrow 사일런트
                    if (!Is_CastingZone[3])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[3] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 18;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[0] = Instantiate(skill_prefab[0], _character1.transform); //기모으는 이펙트
                        casting_skill0 = prefab_bluestraight;
                        casting_skill1 = skill_prefab_temp[0];
                        prefab_bluestraight.transform.localScale = new Vector3(18, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        Destroy(prefab_bluestraight);
                        casting_skill0 = null;
                        Destroy(skill_prefab_temp[0]);
                        casting_skill1 = null;
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 3];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        skill_prefab_temp[80] = Instantiate(skill_prefab[80], _character1.transform); //화살
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(80, skill_prefab_temp[80].transform);
                        }
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    break;
                case 2:
                    if (!Is_CastingZone[3])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[3] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 16;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);
                        prefab_bluestraight = Instantiate(BlueStraightRange, _character1.transform);
                        skill_prefab_temp[22] = Instantiate(skill_prefab[22], _character1.transform);
                        casting_skill1 = skill_prefab_temp[12];
                        casting_skill0 = prefab_bluestraight;
                        prefab_bluestraight.transform.localScale = new Vector3(10, 1, 2);
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    if (prefab_bluestraight)
                    {
                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y);

                            if (mouse_pos.x > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                            else if (mouse_pos.x < 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y) / (mouse_pos.x)));
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                            _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                            if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char1_pos.position.x) > 0)
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                            else
                                prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((mouse_pos.y - _char1_pos.position.y) / (mouse_pos.x - _char1_pos.position.x)));
                        }

                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    if (Input.GetMouseButtonUp(0) && prefab_bluestraight) //떼면 이펙트
                    {
                        Destroy(prefab_bluestraight);
                        casting_skill0 = null;
                        Destroy(skill_prefab_temp[22]);
                        casting_skill1 = null;
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 3];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir + (Vector2)player1.transform.position;
                        }
                        else
                        {
                            mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        skill_prefab_temp[26] = Instantiate(skill_prefab[26], _character1.transform); //팡!
                        Destroy(skill_prefab_temp[26], 1f);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(26, skill_prefab_temp[26].transform,1f);
                        }
                        skill_prefab_temp[27] = Instantiate(skill_prefab[27], _character1.transform); //마나번구체 생성
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(27, skill_prefab_temp[27].transform);
                        }

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    break;
                case 3: //Hammer DDD
                    if (!Is_CastingZone[3])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[3] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill2 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 7;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        HammerDDD = 0;
                        prefab_bluecircle = Instantiate(BlueCircleRange, _character1.transform);
                        skill_prefab_temp[30] = Instantiate(skill_prefab[30], _character1.transform); //기모으는 이펙트
                        casting_skill0 = prefab_bluecircle;
                        casting_skill1 = skill_prefab_temp[30];
                        prefab_bluecircle.transform.localScale = Vector3.one * 3;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();
                        casting_skill2 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    if (prefab_bluecircle)
                    {
                        //누르고 돌리는동안 위치 바뀜
                        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;
                            vector_temp = (mouse_pos).normalized * 1.4f;
                            vector_temp.z = 0;
                            prefab_bluecircle.transform.localPosition = vector_temp;
                        }
                        else
                        {
                            vector_temp = (mouse_pos - (Vector2)_character1.transform.position).normalized * 1.4f;
                            vector_temp.z = 0;
                            prefab_bluecircle.transform.localPosition = vector_temp;
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;

                        //_character1.GetComponent<Character_Control>().transform.Translate(vector_temp * _character1.GetComponent<Character_Control>().move_speed * 0.5f * Time.deltaTime);
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluecircle) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 3];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();


                        StartCoroutine(DDD(_character1.transform));

                        Destroy(prefab_redcircle);
                        Destroy(prefab_bluecircle);
                        Destroy(skill_prefab_temp[30]);
                        casting_skill0 = null;
                        casting_skill1 = null;
                        casting_skill2 = null;

                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    break;                
                case 4: // 마나 오브
                    if (!Is_CastingZone[3])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[3] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill2 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 10;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        prefab_bluecircle = Instantiate(BlueCircleRange, _character1.transform);
                        prefab_bluecircle.transform.position = new Vector3(prefab_bluecircle.transform.position.x, prefab_bluecircle.transform.position.y, 0);
                        skill_prefab_temp[41] = Instantiate(skill_prefab[41], _character1.transform); //기모으는 이펙트
                        casting_skill0 = prefab_bluecircle;
                        casting_skill1 = skill_prefab_temp[41];
                        prefab_bluecircle.transform.localScale = Vector3.one * 3;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();
                        casting_skill2 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    if (prefab_bluecircle)
                    {
                        //누르고 돌리는동안 위치 바뀜
                        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);

                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            prefab_bluecircle.transform.position = (Vector2)_character1.transform.position + mouse_pos * 60;
                        }
                        else
                        {
                            if (Vector2.Distance(mouse_pos, _character1.transform.position) > 5f)
                                prefab_bluecircle.transform.localPosition = (mouse_pos - (Vector2)_character1.transform.position).normalized * 2f;
                            else
                                prefab_bluecircle.transform.position = mouse_pos;
                        }

                        _character1.GetComponent<Character_Control>().Runable = false;
                    }

                    if (Input.GetMouseButtonUp(0) && prefab_bluecircle) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 3];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();


                        StartCoroutine(HealerSkill4(player1, prefab_bluecircle.transform.position));

                        Destroy(prefab_redcircle);
                        Destroy(prefab_bluecircle);
                        Destroy(skill_prefab_temp[41]);
                        casting_skill0 = null;
                        casting_skill1 = null;
                        casting_skill2 = null;

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    break;
                case 5: // 법사 메테오
                    if (!Is_CastingZone[3])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[3] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill2 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 15;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        prefab_bluecircle = Instantiate(BlueCircleRange, _character1.transform);
                        skill_prefab_temp[51] = Instantiate(skill_prefab[51], _character1.transform);

                        casting_skill1 = skill_prefab_temp[51];
                        casting_skill0 = prefab_bluecircle;
                        prefab_bluecircle.transform.localScale = Vector3.one * 3;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        casting_skill2 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    if (prefab_bluecircle)
                    {
                        //누르고 돌리는동안 위치 바뀜
                        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector3 temp = mouse_pos;
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character1.transform.position.x);
                        _character1.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character1.transform.position.y);



                        if (OptionManager.instance.Is_UseJoyStick)
                        {
                            mouse_pos = GameManage.instance.JoysitckDir;

                            prefab_bluecircle.transform.position = (Vector2)_character1.transform.position + mouse_pos * 90;
                        }
                        else
                        {
                            if (Vector2.Distance(mouse_pos, _character1.transform.position) > 7.5f)
                            {
                                vector_temp = (mouse_pos - (Vector2)_character1.transform.position).normalized * 3f;
                                vector_temp.z = -100f;
                                prefab_bluecircle.transform.localPosition = vector_temp;
                            }
                            else
                            {
                                temp.z = -100f;
                                prefab_bluecircle.transform.position = temp;
                            }
                        }
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    if (Input.GetMouseButtonUp(0) && prefab_bluecircle) //떼면 이펙트
                    {
                        //마나 쿨타임
                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 3];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        
                        skill_prefab_temp[59] = Instantiate(skill_prefab[59], prefab_bluecircle.transform.position, Quaternion.identity);
                        skill_prefab_temp[59].GetComponent<Mage_Skill4_New>().GetPlayer(_character1.gameObject);
                        skill_prefab_temp[59].transform.localRotation = Quaternion.Euler(-35, 0, 0);
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        mouse_pos = prefab_bluecircle.transform.position;
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(59, skill_prefab_temp[59].transform);
                        }
                        Destroy(prefab_redcircle);
                        Destroy(prefab_bluecircle);
                        Destroy(skill_prefab_temp[51]);
                        casting_skill0 = null;
                        casting_skill1 = null;
                        casting_skill2 = null;

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    break;
                case 6:
                    if (!Is_CastingZone[3])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[3] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 3;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);

                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 3];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        skill_prefab_temp[140] = Instantiate(skill_prefab[140], _char1_pos);
                        skill_prefab_temp[141] = Instantiate(skill_prefab[141], _char1_pos);
                        Destroy(skill_prefab_temp[140], 2f);
                        Destroy(skill_prefab_temp[141], 2f);
                        if (NetworkManager.instance.is_multi)
                        {
                            NetworkManager.instance.InstantiateOtherPlayerSkill(140, skill_prefab_temp[140].transform,2f);
                            NetworkManager.instance.InstantiateOtherPlayerSkill(141, skill_prefab_temp[141].transform,2f);
                        }
                        vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char1_pos.position;

                        StartCoroutine(ShieldSkill4(player1, vector_temp));

                        Is_CastingZone[3] = false;
                        casting_skill0 = null;

                        skill_on_1[3] = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    break;
                case 7: // 스피어 8뱡향 밀쳐내기
                    if (!Is_CastingZone[3])//바로 나오는것 원 범위
                    {
                        Is_CastingZone[3] = true;
                        if (prefab_redcircle) Destroy(prefab_redcircle);
                        prefab_redcircle = Instantiate(BlueCircleRange, _character1.transform);
                        casting_skill0 = prefab_redcircle;
                        prefab_redcircle.transform.localScale = Vector3.one * 3;
                        _character1.GetComponent<Character_Control>().Runable = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && !Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5)) //누르면 범위가 나옴
                    {
                        Destroy(prefab_redcircle);

                        player1.GetComponent<CharacterStat>().MP -= skill_mana[4 * player_1_num + 3];
                        if (player1.GetComponent<CharacterStat>().is_firstCharacter)
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[0] = 0;
                        else
                            SkillButton[3].GetComponent<ButtonScript>().cool_time_temp[1] = 0;
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        StartCoroutine(SpearSkill4(player1));

                        Is_CastingZone[3] = false;
                        casting_skill0 = null;
                        _character1.GetComponent<Character_Control>().Runable = true;
                        skill_on_1[3] = false;
                    }
                    else if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(UI_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000f, 1 << 5))//취소하려고 다시 누른 경우
                    {
                        Destroy(prefab_redcircle);
                        SkillButton[3].GetComponent<ButtonScript>().SkillOff();

                        _character1.GetComponent<Character_Control>().Runable = true;
                        Is_CastingZone[3] = false;
                        skill_on_1[3] = false;
                    }
                    break;
            }

        }
            
     //   // 12시 스킬
     //   if (skill_on_2[0])  
     //   {
     //       switch (player_2_num)
     //       {
     //           case 0: //ass 베기 방감 + 연계
     //               if (Input.GetMouseButtonDown(0)) //누르자마자
     //               {
     //                   skill_prefab_temp[10] = Instantiate(skill_prefab[10], _character2.transform); //주변 데미지 이펙트
     //                   vector_temp.x = _character2.GetComponent<Character_Control>()._animator.GetFloat("DirX");
     //                   vector_temp.y = _character2.GetComponent<Character_Control>()._animator.GetFloat("DirY");
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();
     //                   TurnSkillDir(skill_prefab_temp[10], Vector3.zero, vector_temp);

     //                   skill_on_2[0] = false;
     //               }
     //               break;
     //           case 1: //arrow 독화살
     //               if (Input.GetMouseButtonDown(0)) //눌렀을때 달리면서 쏠수있음
     //               {
     //                   skill_prefab_temp[0] = Instantiate(skill_prefab[0], _character2.transform); //기모으는 이펙트
     //                   //활시위 땡기는 애니메이션
     //                   prefab_bluestraight = Instantiate(BlueStraightRange, _character2.transform);
     //                   casting_skill0 = skill_prefab_temp[0];
     //                   casting_skill1 = prefab_bluestraight;

     //                   vector_temp = prefab_bluestraight.transform.localScale;
     //                   vector_temp.x *= 4;
     //                   prefab_bluestraight.transform.localScale = vector_temp;

     //               }

     //               mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               _character2.GetComponent<Character_Control>().Skill_ready = true;
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);

     //               if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x) > 0)
     //                   prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //               else
     //                   prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));


     //               if (Input.GetMouseButtonUp(0)) //스킬 땟을때 스킬 날아감
     //               {
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;
     //                   Destroy(prefab_bluestraight);
     //                   Destroy(skill_prefab_temp[0]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   _character2.GetComponent<Character_Control>().RunningStop();//멈추고
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   skill_prefab_temp[1] = Instantiate(skill_prefab[1], _character2.transform); //쏘는 이펙트
     //                   Destroy(skill_prefab_temp[1], 2.0f); //
     //                   if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x) > 0)
     //                       skill_prefab_temp[1].transform.rotation = Quaternion.Euler(Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)),-90, 0);
     //                   else
     //                       skill_prefab_temp[1].transform.rotation = Quaternion.Euler(180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)), -90, 0);

     //                   skill_prefab_temp[2] = Instantiate(skill_prefab[2], _character2.transform); //화살
     //                   skill_on_2[0] = false;
     //               }

     //               break;
                
     //           case 2: //Fan Tornado
     //               if (Input.GetMouseButtonDown(0)) // 눌렀을 때
     //               {
     //                   skill_prefab_temp[20] = Instantiate(skill_prefab[20], _character2.transform); //기모으는 이펙트
     //                   prefab_greenstraight = Instantiate(GreenStraightRange, _character2.transform);
     //                   casting_skill0 = skill_prefab_temp[20];
     //                   casting_skill1 = prefab_greenstraight;

     //                   vector_temp = prefab_greenstraight.transform.localScale;
     //                   vector_temp.x *= 1;
     //                   prefab_greenstraight.transform.localScale = vector_temp;
     //               }

     //               mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               _character2.GetComponent<Character_Control>().Skill_ready = true;
     //               _character2.GetComponent<Character_Control>().RunningStop();
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);

     //               if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x) > 0)
     //                   prefab_greenstraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //               else
     //                   prefab_greenstraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));


     //               if (Input.GetMouseButtonUp(0)) //스킬 땟을때 스킬 날아감
     //               {
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;
     //                   Destroy(prefab_greenstraight);
     //                   Destroy(skill_prefab_temp[20]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   skill_prefab_temp[21] = Instantiate(skill_prefab[21], _character2.transform); //토네이도

     //                   skill_on_2[0] = false;
     //               }

     //               break;
     //           case 3: //Hammer HammerDown!
     //               if (Input.GetMouseButtonDown(0)) //눌렀을때
     //               {
     //                   skill_prefab_temp[30] = Instantiate(skill_prefab[30], _character2.transform); //기모으는 이펙트
     //                   casting_skill0 = skill_prefab_temp[30];
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //               }

     //               mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               _character2.GetComponent<Character_Control>().Skill_ready = true;
     //               _character2.GetComponent<Character_Control>()._animator.SetBool("Running",true);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);
     //               vector_temp.x = mouse_pos.x - _character2.transform.position.x;
     //               vector_temp.y = mouse_pos.y - _character2.transform.position.y;
     //               vector_temp.Normalize();
     //               _character2.GetComponent<Character_Control>().transform.Translate(vector_temp * _character2.GetComponent<Character_Control>().move_speed*0.5f* Time.deltaTime);


     //               if (Input.GetMouseButtonUp(0)) //스킬 땟을때 스킬 날아감
     //               {
     //                   _character2.GetComponent<Character_Control>()._animator.SetBool("Running", false);
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;
     //                   Destroy(skill_prefab_temp[30]);
     //                   casting_skill0 = null;
     //                   skill_prefab_temp[31] = Instantiate(skill_prefab[31], _character2.transform); //쾅내려찍기
     //                   if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x) > 0)
     //                       skill_prefab_temp[31].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //                   else
     //                   {
     //                       skill_prefab_temp[31].GetComponent<ParticleSystemRenderer>().flip = new Vector3(0, 1, 0);
     //                       skill_prefab_temp[31].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //                   }

     //                   skill_on_2[0] = false;
     //               }

     //               break;

     //           case 4: // 힐러 힐링 오브 생성
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   _animator = player2.GetComponent<Animator>();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
                       
     //                   skill_prefab_temp[40] = Instantiate(skill_prefab[40], _char2_pos.transform);    // 위치 이펙트
     //                   skill_prefab_temp[41] = Instantiate(skill_prefab[41], _char2_pos.transform);
     //                   casting_skill0 = skill_prefab_temp[40];
     //                   casting_skill1 = skill_prefab_temp[41];
     //               }
     //               mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
     //               _animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);
     //               _char2_pos = _character2.transform;
     //               if (Vector2.Distance(_char2_pos.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 3f)
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   vector_temp.z = 0;
     //                   skill_prefab_temp[40].transform.position = vector_temp;
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               }
     //               else
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();
     //                   skill_prefab_temp[40].transform.position = vector_temp * 3f + _char2_pos.position;
     //                   mouse_pos = skill_prefab_temp[40].transform.position;
     //               }


     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   Destroy(skill_prefab_temp[40]);
     //                   Destroy(skill_prefab_temp[41]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   StartCoroutine(HealerSkill1(player2, mouse_pos));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[0] = false;
     //               }
     //               break;

     //           case 5: // 법사 장판
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   _animator = player2.GetComponent<Animator>();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //                   _character2.GetComponent<Character_Control>().RunningStop();
                        
     //                   skill_prefab_temp[50] = Instantiate(skill_prefab[50], _char2_pos.transform);    // 위치 이펙트
     //                   skill_prefab_temp[51] = Instantiate(skill_prefab[51], _char2_pos.transform);    // 캐스팅 이펙트
     //                   casting_skill0 = skill_prefab_temp[50];
     //                   casting_skill1 = skill_prefab_temp[51];
     //               }
     //               mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
     //               _animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);
     //               if (Vector2.Distance(_char2_pos.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < skill_range[24])
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   vector_temp.z = 0;
     //                   skill_prefab_temp[50].transform.position = vector_temp;
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               }
     //               else
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();
     //                   skill_prefab_temp[50].transform.position = vector_temp * skill_range[24] + _char2_pos.position;
     //                   mouse_pos = skill_prefab_temp[50].transform.position;
     //               }

     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   Destroy(skill_prefab_temp[50]);
     //                   Destroy(skill_prefab_temp[51]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   // TODO 존재 시간 수정 요망
     //                   StartCoroutine(MageSkill1(player2, mouse_pos, vector_temp, 5f));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[0] = false;

     //               }
     //               break;

     //           case 6:     // 쉴드 방패치기
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   skill_prefab_temp[60] = Instantiate(skill_prefab[60], _char2_pos.transform);    // 방향 이펙트
     //                   skill_prefab_temp[61] = Instantiate(skill_prefab[61], _char2_pos.transform);
     //                   skill_prefab_temp[62] = Instantiate(skill_prefab[62], _char2_pos.transform);
     //                   casting_skill0 = skill_prefab_temp[60];
     //                   casting_skill1 = skill_prefab_temp[61];
     //                   casting_skill2 = skill_prefab_temp[62];
     //                   _animator = player2.GetComponent<Animator>();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //                   _character2.GetComponent<Character_Control>().RunningStop();
                       
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //               }
     //               if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x > 0)
     //                   skill_prefab_temp[60].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //               else
     //                   skill_prefab_temp[60].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //               mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
     //               _animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);
     //               if (Vector2.Distance(_char2_pos.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 2f)
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   vector_temp.z = 0;
     //                   skill_prefab_temp[61].transform.position = vector_temp;
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               }
     //               else
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();
     //                   skill_prefab_temp[61].transform.position = vector_temp * 2f + _char2_pos.position;
     //                   mouse_pos = skill_prefab_temp[61].transform.position;
     //               }

     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   Destroy(skill_prefab_temp[60]);
     //                   Destroy(skill_prefab_temp[61]);
     //                   Destroy(skill_prefab_temp[62]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   casting_skill2 = null;
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   vector_temp.z = 0;
     //                   StartCoroutine(ShieldSkill1(player2, mouse_pos));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[0] = false;
     //               }
     //               break;

     //           case 7: // 스피어
     //               if (Input.GetMouseButtonDown(0)) // 눌렀을 때
     //               {
     //                   skill_prefab_temp[70] = Instantiate(skill_prefab[70], _char2_pos.transform);    // 방향 이펙트
     //                   skill_prefab_temp[71] = Instantiate(skill_prefab[71], _char2_pos.transform);
     //                   casting_skill0 = skill_prefab_temp[70];
     //                   casting_skill1 = skill_prefab_temp[71];
     //                   _animator = player2.GetComponent<Animator>();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //                   _character2.GetComponent<Character_Control>().RunningStop();
                        
     //               }
     //               if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x > 0)
     //                   skill_prefab_temp[70].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //               else
     //                   skill_prefab_temp[70].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));

     //               mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
     //               _animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);

     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   Destroy(skill_prefab_temp[70]);
     //                   Destroy(skill_prefab_temp[71]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   vector_temp.z = 0;

     //                   StartCoroutine(SpearSkill1(_char2_pos.position, vector_temp, _char2_pos));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[0] = false;
     //               }
     //               break;
     //       }

     //   }
     //   // 3시 스킬
     //   else if (skill_on_2[1])
     //   {
     //       switch (player_2_num)
     //       {
     //           case 0: //Ass Dash
     //               if (Input.GetMouseButtonDown(0)) //눌렀을때
     //               {
     //                   prefab_bluestraight = Instantiate(BlueStraightRange, _character2.transform);
     //                   casting_skill0 = prefab_bluestraight;
     //                   vector_temp = prefab_bluestraight.transform.localScale;
     //                   vector_temp.x *= 2;
     //                   prefab_bluestraight.transform.localScale = vector_temp;
     //                   //위치알려주도록
     //               }

     //               mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

     //               if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x) > 0)
     //                   prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //               else
     //                   prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));

     //               if (Input.GetMouseButtonUp(0)) //스킬 땟을때 스킬 날아감
     //               {
     //                   Destroy(prefab_bluestraight);
     //                   casting_skill0 = null;

     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   skill_prefab_temp[4] = Instantiate(skill_prefab[4], _character2.transform); //펑
     //                   skill_prefab_temp[4].transform.SetParent(skill_prefab_temp[4].transform.parent.parent);
     //                   Destroy(skill_prefab_temp[4], 1.0f);

     //                   skill_prefab_temp[11] = Instantiate(skill_prefab[11], _character2.transform); //대쉬
     //                   TurnSkillDir(skill_prefab_temp[11], _character2.transform.position, mouse_pos); //돌리고
     //                   skill_on_2[1] = false;
     //               }
     //               break;
     //           case 1: //Arrow 후퇴사격
     //               if (Input.GetMouseButtonDown(0)) //누르는 순간 바로 실행
     //               {
     //                   skill_prefab_temp[3] = Instantiate(skill_prefab[3], _character2.transform); //뒤로 물러나는 발자국 이펙트

     //                   skill_prefab_temp[4] = Instantiate(skill_prefab[4], _character2.transform); //뒤로 물러나는 펑 이펙트
     //                   skill_prefab_temp[4].transform.SetParent(skill_prefab_temp[4].transform.parent.parent);

     //                   skill_prefab_temp[5] = Instantiate(skill_prefab[5], _character2.transform); //화살 날리는 이펙트
     //                   vector_temp.x = _character2.GetComponent<Character_Control>()._animator.GetFloat("DirX");
     //                   vector_temp.y = _character2.GetComponent<Character_Control>()._animator.GetFloat("DirY");
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();
     //                   TurnSkillDir(skill_prefab_temp[5], Vector3.zero , vector_temp);

     //                   Destroy(skill_prefab_temp[4], 2f);
     //                   skill_on_2[1] = false;
     //               }

     //               break;
                
 				//case 2: //Fan Vanish 타겟팅
     //               if (Input.GetMouseButtonDown(0)) // 눌렀을 때
     //               {
     //                   skill_prefab_temp[22] = Instantiate(skill_prefab[22], _character2.transform);
     //                   casting_skill0 = skill_prefab_temp[22]; 
     //               }

     //               mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               _character2.GetComponent<Character_Control>().Skill_ready = true;
     //               _character2.GetComponent<Character_Control>().RunningStop();
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);
     //               //타겟팅하려고 손 가져갔을때
     //               //어떤 상대가 타겟팅 되는지 보여주어야함...
     //               Vector2 pos= Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 1000f);

     //               if(hit)
     //               {
     //                   if (hit.transform.tag == "Player")
     //                   {
     //                       hit.transform.GetComponent<SpriteRenderer>().material = Resources.Load("Material/ColorGreen") as Material;
     //                       if(gameobject_temp!=hit.transform.gameObject&&gameobject_temp!=null)
     //                       {
     //                           gameobject_temp.GetComponent<SpriteRenderer>().material = Resources.Load("Material/AlphaIntensity") as Material;
     //                       }
     //                       gameobject_temp = hit.transform.gameObject;
     //                   }
     //                   else if (hit.transform.tag == "Boss")
     //                   {
     //                       hit.transform.GetComponent<SpriteRenderer>().material = Resources.Load("Material/ColorRed") as Material;
     //                       if (gameobject_temp != hit.transform.gameObject && gameobject_temp != null)
     //                       {
     //                           gameobject_temp.GetComponent<SpriteRenderer>().material = Resources.Load("Material/AlphaIntensity") as Material;
     //                       }
     //                       gameobject_temp = hit.transform.gameObject;
     //                   }
     //               }
     //               else  //땅바닥누르는경우
     //               {
     //                   if(gameobject_temp)
     //                       gameobject_temp.GetComponent<SpriteRenderer>().material = Resources.Load("Material/AlphaIntensity") as Material;

     //                   gameobject_temp = null;
     //               }
     //               //=--------------------------------------이부분 나중에 어떻게 멀티프레이 구현할지,,,,------------------------------//
     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   Destroy(skill_prefab_temp[22]);
     //                   casting_skill0 = null;
     //                   if (gameobject_temp)
     //                   {
     //                       skill_prefab_temp[23] = Instantiate(skill_prefab[23], gameobject_temp.transform); //색깔 바뀌어있는애!!
     //                       gameobject_temp.GetComponent<SpriteRenderer>().material = Resources.Load("Material/AlphaIntensity") as Material;
     //                   }

     //                   skill_on_2[1] = false;
     //               }
     //               break;
     //           case 3: //Hammer ReduceDamage
     //               if (Input.GetMouseButtonDown(0)) //누르는 순간 바로 실행
     //               {
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   skill_prefab_temp[32] = Instantiate(skill_prefab[32], _character2.transform); //쓸때 이펙트
     //                   Destroy(skill_prefab_temp[32], 2f);
     //                   skill_prefab_temp[33] = Instantiate(skill_prefab[33], _character2.transform);
     //                   //버프 해주어야함
     //                   skill_on_2[1] = false;
     //               }

     //               break;

     //           case 4: // 힐러 범위 힐
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   _animator = player2.GetComponent<Animator>();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //                   _character2.GetComponent<Character_Control>().RunningStop();
                       
     //                   skill_prefab_temp[40] = Instantiate(skill_prefab[40], _char2_pos.transform);    // 위치 이펙트
     //                   skill_prefab_temp[41] = Instantiate(skill_prefab[41], _char2_pos.transform);
     //                   casting_skill0 = skill_prefab_temp[40];
     //                   casting_skill1 = skill_prefab_temp[41];

     //               }
     //               mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
     //               _animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);
     //               _char2_pos = _character2.transform;
     //               if (Vector2.Distance(_char2_pos.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 1f)
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   vector_temp.Normalize();
     //                   vector_temp.z = 0;
     //                   skill_prefab_temp[40].transform.position = vector_temp;
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               }
     //               else
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();
     //                   skill_prefab_temp[40].transform.position = vector_temp * 1f + _char2_pos.position;
     //                   mouse_pos = skill_prefab_temp[40].transform.position;
     //               }

     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   Destroy(skill_prefab_temp[40]);
     //                   Destroy(skill_prefab_temp[41]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   StartCoroutine(HealerSkill2(player2, mouse_pos));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[1] = false;
     //               }
     //               break;

     //           case 5: // 법사 화염 방어막
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   _animator = player2.GetComponent<Animator>();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //                   _character2.GetComponent<Character_Control>().RunningStop();
                       

     //                   skill_prefab_temp[51] = Instantiate(skill_prefab[51], _char1_pos);
     //                   casting_skill0 = skill_prefab_temp[51];
     //               }
     //               mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
     //               _animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);
     //               pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               hit = Physics2D.Raycast(pos, Vector2.zero, 1000f);

     //               if (hit)
     //               {
     //                   if (hit.transform.tag == "Player")
     //                   {
     //                       hit.transform.GetComponent<SpriteRenderer>().material = Resources.Load("Material/ColorGreen") as Material;
     //                       if (gameobject_temp != hit.transform.gameObject && gameobject_temp != null)
     //                       {
     //                           gameobject_temp.GetComponent<SpriteRenderer>().material = material_temp;
     //                       }
     //                       gameobject_temp = hit.transform.gameObject;
     //                   }
     //               }
     //               else  //땅바닥누르는경우
     //               {
     //                   if (gameobject_temp)
     //                       gameobject_temp.GetComponent<SpriteRenderer>().material = material_temp;

     //                   gameobject_temp = null;
     //               }

     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   if (gameobject_temp != null)
     //                   {
     //                       vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                       gameobject_temp.GetComponent<SpriteRenderer>().material = material_temp;
     //                       StartCoroutine(MageSkill2(gameobject_temp, player2, vector_temp));
     //                       _character2.GetComponent<Character_Control>().Skill_ready = false;
     //                       skill_on_2[1] = false;
     //                   }
     //                   else
     //                   {
     //                       Destroy(skill_prefab_temp[51]);
     //                       casting_skill0 = null;
     //                       _character2.GetComponent<Character_Control>().Skill_ready = false;
     //                       skill_on_2[1] = false;
     //                   }
     //               }
     //               break;

     //           case 6:
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   skill_prefab_temp[67] = Instantiate(skill_prefab[67], _char2_pos);
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   _animator = player2.GetComponent<Animator>();
                        
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   StartCoroutine(ShieldSkill2(player2, vector_temp));
     //                   skill_on_2[1] = false;
     //               }

     //               break;

     //           case 7:     // 스피어 대쉬후 찌르기
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
     //                   skill_prefab_temp[74] = Instantiate(skill_prefab[74], _character2.transform);
     //                   casting_skill0 = skill_prefab_temp[74];
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;

     //                   skill_prefab_temp[75] = Instantiate(skill_prefab[75], _character2.transform);
     //                   casting_skill1 = skill_prefab_temp[75];

     //               }
     //               _char2_pos = _character2.transform;
     //               if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x > 0)
     //                   skill_prefab_temp[74].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //               else
     //                   skill_prefab_temp[74].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));

     //               if (Vector2.Distance(_char2_pos.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 2f)
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();
     //                   mouse_pos = vector_temp * 2f + _char2_pos.position;
     //               }
     //               else
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();
     //                   mouse_pos = vector_temp * 2f + _char2_pos.position;
     //               }
     //               if (Input.GetMouseButtonUp(0))
     //               {
                        
     //                   _animator = player2.GetComponent<Animator>();
     //                   temp = _char2_pos.position;
     //                   skill_prefab_temp[152] = Instantiate(skill_prefab[152], _character2.transform);
     //                   skill_prefab_temp[152].transform.SetParent(skill_prefab_temp[152].transform.parent.parent);
     //                   Destroy(skill_prefab_temp[152], 0.5f);

     //                   StartCoroutine(SpearSkill2(vector_temp, mouse_pos, _char2_pos));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[1] = false;

     //               }
     //               break;
     //       }
     //   }
     //   // 6시 스킬
     //   else if (skill_on_2[2])
     //   {
     //       switch (player_2_num)
     //       {
     //           case 0: //ass power
     //               if (Input.GetMouseButtonDown(0)) //눌렀을때
     //               {
     //                   skill_prefab_temp[12] = Instantiate(skill_prefab[12], _character2.transform); //기모으는 이펙트
     //                   prefab_bluecircle = Instantiate(BlueCircleRange, _character2.transform);
     //                   prefab_bluecircle.transform.localScale *= 0.2f;

     //                   casting_skill0 = skill_prefab_temp[12];
     //                   casting_skill1 = prefab_bluecircle;

     //               }

     //               //누르는동안 범위보여주기

     //               if (Input.GetMouseButtonUp(0)) //스킬 땟을때 스킬 날아감
     //               {
     //                   Destroy(prefab_bluecircle);
     //                   Destroy(skill_prefab_temp[12]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;

     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   skill_prefab_temp[13] = Instantiate(skill_prefab[13], _character2.transform); //쏘는 이펙트
     //                   skill_on_2[2] = false;
     //               }
     //               break;
     //           case 1: //Arrow 강공격 
     //               if (Input.GetMouseButtonDown(0)) //눌렀을때
     //               {
     //                   SoundManager.instance.Play(13);

     //                   skill_prefab_temp[6] = Instantiate(skill_prefab[6], _character2.transform); //준비
     //                   timer_temp = 0;

     //                   prefab_bluestraight = Instantiate(BlueStraightRange, _character2.transform);
     //                   casting_skill0 = skill_prefab_temp[6];
     //                   casting_skill1 = prefab_bluestraight;

     //                   vector_temp = prefab_bluestraight.transform.localScale;
     //                   vector_temp.x *= 4;
     //                   prefab_bluestraight.transform.localScale = vector_temp;
     //               }

     //               _character2.GetComponent<Character_Control>().RunningStop();
     //               _character2.GetComponent<Character_Control>().Skill_ready = true;
     //               mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);


     //               if (timer_temp<5)
     //                   timer_temp += Time.deltaTime;
     //               if(skill_prefab_temp[6].transform.localScale.x<0.65)
     //                   skill_prefab_temp[6].transform.localScale *= 1+(Time.deltaTime / 5);

                    
     //               if(timer_temp<5)
     //                   vector_temp.y += Time.deltaTime * 0.2f;
     //               prefab_bluestraight.transform.localScale = vector_temp;


     //               if ((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x) > 0)
     //                   prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //               else
     //                   prefab_bluestraight.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));



     //               if (Input.GetMouseButtonUp(0)) //스킬 땟을때 스킬 날아감
     //               {
     //                   SoundManager.instance.Stop(13);


     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;
     //                   Destroy(prefab_bluestraight);
     //                   Destroy(skill_prefab_temp[6]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   skill_prefab_temp[7] = Instantiate(skill_prefab[7], _character2.transform); //팡!
     //                   skill_prefab_temp[7].transform.localScale *= 1+ (timer_temp / 5);
     //                   Destroy(skill_prefab_temp[7],1.5f);
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   skill_prefab_temp[8] = Instantiate(skill_prefab[8], _character2.transform); //화려한화살

     //                   skill_on_2[2] = false;
     //               }
     //               break;
 				
     //           case 2: //Fan 번개!
     //               if (Input.GetMouseButtonDown(0)) //눌렀을때
     //               {
     //                   skill_prefab_temp[22] = Instantiate(skill_prefab[22], _character2.transform); //준비
     //                   skill_prefab_temp[24] = Instantiate(skill_prefab[24], _character2.transform); //빠직빠직어디떨어뜨릴지
     //                   skill_prefab_temp[24].transform.SetParent(skill_prefab_temp[24].transform.parent.parent);
     //                   casting_skill0 = skill_prefab_temp[22];
     //                   casting_skill1 = skill_prefab_temp[24];
     //               }
     //               vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               vector_temp.z = -500;
     //               skill_prefab_temp[24].transform.position = vector_temp;

     //               _character2.GetComponent<Character_Control>().RunningStop();
     //               _character2.GetComponent<Character_Control>().Skill_ready = true;
     //               mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);

     //               if (Input.GetMouseButtonUp(0)) //스킬 땟을때 스킬 날아감
     //               {
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //나중에 범위 만들어야함 mousepos ㄴㄴ
     //                   Destroy(skill_prefab_temp[22]);
     //                   casting_skill0 = null;
     //                   skill_prefab_temp[25] = Instantiate(skill_prefab[25], _character2.transform); //팡!
     //                   skill_prefab_temp[25].transform.position = skill_prefab_temp[24].transform.position;
     //                   Destroy(skill_prefab_temp[24]);
     //                   casting_skill1 = null;
     //                   skill_on_2[2] = false;
     //               }
     //               break;
     //           case 3: //Hammer Buff 
     //               if (Input.GetMouseButtonDown(0)) //눌렀을때
     //               {
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   skill_prefab_temp[34] = Instantiate(skill_prefab[34], _character2.transform); //준비
     //                   Destroy(skill_prefab_temp[34], 2f);

     //                   player_transtorms = GameObject.FindGameObjectsWithTag("Player");
     //                   for(int i=0;i<player_transtorms.Length;i++)
     //                   {
     //                       //버프걸어주고
     //                       skill_prefab_temp[35] = Instantiate(skill_prefab[35], player_transtorms[i].transform);
     //                   }

     //                   skill_on_2[2] = false;
     //               }
     //               break;
     //           case 4: // 힐러 범위 공격
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   _animator = player2.GetComponent<Animator>();
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
                        
     //                   skill_prefab_temp[40] = Instantiate(skill_prefab[40], _char2_pos.transform);    // 위치 이펙트
     //                   skill_prefab_temp[41] = Instantiate(skill_prefab[41], _char2_pos.transform);

     //                   casting_skill0 = skill_prefab_temp[40];
     //                   casting_skill1 = skill_prefab_temp[41];

     //               }
     //               mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
     //               _animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);
     //               _char2_pos = _character2.transform;
     //               if (Vector2.Distance(_char2_pos.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 1f)
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   vector_temp.Normalize();
     //                   vector_temp.z = 0;
     //                   skill_prefab_temp[40].transform.position = vector_temp;
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               }
     //               else
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();
     //                   skill_prefab_temp[40].transform.position = vector_temp * 1f + _char2_pos.position;
     //                   mouse_pos = skill_prefab_temp[40].transform.position;
     //               }

     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   Destroy(skill_prefab_temp[40]);
     //                   Destroy(skill_prefab_temp[41]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   StartCoroutine(HealerSkill3(player2, mouse_pos));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[2] = false;
     //               }
     //               break;
     //           case 5: // 법사 범위 공격
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   _animator = player2.GetComponent<Animator>();
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
                        

     //                   skill_prefab_temp[56] = Instantiate(skill_prefab[56], mouse_pos, Quaternion.identity);
     //                   skill_prefab_temp[51] = Instantiate(skill_prefab[51], _char1_pos);

     //                   casting_skill0 = skill_prefab_temp[56];
     //                   casting_skill1 = skill_prefab_temp[51];

     //               }
     //               if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x > 0)
     //                   skill_prefab_temp[56].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //               else
     //                   skill_prefab_temp[56].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - _char2_pos.position.y) / (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - _char2_pos.position.x)));
     //               mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
     //               _animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);
     //               if (Vector2.Distance(_char2_pos.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 1f)
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   vector_temp.Normalize();
     //                   vector_temp.z = 0;
     //                   skill_prefab_temp[56].transform.position = vector_temp;
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               }
     //               else
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();
     //                   skill_prefab_temp[56].transform.position = vector_temp * 1f + _char2_pos.position;
     //                   mouse_pos = skill_prefab_temp[56].transform.position;
     //               }

     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   Destroy(skill_prefab_temp[56]);
     //                   Destroy(skill_prefab_temp[51]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   temp = _char2_pos.position;
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   StartCoroutine(MageSkill3(player2, mouse_pos, vector_temp));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[2] = false;
     //               }

     //               break;
     //           case 6: // 쉴드 도발
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   _animator = player2.GetComponent<Animator>();
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
                        
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;

     //                   StartCoroutine(ShieldSkill3(player2, vector_temp));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[2] = false;

     //               }
     //               break;
     //           case 7:     // 공속버프, 피흡
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   _animator = player2.GetComponent<Animator>();
                       
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //                   StartCoroutine(SpearSkill3(player2));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[2] = false;
     //               }
     //               break;
     //       }
     //   }

     //   // 9시 스킬
     //   else if (skill_on_2[3])
     //   {
     //       switch (player_2_num)
     //       {
     //           case 0: //ass cancel
     //               if (Input.GetMouseButtonDown(0)) //눌렀을때
     //               {
     //                   skill_prefab_temp[14] = Instantiate(skill_prefab[14], _character2.transform); //누르자마자 빵
     //                   vector_temp.x = _character2.GetComponent<Character_Control>()._animator.GetFloat("DirX");
     //                   vector_temp.y = _character2.GetComponent<Character_Control>()._animator.GetFloat("DirY");
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();

     //                   TurnSkillDir(skill_prefab_temp[14], Vector2.zero, vector_temp);
     //                   //스킬 돌리기
     //                   skill_on_2[3] = false;
     //               }
     //               break;
     //           case 1: //Arrow 사일런트
     //               if (Input.GetMouseButtonDown(0)) //눌렀을때
     //               {
     //                   skill_prefab_temp[9] = Instantiate(skill_prefab[9], _character2.transform); //준비
     //                   casting_skill0 = skill_prefab_temp[9];
     //               }

     //               _character2.GetComponent<Character_Control>().RunningStop();
     //               _character2.GetComponent<Character_Control>().Skill_ready = true;
     //               mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);

     //               if (Input.GetMouseButtonUp(0)) //스킬 땟을때 스킬 날아감
     //               {
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;
     //                   Destroy(skill_prefab_temp[9]);
     //                   casting_skill0 = null;
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);                       
     //                   skill_prefab_temp[80] = Instantiate(skill_prefab[80], _character2.transform); //화살
     //                   skill_on_2[3] = false;
     //               }
     //               break;				
     //           case 2: //Fan manaburn
     //               if (Input.GetMouseButtonDown(0)) //눌렀을때
     //               {
     //                   skill_prefab_temp[22] = Instantiate(skill_prefab[22], _character2.transform); //준비
     //                   casting_skill0 = skill_prefab_temp[22];
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //               }
                   
     //               //방향지정하고
     //               mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                  
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x-_character2.transform.position.x);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);

     //               if (Input.GetMouseButtonUp(0)) //스킬 땟을때 스킬 날아감
     //               {
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   Destroy(skill_prefab_temp[22]);
     //                   casting_skill0 = null;
     //                   skill_prefab_temp[26] = Instantiate(skill_prefab[26], _character2.transform); //팡!
     //                   Destroy(skill_prefab_temp[26], 1f);
     //                   skill_prefab_temp[27] = Instantiate(skill_prefab[27], _character2.transform); //마나번구체 생성

     //                   skill_on_2[3] = false;
     //               }
     //               break;

     //           case 3: //Hammer DDD
     //               if (Input.GetMouseButtonDown(0)) //눌렀을때 차징
     //               {
     //                   HammerDDD = 0;
     //                   skill_prefab_temp[30] = Instantiate(skill_prefab[30], _character2.transform); //누르면 기모으고
     //                   casting_skill0 = skill_prefab_temp[30];
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //               }

     //               mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _character2.GetComponent<Character_Control>()._animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);

     //               if (Input.GetMouseButtonUp(0)) //스킬 쾅
     //               {
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   Destroy(skill_prefab_temp[30]);
     //                   casting_skill0 = null;
     //                   StartCoroutine(DDD(_character2.transform));

     //                   skill_on_2[3] = false;
     //               }

     //               break;
     //           case 4: // 힐러 리바이브
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   _animator = player2.GetComponent<Animator>();
                       
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //                   skill_prefab_temp[40] = Instantiate(skill_prefab[40], _char2_pos.transform);    // 위치 이펙트
     //                   skill_prefab_temp[41] = Instantiate(skill_prefab[41], _char2_pos.transform);
     //                   casting_skill0 = skill_prefab_temp[40];
     //                   casting_skill1 = skill_prefab_temp[41];

     //               }
     //               mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
     //               _animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);
     //               _char2_pos = _character2.transform;
     //               if (Vector2.Distance(_char2_pos.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 4f)
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //                   vector_temp.z = 0;
     //                   skill_prefab_temp[40].transform.position = vector_temp;
     //                   mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     //               }
     //               else
     //               {
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;
     //                   vector_temp.z = 0;
     //                   vector_temp.Normalize();
     //                   skill_prefab_temp[40].transform.position = vector_temp * 4f + _char2_pos.position;
     //                   mouse_pos = skill_prefab_temp[40].transform.position;
     //               }


     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   Destroy(skill_prefab_temp[40]);
     //                   Destroy(skill_prefab_temp[41]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   StartCoroutine(HealerSkill4(player2, mouse_pos));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[3] = false;
     //               }
     //               break;
     //           case 5: // 법사 메테오
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   timer_temp = 0;
     //                   _animator = player2.GetComponent<Animator>();
                       
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //                   vector_temp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_char2_pos.position.z - 10));
     //                   skill_prefab_temp[51] = Instantiate(skill_prefab[51], _char2_pos);  // 시전 이펙트
     //                   skill_prefab_temp[59] = Instantiate(skill_prefab[59], vector_temp, Quaternion.identity);
     //                   skill_prefab_temp[59].transform.SetParent(player2.transform);
     //                   casting_skill0 = skill_prefab_temp[51];
     //                   casting_skill1 = skill_prefab_temp[59];

     //               }
     //               mouse_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
     //               _animator.SetFloat("DirX", mouse_pos.x - _character2.transform.position.x);
     //               _animator.SetFloat("DirY", mouse_pos.y - _character2.transform.position.y);
     //               TranslateMeteo(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_char2_pos.position.z - 10)), skill_prefab_temp[59]);
     //               _animator.SetFloat("DirX", mouse_pos.x);
     //               _animator.SetFloat("DirY", mouse_pos.y);
     //               if (timer_temp < 5)
     //                   timer_temp += Time.deltaTime;
     //               if (skill_prefab_temp[59] != null)
     //               {
     //                   if (skill_prefab_temp[59].transform.localScale.x < 0.65)
     //                       skill_prefab_temp[59].transform.localScale *= 1 + (Time.deltaTime / 5);
     //               }

     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   StartCoroutine(MageSKill4End(mouse_pos));
     //                   Destroy(skill_prefab_temp[51]);
     //                   Destroy(skill_prefab_temp[59]);
     //                   casting_skill0 = null;
     //                   casting_skill1 = null;
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;
     //                   skill_on_2[3] = false;
     //               }
     //               break;

     //           case 6:
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   _animator = player2.GetComponent<Animator>();
                        
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //                   skill_prefab_temp[140] = Instantiate(skill_prefab[140], _char2_pos);
     //                   casting_skill0 = skill_prefab_temp[140];
     //               }

     //               if (Input.GetMouseButtonUp(0))
     //               {
     //                   Destroy(skill_prefab_temp[140]);
     //                   casting_skill0 = null;
     //                   skill_prefab_temp[141] = Instantiate(skill_prefab[141], _char2_pos);
     //                   Destroy(skill_prefab_temp[141], 0.5f);
     //                   vector_temp = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _char2_pos.position;

     //                   StartCoroutine(ShieldSkill4(player2, vector_temp));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[3] = false;

     //               }
     //               break;


     //           case 7: // 스피어 8뱡향 밀쳐내기
     //               if (Input.GetMouseButtonDown(0))
     //               {
     //                   _animator = player2.GetComponent<Animator>();
                       
     //                   _character2.GetComponent<Character_Control>().RunningStop();
     //                   _character2.GetComponent<Character_Control>().Skill_ready = true;
     //                   StartCoroutine(SpearSkill4(player2));
     //                   _character2.GetComponent<Character_Control>().Skill_ready = false;

     //                   skill_on_2[3] = false;
     //               }
     //               break;

     //       }
     //   }
        
    }


    private void TurnSkillDir(GameObject _gameobject, Vector3 character, Vector3 togo)
    {
        if ((togo.x - character.x) > 0)
            _gameobject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((togo.y - character.y) / (togo.x - character.x)));
        else
            _gameobject.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((togo.y - character.y) / (togo.x - character.x)));

    }






    /*-------------------해머 스킬------------------------*/
    IEnumerator DDD(Transform Player)
    {
        Vector2 temp = mouse_pos;
        Player.GetComponent<Character_Control>()._animator.SetBool("DDD", true);
        Player.GetComponent<Character_Control>().Runable = false;
        skill_prefab_temp[36] = Instantiate(skill_prefab[36], Player.transform); //D
        if (temp.x - Player.position.x > 0)
        {
            skill_prefab_temp[36].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((temp.y - Player.position.y) / (temp.x - Player.position.x)));
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(36, skill_prefab_temp[36].transform);
            }
        }
        else
        {
            skill_prefab_temp[36].GetComponent<ParticleSystemRenderer>().flip = new Vector3(0, 1, 0);
            skill_prefab_temp[36].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((temp.y - Player.position.y) / (temp.x - Player.position.x)));
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(36, skill_prefab_temp[36].transform);
            }
        }
        yield return new WaitForSeconds(0.4f);
        skill_prefab_temp[37] = Instantiate(skill_prefab[37], Player.transform); //DD
        if (temp.x - Player.position.x > 0)
        {
            skill_prefab_temp[37].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((temp.y - Player.position.y) / (temp.x - Player.position.x)));
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(37, skill_prefab_temp[37].transform);
            }
        }
        else
        {
            skill_prefab_temp[37].GetComponent<ParticleSystemRenderer>().flip = new Vector3(0, 1, 0);
            skill_prefab_temp[37].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((temp.y - Player.position.y) / (temp.x - Player.position.x)));
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(37, skill_prefab_temp[37].transform);
            }
        }
        yield return new WaitForSeconds(0.4f);

        skill_prefab_temp[38] = Instantiate(skill_prefab[38], Player.transform); //DDD
        if (temp.x - Player.position.x > 0)
        {
            skill_prefab_temp[38].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((temp.y - Player.position.y) / (temp.x - Player.position.x)));
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(38, skill_prefab_temp[38].transform);
            }
        }
        else
        {
            skill_prefab_temp[38].GetComponent<ParticleSystemRenderer>().flip = new Vector3(0, 1, 0);
            skill_prefab_temp[38].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((temp.y - Player.position.y) / (temp.x - Player.position.x)));
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(38, skill_prefab_temp[38].transform);
            }
        }
        Player.GetComponent<Character_Control>()._animator.SetBool("DDD", false);
        yield return new WaitForSeconds(0.4f);
        Player.GetComponent<Character_Control>().Runable = true;
    }

    /*-------------------해머 스킬 끝---------------------*/
    
  
    /*-------------------힐러 스킬------------------------*/

    IEnumerator HealerSkill1(GameObject player, Vector3 position)
    {
        _animator.SetFloat("DirX", (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - player.transform.position.x));
        _animator.SetFloat("DirY", (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - player.transform.position.y));
        _animator.SetBool("IsMagic", true);
        // 오브젝트 생성
        position.z = -10;
        position.y += 0.6f;
        for(int i = 0; i < 3; i++)
        {
            Vector3 temp = Random.insideUnitSphere * 3f + position;
            skill_prefab_temp[42] = Instantiate(skill_prefab[42], temp, Quaternion.identity);
            skill_prefab_temp[42].transform.rotation = Quaternion.Euler(35, 180, 180);
            skill_prefab_temp[42].transform.SetParent(player.transform);
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(42, skill_prefab_temp[42].transform); //1-2
            }
            temp.y -= 0.4f;
            skill_prefab_temp[43] = Instantiate(skill_prefab[43], player.transform);
            skill_prefab_temp[43].transform.position = temp;
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(43, skill_prefab_temp[43].transform);
            }
        }

        //skill_prefab_temp[43].transform.SetParent(skill_prefab_temp[42].transform);
        yield return new WaitForSeconds(0.3f);
        _animator.SetBool("IsMagic", false);

        yield return 0;

    }

    IEnumerator HealerSkill2(GameObject player, Vector3 position)
    {
        _animator.SetFloat("DirX", (position.x - player.transform.position.x));
        _animator.SetFloat("DirY", (position.y - player.transform.position.y));
        _animator.SetBool("IsMagic", true);
        skill_prefab_temp[44] = Instantiate(skill_prefab[44], player.transform.position, Quaternion.identity);
        skill_prefab_temp[44].transform.SetParent(player.transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(44, skill_prefab_temp[44].transform);
        }
        yield return new WaitForSeconds(0.3f);


        _animator.SetBool("IsMagic", false);
        yield return 0;
    }

    IEnumerator HealerSkill3(GameObject player, Vector3 position)
    {
        _animator.SetFloat("DirX", (position.x - player.transform.position.x));
        _animator.SetFloat("DirY", (position.y - player.transform.position.y));
        _animator.SetBool("IsMagic", true);
        skill_prefab_temp[45] = Instantiate(skill_prefab[45], player.transform.position,Quaternion.identity);
        skill_prefab_temp[45].transform.SetParent(player.transform);        
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(45, skill_prefab_temp[45].transform);
        }
        yield return new WaitForSeconds(0.3f);


        _animator.SetBool("IsMagic", false);
        yield return 0;
    }

    IEnumerator HealerSkill4(GameObject player, Vector3 position)
    {
        _animator.SetFloat("DirX", (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - player.transform.position.x));
        _animator.SetFloat("DirY", (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - player.transform.position.y));
        _animator.SetBool("IsMagic", true);
        // 오브젝트 생성
        position.z = -10;
        position.y += 0.6f;

        skill_prefab_temp[49] = Instantiate(skill_prefab[49], position, Quaternion.identity);
        skill_prefab_temp[49].transform.rotation = Quaternion.Euler(35, 180, 180);
        skill_prefab_temp[49].transform.SetParent(player.transform);
        position.y -= 0.4f;
        skill_prefab_temp[48] = Instantiate(skill_prefab[48], player.transform);
        skill_prefab_temp[48].transform.position = position;
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(49, skill_prefab_temp[49].transform);
        }
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(48, skill_prefab_temp[48].transform);
        }


        //skill_prefab_temp[43].transform.SetParent(skill_prefab_temp[42].transform);
        yield return new WaitForSeconds(0.3f);
        _animator.SetBool("IsMagic", false);

        yield return 0;

    }

    /*-------------------힐러 스킬 끝---------------------*/

    /*-------------------법사 스킬------------------------*/

    IEnumerator MageSkill1(GameObject player, Vector3 casting_position,Vector3 dir, float existing_time)
    {
        _animator.SetFloat("DirX", (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - player.transform.position.x));
        _animator.SetFloat("DirY", (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - player.transform.position.y));
        _animator.SetBool("IsMagic", true);

        casting_position.z = -1;
        skill_prefab_temp[52] = Instantiate(skill_prefab[52], casting_position, Quaternion.identity);        
        skill_prefab_temp[52].transform.rotation = Quaternion.Euler(-90, 0, 0);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(52, skill_prefab_temp[52].transform, existing_time,false,true);
        }
        Destroy(skill_prefab_temp[52], existing_time);
        yield return new WaitForSeconds(0.5f);
        skill_prefab_temp[53] = Instantiate(skill_prefab[53], casting_position, Quaternion.identity);        
        skill_prefab_temp[53].transform.rotation = Quaternion.Euler(90, 0, 0);
        skill_prefab_temp[53].transform.SetParent(player.transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(53, skill_prefab_temp[53].transform, existing_time, false, true);
        }
        Destroy(skill_prefab_temp[53], existing_time);
        _animator.SetBool("IsMagic", false);
        yield return 0;
    }


    IEnumerator MageSkill2(GameObject target, GameObject player, Vector3 dir)
    {
        _animator.SetFloat("DirX", (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - player.transform.position.x));
        _animator.SetFloat("DirY", (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - player.transform.position.y));
        _animator.SetBool("IsMagic", true);

        Destroy(skill_prefab_temp[51]);
        skill_prefab_temp[54] = Instantiate(skill_prefab[54], target.transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(54, skill_prefab_temp[54].transform, 0.5f);
        }
        Destroy(skill_prefab_temp[54], 0.5f);
        yield return new WaitForSeconds(0.5f);
        skill_prefab_temp[55] = Instantiate(skill_prefab[55], player.transform);
        vector_temp = target.transform.position;
        vector_temp.y -= 0.25f;
        skill_prefab_temp[55].transform.position = vector_temp;
        skill_prefab_temp[55].GetComponent<MageSkill2>().GetTargetPlayer(target);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(55, skill_prefab_temp[55].transform,100,false,true);
        }
        _animator.SetBool("IsMagic", false);

        yield return 0;
    }

    IEnumerator MageSkill3(GameObject player, Vector3 mouse_pos, Vector3 dir)
    {
        _animator.SetFloat("DirX", (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - player.transform.position.x));
        _animator.SetFloat("DirY", (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - player.transform.position.y));
        _animator.SetBool("IsMagic", true);
        skill_prefab_temp[57] = Instantiate(skill_prefab[57], player.transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(57, skill_prefab_temp[57].transform,0.5f);
        }
        Destroy(skill_prefab_temp[57], 0.5f);
        skill_prefab_temp[58] = Instantiate(skill_prefab[58], player.transform.position, Quaternion.identity);
        skill_prefab_temp[58].transform.SetParent(player.transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(58, skill_prefab_temp[58].transform,2f);
        }
        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("IsMagic", false);

        Destroy(skill_prefab_temp[58], 2f);
        yield return 0;
    }

    IEnumerator MageSKill4End(Vector3 dir)
    {
        _animator.SetFloat("DirX", dir.x);
        _animator.SetFloat("DirY", dir.y);
        _animator.SetBool("IsMagic", true);

        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("IsMagic", false);
    }


    private void TranslateMeteo(Vector3 mouse_pos, GameObject meteo)
    {
        if(meteo != null)
        {
            Vector3 dir;
            dir.x = mouse_pos.x - meteo.transform.position.x;
            dir.y = mouse_pos.y - meteo.transform.position.y;
            dir.z = 0;

            meteo.transform.Translate(dir * 1f * Time.deltaTime);
        }
    }

    /*-------------------법사 스킬 끝---------------------*/



    /*-------------------쉴드 스킬------------------------*/
    IEnumerator ShieldSkill1(GameObject player, Vector3 mouse_pos)
    {
        _animator = player.GetComponent<Animator>();
        _animator.SetFloat("DirX", (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - player.transform.position.x));
        _animator.SetFloat("DirY", (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - player.transform.position.y));
        _animator.SetBool("IsMagic", true);
        yield return new WaitForSeconds(0.2f);


        skill_prefab_temp[68] = Instantiate(skill_prefab[68], player.transform.position, Quaternion.identity);
        skill_prefab_temp[68].GetComponent<ShieldSkill1>().GetPlayer(player);
        skill_prefab_temp[68].transform.localScale = Vector3.one * 3f;
        

        if (mouse_pos.x - player.transform.position.x > 0)
        {
            float temp = Mathf.Rad2Deg * Mathf.Atan(mouse_pos.y - player.transform.position.y) / (mouse_pos.x - player.transform.position.x);
            if(temp >= 90f)
            {
                temp = 90;
            }
            if(temp <= -90)
            {
                temp = -90;
            }
            skill_prefab_temp[68].transform.rotation = Quaternion.Euler(0, 0, temp);
        }
        else
        {
            float temp = 180 + Mathf.Rad2Deg * Mathf.Atan(mouse_pos.y - player.transform.position.y) / (mouse_pos.x - player.transform.position.x);

            skill_prefab_temp[68].transform.rotation = Quaternion.Euler(0, 0, temp);
        }
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(68, skill_prefab_temp[68].transform,100,false,true);
        }
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("IsMagic", false);

        yield return 0;
    }

    IEnumerator ShieldSkill2(GameObject player, Vector3 dir)
    {
        player_sprite = player.GetComponent<SpriteRenderer>();
        _animator = player.GetComponent<Animator>();
        _animator.SetFloat("DirX", (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - player.transform.position.x));
        _animator.SetFloat("DirY", (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - player.transform.position.y));
        _animator.SetBool("IsMagic", true);
        skill_prefab_temp[69] = Instantiate(skill_prefab[69], player.transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(69, skill_prefab_temp[69].transform,0.5f);
        }
        Destroy(skill_prefab_temp[69], 0.5f);
        yield return new WaitForSeconds(0.2f);
        player_sprite.material = Resources.Load("Material/TurnGold") as Material;
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.MaterialChange("Material/AlphaIntensity", "Material/TurnGold", 3f);
        }
        player.GetComponent<Character_Control>().Skill_ready = false;
        _animator.SetBool("IsMagic", false);
        player.GetComponent<Character_Control>().Skill_ready = false;
        yield return new WaitForSeconds(3f);
        player_sprite.material = material_temp;
        yield return 0;
    }

    IEnumerator ShieldSkill3(GameObject player, Vector3 dir)
    {
        _animator = player.GetComponent<Animator>();
        _animator.SetFloat("DirX", (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - player.transform.position.x));
        _animator.SetFloat("DirY", (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - player.transform.position.y));
        _animator.SetBool("IsMagic", true);
        yield return new WaitForSeconds(0.2f);
        skill_prefab_temp[64] = Instantiate(skill_prefab[64], player.transform);
        skill_prefab_temp[65] = Instantiate(skill_prefab[65], player.transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(64, skill_prefab_temp[64].transform);
            NetworkManager.instance.InstantiateOtherPlayerSkill(65, skill_prefab_temp[65].transform);
        }

        _character1.GetComponent<Character_Control>().Runable = true;
        _animator.SetBool("IsMagic", false);
        player.GetComponent<Character_Control>().Skill_ready = false;
        yield return 0;

    }

    IEnumerator ShieldSkill4(GameObject player, Vector3 dir)
    {
        //player_transtorms = GameObject.FindGameObjectsWithTag("Player");

        _animator = player.GetComponent<Animator>();
        _animator.SetFloat("DirX", dir.x);
        _animator.SetFloat("DirY", dir.y);
        _animator.SetBool("IsMagic", true);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("IsMagic", false);
        yield return new WaitForSeconds(0.1f);
        
        skill_prefab_temp[66] = Instantiate(skill_prefab[66], GameManage.instance.player[0].transform);
        _character1.GetComponent<Character_Control>().Runable = true;
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerAllBuff(66);
        }
    }

    /*-------------------쉴드 스킬 끝---------------------*/


    /*-----------------스피어 스킬------------------------*/

    IEnumerator SpearSkill1(Vector3 _char_pos, Vector3 mouse_pos, Transform player)
    {
        _animator = player.GetComponent<Animator>();
        _animator.SetFloat("DirX", mouse_pos.x);
        _animator.SetFloat("DirY", mouse_pos.y);


        _animator.SetBool("IsSkill1", true);
        player.gameObject.GetComponent<Character_Control>().Runable = false;
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.2f);
            _animator.SetBool("IsSkill1", false);
            _char_pos.z = 0;
            skill_prefab_temp[73] = Instantiate(skill_prefab[73], _char_pos, Quaternion.identity);
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(73, skill_prefab_temp[73].transform,100,false,true);
            }
            skill_prefab_temp[73].transform.SetParent(player);
        }
        player.gameObject.GetComponent<Character_Control>().Runable = true;

        yield return new WaitForSeconds(1f);

        yield return 0;
    }

    IEnumerator SpearSkill2(Vector3 dir, Vector3 mouse_pos, Transform player)
    {
        //Destroy(skill_prefab_temp[74]);
        //Destroy(skill_prefab_temp[75]);
        //casting_skill0 = null;
        //casting_skill1 = null;

        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.01f);

        Vector2 temp = (mouse_pos - player.position);
        temp.Normalize();
        float speed = 12f;
        int t = 0;
        //_animator.SetFloat("DirX", dir.x);
        //_animator.SetFloat("DirY", dir.y);
        _animator.SetBool("Attack", true);

        //player.gameObject.GetComponent<Character_Control>().RunningStop();

        while (true)
        {
            yield return waittime;
            player.transform.Translate(temp * speed * Time.deltaTime, Space.World);
            speed -= 0.05f;
            skill_prefab_temp[77] = Instantiate(skill_prefab[77], player.transform.position,Quaternion.identity);
            skill_prefab_temp[77].transform.localScale *= 2.5f;
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(77, skill_prefab_temp[77].transform,100,false,true);
            }
            t += 1;
            if(t > 20)
            {
                break;
            }
        }
        _character1.GetComponent<Character_Control>().Runable = true;
        yield return new WaitForSeconds(0.1f);

        skill_prefab_temp[76] = Instantiate(skill_prefab[76], player);
        if(temp.x>0)
        {
            skill_prefab_temp[76].transform.rotation = Quaternion.Euler(-1 * Mathf.Atan2(temp.y, temp.x) * Mathf.Rad2Deg, 90, -90);
        }
        else
        {
            skill_prefab_temp[76].transform.rotation = Quaternion.Euler(180 + Mathf.Atan2(temp.y, temp.x) * Mathf.Rad2Deg, -90, 90);
        }

        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(76, skill_prefab_temp[76].transform);
        }
        _animator.SetBool("Attack", false);

        yield return 0;
    }

    IEnumerator SpearSkill3(GameObject player)
    {
        _animator.SetBool("IsMagic", true);
        
        skill_prefab_temp[79] = Instantiate(skill_prefab[79], GameManage.instance.player[0].transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerAllBuff(79);
        }
        yield return new WaitForSeconds(0.4f);

        skill_prefab_temp[78] = Instantiate(skill_prefab[78], player.transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(78, skill_prefab_temp[78].transform,0.7f);
        }
        Destroy(skill_prefab_temp[78], 0.7f);
        _character1.GetComponent<Character_Control>().Runable = true;

        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("IsMagic", false);
        yield return 0;
    }

    IEnumerator SpearSkill4(GameObject player)
    {
        Vector3 temp_pos = player.transform.position;
        temp_pos.y += 8.5f;
        _animator = player.GetComponent<Animator>();
        _animator.SetBool("IsMagic", true);

        skill_prefab_temp[151] = Instantiate(skill_prefab[151], player.transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(151, skill_prefab_temp[151].transform, 0.5f);
        }
        Destroy(skill_prefab_temp[151], 0.5f);
        skill_prefab_temp[150] = Instantiate(skill_prefab[150], temp_pos, Quaternion.identity);
        skill_prefab_temp[150].transform.SetParent(player.transform);
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.InstantiateOtherPlayerSkill(150, skill_prefab_temp[150].transform);
        }
        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("IsMagic", false);
        yield return 0;
    }
    /*-----------------스피어 스킬 끝------------------------*/






    //아래 함수들은 스위치
    public void Use_player1_1_Skill()
    {
        skill_on_1[0] = true;
    }
    public void Use_player1_2_Skill()
    {
        skill_on_1[1] = true;
    }
    public void Use_player1_3_Skill()
    {
        skill_on_1[2] = true;
    }
    public void Use_player1_4_Skill()
    {
        skill_on_1[3] = true;
    }

    public void SkillOff()
    {
        for(int i=0;i<4;i++)
        {
            skill_on_1[i] = false;
        }
    }

    //public void Use_player2_1_Skill()
    //{
    //    skill_on_2[0] = true;
    //}
    //public void Use_player2_2_Skill()
    //{
    //    skill_on_2[1] = true;
    //}
    //public void Use_player2_3_Skill()
    //{
    //    skill_on_2[2] = true;
    //}
    //public void Use_player2_4_Skill()
    //{
    //    skill_on_2[3] = true;
    //}
    public void SkillCanceled() 
    {
        SoundManager.instance.Stop(13);

        if (casting_skill0)
            Destroy(casting_skill0);
        if(casting_skill1)
            Destroy(casting_skill1);
        if(casting_skill2)
            Destroy(casting_skill2);
        
        for (int i = 0; i < 4; i++)
        {
            SkillButton[i].GetComponent<ButtonScript>().Character_Change_skill_off();
            Is_CastingZone[i] = false;
            skill_on_1[i] = false;
        }

        //else
        //{
        //    for (int i = 0; i < 4; i++)
        //        skill_on_2[i] = false;
        //}
    }

    //IEnumerator CoolTime(float cool, Image skill)
    //{
    //    while(cool > 1.0f)
    //    {
    //        cool -= Time.deltaTime;
    //        skill.fillAmount = (1.0f / cool);
    //        yield return new WaitForSeconds(1f);
    //    }

    //}
}
