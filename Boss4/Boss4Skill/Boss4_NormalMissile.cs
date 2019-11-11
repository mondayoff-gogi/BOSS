using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_NormalMissile : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Delay());
        Destroy(this.gameObject, 1f);
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.PhysicalAttackPower*0.5f, false);
        }
    }
}
