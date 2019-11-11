using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7_SpinBat : MonoBehaviour
{
    public GameObject bat;
    private GameObject[] third_bats;
    private float x, y;
    private float third_angle = 100f;
    private float third_radius = 25f;
    private float third;
    // Start is called before the first frame update
    void Start()
    {

        third_bats = new GameObject[40];
        third = Random.Range(0, 14);

        for (int i = 0; i < third_bats.Length; i++)
        {
            if (i >= third && i < (third + (30/(UpLoadData.boss_level+3))))
            {
                third_angle += (360f / 40f);
                continue;
            }
            if (i >= (third+16) && i < (third + 16 + (30 / (UpLoadData.boss_level + 3))))
            {
                third_angle += (360f / 40f);
                continue;
            }
            x = Mathf.Cos(Mathf.Deg2Rad * third_angle) * third_radius + this.transform.position.x;
            y = Mathf.Sin(Mathf.Deg2Rad * third_angle) * third_radius+ this.transform.position.y;
            third_angle += (360f / 40f);
            third_bats[i] = Instantiate(bat, new Vector3(x, y,-15f), Quaternion.identity);
        }

    }
}
