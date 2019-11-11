using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Skill_0 : MonoBehaviour
{
    [SerializeField] [Range(0f, 10f)] private float speed = 1;
    [SerializeField] [Range(0f, 10f)] private float radius = -1;
    public GameObject parent_parent_obj;
    public GameObject parent_obj;
    public float runningTime = 0;
    private Vector2 newPos = new Vector2();
    private float time = 0;

    public GameObject destroy_obj;
    private GameObject destroy_temp;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        time += Time.deltaTime;
        if(time < 3)
        {
            radius += Time.deltaTime * 0.8f;
            speed += Time.deltaTime * 0.3f;
            runningTime += Time.deltaTime * speed;
            float x = radius * Mathf.Cos(runningTime);
            float y = radius * Mathf.Sin(runningTime);
            newPos = new Vector2(parent_parent_obj.transform.position.x + x, parent_parent_obj.transform.position.y + y);
            this.transform.position = newPos;
            if (speed > 3)
            {
                speed = 5;
            }

            if (radius > 3)
            {
                radius = 3;
            }
        }
        else if(time < 5)
        {
            radius += Time.deltaTime * 1f;
            runningTime += Time.deltaTime * speed;
            float x = radius * Mathf.Cos(runningTime);
            float y = radius * Mathf.Sin(runningTime);
            newPos = new Vector2(parent_obj.transform.position.x + x, parent_obj.transform.position.y + y);
            this.transform.position = newPos;
        }
        else
        {
            this.transform.position = this.transform.position;
            destroy_temp = Instantiate(destroy_obj, this.transform);
            Destroy(this.gameObject, 1f);
        }

    }
}
