using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    static public FloatingText instance;

    public bool is_discription;

    [HideInInspector]
    public string text;

    private float moveSpeed = 30.0f;
    private float destroyTime = 1.2f;

    private Vector3 position;
    private Color temp_color;

    private void Start()
    {
        if (is_discription)
        {
            destroyTime = 3f;
            moveSpeed = 0f;
        }
        this.GetComponent<UILabel>().text = text;
        temp_color = this.GetComponent<UILabel>().gradientTop;
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.position.Set(this.transform.position.x, this.transform.position.y + (moveSpeed * Time.deltaTime), this.transform.position.z);


        destroyTime -= Time.deltaTime;

        temp_color.a -= Time.deltaTime/destroyTime;

        this.GetComponent<UILabel>().gradientTop = temp_color;
        this.GetComponent<UILabel>().gradientBottom = temp_color;

        if (destroyTime <= 0)
            Destroy(this.gameObject);
    }
}
