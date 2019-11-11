﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertBossMove_koki : MonoBehaviour
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
        CrossAttack,
        RotateAttack,
        Spread,
        VerticalExplosion,
        Hp_Spread1,
        Hp_Spread2,
        Hp_Spread3,
        Pillar,
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

    [HideInInspector]
    public float Stunning_time = 0;
    [HideInInspector]
    public bool is_stunned = false;

    // 스킬 캐스팅 중인지 확인하는 flag
    private bool is_skill_casting;

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
    const float StoppingDistance = 2+.0f;
    const float Skill_1_Distnace = 6.0f;
    const float Skill_2_Distnace = 3.0f;
    const float Skill_Around_Distnace = 5.0f;
    const float skill_throw_speed = 0.1f;

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


    private Vector3 pos_memo; //pos기억용

    private Vector3 vector3_temp; //스킬 활용할때 쓰는 임시용 vector

    public Vector3 vector3_throw_attack;

    // 현재 이동 속도
    private Vector3 velocity = Vector3.zero;   

    // 목적지
    private Vector3 destination;

    public Camera main;

    private int ult_count = 0;


    private void Start()
    {
        if (NetworkManager.instance.is_multi)
        {
            player_Object = (GameObject[])BossStatus.instance.player.Clone();
            //target_player = player_Object[0];
        }
        else
            player_Object = (GameObject[])BossStatus.instance.player.Clone();
        //player_Object = BossStatus.instance.PlayerObjectSort(player_Object);
        //InitializeAggro();
        skill_prefab_temp = new GameObject[skill_prefab.Length];
        skill_cooltime_temp = new float[skill_cooltime.Length];
        walkSpeed = BossStatus.instance.moveSpeed;
        for (int i = 0; i < skill_cooltime.Length; i++)
        {
            skill_cooltime_temp[i] = 0;
        }

        is_skill_casting = false;
        _animator = this.GetComponent<Animator>();
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        StartCoroutine(Exec());
    }

    private void Update()
    {
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            BossAct(boss_act_num);
        }
        else if(NetworkManager.instance.is_multi && !NetworkManager.instance.is_host)
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
            if(target_player != temp)
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
        if(GameManage.instance.IsGameEnd)
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

    //// 최초 어그로 초기화.
    //public void InitializeAggro()
    //{
    //    for (int i = 0; i < player_Object.Length; i++)
    //    {
    //        AggroSum = 0;
    //    }
    //}

    //public void DecreaseAggro()
    //{
    //    for (int i = 0; i < (int)_PLAYER.PLAYER_NUM; i++)
    //    {
    //        AggroSum -= 2.0f;
    //    }
    //}

    IEnumerator DecreasePlayerMovementSpeed()
    {
        yield return new WaitForSeconds(3f);
        for(int i=0;i< player_Object.Length;i++)
        {
                player_Object[i].GetComponent<Character_Control>().move_speed *= 0.8f; //속도 0.8배 됨
        }
    }

    private void BossAct(int act_num)
    {
        BossStatus.instance.MP += Time.deltaTime;
        if(BossStatus.instance.MP> BossStatus.instance.MaxMp)
        {
            BossStatus.instance.MP = BossStatus.instance.MaxMp;
        }
        if(BossStatus.instance.HP <=0)
        {
            act_num = (int)_BossAct.Stay;
        }
        
        skill_cooltime_temp[0] += Time.deltaTime; //Cross
        skill_cooltime_temp[1] += Time.deltaTime; //모래늪 쿨타임
        skill_cooltime_temp[2] += Time.deltaTime; //원형베기 쿨타임
        skill_cooltime_temp[3] += Time.deltaTime; //모래바람 쿨타임
        skill_cooltime_temp[4] += Time.deltaTime; //평타


        SelectTarget();

        switch (act_num)
        {
            
            case (int)_BossAct.Stay:
                if(BossStatus.instance.Silent)
                {
                    BossGetSilent();
                    boss_act_num = (int)_BossAct.Chasing;
                    BossStatus.instance.BossSilentAble(false);
                    BossStatus.instance.Silent = false;
                }                
                break;
            case (int)_BossAct.Chasing:
                if (MovetoTarget()) return;
                //boss_act_num = (int)_BossAct.VerticalExplosion; //테스트용으로 쓰는 코드
                //break;
                if (BossStatus.instance.MP >= BossStatus.instance.MaxMp) //황금 기둥소환! : mp 100이상일때 소환
                {
                    BossStatus.instance.MP = 0;
                    ult_count++;
                    if(ult_count >= 2)
                    {
                        BossStatus.instance.GetBuff(8);
                        ult_count = 1;
                    }
                    if(UpLoadData.boss_level>=2)
                        boss_act_num = (int)_BossAct.Pillar;
                    else
                        boss_act_num = (int)_BossAct.VerticalExplosion;


                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.775&&!is_HP_first) //몬스터 소환 : 보스 체력이 낮아질수록 많이 생성    보스의 체력 25, 50 , 75% 일때  생성
                {
                    is_HP_first = true;
                    boss_act_num = (int)_BossAct.Hp_Spread1;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.5&&!is_HP_second) //몬스터 소환 : 보스 체력이 낮아질수록 많이 생성    보스의 체력 25, 50 , 75% 일때  생성
                {
                    is_HP_second = true;
                    boss_act_num = (int)_BossAct.Hp_Spread2;
                    break;
                }
                if (BossStatus.instance.HP / BossStatus.instance.MaxHP < 0.225&&!is_HP_third&&UpLoadData.boss_level>=3) //몬스터 소환 : 보스 체력이 낮아질수록 많이 생성    보스의 체력 25, 50 , 75% 일때  생성
                {
                    is_HP_third = true;
                    boss_act_num = (int)_BossAct.Hp_Spread3;
                    break;
                }

                //장판깔기
                if (skill_cooltime_temp[2] > skill_cooltime[2]&&UpLoadData.boss_level>=2) //spread
                {
                    skill_cooltime_temp[2] = 0;
                    boss_act_num = (int)_BossAct.Spread;
                    break;
                }
                if (skill_cooltime_temp[0] > skill_cooltime[0]) //Cross
                {
                    skill_cooltime_temp[0] = 0;
                    boss_act_num = (int)_BossAct.CrossAttack;
                    break;
                }

                if(skill_cooltime_temp[1]>skill_cooltime[1] && UpLoadData.boss_level >= 1) //rotate
                {
                    skill_cooltime_temp[1] = 0;
                    boss_act_num = (int)_BossAct.RotateAttack;
                    break;
                }

                //폭탄
                if (skill_cooltime_temp[3] > skill_cooltime[3] && UpLoadData.boss_level >= 2)
                {
                    skill_cooltime_temp[3] = 0;
                    boss_act_num = (int)_BossAct.VerticalExplosion;
                    break;
                }                
                //일반 공격                
                if (arrived || skill_cooltime_temp[4]> skill_cooltime[4])
                {
                    skill_cooltime_temp[4] = 0;
                    boss_act_num = (int)_BossAct.Attacking;
                    break;
                }
                break;
            case (int)_BossAct.Attacking:                                           // 0번스킬
                boss_act_num = (int)_BossAct.Stay;
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[0][(int)_BossAct.Attacking - 1])
                        {
                            UpLoadData.boss_usedskill_list[0][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }  
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[1][(int)_BossAct.Attacking - 1])   // 1번스킬
                        {
                            UpLoadData.boss_usedskill_list[1][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[2][(int)_BossAct.Attacking - 1])   // 3번스킬  
                        {
                            UpLoadData.boss_usedskill_list[2][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[3][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[3][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[4][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[4][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[5][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[5][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[6][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[6][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[7][(int)_BossAct.Attacking - 1])   // 3번스킬    
                        {
                            UpLoadData.boss_usedskill_list[7][(int)_BossAct.Attacking - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(Attack_active());
                break;

            case (int)_BossAct.Spread:
                boss_act_num = (int)_BossAct.Stay;
                switch (UpLoadData.boss_level)
                {
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[2][4])   // 3번스킬   
                        {
                            UpLoadData.boss_usedskill_list[2][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[3][4])   // 3번스킬   
                        {
                            UpLoadData.boss_usedskill_list[3][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[4][4])   // 3번스킬   
                        {
                            UpLoadData.boss_usedskill_list[4][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[5][4])   // 3번스킬   
                        {
                            UpLoadData.boss_usedskill_list[5][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[6][4])   // 3번스킬   
                        {
                            UpLoadData.boss_usedskill_list[6][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[7][4])   // 3번스킬   
                        {
                            UpLoadData.boss_usedskill_list[7][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }       
                StartCoroutine(SpreadSkill());
                break;

            case (int)_BossAct.CrossAttack:
                boss_act_num = (int)_BossAct.Stay;
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[0][(int)_BossAct.CrossAttack - 1])   // 1번스킬
                        {
                            UpLoadData.boss_usedskill_list[0][(int)_BossAct.CrossAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[1][(int)_BossAct.CrossAttack - 1])   // 1번스킬
                        {
                            UpLoadData.boss_usedskill_list[1][(int)_BossAct.CrossAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[2][(int)_BossAct.CrossAttack - 1])   // 3번스킬      
                        {
                            UpLoadData.boss_usedskill_list[2][(int)_BossAct.CrossAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[3][(int)_BossAct.CrossAttack - 1])   // 3번스킬
                        {
                            UpLoadData.boss_usedskill_list[3][(int)_BossAct.CrossAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[4][(int)_BossAct.CrossAttack - 1])   // 3번스킬
                        {
                            UpLoadData.boss_usedskill_list[4][(int)_BossAct.CrossAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[5][(int)_BossAct.CrossAttack - 1])   // 3번스킬
                        {
                            UpLoadData.boss_usedskill_list[5][(int)_BossAct.CrossAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[6][(int)_BossAct.CrossAttack - 1])   // 3번스킬
                        {
                            UpLoadData.boss_usedskill_list[6][(int)_BossAct.CrossAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[7][(int)_BossAct.CrossAttack - 1])   // 3번스킬
                        {
                            UpLoadData.boss_usedskill_list[7][(int)_BossAct.CrossAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(CrossAttackSkill());
                break;

            case (int)_BossAct.RotateAttack:
                boss_act_num = (int)_BossAct.Stay;
                switch (UpLoadData.boss_level)
                {
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[1][(int)_BossAct.RotateAttack - 1])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[1][(int)_BossAct.RotateAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[2][(int)_BossAct.RotateAttack - 1])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[2][(int)_BossAct.RotateAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[3][(int)_BossAct.RotateAttack - 1])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[3][(int)_BossAct.RotateAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[4][(int)_BossAct.RotateAttack - 1])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[4][(int)_BossAct.RotateAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[5][(int)_BossAct.RotateAttack - 1])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[5][(int)_BossAct.RotateAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[6][(int)_BossAct.RotateAttack - 1])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[6][(int)_BossAct.RotateAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[7][(int)_BossAct.RotateAttack - 1])    //2번 스킬
                        {
                            UpLoadData.boss_usedskill_list[7][(int)_BossAct.RotateAttack - 1] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(RotateAttackSkill());
                break;

            case (int)_BossAct.VerticalExplosion:
                boss_act_num = (int)_BossAct.Stay;
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[32][3])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[32][3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[1][3])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[1][3] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[2][5])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[2][5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[3][5])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[3][5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[4][5])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[4][5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[5][5])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[5][5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[6][5])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[6][5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[7][5])    //4번 스킬
                        {
                            UpLoadData.boss_usedskill_list[7][5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                }
                StartCoroutine(Skill_ExplosionVertical());
                break;

            case (int)_BossAct.Pillar:
                boss_act_num = (int)_BossAct.Stay;
                switch (UpLoadData.boss_level)
                {
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[2][6])    //7번 스킬
                        {
                            UpLoadData.boss_usedskill_list[2][6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[3][6])    //7번 스킬
                        {
                            UpLoadData.boss_usedskill_list[3][6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[4][6])    //7번 스킬
                        {
                            UpLoadData.boss_usedskill_list[4][6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[5][6])    //7번 스킬
                        {
                            UpLoadData.boss_usedskill_list[5][6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[6][6])    //7번 스킬
                        {
                            UpLoadData.boss_usedskill_list[6][6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[7][6])    //7번 스킬
                        {
                            UpLoadData.boss_usedskill_list[7][6] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(PillarSkill());
                break;

            case (int)_BossAct.Hp_Spread1:
                boss_act_num = (int)_BossAct.Stay;
                switch (UpLoadData.boss_level)
                {
                    case 0:
                        if (!UpLoadData.boss_usedskill_list[0][2])    //5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[0][2] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 1:
                        if (!UpLoadData.boss_usedskill_list[1][5])    //5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[1][5] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 2:
                        if (!UpLoadData.boss_usedskill_list[2][4])    //5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[2][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 3:
                        if (!UpLoadData.boss_usedskill_list[3][4])    //5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[3][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                        // 멀티 난이도
                    case 4:
                        if (!UpLoadData.boss_usedskill_list[4][4])    //5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[4][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 5:
                        if (!UpLoadData.boss_usedskill_list[5][4])    //5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[5][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 6:
                        if (!UpLoadData.boss_usedskill_list[6][4])    //5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[6][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    case 7:
                        if (!UpLoadData.boss_usedskill_list[7][4])    //5번 스킬
                        {
                            UpLoadData.boss_usedskill_list[7][4] = true;
                            UpLoadData.new_boss_skill_count++;
                        }
                        break;
                    default:
                        break;
                }
                StartCoroutine(HP_SpreadSkill(1));
                break;

            case (int)_BossAct.Hp_Spread2:
                boss_act_num = (int)_BossAct.Stay;
                StartCoroutine(HP_SpreadSkill(2));
                break;

            case (int)_BossAct.Hp_Spread3:
                boss_act_num = (int)_BossAct.Stay;
                StartCoroutine(HP_SpreadSkill(3));
                break;

        }
    }

    // 기본 공격
    IEnumerator Attack_active()
    {
        _animator.SetBool("Running", false);
        GameObject temp_target;

        temp_target = target_player;
        _animator.SetFloat("DirX", temp_target.transform.position.x - this.transform.position.x);
        _animator.SetFloat("DirY", temp_target.transform.position.y - this.transform.position.y);
        int count = UpLoadData.boss_level + 1;

        float time_level = (float)6 / (UpLoadData.boss_level + 3);

        WaitForSeconds waiting_time = new WaitForSeconds(time_level);
        WaitForSeconds waiting_second = new WaitForSeconds(time_level/5);
        WaitForSeconds waittime = new WaitForSeconds(0.01f);

        Quaternion temp;

        for (int i = 0; i < count; i++)
        {
            _animator.SetBool("Attack", true);
            SoundManager.instance.Play(14);

            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            var simulate = ps.main;
            simulate.simulationSpeed = (float)2/ time_level;
            skill_prefab_temp[0].transform.localScale = new Vector3(13f, 2f);
            if (temp_target.transform.position.x - this.transform.position.x >= 0)
                skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((temp_target.transform.position.y - this.transform.position.y) / (temp_target.transform.position.x - this.transform.position.x)));
            else
                skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((temp_target.transform.position.y - this.transform.position.y) / (temp_target.transform.position.x - this.transform.position.x)));

            temp = skill_prefab_temp[0].transform.rotation;
            if(NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, time_level, true,true);
            }
                Destroy(skill_prefab_temp[0], time_level);
            yield return waiting_time;
            SoundManager.instance.Play(2);
            main.GetComponent<Camera_move>().VivrateForTime(0.5f);
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = temp;
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, time_level, false, true);
            }
            _animator.SetBool("Attack", false);

            yield return 0;
        }

        _animator.SetBool("AttackEnd", true);
        yield return waiting_second;
        _animator.SetBool("AttackEnd", false);

        float timer = 0;
        while(timer<1)
        {
            timer += Time.deltaTime;
            
            MovetoTarget();
            yield return waittime;
        }

        

        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    // 필라 생성
    IEnumerator Skill_2_Pillar()
    {

        if (this.transform.childCount > 0)
        {
            foreach (Transform child in this.transform)
            {   
                if(child.name== "Dessert_Circle_Warning_Press(Clone)")
                {
                    skill_prefab_temp[3] = Instantiate(skill_prefab[3], child.transform); //스킬이펙트
                    skill_prefab_temp[3].transform.parent = this.transform.parent;
                    Destroy(child.gameObject);
                }
            }
        }
        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }


    // 세로 폭발
    IEnumerator Skill_ExplosionVertical()
    {
        int exception = Random.Range(1, 5);
        int exception2 = Random.Range(1, 4); 
        Vector2[] Vertical_positions = new Vector2[6];
        Vector2[] Horizental_positions = new Vector2[5];
        int t = 0;
        int h = 0;

        _animator.SetFloat("DirX", 0);
        _animator.SetFloat("DirY", -1);
        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);

        //this.transform.position = new Vector3(0, 0, 0); //맵 가운데로 보냄

        for(int i = 0; i < Vertical_positions.Length; i++)
        {
            Vertical_positions[i] = new Vector2(-10 + t, 10);
            t += 4;
        }

        for (int i = 0; i < Vertical_positions.Length; i++)
        {
            if(i != exception)
            {
                SoundManager.instance.Play(14);

                skill_prefab_temp[4] = Instantiate(skill_prefab[4], Vertical_positions[i], Quaternion.Euler(0,0,-90));
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 4, skill_prefab_temp[4].transform, 3, true, false);
                }
                ps = skill_prefab_temp[4].GetComponent<ParticleSystem>();
                var simulate = ps.main;
                simulate.simulationSpeed = 1 / 3f;
                Destroy(skill_prefab_temp[4], 3f);
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                yield return 0;
            }
        }
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < Horizental_positions.Length; i++)
        {
            Horizental_positions[i] = new Vector2(-12, 8 + h);
            h -= 4;
        }

        for (int i = 0; i < Horizental_positions.Length; i++)
        {
            if (i != exception2)
            {
                SoundManager.instance.Play(14);

                skill_prefab_temp[5] = Instantiate(skill_prefab[5], Horizental_positions[i], Quaternion.Euler(0, 0, 0));
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 5, skill_prefab_temp[5].transform, 3, true, false);
                }
                ps = skill_prefab_temp[5].GetComponent<ParticleSystem>();
                var simulate = ps.main;
                simulate.simulationSpeed = 1 / 3f;
                Destroy(skill_prefab_temp[5], 3f);
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                yield return 0;
            }
        }
        _animator.SetBool("Magic", false);

        yield return new WaitForSeconds(1.0f);

        
        main.GetComponent<Camera_move>().VivrateForTime(1f);
        for (int i = 0; i < Vertical_positions.Length; i++)
        {
            if (i != exception)
            {
                SoundManager.instance.Play(7);
                Vertical_positions[i].y = 0;
                skill_prefab_temp[6] = Instantiate(skill_prefab[6], Vertical_positions[i], Quaternion.identity);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 6, skill_prefab_temp[6].transform, 1, true, false);
                }
                Destroy(skill_prefab_temp[6], 1f);
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                yield return 0;
            }
        }
        //yield return new WaitForSeconds(0.2f);

        
        yield return new WaitForSeconds(1.0f);
        main.GetComponent<Camera_move>().VivrateForTime(1f);

        for (int i = 0; i < Horizental_positions.Length; i++)
        {
            if (i != exception2)
            {
                SoundManager.instance.Play(7);
                Horizental_positions[i].x = 0;
                skill_prefab_temp[7] = Instantiate(skill_prefab[7], Horizental_positions[i], Quaternion.identity);
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 7, skill_prefab_temp[7].transform, 1, true, false);
                }
                Destroy(skill_prefab_temp[7], 1f);
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                yield return 0;
            }
        }
        _animator.SetBool("MagicEnd", true);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("MagicEnd", false);
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            MovetoTarget();
            if (timer > 1)
            {
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;

    }

    IEnumerator SpreadSkill()
    {
        SoundManager.instance.Play(8);

        _animator.SetBool("Running", false);
        _animator.SetBool("Magic", true);
        yield return new WaitForSeconds(0.2f);


        skill_prefab_temp[14] = Instantiate(skill_prefab[14], target_player.transform); //장판 락온
        skill_prefab_temp[14].transform.localScale *= 2f;
        int index = 0;
        for(int i = 0; i < GameManage.instance.all_character_array.Length; i++)
        {
            if (target_player.GetComponent<CharacterStat>().My_Index == i)
                index = i;
        }

        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateTargetingBossSkill(UpLoadData.boss_index, 14, index, 2, true);
        }
        Destroy(skill_prefab_temp[14], 2f);

        vector3_temp = new Vector3(0, -0.2f);
        skill_prefab_temp[14].transform.Translate(vector3_temp);
        temp_target_player = target_player;
        yield return new WaitForSeconds(2.0f);
        _animator.SetBool("Magic", false);


        skill_prefab_temp[8] = Instantiate(skill_prefab[8], temp_target_player.transform); //장판 생성
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateTargetingBossSkill(UpLoadData.boss_index, 8, index, 2, false);
        }
        skill_prefab_temp[9] = Instantiate(skill_prefab[9], temp_target_player.transform);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateTargetingBossSkill(UpLoadData.boss_index, 9, index, 6.5f, true);
        }
        Destroy(skill_prefab_temp[9], 6.5f);


        yield return new WaitForSeconds(2f);
        _animator.SetBool("MagicEnd", true);
        yield return new WaitForSeconds(2f);
        _animator.SetBool("MagicEnd", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator CrossAttackSkill()
    {
        _animator.SetBool("Running", false);
        float timer = (float)9 / (UpLoadData.boss_level + 3);
        Vector3 size = new Vector3(13f, 2f, 1f);
        if (UpLoadData.boss_level >= 2)
        {
            _animator.SetBool("Attack", true);
            SoundManager.instance.Play(14);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 0);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1.5f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 90);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1.5f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1.5f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 270);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1.5f);

            yield return new WaitForSeconds(1.5f);
            SoundManager.instance.Play(14);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 45);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1.5f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 135);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1.5f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 225);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1.5f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 315);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1.5f);

            yield return new WaitForSeconds(1.5f);
            SoundManager.instance.Play(14);
            _animator.SetBool("Attack", false);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 22.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 67.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 112.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 157.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true,true);
            }
            Destroy(skill_prefab_temp[0], 1f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 202.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 247.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 292.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1f);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 337.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1.5f, true, true);
            }
            Destroy(skill_prefab_temp[0], 1f);
        }
        else
        {
            _animator.SetBool("Attack", true);
            SoundManager.instance.Play(14);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 0);
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            var simulate = ps.main;
            simulate.simulationSpeed = (float)2 / (timer);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, timer, true, true);
            }
            Destroy(skill_prefab_temp[0], timer);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 90);
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            simulate = ps.main;
            simulate.simulationSpeed = (float)2 / (timer);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, timer, true, true);
            }
            Destroy(skill_prefab_temp[0], timer);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180);
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            simulate = ps.main;
            simulate.simulationSpeed = (float)2 / (timer);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, timer, true, true);
            }
            Destroy(skill_prefab_temp[0], timer);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 270);
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            simulate = ps.main;
            simulate.simulationSpeed = (float)2 / (timer);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, timer, true, true);
            }
            Destroy(skill_prefab_temp[0], timer);

            yield return new WaitForSeconds(timer/2f);
            _animator.SetBool("Attack", false);
            SoundManager.instance.Play(14);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 45);
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            simulate = ps.main;
            simulate.simulationSpeed = (float)2 / (timer);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, timer, true, true);
            }
            Destroy(skill_prefab_temp[0], timer);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 135);
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            simulate = ps.main;
            simulate.simulationSpeed = (float)2 / (timer);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, timer, true, true);
            }
            Destroy(skill_prefab_temp[0], timer);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 225);
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            simulate = ps.main;
            simulate.simulationSpeed = (float)2 / (timer);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, timer, true, true);
            }
            Destroy(skill_prefab_temp[0], timer);
            skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // Warning
            skill_prefab_temp[0].transform.localScale = size;
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 315);
            ps = skill_prefab_temp[0].GetComponent<ParticleSystem>();
            simulate = ps.main;
            simulate.simulationSpeed = (float)2 / (timer);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, timer, true, true);
            }
            Destroy(skill_prefab_temp[0], timer);
        }

        yield return new WaitForSeconds(timer/2f);

        if (UpLoadData.boss_level >= 2)
        {
            SoundManager.instance.Play(2);
            main.GetComponent<Camera_move>().VivrateForTime(0.5f);
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 0);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 90);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 180);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 270);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }


            yield return new WaitForSeconds(1.0f);
            SoundManager.instance.Play(2);
            main.GetComponent<Camera_move>().VivrateForTime(0.5f);

            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 45);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 135);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 225);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 315);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }

            yield return new WaitForSeconds(1.0f);
            SoundManager.instance.Play(2);
            main.GetComponent<Camera_move>().VivrateForTime(0.5f);

            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform);
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 22.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); 
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 67.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform);
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 112.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform);
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 157.5f);
                        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); 
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 202.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); 
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 247.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform);
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 292.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); 
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 337.5f);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
        }
        else
        {
            SoundManager.instance.Play(2);
            main.GetComponent<Camera_move>().VivrateForTime(0.5f);
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 0);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 90);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 180);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 270);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }

            yield return new WaitForSeconds(timer / 2f);
            SoundManager.instance.Play(2);

            main.GetComponent<Camera_move>().VivrateForTime(0.5f);

            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 45);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 135);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 225);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
            skill_prefab_temp[2] = Instantiate(skill_prefab[2], this.transform); //중력 생성
            skill_prefab_temp[2].transform.localScale = new Vector3(0.2f, 0.2f);
            skill_prefab_temp[2].transform.rotation = Quaternion.Euler(0, 0, 315);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 2, skill_prefab_temp[2].transform, timer, false, true);
            }
        }

        yield return new WaitForSeconds(1f);

        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("AttackEnd", false);


        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator RotateAttackSkill()
    {
        _animator.SetBool("Running", false);

        _animator.SetBool("Attack", true);


        float angle = 37;
        SoundManager.instance.Play(14);
        skill_prefab_temp[10] = Instantiate(skill_prefab[10], this.transform); // Warning
        skill_prefab_temp[10].transform.localScale = new Vector3(20f, 1f);
        skill_prefab_temp[10].transform.rotation = Quaternion.Euler(0, 0, 0+ angle);
        if(NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 10, skill_prefab_temp[10].transform, 3, true, true);
        }
        Destroy(skill_prefab_temp[10], 3f);
        skill_prefab_temp[10] = Instantiate(skill_prefab[10], this.transform); // Warning
        skill_prefab_temp[10].transform.localScale = new Vector3(20f, 1f);
        skill_prefab_temp[10].transform.rotation = Quaternion.Euler(0, 0, 90+ angle);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 10, skill_prefab_temp[10].transform, 3, true, true);
        }
        Destroy(skill_prefab_temp[10], 3f);
        skill_prefab_temp[10] = Instantiate(skill_prefab[10], this.transform); // Warning
        skill_prefab_temp[10].transform.localScale = new Vector3(20f, 1f);
        skill_prefab_temp[10].transform.rotation = Quaternion.Euler(0, 0, 180+ angle);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 10, skill_prefab_temp[10].transform, 3, true, true);
        }
        Destroy(skill_prefab_temp[10], 3f);
        skill_prefab_temp[10] = Instantiate(skill_prefab[10], this.transform); // Warning
        skill_prefab_temp[10].transform.localScale = new Vector3(20f, 1f);
        skill_prefab_temp[10].transform.rotation = Quaternion.Euler(0, 0, 270+ angle);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 10, skill_prefab_temp[10].transform, 3, true, true);
        }
        Destroy(skill_prefab_temp[10], 3f);



        BossStatus.instance.BossSilentAble(true);
        float timer = 0;
        while(true)
        {
            timer += Time.deltaTime;
            if (timer >= 2)
                break;
            if(BossStatus.instance.Silent)
            {
                _animator.SetBool("Attack", false);
                _animator.SetBool("Silent", true);

                foreach (Transform child in this.transform.parent)
                {
                    if (child.name == "Straight_WarningRotateRed(Clone)")
                    {
                        Destroy(child.gameObject);
                    }
                }
                _animator.SetBool("Silent", false);

            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        BossStatus.instance.BossSilentAble(false);

        _animator.SetBool("Attack", false);


        SoundManager.instance.Play(5);
        main.GetComponent<Camera_move>().VivrateForTime(0.5f);
      



        skill_prefab_temp[11] = Instantiate(skill_prefab[11], this.transform); //중력 생성
        skill_prefab_temp[11].transform.localScale = new Vector3(0.4f, 0.4f);
        skill_prefab_temp[11].transform.rotation = Quaternion.Euler(0, 0, 0);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 11, skill_prefab_temp[11].transform, 3, false, true);
        }
        skill_prefab_temp[11] = Instantiate(skill_prefab[11], this.transform); //중력 생성
        skill_prefab_temp[11].transform.localScale = new Vector3(0.4f, 0.4f);
        skill_prefab_temp[11].transform.rotation = Quaternion.Euler(0, 0, 90);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 11, skill_prefab_temp[11].transform, 3, false, true);
        }
        skill_prefab_temp[11] = Instantiate(skill_prefab[11], this.transform); //중력 생성
        skill_prefab_temp[11].transform.localScale = new Vector3(0.4f, 0.4f);
        skill_prefab_temp[11].transform.rotation = Quaternion.Euler(0, 0, 180);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 11, skill_prefab_temp[11].transform, 3, false, true);
        }
        skill_prefab_temp[11] = Instantiate(skill_prefab[11], this.transform); //중력 생성
        skill_prefab_temp[11].transform.localScale = new Vector3(0.4f, 0.4f);
        skill_prefab_temp[11].transform.rotation = Quaternion.Euler(0, 0, 270);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 11, skill_prefab_temp[11].transform, 3, false, true);
        }

        _animator.SetBool("AttackEnd", true);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("AttackEnd", false);
        timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            MovetoTarget();
            if(timer>2)
            {
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }

        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }

    IEnumerator PillarSkill()
    {
        _animator.SetBool("Running", true);
        Vector2 temp;
        //가운데로 걸어감
        while(true)
        {
            temp = this.transform.position;
            temp.Normalize();
            this.gameObject.transform.Translate(-temp * Time.deltaTime*BossStatus.instance.moveSpeed*3f,Space.World);
            _animator.SetFloat("DirX", -temp.x);
            _animator.SetFloat("DirY", -temp.y);
            if (this.transform.position.x < 0.1 && this.transform.position.x > -0.1 && this.transform.position.y < 0.1 && this.transform.position.y >-0.1)
                break;           
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _animator.SetBool("Running", false);

        //4 분면 하나씩 pillar소환

        float max = 5;
        float min=1;

        Vector2 cor1 = new Vector2(Random.Range(min, max), Random.Range(min, max));
        Vector2 cor2 = new Vector2(Random.Range(-max, -min), Random.Range(min, max));
        Vector2 cor3 = new Vector2(Random.Range(-max, -min), Random.Range(-max, -min));
        Vector2 cor4 = new Vector2(Random.Range(min, max), Random.Range(-max, -min));

        _animator.SetBool("Magic", true);

        SoundManager.instance.Play(8);

        skill_prefab_temp[12] = Instantiate(skill_prefab[12], cor1,Quaternion.identity); // 마법진
        skill_prefab_temp[12].transform.localScale = new Vector2(0.5f,0.5f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 12, skill_prefab_temp[12].transform, 3, true, false);
            //Debug.Log(skill_prefab_temp[12].transform.position);
            //Debug.Log(skill_prefab_temp[12].transform.rotation);
            //Debug.Log(skill_prefab_temp[12].transform.localScale);


        }
        Destroy(skill_prefab_temp[12],3f);
        skill_prefab_temp[12] = Instantiate(skill_prefab[12], cor2, Quaternion.identity); // 마법진
        skill_prefab_temp[12].transform.localScale = new Vector2(0.5f, 0.5f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 12, skill_prefab_temp[12].transform, 3, true, false);
        }
        Destroy(skill_prefab_temp[12],3f);
        skill_prefab_temp[12] = Instantiate(skill_prefab[12], cor3, Quaternion.identity); // 마법진
        skill_prefab_temp[12].transform.localScale = new Vector2(0.5f, 0.5f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 12, skill_prefab_temp[12].transform, 3, true, false);
        }
        Destroy(skill_prefab_temp[12],3f);
        skill_prefab_temp[12] = Instantiate(skill_prefab[12], cor4, Quaternion.identity); // 마법진
        skill_prefab_temp[12].transform.localScale = new Vector2(0.5f, 0.5f);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 12, skill_prefab_temp[12].transform, 3, true, false);
        }
        Destroy(skill_prefab_temp[12],3f);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("Magic", false);

        yield return new WaitForSeconds(3f);

        _animator.SetBool("MagicEnd", true);

        main.GetComponent<Camera_move>().VivrateForTime(0.5f);


        skill_prefab_temp[3] = Instantiate(skill_prefab[3], this.transform); // 기둥
        skill_prefab_temp[3].transform.position = cor1;
        skill_prefab_temp[3].transform.SetParent(this.transform.parent);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 3, skill_prefab_temp[3].transform, 10, true, false);
        }
        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // 방향 warn
        skill_prefab_temp[0].transform.localScale = new Vector2(20f, 0.4f);

        if (cor1.x - this.transform.position.x > 0)
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((cor1.y - this.transform.position.y) / (cor1.x - this.transform.position.x)));
        else
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((cor1.y - this.transform.position.y) / (cor1.x - this.transform.position.x)));
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1, true, true);
        }

        skill_prefab_temp[3] = Instantiate(skill_prefab[3], this.transform); // 기둥
        skill_prefab_temp[3].transform.position = cor2;
        skill_prefab_temp[3].transform.SetParent(this.transform.parent);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 3, skill_prefab_temp[3].transform, 10, true, false);
        }
        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // 방향 warn
        skill_prefab_temp[0].transform.localScale = new Vector2(20f, 0.4f);

        if (cor2.x - this.transform.position.x > 0)
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((cor2.y - this.transform.position.y) / (cor2.x - this.transform.position.x)));
        else
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((cor2.y - this.transform.position.y) / (cor2.x - this.transform.position.x)));

        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1, true, true);
        }

        skill_prefab_temp[3] = Instantiate(skill_prefab[3], this.transform); // 기둥
        skill_prefab_temp[3].transform.position = cor3;
        skill_prefab_temp[3].transform.SetParent(this.transform.parent);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 3, skill_prefab_temp[3].transform, 10, true, false);
        }
        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // 방향 warn
        skill_prefab_temp[0].transform.localScale = new Vector2(20f, 0.4f);

        if (cor3.x - this.transform.position.x > 0)
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((cor3.y - this.transform.position.y) / (cor3.x - this.transform.position.x)));
        else
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((cor3.y - this.transform.position.y) / (cor3.x - this.transform.position.x)));

        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1, true, true);
        }

        skill_prefab_temp[3] = Instantiate(skill_prefab[3], this.transform); // 기둥
        skill_prefab_temp[3].transform.position = cor4;
        skill_prefab_temp[3].transform.SetParent(this.transform.parent);
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 3, skill_prefab_temp[3].transform, 10, true, false);
        }
        skill_prefab_temp[0] = Instantiate(skill_prefab[0], this.transform); // 방향 warn
        skill_prefab_temp[0].transform.localScale = new Vector2(20f, 0.4f);

        if (cor4.x - this.transform.position.x > 0)
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((cor4.y - this.transform.position.y) / (cor4.x - this.transform.position.x)));
        else
            skill_prefab_temp[0].transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((cor4.y - this.transform.position.y) / (cor4.x - this.transform.position.x)));

        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, skill_prefab_temp[0].transform, 1, true, true);
        }

        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("MagicEnd", false);


        _animator.SetBool("Attack", true);//준비자세로

        float BossHP = BossStatus.instance.HP;
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            if(timer>6f)
            {
                break;
            }
            if (BossHP != BossStatus.instance.HP) //맞아서 체력이 단 경우
            {
                BossHP = BossStatus.instance.HP;
                main.GetComponent<Camera_move>().VivrateForTime(0.1f);
                foreach (Transform child in this.transform)
                {
                    if (child.name == "Straight_WarningRed(Clone)")
                    {
                       child.transform.Rotate(0, 0, 6f);
                        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                        {
                            NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 0, child.transform, 0.5f, true, true);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //휘두르기 , 날리기
        _animator.SetBool("AttackEnd", true);//준비자세로
        SoundManager.instance.Play(6);
        main.GetComponent<Camera_move>().VivrateForTime(0.5f);


        foreach (Transform child in this.transform)
        {
            if (child.name == "Straight_WarningRed(Clone)")
            {                
                skill_prefab_temp[13] = Instantiate(skill_prefab[13], this.transform); // Warning
                skill_prefab_temp[13].transform.localScale = new Vector3(1f, 1f);
                skill_prefab_temp[13].transform.rotation = child.transform.localRotation;
                if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                {
                    NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 13, skill_prefab_temp[13].transform, 3, true, true);
                }
                Destroy(skill_prefab_temp[13], 3f);
                Destroy(child.gameObject);
            }
        }
        yield return new WaitForSeconds(2f);
        _animator.SetBool("AttackEnd", false);//준비자세로

        while (true)
        {
            timer += Time.deltaTime;
            MovetoTarget();
            if (timer > 2)
            {
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }



        GameObject[] Pillar;

        Pillar = GameObject.FindGameObjectsWithTag("Pillar");

        for(int i=0;i<Pillar.Length;i++)
        {
            Destroy(Pillar[i].gameObject,10f);            
        }//pillar삭제
        
        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }


    IEnumerator HP_SpreadSkill(int i)
    {
        _animator.SetBool("Running", false);

        Vector2 vec_temp;
        SoundManager.instance.Play(8);
        switch (i)
        {
            case 1: //첫번째는 왼쪽에 생성
                vec_temp.x = -10;
                _animator.SetFloat("DirX", -1);
                _animator.SetFloat("DirY", 0);
                _animator.SetBool("Magic", true);
                for (int j=7;j>=-8;j-=3)
                {
                    vec_temp.y = j;
                    skill_prefab_temp[14] = Instantiate(skill_prefab[14], vec_temp, Quaternion.identity); //장판 락온
                    skill_prefab_temp[14].transform.localScale *= 2f;
                    if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                    {
                        NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 14, skill_prefab_temp[14].transform, 3, true, false);
                    }
                    Destroy(skill_prefab_temp[14], 3f);
                }
                yield return new WaitForSeconds(2f);
                _animator.SetBool("Magic", false);

                yield return new WaitForSeconds(1f);
                _animator.SetBool("MagicEnd", true);

                for (int j = 7; j >= -8; j -= 3)
                {
                    vec_temp.y = j;
                    skill_prefab_temp[15] = Instantiate(skill_prefab[15], vec_temp, Quaternion.identity); //장판온
                    skill_prefab_temp[15].transform.localScale *= 2f;
                    if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                    {
                        NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 15, skill_prefab_temp[15].transform, 3, false, false);
                    }
                }
                yield return new WaitForSeconds(0.5f);
                _animator.SetBool("MagicEnd", false);
                break;
            case 2: //오른쪽에 생성
                vec_temp.x = 10;
                _animator.SetFloat("DirX", 1);
                _animator.SetFloat("DirY", 0);
                _animator.SetBool("Magic", true);

                for (int j = 7; j >= -8; j -= 3)
                {
                    vec_temp.y = j;
                    skill_prefab_temp[14] = Instantiate(skill_prefab[14], vec_temp, Quaternion.identity); //장판 락온
                    skill_prefab_temp[14].transform.localScale *= 2f;
                    if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                    {
                        NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 14, skill_prefab_temp[14].transform, 3, true, false);
                    }
                    Destroy(skill_prefab_temp[14], 3f);
                }
                yield return new WaitForSeconds(2f);
                _animator.SetBool("Magic", false);

                yield return new WaitForSeconds(1f);
                _animator.SetBool("MagicEnd", true);

                for (int j = 7; j >= -8; j -= 3)
                {
                    vec_temp.y = j;
                    skill_prefab_temp[15] = Instantiate(skill_prefab[15], vec_temp, Quaternion.identity); //장판온
                    skill_prefab_temp[15].transform.localScale *= 2f;
                    if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                    {
                        NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 15, skill_prefab_temp[15].transform, 3, false, false);
                    }
                }
                yield return new WaitForSeconds(0.5f);
                _animator.SetBool("MagicEnd", false);
                break;
            case 3: //가운데에 생성
                vec_temp.y = 0;
                _animator.SetFloat("DirX", 0);
                _animator.SetFloat("DirY", -this.transform.position.y);
                _animator.SetBool("Magic", true);

                for (float j = 7; j >= -7; j -= 3.5f)
                {
                    vec_temp.x = j;
                    skill_prefab_temp[14] = Instantiate(skill_prefab[14], vec_temp, Quaternion.identity); //장판 락온
                    skill_prefab_temp[14].transform.localScale *= 2f;
                    if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                    {
                        NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 14, skill_prefab_temp[14].transform, 3, true, false);
                    }
                    Destroy(skill_prefab_temp[14], 3f);
                }
                yield return new WaitForSeconds(2f);
                _animator.SetBool("Magic", false);

                yield return new WaitForSeconds(1f);
                _animator.SetBool("MagicEnd", true);

                for (float j = 7; j >= -7; j -= 3.5f)
                {
                    vec_temp.x = j;
                    skill_prefab_temp[15] = Instantiate(skill_prefab[15], vec_temp, Quaternion.identity); //장판온
                    skill_prefab_temp[15].transform.localScale *= 2f;
                    if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
                    {
                        NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 15, skill_prefab_temp[15].transform, 3, false, false);
                    }
                }
                yield return new WaitForSeconds(0.5f);
                _animator.SetBool("MagicEnd", false);
                break;
        }

        boss_act_num = (int)_BossAct.Chasing;
        yield return 0;
    }




    public void BossGetSilent()
    {
        StopAllCoroutines();
        StartCoroutine(Exec());

    }
    public void BossGetSilent(GameObject prefab1,GameObject prefab2)
    {
        Destroy(prefab1);
        Destroy(prefab2);
        StopAllCoroutines();
        StartCoroutine(Exec());

    }
    public void GetStunning(float time)
    {
        is_stunned = true;
        Stunning_time = time;
    }

    public bool IsSkillCasting()
    {
        return is_skill_casting;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadPlayer"))
        {

            Physics2D.IgnoreCollision(this.GetComponent<CapsuleCollider2D>(), collision.GetComponent<CapsuleCollider2D>());
        }

    }

}
