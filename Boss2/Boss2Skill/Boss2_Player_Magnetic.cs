using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boss2_Player_Magnetic : MonoBehaviour
{
    public bool magnetic = false; // false == N, true == S
    public GameObject N;
    public GameObject S;
    public GameObject Image;

    public void GetMagnetic(bool mag)
    {
        magnetic = mag;

        if (mag)
        {
            Image = S;
        }
        else
            Image = N;
    }
}
