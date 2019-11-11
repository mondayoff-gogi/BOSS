using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Move : MonoBehaviour
{
    // 현재 실행할 코루틴을 가지고 있음. 
    IEnumerator _current;


    // 어그로 구조체
    public struct _aggro
    {
        public float distance;
        public float attackAggro;
        public float healAggro;

        public _aggro(float dis, float atAg, float hAg)
        {
            this.distance = dis;
            this.attackAggro = atAg;
            this.healAggro = hAg;
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
        Attacking,  // 마공
        Rain,   // 마공
        PinBall,    // 마공
        Stone,  // 즉사
        Pile,   // 물공
        Emission,   //마공
        RotateAround,   // 마공
    }

    // 타겟 플레이어 임시 저장 변수
    [HideInInspector]
    public GameObject temp_target_player;

    public GameObject[] skill_prefab; //이거 나중에 resource로 옮겨서 소스에서 넣기

    // 플레이어 오브젝트 배열
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

    private WaitForSeconds waitforsec;

    private Vector3 pos_memo; //pos기억용

    private Vector3 vector3_temp; //스킬 활용할때 쓰는 임시용 vector

    public Vector3 vector3_throw_attack;
    public Vector2 vector2_throw_attack;
    // 현재 이동 속도
    private Vector3 velocity = Vector3.zero;

    // 목적지
    private Vector3 destination;

    public Camera main;

    private Vector3 temp_vec;
    private ParticleSystem ps;
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


    //public void DecreaseAggro()
    //{
    //    for (int i = 0; i < (int)_PLAYER.PLAYER_NUM; i++)
    //    {
    //        AggroSum[i] -= 2.0f;
    //    }
    //}

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


        skill_cooltime_temp[1] += Time.deltaTime; //RainSkill
        skill_cooltime_temp[2] += Time.deltaTime; //Stone
        skill_cooltime_temp[3] += Time.deltaTime; //Emiision
        skill_cooltime_temp[4] += Time.deltaTime; //RotateAround

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
                //boss_act_num = (int)_BossAct.RotateAround; //테스트용으로 쓰는 코드
                //break;
                if (BossStatus.instance.MP >= BossStatus.instance.MaxMp) //mp max일때 소환
                {
                    BossStatus.instance.MP = 0;
                    ult_count++;
                    if (ult_count >= 2)
                    {
                        ult_count = 1;
                        BossStatus.instance.GetBuff(8);
                    }
                    boss_act_num = (int)_BossAct.PinBall;
                    break;
                }

                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.775 && !is_HP_first)
                {
                    is_HP_first = true;
                    boss_act_num = (int)_BossAct.Pile;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.5 && !is_HP_second && UpLoadData.boss_level >= 2)
                {
                    is_HP_second = true;
                    boss_act_num = (int)_BossAct.Pile;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.225 && !is_HP_third && UpLoadData.boss_level >= 3)
                {
                    is_HP_third = true;
                    boss_act_num = (int)_BossAct.Pile;
                    break;
                }
                if (skill_cooltime_temp[4] > skill_cooltime[4] && UpLoadData.boss_level >= 2)//Rotate
                {
                    skill_cooltime_temp[4] = 0;
                    boss_act_num = (int)_BossAct.RotateAround;
                    break;
                }
                if (skill_cooltime_temp[3] > skill_cooltime[3]) //Emission
                {
                    skill_cooltime_temp[3] = 0;
                    boss_act_num = (int)_BossAct.Emission;
                    break;
                }
                if (skill_cooltime_temp[2] > skill_cooltime[2] && UpLoadData.boss_level >= 1) // Stone
                {
                    skill_cooltime_temp[2] = 0;
                    boss_act_num = (int)_BossAct.Stone;
                    break;
                }
                if (skill_cooltime_temp[1] > skill_cooltime[1] && UpLoadData.boss_level >= 2) //Rain
                {
                    skill_cooltime_temp[1] = 0;
                    boss_act_num = (int)_BossAct.Rain;
                    break;
                }
                if (arrived) //평타
                {
                    skill_cooltime_temp[0] = 0;
                    boss_act_num = (int)_BossAct.Attacking;
                    break;
                }
                break;

            case (int)_BossAct.Attacking:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[16][(int)_BossAct.Attacking - 1])    // 0번스킬
                        {
                            UpLoadData.boss_usedskill_list[16][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[17][(int)_BossAct.Attacking - 1])   // 1번스킬
                        {
                            UpLoadData.boss_usedskill_list[17][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[18][(int)_BossAct.Attacking - 1])   // 3번스킬  
                        {
                            UpLoadData.boss_usedskill_list[18][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[19][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[19][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[20][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[20][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[21][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[21][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[22][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[22][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[23][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[23][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(Attack_active());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Rain:
                switch (UpLoadData.boss_level)
                {
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[18][(int)_BossAct.Rain + 3])   // 3번스킬  
                        {
                            UpLoadData.boss_usedskill_list[18][(int)_BossAct.Attacking + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[19][(int)_BossAct.Attacking + 3])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[19][(int)_BossAct.Attacking + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[20][(int)_BossAct.Attacking + 3])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[20][(int)_BossAct.Attacking + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[21][(int)_BossAct.Attacking + 3])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[21][(int)_BossAct.Attacking + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[22][(int)_BossAct.Attacking + 3])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[22][(int)_BossAct.Attacking + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[23][(int)_BossAct.Attacking + 3])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[23][(int)_BossAct.Attacking + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(RainSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.PinBall:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[16][(int)_BossAct.PinBall - 1])    // 0번스킬
                        {
                            UpLoadData.boss_usedskill_list[16][(int)_BossAct.PinBall - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[17][(int)_BossAct.PinBall - 1])   // 1번스킬
                        {
                            UpLoadData.boss_usedskill_list[17][(int)_BossAct.PinBall - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[18][(int)_BossAct.PinBall + 3])   // 3번스킬  
                        {
                            UpLoadData.boss_usedskill_list[18][(int)_BossAct.PinBall + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[19][(int)_BossAct.PinBall + 3])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[19][(int)_BossAct.PinBall + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[20][(int)_BossAct.PinBall + 3])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[20][(int)_BossAct.PinBall + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[21][(int)_BossAct.PinBall + 3])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[21][(int)_BossAct.PinBall + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[22][(int)_BossAct.PinBall + 3])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[22][(int)_BossAct.PinBall + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[23][(int)_BossAct.PinBall + 3])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[23][(int)_BossAct.PinBall + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(PinBallSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Stone:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[17][(int)_BossAct.Stone - 2])   // 1번스킬
                        {
                            UpLoadData.boss_usedskill_list[17][(int)_BossAct.Stone - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[18][(int)_BossAct.Stone - 2])   // 3번스킬  
                        {
                            UpLoadData.boss_usedskill_list[18][(int)_BossAct.Stone - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[19][(int)_BossAct.Stone - 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[19][(int)_BossAct.Stone - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[20][(int)_BossAct.Stone - 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[20][(int)_BossAct.Stone - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[21][(int)_BossAct.Stone - 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[21][(int)_BossAct.Stone - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[22][(int)_BossAct.Stone - 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[22][(int)_BossAct.Stone - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[23][(int)_BossAct.Stone - 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[23][(int)_BossAct.Stone - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(Stone());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Pile:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[16][(int)_BossAct.Pile - 2])    // 0번스킬
                        {
                            UpLoadData.boss_usedskill_list[16][(int)_BossAct.Pile - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[17][(int)_BossAct.Pile - 1])   // 1번스킬
                        {
                            UpLoadData.boss_usedskill_list[17][(int)_BossAct.Pile - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[18][(int)_BossAct.Pile + 2])   // 3번스킬  
                        {
                            UpLoadData.boss_usedskill_list[18][(int)_BossAct.Pile + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[19][(int)_BossAct.Pile + 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[19][(int)_BossAct.Pile + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[20][(int)_BossAct.Pile + 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[20][(int)_BossAct.Pile + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[21][(int)_BossAct.Pile + 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[21][(int)_BossAct.Pile + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[22][(int)_BossAct.Pile + 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[22][(int)_BossAct.Pile + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[23][(int)_BossAct.Pile + 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[23][(int)_BossAct.Pile + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(Pile());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Emission:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[16][(int)_BossAct.Emission - 4])    // 0번스킬
                        {
                            UpLoadData.boss_usedskill_list[16][(int)_BossAct.Emission - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[17][(int)_BossAct.Emission - 4])   // 1번스킬
                        {
                            UpLoadData.boss_usedskill_list[17][(int)_BossAct.Emission - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[18][(int)_BossAct.Emission - 4])   // 3번스킬  
                        {
                            UpLoadData.boss_usedskill_list[18][(int)_BossAct.Emission - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[19][(int)_BossAct.Emission - 4])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[19][(int)_BossAct.Emission - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[20][(int)_BossAct.Emission - 4])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[20][(int)_BossAct.Emission - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[21][(int)_BossAct.Emission - 4])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[21][(int)_BossAct.Emission - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[22][(int)_BossAct.Emission - 4])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[22][(int)_BossAct.Emission - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[23][(int)_BossAct.Emission - 4])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[23][(int)_BossAct.Emission - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(EmissionSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.RotateAround:
                switch (UpLoadData.boss_level)
                {
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[18][(int)_BossAct.RotateAround - 4])   // 3번스킬  
                        {
                            UpLoadData.boss_usedskill_list[18][(int)_BossAct.RotateAround - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[19][(int)_BossAct.RotateAround - 4])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[19][(int)_BossAct.RotateAround - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[20][(int)_BossAct.RotateAround - 4])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[20][(int)_BossAct.RotateAround - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[21][(int)_BossAct.RotateAround - 4])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[21][(int)_BossAct.RotateAround - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[22][(int)_BossAct.RotateAround - 4])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[22][(int)_BossAct.RotateAround - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[23][(int)_BossAct.RotateAround - 4])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[23][(int)_BossAct.RotateAround - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(RotateAroundSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;

        }
    }


    // 기본 공격
    IEnumerator Attack_active()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);

        // 경고
        Vector2 temp = target_player.transform.position;
        skill_prefab_temp[1] = Instantiate(skill_prefab[1], target_player.transform.position, Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1, true, false);
        }
        Destroy(skill_prefab_temp[1], 1f);

        yield return new WaitForSeconds(1f);
        SoundManager.instance.Play(43);
        skill_prefab_temp[3] = Instantiate(skill_prefab[3], this.transform.position, Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 3, skill_prefab_temp[3].transform, 1, false, false);
        }
        skill_prefab_temp[3].GetComponent<Boss3_NormalAttack>().GetDir(temp);
        _animator.SetBool("Magic", false);

        _animator.SetBool("MagicEnd", true);
        yield return new WaitForSeconds(2f);
        _animator.SetBool("MagicEnd", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;


    }
    IEnumerator RainSkill()
    {
        _animator.SetBool("Running", false);


        _animator.SetFloat("DirX", 0);
        _animator.SetFloat("DirY", -1);

        _animator.SetBool("Magic", true);

        BossStatus.instance.BossSilentAble(true);


        int num_rain = 13;

        for(int i=0;i< num_rain; i++)
        {
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], new Vector2(-12+2*i, 9), Quaternion.Euler(0,0,-90)); // Warning
            skill_prefab_temp[0].transform.localScale = new Vector3(50f, 2f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, num_rain * 0.05f, true, false);
            }
            Destroy(skill_prefab_temp[0], num_rain * 0.05f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], new Vector2(12 - 2 * i, 9), Quaternion.Euler(0, 0, -90)); // Warning
            skill_prefab_temp[0].transform.localScale = new Vector3(50f, 2f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, num_rain * 0.05f, true, false);
            }
            Destroy(skill_prefab_temp[0], num_rain * 0.05f);
            yield return new WaitForSeconds(0.2f);
        }

        float timer = 3f/(UpLoadData.boss_level+5);

        for (int i = 0; i < num_rain; i++)
        {
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], new Vector2(-12 + 2 * i, 9), Quaternion.Euler(0, 0, -90)); // Warning
            skill_prefab_temp[2].transform.localScale = new Vector3(2f, 2f,2f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, 0, false, false);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], new Vector2(12 - 2 * i, 9), Quaternion.Euler(0, 0, -90)); // Warning
            skill_prefab_temp[2].transform.localScale = new Vector3(2f, 2f,2f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, 0, false, false);
            }
            main.GetComponent<Camera_move>().VivrateForTime(0.2f);
            SoundManager.instance.Play(72);
            yield return new WaitForSeconds(timer);
            
        }

        BossStatus.instance.BossSilentAble(false);


        _animator.SetBool("Magic", false);

        _animator.SetBool("MagicEnd", true);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("MagicEnd", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;

    }  

    IEnumerator PinBallSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);

        Vector2 pos;

        int ball_num;

        ball_num = UpLoadData.boss_level*2+8;

        for(int i=0;i< ball_num; i++) //난이도 별로 갯수조절해야할듯
        {
            pos = target_player.transform.position;
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            var simulate = ps.main;
            simulate.simulationSpeed = (float)1 / 0.7f;
            skill_prefab_temp[0].transform.localScale = new Vector3(20f, 1f);
            if (target_player.transform.position.x - this.transform.position.x > 0)
                skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));
            else
                skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));

            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 0.7f, true, false);
            }

            Destroy(skill_prefab_temp[0],0.7f);
            yield return new WaitForSeconds(0.5f);
            vector2_throw_attack = pos;
            SoundManager.instance.Play(73);
            skill_prefab_temp[4] = Instantiate(skill_prefab[4], this.transform); // 핀볼 생성
            skill_prefab_temp[4].transform.localScale = new Vector3(0.5f, 0.5f,0.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 4, skill_prefab_temp[4].transform, 0.7f, false, true);
            }
        }

        
       

        _animator.SetBool("Magic", false);
        
        _animator.SetBool("MagicEnd", true);
        if(UpLoadData.boss_level>=2)
            yield return new WaitForSeconds(18.0f);
        else
            yield return new WaitForSeconds(10.0f);

        _animator.SetBool("MagicEnd", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator Stone()
    {
        _animator.SetBool("Running", false);
        skill_prefab_temp[5] = Instantiate(skill_prefab[5], this.transform);
        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform);
        if (target_player.transform.position.x - this.transform.position.x > 0)
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));
        else
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));

        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 5, skill_prefab_temp[5].transform, 2, true, true);
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 2, true, true);
        }
        Destroy(skill_prefab_temp[0], 2f);
        Destroy(skill_prefab_temp[5], 2f);
        Vector2 temp = target_player.transform.position;
        yield return new WaitForSeconds(2f);

        
        _animator.SetBool("Attack", true);
        SoundManager.instance.Play(11);
        skill_prefab_temp[6] = Instantiate(skill_prefab[6], this.transform.position, Quaternion.identity);
        skill_prefab_temp[6].transform.localScale = new Vector3(5, 4, 4);
        if (temp.x - this.transform.position.x > 0)
            skill_prefab_temp[6].transform.rotation = Quaternion.Euler(-Mathf.Rad2Deg * Mathf.Atan((temp.y - this.transform.position.y) / (temp.x - this.transform.position.x)), 90, 180);
        else
            skill_prefab_temp[6].transform.rotation = Quaternion.Euler(Mathf.Rad2Deg * Mathf.Atan((temp.y - this.transform.position.y) / (temp.x - this.transform.position.x)), -90, 0);

        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 6, skill_prefab_temp[6].transform, 1, true, false);
        }
        Destroy(skill_prefab_temp[6], 1f);

        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Attack", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }
    IEnumerator Pile()
    {
        _animator.SetBool("Running", false);

        WaitForSeconds waittime;

        float time = 6f / (UpLoadData.boss_level + 4);

        waittime = new WaitForSeconds(time);


        List<GameObject> alive_player = new List<GameObject>();
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < player_Object.Length; i++)
        {
            if (player_Object[i].tag == "Player")
            {
                alive_player.Add(player_Object[i]);
            }
        }

        if(alive_player.Count <= 1)
        {
            skill_prefab_temp[1] = Instantiate(skill_prefab[1], alive_player[0].transform);
            ps = skill_prefab_temp[1].GetComponent<ParticleSystem>();
            var simulate = ps.main;
            simulate.simulationSpeed = (float)1 / time;
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateTargetingBossSkill(UpLoadData.boss_index, 1, alive_player[0].GetComponent<CharacterStat>().My_Index, time, true);
            }
            Destroy(skill_prefab_temp[1], time);
        }
        else
        {
            for(int i = 0; i < alive_player.Count; i++)
            {
                if((i % 2) == 1)
                {
                    skill_prefab_temp[1] = Instantiate(skill_prefab[1], alive_player[i].transform);
                    if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                    {
                        NetworkManager.instance.InstantiateTargetingBossSkill(UpLoadData.boss_index, 1, alive_player[i].GetComponent<CharacterStat>().My_Index, time, true);
                    }
                    ps = skill_prefab_temp[1].GetComponent<ParticleSystem>();
                    var simulate = ps.main;
                    simulate.simulationSpeed = (float)1 / time;
                    Destroy(skill_prefab_temp[1], time);
                }
            }
        }
        // 1초 사이에 죽은 경우?
        yield return waittime;
        _animator.SetBool("Attack", true);

        for(int i = 0; i < 4; i++)
        {
            skill_prefab_temp[i + 7] = skill_prefab[i + 7];
        }

        main.GetComponent<Camera_move>().VivrateForTime(1f);
        if(alive_player.Count <= 1)
        {
            Vector3 rand_vec;
            rand_vec.x = Random.Range(-0.1f, 0.1f);            
            rand_vec.y = Random.Range(-0.1f, 0.1f);
            rand_vec.z = -10;
            skill_prefab_temp[7].SetActive(true);
            skill_prefab_temp[7].GetComponent<Boss3_Pile>().GetPosition(alive_player[0].transform.position);
            skill_prefab_temp[7].GetComponent<Boss3_Pile>().GetPlayer(alive_player[0]);
            skill_prefab_temp[7].GetComponent<Monster>().MonsterHpbarSetActive();
            skill_prefab_temp[7].GetComponent<Boss3_Pile>().StartCoturines();
            skill_prefab_temp[7].transform.position = alive_player[0].transform.position + rand_vec;
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.Boss3Pile(7, alive_player[0].GetComponent<CharacterStat>().My_Index, rand_vec);
            }

            skill_prefab_temp[12] = Instantiate(skill_prefab[12], skill_prefab_temp[7].transform.position, Quaternion.identity);
            skill_prefab_temp[12].transform.localScale = new Vector3(3, 3, 3);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 12, skill_prefab_temp[12].transform, 1, true, false);
            }
            Destroy(skill_prefab_temp[12],1f);
        }

        for (int i = 0; i < alive_player.Count; i++)
        {
            if(alive_player[i].tag == "Player" && (i % 2) == 1)
            {
                skill_prefab_temp[(i / 2) + 7].SetActive(true);
                skill_prefab_temp[(i / 2) + 7].GetComponent<Boss3_Pile>().GetPosition(alive_player[i].transform.position);
                skill_prefab_temp[(i / 2) + 7].GetComponent<Boss3_Pile>().GetPlayer(alive_player[i]);
                skill_prefab_temp[(i / 2) + 7].GetComponent<Monster>().MonsterHpbarSetActive();
                skill_prefab_temp[(i / 2) + 7].GetComponent<Boss3_Pile>().StartCoturines();
                skill_prefab_temp[(i / 2) + 7].transform.position = alive_player[i].transform.position;

                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.Boss3Pile((i / 2) + 7, alive_player[i].GetComponent<CharacterStat>().My_Index, Vector3.zero);
                }

            }
        }

        _animator.SetBool("Attack", false);

        yield return new WaitForSeconds(4f);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;

    }

    IEnumerator EmissionSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);

        WaitForSeconds waittime;

        float time = 6f / (UpLoadData.boss_level + 4);
        
        waittime = new WaitForSeconds(time);


        BossStatus.instance.BossSilentAble(true);


        for (int i = 0; i < 8; i++)
        {
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], BossStatus.instance.transform); // Warning
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            var simulate = ps.main;
            simulate.simulationSpeed = (float)1 / time;
            skill_prefab_temp[0].transform.localScale = new Vector3(40f, 1f);
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, i * 45);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, time, true, true);
            }
            Destroy(skill_prefab_temp[0], time);            
        }
        yield return waittime;

        BossStatus.instance.BossSilentAble(false);

        SoundManager.instance.Play(72);

        for (int i = 0; i < 8; i++)
        {
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], BossStatus.instance.transform.position, Quaternion.Euler(0, 0, i*45)); // Warning
            skill_prefab_temp[2].transform.localScale = new Vector3(2f, 2f, 2f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 12, skill_prefab_temp[12].transform, 1, false, false);
            }
            main.GetComponent<Camera_move>().VivrateForTime(0.2f);
        }

        for (int i = 0; i < 8; i++)
        {
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], BossStatus.instance.transform); // Warning
            skill_prefab_temp[0].transform.localScale = new Vector3(50f, 1f);
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 22.5f + i * 45);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, time, true, false);
            }
            Destroy(skill_prefab_temp[0], time);
        }
        yield return waittime;
        SoundManager.instance.Play(72);
        for (int i = 0; i < 8; i++)
        {
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], BossStatus.instance.transform.position, Quaternion.Euler(0, 0, 22.5f+i * 45)); // Warning
            skill_prefab_temp[2].transform.localScale = new Vector3(2f, 2f, 2f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, 1, false, false);
            }
            main.GetComponent<Camera_move>().VivrateForTime(0.2f);

        }

        _animator.SetBool("Magic", false);

        _animator.SetBool("MagicEnd", true);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("MagicEnd", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;

    }
    IEnumerator RotateAroundSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);

        BossStatus.instance.BossSilentAble(true);

        skill_prefab_temp[1] = Instantiate(skill_prefab[1], BossStatus.instance.transform); // Warning
        skill_prefab_temp[1].transform.localScale = new Vector3(5f, 5f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 1, skill_prefab_temp[1].transform, 1, false, true);
        }
        Destroy(skill_prefab_temp[1], 1f);
        
        yield return new WaitForSeconds(1f);

        BossStatus.instance.BossSilentAble(false);

        int num_rain = 4;//UpLoadData.boss_level + 1;
        SoundManager.instance.Play(72);

        for (int j=0;j<2;j++)
        {
            for (int i = 0; i < num_rain; i++)
            {
                skill_prefab_temp[11] = Instantiate(skill_prefab[11], BossStatus.instance.transform.position, Quaternion.Euler(0, 0, i * 90)); // Warning
                skill_prefab_temp[11].transform.localScale = new Vector3(1f, 1f, 1f);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 11, skill_prefab_temp[11].transform, 1, false, false);
                }
                main.GetComponent<Camera_move>().VivrateForTime(0.5f);
            }
            yield return new WaitForSeconds(0.5f);
        }



        _animator.SetBool("Magic", false);

        _animator.SetBool("MagicEnd", true);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("MagicEnd", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;

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
        StartCoroutine(Exec());

        boss_act_num = (int)_BossAct.Chasing;
    }

}
