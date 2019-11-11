using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_RushEffect : MonoBehaviour
{
    private void Start()
    {
        Invoke("DelCol",0.1f);
    }
    void DelCol()
    {
        this.GetComponent<Collider2D>().enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.PhysicalAttackPower, false);
        }
    }
}
