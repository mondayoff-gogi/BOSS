using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Devine : MonoBehaviour
{
    private float speed = 2f;
    private float dir_x = -5;

    public GameObject child;

    private float amount = 10f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveUp());
        Destroy(this.gameObject, amount);
    }

    IEnumerator MoveUp()
    {
        while (this.transform.localPosition.y < 0)
        {
            this.transform.Translate(Vector3.up * speed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(child);
        while (true)
        {
            this.transform.Rotate(dir_x, 0, 0, Space.Self);

            yield return new WaitForSeconds(0.01f);
        }
    }
}
