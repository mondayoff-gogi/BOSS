using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSkill1 : MonoBehaviour
{
    // 200 + 물리 공격력 * 1.3f
    private float physical_attack;

    private GameObject player;

    public GameObject hit_effect;
    private GameObject hit_temp;
    private float multiple = 1.3f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 0.1f);
        if(this.transform.parent!=null)
        {
            physical_attack = player.GetComponent<CharacterStat>().PhysicalAttackPower;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            hit_temp = Instantiate(hit_effect, collision.transform);
            Destroy(hit_temp, 1f);
            if (this.transform.position.z > -774)
            {
                if (Random.Range(1.0f, 100.0f) <= player.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage(800 + physical_attack * multiple, false, player);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(800 + physical_attack * multiple, false, player);
                }
                BossStatus.instance.GetBuff(5);
            }
        }

        if (collision.CompareTag("Monster"))
        {
            hit_temp = Instantiate(hit_effect, collision.transform);
            Destroy(hit_temp, 1f);
            if (this.transform.parent != null)
            {
                if (Random.Range(1.0f, 100.0f) <= player.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage((800 + physical_attack * multiple));
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage((800 + physical_attack * multiple));
                }
            }
        }
    }

    public void GetPlayer(GameObject temp)
    {
        player = temp;
    }
}
