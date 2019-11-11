using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_NormalAttack : MonoBehaviour
{
    private void Start()
    {
        Invoke("DeleteCol", 0.1f);    
    }
    private void DeleteCol()
    {
        this.GetComponent<Collider2D>().enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.PhysicalAttackPower,false);
        }
    }
}
