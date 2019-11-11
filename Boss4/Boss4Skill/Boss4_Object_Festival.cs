using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_Object_Festival : MonoBehaviour
{
    private float speed = 0.5f;
    public Vector2 dir;
    private float dir_x;
    private float dir_y;
    private CircleCollider2D _col;
    private int count = 1;
    private float attack_amount;
    public GameObject hit_effect;
    private GameObject hit_temp;
    private float _scale;
    private Rigidbody2D _rig2d;

    // Start is called before the first frame update
    void Start()
    {
        _rig2d = this.GetComponent<Rigidbody2D>();
        attack_amount = BossStatus.instance.PhysicalAttackPower;
        _col = this.GetComponent<CircleCollider2D>();
        dir_x = Random.Range(-1.0f, 1.0f);
        dir_y = Random.Range(-1.0f, 1.0f);
        dir = new Vector2(dir_x, dir_y);
        dir.Normalize();
        _rig2d.AddForce(dir * speed, ForceMode2D.Impulse);
        _scale = Random.Range(1f, 2f);
        this.transform.localScale = new Vector3(_scale, _scale, _scale);
        Destroy(this.gameObject, 7f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Object"))
        {
            _rig2d.AddForce(collision.gameObject.GetComponent<Boss4_Object_Festival>().dir * 5f, ForceMode2D.Force);
            //Vector2 temp = dir;
            //dir += collision.gameObject.GetComponent<Boss4_Object_Festival>().dir;
            //dir.Normalize();
            //count++;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<CharacterStat>().GetDamage(count * attack_amount, false);
            hit_temp = Instantiate(hit_effect, collision.transform);
            Destroy(hit_temp, 0.5f);
            Destroy(this.gameObject);
        }
    }

}
