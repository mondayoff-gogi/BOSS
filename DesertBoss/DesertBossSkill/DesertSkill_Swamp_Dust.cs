using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesertSkill_Swamp_Dust : MonoBehaviour
{
    public float speed;
    public float temp;

    private Vector3 swamp_pos;
    private float rgb = 255;
    private SpriteRenderer dust_color;


    private Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
        target = this.transform.position;
        target.z = -500;
        this.transform.position = target;
        swamp_pos = this.gameObject.transform.parent.GetComponent<Transform>().position;
        Destroy(this.gameObject, 4f);
        dust_color = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        rgb -= temp;

        target = swamp_pos - this.gameObject.transform.position;
        target.z = 1;

        target.Normalize();

        dust_color.color = new Color(rgb/255, rgb/255, rgb/255);

        this.transform.Translate(target * speed * Time.deltaTime);

    }
}
