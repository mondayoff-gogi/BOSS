using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Thunder : MonoBehaviour
{
    private float damage_amount;

    // Start is called before the first frame update
    void Start()
    {
        damage_amount = BossStatus.instance.MagicAttackPower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * 2f, true);
        }
    }
}
