using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_NormalAttack : MonoBehaviour
{
    private Vector2 dir;
    private GameObject boss;
    private float attack_amount;
    private float multiple = 1f;
    private Collider2D[] Players;


    public GameObject Zone;
    private GameObject zone_prefab;
    private Vector3 hit_effect_position;
    public GameObject hit_effect;
    private GameObject hit_temp;

    public GameObject fire_effect;
    private GameObject fire_temp;
    // Start is called before the first frame update
    private bool flag;

    private const float speed = 2f;


    private void Start()
    {
        flag = false;
        boss = BossStatus.instance.gameObject;
        attack_amount = boss.GetComponent<BossStatus>().MagicAttackPower;
        StartCoroutine(GiveDamage());
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector2.Lerp(this.transform.position, dir, speed * Time.deltaTime);
        if (Vector2.Distance(this.transform.position, dir) < 0.1f&&!flag)
        {
            flag = true;
            zone_prefab = Instantiate(Zone, this.transform);
        }
            
    }

    public void GetDir(Vector2 dir)
    {
        this.dir = dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RainBall"))
        {
            fire_temp = Instantiate(fire_effect, new Vector3( this.transform.position.x, this.transform.position.y, -100), Quaternion.Euler(-90,0,0));
            Destroy(this.gameObject);
        }
    }
    IEnumerator GiveDamage()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(1f);

        while (true)
        {
            if(flag)
            {
                Players = Physics2D.OverlapCircleAll(this.transform.position, 1.7f);
                for (int i = 0; i < Players.Length; i++)
                {
                    if (Players[i].CompareTag("Player"))
                    {
                        Players[i].GetComponent<CharacterStat>().GetDamage(attack_amount * multiple, true);
                        hit_effect_position = Players[i].transform.position;
                        hit_effect_position.y += 1f;
                        hit_temp = Instantiate(hit_effect, hit_effect_position, Quaternion.identity);
                        hit_temp.transform.SetParent(Players[i].transform);
                        Destroy(hit_temp, 1f);
                    }
                }
            }
            
            yield return waittime;
        }
    }
}
