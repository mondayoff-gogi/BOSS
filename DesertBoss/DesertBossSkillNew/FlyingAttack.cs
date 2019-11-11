using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAttack : MonoBehaviour
{
    private float multiple = 5;
    private float multiple_2 = 2;

    private float damage_amount;
    private Vector2 dir;
    private Vector2 plus;
    private float movespeed;

    public GameObject PlayerHit;
    private GameObject PlayerHit_prefab;

    public GameObject PilllarHit;
    private GameObject PilllarHit_prefab;

    private GameObject[] Player;
    private Animator _animator;

    void Start()
    {
        dir = new Vector2(1, 0);
        plus = new Vector2(0.1f, 0.1f);
        this.transform.localScale = plus;
        movespeed = 4f;
        Destroy(this.gameObject, 10f); //10초뒤에 없어짐
        damage_amount = BossStatus.instance.MagicAttackPower;
        this.transform.SetParent(this.transform.parent.parent);
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
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, true);
            PlayerHit_prefab = Instantiate(PlayerHit, collision.transform.position,Quaternion.identity);
            PlayerHit_prefab.transform.Rotate(-65, 0, 0);
            Destroy(PlayerHit_prefab, 2f);
            Destroy(this.gameObject);
        }

        if (collision.CompareTag("Pillar"))
        {
            StartCoroutine(PillarDestroy(collision));
        }
    }
    IEnumerator PillarDestroy(Collider2D collision)
    {
        Material _mateiral= collision.GetComponent<SpriteRenderer>().material;
        float alpha = 1;
        float break_value = 0;

        SoundManager.instance.Play(11);

        while (true)
        {
            _mateiral.SetFloat("_SpriteFade", alpha);
            _mateiral.SetFloat("Break_Value", break_value);
            yield return new WaitForSeconds(Time.deltaTime);
            alpha -= Time.deltaTime;
            break_value += Time.deltaTime;

            if (alpha <= 0)
                break;
        }
        
        SoundManager.instance.Play(28);
        if (BossStatus.instance != null)
            BossStatus.instance.GetComponent<DesertBossMove_koki>().main.GetComponent<Camera_move>().VivrateForTime(0.7f,0.1f);
        PilllarHit_prefab = Instantiate(PilllarHit, collision.transform.position, Quaternion.identity);
        Player = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < Player.Length; i++)
        {
            Player[i].GetComponent<CharacterStat>().GetDamage(damage_amount * multiple_2, true);
        }

        Destroy(PlayerHit_prefab, 2f);
        Destroy(collision.gameObject,1f);
        Destroy(this.gameObject);
    }

}
