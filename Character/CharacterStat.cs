using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    //*************** 기본적인 케릭터 스테이터스 ***************

    // 케릭터의 체력. 
    [HideInInspector]
    public float HP = 100f;
    [HideInInspector]
    public float Tag_HP;
    private float HP_temp;
    [HideInInspector]
    public float MaxHP = 100f;

    // 마나 
    [HideInInspector]
    public float MP = 0f;
    [HideInInspector]
    public float Tag_MP;
    private float MP_temp;

    [HideInInspector]
    public float MaxMp = 0f;

    //물리 공격력. 
    [HideInInspector]
    public float PhysicalAttackPower = 0f;

    //마법 공격력. 
    [HideInInspector]
    public float MagicAttackPower = 0f;

    // 물리 방어력. 
    [HideInInspector]
    public float PhysicalArmor = 0f;

    //마법 방어력. 
    [HideInInspector]
    public float MagicArmorPower = 0f;

    //케릭터 기본공격 사정거리
    [HideInInspector]
    public float Attack_range=0f;

    //케릭터 기본공격 공격속도
    [HideInInspector]
    public float Attack_speed = 0f;

    [HideInInspector]
    public float move_speed = 0f;

    //크리율
    //[HideInInspector]
    public float ciritical = 0f;

    //체젠
    [HideInInspector]
    public float HP_Regenerate = 0f;

    //마젠
    [HideInInspector]
    public float MP_Regenerate = 0f;

    


    //케릭터 직업 번호
    // [HideInInspector]
    public int char_num;

    //케릭터 인덱스!!
    public int char_index;

    private RuntimeAnimatorController[] _animation;  //이거 public으로 할필요 없고 그냥 private 으로 다 넣으면 됩니다..
    [HideInInspector]
    public bool is_firstCharacter;

    //[HideInInspector]
    public bool is_Invincible=false;

    // 아이템을 등록할 리스트
    public List<Item> itemList;

    // 캐릭터가 소지하고 있는 아이템 리스트
    public List<Item> equipList;
    //*************** 기본적인 케릭터 스테이터스 ***************

    public int My_Index;

    private GameObject hit_prefab_temp;
    private GameObject hit_prefab;
    private GameObject heal_prefab_temp;
    private GameObject heal_prefab;

    private void Awake()
    {
        itemList = new List<Item>();
        equipList = new List<Item>();
        //if (is_firstCharacter)                 //바로 복구 바랍니다!!
            char_num = UpLoadData.character_index[0];
        //else
        //    char_num = UpLoadData.character_index[1];
        DatabaseManager.instance.Init_char_stat(char_num);
        MaxHP = DatabaseManager.instance.MaxHP;
        HP = DatabaseManager.instance.HP;
        MP = DatabaseManager.instance.MP;
        MaxMp = DatabaseManager.instance.MaxMp;
        PhysicalAttackPower = DatabaseManager.instance.PhysicalAttackPower;
        MagicAttackPower = DatabaseManager.instance.MagicAttackPower;
        PhysicalArmor = DatabaseManager.instance.PhysicalArmor;
        MagicArmorPower = DatabaseManager.instance.MagicArmorPower;
        Attack_range = DatabaseManager.instance.NormalAttackRange;
        Attack_speed = DatabaseManager.instance.NormalAttackSpeed;
        move_speed = DatabaseManager.instance.MoveSpeed;

        ciritical = DatabaseManager.instance.ciritical;
        HP_Regenerate = DatabaseManager.instance.HP_Regenerate;
        MP_Regenerate = DatabaseManager.instance.MP_Regenerate;


        itemList = DatabaseManager.instance.itemList;
        equipList = DatabaseManager.instance.equipList;
        InputAnimator();
        ChangeSprite();
        DatabaseManager.instance.Init_char_stat(UpLoadData.character_index[1]);
        Tag_HP = DatabaseManager.instance.HP;
        Tag_MP = DatabaseManager.instance.MP;
        DatabaseManager.instance.Init_char_stat(char_num);
    }

    private void Start()
    {
        StartCoroutine(Healself());
        hit_prefab = Resources.Load("Effect/BloodSplat_FX") as GameObject;
        hit_prefab_temp = Instantiate(hit_prefab, this.transform);//맞은 이펙트
        hit_prefab_temp.SetActive(false);
        heal_prefab = Resources.Load("Effect/EnergyPull_FX") as GameObject;
        heal_prefab_temp = Instantiate(heal_prefab, this.transform);//힐 이펙트
        heal_prefab_temp.SetActive(false);
        is_firstCharacter = true;
    }
    public void SwitchPlayer()
    {
        HP_temp = HP;
        MP_temp = MP;

        DatabaseManager.instance.Init_char_stat(char_num);
        MaxHP = DatabaseManager.instance.MaxHP;
        HP = Tag_HP;
        MP = Tag_MP;
        MaxMp = DatabaseManager.instance.MaxMp;
        PhysicalAttackPower = DatabaseManager.instance.PhysicalAttackPower;
        MagicAttackPower = DatabaseManager.instance.MagicAttackPower;
        PhysicalArmor = DatabaseManager.instance.PhysicalArmor;
        MagicArmorPower = DatabaseManager.instance.MagicArmorPower;
        Attack_range = DatabaseManager.instance.NormalAttackRange;
        Attack_speed = DatabaseManager.instance.NormalAttackSpeed;
        move_speed = DatabaseManager.instance.MoveSpeed;

        ciritical = DatabaseManager.instance.ciritical;
        HP_Regenerate = DatabaseManager.instance.HP_Regenerate;
        MP_Regenerate = DatabaseManager.instance.MP_Regenerate;


        itemList = DatabaseManager.instance.itemList;
        equipList = DatabaseManager.instance.equipList;
        ChangeSprite();

        Tag_HP = HP_temp;
        Tag_MP = MP_temp;
        is_firstCharacter = !is_firstCharacter;
    }
    public void ChangeSprite()
    {
        this.GetComponent<Animator>().runtimeAnimatorController = _animation[char_num];
    }
    public void ChangeSprite(int num)
    {
        this.GetComponent<Animator>().runtimeAnimatorController = _animation[num];
    }
    private void InputAnimator()
    {
        _animation = new RuntimeAnimatorController[8];
        _animation[0] = Resources.Load<RuntimeAnimatorController>("Animator/Assassin") as RuntimeAnimatorController;
        _animation[1] = Resources.Load<RuntimeAnimatorController>("Animator/Bow") as RuntimeAnimatorController;
        _animation[2] = Resources.Load<RuntimeAnimatorController>("Animator/Fan") as RuntimeAnimatorController;
        _animation[3] = Resources.Load<RuntimeAnimatorController>("Animator/Hammer") as RuntimeAnimatorController;
        _animation[4] = Resources.Load<RuntimeAnimatorController>("Animator/Healer") as RuntimeAnimatorController;
        _animation[5] = Resources.Load<RuntimeAnimatorController>("Animator/Mage") as RuntimeAnimatorController;
        _animation[6] = Resources.Load<RuntimeAnimatorController>("Animator/Shield") as RuntimeAnimatorController;
        _animation[7] = Resources.Load<RuntimeAnimatorController>("Animator/Spear") as RuntimeAnimatorController;
    }
    public float GetDamage(float _damage,bool is_magic)
    {
        if(this.GetComponentInChildren<ShieldSkill4>() != null)
        {
            GameUI.instance.FloatingGreenEffect(this.transform.position, "Protected");
            this.GetComponentInChildren<ShieldSkill4>().GetShieldDamage(_damage);
            return 0;
        }
        if (this.GetComponentInChildren<MageSkill2>() != null)
        {
            GameUI.instance.FloatingGreenEffect(this.transform.position, "Protected");
            this.GetComponentInChildren<MageSkill2>().GetShieldDamage(_damage);
            return 0;
        }

        if (this.GetComponentInChildren<Vanish>() != null)
        {
            if (is_magic)
            {
                _damage *= 2;
            }
            else
                _damage = 0;
        }
        if (this.GetComponentInChildren<ShieldSkill2>() != null||is_Invincible)
        {
            _damage = 0;
            GameUI.instance.FloatingGreenEffect(this.transform.position, "Invincible");
            return 0;
        }


        StartCoroutine(Blink());

        if (is_magic)
        {
            if(MagicArmorPower>=0)
                _damage = _damage * 100 / (100 + MagicArmorPower);
            else
                _damage = _damage * (2 - 100 / (100 - MagicArmorPower));
        }
        else
        {
            if (PhysicalArmor >= 0)
                _damage = _damage * 100 / (100 + PhysicalArmor);
            else
                _damage = _damage * (2 - 100 / (100 - PhysicalArmor));
        }
        _damage += Random.Range(-_damage * 0.1f, _damage * 0.1f);

        _damage = (int)_damage;

        if(this.GetComponentInChildren<Boss7_BloodPoolDamage>() != null)
        {
            BossStatus.instance.BossGetHeal(_damage);
        }
        GameUI.instance.FloatingRedDamage(this.transform.position, _damage);
        HP -= _damage;

        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.AccumulateBossToPlayerDamage(My_Index, is_firstCharacter, _damage);
        }
        else
        {
            if (is_firstCharacter)
            {
                GameManage.instance.Damage_fromBoss[0] += _damage;
            }
            else
            {
                GameManage.instance.Damage_fromBoss[1] += _damage;
            }
        }


        if (HP <= 0)
        {
            this.tag = "DeadPlayer";
            //this.GetComponentInChildren<CharacterClick>().CharacterDead();
            this.GetComponent<Animator>().SetBool("Running", false);
            this.GetComponent<Animator>().SetBool("IsDead", true);

            SkillManager.instance.SkillCanceled();
            GameUI.instance.SkillOff();
            bool Is_switch = GameUI.instance.SwitchPlayer();


            if(!Is_switch&& NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.CharacterDead();
                GameManage.instance.Multi_MyCharacter_Die();
            }
            else if(!Is_switch)
            {
                GameManage.instance.BossWinGame();
            }
            //this.GetComponent<CapsuleCollider2D>().enabled = false;

            //if (NetworkManager.instance.is_allDead&&BossStatus.instance.gameObject)
            //{
            //    GameManage.instance.GetTargetPlayer(this.gameObject);
            //    GameManage.instance.BossWinGame();
            //}
            //this.GetComponent<Character_Control>().Init_skill();

            //this.GetComponent<SpriteRenderer>().sortingLayerName = "Object";
        }
        return _damage;
    }

    public void Revive()
    {
        this.HP = this.MaxHP / 2;
        this.tag = "Player";
        this.GetComponentInChildren<CharacterClick>().CharacterRevive();
        if(this.GetComponent<Animator>().isActiveAndEnabled != true)
        {
            this.GetComponent<Animator>().enabled = true;
        }
        this.GetComponent<Animator>().SetBool("IsDead", false);
        this.GetComponent<Character_Control>().Skill_ready = false;
        this.GetComponent<CapsuleCollider2D>().enabled = true;
        GameManage.instance.num_char++; //한명 살아남
        //this.GetComponent<Character_Control>().Init_skill();
        SkillManager.instance.SkillCanceled();
        this.GetComponent<SpriteRenderer>().sortingLayerName = "Player";


    }

    public void GetHeal(float _heal,GameObject player, bool ToOther = false)
    {
        StartCoroutine(Heal());

        if (this.GetComponentInChildren<Vanish>() != null)
        {
            _heal *= 2;
        }
        _heal = (int)_heal;
        if(_heal<=0)
        {
            _heal = 1;
        }
        GameUI.instance.FloatingGreenDamage(this.transform.position, _heal);
        if (NetworkManager.instance.is_multi&& ToOther)
        {
            NetworkManager.instance.HealEffect(true, this.transform.position, _heal);
        }

        if (HP+_heal>MaxHP)
        {
            HP = MaxHP;
        }
        else
        {
            HP += _heal;
        }

        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.AccumulatePlayerHeal(My_Index, is_firstCharacter, _heal);
        }
        else
        {
            if (is_firstCharacter)
            {
                GameManage.instance.Heal[0] += _heal;
            }
            else
            {
                GameManage.instance.Heal[1] += _heal;
            }
        }
    }

    public void GetCriticalHeal(float _heal, GameObject player, bool ToOther = false)
    {
        StartCoroutine(Heal());
        _heal *= 2f;
        if (this.GetComponentInChildren<Vanish>() != null)
        {
            _heal *= 2;
        }
        _heal = (int)_heal;
        if (_heal <= 0)
        {
            _heal = 1;
        }
        GameUI.instance.FloatingGreenDamageBold(this.transform.position, _heal);
        if (NetworkManager.instance.is_multi&& ToOther)
        {
            NetworkManager.instance.HealEffect(true, this.transform.position, _heal);
        }
        if (HP + _heal > MaxHP)
        {
            HP = MaxHP;
        }
        else
        {
            HP += _heal;
        }

        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.AccumulatePlayerHeal(My_Index, is_firstCharacter, _heal);
        }
        else
        {
            if (is_firstCharacter)
            {
                GameManage.instance.Heal[0] += _heal;
            }
            else
            {
                GameManage.instance.Heal[1] += _heal;
            }
        }
    }

    public void GetMana(float _mana,bool ToOther=false)
    {
        //StartCoroutine(Mana());
        _mana = (int)_mana;
        if(_mana<=0)
        {
            _mana = 1;
        }
        GameUI.instance.FloatingBlueDamage(this.transform.position, _mana);
        if (NetworkManager.instance.is_multi&& ToOther)
        {
            NetworkManager.instance.HealEffect(false, this.transform.position, _mana);
        }
        if (MP + _mana > MaxMp)
        {
            MP = MaxMp;
        }
        else
        {
            MP += _mana;
        }
    }

    IEnumerator Healself()
    {
        while(true)
        {
            yield return new WaitForSeconds(5.0f);
            if(this.CompareTag("Player")) 
            {
                GetHeal(HP_Regenerate*10, this.gameObject);

                if (MP + MP_Regenerate * 10 > MaxMp)
                    MP = MaxMp;
                else
                    MP += MP_Regenerate*10;
            }
        }

    }

    IEnumerator Blink()
    {
        Color _color;
        _color = new Color(1, 1, 1);
        _color.a = this.GetComponent<SpriteRenderer>().color.a;

        hit_prefab_temp.SetActive(true);
        this.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, this.GetComponent<SpriteRenderer>().color.a);
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<SpriteRenderer>().color = _color;
        hit_prefab_temp.SetActive(false);

    }
    IEnumerator Heal()
    {
        Color _color;
        _color = new Color(1, 1, 1);
        _color.a = this.GetComponent<SpriteRenderer>().color.a;

        heal_prefab_temp.SetActive(true);
        this.GetComponent<SpriteRenderer>().color = new Color(0,1,0, this.GetComponent<SpriteRenderer>().color.a);
        yield return new WaitForSeconds(0.1f);

        this.GetComponent<SpriteRenderer>().color = _color;
        heal_prefab_temp.SetActive(false);
    }
    IEnumerator Mana()
    {
        Color _color;
        _color = new Color(1, 1, 1);
        _color.a = this.GetComponent<SpriteRenderer>().color.a;

        this.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, this.GetComponent<SpriteRenderer>().color.a);
        yield return new WaitForSeconds(0.1f);
        
        this.GetComponent<SpriteRenderer>().color = _color;
    }
}
