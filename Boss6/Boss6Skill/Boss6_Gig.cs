using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_Gig : MonoBehaviour
{
    public GameObject follower;
    private GameObject[] follower_temp;
    private const int num_follower=8;

    private Vector2 dir;
    private float movespeed;
    private float firstmovespeed;
    private GameObject target_player;

    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;

    public GameObject bigExplosionEffect;
    private GameObject bigExplosionEffect_temp;

    private float damage_amount;

    private const float GetFaster = 0.5f;

    private bool flag = false;

    private Collider2D[] player;

    void Start()
    {
        follower_temp = new GameObject[num_follower];
        for(int i=0;i< num_follower; i++)
        {
            follower_temp[i] = Instantiate(follower, this.gameObject.transform.position, Quaternion.identity);
            Destroy(follower_temp[i], 6f);
        }
        target_player = BossStatus.instance.GetComponent<Boss6Move>().temp_target_player;
        movespeed = 30f;
        firstmovespeed = movespeed;
        dir = target_player.transform.position - this.transform.position;
        dir.Normalize();
        damage_amount = BossStatus.instance.PhysicalAttackPower;
        Destroy(this.gameObject, 2f);
    }

    void Update()
    {        
        movespeed -= GetFaster;      

        if (!flag)
        {
            this.transform.Translate(dir * Time.deltaTime * movespeed, Space.World);
        }


        for (int i=0;i< num_follower; i++)
        {
            follower_temp[i].transform.position = (Vector2)this.transform.position*((float)i/(float)num_follower) + (Vector2)BossStatus.instance.transform.position* (((float)num_follower - i) / (float)num_follower);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount, false);
            ExplosionEffect();
            StartCoroutine(Stick(collision));
        }
        if (collision.CompareTag("Boundary"))
        {
            ExplosionEffect();
            for(int i=0;i<num_follower;i++)
            {
                Destroy(follower_temp[i]);                
            }
            Destroy(this.gameObject);
        }
    }
    IEnumerator Stick(Collider2D collision)
    {
        flag = true;
        Vector2 vec2;

        WaitForSeconds waittime = new WaitForSeconds(0.01f);
        while(true)
        {
            vec2 = -this.transform.position + BossStatus.instance.transform.position;
            vec2.Normalize();
            this.transform.Translate(vec2*Time.deltaTime* firstmovespeed, Space.World);
            collision.transform.position = this.transform.position;
            yield return waittime;
            if(Vector2.Distance(this.transform.position,BossStatus.instance.transform.position)<1)//끌어왔을때
            {
                BigExplosionEffect();

                player=Physics2D.OverlapCircleAll(this.transform.position, 4f);
                for(int i=0;i<player.Length;i++)
                {
                    if(player[i].CompareTag("Player"))
                    {
                        player[i].GetComponent<CharacterStat>().GetDamage(damage_amount, false);
                    }
                }
                for (int i = 0; i < num_follower; i++)
                {
                    Destroy(follower_temp[i]);
                }
                Destroy(this.gameObject);
                break;
            }
        }
    }

    private void ExplosionEffect()
    {
        explosionEffect_temp = Instantiate(explosionEffect, this.transform.position,Quaternion.identity);
        explosionEffect_temp.transform.localScale = new Vector2(4f, 4f);
        Destroy(explosionEffect_temp, 2f);
    }
    private void BigExplosionEffect()
    {
        SoundManager.instance.Play(48);
        BossStatus.instance.GetComponent<Boss6Move>().main.GetComponent<Camera_move>().VivrateForTime(1f);
        bigExplosionEffect_temp = Instantiate(bigExplosionEffect, this.transform.position, Quaternion.identity);
        bigExplosionEffect_temp.transform.localScale = new Vector2(4f, 4f);
        Destroy(bigExplosionEffect_temp, 2f);
    }
}
