using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Spear : MonoBehaviour
{
    public GameObject warning;

    private GameObject warning_temp;
    private GameObject[] players;
    private GameObject effect_temp;
    private CircleCollider2D _col;
    private int i = 0;
    // 스킬 데미지 계수
    public float multiple = 1.3f;

    private float physical_attack;

    private Vector3 dir;

    private float dx;
    private float dy;

    private float speed = 30f;

    private float rotate_degree;

    private float time = 0;

    private GameObject character;
    // Start is called before the first frame update
    void Start()
    {
        _col = this.GetComponent<CircleCollider2D>();
        _col.enabled = false;
        players = GameObject.FindGameObjectsWithTag("Player");
        i = Random.Range(0, players.Length);
        SoundManager.instance.Play(22);

        physical_attack = BossStatus.instance.PhysicalAttackPower;

        dx = players[i].transform.position.x - this.transform.position.x;
        dy = players[i].transform.position.y - this.transform.position.y;

        dir = (players[i].transform.position - this.transform.position).normalized;

        rotate_degree = Mathf.Atan(dy/dx) * Mathf.Rad2Deg;



        if (players[i].transform.position.x - this.transform.position.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90 + rotate_degree);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 270 + rotate_degree);
        }

        warning_temp = Instantiate(warning, this.transform.position, Quaternion.identity);
        warning_temp.transform.localScale = new Vector3(50, 3, 3);
        if (players[i].transform.position.x - this.transform.position.x > 0)
        {
            warning_temp.transform.rotation = Quaternion.Euler(0f, 0f,rotate_degree);
        }
        else
        {
            warning_temp.transform.rotation = Quaternion.Euler(0f, 0f,180 + rotate_degree);
        }
        Destroy(warning_temp, 2f);
        Destroy(this.gameObject, 5f);
    }


    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time >= 2f)
        {
            _col.enabled = true;

            this.transform.Translate(dir * speed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(physical_attack * 5f, false);
            Destroy(this.gameObject);
        }
    }
}
