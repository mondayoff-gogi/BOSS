using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChainEffect : MonoBehaviour
{
    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChainIconEffect());
    }

    IEnumerator ChainIconEffect()
    {
        while (true)
        {
            Color color = this.GetComponent<SpriteRenderer>().color;
            while (color.a < 1)
            {
                color.a += 0.03f;
                this.GetComponent<SpriteRenderer>().color = color;
                yield return waitTime;
            }
            while (color.a > 0.5f)
            {
                color.a -= 0.03f;
                this.GetComponent<SpriteRenderer>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);

        }
    }
}
