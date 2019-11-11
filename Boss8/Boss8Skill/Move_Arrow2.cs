using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Arrow2 : MonoBehaviour
{
    private Move_Arrow[] arrows;
    private Vector2 dir = Vector2.right;
    private float speed = 20f;
    // Start is called before the first frame update
    void Start()
    {
        arrows = this.GetComponentsInChildren<Move_Arrow>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] != null)
                arrows[i].gameObject.transform.Translate(dir * speed * Time.deltaTime);
            else
                continue;
        }
    }
}
