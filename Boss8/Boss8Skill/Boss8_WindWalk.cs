using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss8_WindWalk : MonoBehaviour
{
    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;
    private void Start()
    {
        Destroy(this.gameObject, 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.PhysicalAttackPower, false);
            ExplosionEffect(collision);
        }
    }
    private void ExplosionEffect(Collider2D player)
    {
        explosionEffect_temp = Instantiate(explosionEffect, player.gameObject.transform.position, Quaternion.identity);
        explosionEffect_temp.transform.localScale = new Vector2(2f, 2f);
        Destroy(explosionEffect_temp, 2f);
    }
}
