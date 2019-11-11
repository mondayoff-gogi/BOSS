using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Heal : MonoBehaviour
{
    private float damage_amount;
    private float flag = 3;

    // Start is called before the first frame update
    void Start()
    {
        damage_amount = BossStatus.instance.MagicAttackPower;
        BossStatus.instance.BossGetHeal(damage_amount * 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * flag, true);
        }
    }

}
