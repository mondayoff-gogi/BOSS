using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_RandomExplostion : MonoBehaviour
{
    private float damage_amount;
    private CircleCollider2D _col;

    private void Start()
    {
        _col = this.GetComponent<CircleCollider2D>();
        damage_amount = BossStatus.instance.MagicAttackPower;
        Invoke("DeactivateCol", 0.1f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * 2f, true);
        }
    }

    private void DeactivateCol()
    {
        _col.enabled = false;
    }
}
