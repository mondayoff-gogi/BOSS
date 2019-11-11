using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6Move : MonoBehaviour
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
        SpinWave,   // 마공
        Tornado,    // 마공
        Gig,        // 물공
		SuckObj,    // 마공
        Volcano,    // 물공
    }

    // 타겟 플레이어 임시 저장 변수
    [HideInInspector]
    public GameObject temp_target_player;

    public GameObject[] skill_prefab; //이거 나중에 resource로 옮겨서 소스에서 넣기


    // 플레이어 오브젝트 베열
    public GameObject[] player_Object;

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

    public GameObject volcano_obj;

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


        skill_cooltime_temp[0] += Time.deltaTime; //평타
        skill_cooltime_temp[1] += Time.deltaTime; //SpinWave
        skill_cooltime_temp[2] += Time.deltaTime; //Tornado
        skill_cooltime_temp[3] += Time.deltaTime; //Gig


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
                //boss_act_num = (int)_BossAct.Attacking; //테스트용으로 쓰는 코드
                //break;
                if (BossStatus.instance.MP >= BossStatus.instance.MaxMp) //
                {
                    BossStatus.instance.MP = 0;
                    if (ult_count >= 2)
                    {
                        ult_count = 1;
                        BossStatus.instance.GetBuff(8);

                    }
                    boss_act_num = (int)_BossAct.Volcano;
                    break;
                }

                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.775 && !is_HP_first) // 빨아들이기
                {
                    is_HP_first = true;
                    boss_act_num = (int)_BossAct.SuckObj;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.5 && !is_HP_second)
                {
                    is_HP_second = true;
                    boss_act_num = (int)_BossAct.SuckObj;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.225 && !is_HP_third)
                {
                    is_HP_third = true;
                    boss_act_num = (int)_BossAct.SuckObj;
                    break;
                }
                if (skill_cooltime_temp[3] > skill_cooltime[3] && UpLoadData.boss_level >= 1) //Gig
                {
                    skill_cooltime_temp[3] = 0;
                    boss_act_num = (int)_BossAct.Gig;
                    break;
                }
                if (skill_cooltime_temp[2] > skill_cooltime[2]) //Tornado
                {
                    skill_cooltime_temp[2] = 0;
                    boss_act_num = (int)_BossAct.Tornado;
                    break;
                }
                if (skill_cooltime_temp[1] > skill_cooltime[1]&&UpLoadData.boss_level>=1) //SpinWave
                {
                    skill_cooltime_temp[1] = 0;
                    boss_act_num = (int)_BossAct.SpinWave;
                    break;
                }
                if (arrived|| skill_cooltime_temp[0]> skill_cooltime[0]) //평타
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
                        if (!UpLoadData.boss_usedskill_list[40][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[40][(int)_BossAct.Attacking - 1] = true;    //0번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[41][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[41][(int)_BossAct.Attacking - 1] = true;     // 0번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[42][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[42][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[43][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[43][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[44][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[44][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[45][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[45][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[46][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[46][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[47][(int)_BossAct.Attacking - 1])    //0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[47][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(Attack_active());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.SuckObj:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[40][(int)_BossAct.SuckObj - 2])    //3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[40][(int)_BossAct.SuckObj - 2] = true;    //3번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[41][(int)_BossAct.SuckObj])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[41][(int)_BossAct.SuckObj] = true;     // 5번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[42][(int)_BossAct.SuckObj])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[42][(int)_BossAct.SuckObj] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[43][(int)_BossAct.SuckObj])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[43][(int)_BossAct.SuckObj] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[44][(int)_BossAct.SuckObj])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[44][(int)_BossAct.SuckObj] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[45][(int)_BossAct.SuckObj])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[45][(int)_BossAct.SuckObj] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[46][(int)_BossAct.SuckObj])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[46][(int)_BossAct.SuckObj] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[47][(int)_BossAct.SuckObj])    // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[47][(int)_BossAct.SuckObj] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(SuckObjSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
                
            case (int)_BossAct.SpinWave:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[41][(int)_BossAct.SpinWave + 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[41][(int)_BossAct.SpinWave + 1] = true;     // 3번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[42][(int)_BossAct.SpinWave + 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[42][(int)_BossAct.SpinWave + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[43][(int)_BossAct.SpinWave + 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[43][(int)_BossAct.SpinWave + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[44][(int)_BossAct.SpinWave + 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[44][(int)_BossAct.SpinWave + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[45][(int)_BossAct.SpinWave + 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[45][(int)_BossAct.SpinWave + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[46][(int)_BossAct.SpinWave + 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[46][(int)_BossAct.SpinWave + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[47][(int)_BossAct.SpinWave + 1])    // 3번 스킬
                        {
                            UpLoadData.boss_usedskill_list[47][(int)_BossAct.SpinWave + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(SpinWaveSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Tornado:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[40][(int)_BossAct.Tornado - 2])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[40][(int)_BossAct.Tornado - 2] = true;     // 1번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[41][(int)_BossAct.Tornado - 2])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[41][(int)_BossAct.Tornado - 2] = true;     // 1번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[42][(int)_BossAct.Tornado - 2])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[42][(int)_BossAct.Tornado - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[43][(int)_BossAct.Tornado - 2])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[43][(int)_BossAct.Tornado - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[44][(int)_BossAct.Tornado - 2])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[44][(int)_BossAct.Tornado - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[45][(int)_BossAct.Tornado - 2])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[45][(int)_BossAct.Tornado - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[46][(int)_BossAct.Tornado - 2])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[46][(int)_BossAct.Tornado - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[47][(int)_BossAct.Tornado - 2])    // 1번 스킬
                        {
                            UpLoadData.boss_usedskill_list[47][(int)_BossAct.Tornado - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(TornadoSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.Gig:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[41][(int)_BossAct.Gig - 2])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[41][(int)_BossAct.Gig - 2] = true;     // 2번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[42][(int)_BossAct.Gig - 2])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[42][(int)_BossAct.Gig - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[43][(int)_BossAct.Gig - 2])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[43][(int)_BossAct.Gig - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[44][(int)_BossAct.Gig - 2])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[44][(int)_BossAct.Gig - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[45][(int)_BossAct.Gig - 2])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[45][(int)_BossAct.Gig - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[46][(int)_BossAct.Gig - 2])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[46][(int)_BossAct.Gig - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[47][(int)_BossAct.Gig - 2])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[47][(int)_BossAct.Gig - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(GigSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
			case (int)_BossAct.Volcano:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[40][(int)_BossAct.Volcano - 4])    // 2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[40][(int)_BossAct.Volcano - 4] = true;     // 2번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[41][(int)_BossAct.Volcano - 2])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[41][(int)_BossAct.Volcano - 2] = true;     // 4번 스킬
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[42][(int)_BossAct.Volcano - 2])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[42][(int)_BossAct.Volcano - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[43][(int)_BossAct.Volcano - 2])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[43][(int)_BossAct.Volcano - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[44][(int)_BossAct.Volcano - 2])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[44][(int)_BossAct.Volcano - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[45][(int)_BossAct.Volcano - 2])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[45][(int)_BossAct.Volcano - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[46][(int)_BossAct.Volcano - 2])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[46][(int)_BossAct.Volcano - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[47][(int)_BossAct.Volcano - 2])    // 4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[47][(int)_BossAct.Volcano - 2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(VolcanoSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;
        }
    }

    // 기본 공격
    IEnumerator Attack_active()
    {
        _animator.SetBool("Running", false);
        
        Vector2 dir;


        float timer = 1f / (UpLoadData.boss_level + 2);

        WaitForSeconds wait = new WaitForSeconds(timer);


        WaitForSeconds waittime = new WaitForSeconds(0.05f);

        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
        skill_prefab_temp[0].transform.localScale = new Vector3(40f, 2f);
        ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
        var simulate = ps.main;
        simulate.simulationSpeed = (float)1 / timer;
        if (target_player.transform.position.x - this.transform.position.x >= 0)
        {
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));
        }
        else
        {
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));
        }
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, timer, true, true);
        }
        dir = target_player.transform.position - this.transform.position;
        dir.Normalize();
        Destroy(skill_prefab_temp[0], timer);
        yield return wait;

        _animator.SetBool("Attack", true);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Attack", false);
        Vector2 vec2_temp;


        
        for (int i = 0; i < 20; i++)
        {
            yield return waittime;
            
            if (Random.Range(0, 10) > 3+ UpLoadData.boss_level*2) continue;
            vec2_temp = new Vector2(this.transform.position.x+dir.x*(float)(i+6)/2 + Random.Range(-3f,3f), this.transform.position.y + dir.y * (float)(i + 6)/ 2 + Random.Range(-3f, 3f));
            skill_prefab_temp[10] = Instantiate(skill_prefab[10], vec2_temp, Quaternion.identity);
            skill_prefab_temp[10].transform.localScale = new Vector2(4f, 4f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 10, skill_prefab_temp[10].transform, timer, false, false);
            }
            SoundManager.instance.Play(45);
        }
        yield return new WaitForSeconds(1f);
        boss_act_num = (int)_BossAct.Chasing;

        yield return 0;
    }


    IEnumerator SpinWaveSkill()
    {
        _animator.SetBool("Running", false);
        

        _animator.SetFloat("DirX",-this.transform.position.x);
        _animator.SetFloat("DirY", -this.transform.position.y);

        _animator.SetBool("Active", true);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Active", false);

        temp_target_player = target_player;
        skill_prefab_temp[2] = Instantiate(skill_prefab[2], Vector2.zero,Quaternion.identity); // Warning
        skill_prefab_temp[2].transform.localScale = Vector3.one*5;
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, 2, true, false);
        }
        yield return new WaitForSeconds(2f);
        Destroy(skill_prefab_temp[2]);

        Vector2 pos;
        float radius=3f;
        for (int i = 0; i < 12; i++)
        {
            pos = new Vector2(radius*Mathf.Cos(Mathf.Deg2Rad*(i * 30)), radius*Mathf.Sin(Mathf.Deg2Rad * (i * 30)));
            skill_prefab_temp[3] = Instantiate(skill_prefab[3], pos,Quaternion.Euler(0,0,i*30)); // Warning
            skill_prefab_temp[3].transform.localScale = new Vector3(2f, 2f,2f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 3, skill_prefab_temp[3].transform, 2, false, false);
            }
        }
        main.GetComponent<Camera_move>().VivrateForTime(1f);
        SoundManager.instance.Play(46);

        yield return new WaitForSeconds(10.1f);

        boss_act_num = (int)_BossAct.Chasing;

        yield return 0;
    }
    IEnumerator TornadoSkill()
    {
        _animator.SetBool("Running", false);
        WaitForSeconds waittime = new WaitForSeconds(0.01f);

        float time = 2 / (UpLoadData.boss_level + 1);
        WaitForSeconds wait=new WaitForSeconds(time);


        temp_target_player = target_player;
        vector3_throw_attack = temp_target_player.transform.position;
        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
        skill_prefab_temp[0].transform.localScale = new Vector3(40f, 2f);
        ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
        var simulate = ps.main;
        simulate.simulationSpeed = (float)1 / time;
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
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, time, true, false);
        }
        yield return wait;
        Destroy(skill_prefab_temp[0]);

        _animator.SetBool("Magic", true);

        skill_prefab_temp[4] = Instantiate(skill_prefab[4], this.transform.position,Quaternion.Euler(-55,0,0)); //tornado
        skill_prefab_temp[4].transform.localScale = new Vector3(2f, 2f,2f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 4, skill_prefab_temp[4].transform, 2, false, false);
        }

        main.GetComponent<Camera_move>().VivrateForTime(1f);
        SoundManager.instance.Play(43);

        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Magic", false);
        yield return new WaitForSeconds(1f);

        boss_act_num = (int)_BossAct.Chasing;

        yield return 0;
    }    
    IEnumerator GigSkill()
    {
        _animator.SetBool("Running", false);
        WaitForSeconds waittime = new WaitForSeconds(0.01f);

        float timer = 0;

        temp_target_player = target_player;
        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
        skill_prefab_temp[0].transform.localScale = new Vector3(20f, 0.5f);
        while (true)
        {
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
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 0.5f, true, true);
            }
            yield return waittime;
            timer += 0.01f;
            if (timer > 0.5f)
                break;
        }
        Destroy(skill_prefab_temp[0]);

        _animator.SetBool("Active", true);

        skill_prefab_temp[8] = Instantiate(skill_prefab[8], this.transform.position, Quaternion.identity); //tornado
        skill_prefab_temp[8].transform.localScale = new Vector3(2f, 2f, 2f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 8, skill_prefab_temp[8].transform, 2, false, false);
        }

        main.GetComponent<Camera_move>().VivrateForTime(1f);

        SoundManager.instance.Play(43);

        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Active", false);

        yield return new WaitForSeconds(1f);

        boss_act_num = (int)_BossAct.Chasing;

        yield return 0;
    }
	IEnumerator SuckObjSkill()
    {
        _animator.SetBool("Running", false);

        _animator.SetFloat("DirX", -this.transform.position.x);
        _animator.SetFloat("DirY", -this.transform.position.y);

        _animator.SetBool("Magic", true);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Magic", false);
        skill_prefab_temp[5] = Instantiate(skill_prefab[5], Vector2.zero, Quaternion.identity);
        skill_prefab_temp[6] = Instantiate(skill_prefab[6], Vector2.zero, Quaternion.identity);
        skill_prefab_temp[7] = Instantiate(skill_prefab[7], new Vector3(0, 0, -10), Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 5, skill_prefab_temp[5].transform, 2, false, false);
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 6, skill_prefab_temp[6].transform, 10, true, false);
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 7, skill_prefab_temp[7].transform, 10, true, false);
        }
        Destroy(skill_prefab_temp[6], 10f);
        Destroy(skill_prefab_temp[7], 10f);

        boss_act_num = (int)_BossAct.Chasing;

        yield return 0;
    }

	IEnumerator VolcanoSkill()
    {
        _animator.SetBool("Running", false);

        _animator.SetFloat("DirX", -this.transform.position.x);
        _animator.SetFloat("DirY", -this.transform.position.y);

        _animator.SetBool("Attack", true);

        main.GetComponent<Camera_move>().VivrateForTime(3f);

        yield return new WaitForSeconds(3f);
        skill_prefab_temp[9] = Instantiate(skill_prefab[9], volcano_obj.transform);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.Boss6Volcano();
        }
        _animator.SetBool("Attack", false);

        yield return new WaitForSeconds(3.1f);

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
