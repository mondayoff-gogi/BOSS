using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningMagic : MonoBehaviour
{
    private Transform thisparent; //케릭터
    private const float multiple = 1.8f;

    void Start()
    {
        SoundManager.instance.Play(3);

        thisparent = this.gameObject.transform.parent;
        StartCoroutine(ColliderDelete());
        if (thisparent!=null)
        {
            StartCoroutine(Delay());
            this.gameObject.transform.SetParent(thisparent.parent);
        }
        Destroy(this.gameObject,2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (thisparent != null)
        {
            if (collision.CompareTag("Boss"))
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage(210 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * multiple, true, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(210 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * multiple, true, thisparent.gameObject);
                }
            }
            if (collision.CompareTag("Monster"))
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(210 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * multiple);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(210 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * multiple);
                }
            }
        }
    }

    IEnumerator ColliderDelete()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Collider2D>().enabled = false;
    }
    IEnumerator Delay()
    {
        thisparent.GetComponent<Character_Control>().Runable = false;
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", true);
        yield return new WaitForSeconds(0.1f);
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
        thisparent.GetComponent<Character_Control>().Runable = true;
    }
}
