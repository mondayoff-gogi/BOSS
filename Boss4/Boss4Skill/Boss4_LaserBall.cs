using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_LaserBall : MonoBehaviour
{
    private Vector2 dir;
    private float movespeed;

    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;

    void Start()
    {
        dir = new Vector2(1, 0);
        movespeed = 60f;
        //this.transform.SetParent(this.transform.parent.parent);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir * Time.deltaTime * movespeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BossAttack"))
        {
            //쾅 이펙트
            explosionEffect_temp = Instantiate(explosionEffect, collision.transform.position, Quaternion.identity);
            Destroy(explosionEffect_temp, 1f);
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }

        if (collision.CompareTag("Player"))
        {
            explosionEffect_temp = Instantiate(explosionEffect, collision.transform.position, Quaternion.identity);
            Destroy(explosionEffect_temp, 1f);
            collision.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.MagicAttackPower*5, true);

            Destroy(this.gameObject);
        }
        
    }



}
