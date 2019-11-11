using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7Move : MonoBehaviour
{
    // 현재 실행할 코루틴을 가지고 있음. 
    IEnumerator _current;


    // 어그로 구조체
    public struct _aggro
    {
        public float distance;
        public float attackAggro;
        public float healAggro;
        public float goldHand;

        public _aggro(float dis, float atAg, float hAg, float goldHand)
        {
            this.distance = dis;
            this.attackAggro = atAg;
            this.healAggro = hAg;
            this.goldHand = goldHand;
        }
    }

    // 플레이어의 수를 확인하고 배열의 인덱스로 사용
    enum _PLAYER
    {
        NONE = -1,
        PLAYER_1 = 0,
        PLAYER_2,
        PLAYER_NUM
    }

    enum _BossAct
    {
        Stay = -1,
        Chasing,
        Attacking,  //마공
        Totem,      // 마공
        Absorb,     
        Battemp,    // 마공
        Bloodpool,  // 마공
        Moss,       // 마공
        Sickle,     // 마공
    }

    // 타겟 플레이어 임시 저장 변수
    [HideInInspector]
    public GameObject temp_target_player;

    public GameObject[] skill_prefab; //이거 나중에 resource로 옮겨서 소스에서 넣기


    // 플레이어 오브젝트 베열
    private GameObject[] player_Object;

    public float[] skill_cooltime; //스킬 쿨타임 용 배열  나중엔 private으로 바꿔야함    

    // 이동 속도
    [HideInInspector]
    public float walkSpeed = 1.0f;

    // 도착했는가 (도착했다 true / 도착하지 않았다 false)
    [HideInInspector]
    public bool arrived = false;

   
    //private bool is_sandstrom_used = false;

    private bool is_HP_first = false;
    private bool is_HP_second = false;
    private bool is_HP_third = false;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private int boss_act_num = 0;

    private int max_aggro_playerNum;

    private float[] skill_cooltime_temp;
    // 시간 임시 변수

    private float max_aggro;

    // 목적지에 도착했다고 보는 정지 거리
    const float StoppingDistance = 2 + .0f;    

    private GameObject target_player;

    private GameObject[] skill_prefab_temp; //스킬 이펙트 넣기 위한 배열
    /*
     * 0.노말 어택
     * 1.기모으기 이펙트
     * 2.일자로 나가는 범위 이펙트
     * 6.기둥 마법진
     * 7.기둥
     * 10.몬스터 나오는곳 표시
     * 11.몬스터
     * 12.모래바람
     */
    private WaitForSeconds waitforsec;

    private Vector3 pos_memo; //pos기억용

    private Vector3 vector3_temp; //스킬 활용할때 쓰는 임시용 vector

    public Vector3 vector3_throw_attack;

    // 현재 이동 속도
    private Vector3 velocity = Vector3.zero;

    // 목적지
    private Vector3 destination;

    public Camera main;

    private Vector3 temp_vec;

    private List<GameObject> normal_attack;
    private int ult_count = 0;


    private void Start()
    {
        if (NetworkManager.instance.is_multi)
        {
            player_Object = (GameObject[])BossStatus.instance.player.Clone();
        }
        else
            player_Object = (GameObject[])BossStatus.instance.player.Clone();

        skill_prefab_temp = new GameObject[skill_prefab.Length];
        skill_cooltime_temp = new float[skill_cooltime.Length];
        walkSpeed = BossStatus.instance.moveSpeed;
        normal_attack = new List<GameObject>();
        for (int i = 0; i < skill_cooltime.Length; i++)
        {
            skill_cooltime_temp[i] = 0;
        }
        _animator = this.GetComponent<Animator>();
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        waitforsec = new WaitForSeconds(0.01f);
        StartCoroutine(Exec());
    }

    private void Update()
    {
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            BossAct(boss_act_num);
        }
        else if (NetworkManager.instance.is_multi && !NetworkManager.instance.is_host)
        {
            return;
        }
        else
            BossAct(boss_act_num);

    }

    IEnumerator Exec()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            //AggroSum = BossStatus.instance.ReturnAggro();
        }
    }

    public void EndofGame()
    {
        boss_act_num = (int)_BossAct.Stay;
    }

    public void SelectTarget()
    {
        GameObject temp = BossStatus.instance.target_player;
        if (NetworkManager.instance.is_multi)
        {
            if (target_player != temp)
            {
                target_player = temp;
            }
        }
        else
        {
            target_player = GameManage.instance.player[0];
        }
    }

    private bool MovetoTarget()
    {
        if (GameManage.instance.IsGameEnd)
        {
            boss_act_num = (int)_BossAct.Stay;
            return true;
        }
        Move(target_player.transform.position);
        return false;
    }

    public void Move(Vector3 destination)
    {
        Vector3 Destination = destination;
        Destination.z = transform.position.z;


        // 목적지까지의 거리와 방향을 구한다.
        // 방향
        Vector3 direction = (Destination - transform.position).normalized;
        // 거리
        float distance = Vector3.Distance(this.gameObject.transform.position, Destination);


        _animator.SetFloat("DirX", direction.x);
        _animator.SetFloat("DirY", direction.y);
        // 현재 속도를 보관
        Vector3 currentVelocity = velocity;

        // 목적지에 가까이 왔으면 도착
        if (arrived || distance < StoppingDistance)
        {
            arrived = true;
        }

        // 일정거리에 도달하지 않으면 도착하지 않은것
        if (distance > StoppingDistance)
        {
            _animator.SetBool("Running", true);
            arrived = false;
        }

        // 도착했으면 멈추고 아니면 속도를 구함
        if (arrived)
        {
            velocity = Vector3.zero;
            if (_animator.GetBool("Running"))
            {
                _animator.SetBool("Running", false);
            }
        }
        else
            velocity = direction * walkSpeed;

        // 보간처리
        velocity = Vector3.Lerp(currentVelocity, velocity, Mathf.Min(Time.deltaTime * 5.0f, 1.0f));
        velocity.z = 0;
        walkSpeed = BossStatus.instance.moveSpeed;
        this.gameObject.transform.Translate(velocity * 2.0f * Time.deltaTime);

    }


    private void BossAct(int act_num)
    {
        BossStatus.instance.MP += Time.deltaTime;
        if (BossStatus.instance.MP > BossStatus.instance.MaxMp)
        {
            BossStatus.instance.MP = BossStatus.instance.MaxMp;
        }
        if (BossStatus.instance.HP <= 0)
        {
            act_num = (int)_BossAct.Stay;
        }


        skill_cooltime_temp[1] += Time.deltaTime; //Battemp
        skill_cooltime_temp[2] += Time.deltaTime; //Sickle
        skill_cooltime_temp[3] += Time.deltaTime; //Absorb
        skill_cooltime_temp[4] += Time.deltaTime; //Bloodpool

        SelectTarget();

        switch (act_num)
        {

            case (int)_BossAct.Stay:
                if (BossStatus.instance.Silent)
                {
                    BossGetSilent();
                    boss_act_num = (int)_BossAct.Chasing;
                    BossStatus.instance.BossSilentAble(false);
                    BossStatus.instance.Silent = false;
                }
                break;
            case (int)_BossAct.Chasing:
                if (MovetoTarget()) return;
                //boss_act_num = (int)_BossAct.Battemp; //테스트용으로 쓰는 코드
                //break;
                if (BossStatus.instance.MP >= BossStatus.instance.MaxMp) //레데 궁 : mp max일때 소환
                {
                    BossStatus.instance.MP = 0;
                    if (ult_count >= 2)
                    {
                        ult_count = 1;
                        BossStatus.instance.GetBuff(8);
                    }
                    boss_act_num = (int)_BossAct.Moss;
                    break;
                }

                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.775 && !is_HP_first) //Bat
                {
                    is_HP_first = true;
                    boss_act_num = (int)_BossAct.Totem;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.5 && !is_HP_second)
                {
                    is_HP_second = true;
                    boss_act_num = (int)_BossAct.Totem;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.225 && !is_HP_third && UpLoadData.boss_level >= 1)
                {
                    is_HP_third = true;
                    boss_act_num = (int)_BossAct.Totem;
                    break;
                }
                if (skill_cooltime_temp[3] > skill_cooltime[3] && UpLoadData.boss_level >= 1) //Absorb
                {
                    skill_cooltime_temp[3] = 0;
                    boss_act_num = (int)_BossAct.Absorb;
                    break;
                }
                if (skill_cooltime_temp[1] > skill_cooltime[1] && UpLoadData.boss_level >= 1) //Battemp
                {
                    skill_cooltime_temp[1] = 0;
                    boss_act_num = (int)_BossAct.Battemp;
                    break;
                }
                if (skill_cooltime_temp[4] > skill_cooltime[4] && UpLoadData.boss_level >= 1) //BloodPool
                {
                    skill_cooltime_temp[4] = 0;
                    boss_act_num = (int)_BossAct.Bloodpool;
                    break;
                }
                if (arrived) //평타
                {
                    skill_cooltime_temp[0] = 0;
                    boss_act_num = (int)_BossAct.Attacking;
                    break;
                }
                if (skill_cooltime_temp[2] > skill_cooltime[2]) //Sicle
                {
                    skill_cooltime_temp[2] = 0;
                    boss_act_num = (int)_BossAct.Sickle;
                    break;
                }
                break;

            case (int)_BossAct.Attacking:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[48][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[48][(int)_BossAct.Attacking - 1] = true;    //0번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[49][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[49][(int)_BossAct.Attacking - 1] = true;     // 0번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[50][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[50][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[51][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[51][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[52][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[52][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[53][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[53][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[54][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[54][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[55][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[55][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(Attack_active());
                boss_act_num = (int)_BossAct.Stay;
                break;

            case (int)_BossAct.Totem:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[48][(int)_BossAct.Totem + 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[48][(int)_BossAct.Totem + 1] = true;    //3번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[49][(int)_BossAct.Totem + 4])    // 6번 스킬
                        {
                            UpLoadData.boss_usedskill_list[49][(int)_BossAct.Totem + 4] = true;     // 6번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[50][(int)_BossAct.Totem + 4])    // 6번 스킬
                        {
                            UpLoadData.boss_usedskill_list[50][(int)_BossAct.Totem + 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[51][(int)_BossAct.Totem + 4])    // 6번 스킬
                        {
                            UpLoadData.boss_usedskill_list[51][(int)_BossAct.Totem + 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[52][(int)_BossAct.Totem + 4])    // 6번 스킬
                        {
                            UpLoadData.boss_usedskill_list[52][(int)_BossAct.Totem + 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[53][(int)_BossAct.Totem + 4])    // 6번 스킬
                        {
                            UpLoadData.boss_usedskill_list[53][(int)_BossAct.Totem + 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[54][(int)_BossAct.Totem + 4])    // 6번 스킬
                        {
                            UpLoadData.boss_usedskill_list[54][(int)_BossAct.Totem + 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[55][(int)_BossAct.Totem + 4])    // 6번 스킬
                        {
                            UpLoadData.boss_usedskill_list[55][(int)_BossAct.Totem + 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(TotemSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;

            case (int)_BossAct.Absorb:
                switch (UpLoadData.boss_level)
                {

                    case 1:
                        if (!UpLoadData.boss_usedskill_list[49][(int)_BossAct.Absorb - 1])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[49][(int)_BossAct.Absorb - 1] = true;     // 2번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[50][(int)_BossAct.Absorb - 1])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[50][(int)_BossAct.Absorb - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[51][(int)_BossAct.Absorb - 1])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[51][(int)_BossAct.Absorb - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[52][(int)_BossAct.Absorb - 1])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[52][(int)_BossAct.Absorb - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[53][(int)_BossAct.Absorb - 1])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[53][(int)_BossAct.Absorb - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[54][(int)_BossAct.Absorb - 1])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[54][(int)_BossAct.Absorb - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[55][(int)_BossAct.Absorb - 1])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[55][(int)_BossAct.Absorb - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(AbsorbSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Bloodpool:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[49][(int)_BossAct.Bloodpool - 1])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[49][(int)_BossAct.Bloodpool - 1] = true;     // 4번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[50][(int)_BossAct.Bloodpool - 1])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[50][(int)_BossAct.Bloodpool - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[51][(int)_BossAct.Bloodpool - 1])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[51][(int)_BossAct.Bloodpool - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[52][(int)_BossAct.Bloodpool - 1])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[52][(int)_BossAct.Bloodpool - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[53][(int)_BossAct.Bloodpool - 1])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[53][(int)_BossAct.Bloodpool - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[54][(int)_BossAct.Bloodpool - 1])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[54][(int)_BossAct.Bloodpool - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[55][(int)_BossAct.Bloodpool - 1])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[55][(int)_BossAct.Bloodpool - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(BloodpoolSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Moss:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[48][(int)_BossAct.Moss - 4])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[48][(int)_BossAct.Moss - 4] = true;    //  스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[49][(int)_BossAct.Moss - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[49][(int)_BossAct.Moss - 1] = true;     // 5번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[50][(int)_BossAct.Moss - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[50][(int)_BossAct.Moss - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[51][(int)_BossAct.Moss - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[51][(int)_BossAct.Moss - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[52][(int)_BossAct.Moss - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[52][(int)_BossAct.Moss - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[53][(int)_BossAct.Moss - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[53][(int)_BossAct.Moss - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[54][(int)_BossAct.Moss - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[54][(int)_BossAct.Moss - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[55][(int)_BossAct.Moss - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[55][(int)_BossAct.Moss - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(MossSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Battemp:
                switch (UpLoadData.boss_level)
                {

                    case 1:
                        if (!UpLoadData.boss_usedskill_list[49][(int)_BossAct.Battemp - 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[49][(int)_BossAct.Battemp - 1] = true;     // 3번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[50][(int)_BossAct.Battemp - 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[50][(int)_BossAct.Battemp - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[51][(int)_BossAct.Battemp - 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[51][(int)_BossAct.Battemp - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[52][(int)_BossAct.Battemp - 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[52][(int)_BossAct.Battemp - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[53][(int)_BossAct.Battemp - 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[53][(int)_BossAct.Battemp - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[54][(int)_BossAct.Battemp - 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[54][(int)_BossAct.Battemp - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[55][(int)_BossAct.Battemp - 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[55][(int)_BossAct.Battemp - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(Battempkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Sickle:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[48][(int)_BossAct.Sickle - 6])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[48][(int)_BossAct.Sickle - 6] = true;    // 1번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[49][(int)_BossAct.Sickle - 6])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[49][(int)_BossAct.Sickle - 6] = true;     // 1번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[50][(int)_BossAct.Sickle - 6])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[50][(int)_BossAct.Sickle - 6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[51][(int)_BossAct.Sickle - 6])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[51][(int)_BossAct.Sickle - 6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[52][(int)_BossAct.Sickle - 6])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[52][(int)_BossAct.Sickle - 6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[53][(int)_BossAct.Sickle - 6])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[53][(int)_BossAct.Sickle - 6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[54][(int)_BossAct.Sickle - 6])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[54][(int)_BossAct.Sickle - 6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[55][(int)_BossAct.Sickle - 6])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[55][(int)_BossAct.Sickle - 6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(SickleSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
        }
    }

    // 기본 공격
    IEnumerator Attack_active()
    {
        _animator.SetBool("Running", false);


        for(int i=0;i<8;i++)
        {
            skill_prefab_temp[4] = Instantiate(skill_prefab[4], this.transform.position,Quaternion.Euler(0,0,i*45)); // Warning
            skill_prefab_temp[4].transform.localScale = new Vector3(2f, 2f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 4, skill_prefab_temp[4].transform, 2, false, false);
            }
            skill_prefab_temp[4] = Instantiate(skill_prefab[4], this.transform.position, Quaternion.Euler(0, 180, i * 45)); // Warning
            skill_prefab_temp[4].transform.localScale = new Vector3(2f, 2f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 3, skill_prefab_temp[3].transform, 2, false, false);
            }
        }
        yield return new WaitForSeconds(1.5f);
        _animator.SetBool("Attack", true);
        main.GetComponent<Camera_move>().VivrateForTime(1.0f);
        for (int i = 0; i < 8; i++)
        {
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform.position,Quaternion.Euler(0,0,i*45)); // Warning
            skill_prefab_temp[2].transform.localScale = new Vector3(2f, 2f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, 2, false, false);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform.position, Quaternion.Euler(0, 180, i * 45)); // Warning
            skill_prefab_temp[2].transform.localScale = new Vector3(2f, 2f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, 2, false, false);
            }
        }
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Attack", false);

        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator TotemSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);

        int count = UpLoadData.boss_level + 1;

        for(int i = 0; i < player_Object.Length; i++)
        {
            if (player_Object[i].CompareTag("Player"))
            {
                skill_prefab_temp[1] = Instantiate(skill_prefab[1], player_Object[i].transform); // Warning
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateTargetingBossSkill(UpLoadData.boss_index, 1, player_Object[i].GetComponent<CharacterStat>().My_Index, 2, true);
                }
                Destroy(skill_prefab_temp[1], 2f);
            }
            else
                continue;
        }
        

        yield return new WaitForSeconds(2f);
        _animator.SetBool("Magic", false);


        for (int i = 0; i < count; i++)
        {
            if (player_Object[i].CompareTag("Player"))
            {
                Vector2 temp = player_Object[i].transform.position;
                temp.y += 50f;
                skill_prefab_temp[3] = Instantiate(skill_prefab[3], temp, Quaternion.identity); // Warning
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 3, skill_prefab_temp[3].transform, 2, false, false);
                }
                yield return new WaitForSeconds(1f);
            }
            else
            {
                continue;
            }
        }

        _animator.SetBool("MagicEnd", true);
        yield return new WaitForSeconds(1f);
        _animator.SetBool("MagicEnd", false);
        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;

    }

    IEnumerator AbsorbSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);
        yield return new WaitForSeconds(1f);

        skill_prefab_temp[6] = Instantiate(skill_prefab[6], this.transform);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, 2, false, true);
        }
        _animator.SetBool("Magic", false);
        

        _animator.SetBool("MagicEnd", true);
        yield return new WaitForSeconds(1f);

        _animator.SetBool("MagicEnd", false);
        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator BloodpoolSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);

        yield return new WaitForSeconds(1.0f);

        Vector2[] pos;

        pos = new Vector2[player_Object.Length];
            

        for (int j=0;j<8;j++)
        {
            for (int i = 0; i < player_Object.Length; i++)
            {
                if (!player_Object[i].CompareTag("Player")) continue;

                skill_prefab_temp[1] = Instantiate(skill_prefab[1], player_Object[i].transform.position, Quaternion.identity);
                skill_prefab_temp[1].transform.localScale = 2 * Vector2.one;
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 1, skill_prefab_temp[1].transform, 1, true, false);
                }
                pos[i] = skill_prefab_temp[1].transform.position;
                Destroy(skill_prefab_temp[1], 1f);
            }
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < player_Object.Length; i++)
            {
                if (!player_Object[i].CompareTag("Player")) continue;

                skill_prefab_temp[11] = Instantiate(skill_prefab[11], pos[i], Quaternion.Euler(-90, 0, 0));
                skill_prefab_temp[11].transform.localScale = 3 * Vector3.one;
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 11, skill_prefab_temp[11].transform, 2, false, false);
                }
            }
        }

        



        //for (int i = 0; i < player_Object.Length; i++)
        //{
        //    skill_prefab_temp[7] = Instantiate(skill_prefab[7], this.transform.position, Quaternion.identity);
        //    skill_prefab_temp[7].GetComponent<Boss7_BloodPoolMove>().GetTargetPlayer(player_Object[i]);
        //}
        _animator.SetBool("Magic", false);

        _animator.SetBool("MagicEnd", true);
        yield return new WaitForSeconds(1f);

        _animator.SetBool("MagicEnd", false);
        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }    
    IEnumerator MossSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);

        float timer = 0;
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.01f);

        yield return new WaitForSeconds(1.5f); //기다렸다가

        for (int i = 0; i < UpLoadData.boss_level*3+12; i++)
        {
            skill_prefab_temp[8] = Instantiate(skill_prefab[8], this.transform.position, Quaternion.identity);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 8, skill_prefab_temp[2].transform, 2, false, false);
            }
        }

        yield return new WaitForSeconds(1f); //기다렸다가
        _animator.SetBool("Magic", false);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("MagicEnd", true);

        while (true)
        {
            if (timer > 8) break;
            timer += Time.deltaTime;
            MovetoTarget();
            yield return waittime;
        }
        _animator.SetBool("MagicEnd", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator Battempkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);

        skill_prefab_temp[1] = Instantiate(skill_prefab[1], this.transform);
        skill_prefab_temp[1].transform.localScale = Vector2.one * 40f;
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 1, skill_prefab_temp[1].transform, 2, true, true);
        }
        Destroy(skill_prefab_temp[1], 2f);

        BossStatus.instance.BossSilentAble(true, skill_prefab_temp[1]);
        yield return new WaitForSeconds(2f);
        BossStatus.instance.BossSilentAble(false);

        skill_prefab_temp[9] = Instantiate(skill_prefab[9], this.transform);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 9, skill_prefab_temp[9].transform, 2, false, true);
        }
        _animator.SetBool("Magic", false);

        skill_prefab_temp[1] = Instantiate(skill_prefab[1], this.transform);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 1, skill_prefab_temp[1].transform, 2, true, true);
        }
        Destroy(skill_prefab_temp[1], 2f);
        yield return new WaitForSeconds(2f);
        skill_prefab_temp[9] = Instantiate(skill_prefab[9], this.transform);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 9, skill_prefab_temp[9].transform, 2, false, true);
        }
        skill_prefab_temp[1] = Instantiate(skill_prefab[1], this.transform);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 1, skill_prefab_temp[1].transform, 2, true, true);
        }
        Destroy(skill_prefab_temp[1], 2f);
        yield return new WaitForSeconds(2f);
        skill_prefab_temp[9] = Instantiate(skill_prefab[9], this.transform);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 9, skill_prefab_temp[9].transform, 2, false, true);
        }
        yield return new WaitForSeconds(1f);

        _animator.SetBool("MagicEnd", true);
        yield return new WaitForSeconds(5f);

        _animator.SetBool("MagicEnd", false);
        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }
	IEnumerator SickleSkill()
    {
        _animator.SetBool("Running", false);
        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform);
        skill_prefab_temp[0].transform.localScale = new Vector3(17f, 4.5f, 1f);
        Vector2 temp = target_player.transform.position;
        if (target_player.transform.position.x - this.transform.position.x > 0)
        {
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));
        }
        else
        {
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));

        }
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 2, true, true);
        }
        Destroy(skill_prefab_temp[0], 2f);

        yield return new WaitForSeconds(1.5f);
        _animator.SetBool("Attack", true);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Attack", false);
        yield return new WaitForSeconds(0.4f);

        SoundManager.instance.Play(56);
        skill_prefab_temp[10] = Instantiate(skill_prefab[10], this.transform.position, Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.Boss7Sickle(temp);
        }
        skill_prefab_temp[10].GetComponent<Boss7_SicleRotate>().GetDir(temp);
        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(2f);
        SoundManager.instance.Stop(56);

        boss_act_num = (int)_BossAct.Chasing;
    }
    public void BossGetSilent()
    {
        StopAllCoroutines();
        StartCoroutine(Exec());

    }
    public void BossGetSilent(GameObject prefab1, GameObject prefab2)
    {
        Destroy(prefab1);
        Destroy(prefab2);
        StopAllCoroutines();
        StartCoroutine(Exec());

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadPlayer"))
        {

            Physics2D.IgnoreCollision(this.GetComponent<CapsuleCollider2D>(), collision.GetComponent<CapsuleCollider2D>());
        }

    }

    public void ChangeBossAct(int index)
    {
        boss_act_num = index;
    }

    public void StopMagneticCor()
    {
        _animator.SetBool("AttackEnd", false);
        _animator.SetBool("MagicEnd", false);


        StopAllCoroutines();
    }
}
