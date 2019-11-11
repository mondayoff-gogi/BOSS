using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerSkill2_2 : MonoBehaviour
{
    // 마법 공격력의 0.2퍼 + 캐릭터 맥스 hp의 5%
    private float healing_amount = 0;
    private CircleCollider2D _col;
    public GameObject healing_effect;
    private GameObject healing_temp;
    public GameObject thisparent;
    public float hp_percent = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        healing_amount = this.transform.parent.GetComponentInParent<HealerSkill1_2>().heal_amount;
        thisparent = this.transform.parent.GetComponentInParent<HealerSkill1_2>().thisparent;
        Destroy(this.gameObject, 0.1f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            healing_temp = Instantiate(healing_effect, collision.transform);
            Destroy(healing_temp, 0.5f);
            float temp = Random.Range(1.0f, 100.0f);
            if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
            {
                collision.GetComponent<CharacterStat>().GetCriticalHeal((collision.GetComponent<CharacterStat>().MaxHP * hp_percent + healing_amount), thisparent,true);
            }
            else
            {
                collision.GetComponent<CharacterStat>().GetHeal((collision.GetComponent<CharacterStat>().MaxHP * hp_percent + healing_amount), thisparent,true);
            }
        }
        else if(collision.CompareTag("OtherPlayer"))
        {
            healing_temp = Instantiate(healing_effect, collision.transform);
            Destroy(healing_temp, 0.5f);
        }
    }
}
