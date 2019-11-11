using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_Flare_Light : MonoBehaviour
{
    private GameObject target;
    private Vector3 temp;

    // Update is called once per frame
    void Update()
    {
        temp = new Vector3(target.transform.position.x, target.transform.position.y, -11f);
        this.transform.position = temp;
        Destroy(this.gameObject, 10f);
    }

    public void GetTartget(GameObject player)
    {
        target = player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            collision.GetComponent<SpriteRenderer>().enabled = true;
        }

        if (collision.CompareTag("Object"))
        {
            collision.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            collision.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (collision.CompareTag("Object"))
        {
            collision.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
