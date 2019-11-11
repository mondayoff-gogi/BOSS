using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearSkill1 : MonoBehaviour
{
    private Transform thisparent;

    private Vector3 mouse_pos;
    private Vector3 this_position;

    public GameObject effect;
    private GameObject effect_temp;
    // 스킬 데미지 계수
    public float multiple = 1.3f;

    private float physical_attack;

    private Vector3 dir;

    private float dx;
    private float dy;

    private float speed = 20f;

    private float rotate_degree;


    private GameObject character;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play(22);

        thisparent = this.gameObject.transform.parent;

        if (thisparent != null)
        {
            mouse_pos = SkillManager.instance.mouse_pos;
            physical_attack = this.gameObject.GetComponentInParent<CharacterStat>().PhysicalAttackPower;

            character = this.transform.parent.gameObject;
            this.transform.parent = this.transform.parent.parent;

            mouse_pos.z = this.transform.position.z;
            this_position = this.transform.position;

            dx = mouse_pos.x - this_position.x;
            dy = mouse_pos.y - this_position.y;

            dir = (mouse_pos - this.transform.position).normalized;
        }
        else
        {
            dir = NetworkManager.instance.Skill_pos[777 + (int)this.transform.position.z];
            dir.x = dir.x - this.transform.position.x;
            dir.y = dir.y - this.transform.position.y;
            dir.Normalize();

            dx = dir.x;
            dy = dir.y;
        }
        rotate_degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;





        if (dx > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90 + rotate_degree);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 270 + rotate_degree);
        }


        Destroy(this.gameObject, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate( dir * speed * Time.deltaTime, Space.World);
        speed -= 0.5f;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            Vector3 position = this.transform.position;
            effect_temp=Instantiate(effect, position, Quaternion.identity);
            Destroy(effect_temp, 1f);
            if (thisparent != null)
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage((130 + physical_attack * multiple), false, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(130 + physical_attack * multiple, false, thisparent.gameObject);
                }

                BossStatus.instance.GetBuff(6, thisparent.gameObject);
            }
            Destroy(this.gameObject);

        }
        if (collision.CompareTag("Monster"))
        {
            Vector3 position = this.transform.position;
            effect_temp=Instantiate(effect, position, Quaternion.identity);
            Destroy(effect_temp, 1f);
            if (thisparent != null)
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage((130 + physical_attack * multiple));
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage((130 + physical_attack * multiple));
                }
            }
            Destroy(this.gameObject);
        }
    }
}
