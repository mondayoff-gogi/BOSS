using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5Move : MonoBehaviour
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
        Attacking, // 물공
        Bazooka,    // 물공
        Laser,      // 마공
        Mine,       // 물공
        RotateMissile,  // 마공
        Flare,
        Rush,           // 마공
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
    private ParticleSystem ps;
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

    private int ult_count = 0;

    private Vector3 temp_vec;

    private List<GameObject> normal_attack;

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
            arrived = false;
        }

        // 도착했으면 멈추고 아니면 속도를 구함
        if (arrived)
        {
            velocity = Vector3.zero;
            
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
        skill_cooltime_temp[1] += Time.deltaTime; //bazooka
        skill_cooltime_temp[2] += Time.deltaTime; //Laser
        skill_cooltime_temp[3] += Time.deltaTime; //rotate
        skill_cooltime_temp[4] += Time.deltaTime; //Rush

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
                //boss_act_num = (int)_BossAct.Rush; //테스트용으로 쓰는 코드
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
                    boss_act_num = (int)_BossAct.Flare;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.775 && !is_HP_first ) //MINE
                {
                    is_HP_first = true;
                    boss_act_num = (int)_BossAct.Mine;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.5 &&!is_HP_second )
                {
                    is_HP_second = true;
                    boss_act_num = (int)_BossAct.Mine;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.225 && !is_HP_third && UpLoadData.boss_level >= 1)
                {
                    is_HP_third = true;
                    boss_act_num = (int)_BossAct.Mine;
                    break;
                }
                
                if (skill_cooltime_temp[3] > skill_cooltime[3] && UpLoadData.boss_level >= 1) //RotateMissile
                {
                    skill_cooltime_temp[3] = 0;
                    boss_act_num = (int)_BossAct.RotateMissile;
                    break;
                }
                if (skill_cooltime_temp[2] > skill_cooltime[2]&&UpLoadData.boss_level>=1) //Laser
                {
                    skill_cooltime_temp[2] = 0;
                    boss_act_num = (int)_BossAct.Laser;
                    break;
                }
                if (skill_cooltime_temp[4] > skill_cooltime[4]) //Rush
                {
                    skill_cooltime_temp[4] = 0;
                    boss_act_num = (int)_BossAct.Rush;
                    break;
                }
                if (skill_cooltime_temp[1] > skill_cooltime[1] && UpLoadData.boss_level >= 1) //bazooka
                {
                    skill_cooltime_temp[1] = 0;
                    boss_act_num = (int)_BossAct.Bazooka;
                    break;
                }
                if (skill_cooltime_temp[0] > skill_cooltime[0]) //평타
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
                        if (!UpLoadData.boss_usedskill_list[32][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[32][(int)_BossAct.Attacking - 1] = true;    //0번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[33][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[33][(int)_BossAct.Attacking - 1] = true;     // 0번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[34][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[34][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[35][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[35][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[36][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[36][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[37][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[37][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[38][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[38][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[39][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[39][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(Attack_active());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Bazooka:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[33][(int)_BossAct.Bazooka + 2])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[33][(int)_BossAct.Bazooka + 2] = true;     // 4번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[34][(int)_BossAct.Bazooka + 2])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[34][(int)_BossAct.Bazooka + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[35][(int)_BossAct.Bazooka + 2])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[35][(int)_BossAct.Bazooka + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[36][(int)_BossAct.Bazooka + 2])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[36][(int)_BossAct.Bazooka + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[37][(int)_BossAct.Bazooka + 2])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[37][(int)_BossAct.Bazooka + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[38][(int)_BossAct.Bazooka + 2])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[38][(int)_BossAct.Bazooka + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[39][(int)_BossAct.Bazooka + 2])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[39][(int)_BossAct.Bazooka + 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(BazookaSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Laser:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[33][(int)_BossAct.Laser])    //3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[33][(int)_BossAct.Laser] = true;     // 3번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[34][(int)_BossAct.Laser])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[34][(int)_BossAct.Laser] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3: 
                        if (!UpLoadData.boss_usedskill_list[35][(int)_BossAct.Laser])    //3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[35][(int)_BossAct.Laser] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[36][(int)_BossAct.Laser])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[36][(int)_BossAct.Laser] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[37][(int)_BossAct.Laser])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[37][(int)_BossAct.Laser] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[38][(int)_BossAct.Laser])    //3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[38][(int)_BossAct.Laser] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[39][(int)_BossAct.Laser])    //3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[39][(int)_BossAct.Laser] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(LaserSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.RotateMissile:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[33][(int)_BossAct.RotateMissile - 3])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[33][(int)_BossAct.RotateMissile - 3] = true;     // 2번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[34][(int)_BossAct.RotateMissile - 3])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[34][(int)_BossAct.Laser] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[35][(int)_BossAct.RotateMissile - 3])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[35][(int)_BossAct.RotateMissile - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[36][(int)_BossAct.RotateMissile - 3])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[36][(int)_BossAct.RotateMissile - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[37][(int)_BossAct.RotateMissile - 3])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[37][(int)_BossAct.RotateMissile - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[38][(int)_BossAct.RotateMissile - 3])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[38][(int)_BossAct.RotateMissile - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[39][(int)_BossAct.RotateMissile - 3])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[39][(int)_BossAct.RotateMissile - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(RotateMissile());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Mine:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[33][(int)_BossAct.RotateMissile - 3])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[33][(int)_BossAct.RotateMissile - 3] = true;     // 2번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[34][(int)_BossAct.RotateMissile - 3])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[34][(int)_BossAct.Laser] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[35][(int)_BossAct.RotateMissile - 3])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[35][(int)_BossAct.RotateMissile - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[36][(int)_BossAct.RotateMissile - 3])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[36][(int)_BossAct.RotateMissile - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[37][(int)_BossAct.RotateMissile - 3])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[37][(int)_BossAct.RotateMissile - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[38][(int)_BossAct.RotateMissile - 3])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[38][(int)_BossAct.RotateMissile - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[39][(int)_BossAct.RotateMissile - 3])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[39][(int)_BossAct.RotateMissile - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(Mine());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Rush:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[32][(int)_BossAct.Rush - 5])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[32][(int)_BossAct.Rush - 5] = true;     // 1번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[33][(int)_BossAct.Rush - 5])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[33][(int)_BossAct.Rush - 5] = true;     // 1번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[34][(int)_BossAct.Rush - 5])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[34][(int)_BossAct.Rush - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[35][(int)_BossAct.Rush - 5])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[35][(int)_BossAct.Rush - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[36][(int)_BossAct.Rush - 5])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[36][(int)_BossAct.Rush - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[37][(int)_BossAct.Rush - 5])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[37][(int)_BossAct.Rush - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[38][(int)_BossAct.Rush - 5])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[38][(int)_BossAct.Rush - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[39][(int)_BossAct.Rush - 5])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[39][(int)_BossAct.Rush - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(RushSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
 			case (int)_BossAct.Flare:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[32][(int)_BossAct.Flare - 3])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[32][(int)_BossAct.Flare - 3] = true;     // 3번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[33][(int)_BossAct.Flare - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[33][(int)_BossAct.Flare - 1] = true;     // 5번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[34][(int)_BossAct.Flare - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[34][(int)_BossAct.Flare - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[35][(int)_BossAct.Flare - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[35][(int)_BossAct.Flare - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[36][(int)_BossAct.Flare - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[36][(int)_BossAct.Flare - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[37][(int)_BossAct.Flare - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[37][(int)_BossAct.Flare - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[38][(int)_BossAct.Flare - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[38][(int)_BossAct.Flare - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[39][(int)_BossAct.Flare - 1])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[39][(int)_BossAct.Flare - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(Flare());
                boss_act_num = (int)_BossAct.Stay;
                break;
        }
    }

    // 기본 공격
    IEnumerator Attack_active()
    {
        float time = 1.5f / (UpLoadData.boss_level + 1.5f);
        WaitForSeconds level_wait_time = new WaitForSeconds(time);
        float Target_rot;
        WaitForSeconds waittime = new WaitForSeconds(0.01f);

        skill_prefab_temp[17] = Instantiate(skill_prefab[17], this.transform); // Warning
        ps = skill_prefab_temp[17].GetComponent<ParticleSystem>();
        var simulate = ps.main;
        simulate.simulationSpeed = (float)1 / 1.5f;
        skill_prefab_temp[17].transform.localScale = new Vector3(12f, 4f);
        if (target_player.transform.position.x - this.transform.position.x >= 0)
        {
            skill_prefab_temp[17].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));
            Target_rot = Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x));
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 17, skill_prefab_temp[17].transform, time, true, true);
            }
        }
        else
        {
            skill_prefab_temp[17].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));
            Target_rot = 180 + Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x));
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 17, skill_prefab_temp[17].transform, time, true, true);
            }
        }

        Destroy(skill_prefab_temp[17], time);
        /*
        float max = 999;
        int index = -1;
        Vector2 ani_dir;
        ani_dir.x = _animator.GetFloat("DirX");
        ani_dir.y = _animator.GetFloat("DirY");

        if (max > Vector2.Distance(ani_dir, new Vector2(1, 0)))
        {
            max = Vector2.Distance(ani_dir, new Vector2(1, 0));
            index = 0;
        }
        if (max > Vector2.Distance(ani_dir, new Vector2(1, 1)))
        {
            max = Vector2.Distance(ani_dir, new Vector2(1, 1));
            index = 1;
        }
        if (max > Vector2.Distance(ani_dir, new Vector2(0, 1)))
        {
            max = Vector2.Distance(ani_dir, new Vector2(0, 1));
            index = 2;
        }
        if (max > Vector2.Distance(ani_dir, new Vector2(-1, 1)))
        {
            max = Vector2.Distance(ani_dir, new Vector2(-1, 1));
            index = 3;
        }
        if (max > Vector2.Distance(ani_dir, new Vector2(-1, 0)))
        {
            max = Vector2.Distance(ani_dir, new Vector2(-1, 0));
            index = 4;
        }
        if (max > Vector2.Distance(ani_dir, new Vector2(-1, -1)))
        {
            max = Vector2.Distance(ani_dir, new Vector2(-1, -1));
            index = 5;
        }
        if (max > Vector2.Distance(ani_dir, new Vector2(0, -1)))
        {
            max = Vector2.Distance(ani_dir, new Vector2(0, -1));
            index = 6;
        }
        if (max > Vector2.Distance(ani_dir, new Vector2(1, -1)))
        {
            max = Vector2.Distance(ani_dir, new Vector2(1, -1));
            index = 7;
        }

        Vector2 pos = new Vector2(0, 0);

        switch (index)
        {
            case 0://오른쪽부터 시계반대로시작
                pos = this.transform.position;
                pos.x += 3.9f;
                pos.y += 0.3f;
                break;
            case 1:
                pos = this.transform.position;
                pos.x += 3.2f;
                pos.y += 2f;
                break;
            case 2:
                pos = this.transform.position;
                pos.x += 0.6f;
                pos.y += 3f;
                break;
            case 3:
                pos = this.transform.position;
                pos.x += -2.4f;
                pos.y += 2.6f;
                break;
            case 4:
                pos = this.transform.position;
                pos.x += -3.9f;
                pos.y += 1f;
                break;
            case 5:
                pos = this.transform.position;
                pos.x += -3.2f;
                pos.y += -0.6f;
                break;
            case 6:
                pos = this.transform.position;
                pos.x += -0.5f;
                pos.y += -1.5f;
                break;
            case 7:
                pos = this.transform.position;
                pos.x += 2.3f;
                pos.y += -1.1f;
                break;
        }
        */
        yield return level_wait_time;

        main.GetComponent<Camera_move>().VivrateForTime(1f);
        _animator.SetBool("NormalAttack", true);
        int sound_index = 0;
        for (int i=0;i<100;i++)
        {
            if (Random.Range(0, 10) > 3) continue;
            skill_prefab_temp[3] = Instantiate(skill_prefab[3], this.transform.position , Quaternion.identity);
            skill_prefab_temp[3].transform.Rotate(0,0, Target_rot+ Random.Range(-20+UpLoadData.boss_level*8, 20 + UpLoadData.boss_level * 8));
            skill_prefab_temp[3].transform.localScale = Vector3.one * 2f;
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 3, skill_prefab_temp[3].transform, time, false, false);
            }
            if (sound_index++ % 3 == 0)
                SoundManager.instance.Play(42);
            yield return waittime;            
        }
        _animator.SetBool("NormalAttack", false);
        yield return new WaitForSeconds(1f);

        boss_act_num = (int)_BossAct.Chasing;
       
        yield return 0;
    }


    IEnumerator BazookaSkill()
    {
        _animator.SetBool("Magic", true);
        float timer=0f;
        WaitForSeconds waittime = new WaitForSeconds(0.01f);

        temp_target_player = target_player;
        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
        skill_prefab_temp[0].transform.localScale = new Vector3(12f, 0.5f);
        while (true)
        {
            _animator.SetFloat("DirX", temp_target_player.transform.position.x - this.transform.position.x);
            _animator.SetFloat("DirY", temp_target_player.transform.position.y - this.transform.position.y);

            if (temp_target_player.transform.position.x - this.transform.position.x > 0)
            {
                skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((temp_target_player.transform.position.y - this.transform.position.y) / (temp_target_player.transform.position.x - this.transform.position.x)));
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, false);
                }
            }
            else
            {
                skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((temp_target_player.transform.position.y - this.transform.position.y) / (temp_target_player.transform.position.x - this.transform.position.x)));
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, false);
                }
            }
            yield return waittime;
            timer += 0.01f;
            if (timer > 1.5f)
                break;
        }
        Destroy(skill_prefab_temp[0]);
        _animator.SetBool("Magic", false);
        skill_prefab_temp[4] = Instantiate(skill_prefab[4], this.transform);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 4, skill_prefab_temp[4].transform, 1.5f, false, true);
        }
        SoundManager.instance.Play(43);

        yield return new WaitForSeconds(1f);

        boss_act_num = (int)_BossAct.Chasing;

        yield return 0;
    }
    IEnumerator LaserSkill()
    {
        
        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
        skill_prefab_temp[0].transform.position = new Vector2(-20, Random.Range(-4, 4));
        skill_prefab_temp[0].transform.localScale = new Vector3(40f, 5f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 2f, true, true);
        }
        Vector2 skill_pos;
        skill_pos = skill_prefab_temp[0].transform.position;
        Destroy(skill_prefab_temp[0],2f);

        _animator.SetFloat("DirX", skill_prefab_temp[0].transform.position.x-this.transform.position.x);
        _animator.SetFloat("DirY", skill_prefab_temp[0].transform.position.y - this.transform.position.y);
        _animator.SetBool("Active", true);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Active", false);
        yield return new WaitForSeconds(2f);


        main.GetComponent<Camera_move>().VivrateForTime(1f);
        SoundManager.instance.Play(44);
        skill_prefab_temp[6] = Instantiate(skill_prefab[6], skill_pos, Quaternion.identity);

        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 6, skill_prefab_temp[6].transform, 2f, false, true);
        }
        boss_act_num = (int)_BossAct.Chasing;

        yield return 0;
    }

	IEnumerator RotateMissile()
    {
        GameObject temp = target_player;
        if (this.GetComponentInChildren<Boss5_Flare>() != null)
        {
            skill_prefab_temp[11] = Instantiate(skill_prefab[11], temp.transform); // Warning
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateTargetingBossSkill(UpLoadData.boss_index, 11, temp.GetComponent<CharacterStat>().My_Index, 2f, true);
            }
            Destroy(skill_prefab_temp[11], 2f);
        }
        else
        {
            skill_prefab_temp[16] = Instantiate(skill_prefab[16], temp.transform); // Warning
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateTargetingBossSkill(UpLoadData.boss_index, 16, temp.GetComponent<CharacterStat>().My_Index, 2f, true);
            }
            Destroy(skill_prefab_temp[16], 2f);
        }
        int count = UpLoadData.boss_level * 2 + 3;

         for (int i = 0; i < count; i++)
        {
            _animator.SetBool("Attack", true);

            skill_prefab_temp[7] = Instantiate(skill_prefab[7], this.transform.position, Quaternion.identity);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 17, skill_prefab_temp[7].transform, 2f, false, false);
            }
            skill_prefab_temp[7].GetComponent<Boss5_RotateMissile>().GetTargetPosition(temp);
            yield return new WaitForSeconds(0.1f);
            _animator.SetBool("Attack", false);
            yield return new WaitForSeconds(0.9f);

        }
        yield return new WaitForSeconds(1f);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;

    }
	IEnumerator Mine()
    {
        _animator.SetBool("Active", true);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Active", false);

        int count = UpLoadData.boss_level * 3 + 5;
       
        if(this.GetComponentInChildren<Boss5_Flare>() != null)
        {
            for (int i = 0; i < count; i++)
            {
                float x = Random.Range(-9.0f, 9.0f);
                float y = Random.Range(-8.0f, 8.0f);
                yield return new WaitForSeconds(0.1f);
                skill_prefab_temp[5] = Instantiate(skill_prefab[5], new Vector3(x, y, -10), Quaternion.identity);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 5, skill_prefab_temp[5].transform, 2f, false, false);
                }
                SoundManager.instance.Play(50);

                skill_prefab_temp[5].GetComponent<SpriteRenderer>().enabled = false;
            }            
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                float x = Random.Range(-9.0f, 9.0f);
                float y = Random.Range(-8.0f, 8.0f);

                skill_prefab_temp[1] = Instantiate(skill_prefab[1], new Vector2(x, y), Quaternion.identity);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 1, skill_prefab_temp[1].transform, 1f, true, false);
                }
                Destroy(skill_prefab_temp[1], 1f);
                yield return new WaitForSeconds(0.1f);
                skill_prefab_temp[5] = Instantiate(skill_prefab[5], new Vector3(x, y, -10), Quaternion.identity);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 5, skill_prefab_temp[5].transform, 2f, false, false);
                }
                SoundManager.instance.Play(50);

            }
        }

        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }
  	IEnumerator RushSkill()
    {

        Vector2 Rush_pos;
        WaitForSeconds waittime = new WaitForSeconds(0.01f);

        temp_target_player = target_player;

        for (int j = 0; j < 3; j++)
        {
            _animator.SetFloat("DirX",temp_target_player.transform.position.x- this.transform.position.x);
            _animator.SetFloat("DirY", temp_target_player.transform.position.y-this.transform.position.y);

            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = new Vector3(7.5f, 1f);
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            var simulate = ps.main;
            simulate.simulationSpeed = (float)1 / 1.5f;
            BossStatus.instance.BossSilentAble(true, skill_prefab_temp[0]);
            if (temp_target_player.transform.position.x - this.transform.position.x > 0)
            {
                skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((temp_target_player.transform.position.y - this.transform.position.y) / (temp_target_player.transform.position.x - this.transform.position.x)));
            }
            else
            {
                skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((temp_target_player.transform.position.y - this.transform.position.y) / (temp_target_player.transform.position.x - this.transform.position.x)));
            }
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, false);
            }
            Rush_pos = temp_target_player.transform.position - this.transform.position;
            Rush_pos.Normalize();

            Destroy(skill_prefab_temp[0], 1.5f);
            yield return new WaitForSeconds(1.5f);
            BossStatus.instance.BossSilentAble(false);

            skill_prefab_temp[13] = Instantiate(skill_prefab[13], this.transform); // Rush
            skill_prefab_temp[15] = Instantiate(skill_prefab[15], this.transform); // Trail
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 13, skill_prefab_temp[13].transform, 3f, true, true);
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 15, skill_prefab_temp[15].transform, 3f, true, true);

            }
            _animator.SetBool("Running", true);

            float RunSpeed = 30f;
            SoundManager.instance.Play(43);
            while (RunSpeed > 3f)
            {
                RunSpeed -= Time.deltaTime * 60f;

                if (this.transform.position.x > 9 || this.transform.position.x < -9 || this.transform.position.y > 6f || this.transform.position.y < -6f)
                {
                    RunSpeed *= -1;
                    this.transform.Translate(Rush_pos * 2 * RunSpeed * Time.deltaTime);
                    break;
                }
                else
                    this.transform.Translate(Rush_pos * RunSpeed * Time.deltaTime);
                yield return waittime;
            }
            Destroy(skill_prefab_temp[13]);
            Destroy(skill_prefab_temp[15]);
            main.GetComponent<Camera_move>().VivrateForTime(1f);
            SoundManager.instance.Play(28);

            skill_prefab_temp[14] = Instantiate(skill_prefab[14], this.transform); // Effect
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 14, skill_prefab_temp[14].transform, 3f, false, true);

            }
            _animator.SetBool("Running", false);

        }

        yield return new WaitForSeconds(0.1f);


        boss_act_num = (int)_BossAct.Chasing;

        yield return 0;
    }
	IEnumerator Flare()
    {
        _animator.SetBool("Active", true);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Active", false);

        skill_prefab_temp[8] = Instantiate(skill_prefab[8], this.transform.position, Quaternion.identity);
        Destroy(skill_prefab_temp[8], 1f);
        yield return new WaitForSeconds(0.5f);

        skill_prefab_temp[9] = Instantiate(skill_prefab[9], this.transform);

        yield return new WaitForSeconds(1f);
        for(int i = 0; i < player_Object.Length; i++)
        {
            if(player_Object[i].tag == "Player")
            {
                skill_prefab_temp[10] = Instantiate(skill_prefab[10], player_Object[i].transform);
                skill_prefab_temp[10].GetComponent<Boss5_Flare_Light>().GetTartget(player_Object[i]);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateTargetingBossSkill(UpLoadData.boss_index, 10, player_Object[i].GetComponent<CharacterStat>().My_Index, 3f, false);
                }
            }
        }

        yield return new WaitForSeconds(5f);
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

}
