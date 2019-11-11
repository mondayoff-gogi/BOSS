using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScene : MonoBehaviour
{
    private float waittime;
    WaitForSeconds timer;

    // Start is called before the first frame update
    void Start()
    {
        waittime = 0.01f;
        timer = new WaitForSeconds(waittime);
        StartCoroutine(CutScenegogo());
    }
    IEnumerator CutScenegogo()
    {
        yield return new WaitForSeconds(0.4f);
        while(true)
        {
            this.transform.Translate(0.5f, 0, 0);

            if (this.transform.position.x > 2000)
                break;
            yield return timer;
        }
    }
}
