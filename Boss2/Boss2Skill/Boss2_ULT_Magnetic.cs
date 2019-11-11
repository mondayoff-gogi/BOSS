using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_ULT_Magnetic : MonoBehaviour
{
    private bool magnetic = true; // false == N, true == S
    // Start is called before the first frame update
    private Vector2 dir;
    private GameObject obj;
    private float speed;

    public GameObject N;
    public GameObject S;
    public GameObject Image;


    Vector2 vec2_temp;
    Vector2 break_vec;
    private void Start()
    {
        speed = 0.6f;
        break_vec = new Vector2(0.9f, 0.9f);
        dir = new Vector2(0, 0.1f);
        obj = this.transform.parent.gameObject;
    }
    private void Update()
    {
        obj.transform.Translate(dir * Time.deltaTime);
        dir *= break_vec;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.GetComponent<Boss2_Player_Magnetic>() == null)
        {
            return;

        }
        else
        {
            if (collision.CompareTag("BossAttack") && (collision.GetComponent<Boss2_Player_Magnetic>().magnetic == magnetic))
            {
                vec2_temp = obj.transform.position - collision.transform.position;
                vec2_temp.Normalize();
                vec2_temp *= 2 / Vector2.Distance(obj.transform.position, collision.transform.position);
                vec2_temp *= speed;
                dir += vec2_temp;
            }

            if (collision.CompareTag("BossAttack") && (collision.GetComponent<Boss2_Player_Magnetic>().magnetic != magnetic))
            {
                vec2_temp = collision.transform.position - obj.transform.position;
                vec2_temp.Normalize();
                vec2_temp *= 2 / Vector2.Distance(obj.transform.position, collision.transform.position);
                vec2_temp *= speed;
                dir += vec2_temp;
            }
        }
    }

    public void GetMagnetic(bool mag)
    {
        magnetic = mag;
        if (magnetic)
        {
            Image.GetComponent<SpriteRenderer>().sprite = S.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            Image.GetComponent<SpriteRenderer>().sprite = N.GetComponent<SpriteRenderer>().sprite;
        }
    }
}
