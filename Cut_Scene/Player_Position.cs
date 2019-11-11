using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Position : MonoBehaviour
{
    public float DirX;
    public float DirY;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector2(DirX, DirY);

    }

}
