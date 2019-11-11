using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearSkill4 : MonoBehaviour
{
    private Animator _animator;

    private Vector3 dir;
    public float multiple = 1.2f;

    private float physical_attack;

    private Collider2D parent_collider;
    private CircleCollider2D this_collider;

    private GameObject character;

    public GameObject arrive_Effect;
    private GameObject[] arrive_temp;

    public GameObject hit_effect;
    private GameObject hit_temp;

    private int index;

    private bool flag;
    // Start is called before the first frame update
    void Start()
    {
        flag = false;
        _animator = gameObject.GetComponent<Animator>();
        _animator.enabled = false;
        arrive_temp = new GameObject[8];
        this_collider = this.GetComponent<CircleCollider2D>();
        this_collider.enabled = false;
        if (this.transform.parent!=null)
        {
            dir = this.transform.parent.GetComponent<Transform>().position;
            dir.z = this.transform.parent.GetComponent<Transform>().position.z;
            parent_collider = this.transform.parent.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), parent_collider);
            character = this.transform.parent.gameObject;
            physical_attack = character.GetComponent<CharacterStat>().PhysicalAttackPower;
            this.transform.SetParent(this.transform.parent.parent);
        }
        else
        {
            index = (int)this.transform.position.z + 777;
            dir = GameManage.instance.Character_other[index].transform.position;

        }
        Destroy(this.gameObject, 1.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Boss")
        {
            if (BossStatus.instance.SilentAble) //silent맞으면
            {
                BossStatus.instance.BossGetSilent();

                hit_temp = Instantiate(hit_effect, collision.transform);
                hit_temp.transform.SetParent(collision.transform);
                if (this.transform.parent != null)
                {

                    if (Random.Range(1.0f, 100.0f) <= character.GetComponent<CharacterStat>().ciritical)
                    {
                        BossStatus.instance.BossGetCriticalDamage((collision.gameObject.GetComponent<BossStatus>().MaxHP * 0.02f + physical_attack * multiple), false, character.gameObject);
                    }
                    else
                    {
                        collision.gameObject.GetComponent<BossStatus>().BossGetSkillDamage(collision.gameObject.GetComponent<BossStatus>().MaxHP * 0.02f + physical_attack * multiple, false, character.gameObject);
                    }
                }
                Destroy(hit_temp, 0.5f);
            }
            else
            {
                hit_temp = Instantiate(hit_effect, collision.transform);
                hit_temp.transform.SetParent(collision.transform);
                if (this.transform.parent != null)
                {
                    if (Random.Range(1.0f, 100.0f) <= character.GetComponent<CharacterStat>().ciritical)
                    {
                        BossStatus.instance.BossGetCriticalDamage((collision.gameObject.GetComponent<BossStatus>().MaxHP * 0.02f + physical_attack * multiple), false, character.gameObject);
                    }
                    else
                    {
                        collision.gameObject.GetComponent<BossStatus>().BossGetSkillDamage(collision.gameObject.GetComponent<BossStatus>().MaxHP * 0.02f + physical_attack * multiple, false, character.gameObject);
                    }
                }
                Destroy(hit_temp, 0.5f);
            }
        }

        if (collision.gameObject.tag == "Monster")
        {
            hit_temp = Instantiate(hit_effect, collision.transform);
            hit_temp.transform.SetParent(collision.transform);
            if (this.transform.parent != null)
            {
                if (Random.Range(1.0f, 100.0f) <= character.GetComponent<CharacterStat>().ciritical)
                {
                    collision.gameObject.GetComponent<Monster>().GetCritialDamage(collision.gameObject.GetComponent<Monster>().MaxHP * 0.02f + physical_attack * multiple);
                }
                else
                {
                    collision.gameObject.GetComponent<Monster>().GetDamage(collision.gameObject.GetComponent<Monster>().MaxHP * 0.02f + physical_attack * multiple);
                }
            }
            Destroy(hit_temp, 0.5f);
        }
    }



    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Boss")
    //    {
    //        if (BossStatus.instance.SilentAble) //silent맞으면
    //        {
    //            BossStatus.instance.BossGetSilent();
    //            hit_temp = Instantiate(hit_effect, collision.transform);
    //            hit_temp.transform.SetParent(collision.transform);
    //            if (Random.Range(1.0f, 100.0f) <= character.GetComponent<CharacterStat>().ciritical)
    //            {
    //                BossStatus.instance.BossGetCriticalDamage((collision.gameObject.GetComponent<BossStatus>().MaxHP * 0.02f + physical_attack * multiple), false, character.gameObject);
    //            }
    //            else
    //            {
    //                collision.gameObject.GetComponent<BossStatus>().BossGetSkillDamage(collision.gameObject.GetComponent<BossStatus>().MaxHP * 0.02f + physical_attack * multiple, false, character.gameObject);
    //            }
    //            Destroy(hit_temp, 0.5f);
    //        }
    //        else
    //        {
    //            hit_temp = Instantiate(hit_effect, collision.transform);
    //            hit_temp.transform.SetParent(collision.transform);
    //            if (Random.Range(1.0f, 100.0f) <= character.GetComponent<CharacterStat>().ciritical)
    //            {
    //                BossStatus.instance.BossGetCriticalDamage((collision.gameObject.GetComponent<BossStatus>().MaxHP * 0.02f + physical_attack * multiple), false, character.gameObject);
    //            }
    //            else
    //            {
    //                collision.gameObject.GetComponent<BossStatus>().BossGetSkillDamage(collision.gameObject.GetComponent<BossStatus>().MaxHP * 0.02f + physical_attack * multiple, false, character.gameObject);
    //            }
    //            Destroy(hit_temp, 0.5f);
    //        }
    //    }

    //    if (collision.gameObject.tag == "Monster")
    //    {
    //        hit_temp = Instantiate(hit_effect, collision.transform);
    //        hit_temp.transform.SetParent(collision.transform);
    //        if (Random.Range(1.0f, 100.0f) <= character.GetComponent<CharacterStat>().ciritical)
    //        {
    //            collision.gameObject.GetComponent<Monster>().GetCritialDamage(collision.gameObject.GetComponent<Monster>().MaxHP * 0.02f + physical_attack * multiple);
    //        }
    //        else
    //        {
    //            collision.gameObject.GetComponent<Monster>().GetDamage(collision.gameObject.GetComponent<Monster>().MaxHP * 0.02f + physical_attack * multiple);
    //        }
    //        Destroy(hit_temp, 0.5f);
    //    }
    //}


    private void CreateSwamp(float radius, GameObject[] dust_array)
    {
        float x, y, z;
        z = 0f;

        float angle = 20f;
        
        for (int i = 0; i < 8; i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            angle += (360f / 8);
            dust_array[0] = Instantiate(arrive_Effect, new Vector3(x + this.gameObject.transform.position.x, y + this.gameObject.transform.position.y, z), Quaternion.identity) as GameObject;
            dust_array[0].transform.localScale *= 0.75f;
            StartCoroutine(DustVanish(dust_array[0]));
            Destroy(dust_array[0], 1f);
            dust_array[0].transform.SetParent(this.gameObject.transform);
        }
        return;
    }
    IEnumerator DustVanish(GameObject dust)
    {
        WaitForSeconds waittime;
        float time=0.01f;
        waittime = new WaitForSeconds(time);
        Vector2 dir = dust.transform.position - this.transform.position;
        dir.Normalize();
        float timer = 4;
        while(timer>0)
        {            
            dust.transform.Translate(dir*timer*Time.deltaTime);            
            timer -= 12*time;
            yield return waittime;
        }
        yield return 0;
    }
    private void Update()
    {
        this.transform.position = Vector2.Lerp(this.transform.position, dir, Time.deltaTime * 4f);

        if(Vector2.Distance(this.transform.position, dir) < 0.5f&& !flag)
        {
            flag = true;
            CreateSwamp(1, arrive_temp);
            _animator.enabled = true;
            this_collider.enabled = true;
            while (true)
            {
                if (this_collider.radius > 1.6f)
                {
                    break;
                }
                this_collider.radius += 0.01f;
            }
        }
    }
}
