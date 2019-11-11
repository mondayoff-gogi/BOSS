using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss8_StealSkill : MonoBehaviour
{
    public GameObject effect;

    private GameObject effect_temp;
    LineRenderer lr;
    Vector3 this_position, boss_position;
    private GameObject boss;
    private Vector2 dir;
    private float speed = 5f;

    private void Start()
    {
        boss = BossStatus.instance.gameObject;
        lr = GetComponent<LineRenderer>();
        lr.startWidth = .05f;
        lr.endWidth = .05f;
        boss_position = BossStatus.instance.transform.position;
        this_position = this.transform.position;
        dir = boss_position - this_position;
    }

    void Update()
    {
        dir.Normalize();
        lr.SetPosition(0, this.transform.position);
        lr.SetPosition(1, boss.transform.position);

        this.transform.Translate(dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            effect_temp = Instantiate(effect, collision.transform);
            Destroy(effect_temp, 3f);
            Destroy(this.gameObject);
        }
    }

}
