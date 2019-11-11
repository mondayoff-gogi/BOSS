using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assasin_Skill : MonoBehaviour
{
    public GameObject slash;

    private WaitForSeconds wait = new WaitForSeconds(0.5f);
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        while (true)
        {
            Instantiate(slash, this.transform);
            yield return wait;
        }
    }
}
