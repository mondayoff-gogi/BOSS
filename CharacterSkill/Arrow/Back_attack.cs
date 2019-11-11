using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back_attack : MonoBehaviour
{
    private Transform thisparent;

    private Vector2 dir;

    private float movespeed = 20f;
    private float movespeed_temp;
    private const float destroy_time = 0.6f;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play(24);
        Destroy(this.gameObject, destroy_time);
        movespeed_temp = movespeed;
        thisparent = this.gameObject.transform.parent;
        if (thisparent != null)
        {
            this.gameObject.transform.SetParent(thisparent.parent);
            dir.x = thisparent.gameObject.GetComponent<Character_Control>()._animator.GetFloat("DirX");
            dir.y = thisparent.gameObject.GetComponent<Character_Control>()._animator.GetFloat("DirY");
            thisparent.GetComponent<Character_Control>().RunningStop();
            dir.Normalize();        
            StartCoroutine(DeleteCollider());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (thisparent != null)
        {
            thisparent.Translate(-dir * Time.deltaTime * movespeed, Space.World);
            movespeed -= Time.deltaTime * (1 / destroy_time) * movespeed_temp;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss")&& thisparent!=null)
        {
            float temp = Random.Range(1.0f, 100.0f);

            if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
            {
                BossStatus.instance.BossGetCriticalDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 2.0f, false, thisparent.gameObject);
            }
            else
            {
                BossStatus.instance.BossGetSkillDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 2.0f, false, thisparent.gameObject);
            }

        }
        if (collision.CompareTag("Monster")&&thisparent != null)
        {
            float temp = Random.Range(1.0f, 100.0f);

            if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
            {
                collision.GetComponent<Monster>().GetCritialDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 2.0f);
            }
            else
            {
                collision.GetComponent<Monster>().GetDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 2.0f);
            }
        }
    }
    IEnumerator DeleteCollider()
    {
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<Collider2D>().enabled = false;
    }

}
