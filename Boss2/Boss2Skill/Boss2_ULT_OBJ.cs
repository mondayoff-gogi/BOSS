using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss2_ULT_OBJ : MonoBehaviour
{
    Color _color;
    private float existing_time = 0;
    private float exist_time = 20f;

    public GameObject explosion_effect;
    private GameObject explosion_temp;
    private GameObject[] players;
    private GameObject boss;
    private float boss_damage;
    private GameObject[] magnetic_icon;
    // Start is called before the first frame update
    private void Start()
    {
        switch (UpLoadData.boss_level)
        {
            case 0:
                boss_damage = 2000f;
                break;
            case 1:
                boss_damage = 3000f;
                break;
            case 2:
                boss_damage = 7000f;
                break;
            case 3:
                boss_damage = 10000f;
                break;
        }
        players = GameObject.FindGameObjectsWithTag("Player");
        boss = GameObject.FindGameObjectWithTag("Boss");
        magnetic_icon = new GameObject[players.Length];
        if(players.Length == 1)
        {
            magnetic_icon[0] = GameObject.Find("N(Clone)");
        }
        else
        {
            magnetic_icon[0] = GameObject.Find("N(Clone)");
            magnetic_icon[1] = GameObject.Find("S(Clone)");
        }


    }
    // Update is called once per frame
    void Update()
    {
        existing_time += Time.deltaTime;
        if (existing_time > exist_time)
        {
            for(int i = 0; i < players.Length; i++)
            {
                players[i].GetComponent<CharacterStat>().GetDamage(99999, false);
                explosion_temp = Instantiate(explosion_effect, this.transform);
                Destroy(explosion_temp);
            }
            Destroy(this.gameObject);
        }
        _color = new Color(1, (1-existing_time/ exist_time), (1-existing_time/ exist_time), 1);
        this.GetComponent<SpriteRenderer>().color = _color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<CharacterStat>().GetDamage(99999, false);
            explosion_temp = Instantiate(explosion_effect, collision.transform);
            boss.GetComponentInChildren<Boss2_ULT_Check>().CountBlocks();
            if(collision.gameObject.GetComponentInChildren<Boss2_Player_Magnetic>().magnetic == false)
            {
                Destroy(collision.gameObject.transform.Find("N(Clone)").gameObject);
            }
            else
            {
                Destroy(collision.gameObject.transform.Find("S(Clone)").gameObject);
            }
            Destroy(collision.gameObject.GetComponentInChildren<Boss2_Player_Magnetic>().gameObject);
            Destroy(explosion_temp, 0.5f);
            Destroy(this.gameObject);
        }

        if(collision.gameObject.tag == "Boss")
        {
            collision.gameObject.GetComponent<BossStatus>().BossGetDamage(boss_damage, false);
            explosion_temp = Instantiate(explosion_effect, collision.transform);
            boss.GetComponentInChildren<Boss2_ULT_Check>().CountBlocks();
            Destroy(explosion_temp, 0.5f);
            Destroy(this.gameObject);
        }

        if(collision.gameObject.tag == "Monster")
        {
            for(int i =0;i < players.Length; i++)
            {
                players[i].GetComponent<CharacterStat>().GetDamage(99999, false);
                explosion_temp = Instantiate(explosion_effect, players[i].transform);
                boss.GetComponentInChildren<Boss2_ULT_Check>().CountBlocks();
                Destroy(players[i].GetComponentInChildren<Boss2_Player_Magnetic>().gameObject);
                Destroy(explosion_temp, 0.5f);
                Destroy(this.gameObject);
            }
        }
    }
}
