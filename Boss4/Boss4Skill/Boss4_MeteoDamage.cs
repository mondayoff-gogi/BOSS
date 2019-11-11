using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_MeteoDamage : MonoBehaviour
{
    private float damage_amount;
    private float multiple;
    private CircleCollider2D _col;
    // Start is called before the first frame update
    void Start()
    {
        _col = this.GetComponent<CircleCollider2D>();
        damage_amount = BossStatus.instance.PhysicalAttackPower;
        Invoke("DisableCol", 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, false);
        }
    }

    public void GetMulti(float temp)
    {
        multiple = temp;
    }

    private void DisableCol()
    {
        _col.enabled = false;
    }
}
