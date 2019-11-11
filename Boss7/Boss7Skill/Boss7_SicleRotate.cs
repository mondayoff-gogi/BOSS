using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7_SicleRotate : MonoBehaviour
{
    public GameObject hit_effect;
    public GameObject blood;

    private GameObject hit_temp;
    private GameObject blood_temp;
    private float rotate = 0;
    private float damage_amount = 0;
    private Vector2 dir;
    private float speed = 20f;
    // Start is called before the first frame update
    void Start()
    {
        damage_amount = BossStatus.instance.PhysicalAttackPower;
        Destroy(this.gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        speed -= 0.3f;
        rotate -= Time.deltaTime * 1000;
        if(rotate <= -180)
        {
            rotate *= -1;
        }
        this.transform.localRotation = Quaternion.Euler(0, 0, rotate);
        this.transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(GiveDamage(collision));
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
        }
    }


    IEnumerator GiveDamage(Collider2D collision)
    {
        while (true)
        {
            float temp = Random.Range(1.0f, 100.0f);
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * 2f, true);
            hit_temp = Instantiate(hit_effect, collision.transform);
            Destroy(hit_temp, 1f);
            if (temp <= 20)
                blood_temp = Instantiate(blood, collision.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);

            if (collision.CompareTag("DeadPlayer"))
            {
                break;
            }
        }
    }

    public void GetDir(Vector2 player)
    {
        dir = player - (Vector2)this.transform.position;
        dir.Normalize();
    }
}
