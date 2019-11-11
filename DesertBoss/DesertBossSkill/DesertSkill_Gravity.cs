using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSkill_Gravity : MonoBehaviour
{
    private Vector2 temp;
    // Start is called before the first frame update
    private void Start()
    {
        Destroy(this.gameObject, 3f);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            temp= BossStatus.instance.transform.position - collision.transform.position;
            temp.Normalize();
            collision.transform.Translate(temp/20); //얼마나 빠르게 끌려들어오는지
        }
    }
}
