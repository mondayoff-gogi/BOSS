using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss8_Swift : MonoBehaviour
{
    private Vector2 dir;
    private float movespeed;
    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;

    private Collider2D[] _player;
    private bool is_PlayerAttack;
    private GameObject AttackPlayer;

    private float DamageGive;

    private float timer;

    private const float SearchingRaduis = 8f;

    void Start()
    {
        is_PlayerAttack = false;
        movespeed = 50f;
        StartCoroutine(StartMove());

        Destroy(this.gameObject, 5f);
    }
    //근처에 있는지 확인
    
        //있으면 걔한테감
        //없으면 랜덤한위치로 감
    
    IEnumerator StartMove()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.01f);

        while(true)
        {
            _player = Physics2D.OverlapCircleAll(this.transform.position, SearchingRaduis);
            for (int i = 0; i < _player.Length; i++)
            {
                if (AttackPlayer == _player[i].gameObject) continue;
                if (_player[i].CompareTag("Player"))
                {
                    is_PlayerAttack = true;
                    AttackPlayer = _player[i].gameObject;
                    dir = (Vector2)_player[i].transform.position - (Vector2)this.transform.position;
                    break;
                }
            }
            if (is_PlayerAttack)
            {
                is_PlayerAttack = false;
                while (true)
                {
                    dir = AttackPlayer.transform.position - this.transform.position;
                    dir.Normalize();
                    this.transform.Translate(dir * movespeed * Time.deltaTime);
                    yield return waittime;
                    if (Vector2.Distance(this.transform.position, AttackPlayer.transform.position) < 1f)
                    {
                        AttackPlayer.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.PhysicalAttackPower / 15, true);
                        ExplosionEffect();
                        break;
                    }
                }
            }
            else
            {
                timer = 0;
                dir = Random.insideUnitCircle;

                if (this.transform.position.x > 5 && this.transform.position.y > 5)
                    dir = new Vector2(Random.Range(-1f, 0f), Random.Range(-1f, 0f));
                else if (this.transform.position.x < -5 && this.transform.position.y > 5)
                    dir = new Vector2(Random.Range(1f, 0f), Random.Range(-1f, 0f));
                else if (this.transform.position.x < -5 && this.transform.position.y < -5)
                    dir = new Vector2(Random.Range(1f, 0f), Random.Range(1f, 0f));
                else if (this.transform.position.x > 5 && this.transform.position.y < -5)
                    dir = new Vector2(Random.Range(-1f, 0f), Random.Range(1f, 0f));
                AttackPlayer = null;
                dir.Normalize();

                while (true)
                {
                    this.transform.Translate(dir * movespeed * Time.deltaTime);
                    yield return waittime;
                    timer += Time.deltaTime;
                    if (timer > 0.1f)
                        break;
                }
            }
        }
    }



    private void ExplosionEffect()
    {
        explosionEffect_temp = Instantiate(explosionEffect, this.transform.position, Quaternion.identity);
        explosionEffect_temp.transform.localScale = new Vector2(1f, 1f);
        Destroy(explosionEffect_temp, 2f);
    }
}
