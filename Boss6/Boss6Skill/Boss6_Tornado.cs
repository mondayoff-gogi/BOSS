using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_Tornado : MonoBehaviour
{
    private Vector2 temp;

    Collider2D[] _collider;

    private Vector2 dir;

    private Vector2 randomvec;

    private float movespeed;

    public GameObject DestroyEffect;
    private GameObject DestroyEffect_temp;
    private float damage_amount;
    private float multiple = 0.2f;
    private int damageOK;

    private void Start()
    {
        damageOK = 0;
        randomvec = new Vector2(0, 0);
        dir = BossStatus.instance.GetComponent<Boss6Move>().vector3_throw_attack- BossStatus.instance.transform.position;
        movespeed = 4f;
        damage_amount = BossStatus.instance.MagicAttackPower;
        StartCoroutine(Tornado());
        StartCoroutine(DirChange());
    }

    private void Update()
    {
        this.transform.Translate((dir + randomvec) * Time.deltaTime * movespeed,Space.World);
    }

    IEnumerator Tornado()
    {
        while (true)
        {
            _collider = Physics2D.OverlapCircleAll(this.transform.position, 2f);
            for (int i = 0; i < _collider.Length; i++)
            {
                if (_collider[i].tag == "Player")
                {
                    temp = this.transform.position - _collider[i].transform.position;
                    temp.Normalize();
                    _collider[i].transform.Translate(temp/6);
                    if(damageOK%10==0)
                        _collider[i].GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, true);
                    damageOK++;
                }
            }
            yield return new WaitForSeconds(0.02f);
        }
    }
    IEnumerator DirChange()
    {
        float random;
        while (true)
        {
            randomvec = new Vector2(dir.y, -dir.x);

            random = Random.Range(0f, 1f);

            randomvec *= random * 0.05f;



            dir.Normalize();

            yield return new WaitForSeconds(1f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary"))
        {
            DestroyEffect_temp = Instantiate(DestroyEffect, this.transform.position, Quaternion.identity);
            DestroyEffect_temp.transform.localScale = new Vector2(4f, 4f);
            Destroy(DestroyEffect_temp, 3f);            
            Destroy(this.gameObject);
        }
    }
}
