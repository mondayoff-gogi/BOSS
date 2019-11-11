using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerSkill4 : MonoBehaviour
{
    // 150 + 초당 마법 공격력  * 0.01

    private float speed = 1f;
    private float size = 0.01f;
    private float time = 0;

    private float mana_amount = 0;

    private GameObject player;

    public GameObject destroy_effect;
    public GameObject heal_effect;

    private GameObject destroy_temp;
    private GameObject heal_temp;

    public GameObject parent_object;

    //private float multiple = 0.03f;
    private const float multiple2 = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        parent_object = this.transform.parent.gameObject;
        if (this.transform.parent.parent != null)
        {
            player = this.transform.parent.parent.gameObject;
            mana_amount = player.GetComponent<CharacterStat>().MagicAttackPower * 1.5f;
            parent_object.transform.SetParent(player.transform.parent.parent);
        }
        else
        {
            player = GameManage.instance.Character_other[777 + (int)transform.position.z];
            mana_amount = GameManage.instance.Character_other[777 + (int)transform.position.z].GetComponent<CharacterStat>().MagicAttackPower * 1.5f;
        }
        StartCoroutine(MoveOrb());
        StartCoroutine(MakeBigger2());

        // TODO 힐량 계산 공식 수정 요망

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }

    IEnumerator MoveOrb()
    {
        while (true)
        {
            while (speed >= -0.5)
            {
                parent_object.transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
                speed -= 0.008f;
                yield return new WaitForSeconds(0.01f);
            }

            while (speed <= 0.5)
            {
                parent_object.transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
                speed += 0.008f;
                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator MakeBigger2()
    {
        while (time < 15f)
        {
            parent_object.transform.localScale = new Vector3(size, size, size);
            size += 0.02f;
            mana_amount += mana_amount * multiple2;
            yield return new WaitForSeconds(1f);
        }

        yield return 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            destroy_temp = Instantiate(destroy_effect, collision.transform);
            Destroy(destroy_temp, 0.5f);
            Destroy(parent_object);
        }

        if (collision.CompareTag("Player")) //내가 먹으면
        {
            heal_temp = Instantiate(heal_effect, collision.transform);
            float temp = Random.Range(1.0f, 100.0f);

            if (temp <= player.GetComponent<CharacterStat>().ciritical)
            {
                collision.GetComponent<CharacterStat>().GetMana((float)mana_amount + 150,true);
            }
            else
            {
                collision.GetComponent<CharacterStat>().GetMana((float)mana_amount + 150,true);
            }
            
            Destroy(heal_temp, 0.5f);
            Destroy(parent_object);

        }
        if (collision.CompareTag("OtherPlayer"))//남이먹으면
        {
            heal_temp = Instantiate(heal_effect, collision.transform);
            Destroy(heal_temp, 0.5f);
            Destroy(parent_object);
        }
    }
}
