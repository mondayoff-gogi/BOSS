using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    private Transform thisparent;

    private Vector2 dir;

    private float movespeed = 20f;
    private float movespeed_temp;
    private const float destroy_time = 0.5f;

    public GameObject skill;
    private GameObject skill_temp;
    public GameObject trail;
    private GameObject trail_temp;

    private Character_Control _character;

    void Start()
    {
        SoundManager.instance.Play(24);

        Destroy(this.gameObject, destroy_time);
        thisparent = this.gameObject.transform.parent;

        movespeed_temp = movespeed;

        if (thisparent!=null)
        {
            _character = thisparent.gameObject.GetComponent<Character_Control>();
            dir.x = thisparent.transform.position.x - SkillManager.instance.mouse_pos.x;
            dir.y = thisparent.transform.position.y - SkillManager.instance.mouse_pos.y;
            dir.Normalize();

            _character.RunningStop();
            _character._animator.SetFloat("DirX", -dir.x);
            _character._animator.SetFloat("DirY", -dir.y);

            trail_temp = Instantiate(trail, thisparent.transform);
        }
        else
        {
            trail_temp = Instantiate(trail, GameManage.instance.Character_other[777 + (int)this.transform.position.z].transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(thisparent!=null)
        {
            thisparent.Translate(-dir * Time.deltaTime * movespeed, Space.World);
            movespeed -= Time.deltaTime * (1 / destroy_time) * movespeed_temp;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(thisparent != null)
        {
            if (collision.CompareTag("Boss"))
            {
                SoundManager.instance.Play(20);

                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage(60 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 0.75f, false, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(60 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 0.75f, false, thisparent.gameObject);
                }
                skill_temp = Instantiate(skill, collision.transform);
                Destroy(skill_temp, 1f);
            }

            if (collision.CompareTag("Monster"))
            {
                SoundManager.instance.Play(20);

                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(60 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 0.75f);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(60 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 0.75f);
                }
                skill_temp = Instantiate(skill, collision.transform);
                Destroy(skill_temp, 1f);
            }
        }
        else
        {
            if (collision.CompareTag("Monster") ||collision.CompareTag("Boss"))
            {
                SoundManager.instance.Play(20);
                skill_temp = Instantiate(skill, collision.transform);
                Destroy(skill_temp, 1f);
            }
        }

    }
}
