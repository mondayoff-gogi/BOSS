using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_ShootingRock : MonoBehaviour
{
    public GameObject ExplosionEffect;
    private GameObject ExplosionEffect_temp;

    public GameObject StraightWarning;
    private GameObject StraightWarning_temp;

    private Collider2D[] players;

    private Vector2 dir;

    private float movespeed;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 10f);
        dir = new Vector2(-this.transform.position.x, -this.transform.position.y);
        dir.x += Random.Range(-7, 7);
        dir.y += Random.Range(-7, 7);
        dir.Normalize();



        StraightWarning_temp = Instantiate(StraightWarning, this.transform.position, Quaternion.identity);


        if (dir.x > 0)
            StraightWarning_temp.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((dir.y) / (dir.x)));
        else
            StraightWarning_temp.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((dir.y) / (dir.x)));


        StraightWarning_temp.transform.localScale = new Vector2(60, 1f);
        Destroy(StraightWarning_temp, 2f);
        movespeed = 7f;//UpLoadData.boss_level * 1f + 3f;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir* movespeed*Time.deltaTime);
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if(collision.gameObject.CompareTag("BossAttack")|| collision.gameObject.CompareTag("Player"))
    //    {
    //        Explsion();
    //        Destroy(this.gameObject);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BossAttack") || collision.CompareTag("Player"))
        {
            Explsion();
            if(StraightWarning_temp)
                Destroy(StraightWarning_temp);
            Destroy(this.gameObject);
        }
    }
    void Explsion()
    {
        BossStatus.instance.GetComponent<Boss4Move>().main.GetComponent<Camera_move>().VivrateForTime(0.5f);
        SoundManager.instance.Play(7);
        ExplosionEffect_temp = Instantiate(ExplosionEffect, this.transform.position, Quaternion.identity);
        Destroy(ExplosionEffect_temp, 2f);
        players = Physics2D.OverlapCircleAll(this.transform.position, 4f);
        for(int i=0;i< players.Length;i++)
        {
            if(players[i].CompareTag("Player"))
            {
                players[i].gameObject.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.MagicAttackPower, true);
            }
        }
    }
}
