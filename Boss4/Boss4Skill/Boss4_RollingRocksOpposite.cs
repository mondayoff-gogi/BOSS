using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_RollingRocksOpposite: MonoBehaviour
{
    private Vector2 dir;
    private float movespeed;
    private float rotatespeed;

    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;

    int flat = 0;

    private float timer;

    private int rand_flag;

    void Start()
    {
        rand_flag = BossStatus.instance.GetComponent<Boss4Move>().rand_flag;
        timer = 0;
        dir = new Vector2(1, 0);
        movespeed = UpLoadData.boss_level*0.5f +2.5f;
        rotatespeed = -400f;
        //this.transform.SetParent(this.transform.parent.parent);
        Destroy(this.gameObject, 10f);
        this.GetComponent<SpriteSorting>().pivot = -2;

        StartCoroutine(Delay());
    }
    IEnumerator Delay()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.1f);
        if(rand_flag==-1)
        {
            while (this.transform.position.x < 4)
            {
                yield return waittime;
            }
        }
        else
        {
            while (this.transform.position.x > -4)
            {
                yield return waittime;
            }
        }
        
        flat = 1;
    }
    // Update is called once per frame
    void Update()
    {
        if(flat==1)
            timer += Time.deltaTime;
        this.transform.Rotate(0, -Time.deltaTime * rotatespeed, 0);
        //if (this.transform.rotation.x >= 90)
        //    this.transform.Rotate(0, 180, 180);

        this.transform.Translate(dir * (Time.deltaTime * movespeed- timer*0.1f* flat)*(-rand_flag), Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.MagicAttackPower*5, true);
            ExplosionEffect();
        }        
    }


    private void ExplosionEffect()
    {
        explosionEffect_temp = Instantiate(explosionEffect, this.transform);
        explosionEffect_temp.transform.SetParent(this.transform.parent);
        explosionEffect_temp.transform.localEulerAngles = new Vector3(0, 0, 0);
        Destroy(explosionEffect_temp, 2f);
    }


}
