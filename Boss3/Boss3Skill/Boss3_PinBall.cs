using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_PinBall : MonoBehaviour
{
    private Vector2 dir;
    private float movespeed;

    public GameObject pinball1;
    private GameObject pinball1_temp;

    private GameObject Boss;

    private int trigger_num;

    public GameObject effect;
    private GameObject effect_temp;

    private float damage_amount;


    void Start()
    {
        
        Boss = this.transform.parent.gameObject;
        this.transform.SetParent(Boss.transform.parent);

        dir = this.transform.position;
        dir = Boss.GetComponent<Boss3Move>().vector2_throw_attack - dir;

        dir.Normalize();
        movespeed = 5f;
        damage_amount = BossStatus.instance.MagicAttackPower;
        trigger_num = 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir * Time.deltaTime * movespeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount, true);            
            effect_temp = Instantiate(effect, collision.transform);
            SoundManager.instance.Play(74);
        }
        if (collision.CompareTag("Finish"))//좌우
        {
            SoundManager.instance.Play(31);
            trigger_num++;
            if (trigger_num > (int)(UpLoadData.boss_level)/(int)2 + 2)
                Destroy(this.gameObject);
            dir.x *= -1;
            StartCoroutine(Duplicate());
        }
        if (collision.CompareTag("Respawn"))//상하
        {
            SoundManager.instance.Play(31);
            trigger_num++;
            if (trigger_num > UpLoadData.boss_level + 2)
                Destroy(this.gameObject);
            dir.y *= -1;
            StartCoroutine(Duplicate());
        }
    }
    IEnumerator Duplicate()
    {
        yield return new WaitForSeconds(0.1f);
        pinball1_temp = Instantiate(pinball1, this.transform);
        pinball1_temp = Instantiate(pinball1, this.transform);
        if(UpLoadData.boss_level>=2)
        {
            pinball1_temp = Instantiate(pinball1, this.transform);
        }

    }

}
