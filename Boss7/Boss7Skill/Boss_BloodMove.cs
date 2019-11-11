using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_BloodMove : MonoBehaviour
{
    public GameObject heal_effect;

    private GameObject heal_temp;
    private GameObject boss;
    private Vector2 dir;
    private float speed = 20f;
    private float attack_amount;
    private WaitForSeconds wait_time = new WaitForSeconds(0.01f);
    // Start is called before the first frame update
    void Start()
    {
        boss = BossStatus.instance.gameObject;
        if(UpLoadData.boss_level < 2)
        {
            attack_amount = 10f;
        }
        else
        {
            attack_amount = 20f;
        }
            
    }



    private void Update()
    {
        if(GameManage.instance.IsGameEnd==true)
        {
            Destroy(this.gameObject);
        }
        dir = boss.transform.position - this.transform.position;
        dir.Normalize();
        this.transform.Translate(dir * (speed / (Vector2.Distance(boss.transform.position, this.transform.position)+1) * Time.deltaTime));

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            SoundManager.instance.Play(59);
            collision.GetComponent<BossStatus>().MagicAttackPower += attack_amount;
            collision.GetComponent<BossStatus>().PhysicalAttackPower += attack_amount;
            Color color = collision.GetComponent<SpriteRenderer>().color;
            color.g -= 0.05f;
            color.b -= 0.05f;
            collision.GetComponent<SpriteRenderer>().color = color;
            heal_temp = Instantiate(heal_effect, collision.transform);
            Destroy(heal_temp, 0.5f);
            Destroy(this.gameObject);
        }
    }
}
