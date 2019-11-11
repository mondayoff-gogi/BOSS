using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss3_Pile : MonoBehaviour
{
    LineRenderer lr;
    Vector3 cube1Pos, cube2Pos;
    private GameObject players;
    private Vector2 this_position;
    private Vector2 temp;

    public GameObject destroy_effect;
    private GameObject destroy_temp;

    public GameObject bomb_effect;
    private GameObject bomb_temp;

    //private float distance = 6f;
    private float speed = 0f;

    private float existing_time = 0;
    private SpriteRenderer arrow_image;

    private Vector3 hit_effect_position;
    public GameObject hit_effect;
    private GameObject hit_temp;
    private GameObject boss;

    private float damage_amount;

    private float multiple;

    private void Start()
    {
        multiple = (UpLoadData.boss_level+1)/2f;
        SoundManager.instance.Play(11);
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.5f;
        lr.endWidth = 0.3f;
        arrow_image = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        boss = BossStatus.instance.gameObject;
        damage_amount = BossStatus.instance.PhysicalAttackPower;
    }

    void Update()
    {
        existing_time += Time.deltaTime;
        lr.SetPosition(0, this_position);
        lr.SetPosition(1, players.transform.position);

        if (speed > 0)
            speed -= Time.deltaTime;
        else
            speed = 0;

        
        temp = players.transform.position - this.transform.position;
        temp.Normalize();
        players.transform.Translate(temp * speed * Time.deltaTime);
        

        if(players.tag != "Player")
        {
            DestoryEffect();
        }
    }

    public void DestoryEffect()
    {
        SoundManager.instance.Play(74);
        destroy_temp = Instantiate(destroy_effect, this.transform.position, Quaternion.identity);
        StopAllCoroutines();
        this.gameObject.GetComponent<Monster>().SetDeactiveHPBar();
        this.gameObject.GetComponent<Monster>().HP = this.gameObject.GetComponent<Monster>().MaxHP;
        boss.GetComponent<Boss3Move>().StopMagneticCor();
        Destroy(destroy_temp, 0.5f);
        this.gameObject.SetActive(false);
    }


    public void GetPlayer(GameObject player)
    {
        players = player;
        speed = players.GetComponent<CharacterStat>().move_speed;
        speed *= 1f;
    }

    public void GetPosition(Vector2 position)
    {
        this_position = position;
    }

    public void StartCoturines()
    {
        StartCoroutine(GiveDamage(players));
        StartCoroutine(HitEffct(players));
    }

    IEnumerator GiveDamage(GameObject player)
    {
        while (true)
        {
            // TODO 공식 수정 요망
            player.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, false);

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator HitEffct(GameObject players)
    {
        while (true)
        {
            hit_effect_position = players.transform.position;
            hit_effect_position.y += 1f;
            hit_temp = Instantiate(hit_effect, hit_effect_position, Quaternion.identity);
            hit_temp.transform.SetParent(players.transform);
            Destroy(hit_temp, 1f);
            yield return new WaitForSeconds(1f);
        }
    }

}
