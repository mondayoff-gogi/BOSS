using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Hammer : MonoBehaviour
{
    private float damage_amount;
    private float flag = 0;

    // Start is called before the first frame update
    void Start()
    {
        damage_amount = BossStatus.instance.MagicAttackPower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * flag, true);
        }
    }

    public void GetMult(int temp)
    {
        flag = temp;
    }
}
