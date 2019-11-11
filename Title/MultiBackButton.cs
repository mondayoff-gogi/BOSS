using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiBackButton : MonoBehaviour
{
    // Start is called before the first frame update
    private void Update()
    {
        if (NetworkManager.instance.is_multi)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);

        }
    }
}
