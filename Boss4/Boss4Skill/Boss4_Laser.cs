using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_Laser : MonoBehaviour
{
    public GameObject LaserObject;
    private GameObject LaserObject_temp;

    public GameObject StraightWarning;
    private GameObject StraightWarning_temp;

    private GameObject[] player;

    private int rand_num;

    private float timer;

    void Start()
    {
        StraightWarning_temp = Instantiate(StraightWarning, this.transform);

        StraightWarning_temp.transform.localScale = new Vector2(50f, 0.5f);

        player = GameObject.FindGameObjectsWithTag("Player");
        if (player.Length <= 0)
            Destroy(this.gameObject);
        rand_num = Random.Range(0, player.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if (player[rand_num].transform.position.x - this.transform.position.x > 0)
            StraightWarning_temp.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan((player[rand_num].transform.position.y - this.transform.position.y) / (player[rand_num].transform.position.x - this.transform.position.x)));
        else
            StraightWarning_temp.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan((player[rand_num].transform.position.y - this.transform.position.y) / (player[rand_num].transform.position.x - this.transform.position.x)));
            
        
        timer += Time.deltaTime;
        
         
        if(timer>3f)//발사
        {
            LaserObject_temp = Instantiate(LaserObject, this.transform);

            LaserObject_temp.transform.SetParent(this.transform.parent);
            LaserObject_temp.transform.rotation = StraightWarning_temp.transform.rotation;
            Destroy(StraightWarning_temp);
            Destroy(this.gameObject);
        }
        
    }
}
