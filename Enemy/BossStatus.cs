using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* 컨셉 : 부유
 * 스킬 : 부하를 다수
 * 
 * 소환, 보스의 공격에 의해 부하들도 같이 죽음, 하늘에서 아이템을 떨굼, 장비를 뺏음  
 */

public class BossStatus : MonoBehaviour
{
    static public BossStatus instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            //DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }  //--------------인스턴스화를 위함 ----

    //*************** 기본적인 보스의 스테이터스(구체적인 값은 레벨 디자인때 정하기로) ***************

    // 보스의 체력.     
    [HideInInspector]
    public float HP = 100f;
    [HideInInspector]
    public float MaxHP = 100f;

    // 보스의 마나, 때리면 마나를 회복한다. 
    [HideInInspector]
    public float MP = 0f;
    [HideInInspector]
    public float MaxMp = 0f;

    // 보스의 물리 공격력. 
    [HideInInspector]
    public float PhysicalAttackPower = 0f;

    // 보스의 마법 공격력. 
    [HideInInspector]
    public float MagicAttackPower = 0f;

    // 보스의 물리 방어력. 
    [HideInInspector]
    public float PhysicalArmor = 0f;

    // 보스의 마법 방어력. 
    [HideInInspector]
    public float MagicArmorPower = 0f;

    //보스의 이동속도
    [HideInInspector]
    public float moveSpeed;

    // 보스의 이름.
    public string BossName = "wealthyBoss";

    [HideInInspector]
    public float Aggro;
    [HideInInspector]
    public float taunt;
    [HideInInspector]
    public float _distance;
    [HideInInspector]
    public float _heal_sum;
    //[HideInInspector]
    public GameObject[] player;
    [HideInInspector]
    public GameObject target_player;

    [HideInInspector]
    public bool SilentAble;
    [HideInInspector]
    public bool Silent;
    [HideInInspector]
    public bool Skill_Using;

    public class Buff
    {
        public int number;
        public float timer;
    }

    [HideInInspector]
    public List<Buff> buff = new List<Buff>();  //버프 1번부터 시작

    public List<Buff> multi_buff = new List<Buff>();    // 멀티 버프전용 리스트

    private int player_num;
    private bool is_dead;
    private Color temp_color;
    private const float Armor_Debuf = 20f;
    private const float Move_Debuf = 1f;


    private GameObject hit_prefab_temp;
    private GameObject hit_prefab;
    private GameObject[] SilentAblePrefab;

    public GameObject Angry_Effect;
    private GameObject Angry_temp;

    private GameObject[] other_players;
    private GameObject player_character;
    private GameObject[] all_character;

    private void Start()
    {
        DatabaseManager.instance.Init_Boss_Stat(UpLoadData.boss_index, UpLoadData.boss_level); //0 desertboss , 0 level
        hit_prefab = Resources.Load("Effect/Boss_hit") as GameObject;
        hit_prefab_temp = Instantiate(hit_prefab, this.transform);//맞은 이펙트
        hit_prefab_temp.SetActive(false);
        is_dead = false; Silent = false;
        Skill_Using = false;

        player_num = NetworkManager.instance.Player_num;

        SilentAblePrefab = new GameObject[2];
        
        switch (UpLoadData.boss_index)
        {
            case 0:
                BGMManager.instance.Play(1);
                BGMManager.instance.FadeInMusic();
                break;
            case 1:
                BGMManager.instance.Play(2);
                BGMManager.instance.FadeInMusic();
                break;
            case 2:
                BGMManager.instance.Play(3);
                BGMManager.instance.FadeInMusic();
                break;
            case 3:
                BGMManager.instance.Play(4);
                BGMManager.instance.FadeInMusic();
                break;
            case 4:
                BGMManager.instance.Play(5);
                BGMManager.instance.FadeInMusic();
                break;
            case 5:
                BGMManager.instance.Play(6);
                BGMManager.instance.FadeInMusic();
                break;
            case 6:
                BGMManager.instance.Play(7);
                BGMManager.instance.FadeInMusic();
                break;
            case 7:
                BGMManager.instance.Play(8);
                BGMManager.instance.FadeInMusic();
                break;
        }

    }
    //private void PlayerSort()
    //{
    //    GameObject[] playerSorted = new GameObject[player_num];

    //    int char_index=0;

    //    for(int i=0;i< player_num; i++)
    //    {
    //        for(int j=0;j<player_num;j++)
    //        {
    //            if (player[j].GetComponent<CharacterStat>().char_index == char_index)
    //            {
    //                playerSorted[char_index++] = player[j];
    //                break;
    //            }
    //        }           
    //    }
    //    player = playerSorted;        
    //}
    public GameObject[] PlayerObjectSort(GameObject[] player_Object)
    {
        GameObject[] playerSorted = new GameObject[player_Object.Length];

        int char_index = 0;

        for (int i = 0; i < player_Object.Length; i++)
        {
            for (int j = 0; j < player_Object.Length; j++)
            {
                if (player_Object[j].GetComponent<CharacterStat>().char_index == char_index)
                {
                    playerSorted[char_index++] = player_Object[j];
                    break;
                }
            }
        }
        player_Object = playerSorted;

        return player_Object;
    }
    public void BossGetDamage(float _damage, bool is_magic)
    {
        if (buff.Find(item => item.number == 3) != null)
        {
            if (is_magic)
            {
                _damage *= 2;
            }
            else
                _damage = 0;
        }
        if (this.GetComponentInChildren<Move_Devine>() != null)
        {
            _damage = 0;
            GameUI.instance.FloatingGreenEffect(this.transform.position, "Invincible");
            return;

        }

        if (is_magic)
        {
            if (MagicArmorPower >= 0)
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

        _damage += Random.Range(-_damage*0.1f, _damage*0.1f);

        _damage = (int)_damage;
        HP -= _damage;

        GameUI.instance.FloatingWhiteDamage(this.transform.position, _damage);

        StartCoroutine(Hit());

        if (HP <= 0 && !is_dead)//죽으면
        {
            is_dead = true;
            this.GetComponent<SpriteRenderer>().material = Resources.Load("Material/TurnFire") as Material;
            StartCoroutine(Fadeout());
            GameManage.instance.CharacterWinGame();
        }
    }


    public void BossGetDamage(float _damage, bool is_magic,GameObject player)
    {
        if(buff.Find(item=>item.number == 3) !=null)
        {
            if (is_magic)
            {
                _damage *= 2;
            }
            else
                _damage = 0;
        }

        if (is_magic)
        {
            if (MagicArmorPower >= 0)
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
        if (this.GetComponentInChildren<Move_Devine>() != null)
        {
            _damage = 0;
            GameUI.instance.FloatingGreenEffect(this.transform.position, "Invincible");
            return;

        }
        _damage += Random.Range(-_damage * 0.1f, _damage * 0.1f);

        _damage = (int)_damage;
        HP -= _damage;

        if (NetworkManager.instance.is_multi)
        {
            // 서버로 바로 올릴 수 있도록 인덱스와 데미지를 올려준다 클라이언트 인덱스, 캐릭터 순서, 데미지
            Debug.Log(player.GetComponent<CharacterStat>().is_firstCharacter);
            NetworkManager.instance.AccumulatePlayerToBossDamage(NetworkManager.instance.my_index, player.GetComponent<CharacterStat>().is_firstCharacter, _damage);
        }
        else {
            if (player.GetComponent<CharacterStat>().char_num != GameManage.instance.My_Character_num)
            {
                GameManage.instance.Damage_toBoss[1] += _damage;
            }
            else
            {
                GameManage.instance.Damage_toBoss[0] += _damage;
            }
        }
        GameUI.instance.FloatingWhiteDamage(this.transform.position, _damage);

        StartCoroutine(Hit());

        if (HP <= 0 && !is_dead)//죽으면
        {
            is_dead = true;
            this.GetComponent<SpriteRenderer>().material = Resources.Load("Material/TurnFire") as Material;
            StartCoroutine(Fadeout());
            GameManage.instance.CharacterWinGame();
        }
    }

    public void BossGetCriticalDamage(float _damage, bool is_magic, GameObject player)
    {
        _damage *= 2.5f;
        if (buff.Find(item => item.number == 3) != null)
        {
            if (is_magic)
            {
                _damage *= 2;
            }
            else
                _damage = 0;
        }

        if (is_magic)
        {
            if (MagicArmorPower >= 0)
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

        if (this.GetComponentInChildren<Move_Devine>() != null)
        {
            _damage = 0;
            GameUI.instance.FloatingGreenEffect(this.transform.position, "Invincible");
            return;

        }
        _damage += Random.Range(-_damage * 0.1f, _damage * 0.1f);

        _damage = (int)_damage;
        HP -= _damage;
        if (NetworkManager.instance.is_multi)
        {
            // 서버로 바로 올릴 수 있도록 인덱스와 데미지를 올려준다 클라이언트 인덱스, 캐릭터 순서, 데미지
            NetworkManager.instance.AccumulatePlayerToBossDamage(NetworkManager.instance.my_index, player.GetComponent<CharacterStat>().is_firstCharacter, _damage);
        }
        else
        {
            if (player.GetComponent<CharacterStat>().char_num != GameManage.instance.My_Character_num)
            {
                GameManage.instance.Damage_toBoss[1] += _damage;
            }
            else
            {
                GameManage.instance.Damage_toBoss[0] += _damage;
            }
        }
        GameUI.instance.FloatingOrangeDamage(this.transform.position, _damage);

        StartCoroutine(Hit());

        if (HP <= 0 && !is_dead)//죽으면
        {
            is_dead = true;
            this.GetComponent<SpriteRenderer>().material = Resources.Load("Material/TurnFire") as Material;
            StartCoroutine(Fadeout());
            GameManage.instance.CharacterWinGame();
        }
    }


    public void BossGetSkillDamage(float _damage, bool is_magic, GameObject player)
    {
        if (buff.Find(item => item.number == 3) != null)
        {
            if (is_magic)
            {
                _damage *= 2;
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player,true);
                }
            }
            else
            {
                _damage = 0;
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player,true);
                }
            }

        }
        if (this.GetComponentInChildren<Move_Devine>() != null)
        {
            _damage = 0;
            GameUI.instance.FloatingGreenEffect(this.transform.position, "Invincible");
            return;
        }
        if (is_magic)
        {
            if (MagicArmorPower >= 0)
            {
                _damage = _damage * 100 / (100 + MagicArmorPower);
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player,true);
                }

            }
            else
            {
                _damage = _damage * (2 - 100 / (100 - MagicArmorPower));
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player,true);
                }
            }
        }
        else
        {
            if (PhysicalArmor >= 0)
            {
                _damage = _damage * 100 / (100 + PhysicalArmor);
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player,true);
                }
            }
            else
            {
                _damage = _damage * (2 - 100 / (100 - PhysicalArmor));
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player, true);
                }
            }
        }
        _damage += Random.Range(-_damage * 0.1f, _damage * 0.1f);

        _damage = (int)_damage;
        HP -= _damage;

        if (NetworkManager.instance.is_multi)
        {
            // 서버로 바로 올릴 수 있도록 인덱스와 데미지를 올려준다 클라이언트 인덱스, 캐릭터 순서, 데미지
            NetworkManager.instance.AccumulatePlayerToBossDamage(NetworkManager.instance.my_index, player.GetComponent<CharacterStat>().is_firstCharacter, _damage);
        }
        else
        {
            if (player.GetComponent<CharacterStat>().is_firstCharacter)
            {
                GameManage.instance.Damage_toBoss[0] += _damage;
            }
            else
            {
                GameManage.instance.Damage_toBoss[1] += _damage;
            }
        }
        GameUI.instance.FloatingYellowDamage(this.transform.position, _damage);

        StartCoroutine(Hit());

        if (HP <= 0 && !is_dead)//죽으면
        {
            is_dead = true;
            this.GetComponent<SpriteRenderer>().material = Resources.Load("Material/TurnFire") as Material;
            StartCoroutine(Fadeout());
            GameManage.instance.CharacterWinGame();
        }
    }

    public void BossGetSkillCriticalDamage(float _damage, bool is_magic, GameObject player)
    {
        if (buff.Find(item => item.number == 3) != null)
        {
            if (is_magic)
            {
                _damage *= 2;
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player, true);
                }
            }
            else
            {
                _damage = 0;
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player, true);
                }
            }

        }
        if (this.GetComponentInChildren<Move_Devine>() != null)
        {
            _damage = 0;
            GameUI.instance.FloatingGreenEffect(this.transform.position, "Invincible");
            return;
        }

        if (is_magic)
        {
            if (MagicArmorPower >= 0)
            {
                _damage = _damage * 100 / (100 + MagicArmorPower);
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player, true);
                }

            }
            else
            {
                _damage = _damage * (2 - 100 / (100 - MagicArmorPower));
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player, true);
                }
            }
        }
        else
        {
            if (PhysicalArmor >= 0)
            {
                _damage = _damage * 100 / (100 + PhysicalArmor);
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player, true);
                }
            }
            else
            {
                _damage = _damage * (2 - 100 / (100 - PhysicalArmor));
                if (player.GetComponentInChildren<Spear_Skill3_1>() != null)
                {
                    player.GetComponent<CharacterStat>().GetHeal(_damage * player.GetComponentInChildren<Spear_Skill3_1>().percent, player, true);
                }
            }
        }
        _damage += Random.Range(-_damage * 0.1f, _damage * 0.1f);

        _damage = (int)_damage;
        HP -= _damage;
        if (NetworkManager.instance.is_multi)
        {
            // 서버로 바로 올릴 수 있도록 인덱스와 데미지를 올려준다 클라이언트 인덱스, 캐릭터 순서, 데미지
            NetworkManager.instance.AccumulatePlayerToBossDamage(NetworkManager.instance.my_index, player.GetComponent<CharacterStat>().is_firstCharacter, _damage);
        }
        else
        {
            if (player.GetComponent<CharacterStat>().char_num != GameManage.instance.My_Character_num)
            {
                GameManage.instance.Damage_toBoss[1] += _damage;
            }
            else
            {
                GameManage.instance.Damage_toBoss[0] += _damage;
            }
        }
        GameUI.instance.FloatingOrangeDamage(this.transform.position, _damage);

        StartCoroutine(Hit());

        if (HP <= 0 && !is_dead)//죽으면
        {
            is_dead = true;
            this.GetComponent<SpriteRenderer>().material = Resources.Load("Material/TurnFire") as Material;
            StartCoroutine(Fadeout());
            GameManage.instance.CharacterWinGame();
        }
    }

    public void BossGetHeal(float amount)
    {
        this.HP += amount;
        GameUI.instance.FloatingOrangeDamage(this.transform.position, amount);
        if(this.HP >= this.MaxHP)
        {
            this.HP = this.MaxHP;
        }

    }

    public void BossSilentAble(bool able)
    {
        SilentAble = able;
        if (able)
        {
            this.GetComponent<SpriteRenderer>().material = Resources.Load("Material/ColorRed") as Material;
            NetworkManager.instance.BossGetSilent(true);
        }
        else
        {
            this.GetComponent<SpriteRenderer>().material = Resources.Load("Material/AlphaIntensity") as Material;
            NetworkManager.instance.BossGetSilent(false);

        }
    }

    public void BossSilentAble(bool able,GameObject prefab)
    {
        SilentAblePrefab[0] = prefab;
        SilentAble = able;
        if(able)
        {
            this.GetComponent<SpriteRenderer>().material = Resources.Load("Material/ColorRed") as Material;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().material = Resources.Load("Material/AlphaIntensity") as Material;
        }
    }

    public void BossSilentAble(bool able, GameObject prefab1,GameObject prefab2)
    {
        SilentAblePrefab[0] = prefab1;
        SilentAblePrefab[1] = prefab2;
        SilentAble = able;
        if (able)
        {
            this.GetComponent<SpriteRenderer>().material = Resources.Load("Material/ColorRed") as Material;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().material = Resources.Load("Material/AlphaIntensity") as Material;
        }
    }

    public void BossGetSilent()
    {
        Silent = true;
        this.GetComponent<Animator>().SetBool("Attack", false);
        this.GetComponent<Animator>().SetBool("Magic", false);

        this.GetComponent<Animator>().SetBool("Silent", true);

        if (SilentAblePrefab[0])
            Destroy(SilentAblePrefab[0]);
        if (SilentAblePrefab[1])
            Destroy(SilentAblePrefab[1]);

        StartCoroutine(SilentCor());

    }
    IEnumerator SilentCor()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Animator>().SetBool("Silent", false);
    }
    public void BossGetAggro()
    {
        _distance = 20/Vector3.Distance(GameManage.instance.player[0].transform.position, this.transform.position);
    }
    /*
     * 
     * 버프목록
     * 0 : 방감
     * 1 : 연계
     * 2 : 독화살
     * 
     * 
     * 
     */

    public int BuffAmount(int Buffnum)
    {
        return buff.FindAll(item => item.number == Buffnum).Count;       
    }

    public void BossBuffDelete(int Buffnum)
    {
        BuffEffectDeleteAll(Buffnum);
        buff.RemoveAll(item => item.number == Buffnum);
    }

    public void MultiBossBuffDelete(int Buffnum)
    {
        BuffEffectDeleteAll(Buffnum);
        multi_buff.RemoveAll(item => item.number == Buffnum);
    }

    private void BuffEffectDeleteAll(int Buffnum)
    {
        int count;
        count = buff.FindAll(item => item.number == Buffnum).Count;
        for(int i=0;i<count;i++)
        {
            switch (Buffnum)
            {
                case 0:
                    break;
                case 1:
                    this.PhysicalArmor += Armor_Debuf;
                    break;
                case 2: //독
                    break;
                case 4:
                    this.moveSpeed += Move_Debuf;
                    break;
                case 5://공격력감소
                    this.PhysicalAttackPower += 10;
                    this.MagicAttackPower += 10;
                    break;
            }
        }
    }

    private void MultiBuffEffectDeleteAll(int Buffnum)
    {
        int count;
        count = multi_buff.FindAll(item => item.number == Buffnum).Count;
        for (int i = 0; i < count; i++)
        {
            switch (Buffnum)
            {
                case 0:
                    break;
                case 1:
                    this.PhysicalArmor += Armor_Debuf;
                    break;
                case 2: //독
                    break;
                case 4:
                    this.moveSpeed += Move_Debuf;
                    break;
                case 5://공격력감소
                    this.PhysicalAttackPower += 10;
                    this.MagicAttackPower += 10;
                    break;
            }
        }
    }

    public void BossBuffClear()
    {
        /*while (buff[0] != null)
        {
            /*switch (buff[0].number)
            {
                case 0:
                    buff.RemoveAt(0);
                    break;
                case 1:
                    this.PhysicalArmor += Armor_Debuf;
                    buff.RemoveAt(0);
                    break;
                case 2:
                    break;
                case 4:
                    this.moveSpeed += Move_Debuf;
                    break;
                case 5://공격력감소
                    this.PhysicalAttackPower += 10;
                    this.MagicAttackPower += 10;
                    break;

            }
            buff.RemoveAt(0);
        }*/
        buff.RemoveRange(0, buff.Count);
    }

    public void GetBuff(int getbuff)
    {
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.BossGetBuff(getbuff);
            return;
        }

        Buff buf = new Buff();
        buf.number = getbuff;
        buf.timer = SkillManager.instance.buff_timer[getbuff]; 

        buff.Add(buf);
       
        buff.Sort(delegate (Buff A, Buff B)
        {
            if (A.number > B.number) return 1;
            else if (A.number < B.number) return -1;
            return 0;
        });
        BuffEffect(getbuff);
    }

    public void GetBuff(int getbuff,GameObject player)
    {
        if (NetworkManager.instance.is_multi)
        {
            NetworkManager.instance.BossGetBuff(getbuff, player.GetComponent<CharacterStat>().My_Index);
            //Debug.Log(player.GetComponent<CharacterStat>().My_Index);
            return;
        }

        Buff buf = new Buff();
        buf.number = getbuff;
        buf.timer = SkillManager.instance.buff_timer[getbuff];
        
        buff.Add(buf);

        buff.Sort(delegate (Buff A, Buff B)
        {
            if (A.number > B.number) return 1;
            else if (A.number < B.number) return -1;
            return 0;
        });
        BuffEffect(getbuff, player);
    }

    public void MultiGetBuff(int getbuff)
    {
        Buff buf = new Buff();
        buf.number = getbuff;
        buf.timer = SkillManager.instance.buff_timer[getbuff];

        multi_buff.Add(buf);

        multi_buff.Sort(delegate (Buff A, Buff B)
        {
            if (A.number > B.number) return 1;
            else if (A.number < B.number) return -1;
            return 0;
        });

        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            BuffEffect(getbuff);

    }

    public void MultiGetBuff(int getbuff, GameObject player)
    {
        Buff buf = new Buff();
        buf.number = getbuff;
        buf.timer = SkillManager.instance.buff_timer[getbuff];

        multi_buff.Add(buf);

        multi_buff.Sort(delegate (Buff A, Buff B)
        {
            if (A.number > B.number) return 1;
            else if (A.number < B.number) return -1;
            return 0;
        });

        Debug.Log(player.GetComponent<CharacterStat>().My_Index);

        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            BuffEffect(getbuff, player);

    }

    private void BuffEffect(int Buffnum)
    {
        switch(Buffnum)
        {
            case 0: //연계 
                break;
            case 1: //방감
                this.PhysicalArmor -= Armor_Debuf;
                break;
            case 2://독
                break;
            case 3://Vanish
                break;
            case 4: //이속감소
                this.moveSpeed -= Move_Debuf;
                break;
            case 5://공격력감소
                this.PhysicalAttackPower -= 10;
                this.MagicAttackPower -= 10;
                break;
            case 6: //창 도트딜
                break;
            case 7: //도발                
                break;
            case 8: //분노
                GetAngry();
                break;
        }
    }

    private void BuffEffect(int Buffnum,GameObject player)
    {
        switch (Buffnum)
        {
            case 0: //연계 
                break;
            case 1: //방감
                this.PhysicalArmor -= Armor_Debuf;
                break;
            case 2://독
                StartCoroutine(Poison(player));
                break;
            case 3://Vanish
                break;
            case 4: //이속감소
                break;
            case 6: //창 도트딜
                StartCoroutine(Bleed(player));
                break;
            case 7: //도발
                StartCoroutine(Taunt(player));
                break;
        }
    }

    IEnumerator Poison(GameObject player)
    {
        WaitForSeconds Poison_Tic;
        Poison_Tic = new WaitForSeconds(0.5f);
        float tic = 0.5f;
        int timer = (int)(SkillManager.instance.buff_timer[2] / tic);
        yield return new WaitForSeconds(0.1f);
        for(int i=0;i<timer;i++)
        {
            if (HP <= 0) break;
            BossGetSkillDamage(20+player.GetComponent<CharacterStat>().MagicAttackPower*0.1f,true, player.gameObject);
            yield return Poison_Tic;
        }
    }

    IEnumerator Bleed(GameObject player)
    {
        WaitForSeconds Bleed_Tic;
        Bleed_Tic = new WaitForSeconds(0.5f);
        float Tic = 0.5f;
        int timer = (int)(SkillManager.instance.buff_timer[6] / Tic);
        yield return Bleed_Tic;
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < timer; i++)
        {
            if (HP <= 0) break;
            BossGetSkillDamage(player.GetComponent<CharacterStat>().PhysicalAttackPower * 0.1f, false, player.gameObject);
            yield return Bleed_Tic;
        }
    }

    IEnumerator Taunt(GameObject shield)
    {   
        for (int i = 0; i < player_num; i++)
        {
            taunt = 0;
            if (GameManage.instance.player[0].gameObject == shield)
            {
                taunt = 9999;
            }
        }
        yield return new WaitForSeconds(SkillManager.instance.buff_timer[7]);
        for (int i = 0; i < player_num; i++)
        {
            if (GameManage.instance.player[0] == shield)
            {
                taunt = 0;
            }
        }
    }

    private void BuffEffectDelete(int buf)
    {
        switch (buf)
        {
            case 0:
                break;
            case 1:
                this.PhysicalArmor += Armor_Debuf;
                break;
            case 2:
                break;
            case 4:
                this.moveSpeed += Move_Debuf;
                break;
            case 5:
                this.PhysicalAttackPower += 10;
                this.MagicAttackPower += 10;
                break;

        }
    }

    public float ReturnAggro()
    {
        return this.Aggro;
    }

    private void Update()
    {
        if (NetworkManager.instance.is_multi)
        {
            for (int i = 0; i < multi_buff.Count; i++)
            {
                multi_buff[i].timer -= Time.deltaTime;
                if (multi_buff[i].timer <= 0)
                {
                    BuffEffectDelete(multi_buff[i].number);
                    multi_buff.RemoveAt(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < buff.Count; i++)
            {
                buff[i].timer -= Time.deltaTime;
                if (buff[i].timer <= 0)
                {
                    BuffEffectDelete(buff[i].number);
                    buff.RemoveAt(i);
                }
            }
        }


        //Debug.Log(target_player.name);
    }


    IEnumerator Hit()
    {        
        hit_prefab_temp.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hit_prefab_temp.SetActive(false);
    }

    IEnumerator CalAggro()
    {
        while (true)
        {
            BossGetAggro();
            for(int i = 0; i < player_num; i++)
            {
                if (player[0].tag == "DeadPlayer" || player[0].tag == "Abnormal")
                {
                    Aggro = -1000;
                    continue;
                }

                if (NetworkManager.instance.is_multi)
                {
                    Aggro = _distance + taunt + (GameManage.instance.Multi_Damage_toBoss[NetworkManager.instance.my_index,0] + GameManage.instance.Multi_Damage_toBoss[NetworkManager.instance.my_index, 1]) / 10 + (GameManage.instance.Multi_Heal[NetworkManager.instance.my_index, 0]+ GameManage.instance.Multi_Heal[NetworkManager.instance.my_index, 1]) / 5;

                    NetworkManager.instance.EmitAggro(Aggro);
                }
                //else
                //{
                //    Aggro = _distance + taunt +(GameManage.instance.Damage_toBoss[0] + GameManage.instance.Damage_toBoss[0]) / 10 + (GameManage.instance.Multi_Heal[NetworkManager.instance.my_index, 0] + GameManage.instance.Multi_Heal[NetworkManager.instance.my_index, 1]) / 5;
                //}

            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator Fadeout()
    {
        Destroy(this.GetComponentInChildren<Shadow>().gameObject);
        this.tag = "DeadPlayer";
        BossBuffClear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0;i< players.Length;i++)
        {
            players[i].GetComponent<Character_Control>().RunningStop();
        }
        while (true)
        {
            if (this.GetComponent<SpriteRenderer>().color.a <= 0) yield return 0;
            else
            {
                temp_color = this.GetComponent<SpriteRenderer>().color;
                temp_color.a -= 0.1f;
                this.GetComponent<SpriteRenderer>().color = temp_color;
                yield return new WaitForSeconds(0.1f);
            }
        }

    }

    private void GetAngry()
    {
        this.PhysicalAttackPower *= 1.5f;
        this.MagicAttackPower *= 1.5f;
        Angry_temp = Instantiate(Angry_Effect, this.gameObject.transform);
        Destroy(Angry_temp, 2f);
        Color color = this.gameObject.GetComponent<SpriteRenderer>().color;
        color.g -= 0.3f;
        color.b -= 0.3f;
        if(color.g>0.6)
            this.gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    public void StartCalAggro()
    {
        StartCoroutine(CalAggro());
    }
}
