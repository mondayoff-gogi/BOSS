using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Boss5_Flare : MonoBehaviour
{
    private GameObject[] background;
    public Material default_material;
    public Material diffuse;
    private GameObject boss;
    private GameObject[] objects;
    private GameObject[] particle_objects;
    // Start is called before the first frame update
    void Start()
    {
        background = GameObject.FindGameObjectsWithTag("WaterBackGround");
        Color color = new Color(0.1f, 0.1f, 0.1f, 1f);

        for(int i=0;i<background.Length;i++)
        {
            background[i].GetComponent<SpriteRenderer>().color = color;
        }
        boss = BossStatus.instance.gameObject;
        boss.GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(ChangeMaterial());
        objects = GameObject.FindGameObjectsWithTag("Object");
        if (objects.Length > 0)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    IEnumerator ChangeMaterial()
    {
        boss.GetComponent<SpriteRenderer>().material = diffuse;
        for (int i = 0; i < background.Length; i++)
        {
            background[i].GetComponent<SpriteRenderer>().material = diffuse;
        }
        yield return new WaitForSeconds(10f);
        for (int i = 0; i < background.Length; i++)
        {
            background[i].GetComponent<SpriteRenderer>().material = default_material;
        }
        boss.GetComponent<SpriteRenderer>().material = default_material;
        Color color = new Color(1, 1, 1, 1);
        for (int i = 0; i < background.Length; i++)
        {
            background[i].GetComponent<SpriteRenderer>().color = color;
        }

        boss.GetComponent<SpriteRenderer>().enabled = true;

        objects = GameObject.FindGameObjectsWithTag("Object");
        for (int i=0;i<objects.Length;i++)
        {
            objects[i].GetComponent<SpriteRenderer>().enabled = true;
        }

        Destroy(this.gameObject);
        yield return 0;
        
    }
}
