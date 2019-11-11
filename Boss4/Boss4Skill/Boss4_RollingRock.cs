using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_RollingRock : MonoBehaviour
{
    private Vector2 dir;
    private float movespeed;
    private float rotatespeed;

    private GameObject[] players;

    public GameObject destroyEffect;
    private GameObject destroyEffect_temp;

    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;

    public GameObject ObjectLeft;
    private GameObject ObjectLeft_temp;


    void Start()
    {
        dir = new Vector2(0, -1);
        movespeed = 2f;
        rotatespeed = -400f;
        //this.transform.SetParent(this.transform.parent.parent);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Time.deltaTime* rotatespeed, 0, 0);
        //if (this.transform.rotation.x >= 90)
        //    this.transform.Rotate(0, 180, 180);

        this.transform.Translate(dir * Time.deltaTime * movespeed,Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if(collision.CompareTag("EditorOnly")) //아래에 맞은경우
        {
            players=GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
                players[i].GetComponent<CharacterStat>().GetDamage(BossStatus.instance.MagicAttackPower * 5,true);
            ExplosionEffect();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.MagicAttackPower, true);
        }
    }

    public void DestoryEffect()
    {
        destroyEffect_temp = Instantiate(destroyEffect, this.transform);
        destroyEffect_temp.transform.SetParent(this.transform.parent);

        BossStatus.instance.gameObject.GetComponent<Boss4Move>().main.GetComponent<Camera_move>().VivrateForTime(0.5f);

        Vector3 temp;

        temp = this.transform.position;
        temp.z -= 100f;

        destroyEffect_temp.transform.position = temp;

        Destroy(destroyEffect_temp, 2f);


        ObjectLeft_temp = Instantiate(ObjectLeft, this.transform);
        ObjectLeft_temp.transform.SetParent(this.transform.parent);


        this.gameObject.GetComponent<Monster>().SetDeactiveHPBar();
        this.transform.position = new Vector2(100, 100);
        this.GetComponent<Monster>().HP = this.GetComponent<Monster>().MaxHP;

        this.gameObject.SetActive(false);
    }

    private void ExplosionEffect()
    {
        explosionEffect_temp = Instantiate(explosionEffect, this.transform);
        explosionEffect_temp.transform.SetParent(this.transform.parent);
        explosionEffect_temp.transform.localEulerAngles = new Vector3(0, 0, 0);
        Destroy(explosionEffect_temp, 2f);

        this.gameObject.GetComponent<Monster>().SetDeactiveHPBar();
        this.transform.position = new Vector2(100, 100);
        this.GetComponent<Monster>().HP = this.GetComponent<Monster>().MaxHP;
        this.gameObject.SetActive(false);
    }

}
