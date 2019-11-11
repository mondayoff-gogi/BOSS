using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_Fire : MonoBehaviour
{
    private float attack_amount;
    private GameObject boss;

    public GameObject hit_effect;
    private GameObject hit_temp;
    private Vector3 hit_effect_position;
    // Start is called before the first frame update
    void Start()
    {
        boss = BossStatus.instance.gameObject;
        attack_amount = boss.GetComponent<BossStatus>().MagicAttackPower;
        Destroy(this.gameObject, 10f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(GiveDamage(collision));
            StartCoroutine(HitEffct(collision));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
        }
    }

    IEnumerator GiveDamage(Collider2D collision)
    {
        while (true)
        {
            // TODO 공식 수정 요망
            if (collision.CompareTag("Player"))
                collision.GetComponent<CharacterStat>().GetDamage(attack_amount * 1.6f, true);

            yield return new WaitForSeconds(0.5f);
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
            hit_temp.transform.SetParent(collision.transform);
            Destroy(hit_temp, 1f);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
