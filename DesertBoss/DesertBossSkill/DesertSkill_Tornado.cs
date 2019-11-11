using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSkill_Tornado : MonoBehaviour
{
    private Vector2 dir;

    private Vector2 temp;

    private float time;
    private float timer=1;
    private float rand_x;
    private float rand_y;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.SetParent(this.transform.parent.parent);
        Destroy(this.gameObject, 5); //5초뒤에 없어짐
        rand_x = Random.Range(-100, 100);
        rand_y = Random.Range(-100, 100);
        temp = new Vector2(rand_x, rand_y);
        temp.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        dir = new Vector2(Mathf.Cos(time), Mathf.Sin(time));
        dir.Normalize();

        this.transform.Translate(temp/30,Space.World);
        this.transform.Translate(dir/60, Space.World);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            timer += Time.deltaTime;
            if (timer > 0.5)
            {
                collision.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.MagicAttackPower*5f/100f, true);
                timer = 0;
            }
        }
       
    }

}

