using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.parent.CompareTag("Player")|| this.transform.parent.CompareTag("Boss")|| this.transform.parent.CompareTag("OtherPlayer"))
            spriteRenderer.sprite = this.transform.parent.GetComponentInParent<SpriteRenderer>().sprite;
        else
            spriteRenderer.sprite = null;
    }
}
