using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7_BloodFire : MonoBehaviour
{
    private Collider2D[] Player;

    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;
    private float time = 0;
    private float damage_amount;
    private float multiple = 0.2f;

    void Start()
    {
        damage_amount = BossStatus.instance.PhysicalAttackPower;
        StartCoroutine(FireDamage());
        SoundManager.instance.Play(60);

    }
    IEnumerator FireDamage()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.1f);

        while(true)
        {
            Player = Physics2D.OverlapCircleAll(this.transform.position, 0.2f);
            for(int i=0;i< Player.Length;i++)
            {
                if(Player[i].CompareTag("Player"))
                {
                    ExplosionEffect();
                    Player[i].GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, false);
                }
            }
            yield return waittime;
        }

    }
    private void ExplosionEffect()
    {
        explosionEffect_temp = Instantiate(explosionEffect, this.transform.position, Quaternion.identity);
        explosionEffect_temp.transform.localScale = new Vector2(1f, 1f);
        Destroy(explosionEffect_temp, 2f);
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time >= 1.5f)
        {
            Destroy(this.gameObject);
            SoundManager.instance.Stop(60);
        }
    }
}
