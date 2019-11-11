using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSkill2 : MonoBehaviour
{
    private float magical_attack;
    private float existing_time = 5f;
    private GameObject this_parent;
    private float shield_hp;
    private Transform player;
    private GameObject target_player;
    // 계수 변경 요망
    private float multiple = 2f;
    public GameObject hit_effect;
    private GameObject hit_temp;
    private Vector3 hit_effect_position;
    // Start is called before the first frame update
    void Start()
    {        
        player = this.transform.parent.transform;
        magical_attack = player.GetComponent<CharacterStat>().MagicAttackPower;
        // 300 + 마법 공격력 * 1.2
        shield_hp = 200 + magical_attack * multiple;        
        Destroy(this.gameObject, existing_time);        
    }

    // Update is called once per frame
    void Update()
    {
        if(shield_hp <= 0)
        {
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.DeleteObject(0); //0번이 메이지2번
            }
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.transform.parent.parent != null)
        {
            if (collision.CompareTag("Boss"))
            {
                StartCoroutine(GiveDamageToBoss(collision));
            }

            if (collision.CompareTag("Monster"))
            {
                StartCoroutine(GiveDamageToMonster(collision));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (this.transform.parent.parent != null)
        {
            if (collision.CompareTag("Boss"))
            {
                StopCoroutine(GiveDamageToBoss(collision));
            }

            if (collision.CompareTag("Monster"))
            {
                StopCoroutine(GiveDamageToMonster(collision));
            }
        }
    }
    public void GetShieldDamage(float damage)
    {
        shield_hp -= damage;
    }

    IEnumerator GiveDamageToBoss(Collider2D collision)
    {
        while (true)
        {
            // ((마법 공격력 * 0.8) + 보스 일반 공격력 * 0.1)/5
            collision.GetComponent<BossStatus>().BossGetSkillDamage((magical_attack * 1.5f), true, player.gameObject); ;
            hit_effect_position = collision.transform.position;
            hit_effect_position.x = hit_effect_position.x + Random.Range(-1, 1);
            hit_effect_position.y = hit_effect_position.y + Random.Range(0, 3);
            hit_temp = Instantiate(hit_effect, hit_effect_position, Quaternion.identity);
            hit_temp.transform.SetParent(collision.transform);
            Destroy(hit_temp, 1f);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator GiveDamageToMonster(Collider2D collision)
    {
        while (true)
        {
            // ((마법 공격력 * 0.8) + 보스 일반 공격력 * 0.1)/5
            collision.GetComponent<Monster>().GetDamage((magical_attack * 1.5f));
            hit_effect_position = collision.transform.position;
            hit_effect_position.x = hit_effect_position.x + Random.Range(-1, 1);
            hit_effect_position.y = hit_effect_position.y + Random.Range(0, 1);
            hit_temp = Instantiate(hit_effect, hit_effect_position, Quaternion.identity);
            hit_temp.transform.SetParent(collision.transform);
            Destroy(hit_temp, 1f);
            yield return new WaitForSeconds(1f);
        }
    }

    public void GetTargetPlayer(GameObject player)
    {
        target_player = player;
        if(target_player!=null)
            this.transform.SetParent(target_player.transform);
    }
}
