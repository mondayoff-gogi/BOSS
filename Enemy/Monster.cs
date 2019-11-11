using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int monster_num;

    public GameObject MonsterHPbar;

    [HideInInspector]
    public float HP = 100f;
    [HideInInspector]
    public float MaxHP = 100f;
    [HideInInspector]
    public float MP = 0f;
    [HideInInspector]
    public float MaxMp = 0f;
    [HideInInspector]
    public float PhysicalAttackPower = 0f;
    [HideInInspector]
    public float MagicAttackPower = 0f;
    [HideInInspector]
    public float PhysicalArmor = 0f;
    [HideInInspector]
    public float MagicArmorPower = 0f;
    [HideInInspector]
    public float monster_movespeed = 0.5f;
    [HideInInspector]
    public float attack_speed = 2.0f;

    private const float multiple = 0.02f;

    private GameObject boss;
    private Vector2 vec2;

    // Start is called before the first frame update
    void Start()
    {       
        MonsterHpbarSetActive();
        switch (monster_num)
        {
            case 0:
                MaxHP = BossStatus.instance.MaxHP / 96f;
                HP = MaxHP;
                break;
            case 1:
                MaxHP = BossStatus.instance.MaxHP * multiple;
                HP = MaxHP;
                break;
            case 2:
                MaxHP = BossStatus.instance.MaxHP/400f;
                HP = MaxHP;
                break;

        }

    }

    public void MonsterHpbarSetActive()
    {
        MonsterHPbar.SetActive(true);       
    }


    // Update is called once per frame



    public void GetCritialDamage(float _damage)
    {
        _damage *= 2.5f;
        _damage = (int)_damage;
        HP -= _damage;

        vec2 = this.transform.position;
        vec2.y -= 1;

        GameUI.instance.FloatingOrangeDamage(vec2, _damage);

        if (HP <= 0)//죽으면
        {
            MonsterHPbar.SetActive(false);

            switch (monster_num)
            {
                case 0:
                    //MonsterHpBar.GetComponent<MonsterHP>().HpDisalbe();
                    this.GetComponent<Boss2_Fire>().DestoryEffect();
                    break;
                case 1:
                    this.HP = MaxHP;

                    this.GetComponent<Boss3_Pile>().DestoryEffect();
                    break;
                case 2:
                    //MonsterHpBar.GetComponent<MonsterHP>().HpDisalbe();
                    this.GetComponent<Boss4_RollingRock>().DestoryEffect();
                    break;
            }
        }
    }

    public void GetDamage(float _damage)
    {
        _damage = (int)_damage;
        HP -= _damage;

        vec2 = this.transform.position;
        vec2.y -= 1;

        GameUI.instance.FloatingWhiteDamage(vec2, _damage);


        if (HP <= 0)//죽으면
        {
            MonsterHPbar.SetActive(false);

            switch (monster_num)
            {                
                case 0:
                    //MonsterHpBar.GetComponent<MonsterHP>().HpDisalbe();
                    this.GetComponent<Boss2_Fire>().DestoryEffect();
                    break;
                case 1:
                    this.HP = MaxHP;
                    this.GetComponent<Boss3_Pile>().DestoryEffect();
                    break;
                case 2:
                    //MonsterHpBar.GetComponent<MonsterHP>().HpDisalbe();
                    this.GetComponent<Boss4_RollingRock>().DestoryEffect();
                    break;
            }
        }
    }

    public void SetDeactiveHPBar()
    {
        MonsterHPbar.SetActive(false);
    }


}
