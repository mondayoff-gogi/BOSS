using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerSkill3 : MonoBehaviour
{
    // 300 + 마법 공격력 * 1.2
    private GameObject thisparent;
    private float magic_attack = 0;
    
    public GameObject healing_effect;
    private GameObject healing_temp;
    private float multiple = 1.2f;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play(29);

        thisparent = this.transform.parent.GetComponentInParent<HealerSkill1_2>().thisparent;
        magic_attack = thisparent.GetComponent<CharacterStat>().MagicAttackPower;
        Destroy(this.gameObject, 0.2f);
        //this.transform.SetParent(this.transform.parent.parent);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            SoundManager.instance.Play(30);

            healing_temp = Instantiate(healing_effect, collision.transform);
            Destroy(healing_temp, 0.5f);

            if(thisparent.transform.parent!=null)
            {
                float temp = Random.Range(1.0f, 100.0f);
                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage((300 + magic_attack * multiple), true, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage((300 + magic_attack * multiple), true, thisparent.gameObject);
                }
            }
        }
        if (collision.CompareTag("Monster"))
        {
            SoundManager.instance.Play(30);

            healing_temp = Instantiate(healing_effect, collision.transform);
            Destroy(healing_temp, 0.5f);
            if (thisparent.transform.parent != null)
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage((300 + magic_attack * multiple));
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage((300 + magic_attack * multiple));
                }
            }
        }
    }
}
