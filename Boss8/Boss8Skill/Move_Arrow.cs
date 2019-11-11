using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Arrow : MonoBehaviour
{
    public GameObject start_effect;
    public GameObject hit_effect;

    private GameObject start_temp;
    private GameObject hit_temp;

    private float damage_amount;

    // Start is called before the first frame update
    void Start()
    {
        damage_amount = BossStatus.instance.MagicAttackPower;
        start_temp = Instantiate(start_effect, this.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hit_temp = Instantiate(hit_effect, collision.transform);
            Destroy(hit_temp, 0.5f);
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * 5f, true);
            Destroy(this.gameObject);
        }
    }
}
