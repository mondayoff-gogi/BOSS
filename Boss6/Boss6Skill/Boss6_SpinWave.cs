using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_SpinWave : MonoBehaviour
{
    private Vector2 dir;
    private Vector2 dir_plus;
    private float movespeed;
    
    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;

    private float damage_amount;

    private float timer;

    void Start()
    {
        timer = 0f;
        movespeed = 8f;
        dir = new Vector2(1, 0);
        damage_amount = BossStatus.instance.MagicAttackPower;
        Destroy(this.gameObject, 10f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < 6)
        {            
            dir_plus = new Vector2(dir.y, -dir.x);
            dir_plus *= 0.03f;
            dir += dir_plus;

            dir.Normalize();
            this.transform.Translate(dir * Time.deltaTime * movespeed);
        }
        else if (timer < 8)
        {
            movespeed = 10f;

            dir_plus = new Vector2(dir.y, -dir.x);
            dir_plus *= 0.04f;
            dir += dir_plus;

            dir.Normalize();
            this.transform.Translate(dir * Time.deltaTime * movespeed);
        }
        else
        {
            dir_plus = new Vector2(dir.y, -dir.x);
            dir_plus *= 0.01f;
            dir += dir_plus;

            dir.Normalize();
            this.transform.Translate(dir * Time.deltaTime * movespeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.instance.Play(47);
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount, false);
            ExplosionEffect();
        }
    }
    private void ExplosionEffect()
    {

        explosionEffect_temp = Instantiate(explosionEffect, this.transform.position,Quaternion.identity);
        explosionEffect_temp.transform.localScale = new Vector2(4, 4);
        Destroy(explosionEffect_temp, 2f);
    }
}
