using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizantalExplosion : MonoBehaviour
{
    public GameObject explosion;

    private GameObject explosion_temp;
    private Vector2 dir = new Vector2(0, 0);
    private Vector2 this_position;

    void Start()
    {
        this_position = this.transform.position;
        this_position.x += -12f;
        StartCoroutine(GenerateBombs());
    }

    IEnumerator GenerateBombs()
    {
        for (int i = 0; i < 9; i++)
        {
            explosion_temp = Instantiate(explosion, this_position, Quaternion.Euler(0, 180, 180));
            Destroy(explosion_temp, 1f);
            this_position.x += 4f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
