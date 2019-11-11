using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_PinBall1 : MonoBehaviour
{
    private Vector2 dir;



    private float movespeed;

    private int trigger_num;

    private float x, y;

    public GameObject effect;
    private GameObject effect_temp;


    void Start()
    {
        this.transform.SetParent(this.transform.parent.parent);

        if (this.transform.position.x >= 0)
        {
            if (this.transform.position.y >= 0) //1
            {
                x = Random.Range(-0.9f, -0.1f);
                y = Random.Range(-0.9f, -0.1f);
            }
            else //4
            {
                x = Random.Range(-0.9f, -0.1f);
                y = Random.Range(0.1f, 0.9f);
            }
        }
        else
        {
            if (this.transform.position.y >= 0) //2
            {
                x = Random.Range(0.1f, 0.9f);
                y = Random.Range(-0.9f, -0.1f);
            } 
            else //3
            {
                x = Random.Range(0.1f, 0.9f);
                y = Random.Range(0.1f, 0.9f);
            }
        }

        dir.x = x;
        dir.y = y;

        dir.Normalize();
        movespeed = 4f;

        trigger_num = 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir * Time.deltaTime * movespeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.MagicAttackPower, true);
            effect_temp = Instantiate(effect, collision.transform);
            SoundManager.instance.Play(74);
        }
        if (collision.CompareTag("Finish"))//좌우
        {
            SoundManager.instance.Play(31);

            trigger_num++;
            if (trigger_num > UpLoadData.boss_level/2+1)
                Destroy(this.gameObject);
            dir.x *= -1;           
        }
        if (collision.CompareTag("Respawn"))//상하
        {
            SoundManager.instance.Play(31);
            trigger_num++;
            if (trigger_num > UpLoadData.boss_level/2 + 1)
                Destroy(this.gameObject);           
            dir.y *= -1;
        }
    }
}
