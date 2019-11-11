using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_PileSprite : MonoBehaviour
{
    private Vector2 pos;
    public GameObject explosion;
    private Vector2 dir;
    private const float speed = 5f;
    private Vector2 ori;
    // Start is called before the first frame update
    void Start()
    {
        dir = new Vector2(0, -1);
        ori = new Vector2(0, 0);
        pos = this.transform.position;
        pos.y += 20f;
        this.transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(this.transform.position, ori)<0.3f)
        {
            explosion.SetActive(true);
        }
        else
            this.transform.Translate(dir*Time.deltaTime* speed, Space.World);
    }
}
