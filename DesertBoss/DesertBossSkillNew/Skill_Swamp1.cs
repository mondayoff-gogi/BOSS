using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Swamp1 : MonoBehaviour
{
    public float multiple;
    private float magical_attack;
    private GameObject parentobject;
    public GameObject hit_effect;
    private GameObject hit_temp;
    private Vector3 hit_effect_position;
    private Vector3 dir;

    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        parentobject = this.transform.parent.gameObject;
        magical_attack = BossStatus.instance.MagicAttackPower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(GiveDamage(collision));
            StartCoroutine(HitEffct(collision));
        }

        if(collision.GetComponent<DesertSkill_Spread>() != null)
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dir.x = this.transform.position.x - collision.transform.position.x;
            dir.y = this.transform.position.y - collision.transform.position.y;
            dir.z = 0;
            dir.Normalize();
            StartCoroutine(GiveDamage(collision));
            collision.transform.Translate(dir * speed * Time.deltaTime);
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
            if(collision.CompareTag("Player"))
            collision.GetComponent<CharacterStat>().GetDamage(magical_attack * multiple, true);


            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator HitEffct(Collider2D collision)
    {
        while (true)
        {
            hit_effect_position = collision.transform.position;
            hit_effect_position.y += 1f;
            hit_temp = Instantiate(hit_effect, hit_effect_position, Quaternion.identity);
            hit_temp.transform.SetParent(collision.transform);
            Destroy(hit_temp, 1f);
            yield return new WaitForSeconds(1f);
        }
    }

}
