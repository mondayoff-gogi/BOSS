using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Around_1 : MonoBehaviour
{
    private CircleCollider2D trigger;

    public GameObject hit_efffect;
    private GameObject hit_temp;
    private Vector3 boss_poss;
    
    private float physical_attak;
    // Start is called before the first frame update
    void Start()
    {
        trigger = this.GetComponent<CircleCollider2D>();
        physical_attak = this.transform.parent.parent.GetComponent<BossStatus>().PhysicalAttackPower;
        boss_poss = this.transform.parent.gameObject.transform.position;
        trigger.radius += 2.5f;
        Destroy(this.gameObject, 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hit_temp = Instantiate(hit_efffect, collision.transform);          

            collision.GetComponent<CharacterStat>().GetDamage(physical_attak * 10f/100f, false);
            BossStatus.instance.MP += 10f;
            Destroy(hit_temp, 0.5f);
        }
    }
}
