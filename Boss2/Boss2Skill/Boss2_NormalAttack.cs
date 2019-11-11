using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_NormalAttack : MonoBehaviour
{
    private float attack_power;

    // Start is called before the first frame update
    void Start()
    {
        if(BossStatus.instance)
        {
            attack_power = BossStatus.instance.PhysicalAttackPower * 0.7f;
        }
        Destroy(this.gameObject, 0.2f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.gameObject.GetComponent<CharacterStat>().GetDamage(attack_power, false);
        }

    }


}
