using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_RotateMissile : MonoBehaviour
{
    public GameObject destroy_effect;
    private GameObject destroy_temp;

    public GameObject warning_effect;
    private GameObject warning_temp;

    private GameObject target;
    private GameObject boss;
    private float damage_amount;
    private Vector2 dir;
    private float movespeed;
    private bool arrive = false;
    private bool count_bool = false;
    private float running_time;
    private float x;
    private float y;
    private float Angle_add;
    private float radius = 5f;
    private Vector2 newPos;
    private float time;
    private float movetime;
    private int count = 0;
    private Vector2 temp;
    private Vector2 new_dir;
    private bool flag;
    private bool is_create = true;
    private float multiple = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        flag = false;
        boss = BossStatus.instance.gameObject;
        damage_amount = boss.GetComponent<BossStatus>().MagicAttackPower;
        dir = target.transform.position - this.transform.position;
        StartCoroutine(GetPlayerPosition());
        if (UpLoadData.boss_level < 2)
            movespeed = 2f;
        else
            movespeed = 4f;
    }

    // Update is called once per frame
    void Update()
    {
        movetime += Time.deltaTime;
        if (!arrive)
        {
            this.transform.Translate(dir * movespeed * Time.deltaTime);
        }
        if (Vector2.Distance(this.transform.position, target.transform.position) < radius)
        {
            arrive = true;
        }
        if (arrive && (count < 3))
        {
            time += Time.deltaTime;
            running_time += Time.deltaTime * movespeed;

            if(!flag)
            {
                flag = true;
                if(this.transform.position.y - target.transform.position.y<0)
                {
                    Angle_add = -Mathf.Acos((this.transform.position.x - target.transform.position.x) / radius); //rad으로 계산함
                }
                else
                    Angle_add = Mathf.Acos((this.transform.position.x - target.transform.position.x) / radius); //rad으로 계산함
            }
            x = radius * Mathf.Cos(running_time+ Angle_add);
            y = radius * Mathf.Sin(running_time+ Angle_add);
            newPos = new Vector2(target.transform.position.x + x, target.transform.position.y + y);  //rad으로 계산함
            this.transform.position = newPos;
            if(time >= 2f)
            {
                radius -= 0.5f;
                time = 0;
                count += 1;
            }
            if (count == 3)
                count_bool = true;
        }
        if (count_bool)
        {
            running_time += Time.deltaTime * movespeed;
            x = radius * Mathf.Cos(running_time);
            y = radius * Mathf.Sin(running_time);
            radius = 2;
            newPos = new Vector2(temp.x + x, temp.y + y);
            this.transform.position = newPos;
        }
        if(movetime > 6f)
        {
            if (is_create)
            {
                warning_temp = Instantiate(warning_effect, temp, Quaternion.identity);
                Destroy(warning_temp, 0.5f);
                is_create = false;
            }

        }
        if (movetime > 8f)
        {
            count_bool = false;
        }
        if (!count_bool && count == 3)
        {
            new_dir = temp - (Vector2)this.transform.position;
            new_dir.Normalize();
            this.transform.Translate(new_dir * movespeed * Time.deltaTime * 5);
            if(Vector2.Distance(temp, this.transform.position) < 0.3f)
            {
                destroy_temp = Instantiate(destroy_effect, this.transform.position, Quaternion.identity);
                Destroy(destroy_temp, 0.5f);
                Destroy(this.gameObject);
            }
        }
    }

    public void GetTargetPosition(GameObject player)
    {
        target = player;
    }

    IEnumerator GetPlayerPosition()
    {
        yield return new WaitForSeconds(6f);
        temp = target.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, false);
            destroy_temp = Instantiate(destroy_effect, this.transform.position, Quaternion.identity);
            Destroy(destroy_temp, 0.5f);
            Destroy(this.gameObject);
        }
    }
}
