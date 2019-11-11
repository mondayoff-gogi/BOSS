using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_Metor : MonoBehaviour
{   
    public GameObject shadow;
    public GameObject warning;
    Camera _main;

    private GameObject warning_temp;
    private const float movespeed=5;

    Vector3 vector_temp;

    float goal;

    private ParticleSystem ps;
    public GameObject ExploionEffect;
    private GameObject Exploioneffect_temp;
    // Start is called before the first frame update
    void Start()
    {
        _main = Camera.main;
        goal = this.transform.position.y;
        vector_temp = this.transform.position;
        vector_temp.z = -500 + this.transform.position.y;
        vector_temp.y += 15f;
        this.transform.position = vector_temp;

        vector_temp = shadow.transform.position;

        vector_temp.y -= 15f;

        if(UpLoadData.boss_level<2)
        {
            warning_temp = Instantiate(warning, vector_temp, Quaternion.identity);
            ps = warning_temp.GetComponent<ParticleSystem>();
            var simulate = ps.main;
            simulate.simulationSpeed = (float)1 / 6f;
            warning_temp.transform.localScale = this.transform.localScale * 2.3f;
            Destroy(warning_temp, 3f);
        }        
        vector_temp.x -= 15f;

        shadow.transform.position = vector_temp;
        shadow.transform.SetParent(this.transform.parent);


    }

    // Update is called once per frame
    void Update() 
    {
        this.transform.Translate(0, -movespeed * Time.deltaTime, 0,Space.World);
        shadow.transform.Translate(movespeed * Time.deltaTime, 0, 0, Space.World);
        this.transform.Rotate(-Time.deltaTime * 400f, 0, 0);

        if (this.transform.position.y<=goal)
        {
            Explosion();
        }
    }
    void Explosion()
    {
        float radius = this.transform.localScale.x * 0.5f;
        float x, y, z;
        z = 0f;
        float angle = 20f;
        _main.GetComponent<Camera_move>().VivrateForTime(0.3f);

        Exploioneffect_temp = Instantiate(ExploionEffect, this.transform.position, Quaternion.identity);
        Exploioneffect_temp.transform.localScale = this.transform.localScale * 0.7f;
        Exploioneffect_temp.GetComponent<Boss4_MeteoDamage>().GetMulti(Exploioneffect_temp.transform.localScale.x);
        Destroy(Exploioneffect_temp, 1f);
        for (int i = 0; i < 6; i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            angle += (360f / 6f);
            Exploioneffect_temp = Instantiate(ExploionEffect, new Vector3(x + this.gameObject.transform.position.x, y + this.gameObject.transform.position.y, z), Quaternion.identity) as GameObject;
            Exploioneffect_temp.transform.localScale = this.transform.localScale * 0.3f;
            Exploioneffect_temp.GetComponent<Boss4_MeteoDamage>().GetMulti(Exploioneffect_temp.transform.localScale.x);
            Destroy(Exploioneffect_temp, 1f);
        }
        Destroy(shadow);
        Destroy(this.gameObject);
    }
}
