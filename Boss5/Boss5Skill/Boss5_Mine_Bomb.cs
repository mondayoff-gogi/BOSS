using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_Mine_Bomb : MonoBehaviour
{
    private GameObject boss;
    private float damage_amount;
    private float multiple = 1.2f;
    // Start is called before the first frame update
    void Start()
    {
        boss = BossStatus.instance.gameObject;
        damage_amount = boss.GetComponent<BossStatus>().PhysicalAttackPower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, false);
        }
    }
}
