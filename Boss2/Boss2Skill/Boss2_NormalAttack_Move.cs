using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_NormalAttack_Move : MonoBehaviour
{
    private GameObject[] players;
    private Vector2 dir;
    private float speed;

    private float damage_amount;
    private float multiple = 2;

    public GameObject warning_effect;
    private GameObject warning_temp;
    private int flag;
    private float time = 0;
    public GameObject destroy_effect;
    private GameObject destroy_temp;

    private Vector2 randVec;

    private bool Is_trigger = false;

    // Start is called before the first frame update
    void Start()
    {
        speed = UpLoadData.boss_level * 2 + 15;

        Is_trigger = true;
        if (NetworkManager.instance.is_multi)
        {
            players = new GameObject[NetworkManager.instance.Player_num];
            for(int i = 0; i < players.Length; i++)
            {
                players[i] = GameManage.instance.all_character_array[i];
            }
        }
        else
        {
            players = GameManage.instance.player;
        }

        Debug.Log(players.Length);
        damage_amount = BossStatus.instance.PhysicalAttackPower;
        randVec.x = Random.Range(-0.2f, 0.2f);
        randVec.y = Random.Range(-0.2f, 0.2f);

        if(players.Length >= 2)
        {
            flag = Random.Range(0, players.Length);
        }
        else if(players.Length == 1)
        {
            flag = 0;
        }
        else if (players.Length <= 0)
        {
            Destroy(this.gameObject);
        }
        if (players.Length <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            dir = players[flag].transform.position - this.transform.position;
        }
        dir.Normalize();
        dir += randVec;
        dir.Normalize();
        CreateWarning();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > 2f)
        {
            this.transform.Translate(dir * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&& Is_trigger)
        {
            destroy_temp = Instantiate(destroy_effect, this.transform.position, Quaternion.identity);
            collision.gameObject.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, false);
            Destroy(destroy_temp, 0.5f);
            Destroy(this.gameObject);
        }
    }
    private void CreateWarning()
    {
        Destroy(this.gameObject, 10f);
        ParticleSystem ps;
        warning_temp = Instantiate(warning_effect, this.transform.position, Quaternion.identity);
        ps = warning_temp.GetComponent<ParticleSystem>();
        var simulate = ps.main;
        simulate.simulationSpeed = 1 / 2f;
        warning_temp.transform.localScale = new Vector3(40, 0.7f, 0.7f);
        if (dir.x> 0)
            warning_temp.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((dir.y) / dir.x));
        else
            warning_temp.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((dir.y) /dir.x));

        switch (UpLoadData.boss_level)
        {
            case 0:
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 26, warning_temp.transform, 2f, true, false);
                break;

            case 1:
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 26, warning_temp.transform, 1.8f, true, false);
                break;

            case 2:
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 26, warning_temp.transform, 1f, true, false);
                break;

            case 3:
                NetworkManager.instance.InstantiateBossSkill(UpLoadData.boss_index, 26, warning_temp.transform, 0.5f, true, false);
                break;
        }

        switch(UpLoadData.boss_level)
        {
            case 0:
                Destroy(warning_temp, 2f);
                break;

            case 1:
                Destroy(warning_temp, 1.8f);
                break;

            case 2:
                Destroy(warning_temp, 1f);
                break;

            case 3:
                Destroy(warning_temp, 0.5f);
                break;
        }

    }
}
