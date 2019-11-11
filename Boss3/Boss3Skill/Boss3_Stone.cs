using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_Stone : MonoBehaviour
{
    private Vector2 player_position;
    private Vector2 boss_position;
    private float player_ani_x;
    private float player_ani_y;
    private float boss_ani_x;
    private float boss_ani_y;
    private GameObject boss;
    private Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        boss = BossStatus.instance.gameObject;
        boss_position = boss.transform.position;
        boss_ani_x = boss.GetComponent<Animator>().GetFloat("DirX");
        boss_ani_y = boss.GetComponent<Animator>().GetFloat("DirY");
        col = this.GetComponent<Collider2D>();
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player_position = collision.transform.position;
            player_ani_x = collision.GetComponent<Animator>().GetFloat("DirX");
            player_ani_y = collision.GetComponent<Animator>().GetFloat("DirY");
            if (boss_ani_x>=0&& boss_ani_y>=0)
            {
                if (player_ani_x < 0f && player_ani_y < 0f)
                {
                    collision.GetComponent<CharacterStat>().GetDamage(99999, false);
                    if(collision.tag=="DeadPlayer")
                    {
                        collision.GetComponent<Animator>().enabled = false;
                    }
                    

                }
            }
            else if (boss_ani_x < 0 && boss_ani_y >= 0)
            {
                if (player_ani_x >= 0f && player_ani_y < 0f)
                {
                    collision.GetComponent<CharacterStat>().GetDamage(99999, false);
                    if (collision.tag == "DeadPlayer")
                    {
                        collision.GetComponent<Animator>().enabled = false;
                    }
                }
            }
            else if (boss_ani_x < 0 && boss_ani_y < 0)
            {
                if (player_ani_x >= 0f && player_ani_y >= 0f)
                {
                    collision.GetComponent<CharacterStat>().GetDamage(99999, false);
                    if (collision.tag == "DeadPlayer")
                    {
                        collision.GetComponent<Animator>().enabled = false;
                    }
                }
            }
            else if (boss_ani_x >= 0 && boss_ani_y < 0)
            {
                if (player_ani_x < 0f && player_ani_y >= 0f)
                {
                    collision.GetComponent<CharacterStat>().GetDamage(99999, false);
                    if (collision.tag == "DeadPlayer")
                    {
                        collision.GetComponent<Animator>().enabled = false;
                    }
                }
            }


        }
    }
}
