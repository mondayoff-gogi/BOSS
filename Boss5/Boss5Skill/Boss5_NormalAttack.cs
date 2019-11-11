using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_NormalAttack : MonoBehaviour
{
    private Vector2 dir;
    private float movespeed;

    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;

    private float damage_amount;

    void Start()
    {
        dir = new Vector2(1, 0);
        movespeed = UpLoadData.boss_level + 20f; ;//4
        damage_amount = BossStatus.instance.PhysicalAttackPower;
        Destroy(this.gameObject, 3f);
    }

    void Update()
    {
        this.transform.Translate(dir * Time.deltaTime * movespeed/*, Space.World*/);
        if(movespeed>2)
        {
            movespeed -= Time.deltaTime * 20f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount, false);
            ExplosionEffect();
            Destroy(this.gameObject);
        }
    }
    private void ExplosionEffect()
    {
        explosionEffect_temp = Instantiate(explosionEffect, this.transform);
        explosionEffect_temp.transform.SetParent(this.transform.parent);
        Destroy(explosionEffect_temp, 2f);
    }
}
