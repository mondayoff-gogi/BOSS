using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSkill3_2 : MonoBehaviour
{
    private Vector3 Empty_position;
    private Vector3 firball_position;

    private GameObject firball;
    private Vector3 dir;

    private float speed = 0.8f;

    private float dx;
    private float dy;

    public GameObject hit_effect;
    private GameObject hit_temp;

    private Transform player;
    private Vector3 hit_effect_position;

    private float magical_attack;

    private float multiple = 1.2f;

    private void Start()
    {
        Empty_position = this.gameObject.transform.parent.parent.GetComponent<Transform>().position;
        firball_position = this.gameObject.transform.parent.GetComponent<Transform>().position;
        firball = this.transform.parent.gameObject;
        player = this.transform.parent.parent.gameObject.GetComponent<MageSkill3_3>().player;
        magical_attack = player.GetComponent<CharacterStat>().MagicAttackPower;

        if(this.transform.parent.parent.parent!=null)
            firball.transform.SetParent(firball.transform.parent.parent.parent);
        else
            firball.transform.SetParent(null);
        //parent.transform.SetParent(parent.transform.parent);
        dx = Empty_position.x - firball_position.x;
        dy = Empty_position.y - firball_position.y;
        
        Destroy(firball, 2f);
        dir = new Vector3(dx, dy, 0);
    }

    private void Update()
    {
        firball.transform.Translate(-dir * speed * Time.deltaTime, Space.World);
        speed += 0.05f;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            SoundManager.instance.Play(32);
            if (this.transform.parent.parent != null)
            {
                // 100 + 마법 공격력 * 1.2
                if (Random.Range(1.0f, 100.0f) <= player.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<BossStatus>().BossGetCriticalDamage((100 + magical_attack * multiple), true, player.gameObject);
                }
                else
                {
                    collision.GetComponent<BossStatus>().BossGetSkillDamage(100 + magical_attack * multiple, true, player.gameObject);
                }
            }
            hit_effect_position = collision.transform.position;
            hit_effect_position.x = hit_effect_position.x + Random.Range(-1, 1);
            hit_effect_position.y = hit_effect_position.y + Random.Range(0, 3);
            hit_temp = Instantiate(hit_effect, hit_effect_position, Quaternion.identity);
            hit_temp.transform.SetParent(collision.transform);
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(132, hit_temp.transform, 1f);
            }
            Destroy(hit_temp, 1f);
            Destroy(firball);
        }

        if (collision.CompareTag("Monster"))
        {
            if (this.transform.parent.parent.parent != null)
            {
                // 100 + 마법 공격력 * 1.2
                if (Random.Range(1.0f, 100.0f) <= player.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage((100 + magical_attack * multiple) * 2.5f);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(100 + magical_attack * multiple);
                }
            }
            hit_effect_position = collision.transform.position;
            hit_effect_position.x = hit_effect_position.x + Random.Range(-1, 1);
            hit_effect_position.y = hit_effect_position.y + Random.Range(0, 1);
            hit_temp = Instantiate(hit_effect, hit_effect_position, Quaternion.identity);
            hit_temp.transform.SetParent(collision.transform);
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(132, hit_temp.transform, 1f);
            }
            Destroy(hit_temp, 1f);
            Destroy(firball);
        }
    }

}
