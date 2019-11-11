using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7NormalAttack : MonoBehaviour
{
    private Vector2 dir;
    private float dir_plus;
    private float movespeed;
    private float timer;
    private float width;
    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;
    private float damage_amount;
    public GameObject blood;
    private GameObject blood_temp;

    void Start()
    {
        dir = Vector2.right;
        movespeed = 3f;
        width = 3f;
        dir *= movespeed;
        timer = 0;
        damage_amount = BossStatus.instance.MagicAttackPower;
        Destroy(this.gameObject, 6f);
    }

    void Update()
    {
        dir_plus = width*Mathf.Cos(timer*2);
        dir.y = dir_plus;
        timer += Time.deltaTime;
        this.transform.Translate(dir*movespeed*Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount, false);
            blood_temp = Instantiate(blood, this.transform);
            ExplosionEffect();
            Destroy(this.gameObject);
        }
        if (collision.CompareTag("Boundary"))
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
