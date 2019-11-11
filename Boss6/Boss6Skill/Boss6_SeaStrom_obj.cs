using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_SeaStrom_obj : MonoBehaviour
{
    private Vector2 dir;
    private float speed = 15f;
    private float damage_amount;
    private float multiple;

    public GameObject destroy_effect;
    private GameObject destroy_temp;
    // Start is called before the first frame update
    void Start()
    {
        dir = Vector2.zero - (Vector2)this.transform.position;
        dir.Normalize();
        damage_amount = BossStatus.instance.MagicAttackPower;
        if (UpLoadData.boss_level < 2)
            multiple = 2f;
        else
            multiple = 4f;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir * (speed / Vector2.Distance(this.transform.position, Vector2.zero)) * Time.deltaTime);

        if(Vector2.Distance(this.transform.position, Vector2.zero) < 0.5f)
        {
            destroy_temp = Instantiate(destroy_effect, new Vector3(this.transform.position.x, this.transform.position.y, -10), Quaternion.identity);
            Destroy(destroy_temp, 0.5f);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, true);
            destroy_temp = Instantiate(destroy_effect, new Vector3(this.transform.position.x, this.transform.position.y, -10), Quaternion.identity);
            Destroy(destroy_temp, 0.5f);
            Destroy(this.gameObject);
        }
    }
}
