using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss8_obj : MonoBehaviour
{
    public GameObject destroy_effect;

    private GameObject destroy_temp;
    private float damage_amount;
    private Vector2 dir;
    private GameObject boss;

    private float DamageGive;

    private bool flag;

    private void Start()
    {
        boss = BossStatus.instance.gameObject;
        damage_amount = BossStatus.instance.MagicAttackPower;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float percent = Random.Range(0f, 100.0f);
            DamageGive = collision.GetComponent<CharacterStat>().GetDamage(damage_amount, true);
            destroy_temp = Instantiate(destroy_effect, collision.transform);
            Destroy(destroy_temp, 0.5f);
            Destroy(this.gameObject);
        }
    }
}
