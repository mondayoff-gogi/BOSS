using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Slash : MonoBehaviour
{
    public GameObject damage_effect;

    private GameObject damage_temp;
    private float speed = 8f;
    private float damage_amount;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 5f);
        damage_amount = BossStatus.instance.PhysicalAttackPower;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            damage_temp = Instantiate(damage_effect, collision.transform);
            Destroy(damage_temp, 0.5f);
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * 0.3f, false);
            Destroy(this.gameObject);
        }
    }
}
