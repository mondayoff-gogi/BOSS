using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7_Moss : MonoBehaviour
{
    private Vector2 dir;
    private float movespeed;
    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;

    private const float SearchingRaduis = 5f;
    private const float FlyingRaduis = 5f;

    private Collider2D[] _player;
    private bool is_PlayerAttack;
    private GameObject AttackPlayer;    

    public ParticleSystem ps;

    private bool flag;

    private float DamageGive;



    void Start()
    {
        flag = false;

        var main = ps.main;
        main.startColor = new Color(1, 1, 1, 0.2f);

        is_PlayerAttack = false;
        dir = Random.insideUnitCircle;
        movespeed = 7f;
        StartCoroutine(StartMove());

        Destroy(this.gameObject, 10f);
    }
    IEnumerator StartMove()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.01f);
        while (true)
        {
            if (movespeed <= 5) break;

            movespeed -= Time.deltaTime;
            this.transform.Translate(dir * movespeed * Time.deltaTime);
            yield return waittime;
        }
        StartCoroutine(RandomMove());
        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator RandomMove()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.01f);
        while (true && BossStatus.instance)
        {
            Vector2 Boss_Pos = BossStatus.instance.transform.position;
            Vector2 FlyingPos = Random.insideUnitCircle.normalized; //날아다닐수있는 범위
            FlyingPos *= FlyingRaduis;

            FlyingPos += Boss_Pos;

            dir = FlyingPos - (Vector2)this.transform.position;

            _player =Physics2D.OverlapCircleAll(this.transform.position, SearchingRaduis);

            for(int i=0;i<_player.Length;i++)
            {
                if(_player[i].CompareTag("Player")&& flag)
                {
                    var main = ps.main;
                    main.startColor = new Color(1, 0, 0, 1);
                    is_PlayerAttack = true;
                    AttackPlayer = _player[i].gameObject;
                    dir = (Vector2)_player[i].transform.position - (Vector2)this.transform.position;

                    break;
                }
            }


            while (true)
            {
                movespeed = 20f;
                if (is_PlayerAttack)
                {
                    dir = (Vector2)AttackPlayer.transform.position - (Vector2)this.transform.position;
                    FlyingPos = (Vector2)AttackPlayer.transform.position;
                }
                dir.Normalize();

                this.transform.Translate(dir * movespeed * Time.deltaTime);
                SoundManager.instance.Play(58);

                yield return waittime;

                if (Vector2.Distance(this.transform.position,FlyingPos)<0.5f)
                {
                    flag = true;
                    if (is_PlayerAttack)
                    {
                        ExplosionEffect();
                        var main = ps.main;
                        main.startColor = new Color(1, 0, 1, 1);
                        is_PlayerAttack = false;
                        if(AttackPlayer.CompareTag("Player"))
                        {
                            DamageGive = AttackPlayer.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.MagicAttackPower * 0.1f, false);
                            while (true&&BossStatus.instance)
                            {
                                dir = BossStatus.instance.transform.position - this.transform.position;
                                dir.Normalize();
                                this.transform.Translate(dir * movespeed * Time.deltaTime);
                                yield return waittime;
                                if(BossStatus.instance)
                                {
                                    if (Vector2.Distance(this.transform.position, BossStatus.instance.transform.position) < 0.2f)
                                    {
                                        BossStatus.instance.BossGetHeal(DamageGive);
                                        main.startColor = new Color(0, 0, 0, 1);
                                        flag = false;
                                        break;
                                    }
                                }
                                
                            }
                        }                        
                    }
                    else
                    {
                        yield return new WaitForSeconds(Random.Range(0.6f,1.2f));
                    }

                    break;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ExplosionEffect()
    {
        explosionEffect_temp = Instantiate(explosionEffect, this.transform.position,Quaternion.identity);
        explosionEffect_temp.transform.localScale = new Vector2(1f,1f);
        Destroy(explosionEffect_temp, 2f);
    }
}
