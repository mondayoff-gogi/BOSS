using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Move : MonoBehaviour
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
        Attacking,  // 물공
        ClockwiseExplosion, // 마공
        SameDir,    // 물공
        OppositeDir,    // 물공
        FireBar1,       // 물공
        FireBar2,       // 물공
        FireBar3,       // 물공
        RandomExplosion,    // 마공
        Magnetic,    // 물공
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

    private int foothold_Count = 0;

    private Vector3 temp_vec;

    private List<GameObject> normal_attack;
    private int ult_count = 0;

    private void Start()
    {
        if (NetworkManager.instance.is_multi)
        {
            player_Object = (GameObject[])BossStatus.instance.player.Clone();
            target_player = player_Object[0];
        }
        else
            player_Object = (GameObject[])BossStatus.instance.player.Clone();
        //player_Object = GameObject.FindGameObjectsWithTag("Player");
        //player_Object = BossStatus.instance.PlayerObjectSort(player_Object);
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
    //        AggroSum -= 2.0f;
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


        skill_cooltime_temp[0] += Time.deltaTime; //평타
        skill_cooltime_temp[1] += Time.deltaTime; //Clockwise
        skill_cooltime_temp[2] += Time.deltaTime; //SameDir
        skill_cooltime_temp[3] += Time.deltaTime; //OppositeDir
        skill_cooltime_temp[4] += Time.deltaTime; //RandomExplosion


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
                //boss_act_num = (int)_BossAct.RandomExplosion; //테스트용으로 쓰는 코드
                //break;
                if (BossStatus.instance.MP >= BossStatus.instance.MaxMp) //자석소환! : mp max일때 소환
                {
                    BossStatus.instance.MP = 0;
                    ult_count++;
                    if (ult_count >= 2)
                    {
                        ult_count = 1;
                        BossStatus.instance.GetBuff(8);
                    }
                    boss_act_num = (int)_BossAct.Magnetic;
                    break;
                }

                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.775 && !is_HP_first) //불도저
                {
                    is_HP_first = true;
                    boss_act_num = (int)_BossAct.FireBar1;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.5 &&!is_HP_second&&!skill_prefab[13].activeSelf)
                {
                    is_HP_second = true;
                    boss_act_num = (int)_BossAct.FireBar2;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.225 && !is_HP_third && !skill_prefab[13].activeSelf/* && UpLoadData.boss_level >= 2*/)
                {
                    is_HP_third = true;
                    boss_act_num = (int)_BossAct.FireBar3;
                    break;
                }
                if (skill_cooltime_temp[4] > skill_cooltime[4]) //RandomExplosion
                {
                    skill_cooltime_temp[4] = 15-(UpLoadData.boss_level*5);
                    boss_act_num = (int)_BossAct.RandomExplosion;
                    break;
                }
                if (skill_cooltime_temp[3] > skill_cooltime[3]&&UpLoadData.boss_level>=1) //Opposite
                {
                    skill_cooltime_temp[3] = 0;
                    boss_act_num = (int)_BossAct.OppositeDir;
                    break;
                }
                /*if (skill_cooltime_temp[2] > skill_cooltime[2]&&UpLoadData.boss_level>=2) //SameDir
                {
                    skill_cooltime_temp[2] = 0;
                    boss_act_num = (int)_BossAct.SameDir;
                    break;
                }*/
                if (skill_cooltime_temp[1] > skill_cooltime[1]&& UpLoadData.boss_level >= 2) //CLOCKwise
                {
                    skill_cooltime_temp[1] = 0;
                    boss_act_num = (int)_BossAct.ClockwiseExplosion;
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
                        if (!UpLoadData.boss_usedskill_list[9][(int)_BossAct.Attacking - 1])        // 0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[9][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[10][(int)_BossAct.Attacking - 1])        // 0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[10][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[11][(int)_BossAct.Attacking - 1])        // 0번 스킬
                        {
                            UpLoadData.boss_usedskill_list[11][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[12][(int)_BossAct.Attacking - 1])        // 0번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[12][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 플레이 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[13][(int)_BossAct.Attacking - 1])        // 0번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[13][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[14][(int)_BossAct.Attacking - 1])        // 0번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[14][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[15][(int)_BossAct.Attacking - 1])        // 0번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[15][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[16][(int)_BossAct.Attacking - 1])        // 0번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[16][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(Attack_active());
                boss_act_num = (int)_BossAct.Stay;
                break;

            case (int)_BossAct.ClockwiseExplosion:
                switch (UpLoadData.boss_level)
                {
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[11][(int)_BossAct.ClockwiseExplosion + 3])        // 6번 스킬
                        {
                            UpLoadData.boss_usedskill_list[11][(int)_BossAct.ClockwiseExplosion + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[12][(int)_BossAct.ClockwiseExplosion + 3])        // 6번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[12][(int)_BossAct.ClockwiseExplosion + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    //멀티 플레이 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[13][(int)_BossAct.ClockwiseExplosion + 3])        // 6번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[13][(int)_BossAct.ClockwiseExplosion + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[14][(int)_BossAct.ClockwiseExplosion + 3])        // 6번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[14][(int)_BossAct.ClockwiseExplosion + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[15][(int)_BossAct.ClockwiseExplosion + 3])        // 6번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[15][(int)_BossAct.ClockwiseExplosion + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[16][(int)_BossAct.ClockwiseExplosion + 3])        // 6번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[16][(int)_BossAct.ClockwiseExplosion + 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(ClockwiseExplosionSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;

            case (int)_BossAct.SameDir:
                switch (UpLoadData.boss_level)
                {
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[11][(int)_BossAct.SameDir + 1])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[11][(int)_BossAct.SameDir + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[12][(int)_BossAct.SameDir + 1])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[12][(int)_BossAct.SameDir + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 플레이 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[13][(int)_BossAct.SameDir + 1])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[13][(int)_BossAct.SameDir + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[14][(int)_BossAct.SameDir + 1])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[14][(int)_BossAct.SameDir + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[15][(int)_BossAct.SameDir + 1])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[15][(int)_BossAct.SameDir + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[16][(int)_BossAct.SameDir + 1])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[16][(int)_BossAct.SameDir + 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(SameDir());
                boss_act_num = (int)_BossAct.Stay;
                break;

            case (int)_BossAct.OppositeDir:
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[10][(int)_BossAct.OppositeDir - 1])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[10][(int)_BossAct.OppositeDir - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[11][(int)_BossAct.OppositeDir])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[11][(int)_BossAct.OppositeDir] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[12][(int)_BossAct.OppositeDir])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[12][(int)_BossAct.OppositeDir] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 플레이 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[13][(int)_BossAct.OppositeDir])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[13][(int)_BossAct.OppositeDir] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[14][(int)_BossAct.OppositeDir])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[14][(int)_BossAct.OppositeDir] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[15][(int)_BossAct.OppositeDir])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[15][(int)_BossAct.OppositeDir] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[16][(int)_BossAct.OppositeDir])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[16][(int)_BossAct.OppositeDir] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(OppositeDirSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;

            case (int)_BossAct.Magnetic:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[9][(int)_BossAct.Magnetic - 5])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[9][(int)_BossAct.Magnetic - 5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[10][(int)_BossAct.Magnetic - 4])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[10][(int)_BossAct.Magnetic - 4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[11][(int)_BossAct.Magnetic - 3])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[11][(int)_BossAct.Magnetic - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[12][(int)_BossAct.Magnetic - 3])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[12][(int)_BossAct.OppositeDir] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 플레이
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[13][(int)_BossAct.Magnetic - 3])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[13][(int)_BossAct.OppositeDir] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[14][(int)_BossAct.Magnetic - 3])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[14][(int)_BossAct.OppositeDir] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[15][(int)_BossAct.Magnetic - 3])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[15][(int)_BossAct.OppositeDir] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[16][(int)_BossAct.Magnetic - 3])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[16][(int)_BossAct.OppositeDir] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(MagneticSkill());
                boss_act_num = (int)_BossAct.Stay;
                break;

            case (int)_BossAct.FireBar1:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[9][(int)_BossAct.FireBar1 - 3])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[9][(int)_BossAct.FireBar1 - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[10][(int)_BossAct.FireBar1 - 3])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[10][(int)_BossAct.FireBar1 - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[11][(int)_BossAct.FireBar1 - 3])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[11][(int)_BossAct.FireBar1 - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[12][(int)_BossAct.FireBar1 - 3])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[12][(int)_BossAct.FireBar1 - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    // 멀티 플레이
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[13][(int)_BossAct.FireBar1 - 3])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[13][(int)_BossAct.FireBar1 - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[14][(int)_BossAct.FireBar1 - 3])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[14][(int)_BossAct.FireBar1 - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[15][(int)_BossAct.FireBar1 - 3])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[15][(int)_BossAct.FireBar1 - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[16][(int)_BossAct.FireBar1 - 3])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[16][(int)_BossAct.FireBar1 - 3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(FireBarSkill(1));
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.FireBar2:
                StartCoroutine(FireBarSkill(2));
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.FireBar3:
                StartCoroutine(FireBarSkill(3));
                boss_act_num = (int)_BossAct.Stay;
                break;
            case (int)_BossAct.RandomExplosion:
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[4][(int)_BossAct.RandomExplosion - 6])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[4][(int)_BossAct.RandomExplosion - 6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[5][(int)_BossAct.RandomExplosion - 6])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[5][(int)_BossAct.RandomExplosion - 6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[6][(int)_BossAct.RandomExplosion - 6])        // 5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[6][(int)_BossAct.RandomExplosion - 6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[7][(int)_BossAct.RandomExplosion - 6])        // 5번 스킬   
                        {
                            UpLoadData.boss_usedskill_list[7][(int)_BossAct.RandomExplosion - 6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(RandomExplosion());
                boss_act_num = (int)_BossAct.Stay;
                break;
        }
    }

    // 기본 공격
    IEnumerator Attack_active()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.1f);
        Quaternion temp;
        float time_level = (float)6 / (UpLoadData.boss_level + 4);
        WaitForSeconds waiting_time = new WaitForSeconds(time_level);
        _animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);

        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
        skill_prefab_temp[0].transform.localScale = new Vector3(12f, 0.5f);
        if (target_player.transform.position.x - this.transform.position.x >= 0)
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));
        else
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((target_player.transform.position.y - this.transform.position.y) / (target_player.transform.position.x - this.transform.position.x)));

        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, time_level, true, true);
        }

        temp = skill_prefab_temp[0].transform.rotation;

        temp_vec = (target_player.transform.position - this.transform.position) * 0.1f;
        temp_vec.z = 0;
        temp_vec.Normalize();
        Destroy(skill_prefab_temp[0], time_level);
        ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
        var simulate = ps.main;
        simulate.simulationSpeed = (float)2 / time_level;
        yield return waiting_time;
        Vector3 vec2_temp;
        vec2_temp = this.transform.position;
        for (int i = 0; i < 10; i++)
        {
            skill_prefab_temp[3] = Instantiate(skill_prefab[3], vec2_temp + temp_vec * i * 1.2f, Quaternion.identity);
            Destroy(skill_prefab_temp[3], 1f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 3, skill_prefab_temp[3].transform, 1, true, false);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], vec2_temp + temp_vec * i * 1.2f, Quaternion.identity);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, 1, false, false);
            }
            normal_attack.Add(skill_prefab_temp[2]);
            main.GetComponent<Camera_move>().VivrateForTime(0.1f);
            SoundManager.instance.Play(71);
            yield return waittime;
        }
        _animator.SetBool("Attack", false);

        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("AttackEnd", false);


        boss_act_num = (int)_BossAct.Chasing;       
        yield return 0;
    }

    IEnumerator ClockwiseExplosionSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);

        BossStatus.instance.BossSilentAble(true);

        float Alarmtimer = 2f;

        skill_prefab_temp[0] = Instantiate(skill_prefab[0], new Vector2(0,0),Quaternion.identity); // Warning
        skill_prefab_temp[0].transform.localScale = new Vector3(50f, 9f);
        skill_prefab_temp[0].transform.position = new Vector2(-12, 4.5f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[0], 2f);


        skill_prefab_temp[0] = Instantiate(skill_prefab[0], new Vector2(0, 0), Quaternion.identity); // Warning
        skill_prefab_temp[0].transform.localScale = new Vector3(50f, 9f);
        skill_prefab_temp[0].transform.Rotate(0, 0, 180);
        skill_prefab_temp[0].transform.position = new Vector2(12, -4.5f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[0], 2f);

        yield return new WaitForSeconds(Alarmtimer);
        BossStatus.instance.BossSilentAble(false);

        skill_prefab_temp[4] = Instantiate(skill_prefab[4], new Vector2(-12, 4.5f), Quaternion.identity); // Warning
        skill_prefab_temp[4].transform.localScale = new Vector3(1f, 0.5f,1f);
        skill_prefab_temp[4].transform.Rotate(0, 0, 0);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 4, skill_prefab_temp[4].transform, 2, false, false);
        }
        skill_prefab_temp[4] = Instantiate(skill_prefab[4], new Vector2(12, -4.5f), Quaternion.identity); // Warning
        skill_prefab_temp[4].transform.localScale = new Vector3(1f, 0.5f,1f);
        skill_prefab_temp[4].transform.Rotate(0, 0, 180);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 4, skill_prefab_temp[4].transform, 2, false, false);
        }
        main.GetComponent<Camera_move>().VivrateForTime(0.5f);


        skill_prefab_temp[0] = Instantiate(skill_prefab[0], new Vector2(0, 0), Quaternion.identity); // Warning
        skill_prefab_temp[0].transform.localScale = new Vector3(36f, 12f);
        skill_prefab_temp[0].transform.Rotate(0, 0, -90);
        skill_prefab_temp[0].transform.position = new Vector2(6, 9f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[0], 2f);

        skill_prefab_temp[0] = Instantiate(skill_prefab[0], new Vector2(0, 0), Quaternion.identity); // Warning
        skill_prefab_temp[0].transform.localScale = new Vector3(36f, 12f);
        skill_prefab_temp[0].transform.Rotate(0, 0, 90);
        skill_prefab_temp[0].transform.position = new Vector2(-6, -9f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[0], 2f);

        yield return new WaitForSeconds(Alarmtimer);       

        skill_prefab_temp[4] = Instantiate(skill_prefab[4], new Vector2(6, 9f), Quaternion.identity); // Warning
        skill_prefab_temp[4].transform.localScale = new Vector3(1f, 0.66f,1f);
        skill_prefab_temp[4].transform.Rotate(0, 0, -90);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 4, skill_prefab_temp[4].transform, 2, false, false);
        }
        skill_prefab_temp[4] = Instantiate(skill_prefab[4], new Vector2(-6, -9f), Quaternion.identity); // Warning
        skill_prefab_temp[4].transform.localScale = new Vector3(1f, 0.66f,1f);
        skill_prefab_temp[4].transform.Rotate(0, 0, 90);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 4, skill_prefab_temp[4].transform, 2, false, false);
        }

        main.GetComponent<Camera_move>().VivrateForTime(0.5f);
        _animator.SetBool("Attack", false);



        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(1f);
        _animator.SetBool("AttackEnd", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator SameDir()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);

        GameObject[] object_temp = GameObject.FindGameObjectsWithTag("Boss2NormalAttack");
        for(int i = 0; i < object_temp.Length; i++)
        {
            skill_prefab_temp[11] = Instantiate(skill_prefab[11], object_temp[i].transform.position, Quaternion.identity);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 11, skill_prefab_temp[11].transform, 1, true, false);
            }
            Destroy(skill_prefab_temp[11], 1f);
            Destroy(object_temp[i], 0.2f);
        }
        if(NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.Boss2SameDir();
        }

        for (int i = 0; i < player_Object.Length; i++)
        {
            if(player_Object[i].tag == "Player")
            {
                skill_prefab_temp[1] = Instantiate(skill_prefab[1], player_Object[i].transform); // Warning
                skill_prefab_temp[1].transform.localScale = new Vector3(2f, 2f);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 1, skill_prefab_temp[1].transform, 2, true, false);
                }
                Destroy(skill_prefab_temp[1], 2f);
            };
        }

        // 경고
        skill_prefab_temp[22] = Instantiate(skill_prefab[22], new Vector3(-11.5f, 6.5f, 0), Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 22, skill_prefab_temp[22].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[22], 2f);
        skill_prefab_temp[22] = Instantiate(skill_prefab[22], new Vector3(-11.5f, -6.5f, 0), Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 22, skill_prefab_temp[22].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[22], 2f);
        skill_prefab_temp[22] = Instantiate(skill_prefab[22], new Vector3(-9.7f, 9.7f, 0), Quaternion.Euler(0,0,-90));
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 22, skill_prefab_temp[22].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[22], 2f);
        skill_prefab_temp[22] = Instantiate(skill_prefab[22], new Vector3(9.7f, 9.7f, 0), Quaternion.Euler(0, 0, -90));
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 22, skill_prefab_temp[22].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[22], 2f);

        yield return new WaitForSeconds(2f);

        skill_prefab_temp[23] = Instantiate(skill_prefab[23], new Vector3(-11.5f, 6.5f, 0), Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[23], 2f);
        skill_prefab_temp[23] = Instantiate(skill_prefab[23], new Vector3(-11.5f, -6.5f, 0), Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[23], 2f);
        skill_prefab_temp[24] = Instantiate(skill_prefab[24], new Vector3(-9.7f, 9.7f, 0), Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[24], 2f);
        skill_prefab_temp[24] = Instantiate(skill_prefab[24], new Vector3(9.7f, 9.7f, 0), Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[24], 2f);

        yield return new WaitForSeconds(2f);


        //둘이 붙게됨
        int t = 0;
        for (int i = 0; i < player_Object.Length; i++)
        {
            if (player_Object[i].tag == "Player")
                t++;
        }

        if( t >= 2)
        {
            for (int i = 0; i < player_Object.Length; i += 2)
            {
                player_Object[i].GetComponent<Character_Control>().is_MoveTogether = true;
                player_Object[i].GetComponent<Character_Control>().SameDirMove(player_Object[i + 1]);
                player_Object[i + 1].GetComponent<Character_Control>().is_MoveTogether = true;
                player_Object[i + 1].GetComponent<Character_Control>().SameDirMove(player_Object[i]);
            }
        }
        else
        {
        }
        Vector3 temp_1 = player_Object[0].transform.position;
        Vector3 temp_2 = player_Object[1].transform.position;

        float dis = Vector3.Distance(player_Object[0].transform.position, player_Object[1].transform.position);

        _animator.SetBool("Attack", false);

        _animator.SetBool("AttackEnd", true);
        if(t >=2)
        {
            for (int i = 0; i < player_Object.Length; i++)
            {
                skill_prefab_temp[7] = Instantiate(skill_prefab[7], player_Object[i].transform);
                Destroy(skill_prefab_temp[7], 10f);
                player_Object[i].GetComponent<Character_Control>().RunningStop();
            }
        }
        else
        {
            for(int i = 0; i < player_Object.Length; i++)
            {
                if(player_Object[i].tag == "Player")
                {
                    player_Object[i].transform.position = new Vector3(0,0, player_Object[0].transform.position.z);
                    skill_prefab_temp[7] = Instantiate(skill_prefab[7], player_Object[i].transform);
                    Destroy(skill_prefab_temp[7], 10f);
                    player_Object[i].GetComponent<Character_Control>().RunningStop();
                }
            }
        }

        /*************** 랜덤 위치 생성 ****************/
        Vector3[] foothold_position = new Vector3[11];
        foothold_position[0] = Random.insideUnitSphere * 5f;
        foothold_position[0].z = -500;

        // 오른쪽 위
        if((temp_1.x - temp_2.x) <=0 && (temp_1.y - temp_2.y <= 0))
        {
            foothold_position[10] = new Vector3(foothold_position[0].x + Mathf.Sqrt((dis * dis) - (temp_1.y - temp_2.y) * (temp_1.y - temp_2.y)), foothold_position[0].y + Mathf.Sqrt((dis * dis) - (temp_1.x - temp_2.x) * (temp_1.x - temp_2.x)), foothold_position[0].z);
        }
        // 왼쪽 위
        else if((temp_1.x - temp_2.x) > 0 && (temp_1.y - temp_2.y <= 0))
        {

            foothold_position[10] = new Vector3(foothold_position[0].x - Mathf.Sqrt((dis * dis) - (temp_1.y - temp_2.y) * (temp_1.y - temp_2.y)), foothold_position[0].y + Mathf.Sqrt((dis * dis) - (temp_1.x - temp_2.x) * (temp_1.x - temp_2.x)), foothold_position[0].z);
        }
        // 왼쪽 아래
        else if((temp_1.x - temp_2.x) > 0 && (temp_1.y - temp_2.y > 0))
        {

            foothold_position[10] = new Vector3(foothold_position[0].x - Mathf.Sqrt((dis * dis) - (temp_1.y - temp_2.y) * (temp_1.y - temp_2.y)), foothold_position[0].y - Mathf.Sqrt((dis * dis) - (temp_1.x - temp_2.x) * (temp_1.x - temp_2.x)), foothold_position[0].z);
        }
        else
        {
          
            foothold_position[10] = new Vector3(foothold_position[0].x + Mathf.Sqrt((dis * dis) - (temp_1.y - temp_2.y) * (temp_1.y - temp_2.y)), foothold_position[0].y - Mathf.Sqrt((dis * dis) - (temp_1.x - temp_2.x) * (temp_1.x - temp_2.x)), foothold_position[0].z);
        }

        //foothold_position[10] = new Vector3(foothold_position[0].x + dis, foothold_position[0].y, foothold_position[0].z);

        for (int i = 1; i < foothold_position.Length - 1; i++)
        {
            Vector3 temp = Random.insideUnitSphere * 5f;
            temp.z = -500; 
            foothold_position[i] = temp;
        }
        int tmp = Random.Range(0, 3);
        if (tmp == 0)
        {

            foothold_position[0] = new Vector3(foothold_position[0].x - 5f, foothold_position[0].y, foothold_position[0].z);
            foothold_position[10] = new Vector3(foothold_position[10].x, foothold_position[0].y + 5f, foothold_position[10].z);
        }
        // 세로
        else if (tmp == 1)
        {

            foothold_position[0] = new Vector3(foothold_position[0].x + 5f, foothold_position[0].y, foothold_position[0].z);
            foothold_position[10] = new Vector3(foothold_position[10].x, foothold_position[0].y - 5f, foothold_position[10].z);
        }
        else
        {
            foothold_position[0] = new Vector3(foothold_position[0].x + 5f, foothold_position[0].y, foothold_position[0].z);
            foothold_position[10] = new Vector3(foothold_position[10].x - 5f, foothold_position[0].y, foothold_position[10].z);
        }

        GameObject[] bombs = new GameObject[foothold_position.Length];
        ///*************** 랜덤 위치에 폭탄 생성 ****************/
        for (int i = 0; i < foothold_position.Length; i++)
        {
            main.GetComponent<Camera_move>().VivrateForTime(0.1f);
            bombs[i] = Instantiate(skill_prefab[8], foothold_position[i], Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 1; i < foothold_position.Length - 1; i++)
        {
            int temp = Random.Range(1, 5);
            if(temp == 1)
            {
                bombs[i].GetComponent<Boss2_SameDir>().Move(new Vector3(1, 0, 0));
            }
            if(temp == 2)
            {
                bombs[i].GetComponent<Boss2_SameDir>().Move(new Vector3(-1, 0, 0));
            }
            if(temp == 3)
            {
                bombs[i].GetComponent<Boss2_SameDir>().Move(new Vector3(0, 1, 0));
            }
            else
            {
                bombs[i].GetComponent<Boss2_SameDir>().Move(new Vector3(0, -1, 0));
            }
        }
        if (tmp == 0)
        {
            bombs[10].GetComponent<Boss2_SameDir>().Move(new Vector3(0, -1, 0));
            bombs[0].GetComponent<Boss2_SameDir>().Move(new Vector3(1, 0, 0));
        }
        else if (tmp == 1)
        {

            bombs[10].GetComponent<Boss2_SameDir>().Move(new Vector3(0, 1, 0));
            bombs[0].GetComponent<Boss2_SameDir>().Move(new Vector3(-1, 0, 0));
        }
        else
        {
            bombs[10].GetComponent<Boss2_SameDir>().Move(new Vector3(1, 0, 0));
            bombs[0].GetComponent<Boss2_SameDir>().Move(new Vector3(-1, 0, 0));
        }


        //StartCoroutine(FadeOut());
        yield return new WaitForSeconds(6f);
        //StartCoroutine(FadeIn());

        for (int i = 0; i < bombs.Length; i++)
        {
            if (bombs[i].GetComponent<Boss2_SameDir>().is_triggered)
            {
                foothold_Count++;
            }
        }
        
        if (foothold_Count >= 2)
        {
            _animator.SetBool("Attack", true);
            for(int i =0; i < player_Object.Length; i++)
            {
                if(player_Object[i].tag == "Player")
                {
                    skill_prefab_temp[19] = Instantiate(skill_prefab[19], player_Object[i].transform);
                    Destroy(skill_prefab_temp[19], 1f);
                }

            }
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < player_Object.Length; i++)
            {
                if (player_Object[i].tag == "Player")
                {
                    player_Object[i].GetComponent<CharacterStat>().GetDamage(0, false);
                }
            }
            main.GetComponent<Camera_move>().VivrateForTime(2f);

            yield return new WaitForSeconds(1f);
            _animator.SetBool("Attack", false);
            _animator.SetBool("AttackEnd", false);
            foothold_Count = 0;

        }
        else
        {
            _animator.SetBool("Attack", true);
            for (int i = 0; i < player_Object.Length; i++)
            {
                if (player_Object[i].tag == "Player")
                {
                    skill_prefab_temp[20] = Instantiate(skill_prefab[20], player_Object[i].transform);
                    Destroy(skill_prefab_temp[20], 1f);
                }
            }
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < player_Object.Length; i++)
            {
                if (player_Object[i].tag == "Player")
                {
                    player_Object[i].GetComponent<CharacterStat>().GetDamage(99999, false);
                }
            }
            main.GetComponent<Camera_move>().VivrateForTime(2f);

            yield return new WaitForSeconds(1f);
            _animator.SetBool("Attack", false);
            _animator.SetBool("AttackEnd", false);
            foothold_Count = 0;
        }

        for (int i = 0; i < player_Object.Length; i += 2)
        {
            player_Object[i].GetComponent<Character_Control>().is_MoveTogether = false;
            player_Object[i + 1].GetComponent<Character_Control>().is_MoveTogether = false;
        }

        yield return new WaitForSeconds(2f);
        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator OppositeDirSkill()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);

        // 경고
        skill_prefab_temp[22] = Instantiate(skill_prefab[22], new Vector3(-11.5f, 6.5f, 0), Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 22, skill_prefab_temp[22].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[22], 2f);
        skill_prefab_temp[22] = Instantiate(skill_prefab[22], new Vector3(-11.5f, -6.5f, 0), Quaternion.identity);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 22, skill_prefab_temp[22].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[22], 2f);
        skill_prefab_temp[22] = Instantiate(skill_prefab[22], new Vector3(-9.7f, 9.7f, 0), Quaternion.Euler(0, 0, -90));
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 22, skill_prefab_temp[22].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[22], 2f);
        skill_prefab_temp[22] = Instantiate(skill_prefab[22], new Vector3(9.7f, 9.7f, 0), Quaternion.Euler(0, 0, -90));
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 22, skill_prefab_temp[22].transform, 2, true, false);
        }
        Destroy(skill_prefab_temp[22], 2f);

        yield return new WaitForSeconds(2f);

        skill_prefab_temp[23] = Instantiate(skill_prefab[23], new Vector3(-11.5f, 6.5f, 0), Quaternion.identity);
        skill_prefab_temp[23].transform.localScale = new Vector3(1f, 0.25f, 1f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, false, false);
        }
        skill_prefab_temp[23] = Instantiate(skill_prefab[23], new Vector3(-11.5f, -6.5f, 0), Quaternion.identity);
        skill_prefab_temp[23].transform.localScale = new Vector3(1f, 0.25f, 1f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, false, false);
        }
        skill_prefab_temp[23] = Instantiate(skill_prefab[23], new Vector3(-9.7f, 9.7f, 0), Quaternion.Euler(0, 0, -90));
        skill_prefab_temp[23].transform.localScale = new Vector3(1f, 0.25f, 1f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, false, false);
        }
        skill_prefab_temp[23] = Instantiate(skill_prefab[23], new Vector3(9.7f, 9.7f, 0), Quaternion.Euler(0, 0, -90));
        skill_prefab_temp[23].transform.localScale = new Vector3(1f, 0.25f, 1f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, false, false);
        }
        skill_prefab_temp[23] = Instantiate(skill_prefab[23], new Vector3(11.5f, 6.5f, 0), Quaternion.Euler(0, 0, 180));
        skill_prefab_temp[23].transform.localScale = new Vector3(1f, 0.25f, 1f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, false, false);
        }
        skill_prefab_temp[23] = Instantiate(skill_prefab[23], new Vector3(11.5f, -6.5f, 0), Quaternion.Euler(0, 0, 180));
        skill_prefab_temp[23].transform.localScale = new Vector3(1f, 0.25f, 1f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, false, false);
        }
        skill_prefab_temp[23] = Instantiate(skill_prefab[23], new Vector3(-9.7f, -9.7f, 0), Quaternion.Euler(0, 0, 90));
        skill_prefab_temp[23].transform.localScale = new Vector3(1f, 0.25f, 1f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, false, false);
        }
        skill_prefab_temp[23] = Instantiate(skill_prefab[23], new Vector3(9.7f, -9.7f, 0), Quaternion.Euler(0, 0, 90));
        skill_prefab_temp[23].transform.localScale = new Vector3(1f, 0.25f, 1f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 23, skill_prefab_temp[23].transform, 2, false, false);
        }
        yield return new WaitForSeconds(3f);


        //특정위치 가운데 지정해주고
        Vector2 Cord_centor;
        float x, y;
        x = Random.Range(-3, 3);
        y = Random.Range(-3, 3);
        Cord_centor = new Vector2(x, y);

        //x y 정해서 얼마나 멀리 떨어진지 정하고
        x = Random.Range(2, 4);
        y = Random.Range(2, 4);

        Vector2 vec2_temp;
        vec2_temp.x = Cord_centor.x + x;
        vec2_temp.y = Cord_centor.y + y;
        /*
        //먼저 위치 잘 가도록 유도하고
        skill_prefab_temp[9] = Instantiate(skill_prefab[9], vec2_temp, Quaternion.identity); // Warning
        skill_prefab_temp[9].transform.localScale = new Vector3(2f, 2f);
        vec2_temp.x = Cord_centor.x - x;
        vec2_temp.y = Cord_centor.y - y;

        //먼저 위치 잘 가도록 유도하고
        skill_prefab_temp[9] = Instantiate(skill_prefab[9], vec2_temp, Quaternion.identity); // Warning
        skill_prefab_temp[9].transform.localScale = new Vector3(2f, 2f);
        
        x = Random.Range(-2, -4);
        y = Random.Range(2, 4);

        vec2_temp.x = Cord_centor.x + x;
        vec2_temp.y = Cord_centor.y + y;

        //먼저 위치 잘 가도록 유도하고
        skill_prefab_temp[25] = Instantiate(skill_prefab[25], vec2_temp, Quaternion.identity); // Warning
        skill_prefab_temp[25].transform.localScale = new Vector3(2f, 2f);

        vec2_temp.x = Cord_centor.x - x;
        vec2_temp.y = Cord_centor.y - y;

        //먼저 위치 잘 가도록 유도하고
        skill_prefab_temp[25] = Instantiate(skill_prefab[25], vec2_temp, Quaternion.identity); // Warning
        skill_prefab_temp[25].transform.localScale = new Vector3(2f, 2f);
        

        yield return new WaitForSeconds(4f);
        */
   
        ////가장먼저 서로 반대로 가게된다는 사인
        //for (int i = 0; i < player_Object.Length; i++)
        //{
        //    skill_prefab_temp[7] = Instantiate(skill_prefab[7], player_Object[i].transform); // Warning
        //    skill_prefab_temp[7].transform.localScale = new Vector3(0.1f, 0.1f);
        //    Destroy(skill_prefab_temp[7], 10f);
        //}
        ////둘이 반대로가게됨
        //for (int i = 0; i < player_Object.Length; i += 2)
        //{
        //    player_Object[i].GetComponent<Character_Control>().is_MoveTogether = true;
        //    player_Object[i].GetComponent<Character_Control>().OppositeMove(player_Object[i + 1], skill_prefab[10]);
        //    player_Object[i + 1].GetComponent<Character_Control>().is_MoveTogether = true;
        //    player_Object[i + 1].GetComponent<Character_Control>().OppositeMove(player_Object[i], skill_prefab[10]);
        //}

        vec2_temp = new Vector2(-12, 0);

        //main.GetComponent<Camera_move>().VivrateForTime(0.5f);
        //skill_prefab_temp[6] = Instantiate(skill_prefab[6], vec2_temp, Quaternion.identity); // allfire

        //yield return new WaitForSeconds(2f);

      
       
        

        //패턴생성
        for (int j=0;j<3;j++)
        {
            //------------------------원하는 위치---------------------//
            //x = Random.Range(1, 4);
            //y = Random.Range(1, 4);

            //vec2_temp.x = Cord_centor.x + x;
            //vec2_temp.y = Cord_centor.y + y;

            //skill_prefab_temp[9] = Instantiate(skill_prefab[9], vec2_temp, Quaternion.identity);
            //skill_prefab_temp[9].transform.localScale = new Vector3(2f, 2f);

            //vec2_temp.x = Cord_centor.x - x;
            //vec2_temp.y = Cord_centor.y - y;

            //skill_prefab_temp[9] = Instantiate(skill_prefab[9], vec2_temp, Quaternion.identity);
            //skill_prefab_temp[9].transform.localScale = new Vector3(2f, 2f);
            //----------------------원하는 위치-------------------------------//
            //============랜덤위치=-============================//
            for (int i = 0; i < 6; i++)
            {
                x = Random.Range(-4 + i / 10, 4);
                y = Random.Range(-4 + i / 10, 4);
                vec2_temp.x = x*2;
                vec2_temp.y = y*2;
                skill_prefab_temp[9] = Instantiate(skill_prefab[9], vec2_temp, Quaternion.identity);
                skill_prefab_temp[9].transform.localScale = new Vector3(2f, 2f);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 9, skill_prefab_temp[9].transform, 2, false, false);
                }
            }
            //============랜덤위치=-============================//

            yield return new WaitForSeconds(1.5f);

            //==========폭탄======================//
            main.GetComponent<Camera_move>().VivrateForTime(0.5f);
            vec2_temp = new Vector2(-12, 0);
            skill_prefab_temp[6] = Instantiate(skill_prefab[6], vec2_temp, Quaternion.identity); // Warning
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 6, skill_prefab_temp[6].transform, 2, false, false);
            }
            yield return new WaitForSeconds(2f);
        }


        _animator.SetBool("Attack", false);

        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(1f);
        _animator.SetBool("AttackEnd", false);

        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator MagneticSkill()
    {
        WaitForSeconds waittime;

        float time = 1f / (UpLoadData.boss_level + 5);

        waittime = new WaitForSeconds(time);
        
        _animator.SetBool("Running", false);
        
        _animator.SetBool("Attack", true);


        for(int i = 0; i < UpLoadData.boss_level*4; i++)
        {
            float x = Random.Range(-9.0f, 9.0f);
            float y = Random.Range(-8.0f, 8.0f);
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], new Vector2(x,y), Quaternion.identity);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, 2, false, false);
            }
            normal_attack.Add(skill_prefab_temp[2]);
        }

        for (int i = 0; i < normal_attack.Count; i++)
        {
            if(normal_attack[i] != null)
            {
                main.GetComponent<Camera_move>().VivrateForTime(0.2f);

                normal_attack[i].GetComponent<Boss2_NormalAttack_Move>().enabled = true;
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.Boss2Magnetic(i);
                }
                yield return waittime;
            }
        }

        normal_attack.Clear();
        _animator.SetBool("Attack", false);

        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("AttackEnd", false);
        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator FireBarSkill(int i)
    {
 		_animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);
        
        Vector2 vec2_temp;

        vec2_temp = new Vector2(0, 0);
        skill_prefab_temp[14] = Instantiate(skill_prefab[14], vec2_temp, Quaternion.identity); // 불솟음
        skill_prefab_temp[14].transform.localScale = new Vector3(2f, 2f);
        Destroy(skill_prefab_temp[14], 6f);
        yield return new WaitForSeconds(2f);
        _animator.SetBool("Attack", false);
        SoundManager.instance.Play(5);
        switch (i)
        {
            case 1:
                vec2_temp = new Vector2(11, 8);

                //skill_prefab_temp[13] = Instantiate(skill_prefab[13], vec2_temp, Quaternion.identity); // Warning
                skill_prefab[13].SetActive(true);
                skill_prefab[13].GetComponent<Boss2_Fire>().StartCor();
                skill_prefab[13].transform.position = vec2_temp;
                skill_prefab[13].GetComponent<Monster>().MonsterHpbarSetActive();
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.Boss2Fire(vec2_temp);
                }
                break;
            case 2:
                vec2_temp = new Vector2(11, -8);
                skill_prefab[13].SetActive(true);
                skill_prefab[13].GetComponent<Boss2_Fire>().StartCor();
                skill_prefab[13].transform.position = vec2_temp;
                skill_prefab[13].GetComponent<Monster>().MonsterHpbarSetActive();
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.Boss2Fire(vec2_temp);
                }
                break;
            case 3:
                vec2_temp = new Vector2(-11, -8);
                skill_prefab[13].SetActive(true);
                skill_prefab[13].GetComponent<Boss2_Fire>().StartCor();
                skill_prefab[13].transform.position = vec2_temp;
                skill_prefab[13].GetComponent<Monster>().MonsterHpbarSetActive();
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.Boss2Fire(vec2_temp);
                }
                break;
        }
        yield return new WaitForSeconds(1f);
        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("AttackEnd", false);

        _animator.SetBool("Running", true);

        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator RandomExplosion()
    {
        _animator.SetBool("Running", false);
        _animator.SetBool("Attack", true);
        WaitForSeconds waittime;
        float timer_level= 0.4f/(UpLoadData.boss_level+4);
        waittime = new WaitForSeconds(timer_level);
        int count = UpLoadData.boss_level *10 + 30;
        
        for(int i = 0; i < count; i++)
        {
            float x = Random.Range(-10.0f, 11.0f);
            float y = Random.Range(-10.0f, 11.0f);

            float scale = Random.Range(2f, 5.0f);

            skill_prefab_temp[15] = Instantiate(skill_prefab[15], new Vector2(x, y), Quaternion.identity);
            skill_prefab_temp[15].transform.localScale = new Vector3(scale, scale, scale);
            ps = skill_prefab_temp[15].GetComponent<ParticleSystem>();
            var simulate = ps.main;
            simulate.simulationSpeed = (float)1 / 4f;
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 15, skill_prefab_temp[15].transform, 3, true, false);
            }
            Destroy(skill_prefab_temp[15], 3f);
            yield return waittime;
        }
        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("AttackEnd", false);

        _animator.SetBool("Running", true);

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
    }


}
