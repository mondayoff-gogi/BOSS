using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_Bazooka : MonoBehaviour
{
    private Vector2 dir;
    private Vector2 dir_plus;
    private float movespeed;

    private GameObject target_player;

    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;
    private float damage_amount;
    private float multiple = 1.5f;
    private float GetFaster=0f;

    void Start()
    {
        target_player = BossStatus.instance.GetComponent<Boss5Move>().temp_target_player;
        movespeed = 2f;        
        this.transform.SetParent(this.transform.parent.parent);
        dir = target_player.transform.position - this.transform.position;
        dir.Normalize();
        damage_amount = BossStatus.instance.PhysicalAttackPower;
        Destroy(this.gameObject, 6f);
    }

    void Update()
    {
        GetFaster += Time.deltaTime*2;
        movespeed += GetFaster;
        dir_plus = target_player.transform.position-this.transform.position;

        dir_plus.Normalize();
        dir_plus *= 0.02f;
        dir += dir_plus;

        dir.Normalize();

        this.transform.Translate(dir * Time.deltaTime * movespeed, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, false);
            ExplosionEffect();
            Destroy(this.gameObject);
        }
        if(collision.CompareTag("Boundary"))
        {
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
