using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character_Control : MonoBehaviour
{
    public GameObject Select_player_arrow_prefab;
    public GameObject[] skillBackground;
    public GameObject[] skillButton;
    public GameObject[] potion;

    [HideInInspector]
    public Vector3 target_pos; //마우스 땠을때 마지막 위치    
    [HideInInspector]
    public float move_speed = 1.0f; //움직이는 속도  다른곳에서 속도를 바꾸어줄수있도록 public
    [HideInInspector]
    public Vector3 move_dir; //움직이는 방향
    //[HideInInspector]
    public GameObject clickpanel;

    [HideInInspector]
    public bool is_collision;//케릭터에 부딫혔을때
    [HideInInspector]
    public bool is_moving; //마우스땠을때 움직이는 중인가
    [HideInInspector]
    public int Character_Status; //케릭터 상태
    private enum Status
    {
        Stand = 0,
        Running,
        Attack,
        Magic,
        Dead,        
    }
    [HideInInspector]
    public bool Runable;
    [HideInInspector]
    public bool Skill_ready;
    [HideInInspector]
    public bool is_MoveTogether=false;

    public Collider2D monster_col;

    private GameObject temp; //화살표 프리팹
    private GameObject _player;
    private GameObject target_object;

    public float timeOut;

    //private Rigidbody2D _rigidbody;
    [HideInInspector]
    public Animator _animator;
    private Vector2 pos;
    private RaycastHit2D hit;

    private Vector3 touch_pos; //터치하고 있는 곳
    private Vector3 vector3_temp; //임시로 넣는 temp vector
    private float float_temp;
    private LineRenderer _linerenderer;
    
    private const float skill_move_speed = 1.8f; //스킬창 뜨는속도
    //private bool is_skill_active; //스킬 active 되엇는가
    private bool is_SpeedBufOn;
    private CharacterStat characterStat;
    private WaitForSeconds waittime;
    public GameObject[] normal_attack_effect;
    private GameObject normal_attack_temp;
    public string id;

    public bool isLocalPlayer = false;

    public bool isOnline = false;

    private bool is_touch_boundary = false;

    private WaitForSeconds waitAttacktime;


    void Start()
    {
        is_moving = false; //is_skill_active = false; 
        Runable = true; Skill_ready = false;
        //      _linerenderer = GetComponent<LineRenderer>();
        //_linerenderer.startWidth = 0.02f;
        //_linerenderer.endWidth = 0.02f;
        //_linerenderer.positionCount = 2;
        //_linerenderer.material = Resources.Load<Material>("Material/green");
        is_collision = false;
        _animator = this.GetComponent<Animator>();
        characterStat=this.GetComponent<CharacterStat>();
        StartCoroutine(CharacterNormalAttack());
        move_speed = characterStat.move_speed;
        waittime = new WaitForSeconds(0.01f);
        waitAttacktime = new WaitForSeconds(1 / characterStat.Attack_speed);
        _player = this.gameObject;
        SetSkill(characterStat.char_num);
        GameManage.instance.selected_player = this.gameObject;
        PotionChange();
    }
    public void SwitchPlayerStat()
    {
        move_speed = characterStat.move_speed;
        waitAttacktime = new WaitForSeconds(1 / characterStat.Attack_speed);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //누르는 순간
        {
            //Init_skill();

            //pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //hit = Physics2D.Raycast(pos, Vector2.zero, 1000f, 1 << 14);

            //_linerenderer.positionCount = 2;
            //_linerenderer.material= Resources.Load<Material>("Material/green");

            //if (clickpanel.GetComponent<CharacterClick>().IsThisCharacter(hit))
            //{
            //    if (GameObject.Find("player_select_arrow_1(Clone)"))    
            //            Destroy(GameObject.Find("player_select_arrow_1(Clone)"));//화살표 제거
            //    GameManage.instance.selected_player =null;
            //    if (!this.CompareTag("Player")) return;
            //    target_object = null;
            //    _player = this.gameObject; //클릭할때 player 오브젝트 받고
            //    _rigidbody = _player.GetComponent<Rigidbody2D>(); //rigidbody 넣어주고
            //    is_moving = false;
            //    temp = Instantiate(Select_player_arrow_prefab, this.transform); //화살표 효과
            //    GameManage.instance.selected_player = this.gameObject;
            //    //여기 스킬 뜨기
            //    for (int i = 0; i < skillBackground.Length; i++)
            //        skillBackground[i].SetActive(true);
            //    SetSkill(characterStat.char_num); //koki 케릭터 번호를 넣어주어야함
            //    is_skill_active = true;

            PotionChange();

            //    Character_Status = (int)Status.Stand;
            //}
            /*else if(GameManage.instance.selected_player==this.gameObject)//플레이어 이미 누른상태에서 땅누른경우
            {
                is_skill_active = true;
            }
            else if(GameManage.instance.selected_player != this.gameObject)
            {
                Init_skill();
            }*/

        }

        else if (Input.GetMouseButton(0) /*&&!is_moving*//*&&_player!=null*/&&this.tag=="Player") //클릭하는동안 바로바로움직임 가는 곳 보여야함
        {
            touch_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touch_pos.z = this.transform.position.z; //z축은 안움직이도록
            move_dir = (touch_pos - this.transform.position).normalized;

            ////vector3_temp = touch_pos - this.transform.position;

            if(OptionManager.instance.Is_UseJoyStick)
            {
                move_dir = GameManage.instance.JoysitckDir.normalized;
            }
            //선 보이기     
            //_linerenderer.SetPosition(0, _player.transform.position);
            //_linerenderer.SetPosition(1, touch_pos);
            if (Runable) //움직일수 있을때에만
            {
                _animator.SetFloat("DirX", move_dir.x);
                _animator.SetFloat("DirY", move_dir.y);
                _animator.SetBool("Running", true);
                Character_Status = (int)Status.Running;
                _player.transform.Translate(move_dir * move_speed * Time.deltaTime); //움직임
                UpdateStatusToServer();
            }
            else
            {                
                _animator.SetFloat("DirX", move_dir.x);
                _animator.SetFloat("DirY", move_dir.y);
                _animator.SetBool("Running", false);
            }
        }

        else if (Input.GetMouseButtonUp(0)/* &&!is_moving*/ && _player != null && this.tag == "Player") //마우스 때는순간
        {
            //Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0.5f);
            //if(hit)
            //{
            //    if (hit.collider.gameObject.tag == "Boss")//보스에서 마우스를 때는 경우
            //    {
            //        target_object = hit.collider.gameObject;
            //        _linerenderer.material = Resources.Load<Material>("Material/red");
            //    }
            //    else
            //    {
            //        target_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //    }
            //}
            //else
            //{
            //    target_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //}
             RunningStop();
        }
        //else if(Input.GetMouseButton(0)&&this.tag!="Player") //죽은경우 또는 상태 이상인경우
        //{
        //    _linerenderer.positionCount = 0; //선 없애기
        //}

        //if (is_moving&&this.tag=="Player") //마우스 때고나서 움직임    어택하고 바로 클릭 놓으면 움직여짐
        //{
        //    if(target_object != null)
        //    {
        //        target_pos = target_object.transform.position;
        //    }
        //    target_pos.z = this.transform.position.z; //z축은 안움직이도록
        //    move_dir = (target_pos - this.transform.position).normalized;

        //    if (Runable)
        //    {
        //        _animator.SetFloat("DirX", move_dir.x);
        //        _animator.SetFloat("DirY", move_dir.y);
        //        _animator.SetBool("Running", true);
        //        _player.transform.Translate(move_dir * move_speed * Time.deltaTime); //움직임
        //    }
            

        //    _linerenderer.positionCount = 2;
        //    _linerenderer.SetPosition(0, _player.transform.position);
        //    _linerenderer.SetPosition(1, target_pos);


        //    //가는도중 거의 다 도착하면    또는 부딫히면
        //    if ((target_pos-this.transform.position).magnitude< move_speed * Time.deltaTime || is_collision)
        //    {
        //        RunningStop();
        //    }
        //}
        else if(is_moving&&this.tag!="Player") //마우스는 땟지만 죽어있을때
        {
            //_rigidbody = null;
            is_moving = false;
            _linerenderer.positionCount = 0;
        }

        //if(is_skill_active) //스킬창이 십자로 뜨도록   크기/포지션
        //{
        //    is_skill_active = false;

        //    skillBackground[0].transform.Translate(transform.up * skill_move_speed );
        //    skillBackground[1].transform.Translate(transform.right * skill_move_speed);
        //    skillBackground[2].transform.Translate(-transform.up * skill_move_speed);
        //    skillBackground[3].transform.Translate(-transform.right * skill_move_speed);  //포지션 변화            
            
        //    skillBackground[0].transform.localScale += new Vector3(1.4f, 1.4f);
        //    skillBackground[1].transform.localScale += new Vector3(1.4f, 1.4f);
        //    skillBackground[2].transform.localScale += new Vector3(1.4f, 1.4f);
        //    skillBackground[3].transform.localScale += new Vector3(1.4f, 1.4f);
            
        //}
        //if(skillBackground[0].transform.localScale.x>0)
        //{

        //    for (int i=0;i<4;i++)
        //    {
        //        skillBackground[i].transform.localScale = Vector3.one * Camera.main.orthographicSize * 1.4f/6;
        //    }
        //    skillBackground[0].transform.position = this.transform.position + transform.up * skill_move_speed * Camera.main.orthographicSize /6;
        //    skillBackground[1].transform.position = this.transform.position + transform.right * skill_move_speed * Camera.main.orthographicSize /6;
        //    skillBackground[2].transform.position = this.transform.position + -transform.up * skill_move_speed * Camera.main.orthographicSize /6;
        //    skillBackground[3].transform.position = this.transform.position + -transform.right * skill_move_speed * Camera.main.orthographicSize /6;

        //}
    }

    public void PotionChange()
    {
        switch (this.GetComponent<CharacterStat>().char_num) //누르는 순간 uselist로 주소 옮겨옴
        {
            case 0:
                DatabaseManager.instance.UseList = DatabaseManager.instance.UseList0;
                break;
            case 1:
                DatabaseManager.instance.UseList = DatabaseManager.instance.UseList1;
                break;
            case 2:
                DatabaseManager.instance.UseList = DatabaseManager.instance.UseList2;
                break;
            case 3:
                DatabaseManager.instance.UseList = DatabaseManager.instance.UseList3;
                break;
            case 4:
                DatabaseManager.instance.UseList = DatabaseManager.instance.UseList4;
                break;
            case 5:
                DatabaseManager.instance.UseList = DatabaseManager.instance.UseList5;
                break;
            case 6:
                DatabaseManager.instance.UseList = DatabaseManager.instance.UseList6;
                break;
            case 7:
                DatabaseManager.instance.UseList = DatabaseManager.instance.UseList7;
                break;
        }
        if (DatabaseManager.instance.UseList[0].itemID != 0)
        {
            potion[0].GetComponent<UISprite>().color = new Color(1, 1, 1, 1);
            potion[0].GetComponent<UISprite>().spriteName = DatabaseManager.instance.UseList[0].itemIcon.name;
        }
        else
            potion[0].GetComponent<UISprite>().color = new Color(0, 0, 0, 0);
        if (DatabaseManager.instance.UseList[1].itemID != 0)
        {
            potion[1].GetComponent<UISprite>().spriteName = DatabaseManager.instance.UseList[1].itemIcon.name;
            potion[1].GetComponent<UISprite>().color = new Color(1, 1, 1, 1);
        }
        else
            potion[1].GetComponent<UISprite>().color = new Color(0, 0, 0, 0);
    }

    public void RunningStop()
    {
        //_rigidbody = null;
        is_moving = false;
        //is_collision = false;
        //_linerenderer.positionCount = 0;
        //_animator.SetBool("Attack", false);
        _animator.SetBool("Running", false);
        Character_Status = (int)Status.Stand;
    }


    public void SetSkill(int _char) //케릭터 번호가 인자
    {
        //케릭터에 맞는 이미지로 바꾸어줌  스킬 그림 그리고 넣어주세요
        switch(_char)
        {
            case 0: //assa
                skillButton[0].GetComponent<UISprite>().spriteName = "1";
                skillButton[1].GetComponent<UISprite>().spriteName = "2";
                skillButton[2].GetComponent<UISprite>().spriteName = "3";
                skillButton[3].GetComponent<UISprite>().spriteName = "4";
                break;                                        
            case 1:                                           
                skillButton[0].GetComponent<UISprite>().spriteName = "5";
                skillButton[1].GetComponent<UISprite>().spriteName = "6";
                skillButton[2].GetComponent<UISprite>().spriteName = "7";
                skillButton[3].GetComponent<UISprite>().spriteName = "8";
                break;                                        
            case 2://fan                                      
                skillButton[0].GetComponent<UISprite>().spriteName = "9";
                skillButton[1].GetComponent<UISprite>().spriteName = "10";
                skillButton[2].GetComponent<UISprite>().spriteName = "11";
                skillButton[3].GetComponent<UISprite>().spriteName = "12";
                break;                                        
            case 3://hammer                                   
                skillButton[0].GetComponent<UISprite>().spriteName = "13";
                skillButton[1].GetComponent<UISprite>().spriteName = "14";
                skillButton[2].GetComponent<UISprite>().spriteName = "15";
                skillButton[3].GetComponent<UISprite>().spriteName = "16";
                break;                                        
            case 4:                                           
                skillButton[0].GetComponent<UISprite>().spriteName = "17";
                skillButton[1].GetComponent<UISprite>().spriteName = "18";
                skillButton[2].GetComponent<UISprite>().spriteName = "19";
                skillButton[3].GetComponent<UISprite>().spriteName = "20";
                break;                                       
            case 5:                                          
                skillButton[0].GetComponent<UISprite>().spriteName = "21";
                skillButton[1].GetComponent<UISprite>().spriteName = "22";
                skillButton[2].GetComponent<UISprite>().spriteName = "23";
                skillButton[3].GetComponent<UISprite>().spriteName = "24";
                break;                                       
            case 6:                                          
                skillButton[0].GetComponent<UISprite>().spriteName = "25";
                skillButton[1].GetComponent<UISprite>().spriteName = "26";
                skillButton[2].GetComponent<UISprite>().spriteName = "27";
                skillButton[3].GetComponent<UISprite>().spriteName = "28";
                break;                                       
            case 7:                                          
                skillButton[0].GetComponent<UISprite>().spriteName = "29";
                skillButton[1].GetComponent<UISprite>().spriteName = "30";
                skillButton[2].GetComponent<UISprite>().spriteName = "31";
                skillButton[3].GetComponent<UISprite>().spriteName = "32";
                break;
        }

    }
    public void Init_skill() //스킬 칸 다시 제자리로 돌아가기
    {
        //is_skill_active = false;

        skillBackground[0].transform.localPosition = new Vector3(0, 0, 0);
        skillBackground[1].transform.localPosition = new Vector3(0, 0, 0);
        skillBackground[2].transform.localPosition = new Vector3(0, 0, 0);
        skillBackground[3].transform.localPosition = new Vector3(0, 0, 0);

        skillBackground[0].transform.localScale = new Vector3(0, 0);
        skillBackground[1].transform.localScale = new Vector3(0, 0);
        skillBackground[2].transform.localScale = new Vector3(0, 0);
        skillBackground[3].transform.localScale = new Vector3(0, 0);
    }

    public bool SpeedBuf(float speedmultiply,float timer)
    {
        if (is_SpeedBufOn) return false;
        StartCoroutine(SpeedCor(speedmultiply,timer));
        return true;
    }

    IEnumerator SpeedCor(float speedmultiply, float timer)
    {
        is_SpeedBufOn = true;
        this.move_speed *= speedmultiply;
        yield return new WaitForSeconds(timer);
        this.move_speed /= speedmultiply;
        is_SpeedBufOn = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.collider.isTrigger==false&&collision.gameObject.tag=="Player") //케릭터끼리면
        //{
        //    is_collision = true;

        //    vector3_temp.x = (collision.transform.position.x - this.transform.position.x)  * Time.deltaTime;
        //    vector3_temp.y = (collision.transform.position.y - this.transform.position.y)  * Time.deltaTime;

        //    this.transform.position -= vector3_temp;
        //    collision.transform.position += vector3_temp;
        //}

        if(collision.gameObject.tag == "DeadPlayer")
        {
            Physics2D.IgnoreCollision(collision.collider, this.gameObject.GetComponent<CapsuleCollider2D>());
        }

        if(collision.gameObject.tag == "Boundary")
        {
            is_touch_boundary = true;
            //RunningStop();
        }

        if(collision.collider.isTrigger == false&&collision.gameObject.tag=="Boss") //보스한테 부딫히면
        {
            is_collision = true;
            vector3_temp.x = (collision.transform.position.x - this.transform.position.x) * Time.deltaTime;
            vector3_temp.y = (collision.transform.position.y - this.transform.position.y) * Time.deltaTime;

            this.transform.position -= vector3_temp;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") //케릭터끼리면
        {
            is_collision = false;
        }
        if (collision.gameObject.tag == "Boss") //보스한테 부딫히면
        {
            is_collision = false;
        }
        if (collision.gameObject.tag == "Boundary")
        {
            is_touch_boundary = false;
        }
    }

    IEnumerator CharacterNormalAttack()
    {
        WaitForSeconds timer = new WaitForSeconds(0.5f);
        GameObject[] NormalAttack_prefab=new GameObject[8];
        Collider2D[] _col=new Collider2D[4];
        int monster_index=-1;
        while (true) 
        {
            while(this.tag=="Player")//살아있거나 상태이상 이 아닐때
            {
                if (characterStat.Attack_range > Vector3.Distance(BossStatus.instance.transform.position, this.transform.position) && !_animator.GetBool("Running") && Runable && !Skill_ready && BossStatus.instance.CompareTag("Boss")) //공격할만큼 가까운 거리일때 , 가만히 있을때
                {

                    //공격모션
                    vector3_temp = BossStatus.instance.transform.position - this.transform.position;
                    _animator.SetFloat("DirX", vector3_temp.x);
                    _animator.SetFloat("DirY", vector3_temp.y);

                    Runable = false;
                    _animator.SetBool("Attack", true);
                    float_temp = _animator.speed;
                    _animator.speed = characterStat.Attack_speed;

                    yield return waittime;

                    _animator.SetBool("Attack", false);

                    //공격모션간 대기

                    

                    yield return new WaitForSeconds(0.5f / characterStat.Attack_speed);

                    NormalAttack_prefab[characterStat.char_num] = Instantiate(SkillManager.instance.NormalAttack[characterStat.char_num], this.transform); //투사체

                    if(NetworkManager.instance.is_multi)//멀티인 경우 해당 instantiate 가 다르사람에게도 보여야함
                    {
                        NetworkManager.instance.InstantiateOtherPlayerSkill(90+characterStat.char_num, NormalAttack_prefab[characterStat.char_num].transform);
                    }

                    _animator.speed = float_temp;


                    Runable = true;


                    //공격이펙트 (무조건 맞는 걸로)
                    //temp = Instantiate(normal_attack_temp, BossStatus.instance.gameObject.transform);
                    //Destroy(temp, 0.5f);
                }
                else if (monster_index >= 0 && !_animator.GetBool("Running") && Runable && !Skill_ready)
                {
                    //공격모션
                    monster_col = _col[monster_index];
                    vector3_temp = _col[monster_index].transform.position - this.transform.position;
                    _animator.SetFloat("DirX", vector3_temp.x);
                    _animator.SetFloat("DirY", vector3_temp.y);

                    Runable = false;
                    _animator.SetBool("Attack", true);
                    float_temp = _animator.speed;
                    _animator.speed = characterStat.Attack_speed;

                    yield return waittime;

                    _animator.SetBool("Attack", false);
                    
                    //공격모션간 대기
                    yield return new WaitForSeconds(0.5f / characterStat.Attack_speed);

                    NormalAttack_prefab[characterStat.char_num] = Instantiate(SkillManager.instance.NormalAttack_toMonster[characterStat.char_num], this.transform); //투사체
                    if (NetworkManager.instance.is_multi)//멀티인 경우 해당 instantiate 가 다르사람에게도 보여야함
                    {
                        NetworkManager.instance.InstantiateOtherPlayerSkill(90 + characterStat.char_num, NormalAttack_prefab[characterStat.char_num].transform);
                    }
                    _animator.speed = float_temp;

                    Runable = true;

                }
                _col = Physics2D.OverlapCircleAll(this.transform.position, characterStat.Attack_range / 2f);
                for (int i = 0; i < _col.Length; i++)
                {
                    if (_col[i].CompareTag("Monster"))
                    {
                        monster_index = i;
                        break;
                    }
                    else
                        monster_index = -1;
                }

                yield return waitAttacktime;
            }
            yield return waitAttacktime;
        }

    }
    public void StartAttack()
    {
        StartCoroutine(CharacterNormalAttack());
    }



    //public void StickTogether(GameObject AnotherPlayer,float Distance)
    //{
    //    StartCoroutine(StickTogetherSkill(AnotherPlayer, Distance));
    //}
    //IEnumerator StickTogetherSkill(GameObject AnotherPlayer, float Distance)
    //{
    //    Vector3 vec_temp;

    //    while (true)
    //    {
    //        if (!is_StickTogether) break;
    //        if (Vector2.Distance(AnotherPlayer.transform.position, this.transform.position) > Distance+0.25f) //더크면 땡기고
    //        {
    //            vec_temp = AnotherPlayer.transform.position - this.transform.position;
    //            vec_temp.z = 0;
    //            vec_temp.Normalize();
    //            if (_animator.GetBool("Running"))
    //                this.transform.Translate(vec_temp*move_speed * Time.deltaTime);
    //                //this.transform.position = Vector2.Lerp(this.transform.position, AnotherPlayer.transform.position, Time.deltaTime);
    //        }
    //        else if(Vector2.Distance(AnotherPlayer.transform.position, this.transform.position) < Distance - 0.25f)   // 더작으면 멀어지고
    //        {
    //            vec_temp = this.transform.position - AnotherPlayer.transform.position;
    //            vec_temp.z = 0;
    //            vec_temp.Normalize();
    //            if (_animator.GetBool("Running"))
    //                this.transform.Translate(vec_temp * move_speed * Time.deltaTime);
    //                //this.transform.position = Vector2.Lerp(this.transform.position, 2*this.transform.position - AnotherPlayer.transform.position, Time.deltaTime);
    //        }
    //        yield return waittime;

    //    }
    //}

    public void SameDirMove(GameObject AnotherPlayer)
    {
        StartCoroutine(SameDirMoveCor(AnotherPlayer));
    }

    public void OppositeMove(GameObject AnotherPlayer,GameObject prefab)
    {
        StartCoroutine(OppositeMoveCor(AnotherPlayer, prefab));
    }

    IEnumerator SameDirMoveCor(GameObject AnotherPlayer)
    {
        while (true)
        {
            if (_animator.GetBool("Running"))
            {
                if (is_touch_boundary)
                {
                }
                else
                {
                    AnotherPlayer.transform.Translate(move_dir * move_speed * Time.deltaTime);
                }
                if (AnotherPlayer.GetComponent<Character_Control>().is_touch_boundary)
                {
                    RunningStop();
                }
            }
            yield return waittime;

            if (!is_MoveTogether)
                break;
        }
    }

    IEnumerator OppositeMoveCor(GameObject AnotherPlayer,GameObject prefab)
    {
        GameObject prefab_temp=null;
        while(true)
        {
            if(_animator.GetBool("Running")&&!is_collision&&!is_touch_boundary&&this.CompareTag("Player"))
                AnotherPlayer.transform.Translate(-move_dir * move_speed * Time.deltaTime);
            if(AnotherPlayer.CompareTag("DeadPlayer")||AnotherPlayer.GetComponent<Character_Control>().is_collision|| AnotherPlayer.GetComponent<Character_Control>().is_touch_boundary)
            {
                if(prefab_temp==null&&this.CompareTag("Player"))
                {
                    prefab_temp = Instantiate(prefab, this.transform); // Warning
                    Destroy(prefab_temp, 2f);
                }                   
                //발묶인 상태표시
                RunningStop();                
            }            
            yield return waittime;

            if(!is_MoveTogether)
                break;
        }        
    }

    void UpdateStatusToServer()
    {

        //NetworkManager.instance.EmitRotation(transform.rotation);

        NetworkManager.instance.EmitPlayerPosition(transform.position);
    }

    public void UpdatePosition(Vector3 position)
    {
        if (!isLocalPlayer)
        {
            transform.position = new Vector3(position.x, position.y, position.z);
                //UpdateAnimator("IsWalk");
        }

    }

}
