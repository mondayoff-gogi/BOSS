using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_0_2 : MonoBehaviour
{
    private float physical_attack;
    private GameObject skill_prefab;

    public GameObject hit_effect;
    private GameObject hit_temp;
    // Start is called before the first frame update
    void Start()
    {
        skill_prefab = this.transform.parent.parent.parent.gameObject;
        // TODO DesertBoss Skill0번 데미지 공식 변경 바람
        physical_attack = skill_prefab.GetComponent<Skill_0_3>().physical_attack;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hit_temp = Instantiate(hit_effect, collision.transform);
            Destroy(hit_temp, 0.5f);
            collision.GetComponent<CharacterStat>().GetDamage(physical_attack * 5f/100f, false);
            BossStatus.instance.MP += 5;
            Destroy(this.transform.parent.gameObject);
        }
    }
}
