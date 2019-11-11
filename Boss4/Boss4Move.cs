using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4Move : MonoBehaviour
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
        Attacking,
        RollingRock,
        BombTogether,
        LaserInstall,
        Meteo,
        RollingRocks,
        ShootingRock,
    }

    // 타겟 플레이어 임시 저장 변수
    [HideInInspector]
    public GameObject temp_target_player;

    public GameObject[] skill_prefab; //이거 나중에 resource로 옮겨서 소스에서 넣기

    // 플레이어1 오브젝트
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
    private ParticleSystem ps;

    private List<GameObject> normal_attack;

    private int ult_count = 0;

    public int rand_flag;

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


        skill_cooltime_temp[0] += Time.deltaTime; //평타쿨타임
        skill_cooltime_temp[1] += Time.deltaTime; //RollingRock
        skill_cooltime_temp[2] += Time.deltaTime; //RollingRocks
        skill_cooltime_temp[3] += Time.deltaTime; //LaserInstall
        skill_cooltime_temp[4] += Time.deltaTime; //ShootingRock


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
                //boss_act_num = (int)_BossAct.ShootingRock; //테스트용으로 쓰는 코드
                //break;
                if (BossStatus.instance.MP >= BossStatus.instance.MaxMp) //자석소환! : mp max일때 소환
                {
                    BossStatus.instance.MP = 0;
                    ult_count++;
                    if (ult_count >= 2)
                    {
                        BossStatus.instance.GetBuff(8);
                        ult_count = 1;
                    }
                    boss_act_num = (int)_BossAct.Meteo;
                    break;
                }

                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.775 && !is_HP_first) //불도저
                {
                    is_HP_first = true;
                    boss_act_num = (int)_BossAct.BombTogether;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.5 &&!is_HP_second&&UpLoadData.boss_level>=1)
                {
                    is_HP_second = true;
                    boss_act_num = (int)_BossAct.BombTogether;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.225 && !is_HP_third && UpLoadData.boss_level >= 2)
                {
                    is_HP_third = true;
                    boss_act_num = (int)_BossAct.BombTogether;
                    break;
                }
                if (skill_cooltime_temp[4] > skill_cooltime[4] && UpLoadData.boss_level >= 1) //shooting
                {
                    skill_cooltime_temp[4] = 0;
                    boss_act_num = (int)_BossAct.ShootingRock;
                    break;
                }
                if (skill_cooltime_temp[1] > skill_cooltime[1]&& UpLoadData.boss_level >= 1) //Roll
                {
                    skill_cooltime_temp[1] = 0;
                    boss_act_num = (int)_BossAct.RollingRock;
                    break;
                }
                if (skill_cooltime_temp[3] > skill_cooltime[3] && UpLoadData.boss_level >= 1) //Laser
                {
                    skill_cooltime_temp[3] = 0;
                    boss_act_num = (int)_BossAct.LaserInstall;
                    break;
                }
                
                if (skill_cooltime_temp[2] > skill_cooltime[2]) //Bomb
                {
                    skill_cooltime_temp[2] = 0;
                    boss_act_num = (int)_BossAct.RollingRocks;
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
                StartCoroutine(Attack_active());
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[24][(int)_BossAct.Attacking - 1])    // 0번스킬
                        {
                            UpLoadData.boss_usedskill_list[24][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[25][(int)_BossAct.Attacking - 1])   // 0번스킬
                        {
                            UpLoadData.boss_usedskill_list[25][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[26][(int)_BossAct.Attacking - 1])   // 3번스킬  
                        {
                            UpLoadData.boss_usedskill_list[26][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[27][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[27][(int)_BossAct.Attacking + -1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[28][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[28][(int)_BossAct.Attacking + -1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[29][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[29][(int)_BossAct.Attacking + -1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[30][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[30][(int)_BossAct.Attacking + -1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[31][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[31][(int)_BossAct.Attacking + -1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.RollingRock:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[25][(int)_BossAct.RollingRock + 2])   // 4번스킬
                        {
                            UpLoadData.boss_usedskill_list[25][(int)_BossAct.RollingRock + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[26][(int)_BossAct.RollingRock + 2])   // 4번스킬  
                        {
                            UpLoadData.boss_usedskill_list[26][(int)_BossAct.RollingRock + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[27][(int)_BossAct.RollingRock + 2])   // 4번스킬    
                        {
                            UpLoadData.boss_usedskill_list[27][(int)_BossAct.RollingRock + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[28][(int)_BossAct.RollingRock + 2])   // 4번스킬    
                        {
                            UpLoadData.boss_usedskill_list[28][(int)_BossAct.RollingRock + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[29][(int)_BossAct.RollingRock + 2])   // 4번스킬    
                        {
                            UpLoadData.boss_usedskill_list[29][(int)_BossAct.RollingRock + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[30][(int)_BossAct.RollingRock + 2])   // 4번스킬    
                        {
                            UpLoadData.boss_usedskill_list[30][(int)_BossAct.RollingRock + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[31][(int)_BossAct.RollingRock + 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[31][(int)_BossAct.RollingRock + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(RollingRockSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.BombTogether:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[24][(int)_BossAct.BombTogether - 1])    // 2번스킬
                        {
                            UpLoadData.boss_usedskill_list[24][(int)_BossAct.BombTogether - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[25][(int)_BossAct.BombTogether - 1])   // 2번스킬
                        {
                            UpLoadData.boss_usedskill_list[25][(int)_BossAct.BombTogether - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[26][(int)_BossAct.BombTogether - 1])   // 2번스킬  
                        {
                            UpLoadData.boss_usedskill_list[26][(int)_BossAct.BombTogether - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[27][(int)_BossAct.BombTogether - 1])   // 2번스킬    
                        {
                            UpLoadData.boss_usedskill_list[27][(int)_BossAct.BombTogether - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[28][(int)_BossAct.BombTogether - 1])   // 2번스킬    
                        {
                            UpLoadData.boss_usedskill_list[28][(int)_BossAct.BombTogether - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[29][(int)_BossAct.BombTogether - 1])   // 1번스킬    
                        {
                            UpLoadData.boss_usedskill_list[29][(int)_BossAct.BombTogether - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[30][(int)_BossAct.BombTogether - 1])   // 1번스킬    
                        {
                            UpLoadData.boss_usedskill_list[30][(int)_BossAct.BombTogether - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[31][(int)_BossAct.BombTogether - 1])   // 1번스킬    
                        {
                            UpLoadData.boss_usedskill_list[31][(int)_BossAct.BombTogether - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(BombTogether());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.LaserInstall:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[25][(int)_BossAct.LaserInstall - 1])   // 3번스킬
                        {
                            UpLoadData.boss_usedskill_list[25][(int)_BossAct.LaserInstall - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[26][(int)_BossAct.LaserInstall - 1])   // 3번스킬  
                        {
                            UpLoadData.boss_usedskill_list[26][(int)_BossAct.LaserInstall - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[27][(int)_BossAct.LaserInstall - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[27][(int)_BossAct.LaserInstall - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[28][(int)_BossAct.LaserInstall - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[28][(int)_BossAct.LaserInstall - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[29][(int)_BossAct.LaserInstall - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[29][(int)_BossAct.LaserInstall - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[30][(int)_BossAct.LaserInstall - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[30][(int)_BossAct.LaserInstall - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[31][(int)_BossAct.LaserInstall - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[31][(int)_BossAct.LaserInstall - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(LaserInstallSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
			case (int)_BossAct.Meteo:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[24][(int)_BossAct.Meteo - 2])    // 3번스킬
                        {
                            UpLoadData.boss_usedskill_list[24][(int)_BossAct.Meteo - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[25][(int)_BossAct.Meteo + 1])   // 6번스킬
                        {
                            UpLoadData.boss_usedskill_list[25][(int)_BossAct.Meteo + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[26][(int)_BossAct.Meteo + 1])   // 6번스킬  
                        {
                            UpLoadData.boss_usedskill_list[26][(int)_BossAct.Meteo + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[27][(int)_BossAct.Meteo + 1])   // 6번스킬    
                        {
                            UpLoadData.boss_usedskill_list[27][(int)_BossAct.Meteo + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[28][(int)_BossAct.Meteo + 1])   // 6번스킬    
                        {
                            UpLoadData.boss_usedskill_list[28][(int)_BossAct.Meteo + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[29][(int)_BossAct.Meteo + 1])   // 6번스킬    
                        {
                            UpLoadData.boss_usedskill_list[29][(int)_BossAct.Meteo + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[30][(int)_BossAct.Meteo + 1])   // 6번스킬    
                        {
                            UpLoadData.boss_usedskill_list[30][(int)_BossAct.Meteo + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[31][(int)_BossAct.Meteo + 1])   // 6번스킬    
                        {
                            UpLoadData.boss_usedskill_list[31][(int)_BossAct.Meteo + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(MeteoSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.RollingRocks:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[24][(int)_BossAct.RollingRocks - 5])    // 1번스킬
                        {
                            UpLoadData.boss_usedskill_list[24][(int)_BossAct.RollingRocks - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[25][(int)_BossAct.RollingRocks - 5])   // 1번스킬
                        {
                            UpLoadData.boss_usedskill_list[25][(int)_BossAct.RollingRocks - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[26][(int)_BossAct.RollingRocks - 5])   // 1번스킬  
                        {
                            UpLoadData.boss_usedskill_list[26][(int)_BossAct.RollingRocks - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[27][(int)_BossAct.RollingRocks - 5])   // 1번스킬    
                        {
                            UpLoadData.boss_usedskill_list[27][(int)_BossAct.RollingRocks - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[28][(int)_BossAct.RollingRocks - 5])   // 1번스킬    
                        {
                            UpLoadData.boss_usedskill_list[28][(int)_BossAct.RollingRocks - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[29][(int)_BossAct.RollingRocks - 5])   // 1번스킬    
                        {
                            UpLoadData.boss_usedskill_list[29][(int)_BossAct.RollingRocks - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[30][(int)_BossAct.RollingRocks - 5])   // 1번스킬    
                        {
                            UpLoadData.boss_usedskill_list[30][(int)_BossAct.RollingRocks - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[31][(int)_BossAct.RollingRocks - 5])   // 1번스킬    
                        {
                            UpLoadData.boss_usedskill_list[31][(int)_BossAct.RollingRocks - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(RollingRocksSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.ShootingRock:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[25][(int)_BossAct.ShootingRock - 2])   // 5번스킬
                        {
                            UpLoadData.boss_usedskill_list[25][(int)_BossAct.ShootingRock - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[26][(int)_BossAct.ShootingRock - 2])   // 5번스킬  
                        {
                            UpLoadData.boss_usedskill_list[26][(int)_BossAct.ShootingRock - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[27][(int)_BossAct.ShootingRock - 2])   // 5번스킬    
                        {
                            UpLoadData.boss_usedskill_list[27][(int)_BossAct.ShootingRock - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[28][(int)_BossAct.ShootingRock - 2])   // 5번스킬    
                        {
                            UpLoadData.boss_usedskill_list[28][(int)_BossAct.ShootingRock - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[29][(int)_BossAct.ShootingRock - 2])   // 5번스킬    
                        {
                            UpLoadData.boss_usedskill_list[29][(int)_BossAct.ShootingRock - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[30][(int)_BossAct.ShootingRock - 2])   // 5번스킬    
                        {
                            UpLoadData.boss_usedskill_list[30][(int)_BossAct.ShootingRock - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[31][(int)_BossAct.RollingRock + 2])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[31][(int)_BossAct.RollingRock + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(ShootingRockSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
        }
    }

    // 기본 공격
    IEnumerator Attack_active()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);

        Vector2 pos;

        pos = target_player.transform.position- this.transform.position;
        pos.Normalize();

        for(int i=0;i<3;i++)
        {
            skill_prefab_temp[1] = Instantiate(skill_prefab[1], (Vector2)this.transform.position + 2*pos*(i+1), Quaternion.identity); // Warning
            skill_prefab_temp[1].transform.localScale = 2*Vector3.one * (i+1);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 1, skill_prefab_temp[1].transform, 1, true, false);
            }
            Destroy(skill_prefab_temp[1], 1.0f);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1.0f);
        _animator.SetBool("Attack", false);

        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("AttackEnd", false);

        for (int i = 0; i < 3; i++)
        {
            skill_prefab_temp[12] = Instantiate(skill_prefab[12], (Vector2)this.transform.position + 2*pos * (i + 1), Quaternion.identity); // Warning
            skill_prefab_temp[12].transform.localScale = Vector3.one * (i+1)*0.5f;
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 12, skill_prefab_temp[12].transform, 1, true, false);
            }
            Destroy(skill_prefab_temp[12], 1.0f);
            main.GetComponent<Camera_move>().VivrateForTime(0.1f);
            SoundManager.instance.Play(7);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.0f);

        boss_act_num = (int)_BossAct.Chasing;       
        yield return 0;
    }

    IEnumerator RollingRockSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);


        float rad_x;
        rad_x = Random.Range(-9f, 9f);

        skill_prefab_temp[0] = Instantiate(skill_prefab[0], new Vector2(rad_x, 9), Quaternion.Euler(0, 0, -90)); // Warning
        skill_prefab_temp[0].transform.localScale = new Vector3(50f, 4f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[0], 2f);
        yield return new WaitForSeconds(2f);

        _animator.SetBool("Magic", false);
        skill_prefab[2].SetActive(true);
        skill_prefab[2].transform.position = new Vector2(rad_x, 9);
        main.GetComponent<Camera_move>().VivrateForTime(1f);
        skill_prefab[2].GetComponent<Monster>().MonsterHpbarSetActive();
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.Boss4RollingRock(2, new Vector2(rad_x,9));
        }

        //yield return new WaitForSeconds(1f);
        _animator.SetBool("MagicEnd", true);
        yield return new WaitForSeconds(0.1f);

        _animator.SetBool("MagicEnd", false);
        yield return new WaitForSeconds(3f);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;

    }

    IEnumerator BombTogether()      // 멀티 플레이 전용 스킬
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);

        skill_prefab_temp[3] = Instantiate(skill_prefab[3], target_player.transform);
        skill_prefab_temp[3].transform.localScale = new Vector3(1, 1, 1);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateTargetingBossSkill(UpLoadData.boss_index, 3, target_player.GetComponent<CharacterStat>().My_Index, 6, true);
        }
        Destroy(skill_prefab_temp[3], 6f);     

        yield return new WaitForSeconds(3f);
        skill_prefab_temp[3].transform.SetParent(this.transform.parent);
        yield return new WaitForSeconds(1.5f);

        Vector2 temp = skill_prefab_temp[3].transform.position;
        temp.y += 5;

        skill_prefab_temp[4] = Instantiate(skill_prefab[4], temp, Quaternion.identity);
        temp.y -= 5;
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 4, skill_prefab_temp[4].transform, 2, false, false);
        }
        skill_prefab_temp[4].GetComponent<Boss4_Bomb_Together>().GetDestination(temp);


        _animator.SetBool("Attack", false);


        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("AttackEnd", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }


    IEnumerator LaserInstallSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);

        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.01f);

        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform.position, Quaternion.identity); // Warning
        skill_prefab_temp[0].transform.localScale = new Vector3(40f, 2f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 5, true, false);
        }
        float timer = 0;

        GameObject temp_target;
        temp_target = target_player;

        Quaternion temp_quater;
        skill_prefab_temp[10] = Instantiate(skill_prefab[10], this.transform.position, Quaternion.identity); // Laser
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 10, skill_prefab_temp[10].transform, 5, true, false);
        }
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 5)
                break;

            if (temp_target.transform.position.x - this.transform.position.x > 0)
                skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((temp_target.transform.position.y - this.transform.position.y) / (temp_target.transform.position.x - this.transform.position.x)));
            else
                skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((temp_target.transform.position.y - this.transform.position.y) / (temp_target.transform.position.x - this.transform.position.x)));

            yield return waittime;
        }
        temp_quater = skill_prefab_temp[0].transform.rotation;
        Destroy(skill_prefab_temp[0]);
        Destroy(skill_prefab_temp[10]);
        
        _animator.SetBool("Attack", false);



        skill_prefab_temp[11] = Instantiate(skill_prefab[11], this.transform.position, temp_quater); // Laser
        skill_prefab_temp[11].transform.localScale = new Vector3(2f, 2f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 11, skill_prefab_temp[11].transform, 5, false, false);
        }

        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(0.1f);

        _animator.SetBool("AttackEnd", false);


        boss_act_num = (int)_BossAct.Chasing;

        yield return new WaitForSeconds(2.8f);
        main.GetComponent<Camera_move>().VivrateForTime(0.5f);

        yield return 0;
    }
	IEnumerator MeteoSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);

        int count = 5;
        if (UpLoadData.boss_level >= 2)
            count = 10;
        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(-11.0f, 11.0f);
            float y = Random.Range(-8.0f, 8.0f);

            float scale = Random.Range(2f, 5.0f);

            skill_prefab_temp[6] = Instantiate(skill_prefab[6], new Vector2(x, y), Quaternion.identity);
            skill_prefab_temp[6].transform.localScale = new Vector3(scale, scale, scale);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 6, skill_prefab_temp[6].transform, 5, false, false);
            }
            yield return new WaitForSeconds((float)3/ count);
        }

        _animator.SetBool("Attack", false);


        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(2f);
        _animator.SetBool("AttackEnd", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }
    IEnumerator RollingRocksSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);

        rand_flag = Random.Range(0, 2);
        if (rand_flag < 1)
            rand_flag = -1;

        BossStatus.instance.BossSilentAble(true);

        if(rand_flag<1)
        {
            for (int i = 0; i < 5; i++)
            {
                skill_prefab_temp[0] = Instantiate(skill_prefab[0], new Vector2(-12, 7.2f - i * 3.6f), Quaternion.Euler(0, 0, 0)); // Warning
                skill_prefab_temp[0].transform.localScale = new Vector3(50f, 3.6f);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 2, true, false);
                }
                //ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
                //var simulate = ps.main;
                //simulate.simulationSpeed = (float)1 / 2f;
                Destroy(skill_prefab_temp[0], 2f);
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                skill_prefab_temp[0] = Instantiate(skill_prefab[0], new Vector2(12, 7.2f - i * 3.6f), Quaternion.Euler(0, 0, 180)); // Warning
                skill_prefab_temp[0].transform.localScale = new Vector3(50f, 3.6f);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 2, true, false);
                }
                //ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
                //var simulate = ps.main;
                //simulate.simulationSpeed = (float)1 / 2f;
                Destroy(skill_prefab_temp[0], 2f);
            }
        }
        
        yield return new WaitForSeconds(2f);

        BossStatus.instance.BossSilentAble(false);

        _animator.SetBool("Attack", false);


        int rand_num = Random.Range(0, 5);
        if (rand_flag < 1)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == rand_num)
                {
                    skill_prefab_temp[14] = Instantiate(skill_prefab[14], new Vector2(-12, 7.2f - i * 3.6f), Quaternion.Euler(0, 0, 0)); // Warning
                    skill_prefab_temp[14].transform.localScale = new Vector3(3.6f, 3.6f, 3.6f);
                    if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                    {
                        NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 14, skill_prefab_temp[14].transform, 2, false, false);
                    }
                    continue;
                }
                skill_prefab_temp[13] = Instantiate(skill_prefab[13], new Vector2(-12, 7.2f - i * 3.6f), Quaternion.Euler(0, 0, 0)); // Warning
                skill_prefab_temp[13].transform.localScale = new Vector3(3.6f, 3.6f, 3.6f);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 13, skill_prefab_temp[13].transform, 2, false, false);
                }
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == rand_num)
                {
                    skill_prefab_temp[14] = Instantiate(skill_prefab[14], new Vector2(12, 7.2f - i * 3.6f), Quaternion.Euler(0, 0, 180)); // Warning
                    skill_prefab_temp[14].transform.localScale = new Vector3(3.6f, 3.6f, 3.6f);
                    if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                    {
                        NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 14, skill_prefab_temp[14].transform, 2, false, false);
                    }
                    continue;
                }
                skill_prefab_temp[13] = Instantiate(skill_prefab[13], new Vector2(12, 7.2f - i * 3.6f), Quaternion.Euler(0, 0, 180)); // Warning
                skill_prefab_temp[13].transform.localScale = new Vector3(3.6f, 3.6f, 3.6f);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 13, skill_prefab_temp[13].transform, 2, false, false);
                }
            }
        }
        main.GetComponent<Camera_move>().VivrateForTime(1f);


        yield return new WaitForSeconds(1f);
        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(0.1f);

        _animator.SetBool("AttackEnd", false);
        yield return new WaitForSeconds(3f);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;

    }
    IEnumerator ShootingRockSkill()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.1f);

        _animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);

        BossStatus.instance.BossSilentAble(true);

        yield return new WaitForSeconds(2f);

        BossStatus.instance.BossSilentAble(false);

        _animator.SetBool("Attack", false);

        GameObject rock_temp;

        Vector2 pos;

        int count = UpLoadData.boss_level*4+8;

        for(int i=0;i< count; i++)
        {
            pos = Random.insideUnitCircle.normalized * 15;
            rock_temp = Instantiate(skill_prefab[7], pos, Quaternion.identity);
            rock_temp.transform.localScale = Vector3.one * 1.5f;
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 7, skill_prefab_temp[7].transform, 2, false, false);
            }
            yield return waittime;
        }


        main.GetComponent<Camera_move>().VivrateForTime(1f);


        yield return new WaitForSeconds(1f);
        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(0.1f);

        _animator.SetBool("AttackEnd", false);
        yield return new WaitForSeconds(3f);


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

    }
}
