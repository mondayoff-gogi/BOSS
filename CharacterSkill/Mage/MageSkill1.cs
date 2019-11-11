using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSkill1 : MonoBehaviour
{
    // 1초당 1회, 5초 지속, 50 + (마법공격력 / 5) * 1.2f
    private Transform player;
    private float magical_attack;
    private GameObject parentobject;
    public GameObject hit_effect;
    private GameObject hit_temp;
    private Vector3 hit_effect_position;
    private float multiple = 1.2f;
    private bool is_multi;
    // Start is called before the first frame update
    void Start()
    {
        
        SoundManager.instance.Play(30);

        parentobject = this.transform.parent.gameObject;

        if (parentobject.transform.parent!=null)
        {
            is_multi = false;
            player = this.transform.parent.parent.gameObject.transform;
            magical_attack = player.GetComponent<CharacterStat>().MagicAttackPower;
            parentobject.transform.SetParent(parentobject.transform.parent.parent.parent);
        }
        else
        {
            is_multi = true;
            parentobject.transform.SetParent(parentobject.transform.parent);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            if (!is_multi)
            {
                StartCoroutine(GiveDamage(collision));
            }
            StartCoroutine(HitEffct(collision));
        }

        if (collision.CompareTag("Monster"))
        {
            if (!is_multi)
            {
                StartCoroutine(GiveMonsterDamage(collision));
            }
            StartCoroutine(HitEffct(collision));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            if (!is_multi)
            {
                StopCoroutine(GiveDamage(collision));
            }
            StopCoroutine(HitEffct(collision));
        }
        if (collision.CompareTag("Monster"))
        {
            if (!is_multi)
            {
                StopCoroutine(GiveMonsterDamage(collision));
            }
            StopCoroutine(HitEffct(collision));
        }
    }

    IEnumerator GiveDamage(Collider2D collision)
    {
        while (true)
        {
            float temp = Random.Range(1.0f, 100.0f);
            if (temp <= player.GetComponent<CharacterStat>().ciritical && BossStatus.instance.gameObject != null)
            {
                collision.GetComponent<BossStatus>().BossGetCriticalDamage((50 + (magical_attack / 5) * multiple), true, player.gameObject);
            }
            else
            {
                collision.GetComponent<BossStatus>().BossGetSkillDamage(50 + (magical_attack / 5) * multiple, true, player.gameObject);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator GiveMonsterDamage(Collider2D collision)
    {
        while (true)
        {
            float temp = Random.Range(1.0f, 100.0f);
            if (temp <= player.GetComponent<CharacterStat>().ciritical)
            {
                collision.GetComponent<Monster>().GetCritialDamage((50 + (magical_attack / 5) * multiple));
            }
            else
            {
                collision.GetComponent<Monster>().GetDamage(50 + (magical_attack / 5) * multiple);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator HitEffct(Collider2D collision)
    {
        while (true)
        {
            if (GameManage.instance.IsGameEnd)
                break;
            hit_effect_position = collision.transform.position;
            hit_effect_position.x = hit_effect_position.x + Random.Range(-1, 1);
            hit_effect_position.y = hit_effect_position.y + Random.Range(0, 3);
            hit_temp = Instantiate(hit_effect, hit_effect_position, Quaternion.identity);
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(130, hit_temp.transform, 1);
            }
            hit_temp.transform.SetParent(collision.transform);
            Destroy(hit_temp, 1f);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
