using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_0_1 : MonoBehaviour
{
    private float time;
    public float dirX = 0;
    public float dirY = 0;
    private Vector2 temp;
    // Start is called before the first frame update
    void Start()
    {
        temp = new Vector2(dirX, dirY);   
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time > 3)
        {
            this.transform.Translate(temp * 1f * Time.deltaTime);
        }
        
    }
}
