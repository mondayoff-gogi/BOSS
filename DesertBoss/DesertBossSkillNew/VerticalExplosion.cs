using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalExplosion : MonoBehaviour
{
    public GameObject explosion;

    private GameObject explosion_temp;
    private Vector2 dir = new Vector2(0,0);
    private Vector2 this_position;
    // Start is called before the first frame update
    void Start()
    {
        this_position = this.transform.position;
        this_position.y += 10f;
        StartCoroutine(GenerateBombs());
    }


    IEnumerator GenerateBombs()
    {
        for(int i = 0; i < 7; i++)
        {
            explosion_temp = Instantiate(explosion, this_position, Quaternion.Euler(0,180,180));
            Destroy(explosion_temp, 1f);
            this_position.y -= 4f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
